# Aula 02 — Entrada Segura, Decisões (`if`/`switch`), Laços (`for`/`while`/`foreach`) e Depuração

**Duração:** 3h30. **Pré-requisito:** [Aula 01](01-Primeiro-Programa-e-Tipos.md). **Entrega:** aplicativo de console interativo do caixa com leitura blindada (`TryParse` + `do/while`), decisões (`if`/`else if`/`else` e `switch/case`), laços (`for`, `while`, `foreach`) e depuração demonstrada passo a passo com Breakpoint (`F9`/`F5`/`F10`) no Visual Studio 2022 (`A02.01`–`A02.08`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Revisar os 7 tipos da Aula 01 e o perigo de confiar na digitação do usuário |
| Explicação | 30 | `ReadLine`, `??`, `TryParse`/`out`, operadores, `if`/`switch`, laços e Depurador |
| Construção | 60 | Executar os Exemplos Completos #1, #2 e #3 (entrada segura, `if` e `switch`) |
| Intervalo | 15 | Pausa |
| Prática | 60 | Executar o Exemplo #4 (laços) e construir o Caixa de Console Validado com `F9`/`F10` |
| Entrega | 30 | Demonstrar entradas inválidas (`abc`, `0`, `-1`), Breakpoint e registrar evidências |

---

## 1. Lendo Dados do Teclado com Segurança (`ReadLine`, `??`, `TryParse` e `out`)

Na Aula 01, todos os valores estavam fixos no código. Agora o **usuário vai digitar no teclado** usando `Console.ReadLine()`.

- `Console.ReadLine()` sempre devolve **`string?`** (um texto que pode ser **`null`** caso o fluxo do terminal seja encerrado com `Ctrl+Z`).
- O operador **`??` (coalescência nula)** fornece um valor reserva se o lado esquerdo for `null`:
  - `string nome = Console.ReadLine() ?? "";` → se vier `null`, usa `""`; se o usuário digitar `"Arroz"`, usa `"Arroz"`.
  - **Atenção:** `?? ""` trata `null`, mas **não** impede o usuário de apertar apenas Enter (deixando texto vazio `""`).

### Por que `int.Parse(Console.ReadLine())` é perigoso?
Se você escrever `int qtd = int.Parse(Console.ReadLine()!);` e o operador esbarrar na letra `"a"`, o programa lança **`FormatException`** e **fecha instantaneamente na cara do cliente!**

### Exemplo Prático Completo #1 — Leitura Blindada com `TryParse`, `out` e `do / while`

Digite e execute este código completo no `Program.cs`, testando digitar `"abc"`, `"0"`, `"-1"` e só depois `"3"` e `"5,50"`:

```csharp
using System.Globalization;

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");

Console.Write("Nome do produto: ");
string nome = Console.ReadLine() ?? "";

// 1. Leitura segura de QUANTIDADE (inteiro > 0):
int quantidade;
string? entrada;

do
{
    Console.Write("Quantidade (inteiro maior que zero): ");
    entrada = Console.ReadLine();

    if (entrada is null)
    {
        Console.WriteLine("Entrada encerrada pelo usuário.");
        return; // Encerra o programa sem ficar em laço infinito
    }

    if (!int.TryParse(entrada, out quantidade))
    {
        Console.WriteLine("  -> Erro de conversão: digite apenas números inteiros (ex.: 3).");
    }
    else if (quantidade <= 0)
    {
        Console.WriteLine("  -> Erro de regra: a quantidade precisa ser maior que zero.");
    }
} while (!int.TryParse(entrada, out quantidade) || quantidade <= 0);

// 2. Leitura segura de PREÇO (decimal > 0, aceitando vírgula pt-BR):
decimal preco;
do
{
    Console.Write("Preço unitário em R$ (ex.: 5,50): ");
    entrada = Console.ReadLine();

    if (entrada is null)
    {
        Console.WriteLine("Entrada encerrada pelo usuário.");
        return;
    }
} while (!decimal.TryParse(entrada, out preco) || preco <= 0m);

decimal total = quantidade * preco;

Console.WriteLine("\n=== COMPROVANTE DIDÁTICO — SEM VALOR FISCAL ===");
Console.WriteLine($"Produto   : {nome}");
Console.WriteLine($"Cálculo   : {quantidade} x {preco:C}");
Console.WriteLine($"Total     : {total:C}");
```

### Exemplo de Execução no Console

```text
Nome do produto: Arroz
Quantidade (inteiro maior que zero): abc
  -> Erro de conversão: digite apenas números inteiros (ex.: 3).
Quantidade (inteiro maior que zero): 0
  -> Erro de regra: a quantidade precisa ser maior que zero.
Quantidade (inteiro maior que zero): -1
  -> Erro de regra: a quantidade precisa ser maior que zero.
Quantidade (inteiro maior que zero): 3
Preço unitário em R$ (ex.: 5,50): abc
Preço unitário em R$ (ex.: 5,50): 0
Preço unitário em R$ (ex.: 5,50): 5,50

=== COMPROVANTE DIDÁTICO — SEM VALOR FISCAL ===
Produto   : Arroz
Cálculo   : 3 x R$ 5,50
Total     : R$ 16,50
```

### Dissecando a Condição `while (!int.TryParse(entrada, out quantidade) || quantidade <= 0);`
- **`do { ... } while (...);`**: executa o bloco pelo menos uma vez e só testa no final se precisa repetir (note o `;` obrigatório no final!).
- **`int.TryParse(entrada, out quantidade)`**: tenta converter sem jamais travar o programa. Retorna `true` (sucesso) ou `false` (falha) e, pelo modificador **`out`**, grava o número convertido dentro de `quantidade`.
- **`!` (NÃO)** e **`||` (OU com curto-circuito)**: repete se **NÃO** converteu **OU** se o número for `<= 0`. Se o usuário digitou `"abc"`, o lado esquerdo já é `true` e o C# nem testa `quantidade <= 0`.
- **Diferença fundamental:** `"abc"` é **falha de conversão**; `0` e `-1` são inteiros válidos, porém **recusados pela regra de negócio**.

---

## 2. Tomando Decisões: `if / else if / else` e `switch / case`

### Tabela de Operadores Relacionais e Lógicos

| Operador | Significado | Exemplo | Resultado |
| --- | --- | --- | --- |
| `==` | Igual a *(dois sinais! Nunca use `=` para comparar)* | `5 == 5` | `true` |
| `!=` | Diferente de | `5 != 3` | `true` |
| `>` / `>=` | Maior que / Maior ou igual a | `50 > 50` / `50 >= 50` | `false` / `true` |
| `<` / `<=` | Menor que / Menor ou igual a | `10 <= 10` | `true` |
| `&&` | **E** lógico (ambos precisam ser `true`) | `preco > 0 && estoque > 0` | `true` se ambos verdadeiros |
| `\|\|` | **OU** lógico (basta um ser `true`) | `qtd <= 0 \|\| preco <= 0` | `true` se qualquer um verdadeiro |
| `!` | **NÃO** lógico (inverte o valor) | `!ativo` | `true` se `ativo` for `false` |

### Exemplo Prático Completo #2 — `if / else if / else` e Teste de Fronteiras (`>` vs `>=`)

Use **`if / else if / else`** quando precisa testar **faixas de valores** ou condições combinadas. **Regra importante:** teste sempre da faixa **mais alta** para a **mais baixa**!

```csharp
using System.Globalization;

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");

// Testando exatamente as 4 fronteiras críticas da regra de brinde/cupom:
decimal[] comprasDeTeste = { 50.00m, 50.01m, 100.00m, 100.01m };

foreach (decimal totalCompra in comprasDeTeste)
{
    string beneficio;

    if (totalCompra > 100.00m)
    {
        beneficio = "Ganhou CUPOM de 10% para a próxima compra!";
    }
    else if (totalCompra > 50.00m)
    {
        beneficio = "Ganhou um BRINDE na saída!";
    }
    else
    {
        beneficio = "Obrigado pela preferência!";
    }

    Console.WriteLine($"Compra de {totalCompra:C} -> {beneficio}");
}
```

### Saída Esperada no Console

```text
Compra de R$ 50,00 -> Obrigado pela preferência!
Compra de R$ 50,01 -> Ganhou um BRINDE na saída!
Compra de R$ 100,00 -> Ganhou um BRINDE na saída!
Compra de R$ 100,01 -> Ganhou CUPOM de 10% para a próxima compra!
```

> [!NOTE]
> **Por que `50,00` NÃO ganhou brinde?** Porque a condição usou **`>`** (*estritamente maior que `50,00`*). Se a regra dissesse *"a partir de `50,00` inclusive"*, usaríamos **`>=`**. Sempre teste os valores exatos da fronteira!

### Exemplo Prático Completo #3 — Menu de Opções com `switch / case / break / default`

Use **`switch`** quando está comparando **uma única variável** contra opções fixas (como um menu numérico):

```csharp
Console.WriteLine("=== FORMA DE PAGAMENTO (SIMULAÇÃO) ===");
Console.WriteLine("1 - Dinheiro simulado");
Console.WriteLine("2 - Cartão simulado");
Console.WriteLine("3 - Pix simulado");
Console.Write("Escolha uma opção: ");

// Se o usuário digitar "abc", TryParse devolve false e deixa opcao = 0 (cai no default!):
int.TryParse(Console.ReadLine(), out int opcao);

switch (opcao)
{
    case 1:
        Console.WriteLine("Selecionado: Dinheiro simulado. Conferir troco.");
        break; // Obrigatório para encerrar este caso!

    case 2:
        Console.WriteLine("Selecionado: Cartão simulado. Aguardando maquininha fictícia.");
        break;

    case 3:
        Console.WriteLine("Selecionado: Pix simulado. Exibindo QR Code fictício.");
        break;

    default:
        Console.WriteLine("Opção inválida! Escolha 1, 2 ou 3.");
        break;
}
```

---

## 3. Estruturas de Repetição (Laços): `for`, `while` e `foreach`

Cada laço em C# resolve um tipo diferente de problema:

| Laço | Quando escolher? | Ideia central |
| --- | --- | --- |
| **`for`** | Quando você **sabe exatamente quantas vezes** vai repetir (ex.: imprimir 3 vias). | Inicialização; condição; passo (`i++` ou `i--`). |
| **`while`** | Quando você **não sabe quantas voltas** dará, mas tem uma **condição de parada** (ex.: enquanto houver estoque ou tentativas). | Testa a condição **antes** de entrar no bloco (pode rodar 0 vezes!). |
| **`do / while`** | Igual ao `while`, mas precisa rodar **pelo menos 1 vez** (ex.: perguntar dado ao usuário). | Testa a condição no **final**. |
| **`foreach`** | Quando quer percorrer **todos os itens** de uma coleção/array um por um. | Sem índice manual — impossível errar o limite do array! |

### Exemplo Prático Completo #4 — Os 3 Laços Funcionando na Prática

Digite e execute este código completo para observar o comportamento exato de `for`, `while` (com operador composto e com decremento) e `foreach`:

```csharp
// 1. LAÇO FOR — Quantidade exata de repetições (3 vias de comprovante):
Console.WriteLine("=== 1. LAÇO FOR (3 vias) ===");
for (int via = 1; via <= 3; via++)
{
    Console.WriteLine($"Imprimindo Via {via} de 3...");
}

// 2. LAÇO WHILE COM CONDIÇÃO COMPOSTA — Leitura simulada de balança (máx. 5 tentativas):
Console.WriteLine("\n=== 2. LAÇO WHILE (balança com limite de tentativas) ===");
bool balancaEstavel = false;
int tentativas = 0;

while (!balancaEstavel && tentativas < 5)
{
    tentativas++; // Mesmo que: tentativas = tentativas + 1;
    Console.WriteLine($"Tentativa {tentativas}: verificando estabilidade da balança...");

    // Simulação: estabiliza na 4ª tentativa (se nunca estabilizar, para no 5 sem travar!)
    if (tentativas == 4)
    {
        balancaEstavel = true;
        Console.WriteLine("Balança estabilizada com sucesso!");
    }
}

// 3. LAÇO WHILE COM DECREMENTO — Baixa controlada de estoque até zerar:
Console.WriteLine("\n=== 3. LAÇO WHILE (decremento de estoque) ===");
int estoqueMacas = 3;
while (estoqueMacas > 0)
{
    estoqueMacas--; // Diminui 1 unidade por volta (3 -> 2 -> 1 -> 0)
    Console.WriteLine($"Venda de 1 maçã registrada. Restam: {estoqueMacas}");
}
Console.WriteLine("ALERTA: Estoque vazio! Solicitar reposição.");

// 4. LAÇO FOREACH — Percorrendo um array de produtos:
Console.WriteLine("\n=== 4. LAÇO FOREACH (catálogo) ===");
string[] catalogo = { "Arroz", "Feijão", "Macarrão", "Molho de Tomate" };
foreach (string produto in catalogo)
{
    Console.WriteLine($"- Produto disponível: {produto}");
}
```

### Saída Esperada no Console

```text
=== 1. LAÇO FOR (3 vias) ===
Imprimindo Via 1 de 3...
Imprimindo Via 2 de 3...
Imprimindo Via 3 de 3...

=== 2. LAÇO WHILE (balança com limite de tentativas) ===
Tentativa 1: verificando estabilidade da balança...
Tentativa 2: verificando estabilidade da balança...
Tentativa 3: verificando estabilidade da balança...
Tentativa 4: verificando estabilidade da balança...
Balança estabilizada com sucesso!

=== 3. LAÇO WHILE (decremento de estoque) ===
Venda de 1 maçã registrada. Restam: 2
Venda de 1 maçã registrada. Restam: 1
Venda de 1 maçã registrada. Restam: 0
ALERTA: Estoque vazio! Solicitar reposição.

=== 4. LAÇO FOREACH (catálogo) ===
- Produto disponível: Arroz
- Produto disponível: Feijão
- Produto disponível: Macarrão
- Produto disponível: Molho de Tomate
```

---

## 4. Como Usar o Depurador (Debugger) do Visual Studio 2022 (`F9`, `F5`, `F10`)

Um programa pode compilar sem nenhum erro vermelho e ainda ter um **erro de lógica** (por exemplo, escrever `via < 3` em vez de `via <= 3` e imprimir só 2 vias). Em vez de "chutar", usamos o **Depurador**:

1. Clique na margem cinza à esquerda da linha `decimal total = quantidade * preco;` (ou pressione **`F9`**) — aparecerá uma **bolinha vermelha (Breakpoint)**.
2. Coloque um segundo Breakpoint na linha `estoqueMacas--;`.
3. Execute com **`F5`** (*Iniciar com Depuração* — **não** `Ctrl+F5`!).
4. Quando o programa pausar na **linha amarela**, abra **Depurar → Janelas → Locais** (`Ctrl+Alt+V, L`) ou passe o mouse em cima de `quantidade`, `preco` e `estoqueMacas`.
5. Pressione **`F10`** (*Step Over* / Contornar) para avançar **uma linha por vez** vendo as variáveis mudarem em tempo real na memória RAM!
6. Pressione **`F5`** para continuar até o próximo Breakpoint; se cair em um laço infinito acidental, clique no botão quadrado vermelho **Parar Depuração** (`Shift+F5`).

---

## 5. Roteiro de Prática e Entregas no Visual Studio 2022 (`A02.01`–`A02.08`)

No projeto **`TreinoSupermercado`**, monte o seu **Caixa de Console Validado** integrando todos os blocos abaixo:

1. **Leitura blindada e comprovante (`A02.01`–`A02.04`):** leia nome, quantidade positiva (`int.TryParse` testando `abc`, `0`, `-1` e `3`) e preço positivo (`decimal.TryParse` em `pt-BR` testando `abc`, `0` e `5,50`), imprimindo `3 x R$ 5,50 = R$ 16,50`.
2. **Faixas de brinde/cupom (`A02.05`):** aplique o `if / else if / else` sobre `total` testando valores que gerem `50,00`, `50,01`, `100,00` e `100,01`.
3. **Menu simulado (`A02.06`):** construa o `switch` de pagamento (`1`, `2`, `3` e opção inválida) e registre as 4 saídas.
4. **Três laços (`A02.07`):** imprima 3 vias (`for`), simule até 5 leituras de balança (`while` com `!balancaEstavel && tentativas < 5`) e liste 4 produtos (`foreach`).
5. **Estoque com decremento + Depuração (`A02.08`):** leia estoque inicial aceitando **`>= 0`** (zero é válido e não entra no `while`; negativo é recusado), decremente até `0` e **depure com `F9`/`F5`/`F10`** anotando no caderno os valores vistos na janela **Locais** a cada volta.

---

## 6. Erros Comuns de Iniciantes (❌ Errado vs ✅ Certo)

| Erro / Sintoma | ❌ Código que quebra | ✅ Como corrigir |
| --- | --- | --- |
| **`CS0163`** (faltou `break`) | `case 1: Console.WriteLine("Ok"); case 2:` | Encerre **todo** `case` e `default` com `break;`. |
| **Laço infinito no `while`** | `while (estoque > 0) { Console.WriteLine(estoque); }` | Altere a variável da condição dentro do laço (`estoque--;`). |
| **Confundir `=` com `==`** | `if (opcao = 1)` | Use dois sinais para comparar: `if (opcao == 1)`. |
| **Digitar `5,50` falha no `TryParse`** | Esquecer `CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");` | Configure a cultura `pt-BR` no início do `Program.cs`. |
| **Breakpoint não pausa** | Rodar com `Ctrl+F5` (*sem* depuração). | Execute com **`F5`** (*Iniciar com Depuração*). |

[Anterior: Aula 01](01-Primeiro-Programa-e-Tipos.md) · [Próxima: Aula 03 — Métodos, parâmetros e escopo](03-Metodos-Parametros-e-Escopo.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 02 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-02). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
