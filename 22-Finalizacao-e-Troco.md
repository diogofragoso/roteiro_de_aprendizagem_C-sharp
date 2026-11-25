# Aula 22 — Finalização, Cálculo de Troco, Máquina de Estados e Reconciliação no Caixa

**Duração:** 3h30. **Pré-requisito:** [Aula 21](21-Venda-com-Transacao.md). **Entrega:** tela definitiva `FormCaixa` com máquina de estados completa, pagamento em dinheiro simulado, comprovante didático (*sem valor fiscal*), cálculo de troco, proteção de fechamento (`FormClosing`) e reconciliação de tentativa pendente (`A22.01`–`A22.06`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Conferir venda gravada e movimentos negativos da Aula 21 |
| Explicação | 30 | Máquina de estados da UI, erro antes do commit vs resultado incerto pós-commit |
| Construção | 60 | Construir a versão definitiva de `Views/FormCaixa.cs` (painel `edicao` + pagamento) |
| Intervalo | 15 | Pausa |
| Prática | 60 | Implementar `FormClosing`, testar troco/insuficiência e executar `RespostaPerdidaFake` |
| Entrega | 30 | Demonstrar todos os estados (`A22.01`–`A22.06`) e registrar limites honestos |

---

## 1. Máquina de Estados: Erro Antes do Commit vs Resultado Incerto Pós-Commit (`A22.01`)

Se o banco confirma a venda (`COMMIT`), mas a rede cai 1 ms antes da resposta voltar para a tela:
- **Falha conhecida (`ArgumentException`, `InvalidOperationException`, `UnauthorizedAccessException`):** aconteceu **antes do commit** → definimos `pendente = false;` e destravamos `edicao` para o operador corrigir o carrinho ou o valor recebido.
- **Qualquer outra `Exception` (rede/timeout):** **não sabemos** se o commit aconteceu → mantemos **`pendente = true;`**, preservando o **mesmo `Guid operacao`**, os **mesmos itens** e **`recebidoPendente`**, mantendo `edicao.Enabled = false;` (contêiner `FlowLayoutPanel` travado em lote — [Aula 10, Seção 2.10](10-Componentes-Windows-Forms.md)!) até o operador clicar novamente em **Finalizar / consultar tentativa pendente**!

| Estado da Tela | Painel `edicao` | Botão Finalizar | Próxima transição |
| --- | --- | --- | --- |
| **Em montagem** | Habilitado | Habilitado | Congela `recebidoPendente` e marca `pendente = true` |
| **Aguardando banco** (`executando = true`) | Bloqueado | Desabilitado | Aguarda `await` sem permitir duplo clique |
| **Confirmada** | Liberado | Habilitado | Exibe comprovante, limpa carrinho e gera **novo `Guid`** |
| **Recusada antes do commit** | Liberado (`pendente = false`) | Habilitado | Operador corrige dados e tenta de novo |
| **Comunicação incerta** (`pendente = true`) | **Bloqueado** (`!pendente`) | Habilitado | Reenvia mesmo `Guid`, itens e `recebidoPendente` |

> [!WARNING]
> **Limite honesto (`A22.05`):** a tentativa pendente fica na memória RAM enquanto a janela está aberta. Se faltar energia ou encerrarem o processo à força, consulte o `Guid` anotado no banco (`SELECT id, total, recebido FROM venda WHERE operacao = 'GUID_ANOTADO';`) antes de reiniciar a venda.

---

## 2. Código Completo e Compilável de `Views/FormCaixa.cs` (Versão Definitiva)

```csharp
using SupermercadoApp.Models;
using SupermercadoApp.Services;

namespace SupermercadoApp.Views;

public sealed class FormCaixa : Form
{
    private readonly Carrinho carrinho = new();
    private Guid operacao = Guid.NewGuid();
    private bool pendente;
    private bool executando;
    private decimal recebidoPendente;

    public FormCaixa(Sessao sessao, VendaService vendas)
    {
        Permissoes.ExigirCaixa(sessao);

        var p = Ui.Preparar(this, $"MarketFlow — Caixa: {sessao.Username}", 960, 850);

        // Contêiner de edição separado: 1 linha 'edicao.Enabled = false' trava tudo em lote!
        var edicao = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false };
        p.Controls.Add(edicao);

        var codigo = Ui.Texto(edicao, "Código do produto");
        edicao.Controls.Add(Ui.Rotulo("Quantidade (apenas unidades inteiras neste curso)"));
        var quantidade = new NumericUpDown { Minimum = 1, Maximum = 1000000, Value = 1 };
        edicao.Controls.Add(quantidade);

        var grade = Ui.Grade(edicao);
        var total = Ui.Rotulo("Total previsto: R$ 0,00");
        edicao.Controls.Add(total);

        void Atualizar()
        {
            grade.DataSource = carrinho.Itens.ToList();
            total.Text = $"Total previsto: {carrinho.Total:C}";
        }

        Ui.Botao(edicao, "Adicionar", async (sender, _) =>
        {
            var b = (Button)sender!;
            b.Enabled = false; executando = true; edicao.Enabled = false;
            try
            {
                await Ui.ExecutarAsync(async () =>
                {
                    var produto = await vendas.BuscarProdutoAsync(sessao, codigo.Text)
                        ?? throw new InvalidOperationException("Produto não encontrado.");
                    carrinho.Adicionar(produto, (int)quantidade.Value);
                    Atualizar(); codigo.Clear(); codigo.Focus();
                });
            }
            finally { executando = false; b.Enabled = true; edicao.Enabled = true; }
        });

        Ui.Botao(edicao, "Remover selecionado", (_, _) =>
        {
            if (grade.SelectedRows.Count > 0 && grade.SelectedRows[0].DataBoundItem is ItemCarrinho item)
                carrinho.Remover(item.ProdutoId);
            Atualizar();
        });

        Ui.Botao(edicao, "Cancelar carrinho", (_, _) =>
        {
            carrinho.Limpar();
            operacao = Guid.NewGuid();
            Atualizar();
        });

        edicao.Controls.Add(Ui.Rotulo("Valor recebido em dinheiro (pagamento simulado)"));
        var recebido = new NumericUpDown { DecimalPlaces = 2, Maximum = 9999999999.99m, Width = 200 };
        edicao.Controls.Add(recebido);

        // Controles FORA de 'edicao' (permanecem acessíveis para reconciliar se a rede cair!):
        var estado = Ui.Rotulo("Adicione produtos para iniciar.");
        p.Controls.Add(estado);

        Ui.Botao(p, "Finalizar / consultar tentativa pendente", async (sender, _) =>
        {
            if (executando) { MessageBox.Show("Aguarde a consulta de produto terminar."); return; }
            if (carrinho.Itens.Count == 0) { MessageBox.Show("Carrinho vazio."); return; }

            var b = (Button)sender!;
            b.Enabled = false; edicao.Enabled = false; executando = true;
            if (!pendente) recebidoPendente = recebido.Value;
            pendente = true;

            try
            {
                var resultado = await vendas.FinalizarAsync(sessao, operacao, carrinho.Itens, recebidoPendente);
                MessageBox.Show(
                    $"COMPROVANTE DIDÁTICO — SEM VALOR FISCAL\n" +
                    $"Venda: {resultado.Id}\n" +
                    $"Total: {resultado.Total:C}\n" +
                    $"Recebido: {resultado.Recebido:C}\n" +
                    $"Troco: {resultado.Troco:C}",
                    "Venda confirmada");

                carrinho.Limpar();
                operacao = Guid.NewGuid(); // Só renova a chave Guid APÓS confirmação!
                recebido.Value = 0;
                pendente = false;
                Atualizar();
                estado.Text = "Venda confirmada.";
            }
            catch (Exception e) when (e is ArgumentException or InvalidOperationException or UnauthorizedAccessException)
            {
                // Falha conhecida ANTES do commit -> libera edição para correção:
                pendente = false;
                MessageBox.Show(e.Message, "Verifique os dados");
            }
            catch (Exception e)
            {
                // Resultado INCERTO (rede/timeout) -> mantém pendente = true e mesmo Guid!
                System.Diagnostics.Trace.WriteLine($"Falha de venda: {e.GetType().Name}");
                estado.Text = $"Resultado incerto. Operação: {operacao}. Clique em finalizar novamente.";
                MessageBox.Show("Mantenha esta tela aberta. A próxima tentativa consulta a mesma operação sem duplicar.");
            }
            finally
            {
                executando = false;
                b.Enabled = true;
                edicao.Enabled = !pendente; // Só destrava se NÃO estiver em resultado incerto!
            }
        });

        Ui.Botao(p, "Sair / trocar usuário", (_, _) => Close());

        // Proteção de fechamento da Aula 10 (Seção 2.1):
        FormClosing += (_, e) =>
        {
            if (pendente || executando)
            {
                e.Cancel = true;
                MessageBox.Show("Resolva a tentativa pendente antes de sair. Anote o identificador se precisar de suporte.");
            }
            else if (carrinho.Itens.Count > 0 &&
                     MessageBox.Show("Descartar carrinho e sair?", "Confirmar",
                         MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                e.Cancel = true;
            }
        };
    }
}
```

---

## 3. Exemplo Prático Completo — Simulação de Resposta Perdida Pós-Commit (`A22.04`)

Para comprovar em memória como o mesmo `Guid` evita duplicidade quando a resposta se perde após a gravação, estude e execute este dublê de teste *(lembre-se de registrá-lo separadamente como simulação em memória, não como queda real no PostgreSQL)*:

```csharp
public sealed class RespostaPerdidaFake : IVendaRepository
{
    private readonly Dictionary<Guid, VendaConfirmada> gravadas = [];
    public int GravacoesReais { get; private set; }

    public Task<VendaConfirmada?> BuscarAsync(Guid operacao) =>
        Task.FromResult(gravadas.GetValueOrDefault(operacao));

    public Task<VendaConfirmada> FinalizarAsync(Guid operacao, int usuarioId,
        IReadOnlyList<PedidoItem> itens, decimal recebido)
    {
        // Se já foi gravada antes da queda de rede, devolve a venda existente!
        if (gravadas.TryGetValue(operacao, out var existente))
            return Task.FromResult(existente);

        // Na 1ª chamada: grava a venda, mas simula queda de rede ANTES da resposta voltar!
        GravacoesReais++;
        gravadas[operacao] = new VendaConfirmada(GravacoesReais, 60.30m, recebido);
        throw new IOException("Conexão perdida logo após o COMMIT!");
    }
}
```

---

## 4. Roteiro de Verificação (`A22.01`–`A22.06`)

1. **Troco exato (`A22.02` e `A22.06`):** venda `R$ 60,30` com recebido `R$ 100,00` → comprovante não fiscal mostra total `R$ 60,30`, recebido `R$ 100,00` e troco **`R$ 39,70`**.
2. **Recebido insuficiente e mudança de preço:** venda `R$ 60,30` com recebido `R$ 50,00` → recusa preservando carrinho e sem baixa no banco; teste também alterar preço como Admin antes de finalizar.
3. **`FormClosing` (`A22.05`):** tente fechar pelo `X` com carrinho cheio (pede confirmação) e durante espera/incerteza (bloqueia com `e.Cancel = true`).
4. **Simulação de resposta perdida (`A22.04`):** chame `RespostaPerdidaFake` duas vezes com o mesmo `Guid` → 1ª lança `IOException`, 2ª retorna ID `1` com `GravacoesReais == 1`!

[Anterior: Aula 21](21-Venda-com-Transacao.md) · [Próxima: Aula 23 — Relatório de vendas](23-Relatorio-de-Vendas.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 22 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-22). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
