# Aula 12 — Consulta de produtos na grade

**Duração:** 3h30. **Pré-requisito:** aula 11. **Entrega:** janela administrativa que consulta produtos por nome ou código e apresenta resultado vazio sem erro.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Conferir tabela produto e seus tipos |
| Explicação | 30 | SELECT parametrizado e vínculo de dados |
| Construção | 60 | Implementar consulta e serviço |
| Intervalo | 15 | Pausa |
| Prática | 60 | Criar grade, busca e carregamento assíncrono |
| Entrega | 30 | Conferir busca, vazio e acesso negado |

## 1. Implemente a leitura

No contrato `IProdutoRepository`, acrescente `Task<IReadOnlyList<Produto>> ListarAsync(string busca);`. Crie `ProdutoRepository`, recebendo `NpgsqlDataSource` pelo construtor. Copie da [referência](src/SupermercadoApp/Repositories/ProdutoRepository.cs) o método `Ler`, `ListarAsync` e `SalvarAsync` (a escrita já está no contrato e será usada na próxima aula). Por enquanto não adicione outros métodos ao contrato.

```sql
SELECT id, codigo, nome, preco, estoque, ativo FROM produto
WHERE nome ILIKE @busca OR codigo ILIKE @busca
ORDER BY nome LIMIT 200
```

No arquivo C#, as três aspas (`"""`) delimitam uma string de várias linhas, útil para ler SQL sem concatenar pedaços.

O parâmetro recebe `%texto%`. `ILIKE` procura sem distinguir maiúsculas e minúsculas; `%` significa qualquer trecho. Se o estudante digitar `%`, a consulta terá esse significado de busca, mas continuará parametrizada: isso não permite executar comandos SQL.

O limite de 200 evita carregar todo o catálogo nesse exemplo. A tela deve avisar sobre o limite; uma versão maior precisaria de paginação. `Ler` converte as colunas na ordem exata do `SELECT`: mudar a ordem exige atualizar a leitura.

No `ProdutoService`, acrescente `ListarAsync` da [referência](src/SupermercadoApp/Services/ProdutoService.cs). Não ponha SQL na janela.

## 2. Construa a janela mínima

Crie `Views/FormProdutos.cs`, herdando de `Form`. Use `Ui.Preparar`, `Ui.Texto`, `Ui.Grade` e `Ui.Botao`, como na aula 10. Receba `Sessao` e `ProdutoService` pelo construtor. A versão mínima tem busca, grade e botão:

```csharp
var painel = Ui.Preparar(this, "Produtos");
var busca = Ui.Texto(painel, "Nome ou código — até 200 resultados");
var grade = Ui.Grade(painel);
async Task Carregar()
{
    grade.DataSource = (await produtos.ListarAsync(sessao, busca.Text)).ToList();
}
Ui.Botao(painel, "Buscar", async (_, _) => await Ui.ExecutarAsync(Carregar));
Shown += async (_, _) => await Ui.ExecutarAsync(Carregar);
```

Adicione o namespace e os `using` dos tipos utilizados. Confira as opções de `Ui.Grade`: leitura apenas, seleção de linha inteira e sem linha de inclusão. A grid serve para visualizar; edição ocorrerá em campos separados.

## 3. Conecte os objetos

No `Program`, crie `ProdutoRepository` usando a fonte de dados existente e `ProdutoService` usando esse repositório. Passe o serviço para `FormAdministrador`, cujo construtor passa a recebê-lo. Adicione o botão “Produtos” abrindo `new FormProdutos(sessao, produtos)` com `using` e `ShowDialog(this)`. Não copie ainda os botões de relatório ou usuários da referência final.

Antes da aula, o professor cadastra arroz e feijão pela referência no banco didático, conforme [preparação do laboratório](ensino/README.md). Não é necessário criar outro botão provisório. O método de escrita existe no repositório para cumprir o contrato; a turma construirá os campos definitivos na aula 13.

Produza uma tabela das seis colunas retornadas, suas posições de 0 a 5, tipos e propriedades C#. Preveja o que aconteceria se invertesse duas colunas sem mudar `Ler`. Depois diferencie três resultados: lista vazia é consulta concluída sem correspondências; falha de rede é operação não concluída; permissão negada é recusa anterior ao SQL.

## Verifique antes de encerrar

- Busca “arr” encontra arroz; código com zero à esquerda continua texto.
- Busca sem correspondência mostra grade vazia, sem exceção.
- Busca vazia lista até 200 registros e mantém ordem por nome.
- Falha de conexão informa que a consulta não foi concluída.
- Sessão de caixa não consegue chamar `ProdutoService.ListarAsync`.

Uma lista vazia é resultado válido. Não a confunda com falha de rede. Nos registros do caderno, anote o texto buscado e os nomes esperados.

[Anterior: aula 11](11-Modelo-de-Produtos.md) · [Próxima: aula 13](13-Cadastro-de-Produtos.md)

## Atividades por conceito

Realize as atividades da [aula 12 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-12). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
