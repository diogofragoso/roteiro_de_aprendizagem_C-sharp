# Plano de ensino: 20 práticas de 3h30

**Carga horária:** 70 horas, em 20 encontros de 210 minutos, incluindo 15 minutos de intervalo por encontro. Se a instituição contabiliza apenas tempo de atividade, cada encontro contém 195 minutos de atividade e 15 de intervalo; ajuste o calendário institucional antes da oferta.

**Público:** estudantes com pouca experiência. É suficiente saber criar pastas, digitar e usar o Windows. A instalação do Visual Studio e o acesso às contas devem ser preparados pelo professor antes do primeiro encontro. Na aula 05 os estudantes aprendem a reproduzir e conferir essa instalação; assim as aulas 01 a 04 conseguem usar o console desde o início.

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
| 07 | 06, conta Neon | Tabela `usuario` criada; restrições explicadas |
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

Nas interfaces, declare apenas os métodos já implementados. Os tipos novos de `Modelos.cs` também entram quando a aula os apresenta. Não adicione uma assinatura que usa `Produto` antes de criar `Produto`. Até a aula 10, `Program.cs` só compõe os objetos já existentes; abra o login diretamente. Na aula 10, use janelas vazias para os menus; os botões entram nas respectivas aulas. Isso mantém a entrega intermediária compilável.

Para quem precisar de um ponto de partida para recuperação, o professor pode abrir a solução final e desabilitar no menu as funções futuras. Essa é uma alternativa de recuperação, não a sequência principal de construção.

## Avaliação formativa

Em cada encontro, registre no [caderno](CADERNO-DE-EVIDENCIAS.md) o caso normal, um caso inválido e o motivo de uma decisão. Sugestão de rubrica: funcionalidade 40%, explicação 25%, tratamento de dados e erros 25%, organização e evidências 10%. A demonstração de uma tela funcionando não substitui a explicação de quem decide a regra e quem executa o SQL.

Marcos: aula 06 (primeira interface), 10 (acesso separado), 14 (administração de produtos), 17 (venda completa) e 20 (entrega). Ao fechar cada marco, compacte uma cópia do projeto sem `bin`, `obj`, `.vs` ou credenciais. As durações são estimativas para planejamento; o professor deve validar o ritmo na primeira turma e registrar ajustes, sem transferir passos obrigatórios para tarefas fora do encontro.
