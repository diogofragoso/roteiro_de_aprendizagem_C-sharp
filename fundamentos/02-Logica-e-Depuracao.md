# F02 — Decisões, repetições e depuração

**Pré-requisito:** F01. **Objetivo:** controlar quais instruções executam e quantas vezes executam.

Crie `FundamentosF02`. Cada bloco completo desta página substitui o `Program.cs` anterior.

## 1. Expresse uma condição

```csharp
int quantidade = 0;

if (quantidade > 0)
{
    Console.WriteLine("Quantidade aceita");
}
else
{
    Console.WriteLine("A quantidade precisa ser positiva");
}
```

Uma comparação como `quantidade > 0` produz um `bool`. As chaves delimitam o conjunto de instruções de cada caminho. Teste com 0, 1 e -1 antes de acrescentar mais condições.

| Operador | Leitura |
| --- | --- |
| `==` / `!=` | Igual / diferente |
| `>` / `>=` | Maior / maior ou igual |
| `<` / `<=` | Menor / menor ou igual |
| `&&` | As duas condições precisam ser verdadeiras |
| `\|\|` | Pelo menos uma condição precisa ser verdadeira |
| `!` | Inverte verdadeiro e falso |

`=` atribui; `==` compara. Compare `quantidade >= 1 && quantidade <= 10` com `quantidade == 1 || quantidade == 10`: a primeira condição aceita todo o intervalo; a segunda aceita somente seus extremos.

## 2. Repita até obter uma entrada válida

```csharp
int quantidade;

while (true)
{
    Console.Write("Quantidade positiva ou fim da entrada para sair: ");
    string? entrada = Console.ReadLine();
    if (entrada is null)
    {
        return;
    }

    if (int.TryParse(entrada, out quantidade) && quantidade > 0)
    {
        break;
    }

    Console.WriteLine("Entrada inválida. Tente novamente.");
}

Console.WriteLine($"Quantidade aceita: {quantidade}");
```

`while` repete enquanto a condição for verdadeira. Aqui usamos `true` e saímos explicitamente com `break` quando a entrada atende aos critérios. `return` encerra este programa se a entrada terminar. `string?` indica que a variável pode guardar `null`.

O `&&` só verifica `quantidade > 0` quando `TryParse` retorna verdadeiro. Digite `abc`, depois `0`, depois `3`. Devem aparecer duas recusas e uma aceitação. Identifique qual condição falha em cada recusa.

Um laço precisa ter uma forma de terminar. Se você esquecer de mudar o estado que controla a repetição, pode criar um laço que não termina como esperado. Use **Parar Depuração** na IDE se precisar interromper.

## 3. Conte as repetições

```csharp
int soma = 0;

for (int numero = 1; numero <= 3; numero++)
{
    soma = soma + numero;
    Console.WriteLine($"Número: {numero}; soma: {soma}");
}

Console.WriteLine($"Resultado final: {soma}");
```

No `for`, `int numero = 1` executa uma vez, a condição é conferida antes de cada passagem e `numero++` soma 1 ao final de cada passagem. A variável `numero` existe dentro do laço; `soma` foi declarada fora e pode ser lida depois.

Preencha antes de executar:

| Passagem | numero | soma antes | soma depois |
| --- | ---: | ---: | ---: |
| 1 | 1 | 0 | 1 |
| 2 | 2 | 1 | 3 |
| 3 | 3 | 3 | 6 |

## 4. Observe com o depurador

1. Clique na margem esquerda da linha `soma = soma + numero;` para inserir um breakpoint.
2. Execute com **F5**. O programa pausa antes de executar a linha marcada.
3. Passe o mouse sobre `soma` e `numero` ou consulte a janela **Locais**.
4. Use **F10** para executar a próxima instrução e confira o novo valor.
5. Use **F5** para continuar até a próxima parada. Retire o breakpoint ao concluir.

Se o resultado final fosse 3 porque você escreveu `numero < 3`, o código ainda compilaria. Esse é um erro de lógica. O depurador ajuda a comparar a execução com sua previsão.

## 5. Pratique

1. Aceite uma idade inteira entre 0 e 120. Teste `abc`, `-1`, `0`, `120` e `121`; somente 0 e 120 devem ser aceitos nesse conjunto.
2. Mostre os números de 1 a 5 e depois uma mensagem de fim. Não deve aparecer 0 nem 6.
3. Calcule a soma de 1 a 5: esperado 15. Registre uma passagem com breakpoint.
4. Explique o que muda entre `> 0` e `>= 0` no programa da quantidade.

**Critério para avançar:** prever qual bloco será executado, explicar a parada de um laço e encontrar uma diferença entre valor esperado e observado sem alterar o código por tentativa aleatória.

[Anterior: valores e entrada](01-Valores-e-Entrada.md) · [Próximo: métodos](03-Metodos.md)
