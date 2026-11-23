# Aula 17 — Finalização, Cálculo de Troco e Recuperação de Tentativa Pendente

**Duração:** 3h30. **Pré-requisito:** [Aula 16](16-Venda-com-Transacao.md). **Entrega:** tela `FormCaixa` definitiva com máquina de estados completa, recebimento em dinheiro simulado, comprovante didático (*sem valor fiscal*), cálculo de troco, proteção de fechamento (`FormClosing`) e reconciliação segura quando a comunicação falha.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Conferir venda gravada e movimentos negativos da Aula 16 |
| Explicação | 30 | Máquina de estados da UI, erro antes do commit vs resultado incerto pós-commit |
| Construção | 60 | Construir controles de pagamento, estados de bloqueio e comprovante em `FormCaixa` |
| Intervalo | 15 | Pausa |
| Prática | 60 | Implementar reconciliação de tentativa pendente, `FormClosing` e simulação de falha |
| Entrega | 30 | Demonstrar troco, insuficiência, duplo clique e reconciliação (`A17.01`–`A17.06`) |

---

## 1. O Problema Mais Difícil de Sistemas em Rede: Resultado Incerto Pós-Commit

Imagine o operador de caixa clicando em **Finalizar**. O aplicativo envia a venda pela internet ao Neon; o Neon grava a venda e executa `COMMIT`, mas **1 milissegundo antes da resposta voltar**, o cabo de rede desconecta!

O que muitos aplicativos mal projetados fazem nesse momento?
- ❌ Mostram *"Venda cancelada!"*, limpam o carrinho ou geram um novo `Guid` → o caixa passa a compra de novo e o cliente é cobrado **duas vezes** e o estoque baixa **em dobro**!

**Como construímos uma interface profissionalmente resiliente (`A17.01` e `A17.04`)?**
Separando dois tipos completamente diferentes de exceção:

1. **Falhas de regra conhecidas (`ArgumentException`, `InvalidOperationException`, `UnauthorizedAccessException`):**
   Sabemos com **100% de certeza** que aconteceram **antes do `COMMIT`** (ex.: valor recebido menor que o total, ou estoque acabou). Portanto: `pendente = false;`, liberamos a edição para o caixa corrigir o valor recebido ou remover o item!
2. **Qualquer outra `Exception` (falha de rede, timeout, conexão encerrada):**
   **Não sabemos** se o banco chegou a confirmar! Portanto:
   - Mantemos **`pendente = true;`**, preservando o **mesmo `Guid operacao`**, os **mesmos itens** e o **mesmo `recebidoPendente`**!
   - Mantemos o painel de edição **bloqueado** (`edicao.Enabled = false;`), impedindo o usuário de alterar itens de uma intenção ainda pendente.
   - Mostramos o `Guid` na tela e orientamos clicar em **"Finalizar / consultar tentativa pendente"** assim que a rede voltar: o `VendaRepository` (Aula 16) consultará pelo `Guid` e devolverá a venda já confirmada ou a gravará pela primeira vez, **nunca duplicando**!

### Máquina de Estados da Tela `FormCaixa`

| Estado da Tela | Campos de Edição (`edicao`) | Botão Finalizar | O que acontece após a resposta |
| --- | --- | --- | --- |
| **Em montagem** | Habilitados | Habilitado (se houver itens) | Gera/mantém `Guid` e inicia tentativa |
| **Aguardando banco** (`executando = true`) | Bloqueados | Desabilitado | Aguarda retorno assíncrono sem duplo clique |
| **Confirmada** | Liberados para nova compra | Habilitado | Mostra comprovante, limpa carrinho, zera recebido e gera **novo `Guid`** |
| **Recusada antes do commit** | Liberados (`pendente = false`) | Habilitado | Operador corrige recebido ou carrinho mantendo o fluxo |
| **Comunicação incerta** (`pendente = true`) | **Bloqueados** (`!pendente`) | Habilitado para reconciliar | Reenvia exatamente o mesmo `Guid`, itens e `recebidoPendente` |

> [!WARNING]
> **Limite honesto do recorte em memória (`A17.05`):** a tentativa pendente vive na memória RAM enquanto a janela permanece aberta. Se faltar energia elétrica no computador ou alguém encerrar o processo à força no Gerenciador de Tarefas, a recuperação automática não existe neste recorte local — por isso exibimos o `Guid` na tela para consulta manual no banco (`SELECT id, total, recebido FROM venda WHERE operacao = 'GUID_ANOTADO';`) antes de iniciar nova venda. Em sistemas comerciais, isso evolui para uma fila persistente em disco local (SQLite/outbox) + API no servidor.

---

## 2. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

Complete **`Views/FormCaixa.cs`** substituindo o botão temporário da Aula 16 pela máquina de estados definitiva:

### Passo 1 — Campos de Estado na Classe `FormCaixa`
Declare exatamente estes campos privados na classe:
- `private readonly Carrinho carrinho = new();`
- `private Guid operacao = Guid.NewGuid();`
- `private bool pendente;`
- `private bool executando;`
- `private decimal recebidoPendente;`

### Passo 2 — Separar Painel Editável (`edicao`) dos Controles de Finalização
**Por que colocar busca, quantidade, grade, remoção, cancelamento e valor recebido dentro de um `FlowLayoutPanel edicao` separado?**
Porque assim, com uma única linha (`edicao.Enabled = false;` ou `edicao.Enabled = !pendente;`), você trava ou libera **todos** os controles de edição de uma só vez sem esquecer nenhum botão solto!

1. Crie `var edicao = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false };` e adicione ao painel principal `p`.
2. Mova os controles do carrinho (código, quantidade, grade, rótulo total, botões Adicionar, Remover e Cancelar) para dentro de `edicao`.
   - No botão **Adicionar**, atualize também `executando = true; edicao.Enabled = false;` antes da consulta e restaure no `finally` (impedindo finalizar enquanto uma busca de produto ainda está em andamento!).
   - No botão **Cancelar carrinho**, além de `carrinho.Limpar()`, renove `operacao = Guid.NewGuid();`.
3. Ainda dentro de `edicao`, adicione:
   - Rótulo `"Valor recebido em dinheiro (pagamento simulado)"`;
   - Controle `NumericUpDown recebido` com `DecimalPlaces = 2`, `Maximum = 9999999999.99m`, `Width = 200`.
4. **Fora** de `edicao` (diretamente no painel principal `p`, para continuar acessível mesmo quando `edicao` estiver travado!):
   - Crie um rótulo de status: `var estado = Ui.Rotulo("Adicione produtos para iniciar.");`.
   - Crie o botão **`"Finalizar / consultar tentativa pendente"`**.

### Passo 3 — Construir o Evento do Botão Finalizar (`A17.02` e `A17.03`)
No evento `async (sender, _) =>` do botão Finalizar, construa exatamente esta sequência:
1. Se `executando` for `true`: avise para aguardar a consulta terminar e dê `return;`.
2. Se `carrinho.Itens.Count == 0`: avise `"Carrinho vazio."` e dê `return;`.
3. Trave a interface: `var b = (Button)sender!; b.Enabled = false; edicao.Enabled = false; executando = true;`.
4. Se **não** estava pendente (`if (!pendente)`), congele o valor recebido em `recebidoPendente = recebido.Value;` e marque `pendente = true;`.
5. No bloco **`try`**:
   - Chame `var resultado = await vendas.FinalizarAsync(sessao, operacao, carrinho.Itens, recebidoPendente);`.
   - Exiba o comprovante com `MessageBox.Show`: cabeçalho **"COMPROVANTE DIDÁTICO — SEM VALOR FISCAL"**, número `resultado.Id`, `resultado.Total:C`, `resultado.Recebido:C` e `resultado.Troco:C` *(usando o total definitivo devolvido pelo banco!)*.
   - Somente **após** a confirmação: limpe o carrinho (`carrinho.Limpar()`), gere um **novo** `operacao = Guid.NewGuid()`, zere `recebido.Value = 0`, marque `pendente = false`, chame `Atualizar()` e atualize `estado.Text = "Venda confirmada.";`.
6. No primeiro **`catch`** (erros conhecidos antes do commit):
   - `catch (Exception e) when (e is ArgumentException or InvalidOperationException or UnauthorizedAccessException)`
   - Defina `pendente = false;` e exiba `MessageBox.Show(e.Message);`.
7. No segundo **`catch`** (erro de rede / comunicação incerta):
   - `catch (Exception e)`
   - Registre apenas o nome do tipo de exceção em `Trace.WriteLine`.
   - Atualize `estado.Text` exibindo o `Guid` `operacao` e peça para clicar em finalizar novamente (mantendo `pendente = true`!).
8. No **`finally`**:
   - `executando = false; b.Enabled = true; edicao.Enabled = !pendente;`. *(Observe a elegância de `edicao.Enabled = !pendente;`: só destrava a edição se a operação não estiver em estado incerto!)*

### Passo 4 — Proteger o Fechamento da Janela (`FormClosing`) (`A17.05`)
Inscreva o evento `FormClosing += (_, e) =>` no construtor de `FormCaixa`:
- Se `pendente || executando`: defina **`e.Cancel = true;`** (impede a janela de fechar!) e oriente a resolver a tentativa pendente ou anotar o identificador `Guid`.
- Senão, se `carrinho.Itens.Count > 0`: pergunte com `MessageBoxButtons.YesNo` se deseja descartar o carrinho e sair; se a resposta for diferente de `DialogResult.Yes`, defina `e.Cancel = true;`.

---

## 3. Verificação Completa e Simulação de Resposta Perdida (`A17.01`–`A17.06`)

Execute com **`F5`** no Visual Studio 2022 e verifique todos os casos:
1. **Troco exato:** compra de `R$ 60,30` (2 arrozes + 1 feijão) com recebido `R$ 100,00` → comprovante não fiscal mostra total `R$ 60,30`, recebido `R$ 100,00` e troco **`R$ 39,70`**; carrinho limpa e gera novo `Guid`.
2. **Recebido insuficiente:** compra de `R$ 60,30` com recebido `R$ 50,00` → recusa, carrinho preservado, edição liberada, nenhuma baixa no banco.
3. **Mudança de preço concorrente:** adicione um produto ao carrinho, altere o preço dele como Administrador em outra janela e finalize → o total definitivo vem do banco (e recusa se o recebido ficou insuficiente).
4. **`FormClosing`:** tente fechar pelo `X` com itens no carrinho (deve pedir confirmação); durante espera/incerteza, o fechamento é bloqueado.
5. **Simulação de resposta perdida (`A17.04`):** construa ou estude um dublê de teste (`RespostaPerdidaFake`) que registra a venda em memória mas lança `IOException` no primeiro retorno; ao chamar uma segunda vez com o mesmo `Guid`, devolve a venda já gravada sem criar uma segunda gravação! *(Registre separadamente como simulação em memória — não confunda com queda real de rede após commit no PostgreSQL.)*

[Anterior: Aula 16](16-Venda-com-Transacao.md) · [Próxima: Aula 18 — Relatório de vendas](18-Relatorio-de-Vendas.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 17 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-17). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
