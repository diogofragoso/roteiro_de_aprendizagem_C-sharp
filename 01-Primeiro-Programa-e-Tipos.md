# Aula 01 — Primeiro Programa no Visual Studio 2022, Variáveis e Tipos de Dados

**Duração:** 3h30. **Pré-requisito:** nenhum conhecimento prévio de programação (basta saber usar o Windows). **Entrega:** primeiro aplicativo de console criado no Visual Studio 2022 demonstrando ordem de execução, investigação de erro de compilação, os 7 tipos de dados do domínio, constantes, atribuição em memória, conversão com cultura e formatação monetária (`A01.01`–`A01.06`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Conhecer o projeto MarketFlow e entender o que é um algoritmo |
| Explicação | 30 | C#, .NET, Visual Studio 2022, memória RAM, 7 tipos, `const` e cultura `pt-BR` |
| Construção | 60 | Criar o projeto Console no VS 2022 e executar os Exemplos Completos #1 e #2 |
| Intervalo | 15 | Pausa |
| Prática | 60 | Executar os Exemplos Completos #3 e #4 e completar os desafios `A01.01`–`A01.06` |
| Entrega | 30 | Demonstrar execução, explicar a escolha de cada tipo e registrar evidências |

---

## 1. O Que É um Programa? (Algoritmos e Ferramentas)

Um computador não adivinha intenções: ele apenas executa uma **sequência exata de instruções**, linha por linha, de cima para baixo. Essa sequência lógica de passos se chama **algoritmo**.

- **❌ Instrução vaga:** *"Calcule a compra do cliente direitinho."*
- **✅ Algoritmo preciso:**
  1. Guardar o nome do produto (`"Arroz"`);
  2. Guardar a quantidade (`3`);
  3. Guardar o preço unitário (`5,50`);
  4. Multiplicar quantidade pelo preço (`3 × 5,50 = 16,50`);
  5. Exibir o resultado formatado (`R$ 16,50`) na tela.

Antes de escrever código, entenda o papel de cada ferramenta que você vai usar:

| Ferramenta | O que é? | Por que precisamos dela? |
| --- | --- | --- |
| **C# (C-Sharp)** | Linguagem de programação moderna e **fortemente tipada**. | Exige declarar o tipo de cada dado (`int`, `decimal`, `string`), impedindo bugs invisíveis (como somar texto com dinheiro) **antes** de o programa rodar. |
| **.NET SDK** | Compilador e ferramentas que traduzem o código C# em executável. | O processador não lê texto humano; o compilador confere a gramática inteira e gera o programa executável. |
| **Visual Studio 2022** | IDE (ambiente integrado) com editor, Gerenciador de Soluções, Lista de Erros e Depurador. | Destaca erros em vermelho enquanto você digita, sugere comandos (IntelliSense) e permite pausar o programa linha por linha. |
| **Aplicativo de Console** | Programa de tela de texto simples (sem botões gráficos ainda). | Remove distrações visuais no início para você dominar lógica, variáveis, métodos e objetos antes de adicionar telas Windows Forms (Aulas 09–10) e banco de dados (Aula 12). |

---

## 2. Criando Seu Primeiro Projeto no Visual Studio 2022

Siga exatamente esta sequência:

1. Abra o **Visual Studio 2022 Community** pelo menu Iniciar do Windows.
2. Clique em **Criar um novo projeto**.
3. Pesquise `Console`, filtre por linguagem **C#** e escolha **Aplicativo de Console** (.NET moderno).
   - **⚠️ Atenção:** **NÃO** escolha `Aplicativo de Console (.NET Framework)` — o `.NET Framework` é a geração antiga e não suporta recursos modernos de C# usados neste guia.
4. Clique em **Próximo**, digite o nome **`TreinoSupermercado`** e clique em **Próximo**.
5. Selecione a versão moderna do **.NET** instalada no laboratório (ex.: `.NET 8.0`), deixe **desmarcada** a opção *"Não usar instruções de nível superior"* e clique em **Criar**.

### As 3 janelas que você precisa reconhecer no VS 2022
- **Editor de Código (centro):** onde o arquivo `Program.cs` está aberto.
- **Gerenciador de Soluções (direita — `Ctrl+Alt+L`):** mostra arquivos e pastas do projeto.
- **Lista de Erros (inferior — `Ctrl+\, E`):** lista todos os erros vermelhos com código (ex.: `CS1002`), mensagem e número da linha.

- **Compilar (`Ctrl+Shift+B`):** verifica a gramática sem abrir o programa.
- **Executar (`Ctrl+F5`):** compila e roda o console mantendo a janela aberta no final. *(Se der erro de compilação e o VS perguntar se quer rodar a versão antiga, clique sempre em **Não**!)*

---

## 3. Exemplo Prático Completo #1 — Ordem de Execução e Anatomia do C#

Apague o conteúdo inicial de `Program.cs`, digite exatamente o código abaixo e pressione **`Ctrl+F5`**:

```csharp
// Isto é um comentário: o computador ignora tudo após // até o fim da linha.
// As instruções executam SEMPRE de cima para baixo, uma após a outra:

Console.WriteLine("1. Bem-vindo ao MarketFlow!");
Console.WriteLine("2. Sistema de treinamento em C# no Console.");
Console.Write("3. Este texto usa Write (sem pular linha) -> ");
Console.WriteLine("e este completa a mesma linha!");
Console.WriteLine("4. Fim da execução.");
```

### Saída Esperada no Console

```text
1. Bem-vindo ao MarketFlow!
2. Sistema de treinamento em C# no Console.
3. Este texto usa Write (sem pular linha) -> e este completa a mesma linha!
4. Fim da execução.
```

### Dissecando cada símbolo da instrução `Console.WriteLine("...");`

| Símbolo / Palavra | Significado | Por que precisa estar ali? |
| --- | --- | --- |
| `Console` | Classe pronta do .NET que representa o terminal. | C# diferencia maiúsculas de minúsculas (*case-sensitive*): `console` gera erro! |
| `.` | Operador de acesso. | Lê-se *"acesse uma ação dentro de `Console`"*. |
| `WriteLine` vs `Write` | Métodos de escrita. | `WriteLine` imprime e pula linha; `Write` imprime e deixa o cursor na mesma linha. |
| `("...")` | Parênteses com texto literal (`string`). | As aspas duplas marcam início e fim do texto (não são exibidas na tela). |
| `;` | Ponto e vírgula. | Encerra a instrução; sem ele, o compilador junta com a próxima linha e falha (`CS1002`). |

> [!TIP]
> **Experimento obrigatório (`A01.01`):** apague de propósito a aspa dupla final da linha 1, pressione `Ctrl+Shift+B`, observe o erro vermelho `CS1010` na **Lista de Erros**, dê **duplo clique no erro** para o Visual Studio levar seu cursor direto à linha com problema e recoloque a aspa.

---

## 4. Exemplo Prático Completo #2 — Variáveis, os 7 Tipos e o Bug do `double` em Dinheiro

Uma **variável** é uma "caixinha nomeada" na memória RAM que tem um **tipo fixo** e guarda um **valor**.

```text
Sintaxe para criar (declarar) uma variável:
[tipo] [nomeDaVariavel] = [valorInicial];
```

Digite e execute este programa completo no `Program.cs` para ver os **7 tipos essenciais** funcionando na prática:

```csharp
using System.Globalization;

// Padroniza o programa para o formato brasileiro (vírgula decimal e R$):
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");

// 1. string — Textos e códigos que NÃO são contas matemáticas (preserva zeros à esquerda!):
string nomeProduto = "Arroz Integral 5kg";
string codigoBarras = "000123";

// 2. char — Exatamente UM caractere, sempre entre ASPAS SIMPLES (' '):
char setor = 'A';

// 3. int — Número inteiro comum (até ~2,1 bilhões):
int quantidade = 3;

// 4. long — Número inteiro gigante (exige o sufixo L no final):
long contadorGlobal = 7891020304050L;

// 5. double — Número decimal aproximado em base binária (peso, altura, medidas físicas):
double pesoKg = 5.45;

// 6. decimal — Número decimal EXATO em base 10 (OBRIGATÓRIO para dinheiro! Exige sufixo m):
decimal precoUnitario = 5.50m;

// 7. bool — Valor lógico (apenas true ou false):
bool produtoAtivo = true;

Console.WriteLine("=== FICHA DO PRODUTO ===");
Console.WriteLine($"Código    : {codigoBarras}");
Console.WriteLine($"Nome      : {nomeProduto}");
Console.WriteLine($"Setor     : {setor}");
Console.WriteLine($"Quantidade: {quantidade} un.");
Console.WriteLine($"Contador  : {contadorGlobal}");
Console.WriteLine($"Peso      : {pesoKg} kg");
Console.WriteLine($"Preço     : {precoUnitario:C}");
Console.WriteLine($"Ativo?    : {produtoAtivo}");

// PROVA REAL: por que JAMAIS usar 'double' para dinheiro?
double somaDouble = 0.1 + 0.2;
decimal somaDecimal = 0.1m + 0.2m;
Console.WriteLine("\n=== PROVA: double vs decimal ===");
Console.WriteLine($"0.1 + 0.2 em double : {somaDouble}");   // 0,30000000000000004 (ERRO!)
Console.WriteLine($"0.1 + 0.2 em decimal: {somaDecimal}");  // 0,3 (EXATO!)
```

### Saída Esperada no Console

```text
=== FICHA DO PRODUTO ===
Código    : 000123
Nome      : Arroz Integral 5kg
Setor     : A
Quantidade: 3 un.
Contador  : 7891020304050
Peso      : 5,45 kg
Preço     : R$ 5,50
Ativo?    : True

=== PROVA: double vs decimal ===
0.1 + 0.2 em double : 0,30000000000000004
0.1 + 0.2 em decimal: 0,3
```

### Por que escolhemos exatamente cada tipo?

| Tipo | Literal | Regra prática de escolha |
| --- | --- | --- |
| `string` | `"000123"` | Use para nomes, endereços, CPF, telefone e **código de barras** — guardar `"000123"` em `int` apagaria os zeros à esquerda (`123`)! |
| `char` | `'A'` | Apenas 1 letra ou símbolo (aspas simples). |
| `int` | `3` | Contagens inteiras (estoque, quantidade de itens, idade). |
| `long` | `7891020304050L` | Números inteiros acima de 2 bilhões (leva `L`). |
| `double` | `5.45` | Medidas científicas ou físicas aproximadas (peso, distância). **Nunca para dinheiro.** |
| `decimal` | `5.50m` | **Sempre para preços, salários, impostos e troco** (leva `m` de *money*). |
| `bool` | `true` / `false` | Perguntas de sim/não (`ativo`, `temDesconto`, `pagamentoConfirmado`). |

---

## 5. Exemplo Prático Completo #3 — Atribuição na Memória (`=`), Constantes (`const`) e Operadores

Um erro muito comum de iniciantes é achar que o sinal `=` funciona como fórmula do Excel (atualizando sozinho). **Em C#, `=` significa "calcule o lado direito AGORA e grave o resultado na variável da esquerda".**

Execute este exemplo completo e observe atentamente a saída:

```csharp
using System.Globalization;

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");

// Constante: valor fixo que o compilador PROÍBE alterar depois (por convenção, MAIÚSCULAS):
const decimal TAXA_SIMULADA = 0.10m;

int quantidade = 3;
decimal preco = 5.50m;

// Calcula e guarda 16,50 na memória:
decimal subtotal = quantidade * preco;
Console.WriteLine($"1) Com quantidade = {quantidade}: subtotal = {subtotal:C}");

// Agora mudamos APENAS 'quantidade' (sem escrever 'int' de novo, pois ela já existe!):
quantidade = 5;

// ATENÇÃO: 'subtotal' NÃO muda sozinho! Continua guardando 16,50!
Console.WriteLine($"2) Mudei quantidade para {quantidade}, mas NÃO recalculei: subtotal = {subtotal:C}");

// Agora recalculamos e gravamos o novo valor dentro de 'subtotal':
subtotal = quantidade * preco;
Console.WriteLine($"3) Depois de recalcular: subtotal = {subtotal:C}");

// Voltando para quantidade = 3 para calcular taxa de 10%:
quantidade = 3;
subtotal = quantidade * preco;
decimal valorTaxa = subtotal * TAXA_SIMULADA;
decimal totalFinal = subtotal + valorTaxa;

Console.WriteLine("\n=== RESUMO COM TAXA ===");
Console.WriteLine($"Subtotal    : {subtotal:C}");
Console.WriteLine($"Taxa (10%)  : {valorTaxa:C}");
Console.WriteLine($"Total final : {totalFinal:C}");

// ❌ TESTE DO COMPILADOR (descomente para ver o erro CS0131 e depois apague):
// TAXA_SIMULADA = 0.20m;
```

### Saída Esperada no Console

```text
1) Com quantidade = 3: subtotal = R$ 16,50
2) Mudei quantidade para 5, mas NÃO recalculei: subtotal = R$ 16,50
3) Depois de recalcular: subtotal = R$ 27,50

=== RESUMO COM TAXA ===
Subtotal    : R$ 16,50
Taxa (10%)  : R$ 1,65
Total final : R$ 18,15
```

- **Interpolação (`$"..."`):** colocar `$` antes das aspas permite injetar variáveis dentro de `{ }`.
- **Formatador `:C` (*Currency*):** formata automaticamente o `decimal` como moeda da cultura atual (`R$ 16,50`).
- **Operadores aritméticos básicos:** `+` (soma), `-` (subtração), `*` (multiplicação), `/` (divisão) e `%` (resto da divisão).

---

## 6. Exemplo Prático Completo #4 — Conversão de Texto (`Parse`), `CultureInfo` e `+`

No mundo real, dados vindos de arquivos, APIs ou do teclado chegam como **`string`**. Antes de multiplicar `"3"` por `"5.50"`, precisamos **converter** para `int` e `decimal`!

```csharp
using System.Globalization;

CultureInfo culturaBrasil = CultureInfo.GetCultureInfo("pt-BR");
CultureInfo.CurrentCulture = culturaBrasil;

// 1. Diferença crítica do operador '+' entre string e int:
Console.WriteLine("=== CONCATENAÇÃO vs SOMA ===");
Console.WriteLine("\"2\" + \"3\" (textos)  = " + ("2" + "3")); // Junta os textos: "23"
Console.WriteLine("2 + 3     (números) = " + (2 + 3));         // Soma matemática: 5

// 2. Convertendo texto para número com Parse:
string textoQuantidade = "3";
string textoPrecoComPonto = "5.50";     // Padrão internacional (ponto)
string textoPrecoComVirgula = "5,50";   // Padrão brasileiro (vírgula)

int qtdConvertida = int.Parse(textoQuantidade);

// Para ler texto com PONTO independente do Windows estar em português, use InvariantCulture!
decimal precoDePonto = decimal.Parse(textoPrecoComPonto, CultureInfo.InvariantCulture);

// Para ler texto com VÍRGULA brasileira, use a cultura pt-BR:
decimal precoDeVirgula = decimal.Parse(textoPrecoComVirgula, culturaBrasil);

decimal total = qtdConvertida * precoDePonto;

Console.WriteLine("\n=== CONVERSÃO E FORMATAÇÃO ===");
Console.WriteLine($"Convertido de \"5.50\" (Invariant): {precoDePonto:C}");
Console.WriteLine($"Convertido de \"5,50\" (pt-BR)    : {precoDeVirgula:C}");
Console.WriteLine($"Total ({qtdConvertida} x {precoDePonto:C}) = {total.ToString("C", culturaBrasil)}");
```

### Saída Esperada no Console

```text
=== CONCATENAÇÃO vs SOMA ===
"2" + "3" (textos)  = 23
2 + 3     (números) = 5

=== CONVERSÃO E FORMATAÇÃO ===
Convertido de "5.50" (Invariant): R$ 5,50
Convertido de "5,50" (pt-BR)    : R$ 5,50
Total (3 x R$ 5,50) = R$ 16,50
```

> [!WARNING]
> **Por que `CultureInfo.InvariantCulture` é tão importante?** Se você fizer `decimal.Parse("5.50")` em um Windows em português **sem** `InvariantCulture`, o C# acha que o ponto é separador de milhar e transforma `"5.50"` em **`550`**!

---

## 7. Roteiro de Prática e Entregas no Visual Studio 2022 (`A01.01`–`A01.06`)

Agora que você digitou e entendeu os 4 exemplos completos, reúna no projeto **`TreinoSupermercado`** as verificações da aula:

1. **`A01.01` — Ordem e erro de compilação:** exiba 3 mensagens, inverta a ordem prevendo a saída antes de rodar e provoque/corrija o erro de aspa ausente na Lista de Erros.
2. **`A01.02` — Os 7 tipos e tipagem forte:** declare os 7 tipos do domínio (`string` com `"000123"`, `char`, `int`, `long`, `double`, `decimal` `5.50m` e `bool`). Tente escrever `decimal teste = "15.99";`, anote o código de erro **`CS0029`** explicando por que o C# recusou e remova a linha.
3. **`A01.03` — Atribuição na memória:** prove na tela que mudar `quantidade` de `3` para `5` mantém `subtotal` em `R$ 16,50` até você recalcular (`R$ 27,50`).
4. **`A01.04` — Constante:** crie `const decimal TAXA_SIMULADA = 0.10m;`, exiba subtotal (`R$ 16,50`), taxa (`R$ 1,65`) e total (`R$ 18,15`), provoque `CS0131` tentando alterar a constante e remova a linha inválida.
5. **`A01.05` — Conversão e `+`:** converta `"3"` e `"5.50"` com `InvariantCulture`, exiba `R$ 16,50` e registre a diferença entre `"2" + "3"` e `2 + 3`.
6. **`A01.06` — Algoritmo no papel:** escreva no caderno o passo a passo exato (entradas, processamento e saída) para somar três notas (`6`, `7` e `8`) e calcular a média (`7`).

---

## 8. Erros Comuns de Iniciantes (❌ Errado vs ✅ Certo)

| Código do Erro | ❌ Código que quebra | ✅ Como corrigir |
| --- | --- | --- |
| **`CS1002`** (`; esperado`) | `Console.WriteLine("Oi")` | `Console.WriteLine("Oi");` (ponto e vírgula obrigatório). |
| **`CS0664`** (`double` em `decimal`) | `decimal preco = 5.50;` | `decimal preco = 5.50m;` (sufixo `m` obrigatório em literais decimais). |
| **`CS0029`** (tipos incompatíveis) | `decimal preco = "5.50";` | `decimal preco = 5.50m;` ou `decimal.Parse("5.50", CultureInfo.InvariantCulture);`. |
| **`CS0131`** (alterar constante) | `const decimal TAXA = 0.1m; TAXA = 0.2m;` | Constantes não podem mudar; use variável comum se o valor precisar variar. |
| **`CS1012`** (aspas simples em texto) | `string nome = 'Arroz';` | `string nome = "Arroz";` (aspas simples são exclusivas de 1 único `char`). |
| **`CS0128`** (redeclarar variável) | `int q = 3; int q = 5;` | `int q = 3; q = 5;` (só informe o tipo na primeira criação da variável). |

[Próxima aula: Aula 02 — Entrada, decisões, laços e depuração](02-Entrada-Decisoes-Lacos-e-Depuracao.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 01 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-01). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
