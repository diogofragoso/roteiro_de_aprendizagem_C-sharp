using SupermercadoApp.Models;

namespace SupermercadoApp.Services;

public sealed class Carrinho
{
    private readonly List<ItemCarrinho> itens = [];
    public IReadOnlyList<ItemCarrinho> Itens => itens.AsReadOnly();
    public decimal Total => itens.Sum(i => i.Subtotal);

    public void Adicionar(Produto produto, int quantidade)
    {
        if (!produto.Ativo || quantidade <= 0)
            throw new ArgumentException("Produto deve estar ativo e quantidade deve ser positiva.");
        int indice = itens.FindIndex(i => i.ProdutoId == produto.Id);
        int acumulada = checked(quantidade + (indice >= 0 ? itens[indice].Quantidade : 0));
        if (acumulada > produto.Estoque)
            throw new InvalidOperationException("Estoque insuficiente para esta quantidade.");
        // Preserva o preço mostrado anteriormente para um mesmo item.
        if (indice >= 0) itens[indice] = itens[indice] with { Quantidade = acumulada };
        else itens.Add(new ItemCarrinho(produto.Id, produto.Nome, produto.Preco, quantidade));
    }

    public void Remover(int produtoId) => itens.RemoveAll(i => i.ProdutoId == produtoId);
    public void Limpar() => itens.Clear();
}
