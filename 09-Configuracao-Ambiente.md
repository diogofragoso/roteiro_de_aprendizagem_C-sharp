# Aula 09 — Ambiente, Solução e Primeiro Windows Forms no Visual Studio 2022

**Duração:** 3h30. **Pré-requisito:** [Aula 08](08-Arquitetura-e-Padroes.md). **Entrega:** solução `Supermercado` e projeto Windows Forms `SupermercadoApp` organizados em 5 pastas, com `FormLogin` abrindo, fechando e compilando sem erros no Visual Studio 2022.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Comparar aplicativo de console e aplicativo visual desktop |
| Explicação | 30 | IDE, SDK, Runtime, `.sln`, `.csproj`, `partial class` e `Program.cs` |
| Construção | 60 | Conferir Visual Studio 2022 e criar a solução `Supermercado` |
| Intervalo | 15 | Pausa |
| Prática | 60 | Criar as 5 pastas, renomear `FormLogin`, ajustar namespaces e depurar |
| Entrega | 30 | Compilar (`Ctrl+Shift+B`), executar (`F5`) e registrar ambiente |

---

## 1. Entendendo as Peças do Ambiente (IDE vs SDK vs Runtime)

Antes de criar o projeto visual, entenda **por que** precisamos conferir o ambiente no laboratório:

| Componente | O que faz? | Por que precisa estar instalado? |
| --- | --- | --- |
| **Visual Studio 2022 (IDE)** | Editor, Designer visual de telas, Gerenciador de Soluções e Depurador. | É o ambiente onde você projeta formulários, escreve C# e investiga erros. |
| **.NET SDK** | Compilador e ferramentas de linha de comando (`dotnet build`, `dotnet run`). | Instalar apenas o Runtime permite *rodar* programas prontos, mas **não** permite criar nem compilar projetos novos. |
| **.NET Runtime** | Motor que executa o programa compilado no Windows. | Já vem incluído dentro do SDK no computador de desenvolvimento. |
| **Workload Desktop .NET** | Pacote de ferramentas dentro do Visual Studio Installer para Windows Forms e WPF. | Sem essa carga de trabalho marcada, os modelos de *Aplicativo do Windows Forms* nem aparecem na tela de novo projeto! |

### Passo a passo: Conferindo o Visual Studio 2022 no Laboratório (`A09.01`)
1. Abra o menu Iniciar do Windows e procure por **Visual Studio Installer**.
2. Localize **Visual Studio Community 2022** e clique em **Modificar**.
3. Na aba **Cargas de trabalho (Workloads)**, confirme que **Desenvolvimento para desktop com .NET** está marcada. Se não estiver, marque e conclua a instalação.
4. Abra o **Visual Studio 2022**, clique em **Ajuda → Sobre o Microsoft Visual Studio** e anote a versão instalada no seu caderno.
5. No menu superior do VS 2022, abra **Exibir → Terminal** (`Ctrl+'`) e execute:
   ```powershell
   dotnet --list-sdks
   ```
   Confirme que há um SDK moderno do .NET instalado (ex.: `8.0.xxx` nativo do VS 2022, ou a versão definida pelo professor para a turma) e registre a saída no caderno.

> [!NOTE]
> **Escolha do banco de dados:** combine com o professor antes da Aula 12 se a turma seguirá pela trilha principal (**Neon/PostgreSQL**) ou pela trilha local ([**MySQL/MariaDB pelo XAMPP**](INTEGRACAO-MYSQL-XAMPP.md)). Hoje e na próxima aula (catálogo prático de componentes Windows Forms) ainda não precisamos de banco configurado.

---

## 2. Criando a Solução `Supermercado` e o Projeto `SupermercadoApp` no VS 2022

Siga cada tela com atenção (`A09.02`):

1. No **Visual Studio 2022**, clique em **Criar um novo projeto**.
2. Na barra de pesquisa, digite `Windows Forms` e selecione a linguagem **C#** nos filtros.
3. Escolha **Aplicativo do Windows Forms** (**atenção:** **NÃO** escolha a opção que tem o sufixo **`(.NET Framework)`**!).
   - **Por quê?** O `.NET Framework` é o modelo legado (antigo) sem suporte a recursos modernos de C# que usaremos no curso.
4. Clique em **Próximo**.
5. Preencha exatamente assim:
   - **Nome do projeto:** `SupermercadoApp`
   - **Nome da solução:** `Supermercado`
   - **Por que nomes diferentes?** A Solução (`Supermercado.sln`) representa o sistema inteiro (que na Aula 25 pode agrupar também projetos de teste), enquanto `SupermercadoApp` é o projeto do executável Windows Forms.
6. Clique em **Próximo**, selecione a versão moderna do **.NET** da turma (ex.: **.NET 8.0 (Suporte de Longo Prazo)** no VS 2022) e clique em **Criar**.

![Exemplo de criação de projeto no Visual Studio 2022](assets/vs2022_new_project_1791481163568.jpg)

*(As capturas são ilustrativas; guie-se sempre pelo nome exato do modelo e pela versão moderna do .NET.)*

7. Pressione **`Ctrl+Shift+B`** (Compilar Solução) e confira na janela **Saída** (parte inferior) a mensagem de compilação bem-sucedida.
8. Pressione **`F5`** (Iniciar): uma janela vazia chamada `Form1` abrirá no Windows. Feche-a pelo `X` para voltar ao Visual Studio 2022.

---

## 3. Anatomia de um Projeto Windows Forms no Visual Studio 2022

![Exemplo do Designer Windows Forms no Visual Studio 2022](assets/vs2022_winforms_designer_1791481183125.jpg)

Antes de sair clicando, entenda o papel de cada arquivo e janela que o Visual Studio 2022 criou para você:

### As 3 janelas essenciais do VS 2022 para Windows Forms
- **Gerenciador de Soluções (`Ctrl+Alt+L`):** mostra todas as pastas, arquivos `.cs` e **Dependências** do projeto.
- **Caixa de Ferramentas (`Ctrl+Alt+X`):** lista os controles visuais (`Button`, `TextBox`, `Label`, `DataGridView`) que você arrasta para dentro de um formulário no Designer.
- **Janela de Propriedades (`F4`):** mostra e edita aparência, nome (`Name`), texto (`Text`) e eventos (ícone de raio ⚡) do controle que estiver clicado.

### Os 4 arquivos gerados e **por que** existem assim

| Arquivo | O que existe dentro dele? | Por que funciona assim? |
| --- | --- | --- |
| **`SupermercadoApp.csproj`** | Arquivo XML de configuração do projeto (clique duplo no nome do projeto no Gerenciador de Soluções para abrir). | Contém `<OutputType>WinExe</OutputType>` (abre janela sem tela preta), `<TargetFramework>...-windows</TargetFramework>`, `<UseWindowsForms>true</UseWindowsForms>`, `<ImplicitUsings>enable</ImplicitUsings>` e `<Nullable>enable</Nullable>`. |
| **`Program.cs`** | O ponto de entrada (`Main`) com `[STAThread]`, `ApplicationConfiguration.Initialize()` e `Application.Run(new Form1())`. | `[STAThread]` é exigido pelo Windows para gerenciar janelas e área de transferência; `ApplicationConfiguration.Initialize()` ajusta fontes e resolução de tela (DPI); `Application.Run(...)` abre a janela inicial e mantém o aplicativo vivo ouvindo cliques até ela fechar. |
| **`Form1.cs`** | Código escrito **por você** (construtor, eventos de botão, chamadas aos serviços). | Separa sua lógica do código visual gerado automaticamente pela IDE. |
| **`Form1.Designer.cs`** | Código C# gerado **automaticamente pelo Designer** contendo o método `InitializeComponent()` (posições, tamanhos e cores dos controles). | **Por que são dois arquivos para a mesma classe?** Graças à palavra-chave **`partial class`**, o compilador junta `Form1.cs` e `Form1.Designer.cs` em uma única classe na hora de compilar! Assim o Designer pode reescrever o `.Designer.cs` sem apagar o código que você escreveu no `.cs`. |

---

## 4. Roteiro de Construção Guiada: Organizando a Arquitetura no VS 2022 (`A09.03`–`A09.05`)

Organizar as pastas logo no início evita ter de consertar dezenas de `namespace` e `using` depois. Siga passo a passo:

### Passo 1 — Criar as 5 pastas arquiteturais
No **Gerenciador de Soluções**, clique com o botão direito sobre o projeto **`SupermercadoApp`** → **Adicionar → Nova Pasta** e crie exatamente estas cinco pastas:
1. `Models`
2. `Services`
3. `Repositories`
4. `Infrastructure`
5. `Views`

### Passo 2 — Renomear e mover `Form1.cs` para `Views/FormLogin.cs`
1. No **Gerenciador de Soluções**, clique com o botão direito em `Form1.cs` → **Renomear** (`F2`) → digite **`FormLogin.cs`** e pressione `Enter`.
2. Quando o Visual Studio 2022 perguntar se deseja renomear todas as referências ao elemento de código `Form1` neste projeto, clique em **Sim**.
3. Arraste `FormLogin.cs` para dentro da pasta **`Views`** (o arquivo filho `FormLogin.Designer.cs` será movido automaticamente junto com ele).

### Passo 3 — Conferir e corrigir o `namespace` nas DUAS partes da `partial class`!
> [!WARNING]
> **Armadilha clássica no VS 2022:** arrastar um formulário para uma pasta **nem sempre** atualiza o `namespace` automaticamente! E se `FormLogin.cs` ficar em um namespace e `FormLogin.Designer.cs` ficar em outro, o compilador achará que são duas classes diferentes e `InitializeComponent()` desaparecerá com erro vermelho!

1. Clique com o botão direito em `Views/FormLogin.cs` → **Exibir Código** (`F7`). Altere o namespace para **`namespace SupermercadoApp.Views;`** (ou `namespace SupermercadoApp.Views { ... }`).
2. No **Gerenciador de Soluções**, clique na setinha ao lado de `FormLogin.cs` para expandir e abra **`FormLogin.Designer.cs`**. Confirme que o namespace lá **também** é exatamente **`SupermercadoApp.Views`**.
3. Abra **`Program.cs`**:
   - Adicione `using SupermercadoApp.Views;` no topo (para que `Program.cs` enxergue `FormLogin`, que agora mora na subpasta `Views`).
   - Confirme que a linha de abertura chama `Application.Run(new FormLogin());`.

### Passo 4 — Compilar, Depurar e Preparar Entrega Limpa
1. Pressione **`Ctrl+Shift+B`** e confirme zero erros na **Lista de Erros**.
2. Abra `Views/FormLogin.cs` em modo código (`F7`), coloque um **Breakpoint (`F9`)** na linha `InitializeComponent();` dentro do construtor `public FormLogin()`.
3. Execute com **`F5`**, observe a pausa no construtor, pressione **`F10`** para executar `InitializeComponent()` e depois **`F5`** para a janela abrir. Feche a janela e confirme que o aplicativo encerra normalmente.
4. **Cópia limpa de entrega (`A09.05`):** com o Visual Studio 2022 fechado, localize a pasta do projeto no Explorador de Arquivos. Ao preparar um backup ou envio de código-fonte, **nunca** inclua as pastas `bin`, `obj` e `.vs` (elas contêm arquivos temporários pesados gerados automaticamente na compilação). Se utilizar Git, adote o arquivo `.gitignore` da raiz deste repositório.

---

## 5. Erros Comuns no Visual Studio 2022

| Erro na Lista de Erros | Causa provável | Como corrigir |
| --- | --- | --- |
| `CS0103: O nome 'InitializeComponent' não existe no contexto atual` | `FormLogin.cs` e `FormLogin.Designer.cs` estão com `namespace` diferentes! | Deixe exatamente `namespace SupermercadoApp.Views` em **ambos** os arquivos. |
| `CS0246: O nome do tipo ou do namespace 'FormLogin' não pode ser encontrado` | `Program.cs` não tem `using SupermercadoApp.Views;` após você mover o formulário para `Views`. | Adicione `using SupermercadoApp.Views;` no topo do `Program.cs`. |
| O arquivo abre em código em vez do desenho da tela | Você usou `F7` ou abriu `.Designer.cs`. | Clique com o botão direito em `FormLogin.cs` → **Exibir Designer** (`Shift+F7`). |

[Anterior: Aula 08](08-Arquitetura-e-Padroes.md) · [Próxima: Aula 10 — Componentes do Windows Forms na prática](10-Componentes-Windows-Forms.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 09 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-09). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
