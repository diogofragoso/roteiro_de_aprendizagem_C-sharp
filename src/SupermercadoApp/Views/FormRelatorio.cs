using SupermercadoApp.Models;
using SupermercadoApp.Services;

namespace SupermercadoApp.Views;

public sealed class FormRelatorio : Form
{
    public FormRelatorio(Sessao sessao, VendaService vendas)
    {
        Permissoes.ExigirAdministrador(sessao);
        var p = Ui.Preparar(this, "Administração - Vendas por período");
        p.Controls.Add(Ui.Rotulo("Data inicial (horário local do Windows)"));
        var inicio = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today };
        p.Controls.Add(inicio);
        p.Controls.Add(Ui.Rotulo("Data final (inclusive)"));
        var fim = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today };
        p.Controls.Add(fim);
        var grade = Ui.Grade(p);
        var resumo = Ui.Rotulo("Nenhuma consulta realizada."); p.Controls.Add(resumo);
        Ui.Botao(p, "Consultar", async (sender, _) =>
        {
            var b = (Button)sender!; b.Enabled = false;
            try { await Ui.ExecutarAsync(async () =>
            {
                var vendasPeriodo = await vendas.RelatorioAsync(sessao,
                    inicio.Value.Date.ToUniversalTime(), fim.Value.Date.AddDays(1).ToUniversalTime());
                grade.DataSource = vendasPeriodo.Select(v => new
                    { v.Id, DataLocal = v.DataUtc.ToLocalTime(), v.Operador, v.Total }).ToList();
                resumo.Text = $"Vendas: {vendasPeriodo.Count} | Total: {vendasPeriodo.Sum(v => v.Total):C}";
            }); }
            finally { b.Enabled = true; }
        });
    }
}
