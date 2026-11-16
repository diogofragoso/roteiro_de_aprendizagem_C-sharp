# Aula 18 — Relatório de vendas por período

**Duração:** 3h30. **Pré-requisito:** aula 17, com ao menos duas vendas confirmadas. **Entrega:** administrador consulta vendas por datas e vê quantidade e soma dos totais.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Conferir vendas conhecidas no banco |
| Explicação | 30 | Instante UTC, data local e intervalo semiaberto |
| Construção | 60 | Consulta, serviço e filtros |
| Intervalo | 15 | Pausa |
| Prática | 60 | Grade, agregação e comparação SQL |
| Entrega | 30 | Validar limites do período e perfil |

## 1. Declare a consulta

Acrescente a assinatura `RelatorioAsync` ao [IVendaRepository](src/SupermercadoApp/Repositories/IVendaRepository.cs) e o método correspondente ao [VendaRepository](src/SupermercadoApp/Repositories/VendaRepository.cs). Acrescente também o método do [VendaService](src/SupermercadoApp/Services/VendaService.cs), exigindo administrador e intervalo válido.

```sql
WHERE v.criado_em >= @inicio AND v.criado_em < @fim
```

**Por quê não comparar com “23:59:59”?** Um instante pode ter frações de segundo. É mais seguro incluir o início e excluir o começo do dia seguinte. `timestamptz` representa instantes; a interface apresenta no horário local.

## 2. Crie a janela

Crie [FormRelatorio.cs](src/SupermercadoApp/Views/FormRelatorio.cs) com o conteúdo de referência. Ele usa dois `DateTimePicker`, grade, rótulo de resumo e botão Consultar. Não permite edição da grade.

A data final escolhida é inclusiva para a pessoa. Antes de consultar:

```csharp
inicio.Value.Date.ToUniversalTime()
fim.Value.Date.AddDays(1).ToUniversalTime()
```

Configure o Windows do laboratório para o fuso correto antes da prática, como Brasília para esta turma. Formatar moeda em `pt-BR` não altera o fuso. Npgsql recebe os parâmetros com `DateTimeKind.Utc`; converter apenas a apresentação de texto não basta.

Os resultados são convertidos com `ToLocalTime()` para a grade. O total exibido é `vendasPeriodo.Sum(v => v.Total)` e a quantidade é o número de vendas retornadas, não a quantidade de produtos.

## 3. Conecte à administração

Passe `VendaService` também para `FormAdministrador` e acrescente o botão “Relatório de vendas”, abrindo `FormRelatorio` com `using` e `ShowDialog(this)`. Caixa não recebe esse botão e o serviço verifica novamente o perfil.

A consulta não tem paginação neste recorte e materializa o período em memória. Use um intervalo curto de laboratório. Para uma base grande, implemente agregação no SQL e paginação de detalhes antes de usar em produção.

## 4. Compare com um resultado conhecido

Para verificar fronteiras, use [18-fronteiras.sql](database/laboratorio/18-fronteiras.sql) no ambiente preparado pelo professor, com um Caixa existente e scripts 001–003. A massa contém quatro instantes: imediatamente antes de 16/10/2026, início desse dia, seu último microssegundo e início do dia seguinte, no fuso America/Sao_Paulo. O filtro de 16/10 inclui somente os dois registros do meio: duas vendas e R$ 17,00 nessa massa. Há consultas de conferência sem depender de `current_date`; outras vendas no mesmo período precisam ser consideradas na comparação com a UI.

Anote duas vendas, como R$ 60,30 e R$ 8,50, realizadas hoje. Consulte somente hoje. Se não houver outras vendas no ambiente, espere quantidade 2 e soma **R$ 68,80**.

No SQL Editor, usando o fuso local explicitamente, confira:

```sql
SELECT count(*), coalesce(sum(total), 0)
FROM venda
WHERE criado_em >= (current_date::timestamp AT TIME ZONE 'America/Sao_Paulo')
  AND criado_em < ((current_date + 1)::timestamp AT TIME ZONE 'America/Sao_Paulo');
```

`current_date` depende do fuso da sessão do SQL Editor. Para datas reproduzíveis, substitua por um literal como `DATE '2026-10-16'` em ambos os limites; não confunda a data do computador com a configuração da sessão do banco.

## Verifique antes de encerrar

- Data final anterior à inicial é recusada.
- Um período vazio mostra zero vendas e total zero.
- A data final inclui o dia inteiro e não inclui o dia seguinte.
- Soma e contagem conferem com os registros conhecidos.
- Caixa não consegue executar o método do serviço.

[Anterior: aula 17](17-Finalizacao-e-Troco.md) · [Próxima: aula 19](19-Cadastro-de-Usuarios.md)

## Atividades por conceito

Realize as atividades da [aula 18 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-18). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
