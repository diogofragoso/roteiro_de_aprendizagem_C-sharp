# Aula 13 — Cadastro, edição e inativação de produtos

**Duração:** 3h30. **Pré-requisito:** aula 12. **Entrega:** administrador inclui, altera e inativa produtos; duplicidades são tratadas com mensagem compreensível.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Consultar produtos cadastrados |
| Explicação | 30 | INSERT, UPDATE, seleção e exclusão lógica |
| Construção | 60 | Campos e inclusão com validação |
| Intervalo | 15 | Pausa |
| Prática | 60 | Edição, inativação e duplicidade |
| Entrega | 30 | Demonstrar os quatro caminhos do cadastro |

## 1. Construa os campos

Amplie `FormProdutos` seguindo [FormProdutos.cs](src/SupermercadoApp/Views/FormProdutos.cs), até o botão de inativação. **Deixe o bloco de quantidade/reposição para a aula 14.** Acrescente:

| Elemento | Função |
| --- | --- |
| Campo `codigo` | Código externo, texto até 30 caracteres |
| Campo `nome` | Nome legível |
| NumericUpDown `preco` | Duas casas e máximo compatível com o banco |
| Campo privado `produtoId` | Zero para inclusão; ID escolhido para edição |
| Botão Novo | Limpa campos e volta para ID zero |
| Botão Editar selecionado | Copia a linha escolhida para os campos |
| Botão Salvar | Chama serviço e atualiza lista |
| Botão Inativar selecionado | Confirma intenção e chama serviço |

Não permita que o usuário digite o ID. `grade.SelectedRows[0].DataBoundItem` lê o objeto da linha efetivamente selecionada; confira antes se `SelectedRows.Count` é maior que zero. Se não há seleção, mostre “Selecione um produto”. `CurrentRow` pode continuar apontando uma linha mesmo depois de `ClearSelection`, então não basta para essa verificação. A ação usa esse objeto e não o número visual da linha, pois a ordenação pode mudar.

## 2. Entenda a escrita

O `SalvarAsync` do repositório já existe desde a aula 12 para cumprir o contrato. Leia os dois caminhos:

```sql
INSERT INTO produto(codigo, nome, preco) VALUES (@codigo, @nome, @preco);
UPDATE produto SET codigo=@codigo, nome=@nome, preco=@preco WHERE id=@id AND ativo;
```

A inclusão deixa estoque em zero e ativo em verdadeiro pelos padrões do banco. A edição não altera saldo. O retorno de `ExecuteNonQueryAsync` deve ser 1; zero indica que o registro não existe ou está inativo. Isso pode acontecer se outra janela mudou o produto.

Acrescente `InativarAsync` ao contrato, ao serviço e ao repositório, conforme as referências das aulas anteriores. Ele executa `UPDATE produto SET ativo=false ...`, sem `DELETE`. O código de um produto inativo continua reservado por `UNIQUE`, preservando a identidade histórica. Reativação não faz parte deste primeiro fluxo.

## 3. Ligue e proteja os eventos

No botão Salvar, desabilite o botão antes de aguardar e reabilite no `finally`. Só limpe os campos **depois** de o serviço concluir e a grade ser atualizada. Se o SQL falhar, preserve os valores para o estudante corrigir.

O helper `Ui.ExecutarAsync` trata o código PostgreSQL `23505` (`UniqueViolation`). Não tente garantir unicidade apenas com um `SELECT` antes do `INSERT`: duas instâncias podem passar na consulta e tentar inserir juntas. A restrição no banco decide, e a tela explica a recusa.

Antes de inativar, mostre uma confirmação com o nome do produto. Cancelar essa confirmação não deve alterar nada. Depois de confirmar, recarregue a grade.

## Verifique antes de encerrar

Use a mesma sequência no registro: incluir → anotar ID/saldo → editar → tentar duplicar → cancelar inativação → confirmar → reabrir. Identifique a evidência de A13.01–A13.06. Para cada recusa, mostre que os dados anteriores foram preservados; uma captura da mensagem sozinha não demonstra isso.

1. Cadastre “000123 / Arroz / 25,90”; confira estoque 0 e ativo verdadeiro.
2. Edite nome e preço, preservando o ID e o código.
3. Tente outro produto com o mesmo código: mensagem de duplicidade, sem encerrar o aplicativo.
4. Nome vazio e preço zero são recusados antes do banco.
5. Cancele a confirmação de inativação; o produto continua ativo. Confirme depois e confira ativo falso.
6. Feche e abra a tela: as alterações continuam no Neon.

Remova os botões temporários de preparo das aulas 11 e 12. A próxima aula dará entrada no estoque com histórico, sem editar diretamente a coluna de saldo.

[Anterior: aula 12](12-Consulta-de-Produtos.md) · [Próxima: aula 14](14-Reposicao-de-Estoque.md)

## Atividades por conceito

Realize as atividades da [aula 13 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-13). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
