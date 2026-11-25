# Aula 18 — Cadastro, Edição, Unicidade e Inativação Lógica de Produtos

**Duração:** 3h30. **Pré-requisito:** [Aula 17](17-Consulta-de-Produtos.md). **Entrega:** `FormProdutos` completo para incluir, editar e inativar produtos reais no banco (sem o bloco de reposição ainda, que entra na Aula 19), tratando códigos duplicados (`23505`) e preservando os dados digitados em caso de erro.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Consultar os produtos existentes na grade da Aula 17 |
| Explicação | 30 | `INSERT` vs `UPDATE`, `DataBoundItem`, `NumericUpDown`, `23505` e exclusão lógica |
| Construção | 60 | Implementar `InativarAsync`, tratar `23505` no `Ui` e construir campos de cadastro |
| Intervalo | 15 | Pausa |
| Prática | 60 | Construir ações Novo, Editar, Salvar e Inativar com confirmação e proteção de clique |
| Entrega | 30 | Demonstrar a sequência completa (`A18.01`–`A18.06`) e registrar evidências |

---

## 1. Conceitos e Decisões Técnicas Desta Aula (O Quê e Por Quê)

### 1.1 Um Mesmo Formulário para Incluir e Editar (`private int produtoId;`)
Como o formulário sabe se o clique no botão **Salvar** deve criar um produto novo (`INSERT`) ou atualizar um produto existente (`UPDATE`)?
- Mantemos um campo privado **`private int produtoId;`** na classe `FormProdutos`:
  - **`produtoId == 0`:** modo **Inclusão** (novo produto; o banco gera o ID automaticamente via `IDENTITY`, deixa `estoque = 0` e `ativo = true`).
  - **`produtoId > 0`:** modo **Edição** (quando o usuário seleciona uma linha na grade e clica em **Editar selecionado**, guardamos o `Id` real daquela linha em `produtoId` e copiamos `Codigo`, `Nome` e `Preco` para os campos).
- **Por que o usuário NUNCA digita o ID?** O ID é uma identidade interna controlada exclusivamente pelo banco de dados. Clicar em **Novo** limpa os campos e volta `produtoId = 0;` para garantir que o próximo salvamento não sobrescreva o item editado anteriormente!

### 1.2 Lendo o Objeto Real da Grade: `SelectedRows[0].DataBoundItem` (`A18.02`)
**Por que NÃO usar o número da linha visual (`SelectedRows[0].Index`) nem `CurrentRow`?**
1. Se o usuário reordenar a tabela clicando no cabeçalho de uma coluna, a linha `0` deixa de ser o mesmo produto! Já **`DataBoundItem`** devolve diretamente o objeto **`Produto`** original vinculado àquela linha.
2. **`CurrentRow`** pode continuar apontando para a primeira linha mesmo depois que você chama `grade.ClearSelection()`. Por isso, sempre conferimos:
   ```csharp
   if (grade.SelectedRows.Count == 0 || grade.SelectedRows[0].DataBoundItem is not Produto produto)
       throw new InvalidOperationException("Selecione um produto na grade.");
   ```

### 1.3 Por Que Verificar `ExecuteNonQueryAsync() != 1`? (`A18.03`)
No `UPDATE produto SET ... WHERE id=@id AND ativo`:
- `ExecuteNonQueryAsync()` devolve **quantas linhas foram modificadas** no banco.
- Se retornar **`0`**, significa que o produto não existe ou foi inativado por outra janela segundos antes! Lançar `InvalidOperationException` impede que o sistema finja que a edição deu certo quando nada foi gravado.

### 1.4 Concorrência de Código Duplicado: Por Que `SELECT` Antes do `INSERT` Não Basta? (`A18.04`)
Um erro clássico de iniciantes é tentar garantir código único fazendo `SELECT COUNT(*) WHERE codigo = @c` antes do `INSERT`.
- **Por que isso falha na vida real?** Se dois administradores clicarem em Salvar com o código `"000123"` no mesmo milissegundo, **ambos** passam pelo `SELECT` (que ainda retorna `0`) e tentam fazer `INSERT`!
- **A solução profissional:** a restrição **`UNIQUE`** no PostgreSQL é a juíza definitiva. Quando um segundo `INSERT` colide, o PostgreSQL lança `PostgresException` com código **`23505`** (`PostgresErrorCodes.UniqueViolation`). Interceptamos exatamente esse código em `Ui.ExecutarAsync` e mostramos uma mensagem clara!

### 1.5 Exclusão Lógica (`ativo = false`) vs `DELETE` (`A18.05`)
Nunca executamos `DELETE FROM produto` porque vendas antigas (Aula 21) terão chaves estrangeiras apontando para aquele `produto_id`. Inativar apenas marca `ativo = false`: o histórico permanece íntegro e o código antigo continua reservado pela restrição `UNIQUE`.

---

## 2. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

### Passo 1 — Adicionar `InativarAsync` no Contrato, Repositório e Serviço
1. **`Repositories/IProdutoRepository.cs`:** acrescente `Task InativarAsync(int id);`.
2. **`Repositories/ProdutoRepository.cs`:** implemente `InativarAsync(int id)`:
   - Execute `UPDATE produto SET ativo=false WHERE id=@id AND ativo` com parâmetro `"id"`.
   - Verifique `if (await cmd.ExecuteNonQueryAsync() != 1)` → lance `InvalidOperationException("Produto inexistente ou já inativo.")`.
3. **`Services/ProdutoService.cs`:** adicione `public Task InativarAsync(Sessao sessao, int id)`:
   - Exija `Permissoes.ExigirAdministrador(sessao);` e retorne `produtos.InativarAsync(id);`.

### Passo 2 — Tratar Violação de Unicidade (`23505`) em `Views/Ui.cs`
No método `Ui.ExecutarAsync`, adicione `using Npgsql;` no topo do arquivo e acrescente um bloco `catch` **antes** do `catch (Exception e)` genérico:
- `catch (PostgresException e) when (e.SqlState == PostgresErrorCodes.UniqueViolation)` → exiba `MessageBox.Show("Código ou usuário já cadastrado. Use outro valor.");`.
*(Na trilha MySQL/XAMPP, use `catch (MySqlException erro) when (erro.Number == 1062)`.)*

### Passo 3 — Ampliar `Views/FormProdutos.cs` com Campos e Ações CRUD (`A18.01`–`A18.06`)
*(Nesta aula você completa as 4 operações do **CRUD de Produtos** usando os componentes WFA praticados na [Aula 10](10-Componentes-Windows-Forms.md): **C**reate = `TextBox` + `NumericUpDown` + Salvar com ID `0`, **R**ead = `DataGridView` + Buscar/atualizar, **U**pdate = `DataBoundItem` + Editar selecionado + Salvar com ID `> 0` e **D**elete lógico = confirmação `MessageBoxButtons.YesNo` + Inativar selecionado. Deixe os controles de quantidade e reposição para a Aula 19!)*

> [!TIP]
> **Mapa rápido dos componentes WFA usados no CRUD de `FormProdutos`:**
> - `TextBox` (`codigo`, `nome`): preserva zeros à esquerda e recebe texto do produto.
> - `NumericUpDown` (`preco`): configurado com `DecimalPlaces = 2` e `Maximum = 99999999.99m`, entregando `preco.Value` diretamente como `decimal`.
> - `FlowLayoutPanel` horizontal (`acoes`): agrupa os 5 botões lado a lado sem sobreposição.
> - `DataGridView` + `SelectedRows[0].DataBoundItem`: lê o objeto `Produto` real sem depender de índice de coluna.
> - `MessageBox.Show(..., MessageBoxButtons.YesNo)`: protege a inativação contra cliques acidentais.

No **`Views/FormProdutos.cs`**, adicione o campo `private int produtoId;` na classe e amplie o construtor seguindo estas orientações:
1. **Remover o botão `"Buscar"` isolado da Aula 17:** ele agora fará parte da barra horizontal `acoes` abaixo, evitando dois botões de busca duplicados na tela!
2. **Campos de entrada abaixo da grade:**
   - `codigo`: `Ui.Texto(painel, "Código (texto, preserva zeros à esquerda)")`.
   - `nome`: `Ui.Texto(painel, "Nome")`.
   - `preco`: adicione o rótulo `painel.Controls.Add(Ui.Rotulo("Preço de venda"));`, crie um controle **`NumericUpDown`** configurado com `DecimalPlaces = 2`, `Maximum = 99999999.99m`, `Width = 180` e **adicione-o ao painel com `painel.Controls.Add(preco);`** (lembre-se: controles criados com `new` só aparecem na janela depois de adicionados a `Controls`!).
3. **Barra horizontal de ações:** crie um `FlowLayoutPanel` secundário (`var acoes = new FlowLayoutPanel { AutoSize = true, WrapContents = false };`) para alinhar os botões lado a lado e adicione-o ao painel principal (`painel.Controls.Add(acoes);`).
4. **Funções locais auxiliares dentro do construtor:**
   - `void Novo()`: zera `produtoId = 0;`, limpa `codigo` e `nome`, zera `preco.Value = 0;` e chama `grade.ClearSelection();`.
   - `Produto Selecionado()`: verifica `grade.SelectedRows.Count == 0 || grade.SelectedRows[0].DataBoundItem is not Produto produto` (lançando `InvalidOperationException("Selecione um produto na grade.")`) e devolve `produto`.
5. **Os 5 botões na barra `acoes`:**
   - **"Buscar / atualizar":** executa `await Ui.ExecutarAsync(Carregar)`.
   - **"Novo":** chama `Novo()`.
   - **"Editar selecionado":** dentro de `Ui.ExecutarAsync`, pega `var atual = Selecionado();`, preenche `produtoId = atual.Id;`, `codigo.Text = atual.Codigo;`, `nome.Text = atual.Nome;` e `preco.Value = atual.Preco;` (retorne `Task.CompletedTask` se usar a lambda assíncrona).
   - **"Salvar":**
     1. Recupere o botão clicado via `var b = (Button)sender!;` *(aqui `(Button)` converte o `object? sender` de volta para `Button`, e `!` garante ao compilador que `sender` não é `null` num clique de botão)* e desabilite `b.Enabled = false;`.
     2. Em `try`, chame `await Ui.ExecutarAsync(async () => { await produtos.SalvarAsync(sessao, produtoId, codigo.Text, nome.Text, preco.Value); await Carregar(); Novo(); });`.
        - **Por que `Novo()` fica DENTRO da ação, só depois de `SalvarAsync` e `Carregar`?** Porque se houver erro (código duplicado ou falha de rede), `SalvarAsync` lança exceção, `Novo()` **não executa** e os dados digitados permanecem na tela para o usuário corrigir!
     3. No `finally`, reabilite `b.Enabled = true;`.
   - **"Inativar selecionado":** dentro de `Ui.ExecutarAsync`:
     1. Pegue `var atual = Selecionado();`.
     2. Peça confirmação: `if (MessageBox.Show($"Inativar {atual.Nome}?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;`.
     3. Se confirmou, chame `await produtos.InativarAsync(sessao, atual.Id);`, recarregue a grade com `await Carregar();` e chame `Novo();`.

---

## 3. Roteiro de Verificação Sequencial (`A18.01`–`A18.06`)

Execute com **`F5`** no Visual Studio 2022 e execute esta sequência exata registrando IDs e saldos antes/depois:

1. **Incluir:** cadastre `"000123"` / `"Arroz"` / `25,90` → confira zeros preservados, ID gerado, `Estoque = 0` e `Ativo = true`.
2. **Editar:** clique sem selecionar nenhuma linha (deve pedir seleção); depois selecione Arroz, clique em **Editar selecionado**, mude nome/preço e salve → confira que manteve o **mesmo ID** e o saldo intacto.
3. **Novo + Duplicidade:** clique em **Novo** e tente cadastrar outro produto com o mesmo código `"000123"` → deve exibir mensagem amigável de duplicidade (`23505`), sem fechar o app e **sem apagar** os campos digitados.
4. **Validação antes do banco:** tente nome vazio ou preço zero → recusado com mensagem clara.
5. **Inativação (Cancelar vs Confirmar):** selecione o produto, clique em **Inativar**, escolha **Não** (deve continuar `Ativo = true`); repita escolhendo **Sim** (passa para `Ativo = false`).
6. **Persistência real:** feche e reabra `FormProdutos` → todas as alterações continuam persistidas no banco!

[Anterior: Aula 17](17-Consulta-de-Produtos.md) · [Próxima: Aula 19 — Reposição de estoque](19-Reposicao-de-Estoque.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 18 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-18). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
