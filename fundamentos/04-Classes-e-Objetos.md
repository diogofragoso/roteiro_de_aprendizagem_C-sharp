# F04 — Classes, objetos, propriedades e referências

**Pré-requisito:** F03. **Objetivo:** agrupar dados relacionados e explicar a diferença entre classe, objeto e variável que referencia um objeto.

## 1. Por que agrupar dados?

Para representar um livro, usamos título e número de páginas. Para dois livros, poderíamos criar `titulo1`, `paginas1`, `titulo2` e `paginas2`. Conforme a quantidade cresce, fica mais difícil saber quais valores pertencem ao mesmo livro.

Uma classe define um tipo que reúne dados e operações pertinentes ao problema. Aqui queremos acompanhar a leitura: título e páginas bastam. Não precisamos representar todos os detalhes de um livro real. Escolher os detalhes relevantes é uma forma de **abstração**.

## 2. Crie a classe em outro arquivo

Crie o console `FundamentosF04`. Pelo projeto, use **Adicionar → Classe**, nomeie `Livro.cs` e substitua o conteúdo inteiro pelo exemplo abaixo. Para este exercício, deixe a classe sem `namespace`; a organização em namespaces entra em F07.

```csharp
public class Livro
{
    public string Titulo { get; set; } = "";
    public int Paginas { get; set; }

    public string Descrever()
    {
        return $"{Titulo}: {Paginas} páginas";
    }
}
```

`class Livro` declara o tipo. `Titulo` e `Paginas` são **propriedades**: `get` permite ler e `set` permite atribuir. Neste primeiro exemplo, o acesso é público e não há validação. Isso será melhorado em F05. `= ""` define o texto inicial; a propriedade inteira começa em 0.

`Descrever` é um método de instância: trabalha com as propriedades do objeto que recebeu a chamada. `public` permite que o código fora da classe o chame. O método tem retorno `string` e não recebe argumentos.

## 3. Construa dois objetos

Substitua o conteúdo de `Program.cs` por:

```csharp
Livro primeiro = new Livro();
primeiro.Titulo = "Viagem";
primeiro.Paginas = 120;

Livro segundo = new Livro();
segundo.Titulo = "Jardim";
segundo.Paginas = 80;

Console.WriteLine(primeiro.Descrever());
Console.WriteLine(segundo.Descrever());

segundo.Paginas = 90;
Console.WriteLine(primeiro.Paginas);
Console.WriteLine(segundo.Paginas);
```

`new Livro()` cria um objeto, também chamado de instância. Cada uso de `new` aqui cria um livro diferente. O ponto em `primeiro.Titulo` acessa um membro do objeto. Os parênteses em `Descrever()` indicam uma chamada de método.

O conjunto dos valores de um objeto naquele momento é seu **estado**. Depois da alteração, o primeiro continua com 120 páginas e o segundo tem 90. A classe define a estrutura; esses objetos têm estados próprios.

Mantenha instruções de execução no `Program.cs` e a declaração da classe no `Livro.cs`. Não cole instruções soltas depois de uma declaração de classe no mesmo arquivo.

## 4. Atribuir uma referência não copia o objeto

Acrescente ao final de `Program.cs`:

```csharp
Livro outroNome = primeiro;
outroNome.Paginas = 150;

Console.WriteLine(primeiro.Paginas);
Console.WriteLine(outroNome.Paginas);
Console.WriteLine(segundo.Paginas);
```

As saídas adicionais são 150, 150 e 90. `outroNome = primeiro` copia a referência ao mesmo objeto; não contém `new` e não cria outro livro.

```text
primeiro ────┐
             ├──> Livro com título Viagem e 150 páginas
outroNome ───┘

segundo ─────────> Livro com título Jardim e 90 páginas
```

Compare com o inteiro de F03: copiar um inteiro fornece outro valor independente; copiar uma referência de classe permite alcançar o mesmo objeto. Isso será importante quando vários itens compartilharem um produto.

## 5. Pratique

1. Crie `Aluno.cs` com propriedade `Nome` e método `Apresentar` que devolve uma mensagem. Crie dois alunos com nomes diferentes e mostre as duas mensagens.
2. Mude o nome de apenas um dos alunos. O outro deve permanecer igual.
3. Crie uma segunda variável referenciando o primeiro aluno. Altere o nome por ela e explique por que a primeira variável observa a alteração.
4. No exemplo dos livros, conte as variáveis de referência e os objetos após o último trecho: são três variáveis apontando para dois objetos.
5. Indique no seu código uma classe, uma propriedade, uma chamada, uma criação de instância e o estado de um objeto.

**Critério para avançar:** escrever uma classe simples em arquivo separado, criar dois objetos e explicar por que atribuição de referência e criação com `new` produzem comportamentos diferentes.

[Anterior: métodos](03-Metodos.md) · [Próximo: construtores e encapsulamento](05-Construtores-e-Encapsulamento.md)
