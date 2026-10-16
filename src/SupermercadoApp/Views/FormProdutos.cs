using SupermercadoApp.Models;
using SupermercadoApp.Services;

namespace SupermercadoApp.Views;

public sealed class FormProdutos : Form
{
    private int produtoId;
    public FormProdutos(Sessao sessao, ProdutoService produtos)
    {
        Permissoes.ExigirAdministrador(sessao);
        var p = Ui.Preparar(this, "Administração - Produtos e estoque", 960, 900);
        var busca = Ui.Texto(p, "Buscar por nome ou código (até 200 resultados)");
        var grade = Ui.Grade(p);
        var codigo = Ui.Texto(p, "Código (texto, preserva zeros à esquerda)");
        var nome = Ui.Texto(p, "Nome");
        p.Controls.Add(Ui.Rotulo("Preço de venda"));
        var preco = new NumericUpDown { DecimalPlaces = 2, Maximum = 99999999.99m, Width = 180 };
        p.Controls.Add(preco);
        var acoes = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
        p.Controls.Add(acoes);
        async Task Carregar() => grade.DataSource = (await produtos.ListarAsync(sessao, busca.Text)).ToList();
        void Novo() { produtoId = 0; codigo.Clear(); nome.Clear(); preco.Value = 0; grade.ClearSelection(); }
        Produto Selecionado()
        {
            if (grade.SelectedRows.Count == 0 || grade.SelectedRows[0].DataBoundItem is not Produto produto)
                throw new InvalidOperationException("Selecione um produto na grade.");
            return produto;
        }
        Ui.Botao(acoes, "Buscar / atualizar", async (_, _) => await Ui.ExecutarAsync(Carregar));
        Ui.Botao(acoes, "Novo", (_, _) => Novo());
        Ui.Botao(acoes, "Editar selecionado", async (_, _) => await Ui.ExecutarAsync(() =>
        {
            var atual = Selecionado();
            produtoId = atual.Id; codigo.Text = atual.Codigo; nome.Text = atual.Nome; preco.Value = atual.Preco;
            return Task.CompletedTask;
        }));
        Ui.Botao(acoes, "Salvar", async (sender, _) =>
        {
            var b = (Button)sender!; b.Enabled = false;
            try { await Ui.ExecutarAsync(async () =>
                { await produtos.SalvarAsync(sessao, produtoId, codigo.Text, nome.Text, preco.Value); await Carregar(); Novo(); }); }
            finally { b.Enabled = true; }
        });
        Ui.Botao(acoes, "Inativar selecionado", async (_, _) => await Ui.ExecutarAsync(async () =>
        {
            var atual = Selecionado();
            if (MessageBox.Show($"Inativar {atual.Nome}?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            await produtos.InativarAsync(sessao, atual.Id); await Carregar(); Novo();
        }));
        p.Controls.Add(Ui.Rotulo("Quantidade para repor no produto selecionado"));
        var quantidade = new NumericUpDown { Minimum = 1, Maximum = 1000000, Value = 1, Width = 180 };
        p.Controls.Add(quantidade);
        Ui.Botao(p, "Registrar reposição", async (sender, _) =>
        {
            var b = (Button)sender!; b.Enabled = false;
            try { await Ui.ExecutarAsync(async () =>
                { await produtos.ReporAsync(sessao, Selecionado().Id, (int)quantidade.Value); await Carregar(); }); }
            finally { b.Enabled = true; }
        });
        Shown += async (_, _) => await Ui.ExecutarAsync(Carregar);
    }
}
