using SupermercadoApp.Models;
using SupermercadoApp.Repositories;
using SupermercadoApp.Services;
using SupermercadoApp.Views;

namespace SupermercadoApp.Verificacoes;

internal static class Program
{
    private static readonly Sessao Admin = new(1, "admin", Perfil.Administrador);
    private static readonly Sessao Caixa = new(2, "caixa", Perfil.Caixa);
    private static int aprovados;

    [STAThread]
    private static int Main()
    {
        try
        {
            RegrasAsync().GetAwaiter().GetResult();
            Interfaces();
            Console.WriteLine($"{aprovados} verificações aprovadas. Sem conexão com PostgreSQL/Neon.");
            return 0;
        }
        catch (Exception e) { Console.Error.WriteLine($"FALHA: {e.Message}"); return 1; }
    }

    private static void Conferir(bool condicao, string caso)
    {
        if (!condicao) throw new Exception(caso);
        aprovados++;
        Console.WriteLine($"OK: {caso}");
    }
    private static async Task RecusarAsync<T>(Func<Task> acao, string caso) where T : Exception
    {
        try { await acao(); }
        catch (T) { Conferir(true, caso); return; }
        throw new Exception($"Não recusou: {caso}");
    }
    private static Task Executar(Action acao) { acao(); return Task.CompletedTask; }

    private static async Task RegrasAsync()
    {
        var arroz = new Produto(1, "000123", "Arroz", 25.90m, 10, true);
        var feijao = new Produto(2, "000456", "Feijão", 8.50m, 10, true);
        var carrinho = new Carrinho();
        carrinho.Adicionar(arroz, 1); carrinho.Adicionar(arroz, 1); carrinho.Adicionar(feijao, 1);
        Conferir(carrinho.Itens.Count == 2 && carrinho.Total == 60.30m, "Agrupa produtos e calcula R$ 60,30");
        await RecusarAsync<InvalidOperationException>(() => Executar(() => carrinho.Adicionar(arroz, 9)), "Quantidade acumulada excede saldo");
        Conferir(carrinho.Total == 60.30m, "Recusa preserva carrinho anterior");
        await RecusarAsync<ArgumentException>(() => Executar(() => carrinho.Adicionar(arroz with { Ativo = false }, 1)), "Produto inativo");
        await RecusarAsync<ArgumentException>(() => Executar(() => carrinho.Adicionar(arroz, 0)), "Quantidade zero");
        carrinho.Remover(feijao.Id);
        Conferir(carrinho.Total == 51.80m, "Remoção recalcula total");
        carrinho.Limpar(); Conferir(carrinho.Itens.Count == 0 && carrinho.Total == 0, "Cancelamento limpa carrinho");

        var produtos = new ProdutosFake();
        var cadastro = new ProdutoService(produtos);
        await RecusarAsync<UnauthorizedAccessException>(() => cadastro.SalvarAsync(Caixa, 0, "1", "Arroz", 2), "Caixa não cadastra produto");
        await RecusarAsync<ArgumentException>(() => cadastro.SalvarAsync(Admin, 0, "1", "Arroz", 0), "Preço zero");
        await RecusarAsync<ArgumentException>(() => cadastro.SalvarAsync(Admin, 0, "1", "Arroz", 1.999m), "Preço com três casas");
        await RecusarAsync<ArgumentException>(() => cadastro.SalvarAsync(Admin, 0, " ", "Arroz", 2), "Código vazio");
        Conferir(produtos.Gravacoes == 0, "Falhas de validação/autorização não chegam ao repositório");
        await cadastro.SalvarAsync(Admin, 0, " 000123 ", " Arroz ", 25.90m);
        Conferir(produtos.UltimoCodigo == "000123" && produtos.UltimoNome == "Arroz", "Normaliza sem perder zeros");
        await RecusarAsync<ArgumentException>(() => cadastro.ReporAsync(Admin, 1, -1), "Reposição negativa");
        await RecusarAsync<UnauthorizedAccessException>(() => cadastro.ReporAsync(Caixa, 1, 1), "Caixa não repõe estoque");
        await cadastro.ReporAsync(Admin, 1, 3);
        Conferir(produtos.UsuarioReposicao == Admin.Id, "Responsável vem da sessão");

        foreach (decimal precoInvalido in new[] { -1m, 100000000m })
            await RecusarAsync<ArgumentException>(() => cadastro.SalvarAsync(Admin, 0, "1", "Arroz", precoInvalido), "Fronteira de preço inválido");
        foreach (decimal precoValido in new[] { 0.01m, 99999999.99m })
            await cadastro.SalvarAsync(Admin, 0, "1", "Arroz", precoValido);
        Conferir(produtos.Gravacoes == 3, "Mínimo e máximo monetários chegam ao repositório");
        int antes = produtos.Gravacoes;
        await RecusarAsync<ArgumentException>(() => cadastro.SalvarAsync(Admin, -1, "1", "Arroz", 1), "ID negativo");
        await RecusarAsync<ArgumentException>(() => cadastro.SalvarAsync(Admin, 0, new string('a', 31), "Arroz", 1), "Código acima do limite");
        await RecusarAsync<ArgumentException>(() => cadastro.SalvarAsync(Admin, 0, "1", "A", 1), "Nome abaixo do limite");
        await RecusarAsync<ArgumentException>(() => cadastro.SalvarAsync(Admin, 0, "1", new string('a', 121), 1), "Nome acima do limite");
        Conferir(produtos.Gravacoes == antes, "Recusas de fronteira preservam contagem de gravações");
        await cadastro.SalvarAsync(Admin, 0, new string('a', 30), new string('b', 120), 1);
        Conferir(produtos.Gravacoes == antes + 1, "Comprimentos máximos são aceitos");

        var usuarios = new UsuariosFake();
        var autenticacao = new AutenticacaoService(usuarios);
        const string senhaFicticia = "frase de laboratório"; // Somente fake em memória; não é conta do banco.
        await autenticacao.PrepararAdministradorAsync(" ADMIN ", senhaFicticia);
        var primeiroHash = usuarios.Dados[0].SenhaHash;
        Conferir(primeiroHash != senhaFicticia && primeiroHash.Length > 40, "Cadastro guarda hash");
        var sessao = await autenticacao.EntrarAsync("ADMIN", senhaFicticia);
        Conferir(sessao?.Username == "admin" && sessao.Perfil == Perfil.Administrador, "Login normalizado com senha correta");
        Conferir(await autenticacao.EntrarAsync("admin", "senha incorreta") is null, "Senha errada");
        Conferir(await autenticacao.EntrarAsync("ausente", senhaFicticia) is null, "Usuário inexistente");
        await RecusarAsync<InvalidOperationException>(() => autenticacao.PrepararAdministradorAsync("outro", senhaFicticia), "Preparação recusada com usuários existentes");
        await RecusarAsync<UnauthorizedAccessException>(() => autenticacao.CriarAsync(Caixa, "outro", senhaFicticia, Perfil.Administrador), "Caixa não cria administrador");
        await RecusarAsync<ArgumentException>(() => autenticacao.CriarAsync(Admin, "outro", "curta", Perfil.Caixa), "Senha curta");
        await autenticacao.CriarAsync(Admin, "caixa", senhaFicticia, Perfil.Caixa);
        Conferir(usuarios.Dados[1].SenhaHash != primeiroHash, "Salt gera hashes diferentes para a mesma senha");

        await RecusarAsync<ArgumentException>(() => autenticacao.CriarAsync(Admin, "ab", senhaFicticia, Perfil.Caixa), "Username com dois caracteres");
        await RecusarAsync<ArgumentException>(() => autenticacao.CriarAsync(Admin, new string('a', 51), senhaFicticia, Perfil.Caixa), "Username com 51 caracteres");
        await RecusarAsync<ArgumentException>(() => autenticacao.CriarAsync(Admin, "acentuação", senhaFicticia, Perfil.Caixa), "Username com acentos");
        await RecusarAsync<ArgumentException>(() => autenticacao.CriarAsync(Admin, "outro", senhaFicticia, (Perfil)999), "Enum inválido no serviço");
        await RecusarAsync<ArgumentException>(() => autenticacao.CriarAsync(Admin, "outro", new string('s', 129), Perfil.Caixa), "Senha com 129 caracteres");
        await autenticacao.CriarAsync(Admin, "espacos", "  senha com espacos  ", Perfil.Caixa);
        Conferir(await autenticacao.EntrarAsync("espacos", "  senha com espacos  ") is not null
            && await autenticacao.EntrarAsync("espacos", "senha com espacos") is null, "Espaços da senha são preservados");
        var hasherAntigo = new Microsoft.AspNetCore.Identity.PasswordHasher<string>(
            Microsoft.Extensions.Options.Options.Create(new Microsoft.AspNetCore.Identity.PasswordHasherOptions { IterationCount = 10000 }));
        string hashAntigo = hasherAntigo.HashPassword("legado", senhaFicticia);
        await usuarios.CriarAsync("legado", hashAntigo, Perfil.Caixa);
        Conferir(await autenticacao.EntrarAsync("legado", senhaFicticia) is not null
            && usuarios.Dados.Single(u => u.Username == "legado").SenhaHash != hashAntigo, "Login atualiza hash com custo antigo");

        var repositorioVendas = new VendasFake();
        var vendas = new VendaService(repositorioVendas, produtos);
        carrinho.Adicionar(arroz, 2); carrinho.Adicionar(feijao, 1);
        await RecusarAsync<UnauthorizedAccessException>(() => vendas.FinalizarAsync(Admin, Guid.NewGuid(), carrinho.Itens, 100), "Admin não vende neste escopo");
        await RecusarAsync<ArgumentException>(() => vendas.FinalizarAsync(Caixa, Guid.NewGuid(), [], 100), "Carrinho vazio");
        await RecusarAsync<ArgumentException>(() => vendas.FinalizarAsync(Caixa, Guid.Empty, carrinho.Itens, 100), "Operação sem identificador");
        await RecusarAsync<ArgumentException>(() => vendas.FinalizarAsync(Caixa, Guid.NewGuid(), carrinho.Itens, -1), "Recebido negativo");
        var id = Guid.NewGuid();
        var resultado = await vendas.FinalizarAsync(Caixa, id, carrinho.Itens, 100);
        Conferir(resultado.Troco == 39.70m && repositorioVendas.Operacao == id,
            "Troco e mesma chave encaminhada ao repositório");
        Conferir(repositorioVendas.Itens?.Count == 2 && repositorioVendas.Itens[0].Quantidade == 2,
            "Pedido encaminha IDs e quantidades, sem confiar em preço da tela");
        await RecusarAsync<UnauthorizedAccessException>(() => vendas.RelatorioAsync(Caixa, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow), "Caixa não consulta relatório");
        await RecusarAsync<ArgumentException>(() => vendas.RelatorioAsync(Admin, DateTime.Now.AddDays(-1), DateTime.Now), "Relatório exige UTC");
        await RecusarAsync<ArgumentException>(() => vendas.RelatorioAsync(Admin, DateTime.UtcNow, DateTime.UtcNow.AddDays(-1)), "Relatório recusa intervalo invertido");
        await RecusarAsync<ArgumentException>(() => vendas.FinalizarAsync(Caixa, Guid.NewGuid(),
            [carrinho.Itens[0], carrinho.Itens[0]], 100), "Pedido com produto repetido é recusado");
        var excesso = new Carrinho();
        excesso.Adicionar(arroz with { Estoque = int.MaxValue }, int.MaxValue);
        await RecusarAsync<OverflowException>(() => Executar(() => excesso.Adicionar(arroz with { Estoque = int.MaxValue }, 1)), "Checked detecta excesso de quantidade acumulada");
        Conferir(excesso.Itens[0].Quantidade == int.MaxValue, "Excesso preserva quantidade anterior");

        // Aula 17: simulação de resposta perdida, sem PostgreSQL e sem teste de rede.
        var respostaPerdida = new RespostaPerdidaFake();
        var simulacao = new VendaService(respostaPerdida, produtos);
        Guid tentativa = Guid.NewGuid();
        await RecusarAsync<IOException>(() => simulacao.FinalizarAsync(Caixa, tentativa, carrinho.Itens, 100),
            "Simulação perde a primeira resposta após registrar em memória");
        var reconciliada = await simulacao.FinalizarAsync(Caixa, tentativa, carrinho.Itens, 100);
        Conferir(reconciliada.Id == 1 && respostaPerdida.Gravacoes == 1 && respostaPerdida.Operacao == tentativa,
            "Repetição da mesma tentativa recupera resultado sem segunda gravação simulada");
    }

    private static void Interfaces()
    {
        // Smoke test: construção/layout e autorização. Não substitui operação manual.
        var usuarios = new AutenticacaoService(new UsuariosFake());
        var produtos = new ProdutoService(new ProdutosFake());
        var vendas = new VendaService(new VendasFake(), new ProdutosFake());
        using var login = new FormLogin(usuarios);
        using var admin = new FormAdministrador(Admin, produtos, usuarios, vendas);
        using var caixa = new FormCaixa(Caixa, vendas);
        using var cadastro = new FormProdutos(Admin, produtos);
        using var relatorio = new FormRelatorio(Admin, vendas);
        using var contas = new FormUsuarios(Admin, usuarios);
        Conferir(new Form[] { login, admin, caixa, cadastro, relatorio, contas }.All(f => f.Controls.Count > 0),
            "Seis janelas constroem layout sem exceção");
        try { using var proibido = new FormUsuarios(Caixa, usuarios); throw new Exception("Janela de usuários aceitou caixa"); }
        catch (UnauthorizedAccessException) { Conferir(true, "Construtor de usuários recusa caixa"); }
    }
}

internal sealed class UsuariosFake : IUsuarioRepository
{
    public List<Credencial> Dados { get; } = [];
    public Task<Credencial?> BuscarAsync(string username) => Task.FromResult(Dados.SingleOrDefault(u => u.Username == username));
    public Task CriarAsync(string username, string hash, Perfil perfil)
    { Dados.Add(new(Dados.Count + 1, username, hash, perfil)); return Task.CompletedTask; }
    public Task AtualizarHashAsync(int id, string hash)
    { int i = Dados.FindIndex(u => u.Id == id); Dados[i] = Dados[i] with { SenhaHash = hash }; return Task.CompletedTask; }
    public Task<bool> ExisteAsync() => Task.FromResult(Dados.Count > 0);
}
internal sealed class ProdutosFake : IProdutoRepository
{
    public int Gravacoes { get; private set; }
    public string? UltimoCodigo { get; private set; }
    public string? UltimoNome { get; private set; }
    public int UsuarioReposicao { get; private set; }
    public Task<IReadOnlyList<Produto>> ListarAsync(string busca) => Task.FromResult<IReadOnlyList<Produto>>([]);
    public Task<Produto?> BuscarCodigoAsync(string codigo) => Task.FromResult<Produto?>(null);
    public Task SalvarAsync(int id, string codigo, string nome, decimal preco)
    { Gravacoes++; UltimoCodigo = codigo; UltimoNome = nome; return Task.CompletedTask; }
    public Task InativarAsync(int id) => Task.CompletedTask;
    public Task ReporAsync(int id, int quantidade, int usuarioId)
    { UsuarioReposicao = usuarioId; return Task.CompletedTask; }
}
internal sealed class VendasFake : IVendaRepository
{
    public Guid Operacao { get; private set; }
    public IReadOnlyList<PedidoItem>? Itens { get; private set; }
    public Task<VendaConfirmada?> BuscarAsync(Guid operacao) => Task.FromResult<VendaConfirmada?>(null);
    public Task<VendaConfirmada> FinalizarAsync(Guid operacao, int usuarioId, IReadOnlyList<PedidoItem> itens, decimal recebido)
    { Operacao = operacao; Itens = itens; return Task.FromResult(new VendaConfirmada(1, 60.30m, recebido)); }
    public Task<IReadOnlyList<ResumoVenda>> RelatorioAsync(DateTime inicioUtc, DateTime fimUtc) => Task.FromResult<IReadOnlyList<ResumoVenda>>([]);
}

internal sealed class RespostaPerdidaFake : IVendaRepository
{
    public Guid Operacao { get; private set; }
    public int Gravacoes { get; private set; }
    private VendaConfirmada? confirmada;
    public Task<VendaConfirmada?> BuscarAsync(Guid operacao)
        => Task.FromResult(operacao == Operacao ? confirmada : null);
    public Task<VendaConfirmada> FinalizarAsync(Guid operacao, int usuarioId, IReadOnlyList<PedidoItem> itens, decimal recebido)
    {
        if (confirmada is not null && operacao == Operacao) return Task.FromResult(confirmada);
        Operacao = operacao; Gravacoes++;
        confirmada = new VendaConfirmada(1, 60.30m, recebido);
        throw new IOException("Resposta perdida simulada; não houve conexão de rede.");
    }
    public Task<IReadOnlyList<ResumoVenda>> RelatorioAsync(DateTime inicioUtc, DateTime fimUtc)
        => Task.FromResult<IReadOnlyList<ResumoVenda>>([]);
}
