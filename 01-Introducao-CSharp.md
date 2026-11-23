# Aula 01 — Lógica, Tipos de Dados e Controle de Fluxo no Supermercado

**Duração:** 3h30. **Pré-requisito:** percurso [F00–F07](fundamentos/README.md) concluído (ou demonstrado em diagnóstico) e laboratório com **Visual Studio 2022** preparado pelo professor. **Entrega:** programa de console organizado nos blocos `A01.01`–`A01.08` demonstrando tipos, constantes, conversões, decisões e repetições no domínio do supermercado.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Contexto e problema real do supermercado |
| Explicação | 30 | Tipos do domínio, decisões e repetições ("o quê" e "por quê") |
| Construção | 60 | Explorar valores, constantes e conversões no Visual Studio 2022 |
| Intervalo | 15 | Pausa |
| Prática | 60 | Construir decisões (`if`, `switch`) e laços (`for`, `while`, `foreach`) |
| Entrega | 30 | Demonstrar casos normal/inválido e registrar evidências |

> [!NOTE]
> Esta aula aplica os fundamentos de C# ao problema do supermercado. Se você ainda não sabe criar um projeto console no Visual Studio 2022, declarar variáveis ou escrever métodos, faça primeiro o [guia preparatório F00–F07](fundamentos/README.md).

---

## 1. C#, .NET e a Tipagem Forte no Mundo Real

O **C# (C-Sharp)** é a linguagem em que escrevemos o código-fonte; a plataforma **.NET** fornece o compilador (SDK) e o motor de execução (Runtime) que rodam esse programa no Windows.

**Por que a tipagem forte do C# é essencial em um sistema comercial?**
Em um supermercado, misturar texto com dinheiro ou tentar fazer conta com um código de barras gera prejuízo real no fechamento do caixa. Como o C# exige que cada variável declare seu tipo exato, o compilador do **Visual Studio 2022** barra incompatibilidades **antes** de o sistema ir para o caixa.

---

## 2. Escolhendo o Tipo Certo — E os Motivos Técnicos de Cada Escolha

Cada dado do supermercado pede um tipo específico. Entenda **por que** escolhemos cada um (e o erro que ocorreria se escolhêssemos outro):

| Tipo em C# | Para que serve | Exemplo de sintaxe | Por que usar assim (e não de outro jeito)? |
| --- | --- | --- | --- |
| `string` | Textos e **identificadores com zeros** | `"000123"` | **Por que código de barras é `string` e não `int`?** Primeiro, números descartam zeros à esquerda (`000123` viraria `123`). Segundo, um código EAN-13 de 13 dígitos ultrapassa o limite do `int` (máximo ~2,1 bilhões). Terceiro, nunca somamos nem dividimos códigos de barras! |
| `char` | Um único caractere | `'A'` | Usa **aspas simples `' '`** (diferente de `string`, que usa aspas duplas `" "`). Ideal para uma letra de corredor ou setor. |
| `int` | Números inteiros comuns | `150` | Representa quantidades de unidades inteiras no estoque (até ~2 bilhões). |
| `long` | Números inteiros gigantes | `7891020304050L` | Usa o sufixo **`L`** no final. Necessário para contadores de operações que podem ultrapassar 2 bilhões. |
| `double` | Decimais em base binária (científicos) | `5.45` | Bom para medidas físicas aproximadas (como peso ou altura), onde 1 milionésimo de diferença não importa. |
| `decimal` | Decimais exatos em **base 10** | `25.99m` | **Obrigatório para dinheiro!** Em binário (`double`), `0.1 + 0.2` resulta em `0.30000000000000004`. O tipo `decimal` (sufixo **`m`**) calcula em base 10 e evita erros de centavos. |
| `bool` | Verdadeiro ou falso | `true` / `false` | Indica estados binários, como produto ativo/inativo ou se o cliente quer CPF na nota. |

### Constantes (`const`): Quando um Valor Nunca Deve Mudar
Se uma alíquota ou limite fixo do programa não pode ser alterado acidentalmente no meio do código, declaramos com a palavra-chave **`const`**:

```csharp
const decimal TAXA_PADRAO = 0.15m;
```

- **Por que usar `const`?** Se qualquer linha posterior tentar escrever `TAXA_PADRAO = 0.20m;`, o Visual Studio 2022 acusa erro de compilação imediatamente. Por convenção, constantes usam letras maiúsculas separadas por sublinhado (`MAIUSCULA_COM_SUBLINHADO`).

---

## 3. Conversão de Texto para Número e Formatação Monetária (`CultureInfo`)

Tudo que entra por uma tela ou arquivo de texto chega como `string`. Como C# é fortemente tipado, escrever `decimal preco = "5.50";` não compila: é obrigatório converter explicitamente.

- **`int.Parse("3")`** ou **`Convert.ToInt32("3")`**: convertem texto válido para `int`. (Na Aula 02 aprenderemos por que `TryParse` substitui ambos quando o dado vem digitado pelo usuário).
- **Por que passar `CultureInfo.InvariantCulture` ao converter `"5.50"`?**
  No Windows configurado em português (`pt-BR`), o separador decimal esperado é a **vírgula** (`"5,50"`). Se você tentar converter o texto `"5.50"` (com ponto) sem avisar a cultura, o C# pode interpretar o ponto como separador de milhar e transformar `5.50` em **`550`**! Passando `CultureInfo.InvariantCulture`, você informa explicitamente: *"esse texto usa o ponto internacional"*.
- **Formatando dinheiro para exibição (`"C"`):**
  `valor.ToString("C", CultureInfo.GetCultureInfo("pt-BR"))` usa o especificador **`"C"`** (*Currency* / Moeda) para formatar `16.50m` como `R$ 16,50`. Para evitar escrever `System.Globalization.` toda vez, adicione `using System.Globalization;` no topo do arquivo.

---

## 4. Estruturas de Decisão e Repetição: Quando Escolher Cada Uma?

### Decisões: `if / else if / else` vs `switch / case`

| Estrutura | Quando escolher? | Por que usar assim? |
| --- | --- | --- |
| **`if / else if / else`** | Quando você testa **faixas de valores** (`>= 500`, `>= 200`) ou **várias condições combinadas** (`&&`, `\|\|`). | Avalia em ordem de cima para baixo e entra apenas no **primeiro** bloco verdadeiro. |
| **`switch / case`** | Quando você compara **uma única variável** contra valores exatos (ex.: opção `1`, `2`, `3` de um menu). | Deixa o código muito mais limpo do que vários `else if (opcao == ...)`. Cada `case` termina com `break;` (para não continuar executando os próximos) e `default:` trata qualquer valor fora das opções previstas. |

> [!TIP]
> **Ordem das faixas no `if / else if`:** teste sempre da faixa **mais restrita/alta** para a mais baixa (primeiro `>= 500`, depois `>= 200`). Se você testasse `>= 200` primeiro, uma compra de `R$ 600,00` entraria no `>= 200` e nunca chegaria no desconto maior!

### Repetições (Laços): `for` vs `while` vs `foreach`

| Laço | Quando escolher? | Exemplo de sintaxe (contexto genérico) |
| --- | --- | --- |
| **`for`** | Quando você sabe **exatamente quantas vezes** vai repetir (ex.: imprimir 3 vias). | `for (int i = 1; i <= 3; i++) { ... }` |
| **`while`** | Quando você **não sabe** quantas voltas serão necessárias, mas tem uma **condição de parada** (ex.: repetir enquanto o sensor não estabiliza **e** tentativas < 5). | `while (!pronto && tentativas < 5) { tentativas++; }` |
| **`foreach`** | Quando você quer percorrer **todos os itens de uma coleção/array** um por um, sem se preocupar com índices. | `foreach (string item in lista) { ... }` |

---

## 5. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

No **Visual Studio 2022**, abra o console preparado pelo professor (ou crie um Aplicativo de Console chamado `Aula01Logica`) e **construa no mesmo `Program.cs`** os 8 blocos identificados pelos IDs **`A01.01` a `A01.08`**.

> [!IMPORTANT]
> **Sem leitura do teclado ainda:** nesta aula, defina os valores diretamente nas variáveis do código. A leitura interativa com `Console.ReadLine()` e `TryParse` será aprofundada na Aula 02. Balança, cartão e Pix são apenas mensagens de texto simuladas.

Siga as orientações passo a passo para construir cada bloco:

1. **Preparação do topo do arquivo:**
   - Adicione `using System.Globalization;` na primeira linha.
   - Configure `CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");` para padronizar a formatação de moeda.
2. **Bloco `A01.01` — Plataforma e saída:**
   - Exiba uma mensagem inicial no console e registre no seu caderno três frases diferenciando linguagem C#, compilação (.NET SDK) e execução (.NET Runtime).
3. **Bloco `A01.02` — Declaração dos 7 tipos do domínio:**
   - Declare variáveis para: nome do produto (`string`), setor (`char`, aspas simples), código de barras `"000123"` (`string`), quantidade em estoque (`int`), contador de operações (`long` com sufixo `L`), peso (`double`), preço unitário `5.50m` (`decimal` com sufixo `m`) e estado ativo (`bool`).
   - Exiba todas elas comprovando que `"000123"` preservou os zeros à esquerda.
   - **Experimento de tipagem forte:** tente atribuir um texto `"15.99"` diretamente a uma variável `decimal` (sem conversão), leia o erro na **Lista de Erros** do VS 2022, anote por que o compilador recusou e remova a linha inválida.
4. **Bloco `A01.03` — Constante e cálculo de compra:**
   - Declare `const decimal TAXA_SIMULADA = 0.10m;`.
   - Calcule o subtotal de `3` unidades a `5.50m` (esperado: `R$ 16,50`), o valor da taxa (`subtotal * TAXA_SIMULADA`, esperado: `R$ 1,65`) e o total final (esperado: `R$ 18,15`).
   - **Experimento de constante:** tente atribuir um novo valor a `TAXA_SIMULADA`, leia o erro (`CS0131`) no Visual Studio 2022 e apague a linha inválida.
5. **Bloco `A01.04` — Conversão explícita e moeda:**
   - Converta o texto `"3"` para `int` (com `int.Parse`) e o texto `"5.50"` para `decimal` usando `CultureInfo.InvariantCulture`.
   - Multiplique os dois valores convertidos e exiba o resultado formatado como moeda brasileira (`"C"` → `R$ 16,50`).
6. **Bloco `A01.05` — Regras de desconto (`if / else if / else`) e fronteiras:**
   - Escreva a regra de desconto: compras `>= 500.00m` ganham `R$ 50,00`; compras `>= 200.00m` ganham `R$ 15,00`; abaixo disso, `R$ 0,00`.
   - Teste sua regra com os quatro valores de fronteira: `199.99m`, `200.00m`, `499.99m` e `500.00m` (resultados esperados: `0`, `15`, `15` e `50`).
   - Em seguida, teste uma compra com `quantidade = 0`: crie um `if` que recuse a operação antes de calcular o valor.
7. **Bloco `A01.06` — Menu simulado com `switch / case`:**
   - Crie uma estrutura `switch` avaliando um código inteiro de pagamento: `1` (Dinheiro simulado), `2` (Cartão simulado), `3` (Pix simulado) e `default` (Opção inválida), encerrando cada bloco com `break;`.
   - Execute o `switch` para os quatro casos (`1`, `2`, `3` e um número inválido como `9`) para comprovar todas as saídas.
8. **Bloco `A01.07` — Três vias com `for`:**
   - Use um laço `for` começando em `1`, repetindo enquanto `<= 3` e incrementando `via++`, para exibir `"Via 1"`, `"Via 2"` e `"Via 3"` (não deve imprimir via 4).
9. **Bloco `A01.08` — Simulação com `while` e lista com `foreach`:**
   - Declare `bool balancaEstavel = false;` e `int tentativas = 0;`.
   - Escreva um laço `while (!balancaEstavel && tentativas < 5)` que incremente `tentativas` e exiba o número da tentativa. Explique no caderno o papel dos operadores `!` e `&&`.
   - Crie um array `string[]` com 4 nomes de produtos (`"Arroz"`, `"Feijão"`, `"Macarrão"`, `"Molho"`) e percorra-o com `foreach` exibindo cada item uma única vez.

---

## 6. Erros Comuns no Visual Studio 2022

| Erro na Lista de Erros | Causa provável | Como resolver |
| --- | --- | --- |
| `CS0131: O lado esquerdo de uma atribuição precisa ser uma variável...` | Tentou alterar o valor de uma constante (`const`). | Constantes são imutáveis após a declaração; remova a reatribuição. |
| `CS0163: O controle não pode avançar de um rótulo de case para outro` | Esqueceu o `break;` no final de um `case` no `switch`. | Adicione `break;` ao término de cada `case` e do `default`. |
| `CS1012: Muitos caracteres no literal de caractere` | Usou aspas simples `'Arroz'` em vez de aspas duplas `"Arroz"`. | Aspas simples `' '` são exclusivas para um único `char`; textos usam `" "`. |

[Próxima aula: Aula 02 — Console e depuração](02-Pratica-Console.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 01 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-01). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
