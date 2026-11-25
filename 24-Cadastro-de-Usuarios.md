# Aula 24 — Cadastro de Usuários, Reuso de Serviços, `ComboBox` com `Enum` e Composição Final

**Duração:** 3h30. **Pré-requisito:** [Aula 23](23-Relatorio-de-Vendas.md) e administrador autenticado. **Entrega:** tela definitiva `FormUsuarios` integrada ao `FormAdministrador` completo e ao `Program.cs` final, reutilizando `AutenticacaoService.CriarAsync` sem duplicar hash nem SQL (`A24.01`–`A24.06`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Revisar `AutenticacaoService.CriarAsync` criado na Aula 14 |
| Explicação | 30 | Reuso da camada de serviço, `ComboBox` fechado (`DropDownList`) com `Enum` e limites |
| Construção | 60 | Construir `FormUsuarios.cs`, finalizar `FormAdministrador.cs` e `Program.cs` |
| Intervalo | 15 | Pausa |
| Prática | 60 | Cadastrar novas contas, testar fronteiras de username/senha/espaços e fazer login real |
| Entrega | 30 | Demonstrar criação, normalização, duplicidade e bloqueio de `Caixa` (`A24.01`–`A24.06`) |

---

## 1. O Poder da Arquitetura + `ComboBox` Vinculado a `Enum` (`A24.01` e `A24.04`)

Repare como `FormUsuarios` tem menos de 30 linhas! Toda a lógica pesada (permissão de admin, normalização do login, política de 12–128 chars sem `Trim()`, validação de enum e **Hash + Salt** com `PasswordHasher`) já mora em `AutenticacaoService.CriarAsync` (Aula 14).

*(Aplicação prática direta do padrão de `ComboBox` fechado estudado na [Aula 10 — Seção 2.6](10-Componentes-Windows-Forms.md):)*
- **`DropDownStyle = ComboBoxStyle.DropDownList`:** impede digitação livre de perfis inventados.
- **`DataSource = Enum.GetValues<Perfil>()`:** carrega automaticamente todos os valores de `enum Perfil` (`Administrador`, `Caixa`).
- **`(Perfil)perfil.SelectedItem!`:** converte o `object?` selecionado de volta para `Perfil`.
- Mesmo com o `ComboBox` travado na UI, `AutenticacaoService` continua validando `Enum.IsDefined(perfil)` para proteger contra chamadas diretas inválidas (ex.: `(Perfil)999`)!

---

## 2. Código Completo e Compilável desta Etapa (3 Arquivos Finais do App)

### Arquivo 1: `Views/FormUsuarios.cs`

```csharp
using SupermercadoApp.Models;
using SupermercadoApp.Services;

namespace SupermercadoApp.Views;

public sealed class FormUsuarios : Form
{
    public FormUsuarios(Sessao sessao, AutenticacaoService usuarios)
    {
        Permissoes.ExigirAdministrador(sessao);

        var p = Ui.Preparar(this, "Administração — Cadastrar usuário", 440, 400);
        var nome = Ui.Texto(p, "Usuário (letras sem acentos, números ou ponto)");
        var senha = Ui.Texto(p, "Senha (12 a 128 caracteres)", senha: true);

        p.Controls.Add(Ui.Rotulo("Perfil de acesso"));
        var perfil = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            DataSource = Enum.GetValues<Perfil>(),
            Width = 260
        };
        p.Controls.Add(perfil);

        Ui.Botao(p, "Cadastrar", async (sender, _) =>
        {
            var b = (Button)sender!;
            b.Enabled = false;
            try
            {
                await Ui.ExecutarAsync(async () =>
                {
                    await usuarios.CriarAsync(sessao, nome.Text, senha.Text, (Perfil)perfil.SelectedItem!);
                    // Só limpa os campos após sucesso (se houver erro 23505 ou validação, preserva para correção!):
                    senha.Clear();
                    nome.Clear();
                    MessageBox.Show("Usuário cadastrado com sucesso.");
                });
            }
            finally { b.Enabled = true; }
        });
    }
}
```

### Arquivo 2: `Views/FormAdministrador.cs` (Menu Administrativo Completo)

```csharp
using SupermercadoApp.Models;
using SupermercadoApp.Services;

namespace SupermercadoApp.Views;

public sealed class FormAdministrador : Form
{
    public FormAdministrador(Sessao sessao, ProdutoService produtos, AutenticacaoService usuarios, VendaService vendas)
    {
        Permissoes.ExigirAdministrador(sessao);

        var p = Ui.Preparar(this, "MarketFlow — Administração", 480, 350);
        p.Controls.Add(Ui.Rotulo($"Administrador: {sessao.Username}"));

        Ui.Botao(p, "Produtos e estoque", (_, _) =>
        {
            using var f = new FormProdutos(sessao, produtos);
            f.ShowDialog(this);
        });
        Ui.Botao(p, "Cadastrar usuário", (_, _) =>
        {
            using var f = new FormUsuarios(sessao, usuarios);
            f.ShowDialog(this);
        });
        Ui.Botao(p, "Relatório de vendas", (_, _) =>
        {
            using var f = new FormRelatorio(sessao, vendas);
            f.ShowDialog(this);
        });
        Ui.Botao(p, "Sair / trocar usuário", (_, _) => Close());
    }
}
```

### Arquivo 3: `Program.cs` (Composição Definitiva de Todo o MarketFlow)

```csharp
using System.Globalization;
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
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("pt-BR");
        ApplicationConfiguration.Initialize();

        try
        {
            using var banco = Banco.Criar();
            var usuarios = new UsuarioRepository(banco);
            var produtos = new ProdutoRepository(banco);
            var autenticacao = new AutenticacaoService(usuarios);
            var produtoService = new ProdutoService(produtos);
            var vendaService = new VendaService(new VendaRepository(banco), produtos);

            if (args.Contains("--preparar-admin"))
            {
                using var preparacao = new FormLogin(autenticacao, prepararAdmin: true);
                preparacao.ShowDialog();
                return;
            }

            while (true)
            {
                using var login = new FormLogin(autenticacao);
                if (login.ShowDialog() != DialogResult.OK || login.SessaoAutenticada is not { } sessao)
                    break;

                using Form principal = sessao.Perfil == Perfil.Administrador
                    ? new FormAdministrador(sessao, produtoService, autenticacao, vendaService)
                    : new FormCaixa(sessao, vendaService);

                principal.ShowDialog();
            }
        }
        catch (Exception e)
        {
            System.Diagnostics.Trace.WriteLine($"Falha ao iniciar: {e.GetType().Name}");
            MessageBox.Show(e is InvalidOperationException
                ? e.Message
                : "Falha ao iniciar. Verifique a configuração do banco.");
        }
    }
}
```

---

## 3. Verificação das Regras de Fronteira e Fluxo Completo (`A24.01`–`A24.06`)

Execute com **`F5`** (sem nunca registrar senhas reais nas evidências!):
1. **`A24.02` (username e unicidade):** teste `2`, `3`, `50` e `51` chars, acento `"joão"` (recusa), ponto `"caixa.novo"` (aceita) e `"CAIXA.NOVO"` (recusa por unicidade `23505`).
2. **`A24.03` (senha e espaços):** teste `11`, `12`, `128`, `129` chars e senha fictícia com espaços nas bordas comprovando login sem `Trim()`.
3. **`A24.05` (login real e permissão):** entre com `caixa.novo`, confirme que abre **Caixa** e não cria contas pelo serviço; confira no SQL Editor:
   ```sql
   SELECT username, perfil FROM usuario ORDER BY id;
   ```
4. **`A24.06` (evolução):** explique por que sair renova a sessão e o que seria necessário para revogação imediata ou recuperação de senha esquecida.

[Anterior: Aula 23](23-Relatorio-de-Vendas.md) · [Próxima: Aula 25 — Testes e publicação](25-Testes-e-Publicacao.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 24 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-24). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
