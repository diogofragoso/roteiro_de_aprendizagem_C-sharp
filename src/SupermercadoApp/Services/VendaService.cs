using SupermercadoApp.Models;
using SupermercadoApp.Repositories;

namespace SupermercadoApp.Services;

public sealed class VendaService(IVendaRepository vendas, IProdutoRepository produtos)
{
    public Task<Produto?> BuscarProdutoAsync(Sessao sessao, string codigo)
    {
        Permissoes.ExigirCaixa(sessao);
        return produtos.BuscarCodigoAsync(codigo.Trim());
    }

    public Task<VendaConfirmada?> ConsultarAsync(Sessao sessao, Guid operacao)
    {
        Permissoes.ExigirCaixa(sessao);
        return vendas.BuscarAsync(operacao);
    }

    public Task<VendaConfirmada> FinalizarAsync(Sessao sessao, Guid operacao,
        IReadOnlyList<ItemCarrinho> itens, decimal recebido)
    {
        Permissoes.ExigirCaixa(sessao);
        if (operacao == Guid.Empty || itens.Count == 0 || itens.Any(i => i.Quantidade <= 0))
            throw new ArgumentException("Adicione itens válidos antes de finalizar.");
        if (itens.Select(i => i.ProdutoId).Distinct().Count() != itens.Count)
            throw new ArgumentException("Agrupe os produtos repetidos no carrinho.");
        if (recebido < 0 || recebido > 9999999999.99m || decimal.Round(recebido, 2) != recebido)
            throw new ArgumentException("Valor recebido inválido.");
        // Não aceita o preço da tela como fonte da verdade: o banco recalcula na transação.
        return vendas.FinalizarAsync(operacao, sessao.Id,
            itens.Select(i => new PedidoItem(i.ProdutoId, i.Quantidade)).ToArray(), recebido);
    }

    public Task<IReadOnlyList<ResumoVenda>> RelatorioAsync(Sessao sessao, DateTime inicio, DateTime fim)
    {
        Permissoes.ExigirAdministrador(sessao);
        if (inicio.Kind != DateTimeKind.Utc || fim.Kind != DateTimeKind.Utc || fim <= inicio)
            throw new ArgumentException("Informe um intervalo UTC válido.");
        return vendas.RelatorioAsync(inicio, fim);
    }
}
