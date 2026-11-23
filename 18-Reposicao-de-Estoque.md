# Aula 18 — Reposição de Estoque com Transação Atômica e Auditoria

**Duração:** 3h30. **Pré-requisito:** [Aula 17](17-Cadastro-de-Produtos.md) e ao menos um produto ativo cadastrado. **Entrega:** operação de reposição que atualiza `produto.estoque` e registra o histórico em `movimento_estoque` dentro da mesma transação de banco de dados, atribuindo automaticamente o administrador logado como responsável.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Conferir saldo inicial `0` dos produtos cadastrados na Aula 17 |
| Explicação | 30 | Transação ACID, rollback automático no descarte e incremento atômico no SQL |
| Construção | 60 | Construir `ReporAsync` no contrato, repositório e serviço |
| Intervalo | 15 | Pausa |
| Prática | 60 | Adicionar controles de reposição no `FormProdutos` e auditar no SQL Editor |
| Entrega | 30 | Demonstrar conciliação matemática (`A18.01`–`A18.05`) e acesso negado |

---

## 1. Dois Problemas Críticos Que Esta Aula Resolve (Entenda o "Por Quê")

### 1.1 Por que usar Transação (`BeginTransactionAsync` + `CommitAsync`)? (`A18.02`)
Uma reposição de mercadoria executa **duas gravações** em tabelas diferentes:
1. Aumentar o saldo em `produto`;
2. Gravar o registro de auditoria (`+quantidade`, `usuario_id`, `'Reposicao'`) em `movimento_estoque`.

**O que acontece SEM transação se a internet cair exatamente entre o comando 1 e o comando 2?**
O saldo sobe para `+10`, mas ninguém sabe quem colocou aquelas 10 unidades nem quando!
Com uma **Transação**, ambas as gravações formam uma unidade indivisível (**Atomicidade**): **ou as duas confirmam juntas no `CommitAsync()`, ou nenhuma é gravada** (se ocorrer qualquer exceção antes do `CommitAsync()`, o `await using` da transação dispara **Rollback automático** ao encerrar o método!).

> [!IMPORTANT]
> **Regra obrigatória em C# / Npgsql:** todos os comandos que fazem parte da mesma transação precisam receber **a mesma `conexao`** e **a mesma `transacao`** (`new NpgsqlCommand(sql, conexao, transacao)`). Usar `banco.CreateCommand(...)` criaria comandos fora da transação e destruiria a atomicidade!

### 1.2 Por que somar no SQL (`estoque = estoque + @q`) em vez de somar no C#? (`A18.01`)
Imagine o saldo inicial `0` e dois funcionários repondo mercadoria ao mesmo tempo (Funcionário A repõe `+10`, Funcionário B repõe `+3`):

- **❌ Estratégia errada (ler para o C#, somar e sobrescrever `SET estoque = @novo`):**
  1. Funcionário A lê `0` e calcula `0 + 10 = 10` em C#.
  2. Funcionário B também lê `0` (antes do A gravar) e calcula `0 + 3 = 3` em C#.
  3. A grava `10`; logo depois B grava `3` — **as 10 unidades do A sumiram!** (Isso se chama *Lost Update* / Perda de Atualização.)
- **✅ Estratégia correta (Incremento Atômico no SQL):**
  `UPDATE produto SET estoque = estoque + @q WHERE id = @id AND ativo`
  O PostgreSQL trava brevemente a linha durante o `UPDATE`, soma `+10` (vira `10`) e na sequência o segundo comando soma `+3` sobre `10` (vira **`13`**)!

### 1.3 Por que o `usuarioId` vem da `Sessao` e não de um campo digitado na tela? (`A18.03`)
O operador nunca pode digitar *"quem fez a reposição"* em uma caixa de texto. O `ProdutoService` extrai `sessao.Id` do administrador autenticado e repassa ao repositório. Além disso, reposição é uma ação separada do botão **Salvar**: nunca permitimos digitar quantidade negativa para "corrigir" estoque (inventário e estorno exigem regras próprias de auditoria).

---

## 2. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

### Passo 1 — Evoluir `IProdutoRepository.cs` e `ProdutoService.cs`
1. Em **`Repositories/IProdutoRepository.cs`**, adicione a assinatura:
   - `Task ReporAsync(int id, int quantidade, int usuarioId);`
2. Em **`Services/ProdutoService.cs`**, implemente **`public Task ReporAsync(Sessao sessao, int id, int quantidade)`**:
   - Exija `Permissoes.ExigirAdministrador(sessao);`.
   - Valide `if (quantidade <= 0)` → lance `ArgumentException("Reposição deve ser maior que zero.")`.
   - Retorne `produtos.ReporAsync(id, quantidade, sessao.Id);`.

### Passo 2 — Construir `ReporAsync` Transacional em `Repositories/ProdutoRepository.cs`
Implemente `public async Task ReporAsync(int id, int quantidade, int usuarioId)` seguindo exatamente estas 5 etapas:
1. Abra uma conexão dedicada do pool com descarte assíncrono: `await using var conexao = await banco.OpenConnectionAsync();`.
2. Inicie a transação sobre essa conexão: `await using var transacao = await conexao.BeginTransactionAsync();`.
3. **Primeiro comando (Incremento atômico):**
   - Crie `await using var atualizar = new NpgsqlCommand("UPDATE produto SET estoque=estoque+@q WHERE id=@id AND ativo", conexao, transacao);`.
   - Adicione os parâmetros `"q"` (`quantidade`) e `"id"` (`id`).
   - Execute `if (await atualizar.ExecuteNonQueryAsync() != 1)` → lance `InvalidOperationException("Produto inexistente ou inativo.")` *(note que se o produto estiver inativo, a exceção interrompe o método aqui e a transação sofre rollback sem gravar movimento!)*.
4. **Segundo comando (Registro de auditoria):**
   - Crie `await using var movimento = new NpgsqlCommand(""" INSERT INTO movimento_estoque(produto_id, usuario_id, quantidade, motivo) VALUES (@id, @usuario, @q, 'Reposicao') """, conexao, transacao);`.
   - Adicione os parâmetros `"id"`, `"usuario"` (`usuarioId`) e `"q"` (`quantidade`).
   - Execute `await movimento.ExecuteNonQueryAsync();`.
5. **Confirmação única:** somente após os dois comandos terem sucesso, execute **`await transacao.CommitAsync();`**.

### Passo 3 — Adicionar o Bloco de Reposição em `Views/FormProdutos.cs`
No construtor de `FormProdutos.cs`, logo após os botões CRUD da Aula 17 e **antes** da linha `Shown += ...`:
1. Adicione um rótulo: `painel.Controls.Add(Ui.Rotulo("Quantidade para repor no produto selecionado"));`.
2. Crie um `NumericUpDown` chamado `quantidade` configurado com `Minimum = 1`, `Maximum = 1000000`, `Value = 1`, `Width = 180` e adicione-o ao painel com `painel.Controls.Add(quantidade);`.
3. Adicione o botão **`"Registrar reposição"`** com evento `async (sender, _) =>`:
   - Desabilite o botão (`var b = (Button)sender!; b.Enabled = false;`).
   - Dentro de `try`, chame `await Ui.ExecutarAsync(async () => { await produtos.ReporAsync(sessao, Selecionado().Id, (int)quantidade.Value); await Carregar(); });`.
   - No `finally`, reabilite `b.Enabled = true;`.

---

## 3. Auditoria Matemática no Banco e Verificação (`A18.01`–`A18.05`)

1. **Linha do tempo no papel (`A18.01`):** desenhe o comparativo entre duas reposições (`+10` e `+3`) por leitura/sobrescrita vs incremento atômico no SQL.
2. **Teste prático no Visual Studio 2022 (`F5`):**
   - Cadastre um produto novo (nasce com saldo `0`).
   - Tente clicar em **Registrar reposição** sem selecionar nenhuma linha → deve recusar pedindo seleção.
   - Selecione o produto, reponha **`10`** unidades e depois reponha **`3`** unidades → a grade deve atualizar para **`13`**.
3. **Conciliação no SQL Editor (`A18.03`):** execute:
   ```sql
   SELECT id, codigo, estoque FROM produto ORDER BY id;
   SELECT produto_id, usuario_id, quantidade, motivo, criado_em
   FROM movimento_estoque ORDER BY id;
   ```
   Confirme que o saldo é `13`, existem exatamente dois movimentos (`+10` e `+3`), a soma dos movimentos bate com o saldo e o `usuario_id` é o do administrador conectado!
4. **Testes de recusa (`A18.04`):** verifique que quantidade `0`/negativa, produto **inativo** e sessão de **`Caixa`** são recusados sem alterar saldo nem criar movimento.

[Anterior: Aula 17](17-Cadastro-de-Produtos.md) · [Próxima: Aula 19 — Carrinho do caixa](19-Carrinho-do-Caixa.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 18 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-18). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
