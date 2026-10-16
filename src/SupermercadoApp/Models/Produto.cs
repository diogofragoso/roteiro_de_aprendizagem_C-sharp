namespace SupermercadoApp.Models;

public sealed record Produto(int Id, string Codigo, string Nome, decimal Preco, int Estoque, bool Ativo);
