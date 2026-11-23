# Plano de Ensino: Fundamentos de C# e Projeto MarketFlow (Visual Studio 2022)

**Carga horária sugerida para iniciantes:** **98 horas** — 28 horas de preparação em 8 encontros (`F00` a `F07`), seguidas de 70 horas do projeto em 20 encontros (`Aulas 01` a `20`). Cada encontro possui **210 minutos** (3h30), incluindo 15 minutos de intervalo (195 minutos de atividade efetiva). Ajuste o calendário institucional caso a instituição contabilize o intervalo à parte.

**Público-alvo:** estudantes iniciantes que estão começando do zero em programação. Quem ainda não programa ou não domina orientação a objetos deve iniciar obrigatoriamente pelo [**Percurso Preparatório F00–F07**](fundamentos/README.md). Saber usar o Windows é suficiente para começar em `F00`, mas não para pular diretamente para o projeto Windows Forms.

---

## Princípio Pedagógico: O Guia Orienta e Explica, o Estudante Constrói

Este plano foi estruturado para evitar a aprendizagem passiva por cópia de código:

1. **Explicação de cada detalhe e dos "Porquês":** antes de pedir que o estudante programe, cada capítulo apresenta o problema real, explica a sintaxe C# com microexemplos independentes e justifica o motivo técnico das escolhas (ex.: por que usar `decimal`, `TryParse`, `interface`, `async/await`, SQL parametrizado, hash com salt e transações).
2. **Construção autônoma no Visual Studio 2022:** os capítulos não entregam o código pronto da aplicação nem instruem o estudante a copiar arquivos de `src/`. Em vez disso, fornecem **orientações passo a passo de construção** (especificações de arquivos, contratos, passos lógicos e dicas de sintaxe) e caminhos exatos de menus/atalhos no **Visual Studio 2022**.
3. **Verificação por evidência e explicação:** uma tela funcionando não basta se o aluno não consegue explicar o que escreveu. Cada encontro encerra com testes de casos normais e inválidos e explicação individual.

---

## Preparação do Ambiente

- O professor prepara o **Visual Studio 2022 Community** (workload *Desenvolvimento para desktop com .NET*) e o **.NET SDK** compatível antes do encontro `F00`.
- A [Aula 05](05-Configuracao-Ambiente.md) revisita o ambiente ao iniciar a solução Windows Forms.
- Contas de banco na nuvem (**Neon/PostgreSQL**) ou ambiente local (**XAMPP com MySQL/MariaDB**) só entram a partir da [Aula 07](07-Criando-Banco-Dados-Neon.md).

---

## Etapa Preparatória — Antes das 20 Aulas (F00–F07)

Siga a [sequência de oito encontros](fundamentos/README.md#sequência-e-tempo):

- `F00`–`F02`: primeiro programa no Visual Studio 2022, valores, variáveis, entrada validada, decisões, repetições e depuração com Breakpoint.
- `F03`: métodos, parâmetros, retorno e escopo.
- `F04`–`F07`: classes, objetos, referências na memória, construtores, encapsulamento, composição, coleções, interfaces e polimorfismo.

Não antecipe banco de dados, telas gráficas ou repositórios antes de o estudante dominar objetos, construtores e contratos no console. Em turmas com experiência prévia, aplique os critérios de `F00–F07` como diagnóstico prático antes de avançar para a Aula 01.

---

## Opção de Banco de Dados

Defina com a turma antes da Aula 07: **Neon/PostgreSQL** (trilha principal) ou [**MySQL/MariaDB pelo XAMPP**](INTEGRACAO-MYSQL-XAMPP.md). A trilha local substitui a preparação das aulas 07 e 08 e orienta as adaptações nos repositórios das aulas 09 a 20, mantendo integralmente os mesmos 20 encontros, conceitos e critérios de aceite. Os checkpoints automatizados e o runner de integração existentes no repositório são da referência PostgreSQL.

---

## Rotina Padrão de Cada Encontro (210 Minutos)

| Etapa | Minutos | O que se faz |
| --- | ---: | --- |
| Retomada e problema | 15 | Abrir a entrega anterior, revisar uma dúvida e apresentar o problema real do dia |
| Explicação guiada | 30 | Conceitos do zero, anatomia da sintaxe e motivos técnicos ("por que usamos assim?") |
| Construção guiada | 60 | Estudantes constroem a primeira parte no Visual Studio 2022 seguindo as orientações |
| Intervalo | 15 | Pausa |
| Prática acompanhada | 60 | Construir a segunda parte, testar entradas inválidas e usar o depurador |
| Verificação e entrega | 30 | Demonstrar o funcionamento, explicar uma escolha técnica e registrar evidências |
| **Total** | **210** | **3h30** |

Desafios extras são opcionais e não bloqueiam a aula seguinte. Caso um estudante acumule erros estruturais que impeçam iniciar a aula seguinte, o professor fornece um checkpoint funcional da etapa anterior como ponto de recuperação, sem transformar a aula inteira em cópia.

---

## Pré-requisitos e Evidências por Aula

| Aula | Ponto de partida | Evidência de conclusão construída pelo estudante |
| --- | --- | --- |
| 01 | Laboratório com VS 2022 e F00–F07 | Programa de console calculando compra, desconto, menu e laços |
| 02 | 01 | Console resistente a texto/valores inválidos com depuração demonstrada |
| 03 | 02 | Objetos independentes, encapsulamento, subtotal, herança e polimorfismo |
| 04 | 03 | Regras no serviço e substituição real de repositório em memória por contrato |
| 05 | 04 | Solução `Supermercado` com `SupermercadoApp` organizada em 5 pastas compilando |
| 06 | 05 | Tela de login no Designer com `Name`/`Text`, senha oculta, Tab/Enter e validação visual |
| 07 | 06 + Neon ou XAMPP preparado | Tabela `usuario` criada com restrições testadas em tabela temporária |
| 08 | 07 | `Banco.cs` com pool e botão de diagnóstico `SELECT 1` assíncrono sem travar tela |
| 09 | 08 | Admin preparado com hash+salt; login verificando senha correta, incorreta e ausente |
| 10 | 09 | Menus separados por perfil, autorização no serviço e ciclo limpo de troca de usuário |
| 11 | 10 | Script 002 executado; `ProdutoService` validado com repositório Fake em memória |
| 12 | 11 | Busca parametrizada em grade somente leitura com tratamento de lista vazia |
| 13 | 12 | Inclusão, edição, recusa de duplicidade (`23505`) e inativação lógica com confirmação |
| 14 | 13 | Reposição atualizando saldo e registrando movimento na mesma transação |
| 15 | 14 | Carrinho em memória agrupando itens, calculando total e validando estoque previsto |
| 16 | 15 | Script 003; venda atômica com bloqueio de concorrência, idempotência e rollback testado |
| 17 | 16 | Pagamento em dinheiro simulado, cálculo de troco e proteção contra falha pós-commit |
| 18 | 17 | Relatório por intervalo semiaberto UTC com conferência de fronteiras e totalização |
| 19 | 18 | Cadastro de usuários pela interface administrativa reutilizando `AutenticacaoService` |
| 20 | 19 | Verificações com Fakes, roteiro manual em dupla e pasta executável publicada |

---

## Uso da Referência Docente e dos Checkpoints

- A pasta [`src/`](src/README.md) contém a implementação completa de referência **para o professor** conferir nomes, contratos e SQL, além de sustentar os testes automatizados do repositório. **Não instrua o estudante a copiar arquivos diretamente de `src/`.**
- Em cada aula, o estudante declara **apenas** os modelos, métodos de interface e serviços necessários para aquela etapa, mantendo o projeto sempre compilável e enxuto.
- Para recuperação pontual de estudantes, o professor pode gerar os 21 checkpoints independentes seguindo o [**Apoio ao Professor**](ensino/README.md) com `tools/Gerar-Checkpoints.ps1`.

---

## Avaliação Formativa e Rubrica Operacional

Utilize o [**Caderno de Atividades por Conceito**](ATIVIDADES-POR-CONCEITO.md) (114 IDs distribuídos nas 20 aulas) e o [**Caderno de Evidências**](CADERNO-DE-EVIDENCIAS.md). Vários IDs podem ser comprovados no mesmo programa e na mesma demonstração, sem duplicar tarefas.

A correção registra por ID o status (**atingiu**, **parcial** ou **pendente**) com uma ação específica de recuperação. Em trabalhos em dupla, o código pode ser compartilhado, mas a explicação é sempre **individual**.

| Dimensão e peso | Nível 0 | Nível 1 | Nível 2 | Nível 3 |
| --- | --- | --- | --- | --- |
| Funcionalidade — 40% | Ausente ou não compila/executa | Execução parcial | Caso normal correto | Casos normais e fronteiras do enunciado corretos |
| Explicação — 25% | Não identifica o conceito | Repete definição sem ligar ao código | Localiza e explica no próprio código | Prevê efeito de uma mudança e justifica o motivo técnico |
| Dados e erros — 25% | Não valida entradas | Demonstra só caminho feliz | Recusa entrada inválida prevista | Recusa inválida e comprova preservação do estado |
| Organização e evidências — 10% | Sem entrega verificável | Arquivos sem identificação dos casos | IDs, arquivos e resultados organizados | Outra pessoa reproduz com as instruções registradas |

Cálculo da nota percentual: para cada dimensão, calcule `peso × nível / 3` e some os quatro resultados.

**Marcos de verificação individual:** Aula 06 (primeira tela), Aula 10 (autenticação e perfis), Aula 14 (gestão de produtos e estoque), Aula 17 (venda completa no caixa) e Aula 20 (entrega e publicação). Ao fechar cada marco, oriente a turma a guardar uma cópia limpa do projeto sem as pastas `bin`, `obj`, `.vs` e sem credenciais.
