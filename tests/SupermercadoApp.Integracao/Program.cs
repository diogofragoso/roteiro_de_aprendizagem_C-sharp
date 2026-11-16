using Npgsql;
using SupermercadoApp.Models;
using SupermercadoApp.Repositories;
using SupermercadoApp.Services;

// Opt-in: não usa SUPERMERCADO_DB. Cada execução cria um esquema próprio.
if (!args.Contains("--executar") || string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SUPERMERCADO_TEST_DB")))
{
    Console.WriteLine("PENDENTE: configure SUPERMERCADO_TEST_DB em banco didático e use --executar. Nenhuma conexão aberta.");
    return 2;
}
string esquema = "lab_" + Guid.NewGuid().ToString("N");
var configuracao = new NpgsqlConnectionStringBuilder(Environment.GetEnvironmentVariable("SUPERMERCADO_TEST_DB"))
{
    SslMode = SslMode.VerifyFull
};
await using var controle = NpgsqlDataSource.Create(configuracao.ConnectionString);
bool criado = false;
int aprovados = 0;
try
{
    // Identificador gerado internamente, nunca proveniente de entrada externa.
    await using (var criar = controle.CreateCommand($"CREATE SCHEMA {esquema}")) await criar.ExecuteNonQueryAsync();
    criado = true;
    Console.WriteLine($"Esquema exclusivo desta execução: {esquema}");
    configuracao.SearchPath = esquema;
    await using var banco = NpgsqlDataSource.Create(configuracao.ConnectionString);
    foreach (string arquivo in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "database"), "00*.sql").Order())
    {
        await using var cmd = banco.CreateCommand(await File.ReadAllTextAsync(arquivo));
        await cmd.ExecuteNonQueryAsync();
    }
    var usuarios = new AutenticacaoService(new UsuarioRepository(banco));
    string senha = "laboratorio-" + Guid.NewGuid().ToString("N");
    await usuarios.PrepararAdministradorAsync("admin", senha);
    Sessao admin = (await usuarios.EntrarAsync("admin", senha))!;
    await usuarios.CriarAsync(admin, "caixa.um", senha, Perfil.Caixa);
    await usuarios.CriarAsync(admin, "caixa.dois", senha, Perfil.Caixa);
    Sessao caixa1 = (await usuarios.EntrarAsync("caixa.um", senha))!;
    Sessao caixa2 = (await usuarios.EntrarAsync("caixa.dois", senha))!;
    var repositorioProdutos = new ProdutoRepository(banco);
    var produtos = new ProdutoService(repositorioProdutos);
    var vendas = new VendaService(new VendaRepository(banco), repositorioProdutos);

    async Task<int> Numero(string sql)
    {
        await using var cmd = banco.CreateCommand(sql);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }
    void Conferir(bool resultado, string caso)
    {
        if (!resultado) throw new InvalidOperationException(caso);
        aprovados++; Console.WriteLine($"OK: {caso}");
    }
    async Task<Produto> Preparar(string codigo, int saldo)
    {
        await produtos.SalvarAsync(admin, 0, codigo, "Produto " + codigo, 8.50m);
        var produto = (await repositorioProdutos.BuscarCodigoAsync(codigo))!;
        await produtos.ReporAsync(admin, produto.Id, saldo);
        return (await repositorioProdutos.BuscarCodigoAsync(codigo))!;
    }
    static ItemCarrinho Item(Produto p) => new(p.Id, p.Nome, p.Preco, 1);
    async Task<(int Vendas, int Itens, int Movimentos, int Saldo)> Estado()
        => (await Numero("SELECT count(*) FROM venda"), await Numero("SELECT count(*) FROM venda_item"),
            await Numero("SELECT count(*) FROM movimento_estoque"), await Numero("SELECT coalesce(sum(estoque),0) FROM produto"));

    var ultimo = await Preparar("ULTIMO", 1);
    async Task<bool> Tentar(Sessao sessao)
    {
        try { await vendas.FinalizarAsync(sessao, Guid.NewGuid(), [Item(ultimo)], 10); return true; }
        catch (InvalidOperationException) { return false; }
    }
    var resultados = await Task.WhenAll(Tentar(caixa1), Tentar(caixa2));
    Conferir(resultados.Count(x => x) == 1 && await Numero("SELECT estoque FROM produto WHERE codigo='ULTIMO'") == 0,
        "Dois caixas, um último item, uma confirmação");
    Conferir(await Numero("SELECT count(*) FROM venda") == 1 && await Numero("SELECT count(*) FROM movimento_estoque WHERE motivo='Venda'") == 1,
        "Disputa não deixa cabeçalho órfão nem baixa duplicada");

    var repeticao = await Preparar("REPETICAO", 5);
    Guid chave = Guid.NewGuid();
    var repetidas = await Task.WhenAll(
        vendas.FinalizarAsync(caixa1, chave, [Item(repeticao)], 10),
        vendas.FinalizarAsync(caixa1, chave, [Item(repeticao)], 10));
    Conferir(repetidas[0].Id == repetidas[1].Id
        && await Numero("SELECT estoque FROM produto WHERE codigo='REPETICAO'") == 4,
        "Mesma chave concorrente retorna mesmo ID e baixa uma vez");

    var rollback = await Preparar("ROLLBACK", 5);
    var antes = await Estado();
    await using (var falha = banco.CreateCommand("""
        CREATE FUNCTION falhar_movimento() RETURNS trigger LANGUAGE plpgsql AS $$
        BEGIN IF NEW.motivo='Venda' THEN RAISE EXCEPTION 'Falha controlada'; END IF; RETURN NEW; END $$;
        CREATE TRIGGER falha_laboratorio BEFORE INSERT ON movimento_estoque
        FOR EACH ROW EXECUTE FUNCTION falhar_movimento();
        """)) await falha.ExecuteNonQueryAsync();
    Guid operacao = Guid.NewGuid();
    bool falhou = false;
    try { await vendas.FinalizarAsync(caixa1, operacao, [Item(rollback)], 10); }
    catch (PostgresException e) when (e.SqlState == "P0001") { falhou = true; }
    finally
    {
        await using var remover = banco.CreateCommand("DROP TRIGGER falha_laboratorio ON movimento_estoque; DROP FUNCTION falhar_movimento();");
        await remover.ExecuteNonQueryAsync();
    }
    Conferir(falhou && await Estado() == antes, "Rollback após escrita preserva cabeçalhos, itens, movimentos e saldo");
    var recuperada = await vendas.FinalizarAsync(caixa1, operacao, [Item(rollback)], 10);
    var consultada = await vendas.FinalizarAsync(caixa1, operacao, [Item(rollback)], 10);
    Conferir(recuperada.Id == consultada.Id && await Numero("SELECT estoque FROM produto WHERE codigo='ROLLBACK'") == 4,
        "Após retirar falha, mesma operação confirma uma vez");

    await produtos.SalvarAsync(admin, rollback.Id, rollback.Codigo, "Nome alterado", 9);
    await using (var historico = banco.CreateCommand("SELECT nome, preco_unitario FROM venda_item WHERE produto_id=@id"))
    {
        historico.Parameters.AddWithValue("id", rollback.Id);
        await using var r = await historico.ExecuteReaderAsync();
        Conferir(await r.ReadAsync() && r.GetString(0) == rollback.Nome && r.GetDecimal(1) == 8.50m,
            "Histórico preserva nome e preço anteriores");
    }

    // Quatro vendas geradas pelo serviço, posicionadas em fronteiras fixas.
    var dataProduto = await Preparar("DATAS", 10);
    DateTime inicio = new(2026, 10, 16, 3, 0, 0, DateTimeKind.Utc);
    DateTime fim = inicio.AddDays(1);
    foreach (var instante in new[] { inicio.AddTicks(-10), inicio, fim.AddTicks(-10), fim })
    {
        var venda = await vendas.FinalizarAsync(caixa1, Guid.NewGuid(), [Item(dataProduto)], 10);
        await using var ajustar = banco.CreateCommand("UPDATE venda SET criado_em=@instante WHERE id=@id");
        ajustar.Parameters.AddWithValue("instante", instante);
        ajustar.Parameters.AddWithValue("id", venda.Id);
        await ajustar.ExecuteNonQueryAsync();
    }
    // Demais vendas desta execução ficam fora do intervalo da massa.
    await using (var mover = banco.CreateCommand("""
        UPDATE venda SET criado_em=TIMESTAMPTZ '2026-01-01 00:00:00Z'
        WHERE id NOT IN (SELECT venda_id FROM venda_item WHERE produto_id=@produto)
        """)) { mover.Parameters.AddWithValue("produto", dataProduto.Id); await mover.ExecuteNonQueryAsync(); }
    var relatorio = await vendas.RelatorioAsync(admin, inicio, fim);
    Conferir(relatorio.Count == 2 && relatorio.Sum(v => v.Total) == 17m, "Relatório inclui início e último microssegundo, exclui fim");
    Console.WriteLine($"{aprovados} verificações PostgreSQL aprovadas no esquema isolado.");
    return 0;
}
catch (Exception e)
{
    Console.Error.WriteLine($"FALHOU: {e.GetType().Name}. Investigue no depurador sem publicar credenciais.");
    return 1;
}
finally
{
    if (criado)
    {
        // Remove somente o esquema único criado por esta execução.
        await using var limpar = controle.CreateCommand($"DROP SCHEMA {esquema} CASCADE");
        await limpar.ExecuteNonQueryAsync();
    }
}
