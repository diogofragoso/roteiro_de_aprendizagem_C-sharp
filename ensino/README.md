# Apoio ao Professor e Pontos de Recuperação

Este material orienta a condução da turma em conjunto com o [Caderno de Atividades por Conceito](../ATIVIDADES-POR-CONCEITO.md) e os capítulos guiados de construção no **Visual Studio 2022**.

## Condução Pedagógica: Orientar em vez de Mandar Copiar

Os capítulos dos estudantes foram estruturados para explicar detalhadamente cada conceito de C#, a operação do Visual Studio 2022 e o **motivo** de cada decisão técnica, fornecendo **orientações de construção passo a passo** sem entregar o código pronto do projeto.

- **Não compartilhe `src/SupermercadoApp` como material de cópia.** A pasta `src/` serve exclusivamente para conferência docente, preparação de dados de laboratório e validação automatizada.
- **Estimule a investigação de erros:** quando surgir uma mensagem vermelha na **Lista de Erros** do Visual Studio 2022, peça primeiro que o estudante leia o código e a descrição do erro e tente localizar a linha antes de intervir.
- **Peça o "porquê":** ao corrigir ou acompanhar uma dupla, pergunte *por que* aquele campo é `private readonly`, *por que* o preço usa `decimal` ou *por que* a consulta usa `@parametro`.

---

## Antes de Desenvolver o Projeto (Etapa F00–F07)

Para estudantes sem experiência em programação ou POO, inicie obrigatoriamente pelo [Guia Preparatório F00–F07](../fundamentos/README.md) (8 encontros sugeridos, 28 horas). Para essa etapa, prepare apenas o **Visual Studio 2022** com o workload *Desenvolvimento para desktop com .NET*.

Não apresente os quatro pilares de POO em um único encontro inicial. Siga a ordem progressiva: execução e tipos (`F00`–`F01`) → controle de fluxo e depuração (`F02`) → métodos (`F03`) → classes, objetos e referências (`F04`) → construtores e encapsulamento (`F05`) → composição e coleções (`F06`) → interfaces e polimorfismo (`F07`). Os checkpoints `00–20` abaixo pertencem às 20 aulas do projeto e não geram os exercícios `F00–F07`.

---

## Gerar os Checkpoints de Recuperação (00 a 20)

Use os checkpoints apenas como ponto de partida de **recuperação** para estudantes que faltaram ou acumularam erros que impediriam acompanhar a aula do dia. Cada checkpoint contém apenas os arquivos, métodos e controles da etapa correspondente.

Na raiz do repositório, usando PowerShell e o SDK configurado em `global.json`:

```powershell
powershell -ExecutionPolicy Bypass -File tools/Gerar-Checkpoints.ps1
powershell -ExecutionPolicy Bypass -File tools/Validar-Material.ps1
```

As versões são geradas em `artifacts/checkpoints/00` até `20`. A aula `N` começa no estado `N−1` e termina em `N` (o checkpoint `00` é o console inicial da Aula 01). Cada pasta contém um `.csproj` independente, scripts SQL disponíveis até aquela etapa, inventário de arquivos e `METODOS.txt` com assinaturas públicas. O gerador recusa sobrescrever uma pasta existente para não apagar trabalhos; passe `-Destino artifacts/outro-nome` se precisar gerar outra série.

`Validar-Material.ps1` verifica links, âncoras, tabelas Markdown, blocos de código, soma de 210 minutos por aula, IDs das 114 atividades, gera e compila os 21 checkpoints em `Release` e executa os consoles `00–04` com `--verificar`, além da suíte de verificações com Fakes (sem exigir banco de dados ou credenciais).

---

## Preparação Docente Antes dos Encontros

| Antes da aula | Preparação do professor | Evidência da preparação |
| --- | --- | --- |
| F00 / 01 | Conferir Visual Studio 2022 instalado e workload Desktop .NET em todas as máquinas; aplicar diagnóstico rápido sem nota antes da Aula 01. | Projeto console criado e executado com `Ctrl+F5` na máquina padrão do laboratório. |
| 05 | Conferir criação de projeto Windows Forms (.NET moderno, sem `.NET Framework`) e acesso ao `nuget.org` (ou cache local). | Checkpoint 05 compilado sem erros no ambiente da turma. |
| 07 | Separar um banco ou branch vazio por dupla no Neon (ou banco dedicado por dupla no XAMPP). | Lista de ambientes da turma registrada sem expor senhas. |
| 09 | Conferir script `001-usuarios.sql` aplicado e variável `SUPERMERCADO_DB` funcionando. | Diagnóstico `SELECT 1` validado antes de iniciar autenticação. |
| 10 | Preparar uma conta `Caixa` de laboratório para cada dupla usando `--preparar-caixa` (ferramenta administrativa explicada abaixo). | Cada perfil abre sua janela correspondente; nenhuma senha anotada no código. |
| 11 | Conferir script `002-produtos.sql` pronto para execução na aula. | Validação com Fake em memória rejeitando preço inválido sem gravar. |
| 12 | Cadastrar dois produtos fictícios (arroz e feijão) no banco didático pela referência docente antes da aula de consulta. | Busca parcial na grade retorna os produtos fictícios esperados. |
| 16 | Preparar script `003-vendas.sql`, duas contas de caixa, produto com estoque `1` e branch isolada para o teste de rollback com gatilho. | Contagens e saldos iniciais anotados; gatilho ausente antes do experimento. |
| 18 | Preparar a massa de fronteira (`database/laboratorio/18-fronteiras.sql`) e conferir fuso horário de Brasília no Windows. | Contagem e soma conferem com o SQL de referência de datas fixas. |

### Ferramenta docente `--preparar-caixa`

Para criar uma conta de `Caixa` antes da Aula 19 sem adicionar botões provisórios ao projeto dos estudantes, o professor pode executar a referência docente (ou checkpoint 10–18) com o argumento `--preparar-caixa`. O programa exige login de um `Administrador` válido e abre o formulário de cadastro para criar a conta de teste da dupla. Remova o argumento após a preparação.

---

## Condução das Aulas Mais Densas (09 e 16)

- **Aula 09 (Autenticação com Hash):** conduza em quatro paradas obrigatórias com compilação intermediária: (1) modelos `Perfil`, `Credencial`, `Sessao` e ausência de usuário; (2) contrato e repositório SQL parametrizado; (3) serviço de hash com `PasswordHasher` e preparação do primeiro admin; (4) ligação assíncrona no `FormLogin` e `Program`.
- **Aula 16 (Venda com Transação e Concorrência):** nos 30 minutos de explicação, desenhe com a turma no quadro a linha do tempo de dois caixas disputando a última unidade e o risco de deadlock. Nos 60 minutos guiados, oriente a construção do esquema, modelos e serviço; nos 60 de prática, oriente o repositório em blocos e conduza os ensaios de concorrência, repetição de `Guid` e rollback após escrita. Se a rede ou o tempo impedirem um ensaio, registre como *pendente* para recuperação — nunca substitua execução real por previsão.

---

## Devolutiva e Recuperação

Inicie cada aula com uma pergunta rápida de retomada sobre o porquê de uma decisão da aula anterior. Use a rubrica do [Plano de Aulas](../PLANO-DE-AULAS.md) e registre por ID (*atingiu*, *parcial* ou *pendente*) com uma ação concreta de recuperação (ex.: *"Refazer A02.03 testando valor negativo e explicar por que conversão válida ainda precisa de regra de negócio"*). Nos marcos 06, 10, 14, 17 e 20, peça que cada estudante explique individualmente um trecho que construiu.
