# Aula 20 — Carrinho do Operador de Caixa em Memória (`with`, `AsReadOnly`, `checked` e LINQ)

**Duração:** 3h30. **Pré-requisito:** [Aula 19](19-Reposicao-de-Estoque.md), produtos ativos com estoque positivo e conta de `Caixa` preparada. **Entrega:** carrinho em memória (`ItemCarrinho` + `Carrinho`) integrado à tela `FormCaixa`, buscando produtos por código exato, agrupando quantidades, calculando total previsto, removendo itens e cancelando sem alterar o banco (`A20.01`–`A20.06`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Entrar como `Caixa` e conferir estoque disponível no banco |
| Explicação | 30 | `[]`, `AsReadOnly()`, `record` + `with`, `checked` (`OverflowException`) e LINQ |
| Construção | 60 | Construir `ItemCarrinho`, `Carrinho`, `BuscarCodigoAsync` e `VendaService` inicial |
| Intervalo | 15 | Pausa |
| Prática | 60 | Ampliar `FormCaixa`, configurar cultura `pt-BR` e executar asserção + teste `checked` |
| Entrega | 30 | Demonstrar agrupamento (`R$ 60,30`), remoção, cancelamento e estoque intacto no banco |

---

## 1. Recursos Modernos de C# Aplicados ao Carrinho (`A20.02`–`A20.04`)

1. **Coleção privada + `AsReadOnly()`:** `private readonly List<ItemCarrinho> itens = [];` *(atalho C# 12+ equivalente a `new List<ItemCarrinho>()`)*. Expor `public IReadOnlyList<ItemCarrinho> Itens => itens.AsReadOnly();` garante que **toda** inclusão/remoção passe pelos métodos validados de `Carrinho`.
2. **Expressão `with` em `record`:** como `ItemCarrinho` é imutável, atualizamos a quantidade gerando uma nova cópia que preserva `ProdutoId`, `Nome` e `Preco`:
   ```csharp
   itens[indice] = itens[indice] with { Quantidade = acumulada };
   ```
3. **Proteção contra estouro de inteiro (`checked`) (`A20.04`):** sem `checked`, `int.MaxValue + 1` vira **`-2.147.483.648`** silenciosamente! Teste este experimento isolado:
   ```csharp
   try
   {
       int estouro = checked(int.MaxValue + 1);
   }
   catch (OverflowException)
   {
       Console.WriteLine("Estouro de inteiro interceptado com segurança pelo checked!");
   }
   ```
4. **Regra arquitetural (`A20.06`):** o carrinho vive 100% na memória RAM e **não reserva estoque no banco**. A garantia definitiva de estoque acontece na transação com `FOR UPDATE` da **Aula 21**.

---

## 2. Código Completo e Compilável desta Etapa (6 Arquivos)

### Arquivo 1: `Models/ItemCarrinho.cs`

```csharp
namespace SupermercadoApp.Models;

public sealed record ItemCarrinho(int ProdutoId, string Nome, decimal Preco, int Quantidade)
{
    public decimal Subtotal => Preco * Quantidade;
}
```

### Arquivo 2: `Services/Carrinho.cs` (100% em Memória — ZERO SQL e ZERO UI!)

```csharp
using SupermercadoApp.Models;

namespace SupermercadoApp.Services;

public sealed class Carrinho
{
    private readonly List<ItemCarrinho> itens = [];
    public IReadOnlyList<ItemCarrinho> Itens => itens.AsReadOnly();
    public decimal Total => itens.Sum(i => i.Subtotal);

    public void Adicionar(Produto produto, int quantidade)
    {
        if (!produto.Ativo || quantidade <= 0)
            throw new ArgumentException("Produto deve estar ativo e quantidade deve ser positiva.");

        int indice = itens.FindIndex(i => i.ProdutoId == produto.Id);
        int acumulada = checked(quantidade + (indice >= 0 ? itens[indice].Quantidade : 0));

        if (acumulada > produto.Estoque)
            throw new InvalidOperationException("Estoque insuficiente para esta quantidade.");

        // Se já está no carrinho, agrupa somando a quantidade via 'with'; senão adiciona nova linha:
        if (indice >= 0)
            itens[indice] = itens[indice] with { Quantidade = acumulada };
        else
            itens.Add(new ItemCarrinho(produto.Id, produto.Nome, produto.Preco, quantidade));
    }

    public void Remover(int produtoId) => itens.RemoveAll(i => i.ProdutoId == produtoId);
    public void Limpar() => itens.Clear();
}
```

### Arquivo 3 e 4: Busca por Código Exato em `IProdutoRepository` e `ProdutoRepository`

```csharp
// Em Repositories/IProdutoRepository.cs:
Task<Produto?> BuscarCodigoAsync(string codigo);

// Em Repositories/ProdutoRepository.cs (reaproveita Ler(r) e usa '=' exato, NÃO ILIKE!):
public async Task<Produto?> BuscarCodigoAsync(string codigo)
{
    await using var cmd = banco.CreateCommand("""
        SELECT id, codigo, nome, preco, estoque, ativo
        FROM produto
        WHERE codigo = @codigo
        """);
    cmd.Parameters.AddWithValue("codigo", codigo);

    await using var r = await cmd.ExecuteReaderAsync();
    return await r.ReadAsync() ? Ler(r) : null;
}
```

### Arquivo 5: `Services/VendaService.cs` (Versão Inicial — `IVendaRepository` entra na Aula 21)

```csharp
using SupermercadoApp.Models;
using SupermercadoApp.Repositories;

namespace SupermercadoApp.Services;

public sealed class VendaService(IProdutoRepository produtos)
{
    public Task<Produto?> BuscarProdutoAsync(Sessao sessao, string codigo)
    {
        Permissoes.ExigirCaixa(sessao);
        return produtos.BuscarCodigoAsync(codigo.Trim());
    }
}
```

### Arquivo 6: `Views/FormCaixa.cs` (Versão de Carrinho) + Cultura em `Program.cs`

Em **`Program.cs`**, adicione no início de `Main` (`using System.Globalization;`) para todas as threads assíncronas formatarem `R$` uniformemente:
```csharp
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("pt-BR");
// E crie: var vendaService = new VendaService(produtosRepo);
// Passando para: new FormCaixa(sessao, vendaService)
```

Em **`Views/FormCaixa.cs`**:

```csharp
using SupermercadoApp.Models;
using SupermercadoApp.Services;

namespace SupermercadoApp.Views;

public sealed class FormCaixa : Form
{
    private readonly Carrinho carrinho = new();

    public FormCaixa(Sessao sessao, VendaService vendas)
    {
        Permissoes.ExigirCaixa(sessao);

        var p = Ui.Preparar(this, $"MarketFlow — Caixa: {sessao.Username}", 960, 760);
        var codigo = Ui.Texto(p, "Código do produto");

        p.Controls.Add(Ui.Rotulo("Quantidade (unidades inteiras)"));
        var quantidade = new NumericUpDown { Minimum = 1, Maximum = 1000000, Value = 1, Width = 140 };
        p.Controls.Add(quantidade);

        var grade = Ui.Grade(p);
        var total = Ui.Rotulo("Total previsto: R$ 0,00");
        p.Controls.Add(total);

        // Reaplicando DataGridView + Label dinâmico da Aula 10:
        void Atualizar()
        {
            grade.DataSource = carrinho.Itens.ToList();
            total.Text = $"Total previsto: {carrinho.Total:C}";
        }

        Ui.Botao(p, "Adicionar", async (sender, _) =>
        {
            var b = (Button)sender!;
            b.Enabled = false;
            try
            {
                await Ui.ExecutarAsync(async () =>
                {
                    var produto = await vendas.BuscarProdutoAsync(sessao, codigo.Text)
                        ?? throw new InvalidOperationException("Produto não encontrado.");
                    carrinho.Adicionar(produto, (int)quantidade.Value);
                    Atualizar();
                    codigo.Clear();
                    codigo.Focus(); // Devolve cursor para bipar o próximo produto!
                });
            }
            finally { b.Enabled = true; }
        });

        Ui.Botao(p, "Remover selecionado", (_, _) =>
        {
            if (grade.SelectedRows.Count > 0 && grade.SelectedRows[0].DataBoundItem is ItemCarrinho item)
                carrinho.Remover(item.ProdutoId);
            Atualizar();
        });

        Ui.Botao(p, "Cancelar carrinho", (_, _) =>
        {
            carrinho.Limpar();
            Atualizar();
        });

        Ui.Botao(p, "Sair / trocar usuário", (_, _) => Close());
    }
}
```

---

## 3. Verificação e Asserção Executável (`A20.01`–`A20.06`)

1. **Na tela (`F5` como Caixa):** com Arroz (`R$ 25,90`, estoque `10`) e Feijão (`R$ 8,50`, estoque `10`), adicione Arroz `1`, depois Arroz `1` (agrupa para `2`!), depois Feijão `1` → **2 linhas, Total `R$ 60,30`**. Remova Feijão (`R$ 51,80`), cancele (`R$ 0,00`) e comprove no SQL Editor que o estoque no banco **não mudou**.
2. **Testes de recusa (`A20.05`):** código parcial, inexistente, produto inativo e quantidade acumulada acima do estoque preservam o carrinho anterior.
3. **Asserção executável (`A20.01`, reaproveitada na Aula 25):**
   ```csharp
   var c = new Carrinho();
   var arroz = new Produto(1, "000123", "Arroz", 25.90m, 10, true);
   var feijao = new Produto(2, "000456", "Feijão", 8.50m, 10, true);
   c.Adicionar(arroz, 1);
   c.Adicionar(arroz, 1);
   c.Adicionar(feijao, 1);
   if (c.Itens.Count != 2 || c.Itens[0].Quantidade != 2 || c.Total != 60.30m)
       throw new Exception("Falha no agrupamento ou cálculo do carrinho!");
   ```

[Anterior: Aula 19](19-Reposicao-de-Estoque.md) · [Próxima: Aula 21 — Venda com transação e concorrência](21-Venda-com-Transacao.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 20 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-20). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
