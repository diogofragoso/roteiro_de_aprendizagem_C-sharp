using SupermercadoApp.Models;
using SupermercadoApp.Services;

namespace SupermercadoApp.Views;

public sealed class FormLogin : Form
{
    public Sessao? SessaoAutenticada { get; private set; }
    public FormLogin(AutenticacaoService autenticacao, bool prepararAdmin = false)
    {
        var p = Ui.Preparar(this, prepararAdmin ? "Preparar primeiro administrador" : "MarketFlow - Login", 430, 330);
        var txtUsuario = Ui.Texto(p, "Usuário");
        var txtSenha = Ui.Texto(p, "Senha", senha: true);
        var btnLogin = Ui.Botao(p, prepararAdmin ? "Criar administrador" : "Entrar", async (sender, e) =>
        {
            var botao = (Button)sender!;
            botao.Enabled = false;
            await Ui.ExecutarAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(txtUsuario.Text) || txtSenha.Text.Length == 0)
                    throw new ArgumentException("Preencha usuário e senha.");
                if (prepararAdmin)
                {
                    await autenticacao.PrepararAdministradorAsync(txtUsuario.Text, txtSenha.Text);
                    MessageBox.Show("Administrador criado. Inicie novamente sem --preparar-admin.");
                    DialogResult = DialogResult.OK;
                    return;
                }
                SessaoAutenticada = await autenticacao.EntrarAsync(txtUsuario.Text, txtSenha.Text);
                if (SessaoAutenticada is null) MessageBox.Show("Usuário ou senha inválidos.");
                else DialogResult = DialogResult.OK;
            });
            txtSenha.Clear();
            botao.Enabled = true;
        });
        AcceptButton = btnLogin;
    }
}
