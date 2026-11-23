# F03 — Métodos, Parâmetros, Retorno e Escopo

**Pré-requisito:** [F02](02-Logica-e-Depuracao.md).
**Objetivo:** dar nome a operações reutilizáveis, separar cálculo de exibição, entender escopo de variáveis e entrar dentro de chamadas usando o depurador (`F11`) do Visual Studio 2022.

Crie no **Visual Studio 2022** um novo **Aplicativo de Console** chamado `FundamentosF03`.
Nesta etapa, escreveremos **funções locais** dentro do próprio `Program.cs`. **Por quê?** Assim você aprende declaração, chamada, parâmetros e retorno antes de misturar isso com classes e múltiplos arquivos (que entram em `F04`).

---

## 1. Por Que Criar Métodos em Vez de Repetir Código?

Imagine um programa que calcula a área de terrenos em 10 lugares diferentes. Se você copiar a fórmula em 10 pontos e a regra mudar depois (por exemplo, descontando uma faixa de recuo), terá que caçar e consertar 10 lugares — e fatalmente esquecerá algum.

Um **método** (ou função) empacota uma operação sob um nome claro. Você escreve a lógica **uma vez** e a **chama** quantas vezes precisar com dados diferentes.

### Anatomia Completa de um Método (Microexemplo)

Observe este modelo de cálculo de área de um retângulo:

```csharp
// Chamadas (onde a função realmente executa)
decimal areaSala = CalcularArea(4.5m, 3m);
decimal areaQuarto = CalcularArea(3m, 3m);

Console.WriteLine($"Sala: {areaSala} m²");
Console.WriteLine($"Quarto: {areaQuarto} m²");

// Declaração da função (define a receita; não executa sozinha!)
decimal CalcularArea(decimal largura, decimal comprimento)
{
    decimal resultado = largura * comprimento;
    return resultado;
}
```

Dissecando cada elemento dessa estrutura:

| Parte | O que é? | Por que existe? |
| --- | --- | --- |
| `decimal` (antes do nome) | **Tipo de retorno.** | Promete ao compilador qual tipo de dado a função devolverá para quem a chamou. |
| `CalcularArea` | **Nome do método.** | Em C#, usamos **PascalCase** (cada palavra começa com maiúscula) e verbos no infinitivo para deixar clara a ação. |
| `(decimal largura, decimal comprimento)` | **Parâmetros.** | São as "entradas" que a função precisa receber. Funcionam como variáveis locais criadas no momento da chamada. |
| `4.5m, 3m` na chamada | **Argumentos.** | São os valores concretos entregues para preencher os parâmetros naquela execução específica. |
| `return resultado;` | **Instrução de retorno.** | Encerra a função imediatamente e devolve o valor para o ponto onde ela foi chamada (que guarda em `areaSala`). |

> [!NOTE]
> **Declaração não é Execução!** O código entre `{ }` de `CalcularArea` só roda quando outra linha chama `CalcularArea(...)`. Se você declarar uma função e nunca chamá-la, ela nunca será executada.

---

## 2. Retornar um Valor vs Mostrar na Tela (`void`)

Uma das decisões de engenharia mais importantes que você vai aprender desde cedo é **separar quem calcula de quem exibe na tela**:

- Uma função de **cálculo** devolve um resultado (`decimal`, `int`, `bool`, `string`) usando `return` e **não** chama `Console.WriteLine` nem `Console.ReadLine`. **Por quê?** Porque amanhã você poderá usar exatamente o mesmo cálculo em uma tela Windows Forms, em um site ou em um teste automatizado — onde não existe tela preta de console!
- Uma função de **ação** que apenas exibe algo e não devolve nenhum valor usa a palavra-chave **`void`** (vazio) no lugar do tipo de retorno:

```csharp
void ExibirArea(decimal valor)
{
    Console.WriteLine($"Área calculada: {valor} m²");
}
```

Como `ExibirArea` retorna `void`, escrever `decimal x = ExibirArea(10m);` gera erro de compilação: não existe nenhum valor sendo devolvido para guardar em `x`.

---

## 3. Escopo e Passagem por Cópia de Valor

### O que é Escopo?
**Escopo** é a região do código onde uma variável "existe e pode ser vista". Em C#, cada par de chaves `{ }` cria um escopo.
- As variáveis declaradas dentro de `CalcularArea` (como `largura`, `comprimento` e `resultado`) **nascem quando a função é chamada e morrem quando ela termina**.
- Tentar usar `resultado` fora de `CalcularArea` dá erro no Visual Studio 2022 (`CS0103`), mesmo que ambos estejam no mesmo arquivo `Program.cs`. **Por que isso é bom?** Evita que uma função altere acidentalmente variáveis de outra parte do programa.

### Cópia de Valor (Tipos Primitivos)
Quando você passa uma variável `int` ou `decimal` para um parâmetro, a função recebe uma **cópia** do número:

```csharp
int pontos = 10;
int bonus = SomarBonus(pontos); // Passa uma cópia do 10
// Aqui 'pontos' continua sendo 10, e 'bonus' vale 15!

int SomarBonus(int valor)
{
    valor = valor + 5; // Altera apenas a cópia local 'valor'
    return valor;
}
```

---

## 4. Depurando Chamadas de Método no Visual Studio 2022 (`F11` vs `F10`)

Agora que seu código salta para dentro de funções e volta, existe um novo atalho essencial no Visual Studio 2022:

1. Coloque um Breakpoint (`F9`) na linha que **chama** uma função sua (ex.: `decimal media = CalcularMedia(6m, 8m);`).
2. Inicie com **`F5`**.
3. Quando parar na linha amarela da chamada:
   - Se você apertar **`F10`** (*Contornar / Step Over*), o Visual Studio executa a função inteira de uma vez e já vai para a linha seguinte.
   - Se você apertar **`F11`** (*Intervir / Step Into*), o cursor amarelo **entra** na primeira linha de dentro da função!
4. Lá dentro, observe na janela **Locais** que aparecem apenas os parâmetros e variáveis daquela função. Pressione `F10` até o `return` e observe a seta amarela voltar exatamente para o ponto da chamada trazendo o resultado!

---

## 5. Roteiro de Construção Guiada (Sua Vez!)

No projeto `FundamentosF03`, construa as quatro funções abaixo no `Program.cs` seguindo as orientações. Deixe as chamadas e exibições na parte superior do arquivo e as declarações das funções na parte inferior:

### Construção 1 — Função `Somar`
1. Declare uma função que retorna `int`, chamada `Somar`, recebendo dois parâmetros inteiros (`primeiro` e `segundo`).
2. Retorne a soma dos dois parâmetros (sem usar `Console.WriteLine` dentro dela).
3. Na parte superior do `Program.cs`, chame `Somar(2, 3)` e `Somar(-1, 1)` guardando os retornos em variáveis e exiba-os (esperado: `5` e `0`).

### Construção 2 — Função `CalcularMedia`
1. Declare uma função que retorna `decimal`, chamada `CalcularMedia`, recebendo duas notas `decimal` (`primeira` e `segunda`).
2. Calcule `(primeira + segunda) / 2m` e retorne o resultado.
   - **Por que `2m` e parênteses?** Os parênteses garantem que a soma aconteça antes da divisão (precedência matemática) e `2m` mantém a operação em `decimal`.
3. Teste com `6m` e `8m` (esperado: `7`) e com `7.5m` e `8.0m` (esperado: `7,75`).

### Construção 3 — Função de Validação `EhQuantidadeValida`
1. Declare uma função que retorna `bool`, chamada `EhQuantidadeValida`, recebendo `int quantidade`.
2. Retorne `true` se `quantidade > 0` e `false` caso contrário (dica: você pode retornar diretamente a comparação `return quantidade > 0;`!).
3. Na parte superior do `Program.cs`, teste a função com `-1`, `0` e `2`. Use o retorno dela dentro de um `if / else` no console para exibir `"Quantidade aceita"` ou `"Quantidade inválida"` — mantendo a mensagem **fora** da função.

### Construção 4 — Teste de Escopo e Depuração com `F11`
1. Tente escrever `Console.WriteLine(primeiro);` fora da função `Somar`. Leia a mensagem de erro na **Lista de Erros** do Visual Studio 2022, anote o motivo (escopo) e remova a linha inválida.
2. Coloque um Breakpoint (`F9`) na chamada a `CalcularMedia`, rode com `F5` e use **`F11`** para entrar na função e acompanhar o retorno.

---

## 6. Erros Comuns no Visual Studio 2022

| Erro na Lista de Erros | Causa provável | Como corrigir |
| --- | --- | --- |
| `CS0161: Nem todos os caminhos de código retornam um valor` | A função prometeu retornar `int`/`decimal`/`bool`, mas falta um `return` em algum ramo (ex.: no `else`). | Garanta que todo caminho possível termine com `return`. |
| `CS0029: Não é possível converter implicitamente void em ...` | Você tentou guardar em uma variável o resultado de uma função `void`. | Funções `void` são apenas chamadas (`Mostrar(x);`), nunca atribuídas (`y = Mostrar(x);`). |
| `CS7036: Não há nenhum argumento fornecido que corresponda ao parâmetro` | Você chamou a função passando menos (ou mais) argumentos do que os parâmetros declarados. | Confira a quantidade e a ordem dos tipos dentro dos parênteses. |

**Critério para avançar para F04:** distinguir declaração de chamada, parâmetro de argumento e retorno de mensagem no console; construir e chamar funções pequenas com retorno e demonstrar a entrada em uma função usando `F11`.

[Anterior: F02 — Lógica e depuração](02-Logica-e-Depuracao.md) · [Próximo: F04 — Classes, objetos e referências](04-Classes-e-Objetos.md)
