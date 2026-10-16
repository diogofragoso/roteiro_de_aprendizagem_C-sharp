# Aula 15 — Carrinho do operador de caixa

**Duração:** 3h30. **Pré-requisito:** aula 14; produto ativo com estoque e conta de caixa de teste. **Entrega:** carrinho que agrupa produtos, calcula total e permite remoção, ainda sem gravar vendas.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Entrar como caixa e conferir estoque |
| Explicação | 30 | Coleção, subtotal e estado temporário |
| Construção | 60 | Implementar ItemCarrinho e Carrinho |
| Intervalo | 15 | Pausa |
| Prática | 60 | Buscar produto, montar grade e remover itens |
| Entrega | 30 | Conferir contas e cancelamento sem efeito no banco |

## 1. Modele o item e a coleção

Crie [ItemCarrinho.cs](src/SupermercadoApp/Models/ItemCarrinho.cs) e [Carrinho.cs](src/SupermercadoApp/Services/Carrinho.cs). O item guarda o ID, nome, preço mostrado e quantidade. `Subtotal` é calculado; não precisa ser digitado ou salvo separadamente.

A inicialização `List<ItemCarrinho> itens = [];` usa uma expressão de coleção do C# 12: nesse contexto equivale a criar uma lista vazia com `new List<ItemCarrinho>()`.

O carrinho mantém uma lista privada. `Itens` oferece leitura, `Adicionar` agrupa pelo ID e `Remover` retira um produto. A expressão `with { Quantidade = acumulada }` cria uma cópia do record com outra quantidade. `checked` evita que uma soma inteira excessiva dê a volta e vire um número incorreto.

**Regra do encontro:** quantidade positiva, produto ativo e quantidade acumulada até o saldo consultado. Essa validação melhora a experiência, mas não reserva o estoque: outro caixa pode vender antes. A garantia definitiva virá na transação da aula 16.

## 2. Busque o produto pelo código

Acrescente `BuscarCodigoAsync` a `IProdutoRepository` e copie esse método da implementação de referência. Ele usa igualdade por código, sem busca parcial.

Hoje crie um `VendaService` mínimo que recebe apenas `IProdutoRepository` e oferece:

```csharp
public Task<Produto?> BuscarProdutoAsync(Sessao sessao, string codigo)
{
    Permissoes.ExigirCaixa(sessao);
    return produtos.BuscarCodigoAsync(codigo.Trim());
}
```

Na aula 16 adicionaremos a dependência `IVendaRepository`. Não copie ainda a classe completa, que usa tipos de venda não apresentados.

## 3. Amplie a janela do caixa

Receba o serviço no construtor de `FormCaixa`. Acrescente os campos e controles iniciais de [FormCaixa.cs](src/SupermercadoApp/Views/FormCaixa.cs), **até o cancelamento de carrinho**, sem os campos de pagamento e sem o botão Finalizar:

- Código como texto e quantidade em `NumericUpDown`, mínimo 1.
- Grade com `ItemCarrinho` e rótulo de total.
- Botão Adicionar: busca produto, recusa ausência e chama `carrinho.Adicionar`.
- Botão Remover selecionado: remove pelo `ProdutoId`, depois atualiza a grade.
- Botão Cancelar carrinho: limpa apenas a lista em memória.

Declare `private readonly Carrinho carrinho = new();`. A função local `Atualizar` atribui `carrinho.Itens.ToList()` ao `DataSource` e mostra `carrinho.Total` formatado como moeda. Essa nova lista faz a grade enxergar as alterações sem introduzir mecanismos de notificação nesta aula.

No `Program`, componha esse serviço mínimo e passe-o ao caixa. Configure a cultura `pt-BR` no início de `Main`, como no [Program.cs final](src/SupermercadoApp/Program.cs), para que todos os totais tenham a mesma apresentação.

## 4. Faça as contas antes de executar

Prepare arroz a R$ 25,90 com estoque 10 e feijão a R$ 8,50 com estoque 10. Adicione arroz duas vezes, uma unidade por clique, e feijão uma vez. Devem aparecer duas linhas e total **R$ 60,30**. Remova feijão: **R$ 51,80**. Cancele: zero linhas e total zero.

## Verifique antes de encerrar

- Mesmo produto não gera linhas duplicadas; soma quantidades.
- Código inexistente e produto inativo são recusados.
- Quantidade acumulada acima do saldo é recusada.
- Cancelar e fechar carrinho não diminuem o estoque do Neon.
- Não há senha, conexão ou SQL na classe `Carrinho`.

Na próxima aula vamos persistir a venda. Por enquanto, não mostre “venda realizada”: nada foi gravado.

[Anterior: aula 14](14-Reposicao-de-Estoque.md) · [Próxima: aula 16](16-Venda-com-Transacao.md)
