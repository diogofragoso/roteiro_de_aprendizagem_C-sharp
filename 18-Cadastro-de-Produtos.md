# Aula 18 — CRUD de Produtos Completo (Create, Update e Delete Lógico) + Concorrência `23505`

**Duração:** 3h30. **Pré-requisito:** [Aula 17](17-Consulta-de-Produtos.md). **Entrega:** `FormProdutos` realizando as 4 operações do **CRUD** de produtos (**C**reate, **R**ead, **U**pdate via `DataBoundItem` e **D**elete lógico com confirmação `YesNo`), tratando códigos duplicados (`23505`) e preservando os campos digitados em caso de erro (`A18.01`–`A18.06`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Consultar os produtos existentes na grade da Aula 17 |
| Explicação | 30 | `INSERT` vs `UPDATE`, `DataBoundItem`, `NumericUpDown`, `23505` e exclusão lógica |
| Construção | 60 | Implementar `InativarAsync`, tratar `23505` em `Ui.cs` e montar campos do `FormProdutos` |
| Intervalo | 15 | Pausa |
| Prática | 60 | Implementar Novo, Editar, Salvar e Inativar com confirmação `YesNo` e `Enabled` |
| Entrega | 30 | Demonstrar a sequência completa (`A18.01`–`A18.06`) e registrar evidências |

---

## 1. As 5 Decisões Profissionais de um CRUD Real

1. **Um único formulário para Incluir e Editar (`private int produtoId;`):**
   - **`produtoId == 0`** → **Create (`INSERT`)**: o banco gera o `id`, inicia `estoque = 0` e `ativo = true`.
   - **`produtoId > 0`** → **Update (`UPDATE ... WHERE id=@id AND ativo`)**: mantém identidade e saldo intactos.
   - O botão **Novo** zera `produtoId = 0;` para evitar sobrescrever o produto anterior!
2. **Seleção segura via `SelectedRows[0].DataBoundItem` (`A18.02`):** devolve o objeto `Produto` original daquela linha (nunca use índice de célula nem `CurrentRow`, que pode sobrar selecionado após `ClearSelection()`).
3. **Checagem `ExecuteNonQueryAsync() != 1` (`A18.03`):** impede o sistema de anunciar sucesso se o produto tiver sido inativado segundos antes por outra janela!
4. **Por que `SELECT` prévio não impede código duplicado (`A18.04`)?** Dois administradores clicando no mesmo milissegundo passariam pelo `SELECT`. A juíza definitiva é a constraint **`UNIQUE`** do PostgreSQL, que lança `PostgresException` com código **`23505`** (`PostgresErrorCodes.UniqueViolation`).
5. **Exclusão Lógica (`ativo = false`) vs `DELETE` (`A18.05`):** preserva a integridade histórica de vendas antigas (Aula 21) e mantém o código reservado.

---

## 2. Código Completo e Compilável desta Etapa (4 Arquivos)

### Passo 1 — Adicionar `InativarAsync` em `IProdutoRepository`, `ProdutoRepository` e `ProdutoService`

```csharp
// Em Repositories/IProdutoRepository.cs:
Task InativarAsync(int id);

// Em Repositories/ProdutoRepository.cs:
public async Task InativarAsync(int id)
{
    await using var cmd = banco.CreateCommand("UPDATE produto SET ativo=false WHERE id=@id AND ativo");
    cmd.Parameters.AddWithValue("id", id);
    if (await cmd.ExecuteNonQueryAsync() != 1)
        throw new InvalidOperationException("Produto inexistente ou já inativo.");
}

// Em Services/ProdutoService.cs:
public Task InativarAsync(Sessao sessao, int id)
{
    Permissoes.ExigirAdministrador(sessao);
    return produtos.InativarAsync(id);
}
```

### Passo 2 — Tratar Duplicidade (`23505`) em `Views/Ui.cs`

Adicione `using Npgsql;` no topo de **`Views/Ui.cs`** e inclua o bloco `catch (PostgresException ...)` **antes** do `catch (Exception e)` genérico *(na trilha MySQL/XAMPP, use `catch (MySqlException erro) when (erro.Number == 1062)`)!*

```csharp
using Npgsql;

// Dentro de Ui.ExecutarAsync:
try { await acao(); return true; }
catch (PostgresException e) when (e.SqlState == PostgresErrorCodes.UniqueViolation)
{
    MessageBox.Show("Código ou usuário já cadastrado. Use outro valor.", "Duplicidade");
}
catch (Exception e) when (e is ArgumentException or InvalidOperationException or UnauthorizedAccessException)
{
    MessageBox.Show(e.Message, "Verifique os dados");
}
catch (Exception e)
{
    System.Diagnostics.Trace.WriteLine($"Falha: {e.GetType().Name}");
    MessageBox.Show("Não foi possível concluir. Verifique a rede e tente novamente.", "Falha de comunicação");
}
return false;
```

### Passo 3 — `Views/FormProdutos.cs` Completo com as 4 Operações do CRUD (`A18.01`–`A18.06`)

> [!TIP]
> **Mapa dos componentes WFA da [Aula 10](10-Componentes-Windows-Forms.md) aplicados aqui:**
> - `TextBox` (`busca`, `codigo`, `nome`), `NumericUpDown` (`preco` com `DecimalPlaces = 2`), `DataGridView` (`grade` com `DataBoundItem`), `FlowLayoutPanel` horizontal (`acoes` para os 5 botões) e `MessageBoxButtons.YesNo` para confirmação da inativação! *(Deixe o bloco de reposição de estoque para a Aula 19.)*

```csharp
using SupermercadoApp.Models;
using SupermercadoApp.Services;

namespace SupermercadoApp.Views;

public sealed class FormProdutos : Form
{
    private int produtoId; // 0 = Create (novo); > 0 = Update (edição)

    public FormProdutos(Sessao sessao, ProdutoService produtos)
    {
        Permissoes.ExigirAdministrador(sessao);

        var p = Ui.Preparar(this, "Administração — Produtos (CRUD)", 960, 780);
        var busca = Ui.Texto(p, "Buscar por nome ou código (até 200 resultados)");
        var grade = Ui.Grade(p);

        // Campos de entrada para Create / Update:
        var codigo = Ui.Texto(p, "Código (texto, preserva zeros à esquerda)");
        var nome = Ui.Texto(p, "Nome");
        p.Controls.Add(Ui.Rotulo("Preço de venda"));
        var preco = new NumericUpDown
        {
            DecimalPlaces = 2,
            Maximum = 99999999.99m,
            Width = 180
        };
        p.Controls.Add(preco);

        // Barra horizontal de ações CRUD:
        var acoes = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
        p.Controls.Add(acoes);

        async Task Carregar() =>
            grade.DataSource = (await produtos.ListarAsync(sessao, busca.Text)).ToList();

        void Novo()
        {
            produtoId = 0;
            codigo.Clear();
            nome.Clear();
            preco.Value = 0;
            grade.ClearSelection();
        }

        Produto Selecionado()
        {
            if (grade.SelectedRows.Count == 0 || grade.SelectedRows[0].DataBoundItem is not Produto produto)
                throw new InvalidOperationException("Selecione um produto na grade.");
            return produto;
        }

        // R — READ:
        Ui.Botao(acoes, "Buscar / atualizar", async (_, _) => await Ui.ExecutarAsync(Carregar));

        // Resetar para modo CREATE:
        Ui.Botao(acoes, "Novo", (_, _) => Novo());

        // Preparar modo UPDATE lendo DataBoundItem:
        Ui.Botao(acoes, "Editar selecionado", async (_, _) => await Ui.ExecutarAsync(() =>
        {
            var atual = Selecionado();
            produtoId = atual.Id;
            codigo.Text = atual.Codigo;
            nome.Text = atual.Nome;
            preco.Value = atual.Preco;
            return Task.CompletedTask;
        }));

        // C / U — CREATE ou UPDATE (conforme produtoId):
        Ui.Botao(acoes, "Salvar", async (sender, _) =>
        {
            var b = (Button)sender!;
            b.Enabled = false;
            try
            {
                await Ui.ExecutarAsync(async () =>
                {
                    await produtos.SalvarAsync(sessao, produtoId, codigo.Text, nome.Text, preco.Value);
                    await Carregar();
                    Novo(); // Só limpa os campos DEPOIS de gravar com sucesso!
                });
            }
            finally { b.Enabled = true; }
        });

        // D — DELETE LÓGICO (inativação com confirmação):
        Ui.Botao(acoes, "Inativar selecionado", async (_, _) => await Ui.ExecutarAsync(async () =>
        {
            var atual = Selecionado();
            if (MessageBox.Show($"Inativar '{atual.Nome}'?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            await produtos.InativarAsync(sessao, atual.Id);
            await Carregar();
            Novo();
        }));

        Shown += async (_, _) => await Ui.ExecutarAsync(Carregar);
    }
}
```

---

## 3. Roteiro de Verificação Sequencial do CRUD (`A18.01`–`A18.06`)

Execute com **`F5`** e registre IDs e saldos antes/depois:

1. **Create (`A18.01`):** cadastre `"000123"` / `"Arroz"` / `25,90` → zeros preservados, ID gerado, `Estoque = 0`, `Ativo = true`.
2. **Update (`A18.02`):** clique em **Editar selecionado** sem selecionar nada (deve avisar); selecione Arroz, edite nome/preço e salve → manteve o **mesmo ID** e saldo `0`.
3. **Novo + Duplicidade `23505` (`A18.03`–`A18.04`):** clique em **Novo** e tente cadastrar outro produto com `"000123"` → mensagem amigável de duplicidade, **sem apagar** os campos digitados!
4. **Validação antes do banco (`A18.06`):** tente nome vazio ou preço `0` → recusado preservando os campos e reabilitando o botão.
5. **Delete Lógico (`A18.05`):** clique em **Inativar**, escolha **Não** (continua `Ativo = true`); repita escolhendo **Sim** (vira `Ativo = false`). Feche e reabra `FormProdutos` comprovando persistência real no banco!

[Anterior: Aula 17](17-Consulta-de-Produtos.md) · [Próxima: Aula 19 — Reposição de estoque](19-Reposicao-de-Estoque.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 18 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-18). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
