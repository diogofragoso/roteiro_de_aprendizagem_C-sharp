# Aula 16 — Venda, transação e concorrência

**Duração:** 3h30. **Pré-requisito:** aula 15. **Entrega:** operação de venda no repositório que grava cabeçalho, itens e movimentos e baixa o estoque de modo atômico. O pagamento visual será ligado na aula 17.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Mostrar que o carrinho ainda não reserva estoque |
| Explicação | 30 | Transação, bloqueio e repetição segura |
| Construção | 60 | Script 003, modelos e serviço |
| Intervalo | 15 | Pausa |
| Prática | 60 | Repositório, chamada controlada e concorrência |
| Entrega | 30 | Demonstrar rollback e último item disputado |

## 1. Crie as tabelas e resultados

Use as quatro paradas de implementação: **esquema e pedido**, **leitura protegida**, **gravação e commit**, **ensaios de falha e repetição**. O professor prepara o ambiente conforme [roteiro de apoio](ensino/README.md). A versão 15 é o início, e a 16 fornece recuperação com botão de chamada controlada e sem os estados definitivos da aula 17.

Antes do código, desenhe duas linhas do tempo: chaves iguais aguardam no bloqueio da operação e retornam a mesma venda; chaves distintas disputando o último produto aguardam na linha, e apenas uma pode consumir o saldo. Marque leitura, bloqueios, escrita e commit.

Execute [003-vendas.sql](database/003-vendas.sql) uma vez, depois dos scripts 001 e 002. Leia cada tabela:

- `venda`: operação única (`uuid`), operador, instante, total e recebido.
- `venda_item`: produto, quantidade, preço e nome no momento da venda.

**Por quê copiar nome e preço no item?** Se amanhã o cadastro mudar, a venda antiga precisa continuar mostrando o que foi vendido. O histórico não deve recalcular uma compra antiga com o preço de hoje.

Crie em Models [PedidoItem.cs](src/SupermercadoApp/Models/PedidoItem.cs), [VendaConfirmada.cs](src/SupermercadoApp/Models/VendaConfirmada.cs) e [ResumoVenda.cs](src/SupermercadoApp/Models/ResumoVenda.cs). `ResumoVenda` será usado na aula 18; é apenas um resultado de consulta. Crie [IVendaRepository.cs](src/SupermercadoApp/Repositories/IVendaRepository.cs), inicialmente com `BuscarAsync` e `FinalizarAsync`, deixando `RelatorioAsync` para a aula 18.

## 2. Amplie o serviço

Substitua o serviço mínimo da aula 15 pelo [VendaService](src/SupermercadoApp/Services/VendaService.cs), sem `RelatorioAsync` ainda. Ele passa a receber `IVendaRepository` e `IProdutoRepository`; atualize sua criação em `Program`.

O serviço exige caixa, carrinho não vazio, quantidades positivas, produtos agrupados e valor recebido válido. Envia ao repositório somente **produto e quantidade**. O preço da tela não é autoridade: pode estar desatualizado ou ter sido alterado indevidamente.

## 3. Implemente em blocos pequenos

Crie [VendaRepository.cs](src/SupermercadoApp/Repositories/VendaRepository.cs), recebendo a fonte de dados. Implemente `BuscarAsync` e `FinalizarAsync` da referência, lendo as etapas na ordem:

1. **Abrir conexão e transação.** Todos os comandos da venda usam esses mesmos objetos.
2. **Bloquear a operação.** `pg_advisory_xact_lock` usa uma chave derivada do identificador da tentativa. Duas tentativas iguais aguardam uma à outra até a transação terminar.
3. **Consultar venda existente.** Se já foi confirmada para essa operação e usuário, retorna o resultado gravado. Isso é idempotência: repetir a mesma intenção não vende de novo.
4. **Bloquear produtos por ID crescente.** `SELECT ... FOR UPDATE` impede que outro caixa confirme uma alteração dessas linhas enquanto decidimos. A ordenação reduz disputas circulares de bloqueio.
5. **Ler e validar estado atual.** Produto precisa existir, estar ativo e ter saldo. Preço e nome são lidos do banco.
6. **Calcular total e validar recebido.** Se o preço atual não cabe no recebido, interrompe sem gravar. O preço final pode diferir do previsto no carrinho; a aula 17 explica isso ao operador.
7. **Inserir cabeçalho e obter ID.** Use `RETURNING id`; não faça `MAX(id)` nem suponha o próximo número.
8. **Inserir itens, diminuir estoque e registrar movimentos negativos.** Uma unidade vendida produz movimento `-1`.
9. **Confirmar uma única vez.** Só `CommitAsync` torna o conjunto definitivo. Exceção antes da confirmação provoca rollback pelo descarte da transação.

Os bloqueios e a restrição `estoque >= 0` cumprem funções complementares. A restrição impede saldo inválido no banco; o bloqueio permite decidir a venda com o estado protegido. Consulte os [bloqueios do PostgreSQL](https://www.postgresql.org/docs/current/explicit-locking.html).

## 4. Faça uma chamada controlada

No caixa, acrescente temporariamente um botão de teste que chama o serviço com `Guid.NewGuid()`, `carrinho.Itens` e um recebido conhecido, como 100,00. O valor é de pagamento simulado, não uma senha. Aguarde o resultado e mostre ID e total. Remova o botão no próximo encontro, substituindo-o pelo fluxo definitivo.

Prepare um produto novo de R$ 8,50 com estoque 1. Para testar concorrência, use duas instâncias conectadas ao mesmo banco, com o carrinho já preenchido em ambas, e finalize uma unidade em cada. Uma deve confirmar e a outra recusar estoque. Use contas de caixa separadas para representar a situação real.

Para verificar recusa antes da escrita, prepare dois produtos e reduza o saldo do segundo por outra venda antes de confirmar. Nenhum deles deve sofrer baixa na tentativa recusada. Esse caso não comprova rollback de alterações já feitas.

Para entregar A16.05, execute obrigatoriamente, com o professor, o caso [Rollback após alteração real](VALIDACAO-MANUAL.md#rollback-após-alteração-real). O gatilho provoca falha depois de cabeçalho, item e UPDATE de estoque. Anote contagens e saldo antes/depois; todos devem permanecer iguais. Remova o gatilho e repita a mesma tentativa para confirmar uma única vez. Se o ensaio não puder executar, registre pendência e recuperação, sem substituir o resultado por uma previsão.

## Verifique antes de encerrar

```sql
SELECT id, operacao, usuario_id, total, recebido FROM venda ORDER BY id;
SELECT venda_id, produto_id, quantidade, preco_unitario FROM venda_item ORDER BY venda_id;
SELECT produto_id, quantidade, motivo FROM movimento_estoque ORDER BY id;
```

Duas chamadas com **o mesmo Guid**, mesmos itens e mesmo recebido devem devolver o mesmo ID e gerar uma baixa. Cada venda nova usa outro Guid. Não reutilize a chave para um carrinho editado. Verifique também carrinho vazio e recebido insuficiente. Guarde evidências de contagens e saldos, sem inventar resultados quando a rede impedir o teste.

[Anterior: aula 15](15-Carrinho-do-Caixa.md) · [Próxima: aula 17](17-Finalizacao-e-Troco.md)

## Atividades por conceito

Realize as atividades da [aula 16 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-16). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
