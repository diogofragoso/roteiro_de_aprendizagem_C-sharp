# Caderno de atividades por conceito — C# e MarketFlow

Material para entregar aos estudantes, complementar às [20 aulas do projeto](README.md). Para iniciantes, essas aulas vêm depois do [guia preparatório F00–F07](fundamentos/README.md), que tem exercícios e critérios próprios; os IDs A01–A20 desta página não substituem essa preparação. Cada linha é uma atividade com ID, conceito, enunciado e critério de aceite. Agrupe as linhas da aula em **uma única entrega**, reutilizando código e evidências. Não é necessário criar um projeto para cada atividade.

Para cada ID, indique o arquivo/método ou resposta, preparação, resultado esperado e observado e uma explicação curta com suas palavras. Use o [modelo de evidências](CADERNO-DE-EVIDENCIAS.md). Prints sozinhos não substituem código nem explicação. Não envie credenciais, senhas, hashes completos ou dados pessoais reais; use dados fictícios. Os exemplos de cartão, Pix e balança são apenas simulações.

As atividades são obrigatórias na aula indicada, com execução acompanhada quando dependem do laboratório. Se faltar infraestrutura, registre **pendente**; previsão e simulação não comprovam integração. Recursos futuros citados como evolução nas aulas não fazem parte destas entregas.

<a id="aula-01"></a>

## Aula 01 — Lógica e tipos

Use valores definidos no código; leitura do teclado será praticada na aula 02.

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A01.01 | C#, .NET e execução | Escreva três frases relacionando código C#, compilação e execução no .NET; execute uma mensagem no console. | Distingue linguagem de plataforma e identifica o resultado da execução. |
| A01.02 | Variáveis, tipos e tipagem | Declare nome, setor, código `000123`, estoque, contador grande, peso, preço e estado ativo usando `string`, `char`, `int`, `long`, `double`, `decimal` e `bool`. Faça uma atribuição incompatível, registre o diagnóstico e corrija. | O código final compila; zeros são preservados; preço é `decimal`; explica a incompatibilidade. |
| A01.03 | Constantes e operadores | Defina `const decimal TAXA_SIMULADA = 0.10m`; calcule subtotal, valor da taxa e total de 3 unidades a 5,50. Tente alterar a constante e depois retire a linha inválida. | Subtotal 16,50; taxa 1,65; total 18,15; explica multiplicação, adição e constante. |
| A01.04 | Conversão e formatação | Converta `"3"` para inteiro, `"5.50"` para decimal com cultura invariável e formate o produto da multiplicação como moeda `pt-BR`. Compare com a atribuição direta de texto a decimal. | Resultado monetário equivalente a R$ 16,50; diferencia conversão, atribuição e apresentação. |
| A01.05 | `if`, `else if`, `else` e limites | Aplique os descontos da aula aos valores 199,99; 200; 499,99; 500. Acrescente recusa de quantidade zero antes de calcular uma compra. | Descontos 0; 15; 15; 50, respectivamente; quantidade zero não segue como compra válida. |
| A01.06 | `switch`, `case`, `break`, `default` | Faça menu simulado de pagamento com opções 1, 2, 3 e uma inválida. Entregue as quatro saídas. | Cada opção executa somente seu caso; inválida informa recusa; não há integração de pagamento. |
| A01.07 | `for` e incremento | Imprima três vias numeradas de um comprovante didático. Registre valor inicial, condição e incremento. | Saem somente vias 1, 2 e 3; explica por que não sai a quarta. |
| A01.08 | `while`, operadores lógicos e `foreach` | Simule no máximo cinco verificações de balança com `while`; depois percorra quatro nomes de produto com `foreach`. Explique `!` e `&&` na condição. | Simulação termina após cinco tentativas sem estabilidade; cada produto aparece uma vez; distingue condição de parada e percurso de coleção. |

<a id="aula-02"></a>

## Aula 02 — Console e depuração

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A02.01 | Projeto, entrada, saída e interpolação | Crie `TreinoSupermercado`; leia nome, quantidade e preço; imprima comprovante com interpolação e moeda. | Nome digitado aparece; 3 × 5,50 produz 16,50; identifica entrada, processamento e saída. |
| A02.02 | `ReadLine`, nulabilidade e `??` | Explique o resultado de `string? entrada = null; string nome = entrada ?? "";` e compare com entrada `""` e `"Arroz"`. | Obtém vazio, vazio e Arroz; explica que `??` trata `null`, não valida texto vazio. |
| A02.03 | `TryParse`, `out`, `do/while`, `!` e `\|\|` | Valide quantidade com as entradas `abc`, `0`, `-1` e `3`; registre por tentativa se houve conversão e se atendeu à regra. Compare com a falha de `Convert` em `abc`. | Repete para as três primeiras; aceita 3; distingue formato inválido de número não positivo. |
| A02.04 | Cultura e decimal | Configure `pt-BR`; valide preço com `decimal.TryParse`; tente `abc`, `0` e `5,50`. Explique a diferença para o literal C# `5.50m`. | Só 5,50 é aceito nesse conjunto; distingue texto do usuário e sintaxe do código. |
| A02.05 | Decisões e fronteiras | Execute as regras de brinde/cupom da aula com totais 50; 50,01; 100; 100,01. | Respectivamente agradecimento, brinde, brinde e cupom; explica o uso de `>` em vez de `>=`. |
| A02.06 | Decremento, repetição e depuração | Faça o estoque de maçãs cair de 3 a 0; coloque breakpoint na atualização, use F10 e anote antes/depois de cada passo. Teste estoque inicial zero. | Não gera saldo negativo; zero não vende; mostra valores observados no depurador e o alerta final. |

<a id="aula-03"></a>

## Aula 03 — Orientação a objetos

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A03.01 | Classe, objeto, `new` e propriedades | Crie `Produto.cs`, inicialize textos e instancie arroz e feijão; altere apenas o feijão. Mostre código e estados antes/depois. | Dois objetos independentes; explica classe, instância e propriedade. |
| A03.02 | Abstração | Escolha quatro dados úteis para produto e descarte dois que não pertencem ao escopo. Justifique cada escolha. | Relaciona dados ao caso de uso; não cria características sem finalidade. |
| A03.03 | Composição, lista e propriedade calculada | Crie `ItemDeCompra` com produto, quantidade e subtotal calculado; percorra uma `List<ItemDeCompra>` com 2 arrozes a 25,90 e 1 feijão a 8,50. | Total 60,30; explica relação entre item e produto e por que subtotal não é digitado. |
| A03.04 | Encapsulamento, acesso, método e exceção | Use `Usuario.Renomear`; teste nome válido e vazio com `try/catch` orientado pelo professor. Tente atribuir diretamente a propriedade com setter privado e retire a linha inválida. | Nome vazio gera recusa e preserva o nome anterior; acesso direto não compila; explica `public`, `private` e `throw`. |
| A03.05 | Herança | Em exercício separado do modelo final, crie `Pessoa`, `Cliente` e `Funcionario` e execute um método herdado em ambos. | Propriedades comuns vêm da base; mostra uma propriedade específica de cada derivada e justifica a relação. |
| A03.06 | Interface e polimorfismo | Implemente dois pagamentos simulados por `IPagamento`; percorra uma lista desse tipo e chame `Processar` para o mesmo valor. | A mesma chamada produz comportamentos distintos; nenhuma cobrança real; explica implementação de contrato. |

<a id="aula-04"></a>

## Aula 04 — Arquitetura e contratos

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A04.01 | Responsabilidades e fluxo | Distribua entrada, validação, consulta, SQL e composição entre Views, Services, Repositories, Infrastructure e Program; desenhe as chamadas. | SQL não fica na tela; repositório não exibe `MessageBox`; Program conecta dependências. |
| A04.02 | Interface, contrato e `IReadOnlyList` | Implemente `IConsultaProdutos` e a lista em memória; tente usar `Add` pelo contrato e retire a linha inválida. Explique o limite dessa proteção. | Consulta funciona; contrato não oferece `Add`; não afirma que os objetos retornados são necessariamente imutáveis. |
| A04.03 | Construtor e injeção manual | Crie uma segunda implementação com catálogo diferente; entregue-a ao mesmo serviço trocando apenas a criação em Program. | Saída muda sem editar a lógica do serviço; identifica quem cria e quem recebe a dependência. |
| A04.04 | Serviço, `try/catch` e independência de UI | Chame `ValidarPreco` com 25,90; 0; -1 pelo console e registre a mensagem de cada caminho. | Somente positivo passa; exceção é tratada no chamador; regra não exige formulário. |
| A04.05 | Limites da arquitetura | Explique o que precisaria mudar para mover a credencial e a autorização para uma API, e cite uma dificuldade de trocar PostgreSQL por outro banco. | Localiza a fronteira de servidor e não promete que pastas ou interfaces tornam a troca automática. |

<a id="aula-05"></a>

## Aula 05 — Ambiente e solução

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A05.01 | IDE, SDK, runtime e workload | Registre versão da IDE, saída de `dotnet --list-sdks` e finalidade de cada componente. Confira o workload desktop. | Distingue editar/depurar, compilar e executar; registra ambiente realmente disponível. |
| A05.02 | Solução, projeto e framework | Crie solução Windows Forms conforme o ambiente definido pelo professor; identifique no `.csproj` o framework e `UseWindowsForms`. | Compila e abre janela; distingue solução, projeto e .NET Framework. |
| A05.03 | Namespace, `partial` e Designer | Renomeie/mova o login e confira os namespaces das duas partes; explique `.cs`, `.Designer.cs` e `InitializeComponent`. | Designer abre e aplicação compila; as duas partes pertencem à mesma classe. |
| A05.04 | Ponto de entrada e execução | Localize inicialização e `Application.Run`; depure o construtor do login e feche a janela. | Breakpoint é atingido e fechar encerra o aplicativo desta etapa. |
| A05.05 | Organização e entrega de fontes | Organize as cinco pastas e gere cópia de fontes sem `bin`, `obj`, `.vs` ou segredos; confira o que foi incluído. | Outra pessoa identifica o projeto e seus arquivos; não entrega apenas executável. |

<a id="aula-06"></a>

## Aula 06 — Controles e eventos

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A06.01 | Controles, `Name`, `Text` e propriedades | Monte a tela com Labels, TextBoxes e botão; explique três diferenças entre identificador e texto exibido. | Nomes correspondem ao código; rótulos continuam visíveis com campos vazios. |
| A06.02 | Evento `Click`, validação e `return` | Implemente o evento; teste ambos vazios, apenas usuário e ambos preenchidos. | Mensagens corretas; `return` interrompe só o evento; não anuncia autenticação real. |
| A06.03 | Teclado e acessibilidade | Demonstre Tab, Shift+Tab e Enter; registre ordem, contraste e mensagem textual de erro. | Fluxo funciona sem mouse; não depende apenas de cores; Enter aciona uma vez. |
| A06.04 | Ocultação de senha e inscrição de evento | Confira `UseSystemPasswordChar` e a associação única do Click; reabra a janela. | Campo inicia vazio e oculto; explica por que ocultação não protege armazenamento; um clique produz uma mensagem. |

<a id="aula-07"></a>

## Aula 07 — Banco e integridade

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A07.01 | Projeto, branch, banco e isolamento | Registre os nomes do ambiente didático e explique o que deve conferir antes de executar o script 001. | Identifica o ambiente da dupla sem credenciais e não confunde branch com tabela. |
| A07.02 | Tabela, coluna, linha e tipos SQL | Faça dicionário de `usuario` com nome, tipo e finalidade de cada coluna; execute as duas consultas da aula. | Estrutura corresponde ao script e tabela retorna zero usuários nesta etapa. |
| A07.03 | PK, identity, unicidade, nulabilidade e `CHECK` | Em tabela temporária de exercício, sem senhas, reproduza identity/PK, username único/não nulo/minúsculo e perfil restrito. Tente duplicidade, `NULL`, maiúsculas e perfil inválido, uma instrução por vez. | Cada caso é relacionado à restrição correta; não cria contas fictícias com hash inválido na tabela real; distingue `NULL` de vazio. |
| A07.04 | `BEGIN`, `COMMIT` e `ROLLBACK` | Na tabela temporária, abra transação, insira uma linha e reverta; depois repita confirmando. | Linha revertida não permanece; confirmada permanece na sessão; não presume IDs consecutivos. |
| A07.05 | Role, perfil, string de conexão e TLS | Entregue modelo sem valores reais e explique Host, Database, Username, Password e `VerifyFull`; diferencie role PostgreSQL de perfil Caixa. | Não inclui segredo nem desativa verificação de certificado; identifica responsabilidades distintas. |

<a id="aula-08"></a>

## Aula 08 — Assincronia e recursos

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A08.01 | NuGet e dependência | Instale a versão de Npgsql definida para a turma e identifique a referência no projeto. | Compila; distingue pacote de biblioteca e extensão da IDE. |
| A08.02 | Configuração e variável de ambiente | Configure localmente `SUPERMERCADO_DB`; execute com configuração presente e ausente, sem fotografar seu valor. | Ausência produz mensagem compreensível; explica necessidade de reiniciar processos. |
| A08.03 | Data source, pool, conexão e descarte | Desenhe o ciclo de vida de `Banco`, `NpgsqlDataSource`, conexão/comando e descarte em sucesso e falha. | Fonte é reutilizada; recursos de operação são descartados; explica `using` e `await using`. |
| A08.04 | `async`, `await`, `Task` e `async void` | Execute `SELECT 1`; antes da chamada, use temporariamente `await Task.Delay(2000)` para observar a janela responsiva. Remova a demora ao concluir. Explique por que evento usa `async void`. | Janela pode ser movida; retorno da consulta é 1; não usa `.Wait()`/`.Result`; distingue demora simulada de teste de rede. |
| A08.05 | `try/catch/finally` e estado do botão | Teste sucesso e indisponibilidade com orientação do professor; registre estados do botão e conteúdo público da mensagem. | Sem segundo envio pelo botão durante espera; botão reabilitado após ambos os caminhos; não expõe conexão. |

<a id="aula-09"></a>

## Aula 09 — Autenticação

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A09.01 | `enum`, `record` e separação de dados | Crie Perfil, Credencial e Sessao; faça quadro de campos e explique quais chegam à tela. | Sessão não contém senha/hash; enum representa os dois perfis; explica finalidade dos records. |
| A09.02 | Nulabilidade e `Task<T?>` | Consulte usuário inexistente; rastreie o retorno pelo repositório, serviço e evento. | Ausência resulta em login recusado, sem tentar acessar propriedades de `null`. |
| A09.03 | Construtor primário e `sealed` | Reescreva em exercício pequeno um construtor primário como construtor comum e identifique efeito de `sealed`. | Mesma dependência é recebida e usada; não confunde `sealed` com imutabilidade. |
| A09.04 | SQL parametrizado e leitura | Localize o parâmetro de username; teste username contendo apóstrofo na consulta de login. | Texto é dado, não comando SQL; não há concatenação; tentativa inexistente é recusada. |
| A09.05 | Hash, salt e verificação | Em exercício local com PasswordHasher, gere duas representações da mesma senha fictícia e verifique senha correta/incorreta. Registre apenas igualdade/desigualdade e resultados. | Representações diferem e ambas verificam a senha correta; incorreta falha; explica por que não há descriptografia. |
| A09.06 | Rehash e política de senha | Explique os três resultados de verificação, identifique o ramo de rehash e teste criação com 11 e 12 caracteres. | 11 é recusado, 12 atende ao tamanho; atualização ocorre no ramo indicado; não afirma ter provocado rehash sem fazê-lo. |
| A09.07 | Preparação e normalização | Prepare admin em tabela vazia, remova argumento e faça login com username em outra capitalização; tente preparar novamente. | Nova preparação é recusada; login normal funciona; username é normalizado e senha não recebe `Trim`. |
| A09.08 | Evento, retorno e limpeza | Ligue serviço ao login; teste sucesso, senha incorreta e usuário inexistente. | Só sucesso produz sessão/DialogResult OK; duas recusas têm mensagem genérica; senha é limpa e botão reabilitado. |

<a id="aula-10"></a>

## Aula 10 — Sessão e autorização

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A10.01 | Autenticação e autorização | Preencha matriz de ações para Admin e Caixa; explique diferença entre entrar e ter permissão. | Corresponde à política do curso, incluindo admin não vender nessa conta. |
| A10.02 | Permissão no serviço | Com sessão de caixa, chame diretamente uma exigência de administrador, sem depender de botão oculto. | `UnauthorizedAccessException`; nenhuma ação autorizada indevidamente; explica limite do cliente desktop. |
| A10.03 | Herança de Form, layout e lambda | Crie menu mínimo e botão Sair em código; relacione com a criação do botão no Designer e reescreva a lambda como método nomeado. | Clique fecha menu nas duas formas; identifica Form base, painel e evento; não duplica inscrição. |
| A10.04 | `ShowDialog`, `DialogResult`, laço e descarte | Faça três ciclos admin → sair → caixa → sair → login; depois feche login. | Perfil anterior não é reaproveitado; senha vazia; aplicativo termina ao fechar login; explica descarte. |

<a id="aula-11"></a>

## Aula 11 — Produtos e validação

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A11.01 | Modelagem, FK e histórico | Desenhe produto, usuário e movimento com PK/FK e cardinalidades; descreva saldo versus histórico e quantidade com sinal. | Produto tem vários movimentos; movimento identifica produto e responsável; produto nasce com saldo zero. |
| A11.02 | Identificador textual, `numeric` e precisão | Explique código `000123` e `numeric(10,2)`; teste 0; 0,01; 1,999; 99.999.999,99; 100.000.000. | Aceita 0,01 e máximo; rejeita demais no serviço; não arredonda silenciosamente 1,999. |
| A11.03 | `Trim`, tamanhos e ID | Teste código com espaços externos, comprimentos 0/1/30/31, nome com 1/2/120/121 caracteres e IDs -1/0. | Após Trim respeita limites; -1 é recusado, zero representa inclusão. |
| A11.04 | Fake, `Task.CompletedTask` e fronteira da regra | Crie fake com contador de chamadas e argumentos; chame salvar com dados válidos, inválidos e sessão de caixa. | Só dados válidos com admin chegam ao fake; contador comprova ausência de chamada nos demais; explica Task concluída. |
| A11.05 | Validação e integridade | Escolha duas regras e explique o papel do serviço e da restrição de banco em cada uma. | Distingue mensagem amigável de garantia persistente; não afirma que UI é a única proteção. |

<a id="aula-12"></a>

## Aula 12 — Consulta e grade

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A12.01 | `SELECT`, `ILIKE`, parâmetros e curingas | Busque `arr`, código completo, texto ausente, vazio, `%` e apóstrofo; registre expectativas antes de executar. | Distingue pesquisa parcial, ausência e curinga; apóstrofo não altera estrutura SQL. |
| A12.02 | Ordenação, limite e strings multilinha | Localize `ORDER BY`, `LIMIT 200` e string de três aspas; explique a saída de busca vazia e escreva o aviso da tela. | Não promete catálogo completo além do limite; SQL permanece legível e parametrizado. |
| A12.03 | Leitura e mapeamento | Faça tabela coluna SQL → posição → propriedade → tipo C# do método Ler. | Todos os campos correspondem à ordem do SELECT; explica efeito de trocar colunas sem mudar leitor. |
| A12.04 | `DataSource`, `ToList`, `Shown` e função local | Carregue grade ao abrir e ao buscar; explique a função local Carregar e a materialização. | Grade somente leitura e sem linha de inclusão; lista vazia não provoca exceção. |
| A12.05 | Composição, erro e acesso | Passe repositório/serviço até a janela; compare resultado vazio, falha de rede e tentativa de caixa. | Cada caso tem resultado distinto; SQL não está na View; caixa é recusado no serviço. |

<a id="aula-13"></a>

## Aula 13 — Escrita e exclusão lógica

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A13.01 | Inclusão, defaults e `NumericUpDown` | Cadastre produto ativo com preço válido e confira ID, saldo e precisão do controle. | ID gerado, saldo zero, ativo verdadeiro; não permite editar saldo no cadastro. |
| A13.02 | Seleção, edição e identidade | Selecione pelo DataBoundItem, altere nome/preço e confira ID/saldo; tente editar sem seleção. | Mantém identidade e saldo; sem seleção informa necessidade de escolher linha. |
| A13.03 | Estado Novo e linhas afetadas | Após editar, use Novo e inclua outro produto; explique retorno 0 de um UPDATE em produto inativo/inexistente. | Novo não sobrescreve anterior; zero não é apresentado como alteração bem-sucedida. |
| A13.04 | Unicidade e concorrência | Tente código repetido e explique por que SELECT prévio não garante exclusividade entre duas instâncias. | Sem segunda linha; mensagem compreensível; identifica restrição e código `23505`. |
| A13.05 | Inativação, confirmação e persistência | Cancele e depois confirme inativação; feche/reabra a tela e confira registro e código reservado. | Cancelar preserva estado; confirmar muda ativo sem DELETE; alterações persistem. |
| A13.06 | Eventos assíncronos e preservação | Teste nome vazio/preço zero e uma falha de gravação acompanhada pelo professor. | Botão retorna ao estado normal; não limpa dados antes de sucesso; sem falso anúncio de salvamento. |

<a id="aula-14"></a>

## Aula 14 — Reposição

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A14.01 | Incremento atômico | Compare em linha do tempo duas reposições +10 e +3 sobre saldo zero por leitura/soma/gravação e por incremento SQL. Execute as reposições normais. | Explica risco de sobrescrita na primeira estratégia; saldo real final 13. |
| A14.02 | Transação e conexão compartilhada | Marque no método ReporAsync abertura, transação, UPDATE, INSERT e commit; explique falha entre comandos. | Comandos compartilham conexão/transação; não promete confirmação parcial. |
| A14.03 | Auditoria e conciliação | Consulte os dois movimentos e relacione usuário, motivo e quantidade ao saldo. | +10 e +3, saldo 13 e responsável é admin conectado. |
| A14.04 | Validação e autorização | Tente reposição zero/negativa pelo serviço, produto inativo e sessão de caixa. | Nenhum desses casos altera saldo/histórico; registra antes/depois. |
| A14.05 | Seleção e estado de UI | Tente sem seleção e acompanhe clique válido durante espera; confira grade após sucesso. | Sem seleção não repõe; botão desabilitado durante chamada e reabilitado; saldo exibido atualizado. |

<a id="aula-15"></a>

## Aula 15 — Carrinho

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A15.01 | Coleção, subtotal, `Sum` e agrupamento | Adicione arroz duas vezes e feijão uma; compare contas feitas antes com execução; crie uma verificação executável que falhe se total não for 60,30. | Duas linhas, arroz quantidade 2, total 60,30; explica soma de subtotais. |
| A15.02 | Lista privada e leitura | Localize coleção interna, `AsReadOnly` e atualização de grade com `ToList`. Explique quem pode adicionar/remover. | Alterações passam pelos métodos do carrinho; não confunde cópia da lista com cópia profunda. |
| A15.03 | Record, `with` e coleção `[]` | Compare item anterior e resultado de `with { Quantidade = ... }`; reescreva inicialização de lista sem `[]`. | Explica cópia com nova quantidade e equivalência da lista vazia neste contexto. |
| A15.04 | `checked` e excesso de inteiro | Em exercício isolado, some `int.MaxValue` e 1 dentro de `checked`, tratando a exceção. Relacione à quantidade acumulada. | O excesso é detectado; não exige criar estoque enorme no banco. |
| A15.05 | Busca exata e regras de inclusão | Teste código inexistente, parcial, inativo, quantidade zero e acumulada acima do saldo. | Busca por igualdade; recusas preservam carrinho anterior. |
| A15.06 | Remoção, cancelamento e memória | Remova feijão e cancele; consulte estoque antes/depois e explique diferença entre carrinho e reserva. | Totais 51,80 e zero; nenhuma venda ou baixa no banco. |

<a id="aula-16"></a>

## Aula 16 — Venda e concorrência

Realize os testes em ambiente didático preparado pelo professor. Use o [roteiro de integração](VALIDACAO-MANUAL.md) e guarde contagens/saldos iniciais.

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A16.01 | Relacionamentos e snapshot histórico | Desenhe venda/itens/produtos/usuário; confirme venda e depois altere nome/preço do produto. | Item da venda preserva nome/preço originais; distingue cadastro atual e histórico. |
| A16.02 | Pedido e autoridade do preço | Mostre os campos enviados em PedidoItem; compare carrinho vazio, quantidade inválida e pedido válido com preço de tela desatualizado. | Serviço recusa inválidos; preço definitivo vem do banco, não do pedido. |
| A16.03 | `Guid`, UUID e idempotência | Guarde uma chave e repita a venda com mesmos itens, usuário e recebido; depois execute compra nova com outra chave. | Repetição retorna mesmo ID e uma baixa; compra nova usa outra operação. |
| A16.04 | Advisory lock e bloqueio de linhas | Desenhe duas chamadas da mesma chave e duas de chaves distintas para produtos comuns; marque onde aguardam. | Distingue lock da operação e `FOR UPDATE`; explica ordenação crescente e redução de risco de deadlock. |
| A16.05 | Atomicidade, commit e rollback real | Com falha controlada do roteiro após escrita, compare saldo e contagens de venda/itens/movimentos antes/depois; remova a falha ao concluir. | Tudo permanece como antes; identifica ponto da falha; não usa mera validação prévia como prova de rollback de escrita. |
| A16.06 | Concorrência e último item | Em dois caixas, inclua o mesmo produto de estoque 1 e finalize com chaves distintas. | Uma confirmação, uma recusa, saldo zero, uma saída e nenhum cabeçalho órfão. |
| A16.07 | `RETURNING`, total e movimentos | Rastreie uma venda válida desde INSERT até ID retornado e movimento negativo; tente recebido insuficiente. | Não usa MAX(id); soma dos itens confere com total; insuficiência não grava venda/baixa. |
| A16.08 | Mesma chave concorrente | Com apoio do professor, faça duas chamadas da mesma operação com `Task.WhenAll` conforme roteiro. | Mesmo ID e contagens de uma venda; execução pendente é registrada se não houver ambiente. |

<a id="aula-17"></a>

## Aula 17 — Finalização e falhas

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A17.01 | Máquina de estados | Desenhe transições entre montagem, espera, confirmação, recusa e comunicação incerta; indique edição e ações permitidas. | Não permite editar intenção pendente; recusa conhecida e incerteza têm saídas diferentes. |
| A17.02 | Recebido, total definitivo e troco | Venda 60,30 com recebido 100; depois teste recebido insuficiente e mudança de preço antes de confirmar. | Troco 39,70 no primeiro caso; definitivo usa banco; insuficiência preserva carrinho e não baixa saldo. |
| A17.03 | Duplo clique e chave pendente | Execute finalização e acompanhe bloqueio dos controles e renovação do Guid. | Chave muda somente após confirmação; repetir intenção pendente mantém itens e recebido. |
| A17.04 | Falha de comunicação e reconciliação | Simule, com o professor, perda da resposta após confirmação e consulta pela mesma chave; registre separadamente qualquer teste real de rede realizado. | Não anuncia cancelamento sem certeza; consulta evita duplicidade; não confunde simulação com falha real após commit. |
| A17.05 | FormClosing e memória volátil | Tente fechar em montagem e durante espera; explique recuperação após encerramento forçado e escreva consulta por Guid com valor fictício. | Confirma descarte na montagem e bloqueia espera/incerteza; reconhece ausência de recuperação durável automática. |
| A17.06 | Comprovante e escopo | Entregue comprovante fictício com ID, total, recebido, troco e aviso não fiscal. | Valores correspondem à venda; não apresenta simulação como cobrança ou documento fiscal. |

<a id="aula-18"></a>

## Aula 18 — Datas e relatório

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A18.01 | Instante UTC, fuso e cultura | Anote um horário local e equivalente UTC, com fuso/offset usado; explique `DateTimeKind.Utc`, `ToUniversalTime` e `ToLocalTime`. | Representam o mesmo instante; não usa cultura monetária como ajuste de fuso. |
| A18.02 | Intervalo semiaberto | Escolha um dia fixo; classifique registros no início, imediatamente antes do dia seguinte e exatamente no início do dia seguinte. Confira com massa fornecida pelo professor. | Inclui os dois primeiros, exclui o terceiro; não usa 23:59:59 como limite final. |
| A18.03 | DateTimePicker e parâmetros | Mostre transformação da data final inclusiva para início exclusivo do dia seguinte e teste final anterior ao início. | Limites UTC correspondem às datas locais escolhidas; intervalo invertido é recusado. |
| A18.04 | `Sum`, contagem e vazio | Consulte duas vendas conhecidas de 60,30 e 8,50 e compare com SQL de datas fixas; consulte período sem vendas. | Duas vendas e total 68,80 na massa isolada; período vazio tem zero e zero; não conta itens como vendas. |
| A18.05 | Autorização e escala | Chame relatório com sessão de caixa; explique custo de materializar período grande e proponha agregação/paginação. | Caixa recusado no serviço; proposta separa total agregado de detalhes paginados. |

<a id="aula-19"></a>

## Aula 19 — Usuários

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A19.01 | Reuso e composição | Construa formulário usando CriarAsync já existente e conecte-o ao menu administrativo. | View não duplica hash/SQL; serviço e sessão são recebidos por construtor. |
| A19.02 | Username e unicidade | Teste tamanhos 2/3/50/51, acentos, ponto permitido e repetição em maiúsculas de username existente. | Respeita tamanho/caracteres; normalização não cria conta duplicada. |
| A19.03 | Senha, limites e espaços | Teste 11/12/128/129 caracteres e senha fictícia com espaços externos; confirme login sem recorte. | Só tamanhos permitidos cadastram; senha é preservada como digitada e não aparece nas evidências. |
| A19.04 | Enum, ComboBox e perfil inválido | Use DropDownList com Enum.GetValues; tente perfil inválido diretamente no serviço em exercício acompanhado. | UI não aceita texto livre; serviço também rejeita valor fora do enum. |
| A19.05 | Permissão e fluxo completo | Admin cria caixa; novo caixa entra e tenta criar usuário pelo serviço; teste duplicidade na UI. | Perfil correto abre; caixa não cria conta; falha não é apresentada como sucesso e permite correção. |
| A19.06 | Sessão e limites de escopo | Explique por que sair renova a sessão e descreva política que faltaria para revogação imediata ou recuperação de senha. | Distingue implementação atual de evolução; não afirma que revogação já existe. |

<a id="aula-20"></a>

## Aula 20 — Testes e publicação

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A20.01 | Compilação, teste de regra, integração e UI | Classifique build, caso com fake, rollback PostgreSQL e navegação por teclado; execute os comandos da aula. | Explica o alcance de cada evidência; registra saída real, sem inferir integração dos fakes. |
| A20.02 | Asserção e regressão | Reaproveite um caso anterior e acrescente uma verificação própria de fronteira; em cópia local, introduza erro controlado, observe falha e restaure. | Caso detecta o erro escolhido e passa após correção; explica o defeito que protege. |
| A20.03 | Teste manual e reprodutibilidade | Em dupla, execute casos de cada perfil, invertendo os papéis; registre preparação, esperado, observado e pendências. | Outra pessoa reproduz; falha ou pendência não recebe status de sucesso. |
| A20.04 | Release, RID, autocontida e pasta | Publique para win-x64 conforme ambiente da turma; compare fonte, build e pasta publicada; explique dependência de runtime e atualização da autocontida. | Mantém todos os arquivos necessários e não confunde publicação com distribuição externa. |
| A20.05 | Execução fora da IDE e configuração | Execute na máquina de laboratório com variável configurada; demonstre login por perfil e venda fictícia. | Executável funciona fora da IDE; sem segredo na pasta; dependência de rede é declarada. |
| A20.06 | Entrega e limites | Entregue fontes/scripts, evidências por ID, identificação da versão, instruções e limites; cada integrante explique uma transação e um caminho View → Service → Repository. | Material organizado e reproduzível; dinheiro simulado, não fiscal, conexão direta e tentativa volátil explicitados. |

Use a [rubrica e as orientações do plano](PLANO-DE-AULAS.md) para a correção. Para retomar um conceito parcial, refaça a atividade com outra entrada e explique por que o resultado mudou.
