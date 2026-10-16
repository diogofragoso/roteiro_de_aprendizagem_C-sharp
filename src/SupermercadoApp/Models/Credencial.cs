namespace SupermercadoApp.Models;

public sealed record Credencial(int Id, string Username, string SenhaHash, Perfil Perfil);
