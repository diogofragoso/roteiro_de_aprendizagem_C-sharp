namespace SupermercadoApp.Models;

public sealed record VendaConfirmada(int Id, decimal Total, decimal Recebido)
{
    public decimal Troco => Recebido - Total;
}
