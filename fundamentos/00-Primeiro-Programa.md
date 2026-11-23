# F00 — Do Enunciado ao Primeiro Programa no Visual Studio 2022

**Pré-requisito:** nenhum conhecimento de programação (basta saber usar o Windows).
**Objetivo:** entender o que é um algoritmo, conhecer a interface do Visual Studio 2022, construir e executar seu primeiro programa em C# e aprender a investigar erros de compilação.

---

## 1. O Que É um Programa e Por Que o Computador Precisa de Ordens Exatas?

Um computador não adivinha intenções: ele apenas executa **uma lista ordenada de instruções exatas**, linha por linha, de cima para baixo. Essa sequência lógica de passos se chama **algoritmo**.

Compare duas formas de descrever o cálculo de 2 cadernos de R$ 12,00:

- **❌ Vago (o computador não entende):** *"Calcule o valor da compra direitinho."*
- **✅ Preciso (algoritmo):**
  1. Guardar a quantidade: `2`.
  2. Guardar o preço unitário: `12`.
  3. Multiplicar a quantidade pelo preço.
  4. Mostrar o resultado (`24`) na tela.

**Por que planejar no papel antes de abrir o editor?**
Quem começa digitando código direto sem saber quais dados entram, qual conta é feita e o que precisa sair acaba misturando erro de lógica com erro de digitação.

**Atividade no papel (antes do código):** escreva os passos numerados para calcular o dobro de um número qualquer. Indique qual dado entra, qual operação é feita e qual resultado sai. Teste mentalmente com `3` (esperado: `6`) e com `0` (esperado: `0`).

---

## 2. Entendendo Cada Ferramenta (E Por Que Usamos Todas Elas)

Antes de clicar nos botões, entenda o papel de cada peça no seu computador:

| Ferramenta / Termo | O que é exatamente? | Por que usamos dessa forma? |
| --- | --- | --- |
| **C# (C-Sharp)** | A linguagem de programação em que escrevemos as instruções em texto humano. | É moderna, segura contra erros de tipo e usada no mercado para desktop, web, jogos e nuvem. |
| **.NET SDK** | Conjunto de ferramentas (incluindo o compilador) que lê o seu texto em C# e o transforma em um programa executável. | O computador não entende texto em inglês/português; o SDK traduz o código C# para instruções de máquina. |
| **Visual Studio 2022** | O ambiente de desenvolvimento (**IDE**) que junta editor de código, organizador de arquivos, compilador e depurador na mesma janela. | Permite detectar erros de digitação enquanto você escreve, autocompletar nomes (IntelliSense) e pausar o programa linha por linha. |
| **Projeto (`.csproj`)** | Uma pasta contendo os arquivos de código e o arquivo `.csproj` que guarda as configurações daquele programa. | Isola cada exercício ou aplicativo com suas próprias bibliotecas e versão do .NET. |
| **Solução (`.sln`)** | Um "fichário" que pode agrupar um ou mais projetos relacionados. | No futuro, permitirá manter o aplicativo principal e o projeto de testes abertos juntos. |
| **Aplicativo de Console** | Programa de tela de texto simples (sem botões nem janelas gráficas). | **Por que começar no Console?** Remove distrações visuais para você focar 100% em aprender lógica e a linguagem C# antes de lidar com eventos de mouse e janelas. |

> [!NOTE]
> O professor deve deixar o **Visual Studio 2022 Community** preparado com a carga de trabalho **Desenvolvimento para desktop com .NET** (detalhada na [Aula 05](../05-Configuracao-Ambiente.md)). Nesta etapa, você **não** precisa de banco de dados nem XAMPP.

---

## 3. Passo a Passo no Visual Studio 2022: Criando Seu Primeiro Projeto

Siga cada clique com atenção:

1. Abra o **Visual Studio 2022** pelo menu Iniciar do Windows.
2. Na tela inicial, clique no botão **Criar um novo projeto** (canto inferior direito).
3. Na caixa de pesquisa no topo, digite `Console`.
4. Nos filtros abaixo da caixa de pesquisa, garanta que a linguagem esteja em **C#**.
5. Selecione **Aplicativo de Console** (com o ícone de C# e a descrição para .NET moderno).
   - **⚠️ Atenção — Por que NÃO escolher `Aplicativo de Console (.NET Framework)`?** O `.NET Framework` é a geração antiga do Windows (encerrada na versão 4.8). O curso usa o **.NET moderno**, que é mais rápido, atualizado e compatível com os recursos atuais do C#.
6. Clique em **Próximo**.
7. Em **Nome do projeto**, digite `FundamentosF00`. Escolha uma pasta limpa de estudos em **Local** (por exemplo, `C:\EstudosCSharp`) e clique em **Próximo**.
8. Na tela **Informações adicionais**:
   - Em **Estrutura (Framework)**, selecione a versão moderna do .NET configurada na sua turma (por exemplo, **.NET 8.0** no VS 2022).
   - **Deixe DESMARCADA** a caixa *"Não usar instruções de nível superior"*. **Por quê?** Com instruções de nível superior, o arquivo `Program.cs` começa limpo e direto ao ponto, sem obrigar um iniciante a decorar `namespace`, `class Program` e `static void Main(string[] args)` no primeiro dia.
9. Clique em **Criar**.

### Reconhecendo as 3 áreas principais do Visual Studio 2022

- **Editor de Código (centro):** onde o arquivo `Program.cs` está aberto e você digita suas instruções.
- **Gerenciador de Soluções (painel direito):** mostra a árvore de pastas e arquivos do projeto. Se fechar sem querer, reabra pelo menu **Exibir → Gerenciador de Soluções** (`Ctrl+Alt+L`).
- **Lista de Erros (painel inferior):** mostra avisos amarelos e erros vermelhos que impedem o programa de compilar. Reabra em **Exibir → Lista de Erros** (`Ctrl+\, E`).

---

## 4. Anatomia da Primeira Instrução em C#

Ao criar o projeto, o Visual Studio 2022 gera um arquivo `Program.cs` com uma linha pronta:

```csharp
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
```

Entenda **cada detalhe** dessa única linha — nada ali está por acaso:

| Parte do código | O que é? | Por que precisa estar ali? |
| --- | --- | --- |
| `// ...` | **Comentário de linha.** | O compilador ignora tudo após `//` até o fim da linha. Serve exclusivamente para deixar anotações para humanos que leem o código. |
| `Console` | **Classe pronta do .NET** que representa a janela de texto (terminal). | Em C#, letras maiúsculas e minúsculas fazem diferença (*case-sensitive*): `console` em minúsculo gera erro porque o nome oficial é `Console`. |
| `.` (ponto) | **Operador de acesso a membro.** | Significa *"entre em `Console` e use uma ação que pertence a ele"*. |
| `WriteLine` | **Nome do método (ação).** | Em inglês, "escrever linha": mostra um texto na tela e pula automaticamente para a linha de baixo. |
| `( ... )` | **Parênteses de chamada.** | Tudo que vai dentro dos parênteses é o **argumento** (a informação entregue para a ação trabalhar). |
| `"Hello, World!"` | **Texto literal (`string`).** | As aspas duplas avisam ao compilador onde o texto começa e onde termina. As aspas não aparecem na tela. |
| `;` | **Ponto e vírgula.** | Marca o ponto final de uma instrução em C#. Sem ele, o compilador acha que a frase continuou na linha seguinte e acusa erro. |

### Como executar no Visual Studio 2022 (`Ctrl+F5` vs `F5`)

- Pressione **`Ctrl+F5`** (*Depurar → Iniciar sem Depuração*): compila e executa o programa mantendo a janela preta aberta ao final com *"Pressione qualquer tecla para fechar esta janela..."*. É o ideal para ver resultados rápidos no console.
- Pressione **`F5`** (*Iniciar com Depuração*): executa conectado ao depurador (usaremos a partir de `F02` para pausar linha por linha).
- **Importante:** sempre feche a janela preta do console antes de editar e executar novamente, caso contrário o arquivo executável fica travado pelo Windows e o Visual Studio não consegue recompilá-lo.

---

## 5. Compilação vs Execução: Aprendendo a Ler Erros sem Medo

Muitos iniciantes acham que ver um sublinhado vermelho no editor é "quebrar o computador". Na verdade, o compilador é seu **melhor aliado**:

- **Compilar (`Ctrl+Shift+B`):** o Visual Studio verifica se todas as palavras, aspas, parênteses e pontos e vírgulas obedecem às regras do C# e traduz o código. Se houver erro de sintaxe, o programa nem chega a rodar.
- **Executar (`Ctrl+F5`):** roda o programa já compilado. Atenção: um programa pode compilar com zero erros e ainda assim mostrar a conta errada — isso é um **erro de lógica**, não de sintaxe.

> [!WARNING]
> **Regra vital no Visual Studio 2022:** se houver um erro de compilação e o Visual Studio perguntar *"Ocorreram erros de build. Deseja continuar e executar a última compilação com êxito?"*, clique sempre em **Não**. Executar a versão antiga esconde seu erro e faz você testar código velho!

---

## 6. Roteiro de Construção Guiada (Sua Vez!)

Construa as tarefas abaixo no seu `Program.cs` do projeto `FundamentosF00`, **sem copiar código pronto**:

1. **Ordem de execução (3 linhas):**
   - Apague o `"Hello, World!"` inicial.
   - Escreva um comentário no topo explicando o objetivo do programa (lembre-se de iniciar com `//`).
   - Escreva três instruções `Console.WriteLine(...)` em sequência: a primeira informando o início do programa, a segunda exibindo um nome fictício e a terceira exibindo uma meta de estudo.
   - Antes de apertar `Ctrl+F5`, anote no papel a ordem exata em que as 3 linhas vão aparecer. Execute e confirme.
2. **Experimento de inversão:**
   - Troque a primeira instrução pela última, preveja a nova saída no papel e execute novamente com `Ctrl+F5`.
3. **Investigação controlada de erro de sintaxe:**
   - Apague de propósito a aspa dupla de fechamento de uma das linhas.
   - Pressione `Ctrl+Shift+B` (Compilar).
   - Observe a janela **Lista de Erros** na parte inferior do Visual Studio 2022: anote o código do erro (ex.: `CS1010` / `CS1002`), dê duplo clique sobre a linha do erro para o cursor saltar ao local, entenda por que um único erro gerou mais de uma mensagem e recoloque a aspa.
   - Repita o experimento removendo um ponto e vírgula `;`, observe a mensagem e corrija.
4. **Algoritmo no papel para a próxima etapa:**
   - Sem programar ainda, escreva no papel o passo a passo para somar três notas escolares e dividir o resultado por 3. Teste no papel com `6`, `7` e `8` (resultado esperado: `7`).

---

## 7. Erros Comuns no Visual Studio 2022

| Mensagem / Sintoma | Por que aconteceu? | Como resolver |
| --- | --- | --- |
| `CS1002: ; esperado` | Faltou o ponto e vírgula no final de uma instrução (às vezes na linha **anterior** à marcada). | Confira o fim da linha indicada e da linha imediatamente acima. |
| `CS0103: O nome 'console' não existe` | Letras maiúsculas/minúsculas incorretas (`console` ou `writeline`). | C# diferencia maiúsculas e minúsculas: use `Console.WriteLine`. |
| `CS1010: Nova linha na constante` | Uma aspa dupla `"` foi aberta mas não fechada na mesma linha. | Feche o texto com `"` antes do parêntese `)`. |
| `O processo não pode acessar o arquivo .exe` | A janela preta da execução anterior ainda está aberta em segundo plano. | Feche a janela do console anterior e compile novamente. |

**Critério para avançar para F01:** criar e executar seu próprio projeto Console no Visual Studio 2022, prever corretamente a ordem de saída das instruções e explicar com suas palavras a diferença entre comentário, instrução de saída, erro de compilação e erro de lógica.

[Anterior: percurso de fundamentos](README.md) · [Próximo: F01 — Valores, variáveis e entrada](01-Valores-e-Entrada.md)
