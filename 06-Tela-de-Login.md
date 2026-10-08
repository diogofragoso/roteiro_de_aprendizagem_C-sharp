# 6️⃣ Criando a Tela de Login e Autenticação

A primeira impressão do nosso sistema será a tela de login. Queremos algo moderno, com cara de aplicação profissional.

![Mockup da Tela de Login](./assets/winforms_login_mockup_1791481192847.jpg)
*(Ideia de design para a nossa tela de Login)*

## 1. Desenhando a Interface (FormLogin)

No Visual Studio:
1. Mova o `Form1` para a pasta `Views` e renomeie-o para `FormLogin`.
2. Abra o modo Designer (Dê dois cliques em `FormLogin.cs`).
3. Na janela de Propriedades, configure o Formulário:
   - `Text`: Supermercado - Login
   - `BackColor`: Escolha uma cor escura (ex: `45, 45, 48`) ou branca limpa.
   - `StartPosition`: `CenterScreen` (Para abrir no meio da tela).
   - `FormBorderStyle`: `FixedSingle` (Para o usuário não redimensionar).

### Adicionando os Controles (Toolbox)
Arraste os seguintes itens para a tela:
1. **Label** (`lblTitulo`): Escreva "MarketFlow - Sistema de Supermercado".
2. **Label** e **TextBox** para o Usuário:
   - Nomeie o TextBox como `txtUsuario`.
3. **Label** e **TextBox** para a Senha:
   - Nomeie o TextBox como `txtSenha`.
   - Mude a propriedade `PasswordChar` para `*` (para esconder a senha).
4. **Button** (`btnLogin`):
   - Mude o texto para "ENTRAR".
   - Mude o `BackColor` para Verde ou Azul para destacar.

## 2. Escrevendo o Código de Ação

Dê um duplo clique no botão "ENTRAR" no designer. O Visual Studio vai te levar para o código C#. Vamos usar o repositório que criamos no módulo anterior!

```csharp
using System;
using System.Windows.Forms;
using SupermercadoApp.Repositories;
using SupermercadoApp.Models;

namespace SupermercadoApp.Views
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string usuarioDigitado = txtUsuario.Text;
            string senhaDigitada = txtSenha.Text;

            // Validação simples
            if (string.IsNullOrWhiteSpace(usuarioDigitado) || string.IsNullOrWhiteSpace(senhaDigitada))
            {
                MessageBox.Show("Por favor, preencha usuário e senha.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. Instanciar o Repositório
            UsuarioRepository repo = new UsuarioRepository();

            try
            {
                // 2. Chamar o método de autenticação
                Usuario usuarioAutenticado = repo.Autenticar(usuarioDigitado, senhaDigitada);

                // 3. Validar a resposta
                if (usuarioAutenticado != null)
                {
                    MessageBox.Show($"Bem-vindo, {usuarioAutenticado.Username}!\nCargo: {usuarioAutenticado.Cargo}", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Aqui, num sistema real, nós esconderíamos a tela de login
                    // e abriríamos o Form Principal (Dashboard do Supermercado).
                    // Exemplo:
                    // FormPrincipal form = new FormPrincipal();
                    // form.Show();
                    // this.Hide();
                }
                else
                {
                    MessageBox.Show("Usuário ou senha incorretos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro de conexão com o Banco de Dados Neon: " + ex.Message, "Erro Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
```

## 🎉 Parabéns! 

Você acabou de concluir o fluxo completo de uma funcionalidade real!
1. Criou o modelo de dados (`Usuario.cs`).
2. Criou o acesso ao banco isolado (`UsuarioRepository.cs`) usando conexão em nuvem.
3. Desenhou uma interface moderna em Windows Forms (`FormLogin.cs`).
4. Conectou a interface ao banco usando eventos de clique!

### Próximos Desafios (Para você praticar)
- Criar a tela principal (`FormPrincipal`) que abre após o login.
- Criar um Cadastro de Produtos (CRUD: Create, Read, Update, Delete) seguindo a mesma arquitetura.
- Melhorar o design da sua aplicação pesquisando sobre bibliotecas como `MaterialSkin` para Windows Forms!
