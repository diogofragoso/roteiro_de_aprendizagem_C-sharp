# Aula 10 — Sessão, Autorização por Perfil e Ciclo de Vida de Janelas

**Duração:** 3h30. **Pré-requisito:** [Aula 09](09-Autenticacao-Segura.md). **Entrega:** menus mínimos separados para `Administrador` e `Caixa`, verificação obrigatória de permissão fora da interface e laço de sessão no `Program.cs` permitindo trocar de usuário sem vazar dados da sessão anterior.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Demonstrar login do administrador criado na Aula 09 |
| Explicação | 30 | Autenticação vs Autorização, layout em código (`FlowLayoutPanel`), lambdas e `ShowDialog` |
| Construção | 60 | Expandir `Ui.cs` e construir `FormAdministrador` e `FormCaixa` mínimos |
| Intervalo | 15 | Pausa |
| Prática | 60 | Construir o laço de sessão no `Program.cs`, testar perfis e acesso negado |
| Entrega | 30 | Demonstrar 3 ciclos completos de troca de usuário e negação no serviço |

---

## 1. Autenticação vs Autorização — Por Que Esconder um Botão Não Basta (`A10.01` e `A10.02`)

Dois conceitos que nunca devem ser confundidos em segurança de software:

- **Autenticação ("Quem é você?"):** provar identidade com usuário e senha (Aula 09).
- **Autorização ("O que você tem permissão para fazer?"):** decidir se a sessão autenticada pode executar determinada operação (Aula 10 em diante).

### Matriz de Permissões do MarketFlow

| Ação no Sistema | Administrador | Caixa |
| --- | --- | --- |
| Gerenciar produtos e repor estoque | Permitido | Negado |
| Consultar relatório de vendas e cadastrar usuários | Permitido | Negado |
| Montar carrinho e finalizar vendas | Negado | Permitido |
| Sair / trocar de usuário | Permitido | Permitido |

*(Decisão didática do curso: se a mesma pessoa atua como caixa e administrador, utiliza duas contas separadas para manter a rastreabilidade de auditoria.)*

**Por que chamamos `Permissoes.ExigirAdministrador(sessao)` dentro dos Serviços e dos Construtores das janelas?**
Apenas esconder um botão no menu não protege a regra de negócio: se um futuro formulário ou teste chamar o método diretamente com uma sessão de `Caixa`, a camada de serviço barra imediatamente lançando `UnauthorizedAccessException`. *(Lembre-se também do limite de laboratório discutido na Aula 04: em distribuição comercial, essa autorização precisa viver em uma API no servidor.)*

---

## 2. Entendendo a Transição: Do Designer Visual ao Layout em Código (`Ui.cs`) (`A10.03`)

Nas Aulas 05–09 você construiu `FormLogin` usando o **Designer visual** do Visual Studio 2022. A partir desta aula, as telas internas (`FormAdministrador`, `FormCaixa`, `FormProdutos`, `FormRelatorio`, `FormUsuarios`) serão construídas em código C# limpo com apoio de métodos auxiliares em **`Views/Ui.cs`** e do componente **`FlowLayoutPanel`**.

**Por que aprender também a criar telas em código?**
1. Você enxerga **exatamente** o que o Designer do Visual Studio 2022 faz por baixo dos panos no `.Designer.cs` — não existe mágica!
2. Um `FlowLayoutPanel` com `FlowDirection = FlowDirection.TopDown` e `AutoScroll = true` empilha controles automaticamente de cima para baixo com barra de rolagem, sem que botões fiquem sobrepostos em monitores com escalas diferentes.

### Quadro de Equivalência Direta (Designer vs Código C#)

| Ação no Designer do VS 2022 | Código C# equivalente | O que acontece na memória? |
| --- | --- | --- |
| Arrastar um `Button` da Caixa de Ferramentas | `var botao = new Button();` | Instancia o objeto do controle visual |
| Editar `Text` na janela Propriedades (`F4`) | `botao.Text = "Sair";` | Define o rótulo exibido ao usuário |
| Soltar o botão dentro da janela ou painel | `painel.Controls.Add(botao);` | Vincula o controle ao contêiner pai para ser desenhado |
| Duplo clique no botão (aba de raio ⚡) | `botao.Click += AoSair;` | Inscreve um método manipulador no evento `Click` |

### Dissecando Expressões Lambda de Evento: `(_, _) => Close()`
Em C#, quando um evento exige `(object? sender, EventArgs e)` mas não precisamos usar nenhum dos dois argumentos, podemos escrever uma função curta (**expressão lambda**):

```csharp
// Forma curta (lambda com descartes '_'):
botao.Click += (_, _) => Close();

// Forma tradicional 100% equivalente (método nomeado):
void AoSair(object? sender, EventArgs e)
{
    Close();
}
botao.Click += AoSair;
```
- O sublinhado `_` se chama **descarte (*discard*)**: informa ao compilador *"sei que o evento passa `sender` e `e`, mas não vou ler esses parâmetros aqui"*.

---

## 3. Ciclo de Vida de Janelas: Por Que Usar `while (true)`, `ShowDialog()` e `using`? (`A10.04`)

Imagine um supermercado onde o turno da manhã termina: o caixa João clica em **Sair / trocar usuário**, e a caixa Maria precisa fazer login no mesmo computador.

- **Por que NÃO usar uma variável estática global `SessaoAtual`?** Variáveis globais costumam "esquecer" dados do usuário anterior na memória.
- **Como fazemos de forma limpa e segura no `Program.cs`?**
  1. Um laço `while (true)` controla o ciclo de sessões.
  2. Dentro dele, `using var login = new FormLogin(autenticacao);` cria uma tela de login **nova e limpa**.
  3. `login.ShowDialog()` **pausa o `Program.cs`** até o usuário fechar o login ou autenticar (`DialogResult.OK`).
  4. Se o usuário clicou no `X` para sair (ou `SessaoAutenticada` for `null`), executamos `break;` e o aplicativo encerra de verdade sem deixar processos fantasma!
  5. Se o login deu certo, escolhemos a janela com o operador ternário `? :` (`sessao.Perfil == Perfil.Administrador ? new FormAdministrador(sessao) : new FormCaixa(sessao)`), abrimos com `using Form principal = ...` e `principal.ShowDialog()`.
  6. Quando o usuário fecha a janela principal (clicando em **Sair / trocar usuário**), o `using` **destrói completamente** a janela e o laço volta ao início abrindo um novo `FormLogin` limpo!

---

## 4. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

### Passo 1 — Expandir `Views/Ui.cs` com Helpers de Layout
Abra **`Views/Ui.cs`** (onde você criou `ExecutarAsync` na Aula 09) e adicione estes três métodos públicos estáticos:
1. **`Preparar(Form form, string titulo, int largura = 960, int altura = 650)` → retorna `FlowLayoutPanel`:**
   - Configure `form.Text = titulo;`, `form.Size = new Size(largura, altura);` e `form.StartPosition = FormStartPosition.CenterScreen;`.
   - Crie um `FlowLayoutPanel` com `Dock = DockStyle.Fill`, `AutoScroll = true`, `FlowDirection = FlowDirection.TopDown`, `WrapContents = false` e `Padding = new Padding(16)`.
   - Adicione esse painel ao formulário (`form.Controls.Add(painel);`) e retorne-o.
2. **`Rotulo(string texto)` → retorna `Label`:**
   - Crie e retorne um `Label` com `Text = texto`, `AutoSize = true` e `Margin = new Padding(3, 8, 3, 3)`.
3. **`Botao(FlowLayoutPanel painel, string texto, EventHandler clique)` → retorna `Button`:**
   - Crie um `Button` com `Text = texto` e `AutoSize = true`.
   - Associe o evento **uma única vez**: `botao.Click += clique;`.
   - Adicione `botao` ao `painel` e retorne-o.

### Passo 2 — Construir `Views/FormAdministrador.cs` e `Views/FormCaixa.cs`
Crie ambos os arquivos na pasta `Views/` (`Shift+Alt+C`), em `namespace SupermercadoApp.Views;` (com `using SupermercadoApp.Models;` e `using SupermercadoApp.Services;`):

1. **`FormAdministrador.cs`:**
   - Declare `public sealed class FormAdministrador : Form` (herda de `Form`!).
   - Construtor recebendo **`Sessao sessao`**:
     - **Primeira linha obrigatória:** `Permissoes.ExigirAdministrador(sessao);`.
     - Chame `var painel = Ui.Preparar(this, "MarketFlow — Administração", 480, 300);`.
     - Adicione um rótulo de boas-vindas com `sessao.Username`.
     - Adicione um botão `"Sair / trocar usuário"` que chame `Close()`.
     - **Atividade `A10.03`:** teste escrever o manipulador desse botão **das duas formas** — primeiro com método nomeado (`void AoSair(object? sender, EventArgs e) { Close(); }`) e depois com lambda `(_, _) => Close()`, confirmando que ambos fecham a janela com um único clique.
2. **`FormCaixa.cs`:**
   - Declare `public sealed class FormCaixa : Form` com construtor recebendo **`Sessao sessao`**.
   - **Primeira linha obrigatória:** `Permissoes.ExigirCaixa(sessao);`.
   - Prepare a tela com título `"MarketFlow — Caixa"`, rótulo com `sessao.Username` e botão `"Sair / trocar usuário"` chamando `Close()`.

### Passo 3 — Atualizar o Laço de Sessão no `Program.cs`
Substitua a abertura única do login da Aula 09 pelo ciclo completo:
1. Mantenha o modo `--preparar-admin` **fora** do laço: se `args.Contains("--preparar-admin")`, abra `FormLogin(autenticacao, true)` com `ShowDialog()` e encerre com `return;`.
2. Abaixo dele, construa o laço `while (true)` explicado na Seção 3:
   - Abra `using var login = new FormLogin(autenticacao);`.
   - Se `login.ShowDialog() != DialogResult.OK || login.SessaoAutenticada is not { } sessao`, execute `break;`.
   - Instancie `using Form principal = ...` escolhendo `FormAdministrador` ou `FormCaixa` conforme `sessao.Perfil` e chame `principal.ShowDialog();`.

---

## 5. Preparação da Conta Caixa e Verificações Obrigatórias (`A10.01`–`A10.04`)

> [!NOTE]
> **Como obter uma conta de `Caixa` antes da Aula 19?** Não crie botões provisórios de cadastro no seu projeto. O professor preparará a conta `Caixa` de teste no banco da sua dupla usando a ferramenta docente `--preparar-caixa` descrita em [ensino/README.md](ensino/README.md).

Execute com **`F5`** no Visual Studio 2022 e verifique:
1. Entrar como Administrador abre apenas **"Administração"**; entrar como Caixa abre apenas **"Caixa"**.
2. **Teste de autorização independente de menu (`A10.02`):** no depurador do VS 2022 (ou em um teste temporário antes de abrir a janela), chame `Permissoes.ExigirAdministrador(sessaoDeCaixa)` e comprove o lançamento de `UnauthorizedAccessException`. Remova qualquer linha temporária após testar.
3. **Três ciclos completos (`A10.04`):** faça `Admin → Sair → Caixa → Sair → Admin → Sair → fechar Login pelo X`. Confira que a senha volta sempre vazia e que o aplicativo encerra completamente no Visual Studio 2022 (o botão vermelho *Parar* volta a virar verde *Iniciar*).

[Anterior: Aula 09](09-Autenticacao-Segura.md) · [Próxima: Aula 11 — Modelo de produtos](11-Modelo-de-Produtos.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 10 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-10). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
