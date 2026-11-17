# F01 — Valores, variáveis, entrada e saída

**Pré-requisito:** F00. **Objetivo:** identificar os dados de um problema e acompanhar como mudam durante a execução.

Crie um console `FundamentosF01` como em F00. Cada exemplo desta página substitui o `Program.cs` anterior, a menos que a instrução diga para acrescentar código.

## 1. Dê nomes aos valores

Uma variável associa um nome a um valor de determinado tipo. O tipo limita quais valores e operações fazem sentido. Comece com quatro tipos:

| Tipo | Exemplo | Uso no exercício |
| --- | --- | --- |
| `string` | `"Lia"` | Texto |
| `int` | `3` | Quantidade inteira |
| `decimal` | `12.50m` | Valor decimal do preço |
| `bool` | `true` | Condição verdadeira ou falsa |

```csharp
string nome = "Caderno";
int quantidade = 2;
decimal preco = 12.50m;
bool disponivel = true;
decimal total = quantidade * preco;

Console.WriteLine(nome);
Console.WriteLine(quantidade);
Console.WriteLine(disponivel);
Console.WriteLine($"Total: {total}");
```

Leia `int quantidade = 2;` como “declare uma variável inteira chamada quantidade e atribua 2”. `=` é atribuição. `*` multiplica. O sufixo `m` informa que `12.50m` é um literal `decimal`. No código, use ponto nesse literal, independentemente de como o Windows apresenta os números.

Em `$"Total: {total}"`, o `$` permite inserir o valor da expressão entre chaves no texto. Isso é interpolação. A apresentação do separador decimal pode depender da cultura do computador; o valor calculado é 25.

## 2. Acompanhe as atribuições

Substitua o programa por:

```csharp
int quantidade = 2;
decimal preco = 12.50m;
decimal total = quantidade * preco;
quantidade = 3;

Console.WriteLine(total);
total = quantidade * preco;
Console.WriteLine(total);
```

A primeira saída representa 25; a segunda, 37,50. `total` guarda o resultado calculado naquele momento. Alterar `quantidade` não refaz uma atribuição que já aconteceu.

**Preveja:** e se a quantidade passasse para 0? Monte uma tabela com linha executada, quantidade, preço e total.

## 3. Leia uma entrada

```csharp
Console.Write("Como você quer ser chamado? ");
string nome = Console.ReadLine() ?? "";
Console.WriteLine($"Olá, {nome}!");
```

`Write` mostra texto sem terminar a linha. `ReadLine` aguarda uma linha de entrada e a devolve como texto; também pode indicar fim da entrada com `null`. `?? ""` usa texto vazio quando o resultado é `null`. Texto vazio e um nome válido ainda são coisas diferentes: a validação será praticada na próxima etapa.

Digitar `3` no teclado produz o texto `"3"`. Para multiplicá-lo como número, precisamos convertê-lo. Use este programa completo:

```csharp
Console.Write("Quantos cadernos? ");
string entrada = Console.ReadLine() ?? "";

if (int.TryParse(entrada, out int quantidade))
{
    decimal total = quantidade * 12.50m;
    Console.WriteLine($"Total: {total}");
}
else
{
    Console.WriteLine("Digite um número inteiro.");
}
```

`TryParse` tenta converter o texto: retorna `true` se conseguiu e coloca o número em `quantidade`, usando `out`. O `if` executa o bloco entre chaves quando esse resultado é verdadeiro; `else` executa o outro bloco. Na próxima etapa vamos construir decisões e repetições com mais calma.

Não é preciso decorar essa chamada agora. Localize a entrada, a tentativa de conversão e as duas saídas possíveis.

## 4. Diferencie conversão de regra

Execute com `2`, `abc` e `-2`. `abc` não é um inteiro. `-2` é um inteiro, então a conversão funciona, mas uma quantidade negativa não serve para essa compra. Uma regra será necessária além da conversão.

Compare também o texto `"2" + "3"`, que produz `"23"`, com `2 + 3`, que produz `5`. O operador `+` concatena textos ou soma números conforme os operandos.

## 5. Pratique

1. Crie variáveis para nome de um livro, quantidade e preço. Calcule o total de 3 unidades a 8,50: esperado 25,50.
2. Altere apenas a quantidade e explique quando o total precisa ser recalculado.
3. Leia um nome e apresente uma mensagem personalizada.
4. Adapte o exemplo de leitura para calcular o dobro de um inteiro. Teste `4`, `0` e `abc`: esperado 8, 0 e mensagem de conversão inválida.
5. Tente atribuir `"três"` a uma variável `int`, leia o diagnóstico e retire a linha inválida.

**Critério para avançar:** distinguir texto de número, explicar uma atribuição e demonstrar uma entrada válida e outra que não pode ser convertida. Não classifique `-2` como falha de conversão.

[Anterior: primeiro programa](00-Primeiro-Programa.md) · [Próximo: lógica e depuração](02-Logica-e-Depuracao.md)
