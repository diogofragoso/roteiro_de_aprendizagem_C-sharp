# Aula 19 — Reposição de Estoque com Transação Atômica e Auditoria

**Duração:** 3h30. **Pré-requisito:** [Aula 18](18-Cadastro-de-Produtos.md) e ao menos um produto ativo cadastrado. **Entrega:** operação de reposição que incrementa `produto.estoque` atomicamente no SQL e registra a entrada em `movimento_estoque` dentro da mesma transação, atribuindo o administrador autenticado como responsável (`A19.01`–`A19.05`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Conferir saldo inicial `0` dos produtos cadastrados na Aula 18 |
| Explicação | 30 | Transação ACID, rollback automático no descarte e incremento atômico (`estoque + @q`) |
| Construção | 60 | Implementar `ReporAsync` no contrato, repositório transacional e serviço |
| Intervalo | 15 | Pausa |
| Prática | 60 | Adicionar o bloco de reposição em `FormProdutos` e auditar no SQL Editor |
| Entrega | 30 | Demonstrar conciliação matemática (`+10` + `+3` = `13`) e recusas (`A19.01`–`A19.05`) |

---

## 1. Dois Problemas Críticos Que Esta Aula Resolve

### 1.1 Transação ACID (`BeginTransactionAsync` + `CommitAsync`) (`A19.02`)
Uma reposição grava em **duas tabelas** (`UPDATE produto` + `INSERT movimento_estoque`). Sem transação, uma queda de rede entre os dois comandos aumentaria o saldo sem deixar auditoria!
Com transação: **ou os dois confirmam juntos no `CommitAsync()`, ou nenhum é gravado** (se houver qualquer exceção antes do `CommitAsync()`, o `await using var transacao` dispara **Rollback automático**!).

> [!IMPORTANT]
> **Regra de ouro em Npgsql:** todos os comandos da transação precisam receber **a mesma `conexao` e a mesma `transacao`** (`new NpgsqlCommand(sql, conexao, transacao)`). Chamar `banco.CreateCommand(...)` criaria comandos **fora** da transação!

### 1.2 Incremento Atômico no SQL vs Perda de Atualização (*Lost Update*) (`A19.01`)
Com saldo inicial `0`, se dois administradores repõem `+10` e `+3` ao mesmo tempo:
- **❌ Errado (somar no C# e gravar `SET estoque = @novo`):** ambos leem `0`; A grava `10`; B grava `3` → **10 unidades sumiram!**
- **✅ Correto (incremento atômico no SQL):** `UPDATE produto SET estoque = estoque + @q WHERE id = @id AND ativo` → o banco trava a linha, soma `+10` (`10`) e depois soma `+3` (**`13`**)!

---

## 2. Código Completo e Compilável desta Etapa (4 Trechos)

### Passo 1 — Contrato `Repositories/IProdutoRepository.cs` e Serviço `Services/ProdutoService.cs`

```csharp
// Em Repositories/IProdutoRepository.cs:
Task ReporAsync(int id, int quantidade, int usuarioId);

// Em Services/ProdutoService.cs:
public Task ReporAsync(Sessao sessao, int id, int quantidade)
{
    Permissoes.ExigirAdministrador(sessao);
    if (quantidade <= 0)
        throw new ArgumentException("Reposição deve ser maior que zero.");

    // O usuarioId vem SEMPRE da Sessao autenticada, nunca de um campo digitado na tela!
    return produtos.ReporAsync(id, quantidade, sessao.Id);
}
```

### Passo 2 — Implementação Transacional em `Repositories/ProdutoRepository.cs`

```csharp
public async Task ReporAsync(int id, int quantidade, int usuarioId)
{
    // 1. Abre conexão dedicada do Pool e inicia a transação:
    await using var conexao = await banco.OpenConnectionAsync();
    await using var transacao = await conexao.BeginTransactionAsync();

    // 2. Comando 1 — Incremento atômico no SQL (passando conexao E transacao!):
    await using var atualizar = new NpgsqlCommand("""
        UPDATE produto
        SET estoque = estoque + @q
        WHERE id = @id AND ativo
        """, conexao, transacao);
    atualizar.Parameters.AddWithValue("q", quantidade);
    atualizar.Parameters.AddWithValue("id", id);

    if (await atualizar.ExecuteNonQueryAsync() != 1)
        throw new InvalidOperationException("Produto inexistente ou inativo.");

    // 3. Comando 2 — Registro de auditoria na MESMA conexao e transacao:
    await using var movimento = new NpgsqlCommand("""
        INSERT INTO movimento_estoque(produto_id, usuario_id, quantidade, motivo)
        VALUES (@id, @usuario, @q, 'Reposicao')
        """, conexao, transacao);
    movimento.Parameters.AddWithValue("id", id);
    movimento.Parameters.AddWithValue("usuario", usuarioId);
    movimento.Parameters.AddWithValue("q", quantidade);
    await movimento.ExecuteNonQueryAsync();

    // 4. Confirmação única (se qualquer linha acima falhar, o await using faz ROLLBACK!):
    await transacao.CommitAsync();
}
```

### Passo 3 — Bloco de Reposição em `Views/FormProdutos.cs`

Ajuste a altura de `Ui.Preparar(this, "Administração — Produtos e estoque", 960, 900)` e adicione este bloco **logo antes** da linha `Shown += ...` no construtor de `FormProdutos.cs`:

```csharp
p.Controls.Add(Ui.Rotulo("Quantidade para repor no produto selecionado"));

// NumericUpDown INTEIRO (DecimalPlaces = 0, Minimum = 1 — contraste com o de preço na Aula 10!):
var quantidade = new NumericUpDown
{
    Minimum = 1,
    Maximum = 1000000,
    Value = 1,
    Width = 180
};
p.Controls.Add(quantidade);

Ui.Botao(p, "Registrar reposição", async (sender, _) =>
{
    var b = (Button)sender!;
    b.Enabled = false;
    try
    {
        await Ui.ExecutarAsync(async () =>
        {
            await produtos.ReporAsync(sessao, Selecionado().Id, (int)quantidade.Value);
            await Carregar(); // Atualiza a grade mostrando o novo saldo!
        });
    }
    finally { b.Enabled = true; }
});
```

---

## 3. Auditoria Matemática no Banco e Verificação (`A19.01`–`A19.05`)

1. **Teste na tela (`F5`):** cadastre um produto novo (saldo `0`), tente repor sem selecionar nada (recusado), depois selecione-o e reponha **`10`** e em seguida **`3`** → saldo na grade vai para **`13`**.
2. **Conciliação no SQL Editor (`A19.03`):**
   ```sql
   SELECT id, codigo, estoque FROM produto ORDER BY id;
   SELECT produto_id, usuario_id, quantidade, motivo, criado_em
   FROM movimento_estoque ORDER BY id;
   ```
   Confirme que o saldo é `13`, existem exatamente dois movimentos (`+10` e `+3`) e o `usuario_id` é o administrador conectado!
3. **Testes de recusa (`A19.04`):** quantidade `0`/negativa, produto inativo e sessão de `Caixa` são recusados sem alterar saldo nem gravar histórico.

[Anterior: Aula 18](18-Cadastro-de-Produtos.md) · [Próxima: Aula 20 — Carrinho do caixa](20-Carrinho-do-Caixa.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 19 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-19). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
