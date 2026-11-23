# Aula 24 — Verificação Automatizada, Roteiro Manual e Publicação no VS 2022

**Duração:** 3h30. **Pré-requisito:** [Aula 23](23-Cadastro-de-Usuarios.md). **Entrega:** projeto de verificações com Fakes construído e executado com suas próprias asserções, compilação limpa em modo `Release`, roteiro manual conferido em dupla e pasta executável autocontida (`win-x64`) rodando fora do Visual Studio 2022.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Conferir o fluxo completo do MarketFlow e o escopo final da entrega |
| Explicação | 30 | Pirâmide de testes (Build, Fakes, Integração, UI), `Release` vs `Debug` e publicação |
| Construção | 60 | Construir seu projeto console de verificações com Fakes e asserção de regressão |
| Intervalo | 15 | Pausa |
| Prática | 60 | Executar roteiro manual em dupla, publicar pelo VS 2022 e rodar fora da IDE |
| Entrega | 30 | Demonstrar executável, explicar transação/camadas e registrar limites conhecidos |

---

## 1. O Que Cada Tipo de Verificação Comprova (E O Que NÃO Comprova!) (`A24.01`)

Um erro frequente de iniciantes é achar que *"compilou sem erro = sistema sem bugs"*. Cada camada de verificação comprova uma coisa diferente:

| Tipo de Verificação | O que comprova | O que **NÃO** comprova |
| --- | --- | --- |
| **Compilação (`Build Release`)** | Sintaxe válida, tipos compatíveis e assinaturas batendo. | Não comprova que o cálculo do troco ou o SQL estão corretos. |
| **Testes de Regra com Fakes em Memória** | Validações de `Carrinho`, `ProdutoService`, `AutenticacaoService`, `VendaService` e permissões — executam em milissegundos e sem internet! | Não comprova `FOR UPDATE`, rollback real do PostgreSQL nem constraints SQL. |
| **Testes de Integração no Banco** | Atomicidade (rollback após escrita), concorrência pelo último item, idempotência real e intervalo UTC no SQL. | Não comprova navegação por teclado (`Tab`/`Enter`) nem cliques de tela. |
| **Teste Manual de Interface (UI)** | Fluxo real de uso, layout, teclado, mensagens amigáveis e execução fora da IDE. | Demorado para repetir dezenas de fronteiras a cada pequena mudança. |

**Por que precisamos dos quatro?** Porque eles se complementam! Nunca diga que um teste com Fake em memória "testou a transação do banco".

---

## 2. Entendendo `Debug` vs `Release` e Publicação Autocontida (`A24.04`)

Quando você aperta `F5` durante as aulas, o Visual Studio 2022 compila em modo **`Debug`** (inclui informações extras para Breakpoints e desativa otimizações). Para entregar um aplicativo para uso:

- **`Release`:** ativa otimizações do compilador para um executável mais rápido e enxuto.
- **RID (`win-x64`):** *Runtime Identifier* — indica o sistema operacional e arquitetura de destino (Windows 64 bits).
- **Implantação Autocontida (`Self-Contained`):** inclui o motor do .NET dentro da pasta publicada.
  - **Vantagem:** o computador de destino não precisa ter o .NET Runtime pré-instalado.
  - **Trade-off (por que saber o custo):** a pasta fica maior (vários arquivos que precisam ser copiados **juntos** — nunca copie só o `.exe`!) e, quando sair uma atualização de segurança do .NET, você precisa republicar o aplicativo.

---

## 3. Roteiro de Construção Guiada: Suas Verificações e Publicação no VS 2022

### Passo 1 — Construir um Projeto Console de Verificações com Fakes (`A24.02`)
Na sua solução `Supermercado` no **Visual Studio 2022**:
1. Clique com o botão direito na **Solução `Supermercado`** (no topo do Gerenciador de Soluções) → **Adicionar → Novo Projeto...** → **Aplicativo de Console** (C#) → nomeie **`SupermercadoApp.Verificacoes`**.
2. **Ajuste obrigatório do `.csproj` antes de referenciar o Windows Forms:** dê duplo clique sobre `SupermercadoApp.Verificacoes` no Gerenciador de Soluções para abrir o `.csproj`. Como `SupermercadoApp` usa Windows Forms (`net8.0-windows`), um console comum (`net8.0`) acusará erro de incompatibilidade (`NU1201`) ao referenciá-lo! Ajuste `<PropertyGroup>` para casar com o target do seu `SupermercadoApp`:
   - Altere `<TargetFramework>net8.0</TargetFramework>` para **`<TargetFramework>net8.0-windows</TargetFramework>`** (usando a mesma versão `.NET` do seu `SupermercadoApp`);
   - Adicione **`<UseWindowsForms>true</UseWindowsForms>`** logo abaixo e salve (`Ctrl+S`).
3. Agora clique com o botão direito nas **Dependências** de `SupermercadoApp.Verificacoes` → **Adicionar Referência de Projeto...** → marque **`SupermercadoApp`** e clique em **OK**.
   - *(Atenção: mantenha `SupermercadoApp` como Projeto de Inicialização padrão; clique com o botão direito em `SupermercadoApp` → **Definir como Projeto de Inicialização**.)*
4. Dentro de `SupermercadoApp.Verificacoes`, reaproveite os casos das Aulas 15 e 19 e construa **suas próprias verificações automatizadas** com classes Fake implementando `IProdutoRepository`, `IUsuarioRepository` e `IVendaRepository`:
   - Crie uma função auxiliar `Conferir(bool condicao, string descricao)` que lança `Exception` se a condição for falsa;
   - Teste agrupamento e total `60,30` no `Carrinho`, recusa de preço `0` / `1.999m` / sessão de `Caixa` no `ProdutoService`, preservação de espaços na senha e salt no `AutenticacaoService`, e bloqueio de intervalo invertido no `VendaService`.
5. **Prova de regressão (teste que detecta bug real — `A24.02`):** em uma cópia local controlada, introduza deliberadamente um pequeno defeito em uma regra (ex.: alterar `<= 0` para `< 0`), execute seu console de verificações e **comprove que sua asserção falha avisando o defeito**. Restaure a regra correta e execute novamente comprovando aprovação!

*(Para referência docente, a solução do repositório também traz uma suíte de verificações em `tests/SupermercadoApp.Verificacoes` e um runner opt-in de integração PostgreSQL em `tests/SupermercadoApp.Integracao` — consulte [VALIDACAO-MANUAL.md](VALIDACAO-MANUAL.md) e [src/README.md](src/README.md) para detalhes de execução de laboratório.)*

### Passo 2 — Verificação Manual em Dupla (`A24.03`)
Siga o [**Roteiro Manual Completo**](VALIDACAO-MANUAL.md) trabalhando em dupla: uma pessoa opera a aplicação no papel de Administrador e a outra confere os resultados no banco e anota evidências; depois troquem os papéis para operar como Caixa. Qualquer teste que não puder ser executado por falta de rede/ambiente deve ser marcado honestamente como **pendente**, nunca como "passou".

### Passo 3 — Publicar pelo Assistente do Visual Studio 2022 (`A24.04`)
1. No **Gerenciador de Soluções**, clique com o botão direito no projeto **`SupermercadoApp`** (e **não** na solução!) → **Publicar...**.
2. Escolha destino **Pasta** → **Pasta** → informe o caminho local `artifacts\publish` e clique em **Concluir**.
3. Clique em **Mostrar todas as configurações** e ajuste exatamente:
   - **Configuração:** `Release`
   - **Modo de implantação:** `Autocontido` (*Self-contained*)
   - **Runtime de destino:** `win-x64`
   - **Opções de publicação de arquivo:** deixe *Produzir arquivo único* e *Cortar código não usado (Trimming)* **desmarcados** nesta primeira publicação.
4. Salve e clique em **Publicar**. Ao concluir, abra a pasta gerada e observe todos os arquivos criados junto ao `SupermercadoApp.exe`.

*(Equivalente opcional pelo Terminal do Visual Studio 2022: `dotnet publish src/SupermercadoApp/SupermercadoApp.csproj -c Release -r win-x64 --self-contained true -o artifacts/publish`, ajustando o caminho para o `.csproj` do seu projeto.)*

### Passo 4 — Executar Fora do Visual Studio 2022 (`A24.05`)
1. **Feche completamente o Visual Studio 2022.**
2. Confirme que a variável de ambiente `SUPERMERCADO_DB` está configurada na conta Windows (nunca coloque arquivos de credenciais dentro da pasta `publish`!).
3. Abra a pasta `artifacts\publish` pelo Explorador de Arquivos do Windows e execute **`SupermercadoApp.exe`** com duplo clique.
4. Demonstre login como `Administrador`, troca de usuário para `Caixa` e uma venda fictícia completa.

---

## 4. Critérios Finais de Entrega e Limites Conhecidos (`A24.06`)

Sua entrega final deve reunir:
- Solução e scripts SQL organizados (sem pastas `bin`, `obj`, `.vs` e **sem credenciais**).
- Compilação `Release` sem erros e saída registrada das suas verificações com Fakes.
- Evidências manuais dos fluxos de Administrador e Caixa (incluindo casos inválidos e pendências reais).
- Pasta publicada executando fora do Visual Studio 2022 em máquina configurada.
- Explicação individual (com suas palavras) de: (a) como funciona uma transação com bloqueio, (b) como a autorização é protegida no serviço e (c) o caminho completo `View → Service → Repository → Banco`.
- **Limites conhecidos registrados com honestidade:** pagamento em dinheiro simulado, comprovante didático sem valor fiscal, conexão direta didática ao banco (produção exige API autenticada), sem modo offline ou recuperação durável após queda abrupta de energia.

> [!TIP]
> **Próximos passos na sua jornada C#:** agora que você construiu um sistema completo do zero entendendo cada detalhe, desafios excelentes de continuidade são criar uma **ASP.NET Core Web API** para proteger o banco, adotar um framework de testes (**xUnit**), implementar **paginação no relatório**, **abertura/fechamento de caixa**, **devoluções** e **recuperação de senha**!

Consulte a documentação oficial sobre [Implantação de Aplicativos .NET](https://learn.microsoft.com/en-us/dotnet/core/deploying/).

[Anterior: Aula 23](23-Cadastro-de-Usuarios.md) · [Índice Geral](README.md)

## Atividades por conceito

Realize as atividades da [aula 24 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-24). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
