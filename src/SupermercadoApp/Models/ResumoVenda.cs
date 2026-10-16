namespace SupermercadoApp.Models;

public sealed record ResumoVenda(int Id, DateTime DataUtc, string Operador, decimal Total);
