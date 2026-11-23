# Aula 06 — Composição, Coleções (`List<T>`) e Propriedades Calculadas

**Duração:** 3h30. **Pré-requisito:** [Aula 05](05-Construtores-Encapsulamento-e-Excecoes.md). **Entrega:** objetos colaborando via composição (`ItemDeCompra` + `Produto`) dentro de coleções dinâmicas `List<T>`, cálculo de subtotais via propriedade calculada `=>` e totalização em função separada (`A06.01`–`A06.04`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Revisar encapsulamento e exceções da Aula 05 |
| Explicação | 30 | `List<T>`, Generics, `foreach`, Composição ("tem um") e propriedade calculada `=>` |
| Construção | 60 | Construir `Produto.cs` (encapsulado) e `ItemDeCompra.cs` no Visual Studio 2022 |
| Intervalo | 15 | Pausa |
| Prática | 60 | Montar `List<ItemDeCompra>`, calcular total `R$ 60,30` e testar referências/fronteiras |
| Entrega | 30 | Demonstrar lista vazia/preenchida, objeto compartilhado e explicar composição |

---

## 1. Coleções Dinâmicas com `List<T>` (`A06.01`)

Em um supermercado, um carrinho pode ter 0, 2 ou 50 itens. Diferente de arrays fixos (`string[]`), **`List<T>`** cresce e diminui dinamicamente:

```csharp
List<string> itens = new List<string>();
itens.Add("Arroz");
itens.Add("Feijão");

foreach (string nome in itens)
{
    Console.WriteLine(nome);
}
Console.WriteLine($"Quantidade na lista: {itens.Count}");
```

- **`<T>` (Genérico):** `List<string>` só aceita textos; `List<ItemDeCompra>` só aceita objetos `ItemDeCompra`. O compilador do Visual Studio 2022 impede misturar tipos errados.
- **Lista vazia:** tem `Count == 0`, e o `foreach` simplesmente não executa nenhuma volta (sem lançar erro).

---

## 2. Composição ("Tem Um") e Propriedade Calculada com `=>` (`A06.02`)

No mundo real, objetos colaboram entre si. Pense em um **Item de Compra** no supermercado:
- Um `ItemDeCompra` **não é** um `Produto` (portanto **não** deve herdar de `Produto`!).
- Um `ItemDeCompra` **tem um** `Produto` e uma `Quantidade`. Isso se chama **Composição**.

### Anatomia da Composição e do Atalho `=>` (Microexemplo: `ItemTreino.cs`)

```csharp
public class ItemTreino
{
    public Exercicio Exercicio { get; }
    public int Minutos { get; }

    // Atalho '=>' para propriedade somente leitura calculada sob demanda:
    public int TotalCalorias => Exercicio.CaloriasPorMinuto * Minutos;

    public ItemTreino(Exercicio exercicio, int minutos)
    {
        if (exercicio is null)
            throw new ArgumentException("Informe o exercício.");
        if (minutos <= 0)
            throw new ArgumentException("Informe minutos positivos.");

        Exercicio = exercicio;
        Minutos = minutos;
    }
}
```

**Por que usar composição + propriedade calculada?**
1. **Validar `if (exercicio is null)` no construtor:** garante que `Exercicio.CaloriasPorMinuto` nunca explodirá com `NullReferenceException` depois.
2. **Atalho `=>` (Expression-Bodied Property):** equivale exatamente a `{ get { return Exercicio.CaloriasPorMinuto * Minutos; } }`. Como o subtotal é **calculado na hora da leitura**, ninguém consegue digitar um subtotal errado ou desatualizado!
3. **Navegação por referências (`.`):** `item.Exercicio.Nome` segue a seta do `item` até o objeto `Exercicio` e lê o `Nome` dele.

---

## 3. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

Crie um Aplicativo de Console `Aula06Composicao` no **Visual Studio 2022**:

1. **Aquecimento com `List<string>` (`A06.01`):**
   - Teste criar uma `List<string>`, adicionar dois nomes, percorrer com `foreach` e exibir `Count`. Depois remova temporariamente os `.Add(...)` para comprovar que uma lista vazia exibe `Count: 0` sem erro.
2. **Construir `Produto.cs` encapsulado (`Shift+Alt+C`):**
   - Crie `public class Produto` com propriedades somente leitura `string Nome { get; }` e `decimal Preco { get; }`.
   - No construtor `public Produto(string nome, decimal preco)`, recuse com `ArgumentException` se `string.IsNullOrWhiteSpace(nome)` ou `preco <= 0`; guarde `Nome = nome.Trim()` e `Preco = preco`.
3. **Construir `ItemDeCompra.cs` com composição e subtotal calculado (`A06.02`):**
   - Crie `public class ItemDeCompra` com:
     - `public Produto Produto { get; }`
     - `public int Quantidade { get; }`
     - `public decimal Subtotal => Produto.Preco * Quantidade;`
   - No construtor `public ItemDeCompra(Produto produto, int quantidade)`, recuse com `ArgumentException` se `produto is null` ou `quantidade <= 0`.
4. **Totalizar a compra no `Program.cs` (`A06.03` e `A06.04`):**
   - Configure `CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");`.
   - Crie dois produtos: `arroz` (`"Arroz"`, `25.90m`) e `feijao` (`"Feijão"`, `8.50m`).
   - Crie `List<ItemDeCompra> itens = new List<ItemDeCompra>();` e adicione `2` arrozes e `1` feijão.
   - Escreva uma função local `decimal SomarItens(List<ItemDeCompra> lista)` no final do `Program.cs` que apenas percorre com `foreach` e retorna a soma dos `Subtotal` (sem imprimir nada!).
   - Exiba cada linha (`item.Produto.Nome`, `item.Quantidade`, `item.Subtotal:C`) e o retorno de `SomarItens(itens)` (esperado: **`R$ 60,30`**).
   - **Teste de referência compartilhada:** adicione um **terceiro** `ItemDeCompra` usando a **mesma variável** `arroz` com quantidade `1` (novo total: **`R$ 86,20`**). Desenhe no caderno quantos objetos `Produto` existem (2) e quantos objetos `ItemDeCompra` existem (3).
   - **Testes de fronteira:** chame `SomarItens` com uma lista vazia (esperado: `0`), tente criar `new ItemDeCompra(arroz, 0)` dentro de `try/catch` (recusado antes de entrar na lista) e explique no caderno por que uma `List<ItemDeCompra>` na RAM não substitui um banco de dados.

[Anterior: Aula 05](05-Construtores-Encapsulamento-e-Excecoes.md) · [Próxima: Aula 07 — Namespaces, interfaces, herança e polimorfismo](07-Namespaces-Interfaces-Heranca-e-Polimorfismo.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 06 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-06). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
