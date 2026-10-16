using SupermercadoApp.Models;

namespace SupermercadoApp.Repositories;

public interface IVendaRepository
{
    Task<VendaConfirmada?> BuscarAsync(Guid operacao);
    Task<VendaConfirmada> FinalizarAsync(Guid operacao, int usuarioId,
        IReadOnlyList<PedidoItem> itens, decimal recebido);
    Task<IReadOnlyList<ResumoVenda>> RelatorioAsync(DateTime inicioUtc, DateTime fimUtc);
}
