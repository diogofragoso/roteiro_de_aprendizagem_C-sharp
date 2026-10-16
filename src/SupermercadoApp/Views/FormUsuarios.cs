using SupermercadoApp.Models;
using SupermercadoApp.Services;

namespace SupermercadoApp.Views;

public sealed class FormUsuarios : Form
{
    public FormUsuarios(Sessao sessao, AutenticacaoService usuarios)
    {
        Permissoes.ExigirAdministrador(sessao);
        var p = Ui.Preparar(this, "Administração - Cadastrar usuário", 440, 400);
        var nome = Ui.Texto(p, "Usuário (letras sem acentos, números ou ponto)");
        var senha = Ui.Texto(p, "Senha (12 a 128 caracteres)", senha: true);
        p.Controls.Add(Ui.Rotulo("Perfil de acesso"));
        var perfil = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList,
            DataSource = Enum.GetValues<Perfil>(), Width = 260 };
        p.Controls.Add(perfil);
        Ui.Botao(p, "Cadastrar", async (sender, _) =>
        {
            var b = (Button)sender!; b.Enabled = false;
            try { await Ui.ExecutarAsync(async () =>
            {
                await usuarios.CriarAsync(sessao, nome.Text, senha.Text, (Perfil)perfil.SelectedItem!);
                senha.Clear(); nome.Clear(); MessageBox.Show("Usuário cadastrado.");
            }); }
            finally { b.Enabled = true; }
        });
    }
}
