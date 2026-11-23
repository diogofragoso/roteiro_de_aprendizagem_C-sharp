# Caderno de Atividades por Conceito — Trilha Unificada C# e MarketFlow (Aulas 01 a 24)

Material para entregar aos estudantes, acompanhando integralmente as [24 aulas da trilha unificada](README.md) — do primeiro programa em C# no Visual Studio 2022 até a publicação do MarketFlow. Cada linha é uma atividade com ID, conceito, enunciado e critério de aceite. Agrupe as linhas da aula em **uma única entrega**, reutilizando código e evidências. Não é necessário criar um projeto para cada atividade.

Para cada ID, indique o arquivo/método ou resposta, preparação, resultado esperado e observado e uma explicação curta com suas palavras. Use o [modelo de evidências](CADERNO-DE-EVIDENCIAS.md). Prints sozinhos não substituem código nem explicação. Não envie credenciais, senhas, hashes completos ou dados pessoais reais; use dados fictícios. Os exemplos de cartão, Pix e balança são apenas simulações.

As atividades são obrigatórias na aula indicada, com execução acompanhada quando dependem do laboratório. Se faltar infraestrutura, registre **pendente**; previsão e simulação não comprovam integração. Recursos futuros citados como evolução nas aulas não fazem parte destas entregas.

<a id="aula-01"></a>

## Aula 01 — Primeiro programa, variáveis e tipos

Use valores definidos no código; leitura do teclado será praticada na aula 02.

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A01.01 | Algoritmo, C#, .NET, VS 2022 e sintaxe | Crie o projeto console no Visual Studio 2022, exiba 3 mensagens, inverta a ordem prevendo a saída e provoque/corrija um erro de aspa na Lista de Erros. | Distingue linguagem, compilação e execução; localiza e corrige erro de compilação. |
| A01.02 | Variáveis, 7 tipos e tipagem forte | Declare nome, setor, código `000123`, quantidade `3`, contador `long`, peso `double`, preço `5.50m` e ativo `bool`. Provoque atribuição incompatível (`decimal = "15.99"`), registre o erro e remova. | Compila limpo; zeros de `000123` preservados; preço é `decimal`; explica o erro `CS0029`. |
| A01.03 | Atribuição em memória | Calcule `subtotal = quantidade * preco`, altere `quantidade` para `5` e exiba `subtotal` antes e depois de recalcular. | Comprova `16,50` antes e `27,50` depois; explica que atribuição não é fórmula automática. |
| A01.04 | Constantes e operadores | Defina `const decimal TAXA_SIMULADA = 0.10m`; calcule subtotal (`16,50`), taxa (`1,65`) e total (`18,15`). Tente alterar a constante e retire a linha inválida. | Valores corretos formatados com interpolação e `:C`; explica `CS0131`. |
| A01.05 | Conversão, `CultureInfo` e `+` | Converta `"3"` para `int` e `"5.50"` para `decimal` com `InvariantCulture`, multiplique e formate em moeda `pt-BR`. Compare `"2" + "3"` com `2 + 3`. | Exibe `R$ 16,50`; diferencia concatenação, soma, conversão e formatação. |
| A01.06 | Planejamento de algoritmo | Escreva no papel o passo a passo para somar três notas (`6`, `7` e `8`) e calcular a média (`7`). | Identifica entradas, operações na ordem correta e saída esperada. |

<a id="aula-02"></a>

## Aula 02 — Entrada segura, decisões, laços e depuração

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A02.01 | Entrada, saída e interpolação | Leia nome, quantidade e preço no console e imprima comprovante didático com interpolação e `:C`. | Nome digitado aparece; `3 × 5,50` exibe `R$ 16,50`; distingue entrada, processamento e saída. |
| A02.02 | `ReadLine`, `string?` e `??` | Explique o resultado de `entrada ?? ""` para `null`, `""` e `"Arroz"`. | Obtém vazio, vazio e Arroz; explica que `??` trata `null` e não valida texto vazio. |
| A02.03 | `TryParse`, `out`, `do/while`, `!` e `\|\|` | Valide quantidade positiva testando `abc`, `0`, `-1` e `3`; registre conversão vs regra por tentativa. | Repete nas três primeiras, aceita `3`; distingue falha de conversão de número não positivo. |
| A02.04 | Cultura `pt-BR` e `decimal.TryParse` | Configure `pt-BR` e valide preço testando `abc`, `0` e `5,50`. Compare com o literal `5.50m`. | Aceita somente `5,50`; diferencia texto digitado pelo usuário da sintaxe C#. |
| A02.05 | `if`, `else if`, `else` e fronteiras | Teste as faixas de brinde/cupom para totais `50,00`; `50,01`; `100,00`; `100,01`. | Agradecimento, brinde, brinde e cupom; explica `>` versus `>=`. |
| A02.06 | `switch`, `case`, `break`, `default` | Construa menu simulado de pagamento (`1`, `2`, `3` e opção inválida) e entregue as quatro saídas. | Executa apenas o caso correspondente; opção inválida cai no `default`. |
| A02.07 | `for`, `while` e `foreach` | Imprima 3 vias com `for`, simule até 5 leituras de balança com `while` e percorra 4 produtos com `foreach`. | Explica inicialização/condição/incremento, operadores `!`/`&&` e percurso de coleção. |
| A02.08 | Decremento e Depurador (`F9`/`F5`/`F10`) | Simule estoque caindo de `3` até `0` (aceitando `0`, recusando negativo) e depure com Breakpoint e janela Locais. | Sem saldo negativo; zero não vende; registra valores observados passo a passo no VS 2022. |

<a id="aula-03"></a>

## Aula 03 — Métodos, parâmetros, retorno e escopo

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A03.01 | Declaração, chamada e retorno | Construa `CalcularSubtotal(decimal, int)` e `CalcularMedia(decimal, decimal)` usando `2m`. | Retorna `25`, `16,50` e `7` sem imprimir na tela dentro das funções. |
| A03.02 | Separação cálculo vs tela | Construa `CalcularDesconto(decimal)` retornando apenas o valor do desconto. | Faixas `500` e `200` corretas; lógica independente de `Console.WriteLine`. |
| A03.03 | Função booleana e reutilização | Construa `EhQuantidadeValida(int)` retornando `bool` e use o resultado num `if` externo. | `-1` e `0` retornam `false`; `2` retorna `true`; mensagem fica fora da função. |
| A03.04 | Função `void` e extração de leitura | Extraia `LerInteiroPositivo(string)` reutilizando-a duas vezes e crie `ExibirComprovante` como `void`. | Elimina duplicação do laço; distingue método com retorno de método `void`. |
| A03.05 | Escopo, cópia de valor e `F11` | Demonstre `Dobrar(int)` sem alterar o original, provoque erro de escopo (`CS0103`) e entre na função com `F11`. | `numero` continua `3` enquanto dobro é `6`; explica escopo e fluxo de chamada no depurador. |

<a id="aula-04"></a>

## Aula 04 — Classes, objetos, propriedades e referências

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A04.01 | Abstração | Escolha 4 propriedades úteis para produto no supermercado e descarte 2 irrelevantes com justificativa. | Relaciona dados ao objetivo do sistema; evita características sem finalidade. |
| A04.02 | Classe em arquivo separado, `new` e estado | Crie `Produto.cs` inicializando texto com `= ""`, instancie arroz e feijão e altere só o feijão. | Dois objetos independentes; arroz mantém `25,90`; sem avisos de nulabilidade. |
| A04.03 | Referência compartilhada na memória | Em `Livro.cs`, crie `primeiro`, `segundo` e `outroNome = primeiro`; altere por `outroNome`. | Exibe `150`, `150` e `80`; explica por que sem `new` ambas apontam para o mesmo objeto. |
| A04.04 | Mapa de memória e vocabulário POO | Desenhe as variáveis e objetos do exercício dos livros e aponte classe, instância, propriedade, método e estado. | Identifica exatamente 3 variáveis de referência e 2 objetos na memória. |

<a id="aula-05"></a>

## Aula 05 — Construtores, campos, encapsulamento e exceções

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A05.01 | `private set` e validação em método | Crie `Usuario.cs` com `Renomear`; teste nome válido e vazio dentro de `try/catch`. | Recusa preserva o nome anterior; explica `public`, `private set` e `Trim()`. |
| A05.02 | Proteção de compilação (`CS0272`) | Tente atribuir diretamente a `usuario.Username` fora da classe, registre o erro e remova a linha. | Comprova que o compilador barra alteração direta de propriedade encapsulada. |
| A05.03 | Construtor, campo privado e `nameof` | Crie `Livro.cs` exigindo título e páginas no construtor, com campo `paginasLidas` e propriedade somente leitura. | `new Livro()` vazio deixa de compilar; distingue campo privado de propriedade pública. |
| A05.04 | Exceções e preservação de estado | Teste `Livro.Ler` com `20`, `-1`, `0`, `101`, `100` e `1` dentro de `try/catch`. | Cada recusa lança exceção e mantém o estado anterior intacto (`20` e depois `120`). |
| A05.05 | Modelagem independente (`bool` vs `throw`) | Construa `SessaoDeEstudo` usando `throw` no construtor e retorno `bool` em `RegistrarMinutos`. | Atende todos os 6 casos de teste; explica quando lançar exceção vs retornar `bool`. |

<a id="aula-06"></a>

## Aula 06 — Composição, coleções e subtotal

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A06.01 | `List<T>`, `Add`, `Count` e `foreach` | Percorra uma `List<string>` preenchida e depois vazia. | Lista vazia tem `Count == 0` e não gera exceção no `foreach`. |
| A06.02 | Composição ("tem um") e `=>` | Crie `Produto` encapsulado e `ItemDeCompra` validando `produto is null` e calculando `Subtotal => ...`. | Explica por que subtotal é calculado e por que item compõe (não herda) produto. |
| A06.03 | Totalização e função separada | Calcule `2` arrozes (`25,90`) + `1` feijão (`8,50`) em `SomarItens(List<ItemDeCompra>)`. | Total `R$ 60,30`; lista vazia retorna `0`; função de soma não imprime no console. |
| A06.04 | Objeto compartilhado e memória volátil | Adicione terceiro item referenciando o mesmo `arroz` (`86,20`), teste `quantidade = 0` e compare RAM com banco. | Identifica 2 objetos `Produto` para 3 `ItemDeCompra`; explica volatilidade da RAM. |

<a id="aula-07"></a>

## Aula 07 — Namespaces, interfaces, herança e polimorfismo

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A07.01 | `namespace`, `using` e contrato `interface` | Organize `IFormatador` e três implementações em `namespace`; remova temporariamente um método para ver `CS0535`. | Explica que `using` não instala pacotes e que o compilador fiscaliza o contrato. |
| A07.02 | Injeção pelo construtor, `readonly` e `this` | Construa `Apresentador(IFormatador)` e execute os 3 formatadores sem alterar `Apresentador.cs`; depure com `F11`. | Produz `Bom dia`, `BOM DIA` e `Mensagem: Bom dia`; explica troca de dependência. |
| A07.03 | Polimorfismo por interface | Crie `IPagamento` com dinheiro e cartão simulados e percorra `List<IPagamento>` com um único `foreach`. | Mesma chamada `Processar` produz comportamentos distintos; sem cobrança real. |
| A07.04 | Herança (`:`) e reaproveitamento | Crie `Pessoa`, `Cliente` e `Funcionario` e execute `ExibirCadastro()` herdado em ambos. | Propriedades comuns vêm da base; cada derivada acrescenta atributo próprio. |
| A07.05 | `virtual`, `override`, `: base(...)` e decisão | Construa `Aviso` e `AvisoUrgente` usando `: base(titulo)` e `base.Exibir()`; justifique interface vs composição vs herança. | Troca entre `new AvisoUrgente` e `new Aviso` altera a saída; não força herança para produtos. |
<a id="aula-08"></a>

## Aula 08 — Arquitetura e contratos

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A08.01 | Responsabilidades e fluxo | Distribua entrada, validação, consulta, SQL e composição entre Views, Services, Repositories, Infrastructure e Program; desenhe as chamadas. | SQL não fica na tela; repositório não exibe `MessageBox`; Program conecta dependências. |
| A08.02 | Interface, contrato e `IReadOnlyList` | Implemente `IConsultaProdutos` e a lista em memória; tente usar `Add` pelo contrato e retire a linha inválida. Explique o limite dessa proteção. | Consulta funciona; contrato não oferece `Add`; não afirma que os objetos retornados são necessariamente imutáveis. |
| A08.03 | Construtor e injeção manual | Crie uma segunda implementação com catálogo diferente; entregue-a ao mesmo serviço trocando apenas a criação em Program. | Saída muda sem editar a lógica do serviço; identifica quem cria e quem recebe a dependência. |
| A08.04 | Serviço, `try/catch` e independência de UI | Chame `ValidarPreco` com 25,90; 0; -1 pelo console e registre a mensagem de cada caminho. | Somente positivo passa; exceção é tratada no chamador; regra não exige formulário. |
| A08.05 | Limites da arquitetura | Explique o que precisaria mudar para mover a credencial e a autorização para uma API, e cite uma dificuldade de trocar PostgreSQL por outro banco. | Localiza a fronteira de servidor e não promete que pastas ou interfaces tornam a troca automática. |

<a id="aula-09"></a>

## Aula 09 — Ambiente e solução

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A09.01 | IDE, SDK, runtime e workload | Registre versão da IDE, saída de `dotnet --list-sdks` e finalidade de cada componente. Confira o workload desktop. | Distingue editar/depurar, compilar e executar; registra ambiente realmente disponível. |
| A09.02 | Solução, projeto e framework | Crie solução Windows Forms conforme o ambiente definido pelo professor; identifique no `.csproj` o framework e `UseWindowsForms`. | Compila e abre janela; distingue solução, projeto e .NET Framework. |
| A09.03 | Namespace, `partial` e Designer | Renomeie/mova o login e confira os namespaces das duas partes; explique `.cs`, `.Designer.cs` e `InitializeComponent`. | Designer abre e aplicação compila; as duas partes pertencem à mesma classe. |
| A09.04 | Ponto de entrada e execução | Localize inicialização e `Application.Run`; depure o construtor do login e feche a janela. | Breakpoint é atingido e fechar encerra o aplicativo desta etapa. |
| A09.05 | Organização e entrega de fontes | Organize as cinco pastas e gere cópia de fontes sem `bin`, `obj`, `.vs` ou segredos; confira o que foi incluído. | Outra pessoa identifica o projeto e seus arquivos; não entrega apenas executável. |

<a id="aula-10"></a>

## Aula 10 — Controles e eventos

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A10.01 | Controles, `Name`, `Text` e propriedades | Monte a tela com Labels, TextBoxes e botão; explique três diferenças entre identificador e texto exibido. | Nomes correspondem ao código; rótulos continuam visíveis com campos vazios. |
| A10.02 | Evento `Click`, validação e `return` | Implemente o evento; teste ambos vazios, apenas usuário e ambos preenchidos. | Mensagens corretas; `return` interrompe só o evento; não anuncia autenticação real. |
| A10.03 | Teclado e acessibilidade | Demonstre Tab, Shift+Tab e Enter; registre ordem, contraste e mensagem textual de erro. | Fluxo funciona sem mouse; não depende apenas de cores; Enter aciona uma vez. |
| A10.04 | Ocultação de senha e inscrição de evento | Confira `UseSystemPasswordChar` e a associação única do Click; reabra a janela. | Campo inicia vazio e oculto; explica por que ocultação não protege armazenamento; um clique produz uma mensagem. |

<a id="aula-11"></a>

## Aula 11 — Banco e integridade

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A11.01 | Projeto, branch, banco e isolamento | Registre os nomes do ambiente didático e explique o que deve conferir antes de executar o script 001. | Identifica o ambiente da dupla sem credenciais e não confunde branch com tabela. |
| A11.02 | Tabela, coluna, linha e tipos SQL | Faça dicionário de `usuario` com nome, tipo e finalidade de cada coluna; execute as duas consultas da aula. | Estrutura corresponde ao script e tabela retorna zero usuários nesta etapa. |
| A11.03 | PK, identity, unicidade, nulabilidade e `CHECK` | Em tabela temporária de exercício, sem senhas, reproduza identity/PK, username único/não nulo/minúsculo e perfil restrito. Tente duplicidade, `NULL`, maiúsculas e perfil inválido, uma instrução por vez. | Cada caso é relacionado à restrição correta; não cria contas fictícias com hash inválido na tabela real; distingue `NULL` de vazio. |
| A11.04 | `BEGIN`, `COMMIT` e `ROLLBACK` | Na tabela temporária, abra transação, insira uma linha e reverta; depois repita confirmando. | Linha revertida não permanece; confirmada permanece na sessão; não presume IDs consecutivos. |
| A11.05 | Role, perfil, string de conexão e TLS | Entregue modelo sem valores reais e explique Host, Database, Username, Password e `VerifyFull`; diferencie role PostgreSQL de perfil Caixa. | Não inclui segredo nem desativa verificação de certificado; identifica responsabilidades distintas. |

<a id="aula-12"></a>

## Aula 12 — Assincronia e recursos

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A12.01 | NuGet e dependência | Instale a versão de Npgsql definida para a turma e identifique a referência no projeto. | Compila; distingue pacote de biblioteca e extensão da IDE. |
| A12.02 | Configuração e variável de ambiente | Configure localmente `SUPERMERCADO_DB`; execute com configuração presente e ausente, sem fotografar seu valor. | Ausência produz mensagem compreensível; explica necessidade de reiniciar processos. |
| A12.03 | Data source, pool, conexão e descarte | Desenhe o ciclo de vida de `Banco`, `NpgsqlDataSource`, conexão/comando e descarte em sucesso e falha. | Fonte é reutilizada; recursos de operação são descartados; explica `using` e `await using`. |
| A12.04 | `async`, `await`, `Task` e `async void` | Execute `SELECT 1`; antes da chamada, use temporariamente `await Task.Delay(2000)` para observar a janela responsiva. Remova a demora ao concluir. Explique por que evento usa `async void`. | Janela pode ser movida; retorno da consulta é 1; não usa `.Wait()`/`.Result`; distingue demora simulada de teste de rede. |
| A12.05 | `try/catch/finally` e estado do botão | Teste sucesso e indisponibilidade com orientação do professor; registre estados do botão e conteúdo público da mensagem. | Sem segundo envio pelo botão durante espera; botão reabilitado após ambos os caminhos; não expõe conexão. |

<a id="aula-13"></a>

## Aula 13 — Autenticação

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A13.01 | `enum`, `record` e separação de dados | Crie Perfil, Credencial e Sessao; faça quadro de campos e explique quais chegam à tela. | Sessão não contém senha/hash; enum representa os dois perfis; explica finalidade dos records. |
| A13.02 | Nulabilidade e `Task<T?>` | Consulte usuário inexistente; rastreie o retorno pelo repositório, serviço e evento. | Ausência resulta em login recusado, sem tentar acessar propriedades de `null`. |
| A13.03 | Construtor primário e `sealed` | Reescreva em exercício pequeno um construtor primário como construtor comum e identifique efeito de `sealed`. | Mesma dependência é recebida e usada; não confunde `sealed` com imutabilidade. |
| A13.04 | SQL parametrizado e leitura | Localize o parâmetro de username; teste username contendo apóstrofo na consulta de login. | Texto é dado, não comando SQL; não há concatenação; tentativa inexistente é recusada. |
| A13.05 | Hash, salt e verificação | Em exercício local com PasswordHasher, gere duas representações da mesma senha fictícia e verifique senha correta/incorreta. Registre apenas igualdade/desigualdade e resultados. | Representações diferem e ambas verificam a senha correta; incorreta falha; explica por que não há descriptografia. |
| A13.06 | Rehash e política de senha | Explique os três resultados de verificação, identifique o ramo de rehash e teste criação com 11 e 12 caracteres. | 11 é recusado, 12 atende ao tamanho; atualização ocorre no ramo indicado; não afirma ter provocado rehash sem fazê-lo. |
| A13.07 | Preparação e normalização | Prepare admin em tabela vazia, remova argumento e faça login com username em outra capitalização; tente preparar novamente. | Nova preparação é recusada; login normal funciona; username é normalizado e senha não recebe `Trim`. |
| A13.08 | Evento, retorno e limpeza | Ligue serviço ao login; teste sucesso, senha incorreta e usuário inexistente. | Só sucesso produz sessão/DialogResult OK; duas recusas têm mensagem genérica; senha é limpa e botão reabilitado. |

<a id="aula-14"></a>

## Aula 14 — Sessão e autorização

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A14.01 | Autenticação e autorização | Preencha matriz de ações para Admin e Caixa; explique diferença entre entrar e ter permissão. | Corresponde à política do curso, incluindo admin não vender nessa conta. |
| A14.02 | Permissão no serviço | Com sessão de caixa, chame diretamente uma exigência de administrador, sem depender de botão oculto. | `UnauthorizedAccessException`; nenhuma ação autorizada indevidamente; explica limite do cliente desktop. |
| A14.03 | Herança de Form, layout e lambda | Crie menu mínimo e botão Sair em código; relacione com a criação do botão no Designer e reescreva a lambda como método nomeado. | Clique fecha menu nas duas formas; identifica Form base, painel e evento; não duplica inscrição. |
| A14.04 | `ShowDialog`, `DialogResult`, laço e descarte | Faça três ciclos admin → sair → caixa → sair → login; depois feche login. | Perfil anterior não é reaproveitado; senha vazia; aplicativo termina ao fechar login; explica descarte. |

<a id="aula-15"></a>

## Aula 15 — Produtos e validação

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A15.01 | Modelagem, FK e histórico | Desenhe produto, usuário e movimento com PK/FK e cardinalidades; descreva saldo versus histórico e quantidade com sinal. | Produto tem vários movimentos; movimento identifica produto e responsável; produto nasce com saldo zero. |
| A15.02 | Identificador textual, `numeric` e precisão | Explique código `000123` e `numeric(10,2)`; teste 0; 0,01; 1,999; 99.999.999,99; 100.000.000. | Aceita 0,01 e máximo; rejeita demais no serviço; não arredonda silenciosamente 1,999. |
| A15.03 | `Trim`, tamanhos e ID | Teste código com espaços externos, comprimentos 0/1/30/31, nome com 1/2/120/121 caracteres e IDs -1/0. | Após Trim respeita limites; -1 é recusado, zero representa inclusão. |
| A15.04 | Fake, `Task.CompletedTask` e fronteira da regra | Crie fake com contador de chamadas e argumentos; chame salvar com dados válidos, inválidos e sessão de caixa. | Só dados válidos com admin chegam ao fake; contador comprova ausência de chamada nos demais; explica Task concluída. |
| A15.05 | Validação e integridade | Escolha duas regras e explique o papel do serviço e da restrição de banco em cada uma. | Distingue mensagem amigável de garantia persistente; não afirma que UI é a única proteção. |

<a id="aula-16"></a>

## Aula 16 — Consulta e grade

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A16.01 | `SELECT`, `ILIKE`, parâmetros e curingas | Busque `arr`, código completo, texto ausente, vazio, `%` e apóstrofo; registre expectativas antes de executar. | Distingue pesquisa parcial, ausência e curinga; apóstrofo não altera estrutura SQL. |
| A16.02 | Ordenação, limite e strings multilinha | Localize `ORDER BY`, `LIMIT 200` e string de três aspas; explique a saída de busca vazia e escreva o aviso da tela. | Não promete catálogo completo além do limite; SQL permanece legível e parametrizado. |
| A16.03 | Leitura e mapeamento | Faça tabela coluna SQL → posição → propriedade → tipo C# do método Ler. | Todos os campos correspondem à ordem do SELECT; explica efeito de trocar colunas sem mudar leitor. |
| A16.04 | `DataSource`, `ToList`, `Shown` e função local | Carregue grade ao abrir e ao buscar; explique a função local Carregar e a materialização. | Grade somente leitura e sem linha de inclusão; lista vazia não provoca exceção. |
| A16.05 | Composição, erro e acesso | Passe repositório/serviço até a janela; compare resultado vazio, falha de rede e tentativa de caixa. | Cada caso tem resultado distinto; SQL não está na View; caixa é recusado no serviço. |

<a id="aula-17"></a>

## Aula 17 — Escrita e exclusão lógica

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A17.01 | Inclusão, defaults e `NumericUpDown` | Cadastre produto ativo com preço válido e confira ID, saldo e precisão do controle. | ID gerado, saldo zero, ativo verdadeiro; não permite editar saldo no cadastro. |
| A17.02 | Seleção, edição e identidade | Selecione pelo DataBoundItem, altere nome/preço e confira ID/saldo; tente editar sem seleção. | Mantém identidade e saldo; sem seleção informa necessidade de escolher linha. |
| A17.03 | Estado Novo e linhas afetadas | Após editar, use Novo e inclua outro produto; explique retorno 0 de um UPDATE em produto inativo/inexistente. | Novo não sobrescreve anterior; zero não é apresentado como alteração bem-sucedida. |
| A17.04 | Unicidade e concorrência | Tente código repetido e explique por que SELECT prévio não garante exclusividade entre duas instâncias. | Sem segunda linha; mensagem compreensível; identifica restrição e código `23505`. |
| A17.05 | Inativação, confirmação e persistência | Cancele e depois confirme inativação; feche/reabra a tela e confira registro e código reservado. | Cancelar preserva estado; confirmar muda ativo sem DELETE; alterações persistem. |
| A17.06 | Eventos assíncronos e preservação | Teste nome vazio/preço zero e uma falha de gravação acompanhada pelo professor. | Botão retorna ao estado normal; não limpa dados antes de sucesso; sem falso anúncio de salvamento. |

<a id="aula-18"></a>

## Aula 18 — Reposição

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A18.01 | Incremento atômico | Compare em linha do tempo duas reposições +10 e +3 sobre saldo zero por leitura/soma/gravação e por incremento SQL. Execute as reposições normais. | Explica risco de sobrescrita na primeira estratégia; saldo real final 13. |
| A18.02 | Transação e conexão compartilhada | Marque no método ReporAsync abertura, transação, UPDATE, INSERT e commit; explique falha entre comandos. | Comandos compartilham conexão/transação; não promete confirmação parcial. |
| A18.03 | Auditoria e conciliação | Consulte os dois movimentos e relacione usuário, motivo e quantidade ao saldo. | +10 e +3, saldo 13 e responsável é admin conectado. |
| A18.04 | Validação e autorização | Tente reposição zero/negativa pelo serviço, produto inativo e sessão de caixa. | Nenhum desses casos altera saldo/histórico; registra antes/depois. |
| A18.05 | Seleção e estado de UI | Tente sem seleção e acompanhe clique válido durante espera; confira grade após sucesso. | Sem seleção não repõe; botão desabilitado durante chamada e reabilitado; saldo exibido atualizado. |

<a id="aula-19"></a>

## Aula 19 — Carrinho

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A19.01 | Coleção, subtotal, `Sum` e agrupamento | Adicione arroz duas vezes e feijão uma; compare contas feitas antes com execução; crie uma verificação executável que falhe se total não for 60,30. | Duas linhas, arroz quantidade 2, total 60,30; explica soma de subtotais. |
| A19.02 | Lista privada e leitura | Localize coleção interna, `AsReadOnly` e atualização de grade com `ToList`. Explique quem pode adicionar/remover. | Alterações passam pelos métodos do carrinho; não confunde cópia da lista com cópia profunda. |
| A19.03 | Record, `with` e coleção `[]` | Compare item anterior e resultado de `with { Quantidade = ... }`; reescreva inicialização de lista sem `[]`. | Explica cópia com nova quantidade e equivalência da lista vazia neste contexto. |
| A19.04 | `checked` e excesso de inteiro | Em exercício isolado, some `int.MaxValue` e 1 dentro de `checked`, tratando a exceção. Relacione à quantidade acumulada. | O excesso é detectado; não exige criar estoque enorme no banco. |
| A19.05 | Busca exata e regras de inclusão | Teste código inexistente, parcial, inativo, quantidade zero e acumulada acima do saldo. | Busca por igualdade; recusas preservam carrinho anterior. |
| A19.06 | Remoção, cancelamento e memória | Remova feijão e cancele; consulte estoque antes/depois e explique diferença entre carrinho e reserva. | Totais 51,80 e zero; nenhuma venda ou baixa no banco. |

<a id="aula-20"></a>

## Aula 20 — Venda e concorrência

Realize os testes em ambiente didático preparado pelo professor. Use o [roteiro de integração](VALIDACAO-MANUAL.md) e guarde contagens/saldos iniciais.

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A20.01 | Relacionamentos e snapshot histórico | Desenhe venda/itens/produtos/usuário; confirme venda e depois altere nome/preço do produto. | Item da venda preserva nome/preço originais; distingue cadastro atual e histórico. |
| A20.02 | Pedido e autoridade do preço | Mostre os campos enviados em PedidoItem; compare carrinho vazio, quantidade inválida e pedido válido com preço de tela desatualizado. | Serviço recusa inválidos; preço definitivo vem do banco, não do pedido. |
| A20.03 | `Guid`, UUID e idempotência | Guarde uma chave e repita a venda com mesmos itens, usuário e recebido; depois execute compra nova com outra chave. | Repetição retorna mesmo ID e uma baixa; compra nova usa outra operação. |
| A20.04 | Advisory lock e bloqueio de linhas | Desenhe duas chamadas da mesma chave e duas de chaves distintas para produtos comuns; marque onde aguardam. | Distingue lock da operação e `FOR UPDATE`; explica ordenação crescente e redução de risco de deadlock. |
| A20.05 | Atomicidade, commit e rollback real | Com falha controlada do roteiro após escrita, compare saldo e contagens de venda/itens/movimentos antes/depois; remova a falha ao concluir. | Tudo permanece como antes; identifica ponto da falha; não usa mera validação prévia como prova de rollback de escrita. |
| A20.06 | Concorrência e último item | Em dois caixas, inclua o mesmo produto de estoque 1 e finalize com chaves distintas. | Uma confirmação, uma recusa, saldo zero, uma saída e nenhum cabeçalho órfão. |
| A20.07 | `RETURNING`, total e movimentos | Rastreie uma venda válida desde INSERT até ID retornado e movimento negativo; tente recebido insuficiente. | Não usa MAX(id); soma dos itens confere com total; insuficiência não grava venda/baixa. |
| A20.08 | Mesma chave concorrente | Com apoio do professor, faça duas chamadas da mesma operação com `Task.WhenAll` conforme roteiro. | Mesmo ID e contagens de uma venda; execução pendente é registrada se não houver ambiente. |

<a id="aula-21"></a>

## Aula 21 — Finalização e falhas

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A21.01 | Máquina de estados | Desenhe transições entre montagem, espera, confirmação, recusa e comunicação incerta; indique edição e ações permitidas. | Não permite editar intenção pendente; recusa conhecida e incerteza têm saídas diferentes. |
| A21.02 | Recebido, total definitivo e troco | Venda 60,30 com recebido 100; depois teste recebido insuficiente e mudança de preço antes de confirmar. | Troco 39,70 no primeiro caso; definitivo usa banco; insuficiência preserva carrinho e não baixa saldo. |
| A21.03 | Duplo clique e chave pendente | Execute finalização e acompanhe bloqueio dos controles e renovação do Guid. | Chave muda somente após confirmação; repetir intenção pendente mantém itens e recebido. |
| A21.04 | Falha de comunicação e reconciliação | Simule, com o professor, perda da resposta após confirmação e consulta pela mesma chave; registre separadamente qualquer teste real de rede realizado. | Não anuncia cancelamento sem certeza; consulta evita duplicidade; não confunde simulação com falha real após commit. |
| A21.05 | FormClosing e memória volátil | Tente fechar em montagem e durante espera; explique recuperação após encerramento forçado e escreva consulta por Guid com valor fictício. | Confirma descarte na montagem e bloqueia espera/incerteza; reconhece ausência de recuperação durável automática. |
| A21.06 | Comprovante e escopo | Entregue comprovante fictício com ID, total, recebido, troco e aviso não fiscal. | Valores correspondem à venda; não apresenta simulação como cobrança ou documento fiscal. |

<a id="aula-22"></a>

## Aula 22 — Datas e relatório

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A22.01 | Instante UTC, fuso e cultura | Anote um horário local e equivalente UTC, com fuso/offset usado; explique `DateTimeKind.Utc`, `ToUniversalTime` e `ToLocalTime`. | Representam o mesmo instante; não usa cultura monetária como ajuste de fuso. |
| A22.02 | Intervalo semiaberto | Escolha um dia fixo; classifique registros no início, imediatamente antes do dia seguinte e exatamente no início do dia seguinte. Confira com massa fornecida pelo professor. | Inclui os dois primeiros, exclui o terceiro; não usa 23:59:59 como limite final. |
| A22.03 | DateTimePicker e parâmetros | Mostre transformação da data final inclusiva para início exclusivo do dia seguinte e teste final anterior ao início. | Limites UTC correspondem às datas locais escolhidas; intervalo invertido é recusado. |
| A22.04 | `Sum`, contagem e vazio | Consulte duas vendas conhecidas de 60,30 e 8,50 e compare com SQL de datas fixas; consulte período sem vendas. | Duas vendas e total 68,80 na massa isolada; período vazio tem zero e zero; não conta itens como vendas. |
| A22.05 | Autorização e escala | Chame relatório com sessão de caixa; explique custo de materializar período grande e proponha agregação/paginação. | Caixa recusado no serviço; proposta separa total agregado de detalhes paginados. |

<a id="aula-23"></a>

## Aula 23 — Usuários

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A23.01 | Reuso e composição | Construa formulário usando CriarAsync já existente e conecte-o ao menu administrativo. | View não duplica hash/SQL; serviço e sessão são recebidos por construtor. |
| A23.02 | Username e unicidade | Teste tamanhos 2/3/50/51, acentos, ponto permitido e repetição em maiúsculas de username existente. | Respeita tamanho/caracteres; normalização não cria conta duplicada. |
| A23.03 | Senha, limites e espaços | Teste 11/12/128/129 caracteres e senha fictícia com espaços externos; confirme login sem recorte. | Só tamanhos permitidos cadastram; senha é preservada como digitada e não aparece nas evidências. |
| A23.04 | Enum, ComboBox e perfil inválido | Use DropDownList com Enum.GetValues; tente perfil inválido diretamente no serviço em exercício acompanhado. | UI não aceita texto livre; serviço também rejeita valor fora do enum. |
| A23.05 | Permissão e fluxo completo | Admin cria caixa; novo caixa entra e tenta criar usuário pelo serviço; teste duplicidade na UI. | Perfil correto abre; caixa não cria conta; falha não é apresentada como sucesso e permite correção. |
| A23.06 | Sessão e limites de escopo | Explique por que sair renova a sessão e descreva política que faltaria para revogação imediata ou recuperação de senha. | Distingue implementação atual de evolução; não afirma que revogação já existe. |

<a id="aula-24"></a>

## Aula 24 — Testes e publicação

| ID | Conceito | Atividade e entrega | Critério de aceite |
| --- | --- | --- | --- |
| A24.01 | Compilação, teste de regra, integração e UI | Classifique build, caso com fake, rollback PostgreSQL e navegação por teclado; execute os comandos da aula. | Explica o alcance de cada evidência; registra saída real, sem inferir integração dos fakes. |
| A24.02 | Asserção e regressão | Reaproveite um caso anterior e acrescente uma verificação própria de fronteira; em cópia local, introduza erro controlado, observe falha e restaure. | Caso detecta o erro escolhido e passa após correção; explica o defeito que protege. |
| A24.03 | Teste manual e reprodutibilidade | Em dupla, execute casos de cada perfil, invertendo os papéis; registre preparação, esperado, observado e pendências. | Outra pessoa reproduz; falha ou pendência não recebe status de sucesso. |
| A24.04 | Release, RID, autocontida e pasta | Publique para win-x64 conforme ambiente da turma; compare fonte, build e pasta publicada; explique dependência de runtime e atualização da autocontida. | Mantém todos os arquivos necessários e não confunde publicação com distribuição externa. |
| A24.05 | Execução fora da IDE e configuração | Execute na máquina de laboratório com variável configurada; demonstre login por perfil e venda fictícia. | Executável funciona fora da IDE; sem segredo na pasta; dependência de rede é declarada. |
| A24.06 | Entrega e limites | Entregue fontes/scripts, evidências por ID, identificação da versão, instruções e limites; cada integrante explique uma transação e um caminho View → Service → Repository. | Material organizado e reproduzível; dinheiro simulado, não fiscal, conexão direta e tentativa volátil explicitados. |

Use a [rubrica e as orientações do plano](PLANO-DE-AULAS.md) para a correção. Para retomar um conceito parcial, refaça a atividade com outra entrada e explique por que o resultado mudou.
