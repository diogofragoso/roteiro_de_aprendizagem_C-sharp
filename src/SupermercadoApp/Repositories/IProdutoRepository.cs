using SupermercadoApp.Models;

namespace SupermercadoApp.Repositories;

public interface IProdutoRepository
{
    Task<IReadOnlyList<Produto>> ListarAsync(string busca);
    Task<Produto?> BuscarCodigoAsync(string codigo);
    Task SalvarAsync(int id, string codigo, string nome, decimal preco);
    Task InativarAsync(int id);
    Task ReporAsync(int id, int quantidade, int usuarioId);
}
