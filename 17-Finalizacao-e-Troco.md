# Aula 17 — Finalização, troco e tentativa pendente

**Duração:** 3h30. **Pré-requisito:** aula 16. **Entrega:** venda em dinheiro simulado com confirmação, troco e repetição segura quando a comunicação falha.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Conferir venda gravada e movimentos |
| Explicação | 30 | Estados de tela e resultado de confirmação incerto |
| Construção | 60 | Recebido, finalizar e comprovante didático |
| Intervalo | 15 | Pausa |
| Prática | 60 | Falhas, repetição e fechamento da janela |
| Entrega | 30 | Demonstrar sucesso, insuficiência e não duplicação |

## 1. Acrescente o pagamento à janela

Complete [FormCaixa.cs](src/SupermercadoApp/Views/FormCaixa.cs). Remova o botão temporário da aula 16. Acrescente o valor recebido com `NumericUpDown`, duas casas decimais, e o botão “Finalizar / consultar tentativa pendente”.

No início de uma compra, gere `Guid.NewGuid()` e guarde esse valor no campo `operacao`. O Guid identifica a intenção de vender; não é o ID sequencial da venda. O ID só será conhecido depois da gravação.

O exemplo aceita apenas dinheiro simulado. Não mostra QR Code, não se comunica com cartão e não envia cobrança. O comprovante deve dizer **SEM VALOR FISCAL**.

## 2. Entenda os estados antes de copiar o evento

| Estado | Edição do carrinho | Finalizar | Ação após resposta |
| --- | --- | --- | --- |
| Em montagem | Permitida | Permitido com itens | Iniciar tentativa |
| Aguardando banco | Bloqueada | Desabilitado | Aguardar resultado |
| Confirmada | Nova compra liberada | Liberado | Limpar e gerar novo Guid |
| Recusada antes do commit | Liberada | Liberado | Corrigir os dados |
| Comunicação incerta | Bloqueada | Repetir mesma tentativa | Consultar/confirmar a mesma operação |

A tela guarda o valor recebido no começo da tentativa. Não deixe o usuário trocar itens ou recebido enquanto a mesma chave está pendente. O repositório só consegue reconhecer a repetição porque a chave continua igual.

## 3. Ligue confirmação e apresentação

Leia o evento completo da referência. Ele desabilita edição e botão, chama `VendaService.FinalizarAsync` e só depois do retorno confirmado:

1. Mostra número da venda, total definitivo, recebido e troco.
2. Limpa o carrinho e o recebido.
3. Gera um novo Guid.
4. Libera uma nova compra.

O troco é `Recebido - Total`, usando o total definitivo do banco. O preço do carrinho é uma previsão: se o administrador alterar o preço antes da confirmação, o banco recalcula. Nesse recorte didático a tela mostra o total definitivo no comprovante; se o recebido ficar insuficiente, recusa e permite corrigir. Uma evolução possível é pedir nova confirmação explícita do operador quando o preço mudar.

Configure `FormClosing` como na referência: não permitir fechar enquanto executa ou há resultado incerto; se existe apenas um carrinho em montagem, perguntar se deve descartá-lo.

## 4. Trate uma queda de comunicação com cuidado

Uma exceção de rede pode acontecer **depois de o banco confirmar**, antes de a resposta chegar. Nesse caso não diga “venda cancelada” e não gere outra chave. Mantenha o Guid e peça para repetir a consulta/finalização. O repositório retorna a venda existente ou conclui a mesma operação quando ainda não existe.

O exemplo mantém a tentativa **em memória**. Se o processo for encerrado à força ou a máquina desligar, não há recuperação automática do carrinho. Antes de reiniciar uma operação, o professor deve consultar o identificador anotado na tela:

```sql
SELECT id, total, recebido FROM venda WHERE operacao = 'GUID_ANOTADO';
```

Não reabra a compra com um novo Guid sem esclarecer o resultado anterior. Para operação comercial, será necessário um registro local durável de tentativas e reconciliação no servidor; o bloqueio de fechamento não substitui essa solução. Esse limite é parte da aprendizagem sobre falhas, não uma promessa de funcionamento offline.

## Verifique antes de encerrar

- Compra de R$ 60,30 com recebido R$ 100,00: troco R$ 39,70.
- Recebido menor que total: nenhuma venda ou baixa; carrinho preservado.
- Finalizar sem itens: recusado antes do banco.
- Clique duplo não produz dois envios normais; mesma operação repetida não produz nova venda.
- Falha de rede não limpa carrinho nem permite trocar a intenção pendente.
- Sair após uma venda confirmada volta ao login.

[Anterior: aula 16](16-Venda-com-Transacao.md) · [Próxima: aula 18](18-Relatorio-de-Vendas.md)
