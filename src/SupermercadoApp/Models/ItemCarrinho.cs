namespace SupermercadoApp.Models;

public sealed record ItemCarrinho(int ProdutoId, string Nome, decimal Preco, int Quantidade)
{
    public decimal Subtotal => Preco * Quantidade;
}
