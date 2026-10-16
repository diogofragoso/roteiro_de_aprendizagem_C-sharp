using Microsoft.AspNetCore.Identity;
using SupermercadoApp.Models;
using SupermercadoApp.Repositories;

namespace SupermercadoApp.Services;

public sealed class AutenticacaoService(IUsuarioRepository usuarios)
{
    private readonly PasswordHasher<string> hasher = new();

    public async Task<Sessao?> EntrarAsync(string username, string senha)
    {
        var usuario = await usuarios.BuscarAsync(username.Trim().ToLowerInvariant());
        if (usuario is null) return null;
        var resultado = hasher.VerifyHashedPassword(usuario.Username, usuario.SenhaHash, senha);
        if (resultado == PasswordVerificationResult.Failed) return null;
        if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
            await usuarios.AtualizarHashAsync(usuario.Id, hasher.HashPassword(usuario.Username, senha));
        return new Sessao(usuario.Id, usuario.Username, usuario.Perfil);
    }

    public async Task CriarAsync(Sessao administrador, string username, string senha, Perfil perfil)
    {
        Permissoes.ExigirAdministrador(administrador);
        await CadastrarAsync(username, senha, perfil);
    }

    // Operação local do professor, habilitada apenas com --preparar-admin.
    public async Task PrepararAdministradorAsync(string username, string senha)
    {
        if (await usuarios.ExisteAsync())
            throw new InvalidOperationException("A preparação exige a tabela de usuários vazia.");
        await CadastrarAsync(username, senha, Perfil.Administrador);
    }

    private async Task CadastrarAsync(string username, string senha, Perfil perfil)
    {
        username = username.Trim().ToLowerInvariant();
        if (username.Length is < 3 or > 50 || username.Any(c => !char.IsAsciiLetterOrDigit(c) && c != '.'))
            throw new ArgumentException("Usuário: 3 a 50 letras sem acentos, números ou ponto.");
        if (senha.Length is < 12 or > 128)
            throw new ArgumentException("Use uma senha de 12 a 128 caracteres.");
        if (!Enum.IsDefined(perfil)) throw new ArgumentException("Perfil inválido.");
        await usuarios.CriarAsync(username, hasher.HashPassword(username, senha), perfil);
    }
}

public static class Permissoes
{
    public static void ExigirAdministrador(Sessao sessao)
    {
        if (sessao.Perfil != Perfil.Administrador)
            throw new UnauthorizedAccessException("Esta operação exige um administrador.");
    }
    public static void ExigirCaixa(Sessao sessao)
    {
        if (sessao.Perfil != Perfil.Caixa)
            throw new UnauthorizedAccessException("Esta operação exige um operador de caixa.");
    }
}
