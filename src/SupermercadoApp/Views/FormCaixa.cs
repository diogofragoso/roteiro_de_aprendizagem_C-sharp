using SupermercadoApp.Models;
using SupermercadoApp.Services;

namespace SupermercadoApp.Views;

public sealed class FormCaixa : Form
{
    private readonly Carrinho carrinho = new();
    private Guid operacao = Guid.NewGuid();
    private bool pendente;
    private bool executando;
    private decimal recebidoPendente;

    public FormCaixa(Sessao sessao, VendaService vendas)
    {
        Permissoes.ExigirCaixa(sessao);
        var p = Ui.Preparar(this, $"MarketFlow - Caixa: {sessao.Username}", 960, 850);
        var edicao = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false };
        p.Controls.Add(edicao);
        var codigo = Ui.Texto(edicao, "Código do produto");
        edicao.Controls.Add(Ui.Rotulo("Quantidade (apenas unidades inteiras neste curso)"));
        var quantidade = new NumericUpDown { Minimum = 1, Maximum = 1000000, Value = 1 };
        edicao.Controls.Add(quantidade);
        var grade = Ui.Grade(edicao);
        var total = Ui.Rotulo("Total: R$ 0,00");
        edicao.Controls.Add(total);
        void Atualizar() { grade.DataSource = carrinho.Itens.ToList(); total.Text = $"Total previsto: {carrinho.Total:C}"; }
        Ui.Botao(edicao, "Adicionar", async (sender, _) =>
        {
            var b = (Button)sender!; b.Enabled = false; executando = true; edicao.Enabled = false;
            try { await Ui.ExecutarAsync(async () =>
            {
                var produto = await vendas.BuscarProdutoAsync(sessao, codigo.Text)
                    ?? throw new InvalidOperationException("Produto não encontrado.");
                carrinho.Adicionar(produto, (int)quantidade.Value);
                Atualizar(); codigo.Clear(); codigo.Focus();
            }); }
            finally { executando = false; b.Enabled = true; edicao.Enabled = true; }
        });
        Ui.Botao(edicao, "Remover selecionado", (_, _) =>
        {
            if (grade.CurrentRow?.DataBoundItem is ItemCarrinho item) carrinho.Remover(item.ProdutoId);
            Atualizar();
        });
        Ui.Botao(edicao, "Cancelar carrinho", (_, _) =>
        {
            carrinho.Limpar(); operacao = Guid.NewGuid(); Atualizar();
        });
        edicao.Controls.Add(Ui.Rotulo("Valor recebido em dinheiro (pagamento simulado)"));
        var recebido = new NumericUpDown { DecimalPlaces = 2, Maximum = 9999999999.99m, Width = 200 };
        edicao.Controls.Add(recebido);
        var estado = Ui.Rotulo("Adicione produtos para iniciar."); p.Controls.Add(estado);
        Ui.Botao(p, "Finalizar / consultar tentativa pendente", async (sender, _) =>
        {
            if (executando) { MessageBox.Show("Aguarde a consulta de produto terminar."); return; }
            if (carrinho.Itens.Count == 0) { MessageBox.Show("Carrinho vazio."); return; }
            var b = (Button)sender!; b.Enabled = false; edicao.Enabled = false; executando = true;
            if (!pendente) recebidoPendente = recebido.Value;
            pendente = true;
            try
            {
                var resultado = await vendas.FinalizarAsync(sessao, operacao, carrinho.Itens, recebidoPendente);
                MessageBox.Show($"COMPROVANTE DIDÁTICO - SEM VALOR FISCAL\nVenda {resultado.Id}\nTotal: {resultado.Total:C}\nRecebido: {resultado.Recebido:C}\nTroco: {resultado.Troco:C}");
                carrinho.Limpar(); operacao = Guid.NewGuid(); recebido.Value = 0; pendente = false; Atualizar();
                estado.Text = "Venda confirmada.";
            }
            catch (Exception e) when (e is ArgumentException or InvalidOperationException or UnauthorizedAccessException)
            {
                // Essas falhas ocorrem antes do COMMIT; o carrinho pode ser corrigido.
                pendente = false; MessageBox.Show(e.Message);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine($"Falha de venda: {e.GetType().Name}");
                estado.Text = $"Resultado incerto. Operação: {operacao}. Clique em finalizar novamente.";
                MessageBox.Show("Mantenha esta tela aberta. A próxima tentativa consulta a mesma operação, sem duplicar a venda.");
            }
            finally { executando = false; b.Enabled = true; edicao.Enabled = !pendente; }
        });
        Ui.Botao(p, "Sair / trocar usuário", (_, _) => Close());
        FormClosing += (_, e) =>
        {
            if (pendente || executando)
            { e.Cancel = true; MessageBox.Show("Resolva a tentativa pendente antes de sair. Anote o identificador se precisar de suporte."); }
            else if (carrinho.Itens.Count > 0 && MessageBox.Show("Descartar carrinho e sair?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes)
                e.Cancel = true;
        };
    }
}
