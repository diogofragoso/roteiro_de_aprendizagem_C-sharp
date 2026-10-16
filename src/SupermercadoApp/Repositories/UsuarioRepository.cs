using Npgsql;
using SupermercadoApp.Models;

namespace SupermercadoApp.Repositories;

public sealed class UsuarioRepository(NpgsqlDataSource banco) : IUsuarioRepository
{
    public async Task<Credencial?> BuscarAsync(string username)
    {
        await using var cmd = banco.CreateCommand("""
            SELECT id, username, senha_hash, perfil FROM usuario WHERE username = @nome
            """);
        cmd.Parameters.AddWithValue("nome", username);
        await using var leitor = await cmd.ExecuteReaderAsync();
        if (!await leitor.ReadAsync()) return null;
        return new Credencial(leitor.GetInt32(0), leitor.GetString(1), leitor.GetString(2),
            Enum.Parse<Perfil>(leitor.GetString(3)));
    }

    public async Task CriarAsync(string username, string hash, Perfil perfil)
    {
        await using var cmd = banco.CreateCommand("""
            INSERT INTO usuario(username, senha_hash, perfil) VALUES (@nome, @hash, @perfil)
            """);
        cmd.Parameters.AddWithValue("nome", username);
        cmd.Parameters.AddWithValue("hash", hash);
        cmd.Parameters.AddWithValue("perfil", perfil.ToString());
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task AtualizarHashAsync(int id, string hash)
    {
        await using var cmd = banco.CreateCommand("UPDATE usuario SET senha_hash = @hash WHERE id = @id");
        cmd.Parameters.AddWithValue("hash", hash);
        cmd.Parameters.AddWithValue("id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<bool> ExisteAsync()
    {
        await using var cmd = banco.CreateCommand("SELECT EXISTS(SELECT 1 FROM usuario)");
        return (bool)(await cmd.ExecuteScalarAsync())!;
    }
}
