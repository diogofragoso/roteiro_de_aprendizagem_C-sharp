using SupermercadoApp.Models;
using SupermercadoApp.Repositories;

namespace SupermercadoApp.Services;

public sealed class ProdutoService(IProdutoRepository produtos)
{
    public Task<IReadOnlyList<Produto>> ListarAsync(Sessao sessao, string busca)
    {
        Permissoes.ExigirAdministrador(sessao);
        return produtos.ListarAsync(busca.Trim());
    }

    public Task SalvarAsync(Sessao sessao, int id, string codigo, string nome, decimal preco)
    {
        Permissoes.ExigirAdministrador(sessao);
        codigo = codigo.Trim();
        nome = nome.Trim();
        if (id < 0 || codigo.Length is < 1 or > 30 || nome.Length is < 2 or > 120)
            throw new ArgumentException("Informe código (1 a 30) e nome (2 a 120 caracteres).");
        if (preco <= 0 || preco > 99999999.99m || decimal.Round(preco, 2) != preco)
            throw new ArgumentException("Preço deve ser positivo, com até duas casas decimais.");
        return produtos.SalvarAsync(id, codigo, nome, preco);
    }

    public Task InativarAsync(Sessao sessao, int id)
    {
        Permissoes.ExigirAdministrador(sessao);
        return produtos.InativarAsync(id);
    }

    public Task ReporAsync(Sessao sessao, int id, int quantidade)
    {
        Permissoes.ExigirAdministrador(sessao);
        if (quantidade <= 0) throw new ArgumentException("Reposição deve ser maior que zero.");
        return produtos.ReporAsync(id, quantidade, sessao.Id);
    }
}
