# Apoio ao Professor e Pontos de Recuperação

Este material orienta a condução da turma na **trilha unificada de 25 aulas (`01` a `25`)** no **Visual Studio 2022**, em conjunto com o [Caderno de Atividades por Conceito](../ATIVIDADES-POR-CONCEITO.md).

## Condução Pedagógica: Orientar em vez de Mandar Copiar

Os capítulos foram unificados em uma sequência progressiva única — do zero absoluto (**Aulas 01–07**, cobrindo console, lógica, depuração, métodos e POO completa) até a arquitetura em camadas, o catálogo prático de componentes WFA (**Aula 10**) e o sistema Windows Forms MarketFlow (**Aulas 08–25**).

- **Não compartilhe `src/SupermercadoApp` como material de cópia.** A pasta `src/` serve exclusivamente para conferência docente, preparação de dados de laboratório e validação automatizada.
- **Estimule a investigação de erros:** quando surgir uma mensagem vermelha na **Lista de Erros** do Visual Studio 2022, peça primeiro que o estudante leia o código e a descrição do erro e tente localizar a linha antes de intervir.
- **Peça o "porquê":** ao acompanhar uma dupla, pergunte *por que* o preço usa `decimal`, *por que* aquele campo é `private readonly` ou *por que* a consulta usa `@parametro`.

---

## Gerar os Checkpoints de Recuperação

Use os checkpoints apenas como ponto de partida de **recuperação** para estudantes que faltaram ou acumularam erros que impediriam acompanhar a aula do dia. Cada checkpoint contém apenas os arquivos, métodos e controles da etapa correspondente.

Na raiz do repositório, usando PowerShell e o SDK configurado em `global.json`:

```powershell
powershell -ExecutionPolicy Bypass -File tools/Gerar-Checkpoints.ps1
powershell -ExecutionPolicy Bypass -File tools/Validar-Material.ps1
```

As versões ficam em `artifacts/checkpoints/` (checkpoints `00` a `20` correspondendo ao console inicial `00`, revisão aplicada `01–03`, arquitetura `04` = Aula 08, até entrega final `20` = Aula 25). O gerador recusa sobrescrever uma pasta existente para não apagar trabalhos; passe `-Destino artifacts/outro-nome` se precisar gerar outra série.

`Validar-Material.ps1` verifica links, âncoras, tabelas Markdown, blocos de código, soma de 210 minutos nas 25 aulas, unicidade de todos os IDs `A01`–`A25`, gera e compila os checkpoints em `Release` e executa as verificações com Fakes (sem exigir banco de dados ou credenciais).

---

## Preparação Docente Antes dos Encontros

| Antes da aula | Preparação do professor | Evidência da preparação |
| --- | --- | --- |
| 01 | Conferir Visual Studio 2022 instalado com workload Desktop .NET em todas as máquinas. | Projeto console criado e executado com `Ctrl+F5` na máquina padrão do laboratório. |
| 09 | Conferir criação de projeto Windows Forms (.NET moderno, sem `.NET Framework`) e acesso ao `nuget.org` (ou cache local). | Solução `Supermercado` compilada sem erros no ambiente da turma. |
| 10 | Conferir abertura da Caixa de Ferramentas (`Ctrl+Alt+X`) e do Designer WFA para o laboratório de componentes e Mini-CRUD em memória. | Projeto `LabComponentesWFA` compilando e demonstrando `DataGridView`, `NumericUpDown`, `ComboBox` e `DateTimePicker`. |
| 12 | Separar um banco ou branch vazio por dupla no Neon (ou banco dedicado por dupla no XAMPP). | Lista de ambientes da turma registrada sem expor senhas. |
| 14 | Conferir script `001-usuarios.sql` aplicado e variável `SUPERMERCADO_DB` funcionando. | Diagnóstico `SELECT 1` validado antes de iniciar autenticação. |
| 15 | Preparar uma conta `Caixa` de laboratório para cada dupla usando `--preparar-caixa` (ferramenta administrativa explicada abaixo). | Cada perfil abre sua janela correspondente; nenhuma senha anotada no código. |
| 16 | Conferir script `002-produtos.sql` pronto para execução na aula. | Validação com Fake em memória rejeitando preço inválido sem gravar. |
| 17 | Cadastrar dois produtos fictícios (arroz e feijão) no banco didático pela referência docente antes da aula de consulta. | Busca parcial na grade retorna os produtos fictícios esperados. |
| 21 | Preparar script `003-vendas.sql`, duas contas de caixa, produto com estoque `1` e branch isolada para o teste de rollback com gatilho. | Contagens e saldos iniciais anotados; gatilho ausente antes do experimento. |
| 23 | Preparar a massa de fronteira (`database/laboratorio/18-fronteiras.sql`) e conferir fuso horário de Brasília no Windows. | Contagem e soma conferem com o SQL de referência de datas fixas. |

### Ferramenta docente `--preparar-caixa`

Para criar uma conta de `Caixa` antes da Aula 24 sem adicionar botões provisórios ao projeto dos estudantes, o professor pode executar a referência docente com o argumento `--preparar-caixa`. O programa exige login de um `Administrador` válido e abre o formulário de cadastro para criar a conta de teste da dupla. Remova o argumento após a preparação.

---

## Condução das Aulas Mais Densas (14 e 21)

- **Aula 14 (Autenticação com Hash):** conduza em quatro paradas obrigatórias com compilação intermediária: (1) modelos `Perfil`, `Credencial`, `Sessao` e ausência de usuário; (2) contrato e repositório SQL parametrizado; (3) serviço de hash com `PasswordHasher` e preparação do primeiro admin; (4) ligação assíncrona no `FormLogin` e `Program`.
- **Aula 21 (Venda com Transação e Concorrência):** nos 30 minutos de explicação, desenhe no quadro a linha do tempo de dois caixas disputando a última unidade e o risco de deadlock. Nos 60 minutos guiados, oriente a construção do esquema, modelos e serviço; nos 60 de prática, oriente o repositório em blocos e conduza os ensaios de concorrência, repetição de `Guid` e rollback após escrita. Se a rede ou o tempo impedirem um ensaio, registre como *pendente* para recuperação.

---

## Devolutiva e Recuperação

Inicie cada aula com uma pergunta rápida de retomada sobre o porquê de uma decisão da aula anterior. Use a rubrica do [Plano de Aulas](../PLANO-DE-AULAS.md) e registre por ID (*atingiu*, *parcial* ou *pendente*) com uma ação concreta de recuperação. Nos marcos **07, 10, 11, 15, 19, 22 e 25**, peça que cada estudante explique individualmente um trecho que construiu.
