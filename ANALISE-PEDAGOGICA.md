# Análise pedagógica do roteiro de C#

Análise inicial em 16/11/2026 dos 20 capítulos, plano de aulas, caderno de evidências, instruções da referência, scripts e roteiro de validação. A leitura inicial do código foi pontual; este documento não constitui uma auditoria completa do aplicativo. As tabelas abaixo registram os achados originais; a seção 6 registra a implementação posterior, solicitada na mesma data.

O roteiro tem um projeto integrador coerente e entregas verificáveis por aula. A principal melhoria é tornar verificável a aprendizagem de **cada conceito**, pois concluir uma tela ou copiar um serviço não demonstra, por si só, domínio dos elementos utilizados. O [caderno de atividades](ATIVIDADES-POR-CONCEITO.md) acompanha esta análise e transforma os conceitos em tarefas entregáveis, com critérios de aceite.

## 1. Pontos fortes a preservar

| Evidência no material | Valor pedagógico |
| --- | --- |
| Console → objetos → responsabilidades → interface → persistência → venda | O mesmo problema ganha complexidade gradualmente. |
| Projeto único, interfaces pequenas e composição manual na aula 04 | Permite ensinar responsabilidades antes de contêineres, ORMs e múltiplos projetos. |
| Casos normais e inválidos desde as primeiras entregas | Estimula verificar comportamento, além de produzir código. |
| Dinheiro em `decimal`, SQL parametrizado e hash de senha | Introduz decisões técnicas justificadas dentro do projeto. |
| Saldo separado do histórico; venda transacional | Relaciona programação à consistência dos dados. |
| Aula 17 distingue resultado incerto de venda cancelada | Expõe uma dificuldade real de sistemas com rede. |
| `VALIDACAO.md` distingue execução local de integração pendente | Evita confundir compilação e fakes com validação no PostgreSQL. |
| Escopo e limitações explícitos | Mantém o projeto concluível e oferece caminhos de continuidade. |

## 2. Melhorias por prioridade

As prioridades abaixo são de planejamento pedagógico: P0 antes da próxima oferta, P1 durante a preparação da turma e P2 após observar a primeira aplicação.

| Prioridade | Local e achado | Melhoria concreta | Como verificar |
| --- | --- | --- | --- |
| P0 | Aula 01: apresenta constantes, conversões, `switch`, `for` e `while`; a entrega final concentra cálculo, `if` e `foreach`. | Exigir pequenas evidências específicas, reunidas no mesmo programa. | Atividades A01.01–A01.08 com resultados identificados. |
| P0 | Aula 03: herança e polimorfismo ficam principalmente em exemplos e respostas; o subtotal não os demonstra. | Criar uma hierarquia pequena e executar duas implementações pela mesma interface. | A03.05 e A03.06; uma declaração isolada não basta. |
| P0 | Aula 09: hash, salt, `record`, enum, nulabilidade, construtor primário, quatro métodos do repositório e evento assíncrono aparecem no mesmo encontro. | Separar checkpoints: tipos e ausência de usuário; consulta; verificação; ligação da tela. Fornecer infraestrutura inicial ao grupo que precisar. | Compilar e explicar cada checkpoint antes de acrescentar o seguinte. |
| P0 | Aula 16: transação, advisory lock, bloqueio de linhas, histórico e idempotência competem por 30 minutos de explicação. | Usar rastreamento em papel antes do código; professor prepara dois caixas e falha controlada. | A16.03–A16.08, distinguindo previsões de resultados executados. |
| P0 | README e aula 05 tratam o fim do suporte do .NET 8 como futuro. | Atualizar o aviso temporal e planejar migração conjunta de IDE, SDK, dependências e publicação; validar a solução migrada antes de mudar a base da turma. | Ambiente suportado documentado e comandos essenciais novamente executados após migração. |
| P1 | Aula 02 usa `do/while`, `out`, `!`, `\|\|` e `??` na entrega sem explicação proporcional. | Pedir rastreamento da repetição e diferenciar ausência, falha de conversão e valor fora da regra. | A02.02–A02.04. |
| P1 | Aula 03 usa `throw`; aula 04 já usa `try/catch`; aula 08 acrescenta `finally` e descarte. | Introduzir o fluxo de exceções em etapas, com previsão do que executa em cada caminho. | A03.04, A04.04 e A08.04. |
| P1 | Aulas 07 e 11 explicam restrições, mas copiar o script inteiro pode ocultar compreensão. | Produzir dicionário de dados e desenho de relações; testar restrições em tabela temporária de laboratório. | A07.02–A07.04 e A11.01. |
| P1 | Aula 04: a verificação final troca preço da lista, embora a entrega anuncie troca de implementação. | Criar segundo repositório implementando o mesmo contrato e trocar só a composição. | A04.03 exige serviço inalterado. |
| P1 | Aulas 05/06 usam Designer; aula 10 passa a layout em código. | Tornar explícito o equivalente entre controle, propriedade e evento nas duas abordagens. | A10.03 reproduz um botão pelos dois caminhos. |
| P1 | Aulas 10–12 dependem de contas e botões temporários; o código final contém recursos futuros. | Preparar uma versão compilável de início/fim por aula e lista de métodos que entram. | Professor abre cada versão em ambiente limpo; não basta desabilitar menus da referência final. |
| P1 | Evidências atuais são por aula, sem ID de conceito ou nível de autonomia. | Relacionar atividade, arquivo/método, caso, resultado e explicação individual. | Modelo atualizado de `CADERNO-DE-EVIDENCIAS.md`. |
| P1 | Testes automáticos aparecem como unidade própria apenas na aula 20, embora verificações manuais já existam. | Guardar casos reproduzíveis desde a aula 02 e introduzir uma asserção executável na aula 11 ou 15. | A11.04, A15.01 e A20.02 reaproveitam o mesmo caso. |
| P1 | Aula 16 manda demonstrar rollback, mas o caso de estoque insuficiente descrito falha antes de escrever. | Exigir evidência de falha após escrita, conforme roteiro já disponível, com preparação do professor. | Contagens e saldo antes/depois em A16.05; prever rollback não comprova execução. |
| P2 | Aula 18 depende do fuso do Windows e tem teste de limites descrito sem massa pronta. | Fornecer registros fictícios nos limites com datas e fuso explícitos. | A18.01–A18.03 inclui início, fim e instante imediatamente anterior ao fim. |
| P2 | Rubrica percentual não descreve níveis observáveis. | Acrescentar níveis e devolutiva por atividade, preservando os pesos. | Usar a rubrica operacional abaixo. |

Em 16/11/2026, o .NET 8 já está fora de suporte desde 10/11/2026, conforme a [política oficial da Microsoft](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core), consultada nesta análise. Na implementação posterior, código, SDK, dependências e documentação passaram para .NET 10 LTS; a validação local está registrada separadamente.

## 3. Diagnóstico e ajuste por aula

| Aula | Diagnóstico | Ajuste recomendado e evidência |
| --- | --- | --- |
| 01 | Muitos conceitos para uma primeira exposição; exemplos de balança/cartão excedem o escopo implementado. | Tratar esses exemplos como simulações e usar minitarefas com tipos, operadores, seleção e cada laço. Não exigir integrações reais. |
| 02 | A entrada insegura ajuda a observar um problema, mas o gabarito do estoque mantém `Convert`. | Comparar a versão que falha com a versão final validada, incluindo cultura e breakpoint. |
| 03 | Analogias ajudam, mas os quatro pilares chegam juntos; propriedades de referência sem inicialização podem gerar avisos de nulabilidade. | Inicializar textos dos exercícios, diferenciar propriedade de campo e comprovar cada pilar com execução ou decisão de modelagem. |
| 04 | Exemplo acessível, mas `ValidarPreco` fica separado de `Executar`. | Explicar que é um exercício de responsabilidade; pedir rastreamento e substituição real da dependência. Depois integrar validação a um caso de uso de escrita. |
| 05 | Pré-instalação pelo professor resolve o início, mas a reprodução ainda depende do ambiente legado. | Inventário de ferramentas e checklist de compilação com o SDK efetivamente usado. |
| 06 | Há bons critérios de teclado e mensagens. | Transformar as verificações em roteiro registrado e explicar `return` e inscrição única do evento. |
| 07 | Boa distinção entre role do banco e perfil do aplicativo. | Acrescentar dicionário de dados, violação de restrições em tabela temporária e diagrama. |
| 08 | Explica pool e descarte, porém muitos mecanismos são copiados de uma vez. | Pedir mapa de ciclo de vida e rastrear sucesso/falha/finally; usar demora simulada para observar responsividade. |
| 09 | Maior salto inicial de sintaxe e dependências. | Fazer atividades curtas de leitura antes da implementação; comprovar hash/salt sem expor senhas nem hashes completos. |
| 10 | Boa matriz de permissões; preparação do caixa exige intervenção temporária. | Professor prepara a conta antes da prática; aluno demonstra negação no serviço e ciclo de sessão. |
| 11 | Excelente separação entre validação e interface. | Quantificar chamadas ao fake e testar fronteiras de tamanho, valor e casas decimais. |
| 12 | Consulta parametrizada e limite são explícitos. | Acrescentar leitura de `%`, mapeamento das colunas e distinção entre vazio e erro. |
| 13 | Fluxo CRUD inclui inativação e concorrência de duplicidade. | Exigir sequência reproduzível com ID e saldo, incluindo cancelar confirmação e falta de seleção. |
| 14 | Conecta saldo e histórico com clareza. | Pedir conciliação matemática e simular duas reposições para discutir perda de atualização. |
| 15 | Boa oportunidade para avaliar regras sem banco. | Transformar contas prévias em asserções e justificar `with`, lista privada e `checked`. |
| 16 | Conceitos centrais, porém densos e dependentes de laboratório. | Professor fornece preparação e guia do repositório em blocos. Aluno entrega linha do tempo e evidências de atomicidade, concorrência e repetição. |
| 17 | Estados e resultado incerto são bem descritos. | Avaliar transições explicitamente; separar simulação didática de falha real após commit. |
| 18 | Bom intervalo semiaberto e exemplos monetários. | Massa de fronteira e conversão com horário/fuso registrados; contar vendas, não itens. |
| 19 | Reutiliza serviço, evitando duplicação de hash. | Testar limites de username/senha/perfil e login da conta criada; demonstrar rejeição sem menu. |
| 20 | Diferencia tipos de teste e publicação. | Estudante escreve ao menos um caso próprio e entrega instruções reproduzíveis de execução e limitações. |

## 4. Como caber nas 70 horas

O caderno complementar organiza **uma entrega por aula**, com várias evidências por conceito. As atividades substituem verificações dispersas e exercícios repetidos; não devem ser somadas integralmente à prática existente. O tempo real depende de uma aplicação piloto.

Mantenha a tabela de 210 minutos. Use os 60 minutos de construção para as primeiras atividades com apoio; os 60 de prática para variar entradas e produzir evidências; os 30 finais para demonstração e explicação individual. Tarefas de classificação ou previsão podem acontecer dentro da explicação de 30 minutos. Não peça um relatório separado para cada linha: vários IDs podem apontar para o mesmo arquivo ou execução.

Nas aulas 09 e 16, prepare contas, banco, checkpoints e falhas antes do encontro. O professor pode fornecer os blocos de infraestrutura já explicados e avaliar alterações, rastreamento e testes dos conceitos centrais. Se a turma não alcançar esses objetivos em 210 minutos, registre a insuficiência e amplie o calendário ou reduza funcionalidades; não transfira atividades obrigatórias silenciosamente para casa.

Antes da aula 01, faça diagnóstico curto sem nota: criar pasta, abrir projeto, executar exemplo e explicar entrada/saída. No início de cada aula, use uma pergunta de recuperação da anterior. Ao final dos marcos 06, 10, 14, 17 e 20, peça que cada integrante explique uma atividade escolhida pelo professor.

## 5. Rubrica operacional

Preservam-se os pesos do plano: funcionalidade 40%, explicação 25%, dados/erros 25%, organização/evidências 10%. Para cada dimensão, atribua nível de 0 a 3 e calcule `peso × nível / 3`; some para uma nota percentual. Isso permite entregar um único projeto sem perder a avaliação por conceito.

| Dimensão | 0 | 1 | 2 | 3 |
| --- | --- | --- | --- | --- |
| Funcionalidade | Ausente ou não executa | Execução parcial | Caso normal correto | Casos normais e limites do enunciado corretos |
| Explicação | Não identifica o conceito | Repete definição sem relacionar | Localiza e explica no próprio código | Prevê uma mudança e justifica o resultado |
| Dados e erros | Não verifica | Só demonstra caminho feliz | Verifica uma recusa prevista | Verifica recusa e preservação do estado relevante |
| Organização e evidências | Sem entrega verificável | Arquivos sem identificação do caso | IDs, arquivos e resultados identificados | Outra pessoa reproduz com a preparação descrita |

Para atividades conceituais sem entrada inválida, avalie classificação, contraexemplo e justificativa no lugar de tratamento de execução. Em dupla, o projeto pode ser compartilhado, mas a explicação é individual. Registre por ID: atingiu, parcial ou pendente, com uma ação de recuperação específica. Uma nota global não substitui esse mapa de aprendizagem.

## 6. Implementação das melhorias

As 114 [atividades por conceito](ATIVIDADES-POR-CONCEITO.md) foram integradas ao plano, aos capítulos e ao caderno de evidências. Além da lista de atividades, os capítulos agora incluem explicações dos mecanismos antes subexplicados, rastreamento de decisões, substituição real de dependência, verificações executáveis e evidência obrigatória de rollback após escrita.

| Frente | Implementação |
| --- | --- |
| Cobertura por conceito | 114 IDs com enunciado, evidência e aceite; aula 01 cobra todos os blocos e aula 03 executa herança/polimorfismo. |
| Pré-requisitos de sintaxe | Explicações de `??`, `out`, operadores, `do/while`, exceções e descarte; propriedades textuais inicializadas. |
| Arquitetura | Segunda implementação de IConsultaProdutos e troca apenas na composição da aula 04. |
| Aulas densas | Quatro paradas e preparação docente para aulas 09 e 16. |
| Recuperação | Gerador de 21 versões independentes, inventário e assinaturas públicas por etapa; compilação automatizada. |
| Designer e código | Quadro de equivalência e método nomeado para evento na aula 10. |
| Preparação de contas | `--preparar-caixa` exige administrador e evita botões provisórios na turma. |
| Modelagem e integridade | Dicionário, relações e laboratório temporário de restrições na aula 07. |
| Fronteiras e avaliação | Massas SQL fixas na aula 18, rubrica operacional no plano e registro por ID. |
| Testes progressivos | Asserções nas aulas 11/15; suíte local ampliada e simulação de resposta perdida. |
| Integração | Runner opt-in em esquema isolado para concorrência, idempotência, rollback, histórico e datas. |
| Ambiente | .NET 10 LTS, SDK 10.0.404, VS 2026 18.11, Npgsql 10.0.3 e Identity.Core 10.0.15. |

O [apoio ao professor](ensino/README.md) explica geração e uso. O [registro desta revisão](VALIDACAO-MELHORIAS.md) distingue o que foi executado localmente do que depende de PostgreSQL e de operação manual. Os resultados históricos de `VALIDACAO.md` continuam pertencendo à entrega anterior. O ajuste de ritmo após aplicação piloto permanece uma ação da oferta real da turma.
