# Aula 22 — Relatório de Vendas por Período, UTC e Intervalo Semiaberto

**Duração:** 3h30. **Pré-requisito:** [Aula 21](21-Finalizacao-e-Troco.md), com ao menos duas vendas confirmadas no banco. **Entrega:** tela `FormRelatorio` integrada ao menu administrativo que consulta vendas por data local (convertida corretamente para UTC em intervalo semiaberto), exibe a grade e totaliza quantidade de vendas e faturamento.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Conferir vendas reais registradas no banco na Aula 21 |
| Explicação | 30 | Instante UTC (`timestamptz`), horário local, intervalo semiaberto e projeção LINQ |
| Construção | 60 | Implementar `RelatorioAsync` no contrato, repositório (`JOIN`) e serviço |
| Intervalo | 15 | Pausa |
| Prática | 60 | Construir `FormRelatorio` com `DateTimePicker` e conectar ao `FormAdministrador` |
| Entrega | 30 | Conferir fronteiras de data com SQL fixo (`A22.01`–`A22.05`) e perfil negado |

---

## 1. Dois Erros Clássicos com Datas em Sistemas Reais (E Como Evitá-los)

### 1.1 Por Que Guardar em UTC (`timestamptz`) e Converter Só na Tela? (`A22.01`)
Imagine um servidor na nuvem nos EUA (horário UTC) recebendo vendas de um caixa em Brasília (`UTC-3`). Se você gravar datas sem fuso, relatórios quebram na mudança de dia (uma venda às `22:00` de Brasília já é `01:00` do dia seguinte em UTC!).

**A regra de ouro da engenharia:**
1. **No banco (`timestamptz`):** tudo representa um **instante absoluto em UTC**.
2. **Ao enviar filtros da tela para o serviço:** convertemos a data local do `DateTimePicker` para UTC usando **`.Date.ToUniversalTime()`** (isso define `DateTimeKind.Utc`, exigido pelo Npgsql e validado no `VendaService`).
3. **Ao exibir os resultados na grade:** convertemos cada `v.DataUtc` para o horário local do computador usando **`.ToLocalTime()`**.
- **Atenção:** configurar moeda `pt-BR` (`CultureInfo`) **não** muda o fuso horário do Windows! Confira no relógio do Windows do laboratório que o fuso está correto (ex.: Brasília, `America/Sao_Paulo`).

### 1.2 Por Que Usar Intervalo Semiaberto (`>= início AND < fim`) e NUNCA `23:59:59`? (`A22.02` e `A22.03`)
Para o usuário humano, escolher data inicial `16/10` e data final `16/10` significa *"inclua o dia 16 inteiro"*.

- **❌ Jeito perigoso (`<= '2026-10-16 23:59:59'`):** bancos modernos armazenam **microssegundos**! Uma venda gravada às `23:59:59.450` (meio segundo antes da meia-noite) ficaria **de fora** do relatório!
- **✅ Jeito exato (Intervalo Semiaberto `[inicio, proximoDia)`):**
  ```text
  Início inclusivo (>=): 16/10 00:00:00.000 (local → UTC)
  Fim exclusivo    (<) : 17/10 00:00:00.000 (fim.Value.Date.AddDays(1).ToUniversalTime())
  ```
  Qualquer venda até `23:59:59.999999` entra perfeitamente, e uma venda exatamente às `00:00:00.000` do dia `17/10` fica exatamente fora (entrará no relatório do dia 17, sem nunca aparecer duplicada em dois dias!).

### 1.3 Projeção LINQ com Tipo Anônimo (`Select(v => new { ... })`) e Contagem de Vendas
No relatório, `vendasPeriodo` retorna objetos `ResumoVenda(Id, DataUtc, Operador, Total)`. Para exibir a data já convertida em horário local na `DataGridView`, usamos uma projeção LINQ:
```csharp
grade.DataSource = vendasPeriodo
    .Select(v => new { v.Id, DataLocal = v.DataUtc.ToLocalTime(), v.Operador, v.Total })
    .ToList();
```
- **Atenção à métrica (`A22.04`):** `vendasPeriodo.Count` conta o número de **vendas (cupons)**, **não** a soma das quantidades de produtos vendidos; `vendasPeriodo.Sum(v => v.Total)` soma o faturamento do período.
- **Nota de escala (`A22.05`):** neste recorte de laboratório carregamos a lista do período em memória. Em um supermercado com milhões de vendas por ano, a soma e a contagem devem ser calculadas por agregação no SQL (`COUNT(*)`, `SUM(total)`) e os detalhes exibidos com paginação (`LIMIT`/`OFFSET` ou cursor).

---

## 2. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

### Passo 1 — Adicionar `RelatorioAsync` em `IVendaRepository`, `VendaRepository` e `VendaService`
1. **`Repositories/IVendaRepository.cs`:** adicione a última assinatura do contrato:
   - `Task<IReadOnlyList<ResumoVenda>> RelatorioAsync(DateTime inicioUtc, DateTime fimUtc);`
2. **`Repositories/VendaRepository.cs`:** implemente `RelatorioAsync(DateTime inicioUtc, DateTime fimUtc)`:
   - Escreva a consulta SQL unindo `venda` (`v`) e `usuario` (`u`) com **`JOIN`**:
     ```sql
     SELECT v.id, v.criado_em, u.username, v.total
     FROM venda v
     JOIN usuario u ON u.id = v.usuario_id
     WHERE v.criado_em >= @inicio AND v.criado_em < @fim
     ORDER BY v.criado_em, v.id
     ```
   - Adicione os parâmetros `"inicio"` (`inicioUtc`) e `"fim"` (`fimUtc`).
   - Leia com `ExecuteReaderAsync()` e construa a `List<ResumoVenda>` usando `r.GetInt32(0)`, `r.GetDateTime(1)`, `r.GetString(2)` e `r.GetDecimal(3)`.
3. **`Services/VendaService.cs`:** implemente `public Task<IReadOnlyList<ResumoVenda>> RelatorioAsync(Sessao sessao, DateTime inicio, DateTime fim)`:
   - **Primeira linha:** `Permissoes.ExigirAdministrador(sessao);` *(Caixa NÃO acessa relatório financeiro!)*.
   - **Validação de tipo UTC e intervalo:** se `inicio.Kind != DateTimeKind.Utc || fim.Kind != DateTimeKind.Utc || fim <= inicio`, lance `ArgumentException("Informe um intervalo UTC válido.")`.
   - Retorne `vendas.RelatorioAsync(inicio, fim);`.

### Passo 2 — Construir `Views/FormRelatorio.cs` e Conectar ao `FormAdministrador`
1. Crie **`Views/FormRelatorio.cs`** (`public sealed class FormRelatorio : Form`) recebendo **`(Sessao sessao, VendaService vendas)`** no construtor:
   - Exija `Permissoes.ExigirAdministrador(sessao);`.
   - Prepare o painel com `Ui.Preparar(this, "Administração — Vendas por período")`.
   - Crie dois controles **`DateTimePicker`** (`inicio` e `fim`), ambos configurados com `Format = DateTimePickerFormat.Short` e `Value = DateTime.Today`, com rótulos claros (*"Data inicial (horário local do Windows)"* e *"Data final (inclusive)"*), **lembrando de adicionar cada rótulo e cada `DateTimePicker` ao painel com `painel.Controls.Add(...)`**.
   - Crie `var grade = Ui.Grade(painel);` e `var resumo = Ui.Rotulo("Nenhuma consulta realizada."); painel.Controls.Add(resumo);`.
   - Crie o botão **`"Consultar"`** com evento `async (sender, _) =>`:
     1. Desabilite o botão antes do `try` e reabilite no `finally`.
     2. Dentro de `Ui.ExecutarAsync`:
        - Chame `vendas.RelatorioAsync(sessao, inicio.Value.Date.ToUniversalTime(), fim.Value.Date.AddDays(1).ToUniversalTime())`.
        - Projete a lista com `DataLocal = v.DataUtc.ToLocalTime()` para `grade.DataSource`.
        - Atualize `resumo.Text` com `vendasPeriodo.Count` e `vendasPeriodo.Sum(v => v.Total):C`.
2. **Conectar no `FormAdministrador.cs` e `Program.cs`:**
   - Atualize o construtor de `FormAdministrador` para receber também `VendaService vendas` e passe `vendaService` no `Program.cs`.
   - Adicione o botão `"Relatório de vendas"` no `FormAdministrador` abrindo `using var tela = new FormRelatorio(sessao, vendas); tela.ShowDialog(this);`.

---

## 3. Verificação de Fronteiras e Conferência SQL (`A22.01`–`A22.05`)

1. **Massa de fronteiras exatas (`A22.02`):** com orientação do professor, aplique [database/laboratorio/18-fronteiras.sql](database/laboratorio/18-fronteiras.sql) (insere 4 vendas em instantes críticos ao redor de `16/10/2026` no fuso `America/Sao_Paulo`: 1 antes do dia, 2 dentro incluindo o último microssegundo, e 1 exatamente às `00:00:00` do dia `17/10`). Consulte `16/10/2026` a `16/10/2026` no seu `FormRelatorio` → devem aparecer **somente os 2 registros do meio** (soma `R$ 17,00` nessa massa isolada).
2. **Conferência no SQL Editor (usando data literal fixa para reprodutibilidade):**
   ```sql
   SELECT count(*), coalesce(sum(total), 0)
   FROM venda
   WHERE criado_em >= (DATE '2026-10-16'::timestamp AT TIME ZONE 'America/Sao_Paulo')
     AND criado_em < (DATE '2026-10-17'::timestamp AT TIME ZONE 'America/Sao_Paulo');
   ```
3. **Casos de recusa e vazio:**
   - Escolha data final anterior à inicial → recusado pela validação do serviço.
   - Consulte uma data futura sem vendas → grade vazia, `Vendas: 0 | Total: R$ 0,00`.
   - Com sessão de `Caixa`, chame `VendaService.RelatorioAsync` → lança `UnauthorizedAccessException`.

[Anterior: Aula 21](21-Finalizacao-e-Troco.md) · [Próxima: Aula 23 — Cadastro de usuários](23-Cadastro-de-Usuarios.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 22 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-22). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
