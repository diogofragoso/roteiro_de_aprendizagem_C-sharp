# Aula 15 — Sessão, Autorização por Perfil, Layout em Código (`Ui.cs`) e Ciclo de Janelas

**Duração:** 3h30. **Pré-requisito:** [Aula 14](14-Autenticacao-Segura.md). **Entrega:** menus separados para `Administrador` e `Caixa`, autorização protegida fora da UI (`Permissoes`), helpers de layout em `Views/Ui.cs` e laço `while (true)` no `Program.cs` permitindo trocar de usuário sem vazar dados da sessão anterior (`A15.01`–`A15.04`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Demonstrar o login do administrador criado na Aula 14 |
| Explicação | 30 | Autenticação vs Autorização, layout em código (`FlowLayoutPanel`), lambdas e `ShowDialog` |
| Construção | 60 | Expandir `Views/Ui.cs` e construir `FormAdministrador` e `FormCaixa` mínimos |
| Intervalo | 15 | Pausa |
| Prática | 60 | Construir o laço de sessão no `Program.cs`, testar perfis e provar `UnauthorizedAccessException` |
| Entrega | 30 | Demonstrar 3 ciclos completos de troca de usuário e negação direta no serviço |

---

## 1. Autenticação vs Autorização — Por Que Esconder um Botão Não Basta (`A15.01` e `A15.02`)

- **Autenticação ("Quem é você?"):** provar identidade com usuário e senha (Aula 14).
- **Autorização ("O que você pode fazer?"):** decidir se aquela `Sessao` tem permissão para a operação solicitada.

| Ação no MarketFlow | Administrador | Caixa |
| --- | :---: | :---: |
| Gerenciar produtos (CRUD) e repor estoque | Permitido | Negado |
| Consultar relatório de vendas e cadastrar usuários | Permitido | Negado |
| Montar carrinho e finalizar vendas | Negado | Permitido |
| Sair / trocar de usuário | Permitido | Permitido |

**Por que chamamos `Permissoes.ExigirAdministrador(sessao)` nos Serviços e nos Construtores das telas?**
Esconder um botão no menu nunca é segurança real: se outro trecho de código ou teste chamar `ProdutoService` passando uma sessão de `Caixa`, o serviço lança **`UnauthorizedAccessException`** imediatamente antes de tocar no banco!

---

## 2. Do Designer Visual ao Layout em Código (`Ui.cs`) e Expressões Lambda (`A15.03`)

Nas Aulas 09–14 você explorou os componentes no **Designer visual** do Visual Studio 2022 (inclusive no laboratório da [Aula 10](10-Componentes-Windows-Forms.md)) e construiu `FormLogin`. A partir desta aula, as telas internas do CRUD do MarketFlow (`FormAdministrador`, `FormCaixa`, `FormProdutos`, `FormRelatorio`, `FormUsuarios`) serão construídas em código C# limpo com apoio de **`Views/Ui.cs`** e do contêiner **`FlowLayoutPanel`** (visto na Seção 2.10 da [Aula 10](10-Componentes-Windows-Forms.md)).

| Ação no Designer do VS 2022 | Código C# equivalente | O que acontece na memória? |
| --- | --- | --- |
| Arrastar um `Button` da Caixa de Ferramentas | `var botao = new Button();` | Instancia o objeto visual |
| Editar `Text` na janela Propriedades (`F4`) | `botao.Text = "Sair";` | Define o rótulo visível |
| Soltar o botão dentro da janela/painel | `painel.Controls.Add(botao);` | Vincula ao contêiner pai para ser desenhado |
| Duplo clique no botão (aba ⚡) | `botao.Click += AoSair;` | Inscreve o manipulador no evento `Click` |

### Comparando Lambda com Descartes (`_`) vs Método Nomeado (`A15.03`)

```csharp
// Forma 1 — Expressão lambda (usamos '_' quando não vamos ler 'sender' nem 'e'):
botao.Click += (_, _) => Close();

// Forma 2 — Método nomeado tradicional (100% equivalente!):
void AoSair(object? sender, EventArgs e)
{
    Close();
}
botao.Click += AoSair;
```

---

## 3. Código Completo e Compilável desta Etapa (4 Arquivos)

### Arquivo 1: `Views/Ui.cs` (Expandido com Helpers de Layout)

```csharp
namespace SupermercadoApp.Views;

internal static class Ui
{
    public static FlowLayoutPanel Preparar(Form form, string titulo, int largura = 960, int altura = 650)
    {
        form.Text = titulo;
        form.Size = new Size(largura, altura);
        form.StartPosition = FormStartPosition.CenterScreen;

        var painel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(16)
        };
        form.Controls.Add(painel);
        return painel;
    }

    public static Label Rotulo(string texto) =>
        new() { Text = texto, AutoSize = true, Margin = new Padding(3, 8, 3, 3) };

    public static Button Botao(FlowLayoutPanel painel, string texto, EventHandler clique)
    {
        var botao = new Button { Text = texto, AutoSize = true };
        botao.Click += clique; // Inscreve UMA única vez!
        painel.Controls.Add(botao);
        return botao;
    }

    public static async Task<bool> ExecutarAsync(Func<Task> acao)
    {
        try { await acao(); return true; }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException or UnauthorizedAccessException)
        { MessageBox.Show(e.Message, "Verifique os dados"); }
        catch (Exception e)
        {
            System.Diagnostics.Trace.WriteLine($"Falha: {e.GetType().Name}");
            MessageBox.Show("Não foi possível concluir. Verifique a rede e tente novamente.", "Falha de comunicação");
        }
        return false;
    }
}
```

### Arquivo 2: `Views/FormAdministrador.cs` (Menu Mínimo do Administrador)

```csharp
using SupermercadoApp.Models;
using SupermercadoApp.Services;

namespace SupermercadoApp.Views;

public sealed class FormAdministrador : Form
{
    public FormAdministrador(Sessao sessao)
    {
        // 1ª linha obrigatória: barra qualquer tentativa com perfil errado!
        Permissoes.ExigirAdministrador(sessao);

        var painel = Ui.Preparar(this, "MarketFlow — Administração", 480, 300);
        painel.Controls.Add(Ui.Rotulo($"Administrador conectado: {sessao.Username}"));

        // Teste A15.03: experimente também com método nomeado 'void AoSair(object? s, EventArgs e) => Close();'
        Ui.Botao(painel, "Sair / trocar usuário", (_, _) => Close());
    }
}
```

### Arquivo 3: `Views/FormCaixa.cs` (Menu Mínimo do Caixa)

```csharp
using SupermercadoApp.Models;
using SupermercadoApp.Services;

namespace SupermercadoApp.Views;

public sealed class FormCaixa : Form
{
    public FormCaixa(Sessao sessao)
    {
        Permissoes.ExigirCaixa(sessao);

        var painel = Ui.Preparar(this, "MarketFlow — Caixa", 480, 300);
        painel.Controls.Add(Ui.Rotulo($"Operador de caixa: {sessao.Username}"));
        Ui.Botao(painel, "Sair / trocar usuário", (_, _) => Close());
    }
}
```

### Arquivo 4: `Program.cs` (Ciclo Limpo de Sessões com `while (true)` e `using`) (`A15.04`)

```csharp
using SupermercadoApp.Infrastructure;
using SupermercadoApp.Models;
using SupermercadoApp.Repositories;
using SupermercadoApp.Services;
using SupermercadoApp.Views;

namespace SupermercadoApp;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        try
        {
            using var banco = Banco.Criar();
            var autenticacao = new AutenticacaoService(new UsuarioRepository(banco));

            if (args.Contains("--preparar-admin"))
            {
                using var preparo = new FormLogin(autenticacao, prepararAdmin: true);
                preparo.ShowDialog();
                return;
            }

            // Laço de sessões: cada troca de usuário cria telas 100% novas e descarta as antigas!
            while (true)
            {
                using var login = new FormLogin(autenticacao);
                if (login.ShowDialog() != DialogResult.OK || login.SessaoAutenticada is not { } sessao)
                    break; // Usuário fechou o Login pelo X -> encerra o aplicativo sem processo fantasma!

                // Polimorfismo: a variável 'Form principal' recebe FormAdministrador OU FormCaixa!
                using Form principal = sessao.Perfil == Perfil.Administrador
                    ? new FormAdministrador(sessao)
                    : new FormCaixa(sessao);

                principal.ShowDialog();
                // Ao clicar em "Sair / trocar usuário", 'using' destrói a janela e o while abre novo Login limpo!
            }
        }
        catch (Exception erro)
        {
            MessageBox.Show(
                erro is InvalidOperationException or UnauthorizedAccessException
                    ? erro.Message
                    : "Falha ao iniciar. Confira a configuração.");
        }
    }
}
```

---

## 4. Preparação da Conta Caixa e Verificações Obrigatórias (`A15.01`–`A15.04`)

> [!NOTE]
> **Como criar uma conta de `Caixa` antes da Aula 24?**
> - **Em laboratório com professor:** o professor usa a ferramenta docente `--preparar-caixa` ([ensino/README.md](ensino/README.md)).
> - **Estudando sozinho:** após logar como admin, você pode chamar uma única vez `await autenticacao.CriarAsync(sessaoAdmin, "caixa.lab", "CaixaSenha1234", Perfil.Caixa)` e apagar a linha em seguida (**nunca** faça `INSERT` com senha aberta no SQL!). Na Aula 24 construiremos `FormUsuarios`.

Execute com **`F5`** e comprove:
1. **`A15.01`:** Admin abre **Administração**; Caixa abre **Caixa**.
2. **`A15.02`:** no depurador (ou teste temporário), chame `Permissoes.ExigirAdministrador(sessaoDeCaixa)` comprovando `UnauthorizedAccessException`.
3. **`A15.03`:** teste o botão Sair tanto com método nomeado quanto com lambda `(_, _) => Close()`.
4. **`A15.04`:** faça 3 ciclos completos (`Admin → Sair → Caixa → Sair → Admin → Sair → fechar Login pelo X`) confirmando senha limpa e encerramento completo no VS 2022.

[Anterior: Aula 14](14-Autenticacao-Segura.md) · [Próxima: Aula 16 — Modelo de produtos](16-Modelo-de-Produtos.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 15 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-15). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
