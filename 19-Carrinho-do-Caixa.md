# Aula 19 — Carrinho do Operador de Caixa em Memória

**Duração:** 3h30. **Pré-requisito:** [Aula 18](18-Reposicao-de-Estoque.md), produtos ativos com estoque positivo e conta de `Caixa` preparada. **Entrega:** carrinho em memória (`ItemCarrinho` + `Carrinho`) integrado à tela `FormCaixa`, capaz de buscar produto por código exato, agrupar quantidades, calcular total previsto, remover itens e cancelar a compra — ainda sem gravar vendas no banco.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Entrar como `Caixa` e conferir estoque disponível no banco |
| Explicação | 30 | Encapsulamento de lista (`AsReadOnly`), LINQ `Sum`, `with`, `[]` e `checked` |
| Construção | 60 | Construir `ItemCarrinho`, `Carrinho`, `BuscarCodigoAsync` e `VendaService` inicial |
| Intervalo | 15 | Pausa |
| Prática | 60 | Ampliar `FormCaixa`, configurar cultura `pt-BR` e escrever asserção executável |
| Entrega | 30 | Demonstrar agrupamento, remoção, cancelamento e ausência de alteração no banco |

---

## 1. Conceitos Novos de C# Que Você Vai Dominar Nesta Aula (O Quê e Por Quê)

### 1.1 Expressão de Coleção (`[]`) e Lista Privada (`AsReadOnly()`) (`A19.02` e `A19.03`)
- `private readonly List<ItemCarrinho> itens = [];`
  Os colchetes vazios **`[]`** (C# 12+) são um atalho moderno equivalente a `new List<ItemCarrinho>()`.
- `public IReadOnlyList<ItemCarrinho> Itens => itens.AsReadOnly();`
  **Por que a lista interna é `private` e exposta via `.AsReadOnly()`?**
  Para garantir que **ninguém** consiga fazer `carrinho.Itens.Add(...)` burlando as regras de estoque ou duplicando linhas do mesmo produto! Toda inclusão ou remoção precisa obrigatoriamente passar por `carrinho.Adicionar(...)` e `carrinho.Remover(...)`.

### 1.2 Atualização Imutável de `record` com a Palavra-Chave `with` (`A19.03`)
Como `ItemCarrinho` é um `record` (cujas propriedades são somente leitura), não escrevemos `itens[indice].Quantidade = acumulada;`. Em vez disso, usamos a expressão **`with`**:

```csharp
// Cria uma cópia do record mantendo Id, Nome e Preco, mudando apenas Quantidade:
itens[indice] = itens[indice] with { Quantidade = acumulada };
```
**Por que isso é ótimo?** O record anterior permanece imutável, preservando o preço original exibido ao cliente enquanto atualiza apenas a quantidade acumulada!

### 1.3 Proteção Contra Estouro de Inteiro (`checked`) (`A19.04`)
Um `int` em C# aguenta até `2.147.483.647` (`int.MaxValue`). Por padrão, se você somar `int.MaxValue + 1`, o número "dá a volta" silenciosamente e vira **`-2.147.483.648`** (negativo)!
- Envolvendo a soma em **`checked(quantidade + anterior)`**, o C# detecta o estouro imediatamente e lança **`OverflowException`**, impedindo que uma quantidade gigantesca vire negativa!

### 1.4 LINQ Expressivo (`Sum`, `FindIndex`, `RemoveAll`)
- `itens.Sum(i => i.Subtotal)`: percorre todos os itens da lista e soma o `Subtotal` de cada um (retorna `0m` se a lista estiver vazia).
- `itens.FindIndex(i => i.ProdutoId == produto.Id)`: procura a posição (`0`, `1`, ...) do item já existente no carrinho ou devolve `-1` se ainda não estiver lá.
- `itens.RemoveAll(i => i.ProdutoId == produtoId)`: remove o item cujo `ProdutoId` coincide.

### 1.5 Regra Arquitetural Crucial: Carrinho NÃO Reserva Estoque! (`A19.06`)
Validar `acumulada <= produto.Estoque` ao adicionar ao carrinho melhora a experiência do operador, mas **não reserva o produto no banco de dados**: outro caixa pode finalizar a compra daquela última unidade antes. A proteção definitiva de estoque acontecerá na transação atômica da **Aula 20**.

---

## 2. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

### Passo 1 — Construir `Models/ItemCarrinho.cs` e `Services/Carrinho.cs`
1. **`Models/ItemCarrinho.cs`** (`namespace SupermercadoApp.Models;`):
   - Declare `public sealed record ItemCarrinho(int ProdutoId, string Nome, decimal Preco, int Quantidade)` com uma propriedade calculada no corpo `{ ... }`:
     `public decimal Subtotal => Preco * Quantidade;`
2. **`Services/Carrinho.cs`** (`namespace SupermercadoApp.Services;`):
   - Declare `public sealed class Carrinho` (100% em memória — **sem** SQL, banco ou `MessageBox`!).
   - Campo privado: `private readonly List<ItemCarrinho> itens = [];`.
   - Propriedades públicas calculadas:
     - `public IReadOnlyList<ItemCarrinho> Itens => itens.AsReadOnly();`
     - `public decimal Total => itens.Sum(i => i.Subtotal);`
   - Método **`public void Adicionar(Produto produto, int quantidade)`:**
     1. Se `!produto.Ativo || quantidade <= 0` → lance `ArgumentException`.
     2. Localize se o produto já existe no carrinho com `int indice = itens.FindIndex(i => i.ProdutoId == produto.Id);`.
     3. Calcule a quantidade acumulada protegida contra overflow: `int acumulada = checked(quantidade + (indice >= 0 ? itens[indice].Quantidade : 0));`.
     4. Se `acumulada > produto.Estoque` → lance `InvalidOperationException("Estoque insuficiente para esta quantidade.")`.
     5. Se `indice >= 0`, substitua `itens[indice] = itens[indice] with { Quantidade = acumulada };`; senão, faça `itens.Add(new ItemCarrinho(produto.Id, produto.Nome, produto.Preco, quantidade));`.
   - Métodos **`public void Remover(int produtoId)`** (usando `itens.RemoveAll(...)`) e **`public void Limpar()`** (usando `itens.Clear()`).

### Passo 2 — Adicionar `BuscarCodigoAsync` e Criar o `VendaService` Inicial
1. Em **`Repositories/IProdutoRepository.cs`**, acrescente `Task<Produto?> BuscarCodigoAsync(string codigo);`.
2. Em **`Repositories/ProdutoRepository.cs`**, implemente `BuscarCodigoAsync(string codigo)`:
   - Faça `SELECT id, codigo, nome, preco, estoque, ativo FROM produto WHERE codigo = @codigo` (note: igualdade exata `=`, **não** `ILIKE`, pois no caixa o leitor/operador passa o código exato!).
   - Reaproveite seu método `Ler(r)`: retorne `await r.ReadAsync() ? Ler(r) : null;`.
3. Crie **`Services/VendaService.cs`** (`namespace SupermercadoApp.Services;`) recebendo **por enquanto apenas** `IProdutoRepository produtos` no construtor primário (`IVendaRepository` entra na Aula 20!):
   - Implemente `public Task<Produto?> BuscarProdutoAsync(Sessao sessao, string codigo)`:
     - Exija `Permissoes.ExigirCaixa(sessao);` e retorne `produtos.BuscarCodigoAsync(codigo.Trim());`.

### Passo 3 — Ampliar `Views/FormCaixa.cs` e Configurar Cultura em `Program.cs`
*(Deixe os campos de pagamento, valor recebido e botão Finalizar para a Aula 21!)*

1. Em **`Program.cs`**:
   - Configure no início de `Main`:
     `CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pt-BR");`
     `CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("pt-BR");`
     *(Assim todas as threads assíncronas e telas exibem `R$` de forma uniforme!)*
   - Instancie `var vendaService = new VendaService(produtos);` e passe-o para `new FormCaixa(sessao, vendaService)`.
2. Em **`Views/FormCaixa.cs`**:
   - Declare o campo `private readonly Carrinho carrinho = new();`.
   - Altere o construtor para `public FormCaixa(Sessao sessao, VendaService vendas)` mantendo `Permissoes.ExigirCaixa(sessao);`.
   - Monte os controles no painel: `Ui.Preparar`, `codigo` via `Ui.Texto`, rótulo `"Quantidade"` + `NumericUpDown quantidade` (`Minimum = 1`, `Maximum = 1000000`, `Value = 1`, adicionando ambos ao painel com `painel.Controls.Add(...)`), `grade` via `Ui.Grade` e `var total = Ui.Rotulo("Total previsto: R$ 0,00"); painel.Controls.Add(total);`.
   - Crie a função local `void Atualizar()`: atualiza `grade.DataSource = carrinho.Itens.ToList();` e `total.Text = $"Total previsto: {carrinho.Total:C}";`.
   - **Botão "Adicionar":** desabilite durante a consulta; dentro de `Ui.ExecutarAsync`, busque `var produto = await vendas.BuscarProdutoAsync(sessao, codigo.Text) ?? throw new InvalidOperationException("Produto não encontrado.");`, chame `carrinho.Adicionar(produto, (int)quantidade.Value);`, chame `Atualizar()`, limpe `codigo` e devolva o foco com `codigo.Focus();`. Reabilite no `finally`.
   - **Botão "Remover selecionado":** verifique `if (grade.SelectedRows.Count > 0 && grade.SelectedRows[0].DataBoundItem is ItemCarrinho item)` (mesmo padrão seguro da Aula 17!), chame `carrinho.Remover(item.ProdutoId)` e `Atualizar()`.
   - **Botão "Cancelar carrinho":** chame `carrinho.Limpar()` e `Atualizar()` (limpa apenas a memória RAM!).
   - Mantenha o botão `"Sair / trocar usuário"`.

---

## 3. Verificação, Asserção Executável e Experimento `checked` (`A19.01`–`A19.06`)

1. **Conta no papel + Verificação na tela:** prepare no banco Arroz (`R$ 25,90`, estoque `10`) e Feijão (`R$ 8,50`, estoque `10`). Como `Caixa`, adicione Arroz 1 unidade, depois Arroz mais 1 unidade (deve agrupar em 1 linha com quantidade `2`!) e Feijão 1 unidade → **2 linhas, Total previsto `R$ 60,30`**. Remova Feijão (`R$ 51,80`), depois cancele (`R$ 0,00`) e confira no SQL Editor que o estoque no banco **não mudou**!
2. **Asserção executável (`A19.01`, reaproveitada na Aula 24):** escreva um pequeno teste em memória instanciando `Carrinho`, adicionando os itens acima e verificando `if (carrinho.Itens.Count != 2 || carrinho.Total != 60.30m) throw new Exception(...)`. Quebre temporariamente uma quantidade para provar que a asserção detecta o erro e depois restaure.
3. **Experimento isolado de `checked` (`A19.04`):** em um teste em memória sem alterar o banco, some `int.MaxValue` e `1` dentro de `checked(...)` com `try / catch (OverflowException)` comprovando que o excesso é interceptado.

[Anterior: Aula 18](18-Reposicao-de-Estoque.md) · [Próxima: Aula 20 — Venda com transação e concorrência](20-Venda-com-Transacao.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 19 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-19). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
