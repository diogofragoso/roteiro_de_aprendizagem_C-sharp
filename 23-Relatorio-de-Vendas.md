# Aula 23 — Relatório de Vendas por Período: UTC, Intervalo Semiaberto e `DateTimePicker`

**Duração:** 3h30. **Pré-requisito:** [Aula 22](22-Finalizacao-e-Troco.md), com ao menos duas vendas confirmadas no banco. **Entrega:** tela `FormRelatorio` integrada ao menu administrativo consultando vendas via `JOIN` por data local convertida para UTC em intervalo semiaberto, exibindo grade e totalização (`A23.01`–`A23.05`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Conferir vendas reais registradas no banco na Aula 22 |
| Explicação | 30 | Instante UTC (`timestamptz`), horário local, intervalo semiaberto e projeção LINQ |
| Construção | 60 | Implementar `RelatorioAsync` em `IVendaRepository`, `VendaRepository` (`JOIN`) e `VendaService` |
| Intervalo | 15 | Pausa |
| Prática | 60 | Construir `FormRelatorio` com dois `DateTimePicker` e conectar ao `FormAdministrador` |
| Entrega | 30 | Conferir fronteiras exatas com SQL fixo (`A23.01`–`A23.05`) e bloqueio de `Caixa` |

---

## 1. Dois Erros Clássicos com Datas em Sistemas Reais (E Como Evitá-los)

### 1.1 Guardar em UTC (`timestamptz`) e Converter Só na Tela (`A23.01`)
1. **No banco (`timestamptz`):** toda data representa um **instante absoluto em UTC**.
2. **Da tela para o serviço:** convertemos a data local do `DateTimePicker` usando **`.Value.Date.ToUniversalTime()`** (define `DateTimeKind.Utc`).
3. **Do banco para a `DataGridView`:** convertemos `v.DataUtc` para o relógio do Windows usando **`.ToLocalTime()`**.
- *(Lembrete: configurar cultura `pt-BR` formata moeda/datas, mas **não** muda o fuso horário do Windows!)*

### 1.2 Intervalo Semiaberto `[inicio, proximoDia)` — NUNCA `23:59:59`! (`A23.02` e `A23.03`)
- **❌ Perigoso (`<= '2026-10-16 23:59:59'`):** vendas gravadas às `23:59:59.450` (microssegundos) ficam **de fora**!
- **✅ Exato (`>= inicioUtc AND < fimUtc`):**
  ```text
  Início inclusivo (>=): inicio.Value.Date.ToUniversalTime()          -> 16/10 00:00:00.000 local (em UTC)
  Fim exclusivo    (<) : fim.Value.Date.AddDays(1).ToUniversalTime()  -> 17/10 00:00:00.000 local (em UTC)
  ```
  Inclui até `23:59:59.999999` do dia 16 e exclui exatamente `00:00:00.000` do dia 17!

---

## 2. Código Completo e Compilável desta Etapa (4 Arquivos)

### Passo 1 — `RelatorioAsync` em `IVendaRepository`, `VendaRepository` e `VendaService`

```csharp
// Em Repositories/IVendaRepository.cs:
Task<IReadOnlyList<ResumoVenda>> RelatorioAsync(DateTime inicioUtc, DateTime fimUtc);

// Em Repositories/VendaRepository.cs:
public async Task<IReadOnlyList<ResumoVenda>> RelatorioAsync(DateTime inicioUtc, DateTime fimUtc)
{
    await using var cmd = banco.CreateCommand("""
        SELECT v.id, v.criado_em, u.username, v.total
        FROM venda v
        JOIN usuario u ON u.id = v.usuario_id
        WHERE v.criado_em >= @inicio AND v.criado_em < @fim
        ORDER BY v.criado_em, v.id
        """);
    cmd.Parameters.AddWithValue("inicio", inicioUtc);
    cmd.Parameters.AddWithValue("fim", fimUtc);

    var resultado = new List<ResumoVenda>();
    await using var r = await cmd.ExecuteReaderAsync();
    while (await r.ReadAsync())
    {
        resultado.Add(new ResumoVenda(
            r.GetInt32(0),
            r.GetDateTime(1),
            r.GetString(2),
            r.GetDecimal(3)));
    }
    return resultado;
}

// Em Services/VendaService.cs:
public Task<IReadOnlyList<ResumoVenda>> RelatorioAsync(Sessao sessao, DateTime inicio, DateTime fim)
{
    Permissoes.ExigirAdministrador(sessao); // Caixa NÃO acessa relatório financeiro!
    if (inicio.Kind != DateTimeKind.Utc || fim.Kind != DateTimeKind.Utc || fim <= inicio)
        throw new ArgumentException("Informe um intervalo UTC válido.");
    return vendas.RelatorioAsync(inicio, fim);
}
```

### Passo 2 — `Views/FormRelatorio.cs` Completo (`DateTimePicker` + `DataGridView` + Projeção LINQ)

```csharp
using SupermercadoApp.Models;
using SupermercadoApp.Services;

namespace SupermercadoApp.Views;

public sealed class FormRelatorio : Form
{
    public FormRelatorio(Sessao sessao, VendaService vendas)
    {
        Permissoes.ExigirAdministrador(sessao);

        var p = Ui.Preparar(this, "Administração — Vendas por período");

        // Reaplicando DateTimePicker (Aula 10, Seção 2.8): Format.Short e .Value.Date!
        p.Controls.Add(Ui.Rotulo("Data inicial (horário local do Windows)"));
        var inicio = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today };
        p.Controls.Add(inicio);

        p.Controls.Add(Ui.Rotulo("Data final (inclusive)"));
        var fim = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today };
        p.Controls.Add(fim);

        var grade = Ui.Grade(p);
        var resumo = Ui.Rotulo("Nenhuma consulta realizada.");
        p.Controls.Add(resumo);

        Ui.Botao(p, "Consultar", async (sender, _) =>
        {
            var b = (Button)sender!;
            b.Enabled = false;
            try
            {
                await Ui.ExecutarAsync(async () =>
                {
                    var vendasPeriodo = await vendas.RelatorioAsync(
                        sessao,
                        inicio.Value.Date.ToUniversalTime(),
                        fim.Value.Date.AddDays(1).ToUniversalTime());

                    // Projeção LINQ convertendo UTC para horário local apenas na exibição:
                    grade.DataSource = vendasPeriodo
                        .Select(v => new
                        {
                            v.Id,
                            DataLocal = v.DataUtc.ToLocalTime(),
                            v.Operador,
                            v.Total
                        })
                        .ToList();

                    // Count = número de vendas (cupons), NÃO soma de itens!
                    resumo.Text = $"Vendas: {vendasPeriodo.Count} | Total: {vendasPeriodo.Sum(v => v.Total):C}";
                });
            }
            finally { b.Enabled = true; }
        });
    }
}
```

### Passo 3 — Conectar em `FormAdministrador.cs` e `Program.cs`
Passe `VendaService vendas` para `FormAdministrador` e adicione o botão `"Relatório de vendas"` abrindo `using var f = new FormRelatorio(sessao, vendas); f.ShowDialog(this);`.

---

## 3. Verificação de Fronteiras e Conferência SQL (`A23.01`–`A23.05`)

1. **Massa de fronteiras (`A23.02`):** com orientação do professor, aplique [database/laboratorio/18-fronteiras.sql](database/laboratorio/18-fronteiras.sql) (4 vendas ao redor de `16/10/2026` em `America/Sao_Paulo`). Consulte `16/10/2026` a `16/10/2026` no `FormRelatorio` → entram **exatamente 2 vendas** (soma `R$ 17,00` nessa massa isolada).
2. **Conferência SQL fixa:**
   ```sql
   SELECT count(*), coalesce(sum(total), 0)
   FROM venda
   WHERE criado_em >= (DATE '2026-10-16'::timestamp AT TIME ZONE 'America/Sao_Paulo')
     AND criado_em <  (DATE '2026-10-17'::timestamp AT TIME ZONE 'America/Sao_Paulo');
   ```
3. **Recusas, vazio e escala (`A23.03`–`A23.05`):** data final anterior à inicial (recusada), data futura vazia (`Vendas: 0 | Total: R$ 0,00`), sessão de `Caixa` (`UnauthorizedAccessException`) e registro da proposta de agregação SQL (`COUNT`/`SUM`) + paginação para grandes volumes.

[Anterior: Aula 22](22-Finalizacao-e-Troco.md) · [Próxima: Aula 24 — Cadastro de usuários](24-Cadastro-de-Usuarios.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 23 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-23). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
