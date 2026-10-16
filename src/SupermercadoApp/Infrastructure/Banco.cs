using Npgsql;

namespace SupermercadoApp.Infrastructure;

public static class Banco
{
    public static NpgsqlDataSource Criar()
    {
        string? texto = Environment.GetEnvironmentVariable("SUPERMERCADO_DB");
        if (string.IsNullOrWhiteSpace(texto))
            throw new InvalidOperationException("Configure a variável SUPERMERCADO_DB e reabra o Visual Studio.");
        var builder = new NpgsqlConnectionStringBuilder(texto);
        // O servidor precisa apresentar certificado válido para o hostname usado.
        builder.SslMode = SslMode.VerifyFull;
        builder.Timeout = 15;
        builder.CommandTimeout = 30;
        return NpgsqlDataSource.Create(builder.ConnectionString);
    }
}
