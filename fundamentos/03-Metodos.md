# F03 — Métodos, parâmetros, retorno e escopo

**Pré-requisito:** F02. **Objetivo:** dar nome a uma operação e reutilizá-la com entradas diferentes.

Crie `FundamentosF03`. Nesta etapa, vamos usar funções locais dentro do `Program.cs`. Elas permitem aprender declaração, chamada, parâmetros e retorno antes de escrever métodos de uma classe.

## 1. Separe uma operação

Imagine que o programa calcula várias vezes o preço de uma quantidade. Copiar a multiplicação em muitos lugares aumenta o trabalho se a regra mudar. Podemos nomear a operação:

```csharp
decimal primeiroTotal = CalcularTotal(12.50m, 2);
decimal segundoTotal = CalcularTotal(8m, 3);

Console.WriteLine(primeiroTotal);
Console.WriteLine(segundoTotal);

decimal CalcularTotal(decimal preco, int quantidade)
{
    decimal resultado = preco * quantidade;
    return resultado;
}
```

As saídas representam 25 e 24. A declaração da função descreve a operação; cada chamada a executa. O código entre as chaves não roda por estar escrito ali: roda quando a função é chamada.

| Parte | Significado |
| --- | --- |
| Primeiro `decimal` | Tipo do valor que a função devolve |
| `CalcularTotal` | Nome da operação |
| `decimal preco, int quantidade` | Parâmetros: nomes e tipos recebidos pela função |
| `12.50m, 2` na chamada | Argumentos: valores usados nessa execução |
| `return resultado;` | Encerra a função e entrega um valor ao chamador |

No retorno da primeira chamada, o valor 25 é atribuído a `primeiroTotal`. `Console.WriteLine` só apresenta esse valor depois. Calcular e apresentar são operações diferentes.

## 2. Compare retorno com mensagem

Substitua o programa inteiro por:

```csharp
decimal total = CalcularTotal(10m, 2);
MostrarTotal(total);

decimal CalcularTotal(decimal preco, int quantidade)
{
    return preco * quantidade;
}

void MostrarTotal(decimal valor)
{
    Console.WriteLine($"Total calculado: {valor}");
}
```

`void` informa que a função não devolve um valor ao chamador. Ela pode realizar uma ação, como mostrar uma mensagem. `MostrarTotal` não pode ser usado como se devolvesse um `decimal`.

O método de cálculo não lê teclado nem escreve no console. Assim, o mesmo cálculo poderá ser chamado por uma tela no futuro.

## 3. Entenda o alcance dos nomes

Os parâmetros `preco` e `quantidade` e a variável `resultado` do primeiro exemplo pertencem à execução da função. Você não pode usar `resultado` fora dela só porque o nome aparece no arquivo. Isso é uma questão de **escopo**.

Substitua o programa por este experimento:

```csharp
int numero = 3;
int dobro = Dobrar(numero);
Console.WriteLine(numero);
Console.WriteLine(dobro);

int Dobrar(int valor)
{
    valor = valor * 2;
    return valor;
}
```

A saída é 3 e 6. Nesse caso, o parâmetro recebe uma cópia do valor inteiro. Alterar `valor` não altera `numero` no chamador.

## 4. Acompanhe uma chamada

Coloque um breakpoint na chamada a `Dobrar`. Use **F11** para entrar na função e **F10** para acompanhar suas instruções. Anote o argumento recebido e o valor que retorna. Observe que o fluxo volta ao ponto da chamada.

Ao introduzir classes, escreveremos essas operações como métodos dentro delas. Alguns métodos agirão sobre o estado de um objeto; por isso será necessário entender quem recebeu a chamada.

## 5. Pratique

1. Escreva `Somar(int primeiro, int segundo)` com retorno `int`. Teste 2 e 3: esperado 5; teste -1 e 1: esperado 0.
2. Escreva `CalcularMedia(decimal primeira, decimal segunda)`. Teste 6 e 8: esperado 7. Use `2m` na divisão para manter a operação decimal.
3. Escreva `EhQuantidadeValida(int quantidade)` retornando `bool`. Teste -1, 0 e 2: esperado falso, falso e verdadeiro.
4. Reutilize `EhQuantidadeValida` em uma decisão no console, deixando a mensagem fora da função.
5. Tente ler o parâmetro `primeiro` fora de `Somar`, registre o erro de escopo e retire a linha inválida.

**Critério para avançar:** distinguir declaração de chamada, parâmetro de argumento e retorno de mensagem; escrever e chamar uma função pequena sem copiar o exemplo completo.

[Anterior: lógica](02-Logica-e-Depuracao.md) · [Próximo: classes e objetos](04-Classes-e-Objetos.md)
