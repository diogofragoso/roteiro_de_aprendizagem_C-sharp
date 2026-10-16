using SupermercadoApp.Models;

namespace SupermercadoApp.Repositories;

public interface IUsuarioRepository
{
    Task<Credencial?> BuscarAsync(string username);
    Task CriarAsync(string username, string hash, Perfil perfil);
    Task AtualizarHashAsync(int id, string hash);
    Task<bool> ExisteAsync();
}
