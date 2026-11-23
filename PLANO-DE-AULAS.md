# Plano de Ensino — Trilha Unificada de C# e MarketFlow (Visual Studio 2022)

**Carga horária sugerida:** **84 horas** em **24 encontros contínuos e progressivos** (`Aulas 01` a `24`), divididos em **6 módulos**. Cada encontro possui **210 minutos** (3h30), incluindo 15 minutos de intervalo (195 minutos de atividade efetiva).

**Público-alvo:** estudantes iniciantes que estão começando **absolutamente do zero** em programação. A trilha foi unificada em uma única sequência linear — eliminando a dispersão entre "fundamentos separados" e "aulas introdutórias repetidas". Saber usar o Windows é suficiente para iniciar a [Aula 01](01-Primeiro-Programa-e-Tipos.md). *(Em turmas com experiência prévia em lógica e POO, o professor pode aplicar um diagnóstico prático sobre os critérios das Aulas 01–07 antes de iniciar na Aula 08.)*

---

## Princípio Pedagógico: O Guia Orienta e Explica, o Estudante Constrói

1. **Explicação de cada detalhe e dos "Porquês":** cada aula explica o conceito do zero, disseca a sintaxe C# com microexemplos independentes e justifica o motivo técnico das escolhas (`decimal`, `TryParse`, `interface`, `async/await`, SQL parametrizado, hash com salt, transações e bloqueios).
2. **Construção autônoma no Visual Studio 2022:** os capítulos não entregam código pronto do projeto nem instruem o aluno a copiar arquivos de `src/`. Cada aula fornece **orientações passo a passo de construção** e caminhos exatos de menus/atalhos no **Visual Studio 2022**.
3. **Verificação por evidência e explicação:** cada encontro termina com testes de casos normais e inválidos e explicação individual.

---

## Preparação do Ambiente e Escolha de Banco

- Antes da **Aula 01**, o professor prepara o **Visual Studio 2022 Community** com a carga de trabalho *Desenvolvimento para desktop com .NET* e o **.NET SDK** compatível.
- A [Aula 09](09-Configuracao-Ambiente.md) revisita o ambiente ao criar a solução Windows Forms.
- Defina com a turma antes da **Aula 11**: **Neon/PostgreSQL** (trilha principal) ou [**MySQL/MariaDB pelo XAMPP**](INTEGRACAO-MYSQL-XAMPP.md). Os checkpoints e o runner de integração existentes no repositório usam PostgreSQL.

---

## Rotina Padrão de Cada Encontro (210 Minutos)

| Etapa | Minutos | O que se faz |
| --- | ---: | --- |
| Retomada e problema | 15 | Abrir a entrega anterior, revisar dúvidas e apresentar o problema real do dia |
| Explicação guiada | 30 | Conceitos do zero, anatomia da sintaxe e motivos técnicos ("por que usamos assim?") |
| Construção guiada | 60 | Estudantes constroem a primeira parte no Visual Studio 2022 seguindo as orientações |
| Intervalo | 15 | Pausa |
| Prática acompanhada | 60 | Construir a segunda parte, testar entradas inválidas e usar o depurador |
| Verificação e entrega | 30 | Demonstrar o funcionamento, explicar uma escolha técnica e registrar evidências |
| **Total** | **210** | **3h30** |

---

## Sequência das 24 Aulas, Pré-requisitos e Evidências

| Aula | Módulo | Evidência de conclusão construída pelo estudante |
| --- | --- | --- |
| 01 | M1 — Lógica | Primeiro console no VS 2022, os 7 tipos, `const`, atribuição em memória e moeda `:C` |
| 02 | M1 — Lógica | Console validado com `TryParse`/`do-while`, `if`/`switch`, laços e depuração `F9`/`F10` |
| 03 | M1 — Lógica | Funções puras com retorno separadas de `void`, escopo, cópia de valor e depuração `F11` |
| 04 | M2 — POO | `Produto.cs` e `Livro.cs` em arquivos separados; desenho de referências na RAM |
| 05 | M2 — POO | Construtores obrigatórios, `private set`, `throw`/`try-catch` e desafio `SessaoDeEstudo` |
| 06 | M2 — POO | `List<T>`, composição `ItemDeCompra` + `Produto`, subtotal calculado (`=>`) e total `60,30` |
| 07 | M2 — POO | `namespace`, `interface`, injeção pelo construtor, herança (`virtual`/`override`) e polimorfismo |
| 08 | M3 — WinForms | Regra no serviço e substituição real de repositório em memória por contrato |
| 09 | M3 — WinForms | Solução `Supermercado` e `SupermercadoApp` organizados em 5 pastas compilando |
| 10 | M3 — WinForms | Tela `FormLogin` no Designer com `Name`/`Text`, senha oculta, Tab/Enter e validação visual |
| 11 | M4 — Dados/Seg. | Tabela `usuario` criada (Neon ou XAMPP) e restrições testadas em tabela temporária |
| 12 | M4 — Dados/Seg. | `Banco.cs` com pool e botão de diagnóstico `SELECT 1` assíncrono sem travar a janela |
| 13 | M4 — Dados/Seg. | Admin criado com hash+salt; login verificando senha correta, incorreta e inexistente |
| 14 | M4 — Dados/Seg. | Menus separados por perfil, autorização no serviço e ciclo limpo de troca de usuário |
| 15 | M5 — PDV | Script 002 executado; `ProdutoService` validado com Fake em memória contando chamadas |
| 16 | M5 — PDV | Grade `DataGridView` consultando nome/código (`ILIKE`/`LIMIT 200`), inclusive lista vazia |
| 17 | M5 — PDV | Incluir, editar por `DataBoundItem`, recusar duplicidade (`23505`) e inativar logicamente |
| 18 | M5 — PDV | Reposição com incremento atômico no SQL e histórico gravados na mesma transação |
| 19 | M5 — PDV | Carrinho em memória agrupando itens (`AsReadOnly`, `with`, `checked`) sem alterar banco |
| 20 | M5 — PDV | Script 003; venda atômica com `Guid`, `FOR UPDATE`, disputa pelo último item e rollback real |
| 21 | M5 — PDV | Pagamento simulado, troco, `FormClosing` e reconciliação de tentativa pendente |
| 22 | M6 — Entrega | Relatório por intervalo semiaberto UTC com conferência de fronteiras e totalização |
| 23 | M6 — Entrega | Cadastro de contas pela UI com `ComboBox`/`Enum`; novo caixa entra e não cria usuários |
| 24 | M6 — Entrega | Projeto de verificações com Fakes, roteiro manual em dupla e pasta publicada fora da IDE |

---

## Uso da Referência Docente e dos Checkpoints

- A pasta [`src/`](src/README.md) contém a implementação completa de referência **para uso exclusivo do professor** conferir nomes, contratos e SQL, além de sustentar os testes automatizados. **Não instrua o estudante a copiar arquivos de `src/`.**
- Para recuperação pontual de estudantes, o professor pode gerar os checkpoints independentes seguindo o [**Apoio ao Professor**](ensino/README.md) com `tools/Gerar-Checkpoints.ps1`.

---

## Avaliação Formativa e Rubrica Operacional

Utilize o [**Caderno de Atividades por Conceito (`A01`–`A24`)**](ATIVIDADES-POR-CONCEITO.md) e o [**Caderno de Evidências**](CADERNO-DE-EVIDENCIAS.md). A correção registra por ID o status (**atingiu**, **parcial** ou **pendente**) com ação específica de recuperação. Em duplas, a explicação é sempre **individual**.

| Dimensão e peso | Nível 0 | Nível 1 | Nível 2 | Nível 3 |
| --- | --- | --- | --- | --- |
| Funcionalidade — 40% | Ausente ou não compila/executa | Execução parcial | Caso normal correto | Casos normais e fronteiras do enunciado corretos |
| Explicação — 25% | Não identifica o conceito | Repete definição sem ligar ao código | Localiza e explica no próprio código | Prevê efeito de uma mudança e justifica o motivo técnico |
| Dados e erros — 25% | Não valida entradas | Demonstra só caminho feliz | Recusa entrada inválida prevista | Recusa inválida e comprova preservação do estado |
| Organização e evidências — 10% | Sem entrega verificável | Arquivos sem identificação dos casos | IDs, arquivos e resultados organizados | Outra pessoa reproduz com as instruções registradas |

Cálculo percentual: para cada dimensão, `peso × nível / 3`, somando as quatro parcelas.

**Marcos de verificação individual:** Aula 07 (fechamento de Fundamentos e POO), Aula 10 (primeira interface), Aula 14 (acesso separado por perfil), Aula 18 (gestão de produtos e estoque), Aula 21 (venda completa no caixa) e Aula 24 (entrega e publicação). Ao fechar cada marco, oriente a turma a guardar uma cópia limpa do projeto sem `bin`, `obj`, `.vs` e sem credenciais.
