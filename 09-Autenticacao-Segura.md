# Aula 09 — Autenticação com hash

**Duração:** 3h30. **Pré-requisito:** aula 08; tabela `usuario` vazia e conexão funcionando. **Entrega:** primeiro administrador cadastrado com hash e login que aceita/rejeita credenciais sem expor a senha.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Demonstrar SELECT 1 e remover diagnóstico da tela |
| Explicação | 30 | Hash, salt, null e contrato assíncrono |
| Construção | 60 | Modelos, repositório e serviço de autenticação |
| Intervalo | 15 | Pausa |
| Prática | 60 | Preparar administrador e ligar o botão Entrar |
| Entrega | 30 | Verificar senha correta/incorreta e banco |

## 1. Crie os tipos e o contrato

Trabalhe em quatro checkpoints: **tipos/ausência**, **consulta**, **hash/verificação** e **evento da tela**. Use a versão 08 como início e a 09 como recuperação, geradas conforme [apoio ao professor](ensino/README.md). Compile em cada parada, antes de ligar o botão Entrar. Não adicione simultaneamente arquivos que você ainda não consegue localizar no fluxo.

| Parada | Pergunta de leitura | Verificação antes de avançar |
| --- | --- | --- |
| Tipos | O que a tela pode receber e o que fica interno? | Sessao não tem hash; explique `Credencial?`. |
| Consulta | O que retorna para username inexistente? | `null`, sem acessar propriedades de usuário ausente. |
| Verificação | O hash é comparado pelo SQL ou pela biblioteca? | Biblioteca verifica senha correta e incorreta; salt produz representações diferentes. |
| Evento | Quando pode devolver DialogResult.OK? | Somente após preparação ou autenticação bem-sucedida; senha limpa ao terminar. |

Em **Gerenciador de Soluções → Models → Adicionar → Classe**, crie os arquivos abaixo, substituindo o conteúdo gerado pelo conteúdo da referência:

- [Perfil.cs](src/SupermercadoApp/Models/Perfil.cs): `enum` com `Administrador` e `Caixa`.
- [Credencial.cs](src/SupermercadoApp/Models/Credencial.cs): dados internos necessários à verificação da senha.
- [Sessao.cs](src/SupermercadoApp/Models/Sessao.cs): dados devolvidos para a interface, sem hash.

`record` é uma forma concisa de representar um conjunto de dados. Seus parâmetros viram propriedades; não precisamos escrever getters manualmente. Usamos esses objetos como resultados de consulta, sem modificá-los na tela.

Em `Repositories`, crie [IUsuarioRepository.cs](src/SupermercadoApp/Repositories/IUsuarioRepository.cs) e [UsuarioRepository.cs](src/SupermercadoApp/Repositories/UsuarioRepository.cs). Implemente os quatro métodos da referência. `Task<Credencial?>` informa que a consulta é assíncrona e pode retornar ausência de usuário. `@nome` é um parâmetro: o texto digitado nunca é concatenado ao SQL.

A declaração `class AutenticacaoService(IUsuarioRepository usuarios)` usa um **construtor primário**, disponível desde o C# 12 e também no .NET 10: recebe a dependência ao criar o objeto, como o construtor escrito por extenso na aula 04. `sealed` impede herança dessa classe; não altera a forma de usá-la.

**Por quê buscar somente pelo username?** A senha não pode ser comparada como texto SQL, porque o banco guarda um hash. O serviço recebe a credencial e pede à biblioteca para verificar.

## 2. Adicione a biblioteca de senha e o serviço

No NuGet, instale `Microsoft.Extensions.Identity.Core` **10.0.15** no projeto. Apesar do namespace `Microsoft.AspNetCore.Identity`, a classe `PasswordHasher` pode ser usada nesse aplicativo desktop; não estamos criando um servidor web.

Crie [AutenticacaoService.cs](src/SupermercadoApp/Services/AutenticacaoService.cs) com o conteúdo de referência. Nesse encontro a classe `Permissoes` do mesmo arquivo pode ser copiada também: são duas verificações simples de perfil que usaremos na aula 10. Leia com atenção:

```csharp
var resultado = hasher.VerifyHashedPassword(usuario.Username, usuario.SenhaHash, senha);
if (resultado == PasswordVerificationResult.Failed) return null;
```

O hash não é “descriptografado”. A biblioteca verifica a senha usando o formato armazenado. O salt permite que senhas iguais gerem representações diferentes. `SuccessRehashNeeded` indica que o hash pode ser atualizado para os parâmetros atuais; o serviço já faz essa atualização.

A política didática pede 12 a 128 caracteres. Não fazemos `Trim` na senha: espaços podem fazer parte dela. O username é normalizado para minúsculas, para que `ADMIN` e `admin` indiquem a mesma conta.

## 3. Prepare o primeiro administrador

Há um problema inicial: quem cadastra o primeiro administrador se ainda não podemos entrar? A referência tem um modo local de preparação, **somente para tabela vazia**, acionado por `--preparar-admin`. Ele recebe a senha em um campo oculto, calcula o hash e insere a conta; não contém senha padrão.

Para seu projeto com Designer:

1. Remova `btnTestarConexao` e o campo `NpgsqlDataSource` de `FormLogin`. A tela recebe agora `AutenticacaoService` no construtor. Mantenha `InitializeComponent()` e salve a dependência em um campo `private readonly`.
2. Adicione também o parâmetro opcional `bool prepararAdmin = false` e salve-o em um campo. Se verdadeiro, mude o texto do botão para “Criar administrador”.
3. Crie uma propriedade `public Sessao? SessaoAutenticada { get; private set; }` no formulário.
4. No clique, mantenha a validação de campos da aula 06. Troque o corpo pela lógica de [FormLogin.cs](src/SupermercadoApp/Views/FormLogin.cs): desabilitar botão, preparar conta ou autenticar, limpar senha e reabilitar. A referência usa uma lambda de evento; no Designer você coloca esse mesmo corpo em `private async void btnLogin_Click(...)`.
5. Copie também [Ui.cs](src/SupermercadoApp/Views/Ui.cs) para `Views`: hoje usamos `ExecutarAsync` para apresentar erros. As funções de layout serão utilizadas nas telas posteriores.
6. Em `Program`, crie `Banco`, `UsuarioRepository` e `AutenticacaoService` e abra `new FormLogin(autenticacao, prepararAdmin)`. Use `ShowDialog()` para receber o resultado. Ainda não adicione os serviços de produtos e vendas do `Program.cs` final; eles não existem nesta aula.

Trecho mínimo dentro de `Main(string[] args)`, depois de `ApplicationConfiguration.Initialize()`:

```csharp
using var banco = Banco.Criar();
var autenticacao = new AutenticacaoService(new UsuarioRepository(banco));
bool prepararAdmin = args.Contains("--preparar-admin");
using var login = new FormLogin(autenticacao, prepararAdmin);
login.ShowDialog();
if (!prepararAdmin && login.SessaoAutenticada is { } sessao)
    MessageBox.Show($"Bem-vindo, {sessao.Username} ({sessao.Perfil})");
```

Adicione os `using` de `Infrastructure`, `Repositories` e `Services` no início de `Program.cs`. Preserve o tratamento de configuração ausente da aula 08 em volta desse trecho.

### Código do login para quem manteve o Designer

Para evitar misturar as duas formas de construir a tela, este é o conteúdo de `FormLogin.cs` com os controles da aula 06. Mantenha `FormLogin.Designer.cs` existente e confira o evento Click nas Propriedades. Este código substitui o comportamento antigo; não crie uma segunda classe `FormLogin`.

```csharp
using SupermercadoApp.Models;
using SupermercadoApp.Services;

namespace SupermercadoApp.Views;

public partial class FormLogin : Form
{
    private readonly AutenticacaoService autenticacao;
    private readonly bool prepararAdmin;
    public Sessao? SessaoAutenticada { get; private set; }

    public FormLogin(AutenticacaoService autenticacao, bool prepararAdmin = false)
    {
        InitializeComponent();
        this.autenticacao = autenticacao;
        this.prepararAdmin = prepararAdmin;
        if (prepararAdmin) btnLogin.Text = "Criar administrador";
    }

    private async void btnLogin_Click(object sender, EventArgs e)
    {
        btnLogin.Enabled = false;
        try
        {
            await Ui.ExecutarAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(txtUsuario.Text) || txtSenha.Text.Length == 0)
                    throw new ArgumentException("Preencha usuário e senha.");
                if (prepararAdmin)
                {
                    await autenticacao.PrepararAdministradorAsync(txtUsuario.Text, txtSenha.Text);
                    MessageBox.Show("Administrador criado. Remova o argumento de preparação.");
                    DialogResult = DialogResult.OK;
                    return;
                }
                SessaoAutenticada = await autenticacao.EntrarAsync(txtUsuario.Text, txtSenha.Text);
                if (SessaoAutenticada is null)
                    MessageBox.Show("Usuário ou senha inválidos.");
                else DialogResult = DialogResult.OK;
            });
        }
        finally
        {
            txtSenha.Clear();
            btnLogin.Enabled = true;
        }
    }
}
```

O formulário é `partial` porque a outra parte está no Designer. A referência sem Designer pode ser `sealed` e não precisa de `InitializeComponent`. Em ambos os casos a tela chama o mesmo serviço e devolve a mesma sessão.

Em **Propriedades do projeto → Depurar → Abrir perfis de inicialização**, coloque `--preparar-admin` nos argumentos de linha de comando. Execute F5, escolha seu username e senha e crie o administrador. Pare, **remova o argumento**, execute novamente e faça login. Se a tabela já contém alguém, a preparação deve recusar a execução. O professor deve executar esse preparo uma única vez por ambiente, sem duas instâncias concorrentes.

## 4. Confira no banco e na tela

```sql
SELECT username, perfil, length(senha_hash) AS tamanho_hash FROM usuario;
```

Não projete a coluna inteira de hash na apresentação da turma. A senha deve ser diferente do valor armazenado; a consulta acima permite conferir presença sem expor o conteúdo.

- Login correto: mensagem de boas-vindas; incorreto: “Usuário ou senha inválidos”.
- Usuário inexistente recebe a mesma mensagem de senha incorreta.
- Campo vazio é recusado antes da consulta; Enter aciona o login uma vez.
- Sem conexão, o botão volta a ficar disponível e não há mensagem de sucesso.

Na aula 10 trocaremos a mensagem pelas janelas de cada perfil. Autenticação identifica a pessoa; autorização decide o que ela pode fazer. A biblioteca de hash não adiciona bloqueio por tentativas, recuperação de senha ou autenticação de servidor: esses são aprimoramentos posteriores ao recorte local.

[Anterior: aula 08](08-Conectando-Banco-Dados.md) · [Próxima: aula 10](10-Sessao-e-Perfis.md)

## Atividades por conceito

Realize as atividades da [aula 09 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-09). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
