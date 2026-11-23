# Aula 16 — Consulta Parametrizada de Produtos e Grade (`DataGridView`)

**Duração:** 3h30. **Pré-requisito:** [Aula 15](15-Modelo-de-Produtos.md). **Entrega:** janela `FormProdutos` abrindo a partir do menu administrativo, buscando produtos reais no banco por nome ou código (com limite explícito de 200 linhas) e exibindo resultados ou lista vazia em uma grade somente leitura.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Conferir tabela `produto` e contrato `IProdutoRepository` da Aula 15 |
| Explicação | 30 | `SELECT` com `ILIKE`, curinga `%`, `LIMIT 200`, mapeamento `DataReader` e `DataGridView` |
| Construção | 60 | Construir `ProdutoRepository` (leitura e escrita do contrato) e `ProdutoService.ListarAsync` |
| Intervalo | 15 | Pausa |
| Prática | 60 | Construir `Ui.Texto`, `Ui.Grade`, `FormProdutos` mínimo e conectar ao `FormAdministrador` |
| Entrega | 30 | Testar busca parcial, código exato, lista vazia, apóstrofo, rede e acesso negado |

---

## 1. Conceitos Novos Desta Aula (O Quê e Por Quê)

### 1.1 Busca Textual Segura: `ILIKE`, Curinga `%` e `LIMIT 200` (`A16.01` e `A16.02`)
Para pesquisar produtos digitando apenas um pedaço do nome (como `"arr"` para achar `"Arroz"`):

- **`ILIKE`** (PostgreSQL): compara textos ignorando maiúsculas/minúsculas (*Case-Insensitive LIKE*). *(Na trilha MySQL/XAMPP, use `LOWER(nome) LIKE LOWER(@busca)`.)*
- **Curinga `%`:** significa *"zero ou mais caracteres quaisquer"*. Passando o valor `$"%{busca}%"` no parâmetro `@busca`, `"arr"` encontra `"Arroz"` e `"Macarrão"`.
  - **Segurança:** se o usuário digitar um apóstrofo `'` ou `' OR 1=1`, continua 100% seguro porque usamos `@busca` como parâmetro! Se o usuário digitar `%`, o SQL apenas procura qualquer texto (curinga de busca), sem jamais executar comandos invasores.
- **Por que `ORDER BY nome LIMIT 200`?** Em um supermercado real com 50.000 produtos, carregar tudo de uma vez travaria a rede e a memória RAM do caixa. `LIMIT 200` protege a consulta e **a tela deve avisar claramente** o usuário sobre esse limite (sistemas maiores usam paginação).

### 1.2 Mapeamento Posicional do `NpgsqlDataReader` (`A16.03`)
Ao executar `ExecuteReaderAsync()`, o banco devolve um cursor linha por linha (`while (await r.ReadAsync())`). Cada coluna do `SELECT` recebe uma posição começando em **zero**:

| Coluna no `SELECT` | Posição no Reader | Método de Leitura | Propriedade em `Produto` |
| --- | --- | --- | --- |
| `id` | Posição 0 | `r.GetInt32(0)` | `Id` (`int`) |
| `codigo` | Posição 1 | `r.GetString(1)` | `Codigo` (`string`) |
| `nome` | Posição 2 | `r.GetString(2)` | `Nome` (`string`) |
| `preco` | Posição 3 | `r.GetDecimal(3)` | `Preco` (`decimal`) |
| `estoque` | Posição 4 | `r.GetInt32(4)` | `Estoque` (`int`) |
| `ativo` | Posição 5 | `r.GetBoolean(5)` | `Ativo` (`bool`) |

**Por que extrair um método auxiliar `private static Produto Ler(NpgsqlDataReader r)`?**
Para não repetir esses 6 `Get...(...)` em toda consulta (`ListarAsync`, `BuscarCodigoAsync`). **Atenção:** se você trocar a ordem de duas colunas no `SELECT` sem ajustar o `Ler`, o programa tenta ler um texto como inteiro e lança `InvalidCastException`!

### 1.3 Vínculo de Dados na `DataGridView`, `.ToList()` e Evento `Shown` (`A16.04`)
- **`DataGridView`:** componente de tabela do Windows Forms. Configuramos `ReadOnly = true`, `AllowUserToAddRows = false`, `AllowUserToDeleteRows = false`, `MultiSelect = false` e `SelectionMode = DataGridViewSelectionMode.FullRowSelect`. **Por quê?** A grade serve apenas para visualizar e selecionar; toda edição/inclusão acontecerá em campos controlados (Aula 17).
- **`grade.DataSource = (...).ToList();`:** o `DataGridView` inspeciona automaticamente as propriedades públicas do record `Produto` e cria as colunas! Materializamos com `.ToList()` para entregar uma nova lista concreta a cada atualização.
- **Por que carregar os dados no evento `Shown += ...` e não no construtor da tela?**
  Construtores em C# **não podem ser `async`**. O evento **`Shown`** dispara automaticamente assim que a janela termina de aparecer na tela, permitindo rodar `await Ui.ExecutarAsync(Carregar)` sem travar a abertura da janela!

---

## 2. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

### Passo 1 — Evoluir o Contrato `Repositories/IProdutoRepository.cs`
Adicione apenas a nova assinatura de consulta ao contrato da Aula 15 (mantendo `SalvarAsync`):
- `Task<IReadOnlyList<Produto>> ListarAsync(string busca);`

### Passo 2 — Construir `Repositories/ProdutoRepository.cs`
Crie **`Repositories/ProdutoRepository.cs`** em `namespace SupermercadoApp.Repositories;` (`using Npgsql;` e `SupermercadoApp.Models;`):
1. Declare `public sealed class ProdutoRepository(NpgsqlDataSource banco) : IProdutoRepository`.
2. Crie o método auxiliar `private static Produto Ler(NpgsqlDataReader r)` instanciando `new Produto(...)` a partir das posições 0 a 5 conforme a tabela da Seção 1.2.
3. Implemente **`ListarAsync(string busca)`:**
   - Crie o comando com `await using var cmd = banco.CreateCommand(""" ... """);` contendo:
     `SELECT id, codigo, nome, preco, estoque, ativo FROM produto WHERE nome ILIKE @busca OR codigo ILIKE @busca ORDER BY nome LIMIT 200`.
   - Adicione o parâmetro `cmd.Parameters.AddWithValue("busca", $"%{busca}%");`.
   - Crie `var produtos = new List<Produto>();`, abra `await using var r = await cmd.ExecuteReaderAsync();`, percorra com `while (await r.ReadAsync()) produtos.Add(Ler(r));` e retorne `produtos`.
4. Implemente **`SalvarAsync(int id, string codigo, string nome, decimal preco)`** (para cumprir o contrato já existente — usaremos a tela de escrita na Aula 17):
   - Se `id == 0`, use `INSERT INTO produto(codigo, nome, preco) VALUES (@codigo, @nome, @preco)`.
   - Caso contrário, use `UPDATE produto SET codigo=@codigo, nome=@nome, preco=@preco WHERE id=@id AND ativo` e adicione o parâmetro `"id"`.
   - Adicione os parâmetros `"codigo"`, `"nome"`, `"preco"`.
   - Execute `if (await cmd.ExecuteNonQueryAsync() != 1)` → lance `InvalidOperationException("Produto inexistente ou inativo. Atualize a lista.")`.

### Passo 3 — Adicionar `ListarAsync` em `Services/ProdutoService.cs`
Adicione o método **`public Task<IReadOnlyList<Produto>> ListarAsync(Sessao sessao, string busca)`**:
1. Exija permissão com `Permissoes.ExigirAdministrador(sessao);`.
2. Retorne `produtos.ListarAsync(busca.Trim());`. *(Nenhuma linha de SQL aqui!)*

### Passo 4 — Adicionar `Ui.Texto` e `Ui.Grade` em `Views/Ui.cs`
1. **`Texto(FlowLayoutPanel painel, string rotulo, bool senha = false)` → retorna `TextBox`:**
   - Adicione `Rotulo(rotulo)` ao painel, crie um `TextBox` com `Width = 260` e `UseSystemPasswordChar = senha`, adicione ao painel e retorne-o.
2. **`Grade(FlowLayoutPanel painel)` → retorna `DataGridView`:**
   - Crie um `DataGridView` com `Width = 860`, `Height = 240`, `ReadOnly = true`, `AllowUserToAddRows = false`, `AllowUserToDeleteRows = false`, `MultiSelect = false`, `SelectionMode = DataGridViewSelectionMode.FullRowSelect` e `AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill`, adicione ao painel e retorne-o.

### Passo 5 — Construir `Views/FormProdutos.cs` (Versão de Consulta) e Ligar no Menu
1. Crie **`Views/FormProdutos.cs`** (`public sealed class FormProdutos : Form`) com construtor recebendo **`(Sessao sessao, ProdutoService produtos)`**:
   - Primeira linha: `Permissoes.ExigirAdministrador(sessao);`.
   - Prepare o painel com `Ui.Preparar(this, "Administração — Produtos")`.
   - Crie `var busca = Ui.Texto(painel, "Buscar por nome ou código (até 200 resultados)");`.
   - Crie `var grade = Ui.Grade(painel);`.
   - Declare uma **função local assíncrona** dentro do construtor:
     `async Task Carregar() { grade.DataSource = (await produtos.ListarAsync(sessao, busca.Text)).ToList(); }`.
   - Crie o botão `"Buscar"` chamando `async (_, _) => await Ui.ExecutarAsync(Carregar)`.
   - Inscreva o evento **`Shown += async (_, _) => await Ui.ExecutarAsync(Carregar);`** para a grade já carregar automaticamente ao abrir a janela!
2. **Conectar a composição (`A16.05`):**
   - No **`Program.cs`**: crie `var produtos = new ProdutoRepository(banco);` e `var produtoService = new ProdutoService(produtos);`, passando `produtoService` para `new FormAdministrador(sessao, produtoService)`.
   - No **`FormAdministrador.cs`**: receba `ProdutoService produtos` no construtor e adicione um botão `"Produtos"` cujo clique abre `using var tela = new FormProdutos(sessao, produtos); tela.ShowDialog(this);`.

---

## 3. Verificação e Comparação dos 3 Resultados Possíveis (`A16.01`–`A16.05`)

*(Antes desta prática, o professor cadastra dois produtos fictícios — Arroz e Feijão — no banco didático para teste da consulta.)*

Execute com **`F5`** no Visual Studio 2022 e diferencie no caderno **três cenários distintos**:
1. **Lista vazia (resultado válido):** busque `"zzzzz"` → a grade fica vazia sem erro nem exceção.
2. **Consultas especiais:** teste `"arr"`, o código completo com zeros à esquerda, busca vazia (lista ordenada por nome até 200), `"%"` e um apóstrofo `'` (confirmando que não quebra o SQL!).
3. **Falha de rede vs Permissão negada:** compare o comportamento sem internet (consulta não concluída, aviso amigável) e ao chamar `ProdutoService.ListarAsync` com sessão de `Caixa` (recusa imediata *antes* de tocar no banco).

[Anterior: Aula 15](15-Modelo-de-Produtos.md) · [Próxima: Aula 17 — Cadastro, edição e inativação](17-Cadastro-de-Produtos.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 16 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-16). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
