# F06 — Composição: objetos que trabalham juntos

**Pré-requisito:** F05. **Objetivo:** representar uma relação entre objetos e percorrer uma coleção.

Crie o console `FundamentosF06`. Agora faremos uma pequena lista de materiais, ainda sem banco, interface gráfica ou projeto de supermercado.

## 1. Comece com uma coleção de valores

Substitua `Program.cs` por:

```csharp
List<string> nomes = new List<string>();
nomes.Add("Caderno");
nomes.Add("Caneta");

foreach (string nome in nomes)
{
    Console.WriteLine(nome);
}

Console.WriteLine($"Itens: {nomes.Count}");
```

`List<string>` é uma lista cujos elementos são textos. `Add` acrescenta um elemento; `Count` informa quantos há. `foreach` recebe cada elemento, um por vez, na variável `nome`. O resultado é Caderno, Caneta e a contagem 2.

Uma lista vazia tem contagem 0. Percorrê-la não executa o corpo do `foreach` nenhuma vez. Antes de trabalhar com objetos, retire os dois `Add` e confira esse comportamento.

## 2. Modele material e quantidade

Crie `Material.cs`:

```csharp
public class Material
{
    public string Nome { get; }
    public decimal Preco { get; }

    public Material(string nome, decimal preco)
    {
        if (string.IsNullOrWhiteSpace(nome) || preco <= 0)
            throw new ArgumentException("Informe nome e preço positivo.");

        Nome = nome.Trim();
        Preco = preco;
    }
}
```

Crie `ItemMaterial.cs`:

```csharp
public class ItemMaterial
{
    public Material Material { get; }
    public int Quantidade { get; }

    public decimal Subtotal
    {
        get { return Material.Preco * Quantidade; }
    }

    public ItemMaterial(Material material, int quantidade)
    {
        if (material is null)
            throw new ArgumentException("Informe o material.");
        if (quantidade <= 0)
            throw new ArgumentException("Informe quantidade positiva.");

        Material = material;
        Quantidade = quantidade;
    }
}
```

Um `ItemMaterial` **tem um** `Material` e uma quantidade. A propriedade mantém uma referência ao material recebido. Esse modo de construir um objeto usando outro é composição de comportamentos e dados; a relação não é de herança. O material pode existir antes e ser compartilhado por diferentes itens.

`Subtotal` calcula o valor quando é consultado; não precisamos manter um campo separado sincronizado. O acesso `item.Material.Nome` percorre a relação: do item para o material e dele para o nome.

## 3. Some os itens

Substitua todo o `Program.cs` por:

```csharp
Material caderno = new Material("Caderno", 12.50m);
Material caneta = new Material("Caneta", 3m);

List<ItemMaterial> itens = new List<ItemMaterial>();
itens.Add(new ItemMaterial(caderno, 2));
itens.Add(new ItemMaterial(caneta, 3));

decimal total = 0m;
foreach (ItemMaterial item in itens)
{
    Console.WriteLine($"{item.Material.Nome}: {item.Quantidade} = {item.Subtotal}");
    total = total + item.Subtotal;
}

Console.WriteLine($"Total: {total}");
```

Os subtotais são 25 e 9; o total é 34. As classes cuidam dos dados e das regras dos objetos; o console apresenta os resultados. Não foi necessário herdar de outra classe para representar essa relação.

Antes de executar, desenhe dois materiais, dois itens e as setas entre eles. Cada item referencia um material; a lista contém referências aos itens.

## 4. Faça uma alteração controlada

Acrescente um terceiro item que recebe a **mesma variável** `caderno`, com quantidade 1. Você cria outro item, mas não outro material: os dois itens referenciam o mesmo caderno. A quantidade de cada item é independente. O novo total deve ser 46,50.

Neste exercício o preço só é definido no construtor e não muda. No projeto futuro será preciso decidir se um item guarda uma cópia do preço ou consulta um preço que pode mudar. Observe como a escolha do modelo afeta o comportamento.

## 5. Pratique

1. Retire os itens, mantendo a lista criada: o total deve ser 0.
2. Crie dois itens do mesmo material com quantidades 1 e 4. Explique quantos objetos de material existem e quantos objetos de item existem nesse caso.
3. Tente criar um item com quantidade 0 dentro de `try/catch`. A criação deve ser recusada antes de adicionar algo à lista.
4. Escreva uma função local `decimal SomarItens(List<ItemMaterial> itens)` no `Program.cs`, depois das instruções de execução. Transfira apenas a soma para ela, sem mensagens. Use o retorno para apresentar o total no console.
5. Explique por que uma lista de itens não é um banco de dados. Fechar e abrir o programa recria os objetos a partir do código, sem persistir alterações da execução anterior.

**Critério para avançar:** criar uma coleção de objetos, explicar a relação “tem um”, calcular o total de uma lista vazia e de uma preenchida, e distinguir o objeto compartilhado dos itens que o usam.

[Anterior: encapsulamento](05-Construtores-e-Encapsulamento.md) · [Próximo: interfaces e transição](07-Interfaces-e-Transicao.md)
