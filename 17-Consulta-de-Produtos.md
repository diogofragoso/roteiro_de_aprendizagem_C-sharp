# Aula 17 — CRUD de Produtos (R — Read): Consulta Parametrizada e Grade (`DataGridView`)

**Duração:** 3h30. **Pré-requisito:** [Aula 16](16-Modelo-de-Produtos.md). **Entrega:** janela `FormProdutos` aberta a partir do `FormAdministrador`, realizando a operação **R (Read)** do CRUD com busca parametrizada (`ILIKE` + `LIMIT 200`) e exibindo resultados ou lista vazia em uma `DataGridView` somente leitura (`A17.01`–`A17.05`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Conferir tabela `produto` e contrato `IProdutoRepository` da Aula 16 |
| Explicação | 30 | `SELECT` com `ILIKE`, curinga `%`, `LIMIT 200`, mapeamento `DataReader` e `DataGridView` |
| Construção | 60 | Construir `ProdutoRepository` (`Ler`, `ListarAsync`, `SalvarAsync`) e `ProdutoService.ListarAsync` |
| Intervalo | 15 | Pausa |
| Prática | 60 | Adicionar `Ui.Texto`/`Ui.Grade`, construir `FormProdutos` e conectar ao menu administrativo |
| Entrega | 30 | Testar busca parcial, código exato, lista vazia, `%`, apóstrofo, rede e acesso negado |

---

## 1. Conceitos do **Read (R)** no CRUD (O Quê e Por Quê)

### 1.1 Busca Textual Segura: `ILIKE`, Curinga `%` e `LIMIT 200` (`A17.01` e `A17.02`)
- **`ILIKE`** (PostgreSQL): busca textual ignorando maiúsculas/minúsculas *(no MySQL/XAMPP, use `LOWER(nome) LIKE LOWER(@busca)`)*.
- **Curinga `%`:** passando `$"%{busca}%"` no parâmetro `@busca`, digitar `"arr"` encontra `"Arroz"` e `"Macarrão"`.
  - Mesmo digitando apóstrofo `'` ou `' OR 1=1`, continua 100% seguro porque vai dentro de `@busca`!
- **`ORDER BY nome LIMIT 200`:** evita travar memória/rede em catálogos gigantes; a tela deve informar claramente esse limite ao operador.

### 1.2 Mapeamento Posicional do `NpgsqlDataReader` (`A17.03`)

| Coluna no `SELECT` | Índice | Método do Reader | Propriedade em `Produto` |
| --- | :---: | --- | --- |
| `id` | `0` | `r.GetInt32(0)` | `Id` (`int`) |
| `codigo` | `1` | `r.GetString(1)` | `Codigo` (`string`) |
| `nome` | `2` | `r.GetString(2)` | `Nome` (`string`) |
| `preco` | `3` | `r.GetDecimal(3)` | `Preco` (`decimal`) |
| `estoque` | `4` | `r.GetInt32(4)` | `Estoque` (`int`) |
| `ativo` | `5` | `r.GetBoolean(5)` | `Ativo` (`bool`) |

*(Atenção: trocar a ordem de duas colunas no `SELECT` sem atualizar `Ler(r)` gera `InvalidCastException`!)*

### 1.3 `DataGridView`, `.ToList()` e Evento `Shown` (`A17.04`)
*(Revise a configuração detalhada da `DataGridView` e do evento `Shown` na [Aula 10 — Seções 2.1 e 2.9](10-Componentes-Windows-Forms.md).)*
- Configuramos `ReadOnly = true`, `AllowUserToAddRows = false`, `AllowUserToDeleteRows = false`, `MultiSelect = false` e `SelectionMode = FullRowSelect`.
- **Por que carregar no evento `Shown += ...`?** Porque construtores C# não podem ser `async`; o evento `Shown` dispara assim que a janela termina de abrir, rodando `await Carregar()` sem travar a interface!

---

## 2. Código Completo e Compilável desta Etapa (6 Arquivos)

### Arquivo 1: `Repositories/IProdutoRepository.cs`

```csharp
using SupermercadoApp.Models;

namespace SupermercadoApp.Repositories;

public interface IProdutoRepository
{
    Task<IReadOnlyList<Produto>> ListarAsync(string busca);
    Task SalvarAsync(int id, string codigo, string nome, decimal preco);
}
```

### Arquivo 2: `Repositories/ProdutoRepository.cs`

```csharp
using Npgsql;
using SupermercadoApp.Models;

namespace SupermercadoApp.Repositories;

public sealed class ProdutoRepository(NpgsqlDataSource banco) : IProdutoRepository
{
    // Método auxiliar reutilizado por todas as consultas de produto:
    private static Produto Ler(NpgsqlDataReader r) => new(
        r.GetInt32(0),
        r.GetString(1),
        r.GetString(2),
        r.GetDecimal(3),
        r.GetInt32(4),
        r.GetBoolean(5));

    public async Task<IReadOnlyList<Produto>> ListarAsync(string busca)
    {
        await using var cmd = banco.CreateCommand("""
            SELECT id, codigo, nome, preco, estoque, ativo
            FROM produto
            WHERE nome ILIKE @busca OR codigo ILIKE @busca
            ORDER BY nome
            LIMIT 200
            """);
        cmd.Parameters.AddWithValue("busca", $"%{busca}%");

        var produtos = new List<Produto>();
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
            produtos.Add(Ler(r));

        return produtos;
    }

    public async Task SalvarAsync(int id, string codigo, string nome, decimal preco)
    {
        await using var cmd = banco.CreateCommand(id == 0
            ? "INSERT INTO produto(codigo, nome, preco) VALUES (@codigo, @nome, @preco)"
            : "UPDATE produto SET codigo=@codigo, nome=@nome, preco=@preco WHERE id=@id AND ativo");

        cmd.Parameters.AddWithValue("codigo", codigo);
        cmd.Parameters.AddWithValue("nome", nome);
        cmd.Parameters.AddWithValue("preco", preco);
        if (id != 0) cmd.Parameters.AddWithValue("id", id);

        if (await cmd.ExecuteNonQueryAsync() != 1)
            throw new InvalidOperationException("Produto inexistente ou inativo. Atualize a lista.");
    }
}
```

### Arquivo 3: `Services/ProdutoService.cs` (Acrescentando `ListarAsync`)

```csharp
public Task<IReadOnlyList<Produto>> ListarAsync(Sessao sessao, string busca)
{
    Permissoes.ExigirAdministrador(sessao);
    return produtos.ListarAsync(busca.Trim());
}
```

### Arquivo 4: Novos Métodos em `Views/Ui.cs` (`Texto` e `Grade`)

```csharp
public static TextBox Texto(FlowLayoutPanel painel, string rotulo, bool senha = false)
{
    painel.Controls.Add(Rotulo(rotulo));
    var campo = new TextBox { Width = 260, UseSystemPasswordChar = senha };
    painel.Controls.Add(campo);
    return campo;
}

public static DataGridView Grade(FlowLayoutPanel painel)
{
    var grade = new DataGridView
    {
        Width = 860,
        Height = 240,
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        MultiSelect = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
    };
    painel.Controls.Add(grade);
    return grade;
}
```

### Arquivo 5: `Views/FormProdutos.cs` (Versão Read / Consulta)

```csharp
using SupermercadoApp.Models;
using SupermercadoApp.Services;

namespace SupermercadoApp.Views;

public sealed class FormProdutos : Form
{
    public FormProdutos(Sessao sessao, ProdutoService produtos)
    {
        Permissoes.ExigirAdministrador(sessao);

        var painel = Ui.Preparar(this, "Administração — Produtos");
        var busca = Ui.Texto(painel, "Buscar por nome ou código (até 200 resultados)");
        var grade = Ui.Grade(painel);

        // Função local assíncrona que consulta o serviço e materializa com .ToList() para a grade:
        async Task Carregar() =>
            grade.DataSource = (await produtos.ListarAsync(sessao, busca.Text)).ToList();

        Ui.Botao(painel, "Buscar", async (_, _) => await Ui.ExecutarAsync(Carregar));

        // Carrega automaticamente assim que a janela terminar de aparecer na tela:
        Shown += async (_, _) => await Ui.ExecutarAsync(Carregar);
    }
}
```

### Arquivo 6: Conectando em `Views/FormAdministrador.cs` e `Program.cs` (`A17.05`)

```csharp
// Em Views/FormAdministrador.cs:
public FormAdministrador(Sessao sessao, ProdutoService produtos)
{
    Permissoes.ExigirAdministrador(sessao);
    var painel = Ui.Preparar(this, "MarketFlow — Administração", 480, 350);
    painel.Controls.Add(Ui.Rotulo($"Administrador: {sessao.Username}"));
    Ui.Botao(painel, "Produtos", (_, _) =>
    {
        using var tela = new FormProdutos(sessao, produtos);
        tela.ShowDialog(this);
    });
    Ui.Botao(painel, "Sair / trocar usuário", (_, _) => Close());
}

// Em Program.cs (antes do laço while):
var produtosRepo = new ProdutoRepository(banco);
var produtoService = new ProdutoService(produtosRepo);
// E no operador ternário dentro do while:
using Form principal = sessao.Perfil == Perfil.Administrador
    ? new FormAdministrador(sessao, produtoService)
    : new FormCaixa(sessao);
```

---

## 3. Verificação e Comparação dos 3 Cenários (`A17.01`–`A17.05`)

> [!NOTE]
> **Massa de teste para a consulta:** primeiro teste a grade vazia (zero erros!); depois insira 2 produtos didáticos pelo SQL Editor para validar a busca hoje (o cadastro pela UI entra na próxima aula, Aula 18):
> ```sql
> INSERT INTO produto(codigo, nome, preco) VALUES ('000123', 'Arroz', 25.90), ('000456', 'Feijão', 8.50);
> ```

Execute com **`F5`** e comprove:
1. **Lista vazia válida:** busque `"zzzzz"` → grade vazia sem exceção.
2. **Consultas especiais:** teste `"arr"`, `"000123"`, busca vazia (ordenada por nome até 200), `"%"` e um apóstrofo `'` (confirmando SQL seguro!).
3. **Falha de rede vs Permissão negada:** compare queda de rede (mensagem amigável) com chamada de `ProdutoService.ListarAsync` usando sessão de `Caixa` (`UnauthorizedAccessException` antes de tocar no banco).

[Anterior: Aula 16](16-Modelo-de-Produtos.md) · [Próxima: Aula 18 — Cadastro, edição e inativação](18-Cadastro-de-Produtos.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 17 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-17). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
