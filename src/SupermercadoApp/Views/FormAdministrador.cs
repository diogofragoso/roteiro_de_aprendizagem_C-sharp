using SupermercadoApp.Models;
using SupermercadoApp.Services;

namespace SupermercadoApp.Views;

public sealed class FormAdministrador : Form
{
    public FormAdministrador(Sessao sessao, ProdutoService produtos, AutenticacaoService usuarios, VendaService vendas)
    {
        Permissoes.ExigirAdministrador(sessao);
        var p = Ui.Preparar(this, "MarketFlow - Administração", 480, 350);
        p.Controls.Add(Ui.Rotulo($"Administrador: {sessao.Username}"));
        Ui.Botao(p, "Produtos e estoque", (_, _) => { using var f = new FormProdutos(sessao, produtos); f.ShowDialog(this); });
        Ui.Botao(p, "Cadastrar usuário", (_, _) => { using var f = new FormUsuarios(sessao, usuarios); f.ShowDialog(this); });
        Ui.Botao(p, "Relatório de vendas", (_, _) => { using var f = new FormRelatorio(sessao, vendas); f.ShowDialog(this); });
        Ui.Botao(p, "Sair / trocar usuário", (_, _) => Close());
    }
}
