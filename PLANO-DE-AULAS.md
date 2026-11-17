# Plano de ensino: 20 práticas de 3h30

**Carga horária:** 70 horas, em 20 encontros de 210 minutos, incluindo 15 minutos de intervalo por encontro. Se a instituição contabiliza apenas tempo de atividade, cada encontro contém 195 minutos de atividade e 15 de intervalo; ajuste o calendário institucional antes da oferta.

**Público:** estudantes com pouca experiência. É suficiente saber criar pastas, digitar e usar o Windows. A instalação do Visual Studio e o acesso às contas devem ser preparados pelo professor antes do primeiro encontro. Na aula 05 os estudantes aprendem a reproduzir e conferir essa instalação; assim as aulas 01 a 04 conseguem usar o console desde o início.

## Opção de banco de dados

Defina a escolha antes da aula 07: Neon/PostgreSQL ou [MySQL/MariaDB pelo XAMPP](INTEGRACAO-MYSQL-XAMPP.md). A trilha local substitui a preparação das aulas 07/08 e orienta os ajustes de persistência posteriores, preservando as 20 aulas, os tempos e as entregas. O professor deve preparar o conector, validar os scripts e executar os cenários de transação no servidor escolhido. Os checkpoints e o runner de integração existentes são PostgreSQL.

## Rotina de cada encontro

| Etapa | Minutos | O que se faz |
| --- | ---: | --- |
| Retomada e problema | 15 | Abrir a entrega anterior e demonstrar o problema do dia |
| Explicação guiada | 30 | Conceitos mínimos, exemplo e decisão técnica |
| Construção guiada | 60 | Professor e estudantes implementam a primeira parte |
| Intervalo | 15 | Pausa |
| Prática acompanhada | 60 | Implementar a segunda parte e testar casos diferentes |
| Verificação e entrega | 30 | Demonstrar, explicar uma escolha e registrar evidências |
| **Total** | **210** | **3h30** |

Cada capítulo especifica o conteúdo desses blocos. Desafios extras são opcionais e não condicionam a próxima aula. O professor deve fornecer uma cópia funcional da aula anterior a quem precisar recuperar o ponto de partida, sem consumir o encontro inteiro com erros acumulados.

## Pré-requisitos e evidências por aula

| Aula | Ponto de partida | Evidência de conclusão |
| --- | --- | --- |
| 01 | Laboratório com VS instalado | Código que calcula compra e decide sobre desconto |
| 02 | 01 | Console não encerra com quantidade inválida; depuração demonstrada |
| 03 | 02 | Dois objetos independentes e carrinho calculando subtotal |
| 04 | 03 | Regra no serviço e dados em repositório em memória |
| 05 | 04 | `SupermercadoApp` abre uma janela e compila |
| 06 | 05 | Login com nomes de controles, senha oculta e Tab/Enter |
| 07 | 06, conta Neon ou XAMPP local preparado | Tabela `usuario` criada; restrições explicadas |
| 08 | 07 | `SELECT 1` assíncrono e falha de rede tratada |
| 09 | 08 | Admin criado com hash; senha correta e incorreta verificadas |
| 10 | 09 | Cada perfil acessa sua janela; sair retorna ao login |
| 11 | 10 | Script 002 e regras rejeitando preço inválido |
| 12 | 11 | Grade consulta nome e código, inclusive sem resultados |
| 13 | 12 | Criar, editar, recusar duplicidade e inativar |
| 14 | 13 | Uma reposição gera saldo e movimento coerentes |
| 15 | 14 | Carrinho agrupa produtos e não altera banco |
| 16 | 15 | Script 003; rollback e disputa pelo último item verificados |
| 17 | 16 | Venda em dinheiro, troco, repetição da mesma operação |
| 18 | 17 | Relatório inclui datas inicial/final e soma valores corretos |
| 19 | 18 | Novo caixa entra; caixa não cria usuários |
| 20 | 19 | Verificações automáticas, roteiro manual e publicação local |

## Como usar a referência sem perder o passo a passo

O [exemplo](src/README.md) reúne o resultado de todas as aulas para o professor conferir nomes, assinaturas e SQL. Os capítulos dizem quais arquivos e métodos pertencem ao encontro. Construa primeiro a versão mínima indicada; copie apenas os trechos daquela aula. Quando o arquivo de referência possui funções futuras, deixe-as de fora até sua aula.

Nas interfaces, declare apenas os métodos já implementados. Os tipos novos da pasta `Models` também entram quando a aula os apresenta. Não adicione uma assinatura que usa `Produto` antes de criar `Produto`. Até a aula 10, `Program.cs` só compõe os objetos já existentes; abra o login diretamente. Na aula 10, use janelas vazias para os menus; os botões entram nas respectivas aulas. Isso mantém a entrega intermediária compilável.

Para recuperação, gere as [versões por aula](ensino/README.md) com `tools/Gerar-Checkpoints.ps1`. A aula N começa no checkpoint N−1 e termina no N; o checkpoint 00 prepara o primeiro console. Os projetos são independentes e possuem inventário de fontes e scripts. O formulário antecipado de contas é uma ferramenta explícita do professor, disponível por argumento e com autorização, até entrar no menu na aula 19.

Antes da aula 01, faça diagnóstico sem nota de arquivos, IDE e execução. Em cada retomada, peça previsão de uma entrada diferente da aula anterior; nos marcos, peça explicação individual. A [preparação do professor](ensino/README.md) detalha banco, contas e quatro paradas para as aulas 09 e 16.

## Avaliação formativa

Use o [caderno de atividades por conceito](ATIVIDADES-POR-CONCEITO.md) como enunciado de entrega das 20 aulas. Cada atividade tem ID e critério de aceite; vários IDs podem usar o mesmo código e a mesma demonstração. Reúna tudo em uma entrega por aula, registrando a localização de cada evidência no caderno.

As atividades substituem verificações dispersas e exercícios equivalentes, dentro dos blocos já previstos; não representam uma segunda lista a acrescentar às 70 horas. Planeje construção com apoio, variação de entradas na prática e explicação individual no fechamento. Nas aulas 09 e 16, o professor deve preparar infraestrutura e checkpoints antes do encontro. Ajuste o calendário se os objetivos não couberem na aplicação piloto, sem deslocar obrigatoriedades para casa.

A correção deve registrar por ID: atingiu, parcial ou pendente, com uma ação de recuperação. Em dupla, mantenha a explicação individual. Exercícios com gabarito expansível servem à autoavaliação; as evidências entregues devem demonstrar o trabalho do estudante.

| Dimensão e peso | Nível 0 | Nível 1 | Nível 2 | Nível 3 |
| --- | --- | --- | --- | --- |
| Funcionalidade — 40% | Ausente/não executa | Execução parcial | Caso normal correto | Normal e limites corretos |
| Explicação — 25% | Não identifica conceito | Repete definição | Explica no próprio código | Prevê mudança e justifica |
| Dados/erros — 25% | Não verifica | Só caminho feliz | Demonstra recusa | Recusa e preservação do estado |
| Organização/evidências — 10% | Sem entrega verificável | Arquivos sem caso | IDs, arquivos e resultados | Outra pessoa reproduz |

Calcule cada parcela como `peso × nível / 3` e some para obter percentual. Em atividade conceitual, use justificativa e contraexemplo no lugar de execução. A devolutiva deve citar o ID e a próxima ação de recuperação; não substitua o mapa de conceitos por uma nota global.

Em cada encontro, registre no [caderno](CADERNO-DE-EVIDENCIAS.md) o caso normal, um caso inválido e o motivo de uma decisão. Sugestão de rubrica: funcionalidade 40%, explicação 25%, tratamento de dados e erros 25%, organização e evidências 10%. A demonstração de uma tela funcionando não substitui a explicação de quem decide a regra e quem executa o SQL.

Marcos: aula 06 (primeira interface), 10 (acesso separado), 14 (administração de produtos), 17 (venda completa) e 20 (entrega). Ao fechar cada marco, compacte uma cópia do projeto sem `bin`, `obj`, `.vs` ou credenciais. As durações são estimativas para planejamento; o professor deve validar o ritmo na primeira turma e registrar ajustes, sem transferir passos obrigatórios para tarefas fora do encontro.
