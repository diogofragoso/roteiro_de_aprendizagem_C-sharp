# Aula 14 — Reposição e histórico de estoque

**Duração:** 3h30. **Pré-requisito:** aula 13; produto ativo cadastrado. **Entrega:** reposição feita por administrador, com saldo e movimento gravados na mesma transação.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Conferir saldo inicial e produto ativo |
| Explicação | 30 | Transação, incremento e trilha de estoque |
| Construção | 60 | Serviço e repositório de reposição |
| Intervalo | 15 | Pausa |
| Prática | 60 | Controles, integração e conferência SQL |
| Entrega | 30 | Validar saldo, movimento e acesso negado |

## 1. Descubra por que precisamos de uma transação

Se atualizarmos o saldo e a internet cair antes de gravar o movimento, teremos quantidade sem explicação. Se fizermos o inverso, teremos explicação sem saldo. A transação torna as duas gravações uma unidade: **ambas confirmam ou nenhuma permanece**.

No `IProdutoRepository`, acrescente `Task ReporAsync(int id, int quantidade, int usuarioId);`. No `ProdutoService`, acrescente o `ReporAsync` da referência: exige administrador e quantidade positiva e usa `sessao.Id` como responsável. O operador não pode digitar “quem fez”.

## 2. Implemente a operação no repositório

Copie `ReporAsync` de [ProdutoRepository.cs](src/SupermercadoApp/Repositories/ProdutoRepository.cs). Identifique as etapas:

1. Abre uma conexão e inicia uma transação.
2. Executa `UPDATE produto SET estoque=estoque+@q WHERE id=@id AND ativo`.
3. Exige uma linha alterada; produto ausente/inativo impede a continuação.
4. Insere `movimento_estoque` com quantidade positiva e motivo `Reposicao`.
5. Executa `CommitAsync` somente após as duas gravações.

Todos os comandos recebem **a mesma conexão e a mesma transação**. A disposição com `await using` desfaz o que não foi confirmado se uma exceção interromper o método. O incremento ocorre no SQL; não leia o saldo, some em C# e grave o número, pois duas reposições poderiam sobrescrever uma à outra.

## 3. Adicione a ação à janela

Complete o bloco de reposição de [FormProdutos.cs](src/SupermercadoApp/Views/FormProdutos.cs): `NumericUpDown` com mínimo 1 e botão “Registrar reposição”. O produto vem da linha selecionada. Desabilite o botão enquanto aguarda e, após sucesso, recarregue a grade.

Reposição é uma ação separada do botão Salvar. O cadastro altera descrição/preço; reposição registra entrada física. Nunca permita saldo negativo como forma de “corrigir” entrada. Ajustes com motivo, inventário e estorno são funcionalidades posteriores que exigem regras próprias.

## 4. Confira os efeitos

Cadastre um produto novo, reponha 10 unidades e consulte:

```sql
SELECT id, codigo, estoque FROM produto ORDER BY id;
SELECT produto_id, usuario_id, quantidade, motivo, criado_em
FROM movimento_estoque ORDER BY id;
```

O saldo deve ser 10 e deve existir um movimento `+10`. Reponha 3: saldo 13 e segundo movimento `+3`. Nas duas linhas, o usuário deve corresponder ao administrador conectado.

## Verifique antes de encerrar

- Quantidade zero/negativa é recusada pelo serviço, mesmo se chamada sem tela.
- Produto inativo não recebe reposição.
- Caixa não consegue chamar o serviço de reposição.
- A soma dos movimentos é igual ao saldo para o produto novo usado no teste.
- Não apareceram duas reposições por um único clique.

Na venda, os movimentos terão quantidade negativa. Usaremos a mesma ideia de transação para incluir cabeçalho, itens e baixa de estoque juntos.

[Anterior: aula 13](13-Cadastro-de-Produtos.md) · [Próxima: aula 15](15-Carrinho-do-Caixa.md)
