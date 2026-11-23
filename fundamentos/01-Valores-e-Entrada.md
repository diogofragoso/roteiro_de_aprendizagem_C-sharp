# F01 — Valores, Variáveis, Tipos e Entrada do Teclado

**Pré-requisito:** [F00](00-Primeiro-Programa.md).
**Objetivo:** entender como o computador guarda informações na memória usando variáveis tipadas, acompanhar como os valores mudam linha a linha e ler dados digitados pelo usuário com segurança usando `TryParse`.

Crie no **Visual Studio 2022** um novo **Aplicativo de Console** chamado `FundamentosF01` seguindo os mesmos passos de `F00`.

---

## 1. Por Que Variáveis Precisam de Tipos? (A Memória RAM)

Imagine a memória RAM do computador como um grande armário cheio de gavetas. Uma **variável** é uma gaveta com:
1. Um **tipo** (define o formato do que cabe ali e quais operações são permitidas);
2. Um **nome** (o rótulo que você usa no código para encontrar a gaveta);
3. Um **valor** (o conteúdo guardado ali dentro naquele instante).

**Por que C# exige declarar o tipo de cada variável?**
C# é uma linguagem **fortemente tipada**. Se você cria uma gaveta de número inteiro (`int`), o compilador do Visual Studio 2022 impede que você tente guardar um texto `"abacaxi"` dentro dela. Isso descobre bugs graves **antes** de o programa rodar na mão do usuário.

### Os 4 tipos essenciais para começar

| Tipo em C# | O que armazena | Exemplo de literal no código | Por que escolher este tipo? |
| --- | --- | --- | --- |
| `string` | Texto (zero, um ou muitos caracteres) | `"Caderno"` ou `""` | Tudo que é nome, frase, código com zero à esquerda (`"0012"`). Sempre entre aspas duplas `"..."`. |
| `int` | Número inteiro (positivo, negativo ou zero) | `3`, `0`, `-5` | Quantidades contáveis (unidades, idade, páginas). Não aceita casas decimais. |
| `decimal` | Número com casas decimais de **alta precisão na base 10** | `12.50m` | **Obrigatório para dinheiro e preços.** O sufixo `m` (de *money*) avisa ao compilador que o número é `decimal` e não `double`. |
| `bool` | Valor lógico (apenas verdadeiro ou falso) | `true` ou `false` | Perguntas de sim/não (ex.: está disponível? tem desconto?). |

> [!IMPORTANT]
> **Ponto no código vs Vírgula na tela:** Dentro do código-fonte C#, números decimais **sempre** usam ponto e o sufixo `m` (`12.50m`), porque a gramática da linguagem segue o padrão internacional. Já quando o programa exibe na tela no Windows em português, ele mostra `12,50`.

### Anatomia da Declaração e da Interpolação (Microexemplo)

Observe este pequeno exemplo de uma passagem de ônibus:

```csharp
string destino = "Campinas";
int passagens = 2;
decimal valorUnitario = 14.50m;
bool idaEVolta = true;
decimal total = passagens * valorUnitario;

Console.WriteLine($"Destino: {destino} | Total: {total}");
```

Dissecando cada detalhe:
- `int passagens = 2;`: leia da direita para a esquerda — *"crie uma variável do tipo `int` chamada `passagens` e **atribua** (`=`) o valor `2` dentro dela"*.
- **O símbolo `=` em C# NÃO significa "igualdade matemática"**, e sim **atribuição** ("guarde o resultado da direita dentro da variável da esquerda").
- `*` é o operador de multiplicação (`+` soma, `-` subtrai, `/` divide).
- `$"Destino: {destino} | Total: {total}"`: o símbolo `$` antes das aspas ativa a **interpolação de string**. Tudo que estiver entre chaves `{ }` é avaliado e substituído pelo valor atual da variável. **Por que usar interpolação?** Fica muito mais legível e menos propenso a erros de espaço do que colar pedaços com `+`.

---

## 2. Como a Memória Muda ao Longo do Tempo (Atribuição Não É Fórmula de Planilha)

Um erro muito comum de iniciantes é achar que uma variável funciona como fórmula do Excel (que recalcula sozinha quando outra célula muda). **Em programação, não funciona assim!**

Observe o rastreamento linha a linha abaixo:

```csharp
int ingressos = 2;
decimal preco = 10.00m;
decimal total = ingressos * preco; // Calcula AGORA: 2 * 10 = 20
ingressos = 5;                     // Muda APENAS 'ingressos' para 5!
Console.WriteLine(total);          // Ainda mostra 20!
```

| Linha executada | `ingressos` na RAM | `preco` na RAM | `total` na RAM |
| --- | ---: | ---: | ---: |
| `int ingressos = 2;` | 2 | — | — |
| `decimal preco = 10.00m;` | 2 | 10.00 | — |
| `decimal total = ingressos * preco;` | 2 | 10.00 | 20.00 |
| `ingressos = 5;` | 5 | 10.00 | **20.00** (não mudou!) |

**Por quê?** O computador executa uma instrução por vez. `total` guardou o número `20.00`, não a "fórmula". Se você quiser atualizar `total` depois de mudar `ingressos`, precisa fazer uma nova atribuição: `total = ingressos * preco;`. (Note que na segunda vez **não** escrevemos `decimal` na frente de `total`, porque a variável já existe na memória — escrever o tipo de novo geraria erro de variável duplicada).

---

## 3. Lendo Dados do Teclado e Convertendo com Segurança

Até agora os valores estavam fixos no código. Para perguntar algo ao usuário no console, usamos duas operações:

- `Console.Write("Pergunta: ");`: mostra o texto e **mantém o cursor na mesma linha** (diferente do `WriteLine`, que pula linha).
- `Console.ReadLine()`: pausa o programa, espera o usuário digitar e apertar `Enter`, e devolve **sempre um texto (`string?`)**.

### Por que aparece `?? ""` depois de `Console.ReadLine()`?

O sinal `?` em `string?` significa que `ReadLine()` pode devolver `null` (ausência total de texto — por exemplo, se o fluxo de entrada for encerrado com `Ctrl+Z`).
O operador `??` (**coalescência nula**) funciona como um plano B: *"use o texto da esquerda; mas se ele for `null`, use o texto vazio `""` da direita"*.

> [!WARNING]
> Usar `?? ""` evita o aviso de nulabilidade do compilador, mas **não valida se o usuário digitou um texto útil**. Se o usuário apenas apertar `Enter`, o resultado será `""` (vazio). Veremos como recusar texto vazio em `F02`.

### Por que precisamos converter texto em número (`TryParse` e `out`)?

Mesmo quando o usuário digita `3` no teclado, `Console.ReadLine()` devolve o **texto** `"3"`, não o número `3`. Veja a diferença do operador `+`:
- `"2" + "3"` resulta em `"23"` (junta dois textos — **concatenação**).
- `2 + 3` resulta em `5` (soma matemática).

Para transformar texto em inteiro **sem derrubar o programa** caso o usuário digite `"abc"`, usamos `int.TryParse`:

```csharp
Console.Write("Quantos ingressos? ");
string entrada = Console.ReadLine() ?? "";

if (int.TryParse(entrada, out int quantidade))
{
    Console.WriteLine($"Conversão deu certo! Número: {quantidade}");
}
else
{
    Console.WriteLine("Você não digitou um número inteiro válido.");
}
```

**Anatomia completa de `int.TryParse(entrada, out int quantidade)`:**

| Peça | Significado | Por que funciona assim? |
| --- | --- | --- |
| `int.TryParse(...)` | "Tente converter (`Try` = tentar, `Parse` = analisar texto) para `int`". | Diferente de `int.Parse` (que explode o programa com erro se vier `"abc"`), o `TryParse` **nunca trava** o programa: ele apenas devolve `true` (conseguiu) ou `false` (falhou). |
| `entrada` | O texto que queremos tentar converter. | É o dado bruto digitado pelo usuário. |
| `out int quantidade` | Parâmetro de **saída (`out`)**. | Como o retorno do método já foi ocupado pelo `bool` (`true`/`false`), o modificador `out` cria uma segunda "porta de saída" para entregar o número convertido dentro da variável `quantidade`. |
| `if (...) { ... } else { ... }` | Estrutura de decisão ("se ... senão ..."). | Se `TryParse` devolveu `true`, entra no primeiro par de chaves `{ }`; se devolveu `false`, entra no bloco `else`. |

### Diferença essencial: Falha de Conversão vs Regra de Negócio

Teste mentalmente três entradas para "Quantos ingressos?":
1. `"2"` → `TryParse` retorna `true` e `quantidade` recebe `2`. (Conversão OK, regra OK).
2. `"abc"` → `TryParse` retorna `false`. (**Falha de conversão**: `"abc"` não é um número inteiro).
3. `"-2"` → `TryParse` retorna **`true`** e `quantidade` recebe `-2`! **Por quê?** Porque `-2` **é** um número inteiro válido! O problema dele não é conversão, é **regra de negócio** (ninguém compra -2 ingressos). Aprenderemos a combinar conversão + regra na etapa `F02`.

---

## 4. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

No projeto `FundamentosF01`, construa cada exercício substituindo ou comentando o anterior em `Program.cs`. **Escreva o código seguindo as orientações — não copie o exemplo da passagem:**

### Construção 1 — Cálculo com Variáveis Tipadas
1. Declare três variáveis representando a compra de um **livro**:
   - Nome do livro (texto);
   - Quantidade comprada (`int`, começando com `3`);
   - Preço unitário (`decimal`, valendo `8.50m`).
2. Declare uma quarta variável `decimal` para guardar o `total` (multiplicação da quantidade pelo preço).
3. Mostre no console, usando interpolação `$"..."`, o nome do livro, a quantidade, o preço e o total (resultado esperado: `25,50`).
4. **Experimento de memória:** logo após mostrar o resultado, altere a quantidade para `5` (sem redeclarar o tipo `int`). Mostre o `total` novamente **antes** de recalculá-lo, depois faça o recálculo e mostre o `total` atualizado. Anote no caderno por que os dois números exibidos foram diferentes.

### Construção 2 — Saudação Interativa
1. Pergunte o nome do leitor usando `Console.Write` (sem pular linha).
2. Leia a resposta com `Console.ReadLine()` protegendo contra `null` com `?? ""`.
3. Exiba uma mensagem personalizada de boas-vindas usando interpolação.

### Construção 3 — Dobro com Conversão Segura (`TryParse`)
1. Peça ao usuário que digite um número inteiro.
2. Guarde o texto digitado em uma variável `string`.
3. Use `if (int.TryParse(..., out int numero))` para tentar converter o texto.
4. No bloco do `if` (quando deu certo): calcule o dobro do número em uma variável `int` e exiba o resultado formatado.
5. No bloco do `else` (quando falhou): exiba uma mensagem clara informando que o texto digitado não é um número inteiro.
6. **Teste com 4 entradas diferentes:** execute o programa quatro vezes (com `Ctrl+F5`) digitando `4` (esperado: `8`), `0` (esperado: `0`), `-3` (esperado: `-6` — comprove que negativo converte normalmente!) e `abc` (esperado: mensagem de erro de conversão).

### Construção 4 — Testando a Tipagem Forte do Compilador
1. Em uma linha temporária do `Program.cs`, tente escrever `int teste = "três";`.
2. Olhe o sublinhado vermelho no Visual Studio 2022 e leia a mensagem exata na **Lista de Erros** (código `CS0029`).
3. Anote por que o compilador recusou essa linha e apague-a para o projeto voltar a compilar limpo.

---

## 5. Erros Comuns no Visual Studio 2022

| Erro na Lista de Erros | Causa provável | Como corrigir |
| --- | --- | --- |
| `CS0664: Literal de tipo double não pode ser implicitamente convertido em decimal` | Você escreveu `decimal preco = 8.50;` e esqueceu o sufixo `m`. | Coloque `m` no final do número: `8.50m`. |
| `CS0128: Uma variável local chamada '...' já está definida` | Você colocou o tipo (`int`, `decimal`) novamente ao alterar uma variável que já existia. | Para alterar o valor de uma variável existente, escreva apenas `quantidade = 5;`. |
| `CS8600: Conversão de literal nula...` | Usou `Console.ReadLine()` direto em uma `string` sem `?? ""`. | Complete com `?? ""` ou declare `string?`. |

**Critério para avançar para F02:** distinguir texto de número, explicar por que alterar `quantidade` não recalcula `total` automaticamente, demonstrar `TryParse` funcionando para entrada válida e inválida e explicar por que `-3` não é falha de conversão.

[Anterior: F00 — Primeiro programa](00-Primeiro-Programa.md) · [Próximo: F02 — Decisões, repetições e depuração](02-Logica-e-Depuracao.md)
