# Aula 21 — Persistência da Venda: Transação Atômica, Bloqueios (`FOR UPDATE`) e Idempotência (`Guid`)

**Duração:** 3h30. **Pré-requisito:** [Aula 20](20-Carrinho-do-Caixa.md). **Entrega:** tabelas `venda` e `venda_item` criadas, serviço e repositório transacional de venda implementados, com provas reais de rollback após escrita, disputa concorrente pelo último item e idempotência por `Guid` (`A21.01`–`A21.08`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Relembrar que o carrinho da Aula 20 vive só na RAM e não reserva estoque |
| Explicação | 30 | Snapshot histórico, `PedidoItem`, `Guid`, `advisory lock`, `FOR UPDATE` e deadlock |
| Construção | 60 | Paradas 1 e 2: aplicar `003-vendas.sql`, construir modelos, contrato e `VendaService` |
| Intervalo | 15 | Pausa |
| Prática | 60 | Paradas 3 e 4: implementar `VendaRepository` transacional e ensaios de concorrência/rollback |
| Entrega | 30 | Conferir contagens SQL antes/depois e explicar cada etapa da transação |

---

## 1. Esquema `database/003-vendas.sql` e os 5 Pilares de Uma Venda Segura

```sql
BEGIN;

CREATE TABLE venda (
    id         integer       GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    operacao   uuid          NOT NULL UNIQUE,
    usuario_id integer       NOT NULL REFERENCES usuario(id),
    criado_em  timestamptz   NOT NULL DEFAULT now(),
    total      numeric(12,2) NOT NULL CHECK (total > 0),
    recebido   numeric(12,2) NOT NULL CHECK (recebido >= total)
);

CREATE TABLE venda_item (
    venda_id       integer       NOT NULL REFERENCES venda(id),
    produto_id     integer       NOT NULL REFERENCES produto(id),
    nome           varchar(120)  NOT NULL,
    quantidade     integer       NOT NULL CHECK (quantidade > 0),
    preco_unitario numeric(10,2) NOT NULL CHECK (preco_unitario > 0),
    PRIMARY KEY (venda_id, produto_id)
);

CREATE INDEX ix_venda_criado_em ON venda(criado_em);

COMMIT;
```

### Entenda o "Por Quê" de Cada Defesa Antes de Escrever C#

1. **Snapshot Histórico (`A21.01`):** `venda_item` copia `nome` e `preco_unitario` do momento da venda para que futuras alterações de preço nunca mudem comprovantes antigos.
2. **Autoridade do Preço no Banco (`A21.02`):** `VendaService` envia apenas `PedidoItem(ProdutoId, Quantidade)` — o preço oficial é lido direto da tabela `produto` dentro da transação!
3. **Idempotência (`Guid` + `pg_advisory_xact_lock`) (`A21.03` e `A21.08`):** se uma mesma tentativa (`operacao`) for reenviada duas vezes (mesmo simultaneamente), o lock serializa as chamadas e a consulta por `operacao` devolve a venda já gravada sem cobrar nem baixar estoque em dobro!
4. **Concorrência pelo Último Item (`FOR UPDATE`) + Ordenação Anti-Deadlock (`A21.04` e `A21.06`):**
   ```mermaid
   sequenceDiagram
       participant C1 as Caixa 1 (Chave A)
       participant DB as PostgreSQL (Produto 5: Estoque = 1)
       participant C2 as Caixa 2 (Chave B)
       C1->>DB: BEGIN + SELECT ... WHERE id=5 FOR UPDATE
       Note over DB: Linha do Produto 5 TRAVADA pelo Caixa 1!
       C2->>DB: BEGIN + SELECT ... WHERE id=5 FOR UPDATE
       Note over C2: Caixa 2 AGUARDA a liberação da linha...
       C1->>DB: Estoque 1 >= 1 (OK) -> INSERT venda + UPDATE estoque=0 + COMMIT
       DB-->>C1: Venda Confirmada! (Libera bloqueio)
       Note over DB: Caixa 2 agora lê Estoque = 0!
       DB-->>C2: Estoque insuficiente -> ROLLBACK!
   ```
   Ordenar `itens.OrderBy(i => i.ProdutoId)` garante que todos os caixas bloqueiem linhas sempre na mesma ordem crescente, eliminando risco de *deadlock*!
5. **`INSERT ... RETURNING id` (`A21.07`):** devolve atomicamente o ID exato da venda recém-inserida (nunca use `SELECT MAX(id)`!).

---

## 2. Código Completo e Compilável em 4 Paradas (`Ctrl+Shift+B` em cada parada!)

### Parada 1 — Modelos (`Models/`) e Contrato (`Repositories/IVendaRepository.cs`)

Execute [database/003-vendas.sql](database/003-vendas.sql) **uma única vez** no SQL Editor e crie os 4 arquivos:

```csharp
// Models/PedidoItem.cs (NÃO carrega preço da tela!)
namespace SupermercadoApp.Models;
public sealed record PedidoItem(int ProdutoId, int Quantidade);

// Models/VendaConfirmada.cs
namespace SupermercadoApp.Models;
public sealed record VendaConfirmada(int Id, decimal Total, decimal Recebido)
{
    public decimal Troco => Recebido - Total;
}

// Models/ResumoVenda.cs (usado no relatório da Aula 23)
namespace SupermercadoApp.Models;
public sealed record ResumoVenda(int Id, DateTime DataUtc, string Operador, decimal Total);
```

```csharp
// Repositories/IVendaRepository.cs (RelatorioAsync entra na Aula 23!)
using SupermercadoApp.Models;

namespace SupermercadoApp.Repositories;

public interface IVendaRepository
{
    Task<VendaConfirmada?> BuscarAsync(Guid operacao);
    Task<VendaConfirmada> FinalizarAsync(Guid operacao, int usuarioId,
        IReadOnlyList<PedidoItem> itens, decimal recebido);
}
```

### Parada 2 — Evoluir `Services/VendaService.cs`

```csharp
using SupermercadoApp.Models;
using SupermercadoApp.Repositories;

namespace SupermercadoApp.Services;

public sealed class VendaService(IVendaRepository vendas, IProdutoRepository produtos)
{
    public Task<Produto?> BuscarProdutoAsync(Sessao sessao, string codigo)
    {
        Permissoes.ExigirCaixa(sessao);
        return produtos.BuscarCodigoAsync(codigo.Trim());
    }

    public Task<VendaConfirmada?> ConsultarAsync(Sessao sessao, Guid operacao)
    {
        Permissoes.ExigirCaixa(sessao);
        return vendas.BuscarAsync(operacao);
    }

    public Task<VendaConfirmada> FinalizarAsync(Sessao sessao, Guid operacao,
        IReadOnlyList<ItemCarrinho> itens, decimal recebido)
    {
        Permissoes.ExigirCaixa(sessao);

        if (operacao == Guid.Empty || itens.Count == 0 || itens.Any(i => i.Quantidade <= 0))
            throw new ArgumentException("Adicione itens válidos antes de finalizar.");

        if (itens.Select(i => i.ProdutoId).Distinct().Count() != itens.Count)
            throw new ArgumentException("Agrupe os produtos repetidos no carrinho.");

        if (recebido < 0 || recebido > 9999999999.99m || decimal.Round(recebido, 2) != recebido)
            throw new ArgumentException("Valor recebido inválido.");

        // Converte para PedidoItem(ProdutoId, Quantidade): preço oficial vem do banco!
        return vendas.FinalizarAsync(operacao, sessao.Id,
            itens.Select(i => new PedidoItem(i.ProdutoId, i.Quantidade)).ToArray(), recebido);
    }
}
```

### Parada 3 — `Repositories/VendaRepository.cs` Completo + Composição em `Program.cs`

```csharp
using Npgsql;
using SupermercadoApp.Models;

namespace SupermercadoApp.Repositories;

public sealed class VendaRepository(NpgsqlDataSource banco) : IVendaRepository
{
    public async Task<VendaConfirmada?> BuscarAsync(Guid operacao)
    {
        await using var cmd = banco.CreateCommand(
            "SELECT id, total, recebido FROM venda WHERE operacao=@op");
        cmd.Parameters.AddWithValue("op", operacao);
        await using var r = await cmd.ExecuteReaderAsync();
        return await r.ReadAsync() ? new(r.GetInt32(0), r.GetDecimal(1), r.GetDecimal(2)) : null;
    }

    public async Task<VendaConfirmada> FinalizarAsync(Guid operacao, int usuarioId,
        IReadOnlyList<PedidoItem> itens, decimal recebido)
    {
        // 1. Conexão dedicada + Transação ACID:
        await using var conexao = await banco.OpenConnectionAsync();
        await using var transacao = await conexao.BeginTransactionAsync();

        // 2. Advisory lock: serializa tentativas simultâneas da MESMA chave Guid:
        await using (var trava = new NpgsqlCommand(
            "SELECT pg_advisory_xact_lock(hashtextextended(@op, 0))", conexao, transacao))
        {
            trava.Parameters.AddWithValue("op", operacao.ToString());
            await trava.ExecuteNonQueryAsync();
        }

        // 3. Idempotência: se essa operação já foi gravada, devolve sem duplicar!
        await using (var consulta = new NpgsqlCommand(
            "SELECT id, total, recebido FROM venda WHERE operacao=@op AND usuario_id=@usuario",
            conexao, transacao))
        {
            consulta.Parameters.AddWithValue("op", operacao);
            consulta.Parameters.AddWithValue("usuario", usuarioId);
            await using var r = await consulta.ExecuteReaderAsync();
            if (await r.ReadAsync())
                return new(r.GetInt32(0), r.GetDecimal(1), r.GetDecimal(2));
        }

        // 4. Bloqueio de linhas em ordem crescente (FOR UPDATE -> anti-deadlock e anti-estoque negativo):
        var confirmados = new List<ItemCarrinho>();
        foreach (var item in itens.OrderBy(i => i.ProdutoId))
        {
            await using var cmd = new NpgsqlCommand("""
                SELECT nome, preco, estoque, ativo FROM produto WHERE id=@id FOR UPDATE
                """, conexao, transacao);
            cmd.Parameters.AddWithValue("id", item.ProdutoId);
            await using var r = await cmd.ExecuteReaderAsync();
            if (!await r.ReadAsync() || !r.GetBoolean(3) || r.GetInt32(2) < item.Quantidade)
                throw new InvalidOperationException("Produto inativo ou estoque insuficiente. Revise o carrinho.");

            confirmados.Add(new(item.ProdutoId, r.GetString(0), r.GetDecimal(1), item.Quantidade));
        }

        // 5. Recalcula total oficial pelo banco e confere valor recebido:
        decimal total = confirmados.Sum(i => i.Subtotal);
        if (total <= 0 || total > 9999999999.99m || recebido < total)
            throw new InvalidOperationException($"Total atualizado: {total:C}. Valor recebido insuficiente ou total inválido.");

        // 6. Grava cabeçalho capturando o ID gerado atomicamente com RETURNING id:
        int vendaId;
        await using (var cmd = new NpgsqlCommand("""
            INSERT INTO venda(operacao, usuario_id, total, recebido)
            VALUES (@op, @usuario, @total, @recebido) RETURNING id
            """, conexao, transacao))
        {
            cmd.Parameters.AddWithValue("op", operacao);
            cmd.Parameters.AddWithValue("usuario", usuarioId);
            cmd.Parameters.AddWithValue("total", total);
            cmd.Parameters.AddWithValue("recebido", recebido);
            vendaId = (int)(await cmd.ExecuteScalarAsync())!;
        }

        // 7. Grava snapshot dos itens, baixa estoque e registra movimento negativo na MESMA transação:
        foreach (var item in confirmados)
        {
            await using var cmd = new NpgsqlCommand("""
                INSERT INTO venda_item(venda_id, produto_id, nome, quantidade, preco_unitario)
                VALUES (@venda, @produto, @nome, @q, @preco);
                UPDATE produto SET estoque=estoque-@q WHERE id=@produto;
                INSERT INTO movimento_estoque(produto_id, usuario_id, quantidade, motivo)
                VALUES (@produto, @usuario, -@q, 'Venda');
                """, conexao, transacao);
            cmd.Parameters.AddWithValue("venda", vendaId);
            cmd.Parameters.AddWithValue("produto", item.ProdutoId);
            cmd.Parameters.AddWithValue("nome", item.Nome);
            cmd.Parameters.AddWithValue("q", item.Quantidade);
            cmd.Parameters.AddWithValue("preco", item.Preco);
            cmd.Parameters.AddWithValue("usuario", usuarioId);
            await cmd.ExecuteNonQueryAsync();
        }

        // 8. Commit único: ou TUDO acima é salvo junto, ou TUDO sofre rollback!
        await transacao.CommitAsync();
        return new(vendaId, total, recebido);
    }
}
```

Em **`Program.cs`**, atualize a composição:
```csharp
var vendaService = new VendaService(new VendaRepository(banco), produtosRepo);
```

### Parada 4 — Botão Controlado de Teste em `FormCaixa` e Ensaios Reais (`A21.01`–`A21.08`)

Adicione temporariamente em `FormCaixa.cs` o campo `private Guid operacao = Guid.NewGuid();` e este botão de ensaio *(será substituído pela tela definitiva de pagamento na Aula 22)*:

```csharp
Ui.Botao(p, "Teste controlado: recebido 100 / mesma chave", async (_, _) =>
    await Ui.ExecutarAsync(async () =>
    {
        var resultado = await vendas.FinalizarAsync(sessao, operacao, carrinho.Itens, 100m);
        MessageBox.Show($"Venda {resultado.Id}; total {resultado.Total:C}; operacao {operacao}");
    }));
```

Execute e comprove no SQL Editor (antes e depois):
1. **Idempotência (`A21.03`):** clique 2 vezes no botão de teste → retorna o **mesmo ID** e apenas **uma** baixa!
2. **Snapshot histórico (`A21.01`):** altere o preço do produto como Admin e confira que `venda_item.preco_unitario` preservou o valor original.
3. **Último item concorrente (`A21.06`):** produto com estoque `1`, dois caixas com chaves distintas → 1 confirma, 1 recusa, saldo `0`, sem cabeçalho órfão!
4. **Rollback real após escrita (`A21.05`):** siga o gatilho controlado de [VALIDACAO-MANUAL.md](VALIDACAO-MANUAL.md#rollback-após-alteração-real), comprove que todas as tabelas voltaram às contagens iniciais e remova o gatilho.

Consulte também: [Bloqueios no PostgreSQL](https://www.postgresql.org/docs/current/explicit-locking.html).

[Anterior: Aula 20](20-Carrinho-do-Caixa.md) · [Próxima: Aula 22 — Finalização e troco](22-Finalizacao-e-Troco.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 21 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-21). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
