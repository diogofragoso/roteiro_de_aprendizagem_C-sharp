# F06 — Composição e Coleções (`List<T>`): Objetos Que Trabalham Juntos

**Pré-requisito:** [F05](05-Construtores-e-Encapsulamento.md).
**Objetivo:** aprender a trabalhar com listas dinâmicas (`List<T>`), modelar relações de **composição ("tem um")** entre objetos, criar propriedades calculadas e percorrer coleções com `foreach`.

Crie no **Visual Studio 2022** um novo **Aplicativo de Console** chamado `FundamentosF06`.

---

## 1. Por Que Usar `List<T>` em Vez de Variáveis Soltas ou Arrays Fixos?

Quando um cliente vai ao supermercado, não sabemos se ele vai comprar 1 item, 5 itens ou 80 itens. Um array comum (`string[]`) tem tamanho fixo na criação. Já uma **`List<T>`** cresce e diminui dinamicamente conforme adicionamos ou removemos elementos!

### Anatomia de `List<T>` e `foreach` (Microexemplo)

```csharp
List<string> tarefas = new List<string>();
tarefas.Add("Estudar C#");
tarefas.Add("Praticar no VS 2022");

foreach (string tarefa in tarefas)
{
    Console.WriteLine($"- {tarefa}");
}

Console.WriteLine($"Total de tarefas: {tarefas.Count}");
```

Dissecando cada parte:
- `List<string>`: o sinal `< >` indica um **tipo genérico (*Generic*)**. Significa *"uma lista onde cada elemento é obrigatoriamente do tipo `string`"*. Se você tentar fazer `tarefas.Add(123);`, o compilador do Visual Studio 2022 recusa na hora!
- `new List<string>()`: cria a lista vazia na memória (com `Count == 0`).
- `.Add(...)`: acrescenta um novo elemento no final da lista.
- `.Count`: propriedade que informa quantos elementos existem atualmente na lista.
- `foreach (string tarefa in tarefas)`: leia em português — *"para cada (`foreach`) `tarefa` do tipo `string` dentro de (`in`) `tarefas`, execute o bloco abaixo"*.
  - **Por que preferir `foreach` ao `for` para listas?** Você não precisa controlar índice `i`, nem corre risco de acessar uma posição que não existe (`IndexOutOfRangeException`). Se a lista estiver vazia (`Count == 0`), o `foreach` simplesmente não executa nenhuma vez, sem erro.

---

## 2. O Que É Composição ("Tem Um") e Por Que Ela É Melhor Que Misturar Tudo?

No mundo real, objetos colaboram entre si. Pense em uma receita culinária e seus ingredientes, ou em um **Item de Pedido** que referencia um **Material**:
- Um `ItemMaterial` **não é** um `Material` (por isso **não** usamos herança aqui!).
- Um `ItemMaterial` **tem um** `Material` e uma `Quantidade`.

Essa técnica de uma classe guardar uma propriedade que aponta para outro objeto se chama **Composição**.

### Anatomia da Composição e da Propriedade Calculada (Microexemplo)

Observe como um `ItemTreino` compõe um `Exercicio`:

```csharp
public class Exercicio
{
    public string Nome { get; }
    public int CaloriasPorMinuto { get; }

    public Exercicio(string nome, int caloriasPorMinuto)
    {
        if (string.IsNullOrWhiteSpace(nome) || caloriasPorMinuto <= 0)
            throw new ArgumentException("Informe nome e calorias positivas.");

        Nome = nome.Trim();
        CaloriasPorMinuto = caloriasPorMinuto;
    }
}

public class ItemTreino
{
    public Exercicio Exercicio { get; }
    public int Minutos { get; }

    // Propriedade calculada: não guarda campo próprio; calcula na hora em que é lida!
    public int TotalCalorias
    {
        get { return Exercicio.CaloriasPorMinuto * Minutos; }
    }

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

**Por que `TotalCalorias` é uma propriedade calculada (`get { return ...; }`) e não um valor digitado no construtor?**
Porque se alguém pudesse passar `TotalCalorias` manualmente, poderia informar `10 minutos` de um exercício de `5 cal/min` e digitar `TotalCalorias = 999` — gerando dados contraditórios! Calculando sob demanda, o valor é **sempre matematicamente coerente**.

**Por que testamos `if (exercicio is null)` no construtor?**
Porque `Exercicio` é um tipo de referência (classe). Se quem chamar o construtor passar `null`, sem essa checagem o objeto nasceria quebrado e só explodiria depois ao tentar ler `Exercicio.CaloriasPorMinuto` (`NullReferenceException`).

**Navegando pelas referências com ponto `.`:**
Quando você tem um objeto `item` do tipo `ItemTreino`, escrever `item.Exercicio.Nome` percorre duas setas na memória: do `item` → para o objeto `Exercicio` → até a propriedade `Nome`.

---

## 3. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

Construa uma lista de materiais escolares em `FundamentosF06` seguindo rigorosamente as orientações abaixo:

### Passo 1 — Aquecimento com `List<string>`
1. No `Program.cs`, crie uma `List<string>` de nomes de materiais, adicione `"Caderno"` e `"Caneta"`, percorra com `foreach` e exiba `Count`.
2. Comente temporariamente as duas linhas `.Add(...)` e execute com `Ctrl+F5` para confirmar que percorrer uma lista vazia exibe `Count: 0` sem nenhum erro.

### Passo 2 — Criar `Material.cs` (`Shift+Alt+C` no VS 2022)
1. Declare `public class Material` com duas propriedades somente leitura (`{ get; }`):
   - `string Nome`
   - `decimal Preco`
2. Crie o construtor `public Material(string nome, decimal preco)`:
   - Valide que `nome` não é nulo/vazio/espaços (`string.IsNullOrWhiteSpace`) e que `preco > 0`; se violar, lance `ArgumentException`.
   - Atribua `Nome = nome.Trim();` e `Preco = preco;`.

### Passo 3 — Criar `ItemMaterial.cs` (`Shift+Alt+C` no VS 2022)
1. Declare `public class ItemMaterial` com:
   - Propriedade somente leitura `Material Material { get; }` (composição!);
   - Propriedade somente leitura `int Quantidade { get; }`;
   - Propriedade calculada `decimal Subtotal` (apenas `get`) que retorna `Material.Preco * Quantidade`.
2. Crie o construtor `public ItemMaterial(Material material, int quantidade)`:
   - Recuse com `ArgumentException` se `material is null` ou se `quantidade <= 0`.
   - Guarde ambos nas propriedades.

### Passo 4 — Montar e Totalizar a Lista no `Program.cs`
Substitua o `Program.cs` seguindo estas orientações:
1. Instancie dois objetos `Material`: `"Caderno"` por `12.50m` e `"Caneta"` por `3m`.
2. Crie uma `List<ItemMaterial> itens = new List<ItemMaterial>();`.
3. Adicione à lista um item com `2` cadernos e outro com `3` canetas.
4. Declare uma função local `decimal SomarItens(List<ItemMaterial> lista)` no final do `Program.cs` (lembrando de `F03`, ela apenas calcula e retorna o `decimal`, sem imprimir na tela!).
5. No corpo principal, use um `foreach` para exibir cada linha no formato `"{item.Material.Nome}: {item.Quantidade} = {item.Subtotal}"`, chame `SomarItens(itens)` e exiba o total (esperado: subtotais `25` e `9`; total `34`).
6. **Experimento de referência compartilhada:** adicione um **terceiro** `ItemMaterial` usando a **mesma variável** `caderno`, com quantidade `1`.
   - Antes de rodar, desenhe no papel quantos objetos `Material` existem na memória (2) e quantos objetos `ItemMaterial` existem (3).
   - Rode com `Ctrl+F5` e confirme o novo total (`46,50`).
7. **Testes de fronteira:**
   - Teste chamar `SomarItens` com uma lista vazia (esperado: `0`).
   - Dentro de `try/catch`, tente criar um `ItemMaterial` com quantidade `0` e confirme que a exceção impede que o item inválido seja adicionado à lista.
8. **Reflexão final (no caderno):** explique por que uma `List<ItemMaterial>` na memória RAM **não** substitui um banco de dados (o que acontece com os itens quando você fecha a janela preta do console e executa de novo?).

---

## 4. Erros Comuns no Visual Studio 2022

| Erro na Lista de Erros | Causa provável | Como resolver |
| --- | --- | --- |
| `CS1503: Não é possível converter de 'Material' para 'ItemMaterial'` | Tentou fazer `itens.Add(caderno)` diretamente numa `List<ItemMaterial>`. | A lista guarda `ItemMaterial`: use `itens.Add(new ItemMaterial(caderno, 2));`. |
| `NullReferenceException` em tempo de execução | Tentou acessar `.Preco` de uma referência nula porque esqueceu de validar `if (material is null)`. | Valide sempre no construtor antes de guardar a referência. |

**Critério para avançar para F07:** criar uma coleção `List<T>` de objetos compostos, calcular o total para lista vazia e preenchida em função separada e explicar no desenho a relação "tem um" e o compartilhamento de um mesmo objeto `Material` por dois itens.

[Anterior: F05 — Construtores e encapsulamento](05-Construtores-e-Encapsulamento.md) · [Próximo: F07 — Interfaces, polimorfismo e transição](07-Interfaces-e-Transicao.md)
