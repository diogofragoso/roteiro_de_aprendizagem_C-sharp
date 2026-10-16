using Npgsql;
using SupermercadoApp.Models;

namespace SupermercadoApp.Repositories;

public sealed class VendaRepository(NpgsqlDataSource banco) : IVendaRepository
{
    public async Task<VendaConfirmada?> BuscarAsync(Guid operacao)
    {
        await using var cmd = banco.CreateCommand(
            "SELECT id, total, recebido FROM venda WHERE operacao=@op");
        cmd.Parameters.AddWithValue("op", operacao);
        await using var r = await cmd.ExecuteReaderAsync();
        return await r.ReadAsync() ? new(r.GetInt32(0), r.GetDecimal(1), r.GetDecimal(2)) : null;
    }

    public async Task<VendaConfirmada> FinalizarAsync(Guid operacao, int usuarioId,
        IReadOnlyList<PedidoItem> itens, decimal recebido)
    {
        await using var conexao = await banco.OpenConnectionAsync();
        await using var transacao = await conexao.BeginTransactionAsync();
        // Serializa tentativas da mesma operação, inclusive em duas conexões.
        await using (var trava = new NpgsqlCommand(
            "SELECT pg_advisory_xact_lock(hashtextextended(@op, 0))", conexao, transacao))
        {
            trava.Parameters.AddWithValue("op", operacao.ToString());
            await trava.ExecuteNonQueryAsync();
        }
        await using (var consulta = new NpgsqlCommand(
            "SELECT id, total, recebido FROM venda WHERE operacao=@op AND usuario_id=@usuario",
            conexao, transacao))
        {
            consulta.Parameters.AddWithValue("op", operacao);
            consulta.Parameters.AddWithValue("usuario", usuarioId);
            await using var r = await consulta.ExecuteReaderAsync();
            if (await r.ReadAsync())
                return new(r.GetInt32(0), r.GetDecimal(1), r.GetDecimal(2));
        }

        var confirmados = new List<ItemCarrinho>();
        // Mesma ordem de bloqueio reduz a possibilidade de deadlock entre caixas.
        foreach (var item in itens.OrderBy(i => i.ProdutoId))
        {
            await using var cmd = new NpgsqlCommand("""
                SELECT nome, preco, estoque, ativo FROM produto WHERE id=@id FOR UPDATE
                """, conexao, transacao);
            cmd.Parameters.AddWithValue("id", item.ProdutoId);
            await using var r = await cmd.ExecuteReaderAsync();
            if (!await r.ReadAsync() || !r.GetBoolean(3) || r.GetInt32(2) < item.Quantidade)
                throw new InvalidOperationException("Produto inativo ou estoque insuficiente. Revise o carrinho.");
            confirmados.Add(new(item.ProdutoId, r.GetString(0), r.GetDecimal(1), item.Quantidade));
        }
        decimal total = confirmados.Sum(i => i.Subtotal);
        if (total <= 0 || total > 9999999999.99m || recebido < total)
            throw new InvalidOperationException($"Total atualizado: {total:C}. Valor recebido insuficiente ou total inválido.");

        int vendaId;
        await using (var cmd = new NpgsqlCommand("""
            INSERT INTO venda(operacao, usuario_id, total, recebido)
            VALUES (@op, @usuario, @total, @recebido) RETURNING id
            """, conexao, transacao))
        {
            cmd.Parameters.AddWithValue("op", operacao);
            cmd.Parameters.AddWithValue("usuario", usuarioId);
            cmd.Parameters.AddWithValue("total", total);
            cmd.Parameters.AddWithValue("recebido", recebido);
            vendaId = (int)(await cmd.ExecuteScalarAsync())!;
        }
        foreach (var item in confirmados)
        {
            await using var cmd = new NpgsqlCommand("""
                INSERT INTO venda_item(venda_id, produto_id, nome, quantidade, preco_unitario)
                VALUES (@venda, @produto, @nome, @q, @preco);
                UPDATE produto SET estoque=estoque-@q WHERE id=@produto;
                INSERT INTO movimento_estoque(produto_id, usuario_id, quantidade, motivo)
                VALUES (@produto, @usuario, -@q, 'Venda');
                """, conexao, transacao);
            cmd.Parameters.AddWithValue("venda", vendaId);
            cmd.Parameters.AddWithValue("produto", item.ProdutoId);
            cmd.Parameters.AddWithValue("nome", item.Nome);
            cmd.Parameters.AddWithValue("q", item.Quantidade);
            cmd.Parameters.AddWithValue("preco", item.Preco);
            cmd.Parameters.AddWithValue("usuario", usuarioId);
            await cmd.ExecuteNonQueryAsync();
        }
        // Só depois de confirmar é seguro anunciar sucesso e limpar o carrinho.
        await transacao.CommitAsync();
        return new(vendaId, total, recebido);
    }

    public async Task<IReadOnlyList<ResumoVenda>> RelatorioAsync(DateTime inicioUtc, DateTime fimUtc)
    {
        await using var cmd = banco.CreateCommand("""
            SELECT v.id, v.criado_em, u.username, v.total FROM venda v
            JOIN usuario u ON u.id=v.usuario_id
            WHERE v.criado_em >= @inicio AND v.criado_em < @fim
            ORDER BY v.criado_em, v.id
            """);
        cmd.Parameters.AddWithValue("inicio", inicioUtc);
        cmd.Parameters.AddWithValue("fim", fimUtc);
        var resultado = new List<ResumoVenda>();
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
            resultado.Add(new(r.GetInt32(0), r.GetDateTime(1), r.GetString(2), r.GetDecimal(3)));
        return resultado;
    }
}
