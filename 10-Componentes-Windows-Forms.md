# Aula 10 — Componentes do Windows Forms (WFA) na Prática: Do Designer ao Código

**Duração:** 3h30. **Pré-requisito:** [Aula 09](09-Configuracao-Ambiente.md). **Entrega:** projeto de laboratório `LabComponentesWFA` no Visual Studio 2022 demonstrando cada componente essencial do Windows Forms (`Form`, `Label`, `TextBox`, `MaskedTextBox`, `Button`, `NumericUpDown`, `ComboBox`, `ListBox`, `CheckBox`, `RadioButton`, `GroupBox`, `DateTimePicker`, `DataGridView`, `FlowLayoutPanel`, `MessageBox` e `ErrorProvider`) em um **Mini-CRUD em memória** funcionando de ponta a ponta antes de iniciarmos as telas definitivas do MarketFlow.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Revisar Caixa de Ferramentas (`Ctrl+Alt+X`), Propriedades (`F4`) e Eventos (⚡) da Aula 09 |
| Explicação | 30 | Hierarquia de `Control`, ciclo de vida do `Form` e mapa de todos os componentes WFA |
| Construção | 60 | Explorar com exemplos práticos entrada de texto, números, listas, opções, datas e layout |
| Intervalo | 15 | Pausa |
| Prática | 60 | Construir o Mini-CRUD em memória com `DataGridView`, `DataBoundItem` e `MessageBox` |
| Entrega | 30 | Demonstrar todos os controles, testes de validação (`A10.01`–`A10.05`) e registrar evidências |

---

## 1. Por Que Dominar Cada Componente WFA Antes de Fazer o CRUD do Mercado?

Na [Aula 09](09-Configuracao-Ambiente.md), você criou a estrutura da solução Windows Forms no Visual Studio 2022. A partir da [Aula 11](11-Criando-Interface-Login.md), começaremos a construir as telas reais do **MarketFlow** (Login, Menus por Perfil, CRUD de Produtos, Reposição de Estoque, Frente de Caixa, Relatório de Vendas e Cadastro de Usuários).

No Windows Forms (**WFA — Windows Forms Application**), **todo elemento visual herda da classe `Control`**. Isso significa que existem propriedades universais presentes em quase todos os componentes:

| Propriedade Universal | Tipo | Para que serve na prática? |
| --- | --- | --- |
| **`(Name)`** | identificador C# | Nome da variável do controle no código (ex.: `txtNome`, `btnSalvar`, `dgvProdutos`). Sem espaços nem acentos. |
| **`Text`** | `string` | Texto visível para o usuário (título da janela, texto do botão, conteúdo digitado na caixa). |
| **`Enabled`** | `bool` | Quando `false`, o controle fica acinzentado e bloqueia cliques/digitação (essencial contra duplo clique!). |
| **`Visible`** | `bool` | Quando `false`, oculta o controle da tela (sem removê-lo da memória). |
| **`TabIndex`** | `int` | Ordem em que o cursor avança quando o operador pressiona a tecla `Tab`. |
| **`Dock` / `Anchor`** | `DockStyle` / `AnchorStyles` | Controla como o controle se estica ou acompanha o redimensionamento da janela. |
| **`Controls`** | `ControlCollection` | Lista de controles filhos dentro de um contêiner (`Form`, `Panel`, `GroupBox`, `FlowLayoutPanel`). Um controle criado com `new` **só aparece na tela** depois de adicionado com `pai.Controls.Add(controle)`! |

### Convenção de Prefixos Usada em Todo o Guia

| Prefixo | Componente WFA | Exemplo de `(Name)` | Onde usaremos no MarketFlow |
| --- | --- | --- | --- |
| `frm` | `Form` | `FormLogin`, `FormProdutos` | Todas as janelas do sistema |
| `lbl` | `Label` | `lblTotal`, `lblStatus` | Títulos, rótulos e resumo de vendas |
| `txt` | `TextBox` | `txtUsuario`, `txtCodigo` | Login, busca, código e nome de produto |
| `msk` | `MaskedTextBox` | `mskCpf`, `mskTelefone` | Campos com máscara fixa (CPF, CEP, telefone) |
| `btn` | `Button` | `btnEntrar`, `btnSalvar` | Todas as ações e comandos |
| `num` | `NumericUpDown` | `numPreco`, `numQuantidade` | Preço (`decimal`), estoque, carrinho e troco |
| `cmb` | `ComboBox` | `cmbPerfil`, `cmbCategoria` | Escolha fechada de perfil (`Enum`) ou categoria |
| `lst` | `ListBox` | `lstHistorico` | Listas simples visíveis de itens |
| `chk` | `CheckBox` | `chkAtivo`, `chkEmitirRecibo` | Opções verdadeiro/falso independentes |
| `rad` | `RadioButton` | `radDinheiro`, `radCartao` | Escolha única dentro de um `GroupBox` |
| `dtp` | `DateTimePicker` | `dtpInicio`, `dtpFim` | Filtros de data no relatório de vendas |
| `dgv` | `DataGridView` | `dgvProdutos`, `dgvCarrinho` | Grades de consulta, seleção CRUD e carrinho |
| `pnl` / `flp` / `grp` | `Panel` / `FlowLayoutPanel` / `GroupBox` | `flpAcoes`, `grpPagamento` | Organização de layout e bloqueio em lote |

---

## 2. Catálogo Completo dos Componentes WFA com Exemplos Práticos

Percorra cada componente abaixo testando os trechos de código no Visual Studio 2022. Note que **tudo o que fazemos no Designer (`F4`) tem um equivalente exato em código C#** — você usará o Designer na [Aula 11](11-Criando-Interface-Login.md) (`FormLogin`) e também construção via código com `Views/Ui.cs` a partir da [Aula 15](15-Sessao-e-Perfis.md).

---

### 2.1 `Form` — A Janela Base e Seu Ciclo de Vida

O `Form` é o contêiner raiz de cada tela (`public partial class FormExemplo : Form`).

- **Propriedades essenciais:** `Text` (título da barra), `ClientSize` (área útil interna), `StartPosition = FormStartPosition.CenterScreen`, `FormBorderStyle = FormBorderStyle.FixedSingle`, `MaximizeBox = false`, `AcceptButton` (acionado com `Enter`) e `CancelButton` (acionado com `Esc`).
- **Abertura de janelas (`Show()` vs `ShowDialog()`):**
  - `tela.Show()`: abre uma janela paralela sem travar a janela de origem.
  - **`using var tela = new FormProdutos(...); tela.ShowDialog(this);`**: abre como **diálogo modal** (trava a janela de trás até fechar) e devolve um **`DialogResult`** (`OK`, `Cancel`, `Yes`, `No`). É o padrão usado no MarketFlow para Login, Produtos, Relatório e Usuários!
- **Eventos fundamentais do ciclo de vida:**
  - `Load`: dispara antes de a janela ficar visível.
  - **`Shown`**: dispara logo depois que a janela já apareceu na tela — **ideal para carregar dados assíncronos do banco (`async/await`)** sem travar a abertura!
  - **`FormClosing`**: dispara antes de fechar — permite barrar o fechamento com **`e.Cancel = true`** se houver dados não salvos ou operação em andamento (usaremos no `FormCaixa` na Aula 22!).

```csharp
// Exemplo prático de configuração e eventos do Form
public FormExemplo()
{
    InitializeComponent();

    Text = "Painel de Demonstração WFA";
    StartPosition = FormStartPosition.CenterScreen;
    AcceptButton = btnSalvar; // Pressionar Enter aciona btnSalvar

    // Carregamento assíncrono seguro assim que a tela aparecer:
    Shown += async (_, _) =>
    {
        lblStatus.Text = "Carregando dados iniciais...";
        await Task.Delay(300); // Simula leitura de banco
        lblStatus.Text = "Pronto para uso.";
    };

    // Confirmação antes de fechar a janela pelo botão X:
    FormClosing += (_, e) =>
    {
        var resposta = MessageBox.Show(
            "Deseja realmente sair desta tela?",
            "Confirmar saída",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (resposta != DialogResult.Yes)
            e.Cancel = true; // Cancela o fechamento!
    };
}
```

---

### 2.2 `Label` — Rótulos Fixos e Indicadores Dinâmicos

Exibe textos informativos que o usuário **não digita diretamente** (identificação de campos, avisos de status, total da compra).

- **Propriedades principais:** `Text`, `AutoSize = true` (ajusta a largura automaticamente ao texto), `Font`, `ForeColor`, `Margin`.
- **Boa prática de acessibilidade:** mantenha sempre um `Label` visível acima ou ao lado de cada campo de entrada, e nunca dependa apenas de cor vermelha/verde para avisar erros — escreva sempre uma mensagem clara em `lblStatus.Text`.

```csharp
// Criando no código (ou configurando no Designer via F4):
var lblTotal = new Label
{
    Name = "lblTotal",
    Text = "Total previsto: R$ 0,00",
    AutoSize = true,
    Font = new Font("Segoe UI", 11, FontStyle.Bold),
    ForeColor = Color.DarkGreen
};

// Atualizando dinamicamente durante o uso:
decimal total = 60.30m;
lblTotal.Text = $"Total previsto: {total:C}"; // Exibe "Total previsto: R$ 60,30"
```

---

### 2.3 `TextBox` e `MaskedTextBox` — Entrada de Texto, Senha e Máscaras

#### `TextBox`
Recebe texto livre digitado pelo usuário (usuário de login, código de barras `"000123"`, nome do produto, termo de busca).

- **Propriedades essenciais:**
  - `Text`: lê ou altera o texto atual (use `.Trim()` ao validar textos comuns, mas **nunca** em senhas!).
  - `MaxLength`: limita o número de caracteres digitáveis já alinhado ao `varchar(N)` do banco (ex.: `50` para username, `30` para código).
  - `UseSystemPasswordChar = true`: oculta a digitação com bolinhas/asteriscos em campos de senha.
  - `PlaceholderText`: dica cinza exibida enquanto o campo está vazio (ex.: `"Digite nome ou código..."`).
  - `ReadOnly = true`: permite selecionar e copiar o texto, mas impede edição.
  - `Multiline = true` + `ScrollBars = ScrollBars.Vertical`: caixa de várias linhas para observações.
- **Métodos e eventos úteis:** `txtCodigo.Clear()`, `txtCodigo.Focus()`, evento `TextChanged` (dispara a cada tecla digitada) e `KeyDown` (detecta `e.KeyCode == Keys.Enter`).

```csharp
// Exemplo prático: configurando e lendo TextBox
txtCodigo.MaxLength = 30;
txtCodigo.PlaceholderText = "Ex.: 000123";

txtSenha.UseSystemPasswordChar = true;
txtSenha.MaxLength = 128;

// Ao validar um campo de texto comum:
string codigo = txtCodigo.Text.Trim();
if (string.IsNullOrWhiteSpace(codigo))
{
    MessageBox.Show("Informe o código do produto.");
    txtCodigo.Focus(); // Devolve o cursor para o campo com problema
    return;
}

// Exemplo com evento TextChanged (contador de caracteres em tempo real):
txtNome.TextChanged += (_, _) =>
{
    lblContador.Text = $"{txtNome.Text.Length}/120 caracteres";
};
```

#### `MaskedTextBox`
Semelhante ao `TextBox`, mas impõe uma **máscara fixa** de digitação (`0` = dígito obrigatório, `9` = dígito opcional, `L` = letra obrigatória).

- **Quando usar `MaskedTextBox` vs `TextBox`?**
  - Use **`MaskedTextBox`** para formatos padronizados: CPF (`"000.000.000-00"`), CEP (`"00000-000"`), telefone (`"(00) 00000-0000"`).
  - Use **`TextBox`** comum para código de barras/SKU (`"000123"`, `"EAN-789..."`), pois produtos podem ter tamanhos variados de código!

```csharp
// Exemplo prático de MaskedTextBox para CPF
mskCpf.Mask = "000.000.000-00";
mskCpf.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals; // .Text devolve só os 11 dígitos

if (!mskCpf.MaskCompleted)
{
    MessageBox.Show("Preencha todos os 11 dígitos do CPF.");
    return;
}
string cpfLimpo = mskCpf.Text; // Ex.: "12345678901"
```

---

### 2.4 `Button` — Botões de Comando e Proteção Contra Duplo Clique

Dispara uma ação quando clicado com o mouse ou acionado via `Enter`/`Espaço`.

- **Propriedades essenciais:** `(Name)`, `Text`, `Enabled`, `AutoSize = true`, `TabIndex`.
- **O padrão profissional de clique assíncrono no MarketFlow (`A13.05`, `A18.06`):**
  Sempre que um botão dispara uma operação que acessa banco ou rede, **desabilite o botão antes do `try` e reabilite no `finally`** para impedir envios duplicados:

```csharp
// Forma 1 — Evento tradicional criado pelo Designer (duplo clique no botão):
private async void btnSalvar_Click(object sender, EventArgs e)
{
    btnSalvar.Enabled = false; // Bloqueia segundo clique imediato
    try
    {
        await Task.Delay(500); // Simula gravação assíncrona no banco
        MessageBox.Show("Registro salvo com sucesso!");
    }
    catch (Exception erro)
    {
        MessageBox.Show($"Erro ao salvar: {erro.Message}");
    }
    finally
    {
        btnSalvar.Enabled = true; // SEMPRE reabilita, dando certo ou errado!
    }
}

// Forma 2 — Evento via expressão lambda (usado em Views/Ui.cs a partir da Aula 15):
btnLimpar.Click += (sender, _) =>
{
    var botaoClicado = (Button)sender!; // Recupera o próprio Button que disparou o clique
    txtNome.Clear();
    lblStatus.Text = $"Campos limpos pelo botão '{botaoClicado.Text}'.";
};
```

---

### 2.5 `NumericUpDown` — Entrada Numérica Segura (Inteiros e Dinheiro)

**Por que NÃO usar `TextBox` para preço e quantidade no CRUD?**
Em um `TextBox`, o usuário pode digitar `"abc"`, `"12..50"` ou `"-9"`, obrigando você a tratar dezenas de erros de conversão manual. O **`NumericUpDown`** já restringe a entrada a números dentro de limites mínimo e máximo, com setas para incrementar/decrementar!

- **Detalhe crucial de tipo:** a propriedade **`Value`** do `NumericUpDown` **sempre retorna `decimal`** — exatamente o tipo correto para dinheiro em C#! Para quantidades inteiras, fazemos o cast explícito `(int)numQuantidade.Value`.

| Cenário no MarketFlow | `DecimalPlaces` | `Minimum` | `Maximum` | `Increment` | Como ler no C# |
| --- | :---: | :---: | :---: | :---: | --- |
| **Preço do Produto** (`FormProdutos`) | `2` | `0` | `99999999.99m` | `0.50m` | `decimal preco = numPreco.Value;` |
| **Quantidade de Reposição / Carrinho** (`FormProdutos`, `FormCaixa`) | `0` | `1` | `1000000` | `1` | `int qtd = (int)numQuantidade.Value;` |
| **Valor Recebido no Caixa** (`FormCaixa`) | `2` | `0` | `9999999999.99m` | `1.00m` | `decimal recebido = numRecebido.Value;` |

```csharp
// Exemplo prático: configurando dois NumericUpDown e calculando subtotal
var numPreco = new NumericUpDown
{
    DecimalPlaces = 2,
    Minimum = 0m,
    Maximum = 99999999.99m,
    Increment = 0.50m,
    ThousandsSeparator = true,
    Width = 180
};

var numQuantidade = new NumericUpDown
{
    DecimalPlaces = 0,
    Minimum = 1,
    Maximum = 1000000,
    Value = 1,
    Width = 120
};

// Calculando subtotal em tempo real quando qualquer um dos dois mudar:
void AtualizarSubtotal(object? sender, EventArgs e)
{
    decimal preco = numPreco.Value;
    int quantidade = (int)numQuantidade.Value;
    decimal subtotal = preco * quantidade;
    lblSubtotal.Text = $"Subtotal: {subtotal:C}";
}

numPreco.ValueChanged += AtualizarSubtotal;
numQuantidade.ValueChanged += AtualizarSubtotal;
```

---

### 2.6 `ComboBox` e `ListBox` — Listas de Seleção Fechada e Múltipla

#### `ComboBox` (Lista Suspensa)
Ideal quando o usuário precisa escolher **uma opção dentre várias** sem ocupar muito espaço vertical (ex.: categoria do produto, unidade de medida ou **Perfil do Usuário** na Aula 24).

- **Armadilha clássica:** o estilo padrão do `ComboBox` é `DropDown`, que **deixa o usuário digitar texto inventado** (ex.: `"Gerentee"`). Para listas controladas de CRUD, configure SEMPRE **`DropDownStyle = ComboBoxStyle.DropDownList`**!

```csharp
// Exemplo 1 — Lista de textos fixos:
cmbCategoria.DropDownStyle = ComboBoxStyle.DropDownList;
cmbCategoria.Items.AddRange(new object[] { "Mercearia", "Bebidas", "Limpeza", "Hortifrúti" });
cmbCategoria.SelectedIndex = 0; // Já seleciona "Mercearia" por padrão

// Lendo a escolha:
string categoria = cmbCategoria.SelectedItem?.ToString() ?? "";

// Exemplo 2 — Vinculando diretamente a um Enum (exatamente como faremos em FormUsuarios!):
public enum PerfilAcesso { Administrador, Caixa }

cmbPerfil.DropDownStyle = ComboBoxStyle.DropDownList;
cmbPerfil.DataSource = Enum.GetValues<PerfilAcesso>();

// Lendo com cast seguro ao clicar em Cadastrar:
PerfilAcesso perfilEscolhido = (PerfilAcesso)cmbPerfil.SelectedItem!;

// Evento disparado toda vez que o usuário troca a opção:
cmbCategoria.SelectedIndexChanged += (_, _) =>
{
    lblStatus.Text = $"Categoria selecionada: {cmbCategoria.SelectedItem}";
};
```

#### `ListBox` (Lista Visível de Itens)
Exibe vários itens ao mesmo tempo dentro de uma caixa rolável (útil para históricos curtos ou seleção múltipla com `SelectionMode = SelectionMode.MultiSimple`).

- **Quando usar qual lista?**
  - **1 coluna compacta (escolher 1 opção):** `ComboBox` (`DropDownList`).
  - **1 coluna visível (ou selecionar vários itens simples):** `ListBox`.
  - **Várias colunas estruturadas (Código, Nome, Preço, Estoque, Ativo):** **`DataGridView`** (Seção 2.9)!

```csharp
// Exemplo prático de ListBox
lstHistorico.Items.Add("10:15 - Produto 000123 adicionado");
lstHistorico.Items.Add("10:16 - Preço atualizado para R$ 25,90");

if (lstHistorico.SelectedItem is string linha)
{
    MessageBox.Show($"Log selecionado: {linha}");
}
```

---

### 2.7 `CheckBox`, `RadioButton` e `GroupBox` — Marcações e Escolhas Exclusivas

- **`CheckBox`:** cada caixa é **independente** (`true` ou `false`). O usuário pode marcar nenhuma, uma ou várias ao mesmo tempo.
- **`RadioButton`:** escolhas **mutuamente exclusivas** — marcar um desmarca automaticamente os outros **que estiverem no mesmo contêiner** (`GroupBox` ou `Panel`).
- **`GroupBox`:** moldura com título (`Text`) que organiza visualmente campos relacionados **e isola grupos de `RadioButton`**! (Se você colocar 4 `RadioButton` soltos direto no `Form`, só poderá marcar 1 dos 4; separando em dois `GroupBox`, cada grupo tem sua própria escolha independente.)

```csharp
// Exemplo prático com CheckBox:
chkAtivo.Text = "Produto ativo para venda";
chkAtivo.Checked = true; // Padrão marcado

chkAtivo.CheckedChanged += (_, _) =>
{
    lblStatus.Text = chkAtivo.Checked ? "Status: Ativo" : "Status: Inativo";
};

// Exemplo prático com RadioButton dentro de um GroupBox (grpPagamento):
// (radDinheiro, radCartao e radPix ficam DENTRO de grpPagamento)
string ObterFormaPagamento()
{
    if (radDinheiro.Checked) return "Dinheiro (simulado)";
    if (radCartao.Checked) return "Cartão (simulado)";
    if (radPix.Checked) return "Pix (simulado)";
    return "Não selecionada";
}
```

---

### 2.8 `DateTimePicker` — Seleção Segura de Datas e Períodos

Permite escolher uma data pelo calendário visual do Windows, entregando diretamente um objeto **`DateTime`** válido (sem risco de o usuário digitar `"31/02/2026"`).

- **Propriedades principais:** `Format = DateTimePickerFormat.Short` (exibe `dd/MM/aaaa`), `Value` (data/hora selecionada), `MinDate` e `MaxDate`.
- **Dica crucial para relatórios (usada na Aula 23 — `FormRelatorio`):** `dtpInicio.Value` traz também a hora atual; use **`.Value.Date`** para zerar a hora em `00:00:00`!

```csharp
// Exemplo prático: filtro de período com dois DateTimePicker
dtpInicio.Format = DateTimePickerFormat.Short;
dtpInicio.Value = DateTime.Today;

dtpFim.Format = DateTimePickerFormat.Short;
dtpFim.Value = DateTime.Today;

// Validando o período ao clicar em "Filtrar":
DateTime dataInicio = dtpInicio.Value.Date;
DateTime dataFimInclusiva = dtpFim.Value.Date;

if (dataFimInclusiva < dataInicio)
{
    MessageBox.Show("A data final não pode ser anterior à data inicial.");
    return;
}

// Intervalo semiaberto [inicio, diaSeguinte) usado em consultas SQL profissionais:
DateTime fimExclusivo = dataFimInclusiva.AddDays(1);
lblPeriodo.Text = $"Consultando de {dataInicio:dd/MM/yyyy} até {dataFimInclusiva:dd/MM/yyyy}";
```

---

### 2.9 `DataGridView` — A Tabela de Dados (O Coração Visual do CRUD!)

Em quase todo CRUD Windows Forms (`FormProdutos`, `FormCaixa`, `FormRelatorio`), a **`DataGridView`** é o componente que lista os registros vindos da memória ou do banco de dados e permite selecionar uma linha para **Editar**, **Repor** ou **Inativar**.

#### Configuração Profissional Obrigatória para CRUD Controlado
Por padrão, a `DataGridView` permite que o usuário digite direto dentro das células e mostra uma linha vazia extra com `*` no final. Em sistemas comerciais, configuramos estas **6 propriedades** para garantir controle total:

```csharp
// Configuração padrão (exatamente o que o helper Ui.Grade fará na Aula 17):
dgvProdutos.ReadOnly = true;                                         // Impede edição solta na célula
dgvProdutos.AllowUserToAddRows = false;                              // Remove a linha vazia '*' do final
dgvProdutos.AllowUserToDeleteRows = false;                           // Evita apagar com a tecla Delete
dgvProdutos.MultiSelect = false;                                     // Seleciona apenas 1 registro por vez
dgvProdutos.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Seleciona a linha inteira
dgvProdutos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Estica colunas na largura
```

#### Como Carregar Dados (`DataSource`) e Ler a Linha Selecionada (`DataBoundItem`)
Quando você atribui `dgvProdutos.DataSource = lista.ToList();`, a grade cria uma coluna para cada propriedade pública do objeto (`Id`, `Codigo`, `Nome`, `Preco`, `Categoria`, `Ativo`).

> [!IMPORTANT]
> **Regra de ouro da `DataGridView`:** **NUNCA** recupere dados lendo células por número (`dgvProdutos.Rows[0].Cells[2].Value`)! Se alguém trocar a ordem das colunas, seu código quebra silenciosamente. Use sempre **`SelectedRows[0].DataBoundItem`** para recuperar o **objeto C# tipado original** daquela linha!

```csharp
// 1. Vincular / recarregar a grade (sempre com .ToList() para forçar atualização visual):
void AtualizarGrade()
{
    dgvProdutos.DataSource = null;
    dgvProdutos.DataSource = produtos.ToList();
    dgvProdutos.ClearSelection();
}

// 2. Ler o objeto selecionado com segurança antes de Editar ou Remover:
ProdutoDemo? ObterSelecionado()
{
    if (dgvProdutos.SelectedRows.Count == 0 ||
        dgvProdutos.SelectedRows[0].DataBoundItem is not ProdutoDemo item)
    {
        MessageBox.Show("Selecione um produto na tabela primeiro.");
        return null;
    }
    return item;
}
```

---

### 2.10 Contêineres de Layout e Componentes de Apoio Visual

Arrastar controles soltos em posições fixas `(X, Y)` funciona em telas pequenas como o Login, mas em telas maiores com botões dinâmicos ou resoluções diferentes os controles podem se sobrepor. Por isso o WFA oferece **contêineres de layout**:

| Contêiner / Componente | O que faz na prática? | Quando usar? |
| --- | --- | --- |
| **`FlowLayoutPanel`** | Empilha os controles filhos automaticamente na vertical (`TopDown`) ou lado a lado (`LeftToRight`) com rolagem (`AutoScroll = true`). | Telas de cadastro e barras de botões (`Ui.Preparar` e barra `acoes` em `FormProdutos` e `FormCaixa`). |
| **`Panel`** | Agrupa controles em uma área retangular; pode receber barra de rolagem ou desabilitar todos os filhos de uma vez (`painel.Enabled = false`). | Separar seções da tela e travar a área de edição durante uma operação de banco. |
| **`GroupBox`** | Igual ao `Panel`, mas desenha uma borda com título no topo e isola grupos de `RadioButton`. | Agrupar opções de pagamento, filtros de relatório ou dados de endereço. |
| **`TableLayoutPanel`** | Organiza controles em linhas e colunas fixas ou proporcionais (como uma grade CSS). | Formulários responsivos com rótulo à esquerda e campo alinhado à direita. |
| **`TabControl`** | Divide uma mesma janela em abas (`TabPage`), como *"Dados Gerais"* e *"Estoque"*. | Cadastros extensos com muitos campos. |
| **`MenuStrip` / `ToolStrip`** | Barra de menu superior (*Arquivo*, *Cadastros*, *Relatórios*) e barra de ícones rápidos. | Sistemas desktop com janela principal centralizada. |
| **`PictureBox`** | Exibe imagens (`Image`, `SizeMode = PictureBoxSizeMode.Zoom`). | Logotipo da empresa ou foto ilustrativa do produto. |
| **`ProgressBar`** | Barra de progresso (`Style = ProgressBarStyle.Marquee` para espera indeterminada ou `Minimum`/`Maximum`/`Value`). | Indicar carregamento ou exportação demorada. |

```csharp
// Exemplo prático: FlowLayoutPanel vertical + barra horizontal aninhada (padrão do MarketFlow!)
var painelPrincipal = new FlowLayoutPanel
{
    Dock = DockStyle.Fill,
    FlowDirection = FlowDirection.TopDown,
    WrapContents = false,
    AutoScroll = true,
    Padding = new Padding(16)
};

var barraBotoes = new FlowLayoutPanel
{
    FlowDirection = FlowDirection.LeftToRight, // Coloca os botões lado a lado!
    AutoSize = true,
    WrapContents = false
};

barraBotoes.Controls.Add(new Button { Text = "Novo", AutoSize = true });
barraBotoes.Controls.Add(new Button { Text = "Editar", AutoSize = true });
barraBotoes.Controls.Add(new Button { Text = "Salvar", AutoSize = true });
barraBotoes.Controls.Add(new Button { Text = "Remover", AutoSize = true });

painelPrincipal.Controls.Add(barraBotoes);
Controls.Add(painelPrincipal);

// Truque de produtividade: 1 única linha bloqueia TODOS os controles filhos de um contêiner!
barraBotoes.Enabled = false;
```

---

### 2.11 `MessageBox` e `ErrorProvider` — Diálogos e Feedback Visual

- **`MessageBox.Show(mensagem, titulo, botoes, icone)`:** abre uma janela de aviso ou pergunta modal e devolve um **`DialogResult`**.
  - Sempre use confirmação (`MessageBoxButtons.YesNo`) antes de ações destrutivas (excluir/inativar registro ou limpar carrinho!).
- **`ErrorProvider`:** componente não-visual (aparece na bandeja inferior do Designer) que desenha um ícone vermelho piscante com mensagem explicativa ao lado do campo inválido, sem interromper o usuário com vários pop-ups.

```csharp
// Exemplo 1 — Confirmação antes de remover/inativar (padrão obrigatório no CRUD):
var resposta = MessageBox.Show(
    $"Confirma a exclusão do produto '{item.Nome}'?",
    "Confirmar exclusão",
    MessageBoxButtons.YesNo,
    MessageBoxIcon.Warning);

if (resposta != DialogResult.Yes)
    return; // Usuário escolheu "Não" ou fechou a caixa → preserva os dados!

// Exemplo 2 — ErrorProvider destacando campo inválido sem abrir pop-up:
errorProvider1.Clear(); // Limpa avisos anteriores
if (string.IsNullOrWhiteSpace(txtNome.Text))
{
    errorProvider1.SetError(txtNome, "O nome do produto é obrigatório.");
    txtNome.Focus();
    return;
}
```

---

## 3. Roteiro de Construção Guiada: Laboratório WFA + Mini-CRUD em Memória (`A10.01`–`A10.05`)

Antes de conectar banco de dados, você vai construir um pequeno **Laboratório de Componentes WFA** em um projeto separado no Visual Studio 2022 para praticar **todos os controles juntos** em um fluxo completo de **CRUD em memória** (*Create, Read, Update, Delete*). *(Assim sua solução `Supermercado` da Aula 09 permanece intacta e limpa para a Aula 11!)*

### Passo 1 — Criar o Projeto de Laboratório no Visual Studio 2022
1. Abra o Visual Studio 2022 → **Criar um novo projeto** → **Aplicativo do Windows Forms** (C#, .NET moderno — **sem** `.NET Framework`).
2. Nomeie o projeto como **`LabComponentesWFA`** e clique em **Criar**.
3. Crie uma classe modelo **`ProdutoDemo.cs`** (`Shift+Alt+C`) com as propriedades:
   - `public int Id { get; set; }`
   - `public string Codigo { get; set; } = "";`
   - `public string Nome { get; set; } = "";`
   - `public string Categoria { get; set; } = "";`
   - `public decimal Preco { get; set; }`
   - `public int Quantidade { get; set; }`
   - `public DateTime DataCadastro { get; set; }`
   - `public bool Ativo { get; set; }`
   - `public decimal Subtotal => Preco * Quantidade;`

### Passo 2 — Desenhar a Tela no Designer Usando Cada Componente (`A10.01`–`A10.04`)
Abra `Form1.cs` no Designer (`Shift+F7`), redimensione a janela para cerca de `920 × 650`, configure `Text = "Laboratório WFA — Mini-CRUD de Produtos"` e `StartPosition = CenterScreen`, e arraste os seguintes controles da **Caixa de Ferramentas (`Ctrl+Alt+X`)**, nomeando cada um pela janela **Propriedades (`F4`)**:

1. **Entrada de dados:**
   - `Label` + **`TextBox txtCodigo`** (`MaxLength = 30`, `PlaceholderText = "Ex.: 000123"`).
   - `Label` + **`TextBox txtNome`** (`MaxLength = 120`).
   - `Label` + **`ComboBox cmbCategoria`** (`DropDownStyle = DropDownList`, itens `"Mercearia"`, `"Bebidas"`, `"Limpeza"`, `"Hortifrúti"`).
   - `Label` + **`NumericUpDown numPreco`** (`DecimalPlaces = 2`, `Minimum = 0`, `Maximum = 999999.99`, `Increment = 0.50`).
   - `Label` + **`NumericUpDown numQuantidade`** (`DecimalPlaces = 0`, `Minimum = 1`, `Maximum = 10000`, `Value = 1`).
   - `Label` + **`DateTimePicker dtpCadastro`** (`Format = Short`).
   - **`CheckBox chkAtivo`** (`Text = "Produto ativo"`, `Checked = True`).
   - **`GroupBox grpOrigem`** (`Text = "Origem do cadastro"`) contendo dois **`RadioButton`**: `radManual` (`Text = "Manual"`, `Checked = True`) e `radImportado` (`Text = "Importado"`).
   - **`ErrorProvider errValidacao`** (arraste da Caixa de Ferramentas — aparecerá na barra inferior do Designer).
2. **Barra de ações (dentro de um `FlowLayoutPanel flpAcoes` horizontal):**
   - **`Button btnNovo`** (`Text = "Novo"`)
   - **`Button btnSalvar`** (`Text = "Salvar"`)
   - **`Button btnEditar`** (`Text = "Editar selecionado"`)
   - **`Button btnExcluir`** (`Text = "Excluir selecionado"`)
3. **Visualização e resumo:**
   - **`DataGridView dgvProdutos`** configurada com `ReadOnly = True`, `AllowUserToAddRows = False`, `AllowUserToDeleteRows = False`, `MultiSelect = False`, `SelectionMode = FullRowSelect` e `AutoSizeColumnsMode = Fill`.
   - **`Label lblResumo`** (`Text = "Itens: 0 | Total em estoque: R$ 0,00"`).

### Passo 3 — Programar o Ciclo Completo do Mini-CRUD em Memória (`A10.05`)
Abra o código de `Form1.cs` (`F7`) e construa os seguintes elementos:

1. **Estado na memória:**
   - `private readonly List<ProdutoDemo> produtos = [];`
   - `private int proximoId = 1;`
   - `private int idEmEdicao = 0;` *(mesmo padrão que usaremos no `FormProdutos` do MarketFlow: `0` = incluir novo, `> 0` = editar existente!)*.
2. **Métodos auxiliares na classe:**
   - `AtualizarGrade()`: faz `dgvProdutos.DataSource = null; dgvProdutos.DataSource = produtos.ToList(); dgvProdutos.ClearSelection();` e atualiza `lblResumo.Text` com `produtos.Count` e `produtos.Sum(p => p.Subtotal):C`.
   - `LimparCampos()`: zera `idEmEdicao = 0;`, limpa `txtCodigo` e `txtNome`, volta `cmbCategoria.SelectedIndex = 0`, `numPreco.Value = 0`, `numQuantidade.Value = 1`, `dtpCadastro.Value = DateTime.Today`, `chkAtivo.Checked = true`, `radManual.Checked = true`, limpa `errValidacao.Clear()` e chama `txtCodigo.Focus()`.
3. **Os 4 eventos do Mini-CRUD:**
   - **`btnNovo_Click`:** chama `LimparCampos()`.
   - **`btnSalvar_Click` (Create e Update):**
     - Limpa `errValidacao.Clear()`.
     - Valida `txtCodigo` (não vazio), `txtNome` (mínimo 2 caracteres) e `numPreco.Value > 0`; se algum falhar, marque o campo com `errValidacao.SetError(...)` e dê `return;`.
     - Se `idEmEdicao == 0`: cria `new ProdutoDemo { Id = proximoId++, ... }` lendo **cada controle** (`txtCodigo.Text.Trim()`, `txtNome.Text.Trim()`, `cmbCategoria.SelectedItem!.ToString()!`, `numPreco.Value`, `(int)numQuantidade.Value`, `dtpCadastro.Value.Date`, `chkAtivo.Checked`) e adiciona em `produtos.Add(...)`.
     - Se `idEmEdicao > 0`: localiza o item com `produtos.FirstOrDefault(p => p.Id == idEmEdicao)` e atualiza suas propriedades (mantendo o mesmo `Id`!).
     - Chama `AtualizarGrade()` e `LimparCampos()`.
   - **`btnEditar_Click` (Read → preencher controles):**
     - Verifica `dgvProdutos.SelectedRows.Count == 0 || dgvProdutos.SelectedRows[0].DataBoundItem is not ProdutoDemo item`; se não houver seleção, exibe `MessageBox.Show("Selecione um produto na grade.")` e retorna.
     - Preenche `idEmEdicao = item.Id;` e devolve cada valor do objeto para os controles (`txtCodigo.Text`, `txtNome.Text`, `cmbCategoria.SelectedItem`, `numPreco.Value`, `numQuantidade.Value`, `dtpCadastro.Value`, `chkAtivo.Checked`).
   - **`btnExcluir_Click` (Delete com confirmação):**
     - Recupera o objeto selecionado por `DataBoundItem`.
     - Pede confirmação com `MessageBox.Show(..., "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question)`.
     - Só remove (`produtos.Remove(item);`), recarrega a grade e limpa os campos se o retorno for `DialogResult.Yes`!

---

## 4. Roteiro de Testes do Laboratório (`A10.01`–`A10.05`)

Execute com **`F5`** no Visual Studio 2022 e comprove cada comportamento antes de seguir para o projeto MarketFlow:

1. **Validação com `ErrorProvider`:** clique em **Salvar** com nome vazio ou preço `0` → o ícone de erro deve aparecer ao lado do campo, sem adicionar linha vazia na grade.
2. **Create com zeros à esquerda e moeda:** inclua `"000123"` / `"Arroz 5kg"` / `"Mercearia"` / `25,90` / quantidade `2` → a grade deve exibir `000123` preservando os zeros e `lblResumo` deve mostrar `Itens: 1 | Total em estoque: R$ 51,80`.
3. **Lista fechada (`ComboBox`):** tente digitar texto livre dentro de `cmbCategoria` → o modo `DropDownList` deve impedir a digitação livre.
4. **Update via `DataBoundItem`:** clique em **Editar selecionado** sem selecionar nada (deve avisar); depois selecione o Arroz, clique em **Editar selecionado**, mude o preço para `27,00`, desmarque `chkAtivo` e salve → deve atualizar o **mesmo ID `1`** sem duplicar linha!
5. **Delete com confirmação (`YesNo`):** clique em **Excluir selecionado**, responda **Não** (o item continua na grade); repita respondendo **Sim** (o item some e o resumo volta para `0`).
6. **Comparação com o MarketFlow:** anote no caderno qual componente do laboratório de hoje você usará em cada tela do supermercado (`FormLogin`, `FormProdutos`, `FormCaixa`, `FormRelatorio`, `FormUsuarios`).

---

## 5. Erros Comuns com Componentes WFA no Visual Studio 2022

| Sintoma / Erro | Causa provável | Como corrigir |
| --- | --- | --- |
| Controle criado via `new` no código não aparece na janela | Esqueceu de adicioná-lo ao contêiner pai. | Chame `Controls.Add(meuControle)` ou `painel.Controls.Add(meuControle)`. |
| `DataGridView` não atualiza depois de `produtos.Add(...)` | A grade continua apontando para a mesma referência de `List<T>` antiga. | Reatribua `dgv.DataSource = null; dgv.DataSource = produtos.ToList();`. |
| `InvalidCastException` ou erro ao ler `NumericUpDown.Value` | Tentou atribuir `numQuantidade.Value` (que é `decimal`) direto em uma variável `int`. | Faça a conversão explícita: `int qtd = (int)numQuantidade.Value;`. |
| `NullReferenceException` em `cmbCategoria.SelectedItem` | Nenhum item foi selecionado inicialmente no `ComboBox`. | Defina `cmbCategoria.SelectedIndex = 0;` após popular os itens ou valide `if (cmbCategoria.SelectedItem is null)`. |
| Dois grupos de `RadioButton` desmarcam um ao outro | Todos os `RadioButton` foram jogados direto no `Form` em vez de contêineres separados. | Coloque cada grupo exclusivo dentro de seu próprio `GroupBox` ou `Panel`. |
| Usuário consegue apagar linha ou digitar na `DataGridView` | Propriedades padrão da grade não foram travadas. | Configure `ReadOnly = true`, `AllowUserToAddRows = false` e `AllowUserToDeleteRows = false`. |

[Anterior: Aula 09](09-Configuracao-Ambiente.md) · [Próxima: Aula 11 — Interface de login do MarketFlow](11-Criando-Interface-Login.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 10 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-10). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
