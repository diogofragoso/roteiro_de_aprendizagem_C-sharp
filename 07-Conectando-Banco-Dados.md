# 7️⃣ Conectando o Sistema ao Banco (Instalação de Plugins e Autenticação)

Chegou o momento de ligarmos as pontas: fazer a tela de Login (que desenhamos no Módulo 5) acessar o Banco de Dados Neon (que configuramos no Módulo 6).

Por padrão, o C# sabe falar com bancos de dados da própria Microsoft (como SQL Server). Para que ele consiga falar com o **PostgreSQL** (que é o motor por trás do Neon), precisamos instalar um **Plugin / Pacote** chamado `Npgsql`.

## 1. O que é o NuGet e Instalando o Npgsql

No ecossistema .NET, a "loja de plugins" oficial se chama **NuGet Package Manager**. Não assuma que você precisa baixar arquivos em sites, o Visual Studio faz isso automaticamente.

### Passo a passo para instalar o plugin Npgsql:
1. Volte para o seu projeto no **Visual Studio 2022**.
2. No menu superior, clique em **Ferramentas (Tools)**.
3. Vá em **Gerenciador de Pacotes do NuGet (NuGet Package Manager)** e depois clique em **Gerenciar Pacotes do NuGet para a Solução...**.
4. Vai abrir uma tela. Clique na aba superior chamada **Procurar (Browse)**.
5. Na barra de pesquisa, digite exatamente: `Npgsql`
6. Clique no primeiro resultado (geralmente tem um ícone azul do PostgreSQL).
7. No painel da direita, marque a caixinha do seu projeto (`SupermercadoApp`) e clique no botão **Instalar (Install)**.
8. Uma janela de permissão vai aparecer, clique em **OK** ou **Aceitar (I Accept)**.

Pronto! O seu projeto agora sabe se comunicar com o Postgres.

## 2. Preparando os Modelos e o Repositório

Lembra do Módulo 3 sobre a arquitetura Repository? Vamos implementar isso.

### 2.1 Criando os Models
No **Gerenciador de Soluções**, clique com o botão direito na pasta `Models`, vá em `Adicionar -> Classe`. Crie a classe `Usuario.cs`:

```csharp
namespace SupermercadoApp.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Senha { get; set; }
        // Para simplificar, traremos o nome do cargo na mesma classe
        public string CargoNome { get; set; } 
    }
}
```

### 2.2 Criando o Repositório
Clique com o botão direito na pasta `Repositories`, vá em `Adicionar -> Classe`. Crie `UsuarioRepository.cs`:

```csharp
using System;
using Npgsql; // Isso só funciona porque instalamos o Plugin Npgsql!
using SupermercadoApp.Models;

namespace SupermercadoApp.Repositories
{
    public class UsuarioRepository
    {
        // ⚠️ COLOQUE AQUI A SUA STRING DE CONEXÃO DO NEON DB ⚠️
        private string stringDeConexao = "Host=...;Database=...;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true;";

        public Usuario FazerLogin(string username, string senha)
        {
            using (var conexao = new NpgsqlConnection(stringDeConexao))
            {
                conexao.Open(); // Abre a porta de internet com o banco na nuvem

                // Note que fazemos um INNER JOIN para descobrir o nome do cargo!
                string querySql = @"
                    SELECT u.Id, u.Username, t.NomeCargo 
                    FROM Usuario u
                    INNER JOIN TipoUsuario t ON u.TipoUsuarioId = t.Id
                    WHERE u.Username = @usuarioDigitado AND u.Senha = @senhaDigitada";
                
                using (var comando = new NpgsqlCommand(querySql, conexao))
                {
                    // Parâmetros de segurança (Evita ataques Hacker - SQL Injection)
                    comando.Parameters.AddWithValue("usuarioDigitado", username);
                    comando.Parameters.AddWithValue("senhaDigitada", senha);

                    using (var leitor = comando.ExecuteReader())
                    {
                        if (leitor.Read()) // Se o banco de dados achar alguém...
                        {
                            return new Usuario
                            {
                                Id = leitor.GetInt32(0),
                                Username = leitor.GetString(1),
                                CargoNome = leitor.GetString(2)
                            };
                        }
                    }
                }
            }
            return null; // Se errou a senha ou não existe, retorna vazio (null)
        }
    }
}
```

## 3. Conectando tudo no Botão de Entrar

Finalmente, vamos voltar à tela de Login. 
1. No Gerenciador de Soluções, dê um duplo clique no `FormLogin.cs` para abrir o modo visual (Designer).
2. Dê um **duplo clique no botão ENTRAR**. Isso vai gerar um método de "Clique" no código automaticamente.
3. Deixe o código do clique exatamente assim:

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
            // Pega o que o usuário digitou nas caixas de texto
            string usuario = txtUsuario.Text;
            string senha = txtSenha.Text;

            // Validação simples para não enviar vazio
            if (usuario == "" || senha == "")
            {
                MessageBox.Show("Preencha todos os campos!", "Aviso");
                return; // Para a execução do código aqui
            }

            // Chama o nosso repositório
            UsuarioRepository repositorio = new UsuarioRepository();

            try
            {
                // Tenta fazer o login
                Usuario usuarioAutenticado = repositorio.FazerLogin(usuario, senha);

                if (usuarioAutenticado != null)
                {
                    MessageBox.Show($"Login com sucesso!\nBem-vindo {usuarioAutenticado.Username}!\nVocê é um: {usuarioAutenticado.CargoNome}", "Sucesso");
                    
                    // Aqui abriremos a tela do caixa de supermercado no futuro.
                }
                else
                {
                    MessageBox.Show("Usuário ou Senha incorretos.", "Erro");
                }
            }
            catch (Exception erro)
            {
                // Se faltar internet ou a Connection String estiver errada, cairá aqui.
                MessageBox.Show($"Erro de conexão: {erro.Message}", "Erro Crítico");
            }
        }
    }
}
```

## 🎉 Sucesso Absoluto!

Aperte **Iniciar (Start)** no topo do Visual Studio. Seu programa vai rodar.
- Digite `admin` e `123456`.
- Clique em ENTRAR.
- Você verá a mensagem de sucesso resgatada diretamente das nuvens pelo Neon PostgreSQL!

Parabéns! Você concluiu os fundamentos de C#, orientação a objetos, construiu uma arquitetura em camadas e integrou um Banco de Dados Profissional em Nuvem. O céu é o limite para o seu Sistema de Supermercado!
