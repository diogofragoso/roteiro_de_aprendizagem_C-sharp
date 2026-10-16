using Npgsql;
using SupermercadoApp.Models;

namespace SupermercadoApp.Repositories;

public sealed class ProdutoRepository(NpgsqlDataSource banco) : IProdutoRepository
{
    private static Produto Ler(NpgsqlDataReader r) => new(r.GetInt32(0), r.GetString(1),
        r.GetString(2), r.GetDecimal(3), r.GetInt32(4), r.GetBoolean(5));

    public async Task<IReadOnlyList<Produto>> ListarAsync(string busca)
    {
        await using var cmd = banco.CreateCommand("""
            SELECT id, codigo, nome, preco, estoque, ativo FROM produto
            WHERE nome ILIKE @busca OR codigo ILIKE @busca ORDER BY nome LIMIT 200
            """);
        cmd.Parameters.AddWithValue("busca", $"%{busca}%");
        var produtos = new List<Produto>();
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync()) produtos.Add(Ler(r));
        return produtos;
    }

    public async Task<Produto?> BuscarCodigoAsync(string codigo)
    {
        await using var cmd = banco.CreateCommand("""
            SELECT id, codigo, nome, preco, estoque, ativo FROM produto WHERE codigo = @codigo
            """);
        cmd.Parameters.AddWithValue("codigo", codigo);
        await using var r = await cmd.ExecuteReaderAsync();
        return await r.ReadAsync() ? Ler(r) : null;
    }

    public async Task SalvarAsync(int id, string codigo, string nome, decimal preco)
    {
        await using var cmd = banco.CreateCommand(id == 0
            ? "INSERT INTO produto(codigo, nome, preco) VALUES (@codigo, @nome, @preco)"
            : "UPDATE produto SET codigo=@codigo, nome=@nome, preco=@preco WHERE id=@id AND ativo");
        cmd.Parameters.AddWithValue("codigo", codigo);
        cmd.Parameters.AddWithValue("nome", nome);
        cmd.Parameters.AddWithValue("preco", preco);
        if (id != 0) cmd.Parameters.AddWithValue("id", id);
        if (await cmd.ExecuteNonQueryAsync() != 1)
            throw new InvalidOperationException("Produto inexistente ou inativo. Atualize a lista.");
    }

    public async Task InativarAsync(int id)
    {
        await using var cmd = banco.CreateCommand("UPDATE produto SET ativo=false WHERE id=@id AND ativo");
        cmd.Parameters.AddWithValue("id", id);
        if (await cmd.ExecuteNonQueryAsync() != 1)
            throw new InvalidOperationException("Produto inexistente ou já inativo.");
    }

    public async Task ReporAsync(int id, int quantidade, int usuarioId)
    {
        await using var conexao = await banco.OpenConnectionAsync();
        await using var transacao = await conexao.BeginTransactionAsync();
        await using var atualizar = new NpgsqlCommand("""
            UPDATE produto SET estoque=estoque+@q WHERE id=@id AND ativo
            """, conexao, transacao);
        atualizar.Parameters.AddWithValue("q", quantidade);
        atualizar.Parameters.AddWithValue("id", id);
        if (await atualizar.ExecuteNonQueryAsync() != 1)
            throw new InvalidOperationException("Produto inexistente ou inativo.");
        await using var movimento = new NpgsqlCommand("""
            INSERT INTO movimento_estoque(produto_id, usuario_id, quantidade, motivo)
            VALUES (@id, @usuario, @q, 'Reposicao')
            """, conexao, transacao);
        movimento.Parameters.AddWithValue("id", id);
        movimento.Parameters.AddWithValue("usuario", usuarioId);
        movimento.Parameters.AddWithValue("q", quantidade);
        await movimento.ExecuteNonQueryAsync();
        await transacao.CommitAsync();
    }
}
