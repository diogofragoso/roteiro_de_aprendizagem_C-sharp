# Aula 01 — Primeiro Programa no Visual Studio 2022, Variáveis e Tipos de Dados

**Duração:** 3h30. **Pré-requisito:** nenhum conhecimento prévio de programação (basta saber usar o Windows). **Entrega:** primeiro aplicativo de console criado no Visual Studio 2022 demonstrando ordem de execução, investigação de erro de compilação, declaração dos 7 tipos de dados do domínio, constantes, atribuição em memória, conversão com cultura e formatação monetária (`A01.01`–`A01.06`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Conhecer o projeto MarketFlow e o que é um algoritmo |
| Explicação | 30 | C#, .NET, Visual Studio 2022, memória RAM, tipos e tipagem forte |
| Construção | 60 | Criar o primeiro projeto Console no VS 2022 e investigar erro de sintaxe |
| Intervalo | 15 | Pausa |
| Prática | 60 | Construir cálculos com variáveis tipadas, `const`, conversão e moeda `:C` |
| Entrega | 30 | Demonstrar execução, explicar a escolha de cada tipo e registrar evidências |

---

## 1. O Que É um Programa? (Algoritmos e Ferramentas)

Um computador não adivinha intenções: ele apenas executa uma **sequência exata de instruções**, linha por linha, de cima para baixo. Essa sequência lógica se chama **algoritmo**.

- **❌ Vago:** *"Calcule a compra do cliente direitinho."*
- **✅ Preciso (algoritmo):** (1) Guardar a quantidade `3`; (2) Guardar o preço `5,50`; (3) Multiplicar quantidade pelo preço; (4) Mostrar `R$ 16,50` na tela.

Antes de clicar nos botões, entenda o papel de cada ferramenta e **por que** precisamos de todas elas:

| Ferramenta | O que é? | Por que usamos dessa forma? |
| --- | --- | --- |
| **C# (C-Sharp)** | Linguagem de programação moderna e **fortemente tipada**. | Exige declarar o tipo de cada variável, impedindo bugs invisíveis (como misturar texto com dinheiro) **antes** de o programa rodar. |
| **.NET SDK** | Compilador e ferramentas que traduzem seu texto em C# para programa executável. | O computador não entende texto humano; o SDK verifica a gramática e gera o executável. |
| **Visual Studio 2022** | IDE que reúne editor, Gerenciador de Soluções, Lista de Erros e Depurador. | Mostra erros enquanto você digita, completa comandos (IntelliSense) e permite pausar linha a linha. |
| **Aplicativo de Console** | Programa de tela de texto simples (sem botões gráficos). | **Por que começar no Console?** Remove distrações visuais para você dominar lógica, variáveis, métodos e objetos antes de adicionar telas Windows Forms (Aula 09) e banco de dados (Aula 11). |

---

## 2. Passo a Passo no Visual Studio 2022: Criando Seu Primeiro Projeto

1. Abra o **Visual Studio 2022 Community** pelo menu Iniciar do Windows.
2. Clique em **Criar um novo projeto**.
3. Pesquise `Console`, filtre por linguagem **C#** e escolha **Aplicativo de Console** (.NET moderno).
   - **⚠️ Por que NÃO escolher `Aplicativo de Console (.NET Framework)`?** O `.NET Framework` é a geração antiga do Windows. Nosso curso usa o **.NET moderno**, atualizado e compatível com todos os recursos atuais do C#.
4. Clique em **Próximo**, nomeie o projeto como **`TreinoSupermercado`** e clique em **Próximo**.
5. Selecione a versão moderna do **.NET** configurada no laboratório (ex.: `.NET 8.0`), mantenha **desmarcada** a opção *"Não usar instruções de nível superior"* (para começar com um `Program.cs` limpo) e clique em **Criar**.

### Reconhecendo as 3 áreas do Visual Studio 2022
- **Editor de Código (centro):** onde `Program.cs` está aberto.
- **Gerenciador de Soluções (direita — `Ctrl+Alt+L`):** mostra arquivos e pastas do projeto.
- **Lista de Erros (inferior — `Ctrl+\, E`):** lista erros vermelhos que impedem a compilação.

### Anatomia da Primeira Instrução em C#

```csharp
// Comentário: o computador ignora tudo após // até o fim da linha.
Console.WriteLine("Começo do programa");
```

| Símbolo / Palavra | Significado | Por que precisa estar ali? |
| --- | --- | --- |
| `Console` | Classe pronta do .NET que representa o terminal. | C# diferencia maiúsculas de minúsculas (*case-sensitive*): `console` gera erro. |
| `.` | Operador de acesso. | Significa *"acesse uma ação que pertence a `Console`"*. |
| `WriteLine` | Método que imprime texto e pula linha. | `Write` imprime sem pular linha; `WriteLine` pula linha ao final. |
| `("...")` | Parênteses com texto literal (`string`). | As aspas duplas delimitam o texto (não aparecem na tela). |
| `;` | Ponto e vírgula. | Marca o fim da instrução; sem ele, o compilador junta com a próxima linha e falha. |

- **Para executar:** pressione **`Ctrl+F5`** (*Iniciar sem Depuração*) para a janela preta ficar aberta no final. Feche sempre a janela do console antes de editar e executar de novo!
- **Compilar (`Ctrl+Shift+B`) vs Executar:** compilar verifica regras e traduz o código; executar roda o programa. Se houver erro de compilação e o VS 2022 perguntar *"Deseja executar a última compilação com êxito?"*, clique sempre em **Não**!

---

## 3. Variáveis, Memória RAM e os 7 Tipos do Domínio

Uma **variável** é um espaço nomeado na memória RAM com um **tipo** fixo. Entenda **por que** escolhemos cada tipo no supermercado:

| Tipo | Para que serve | Literal no código | Por que usar este tipo (e não outro)? |
| --- | --- | --- | --- |
| `string` | Textos e códigos com zeros | `"000123"` | **Por que código de barras é `string`?** Números (`int`/`long`) apagam zeros à esquerda (`000123` vira `123`) e nunca fazemos soma ou divisão com códigos de barras! |
| `char` | Um único caractere | `'A'` | Usa **aspas simples `' '`**. Ideal para uma letra de setor/corredor. |
| `int` | Número inteiro comum | `150` | Quantidades contáveis de unidades inteiras (até ~2,1 bilhões). |
| `long` | Número inteiro gigante | `7891020304050L` | Usa sufixo **`L`**. Necessário para contadores que ultrapassam o limite do `int`. |
| `double` | Decimal em base binária | `5.45` | Medidas físicas aproximadas (peso, altura), onde milionésimos não importam. |
| `decimal` | Decimal exato em **base 10** | `25.99m` | **Obrigatório para dinheiro!** Em binário (`double`), `0.1 + 0.2` resulta em `0.30000000000000004`. O sufixo **`m`** evita erros de centavos no caixa. |
| `bool` | Lógico (verdadeiro/falso) | `true` / `false` | Estados sim/não (produto ativo, incluir CPF na nota). |

### Constantes (`const`), Atribuição (`=`) e Interpolação (`$"..."`)
- **`const decimal TAXA_PADRAO = 0.10m;`**: trava um valor que nunca pode mudar durante o programa; tentar alterá-lo depois gera erro de compilação (`CS0131`).
- **`=` é atribuição, NÃO fórmula de planilha!** Se você faz `decimal total = quantidade * preco;` e depois muda `quantidade = 5;`, a variável `total` **continua com o valor antigo** até você atribuir o novo cálculo (`total = quantidade * preco;`).
- **Interpolação (`$"Total: {total:C}"`):** o `$` antes das aspas permite inserir variáveis entre `{ }`. O formatador `:C` (*Currency*) exibe o número como moeda (`R$ 16,50`).
- **Ponto no código vs Vírgula na tela:** literais C# sempre usam ponto (`5.50m`), mas a exibição e conversão de texto dependem da cultura! Configure no topo do arquivo `using System.Globalization;` e `CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");`. Ao converter o texto `"5.50"` (com ponto), passe **`CultureInfo.InvariantCulture`** em `decimal.Parse("5.50", CultureInfo.InvariantCulture)` para o C# não confundir o ponto com separador de milhar!

---

## 4. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

No `Program.cs` do projeto `TreinoSupermercado`, construa passo a passo os blocos **`A01.01` a `A01.06`** seguindo as orientações abaixo:

1. **Preparação inicial:** adicione `using System.Globalization;` na linha 1 e configure `CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");`.
2. **Bloco `A01.01` — Ordem de execução e erro de sintaxe:**
   - Escreva um comentário `//` no topo e três linhas `Console.WriteLine(...)` (início, nome do sistema e fim). Preveja a ordem no papel, rode com `Ctrl+F5`, depois inverta a primeira com a última e confirme.
   - **Experimento de erro:** remova de propósito uma aspa dupla `"`, pressione `Ctrl+Shift+B`, localize o código do erro na **Lista de Erros** do VS 2022, dê duplo clique para saltar à linha e corrija.
3. **Bloco `A01.02` — Os 7 tipos do domínio e tipagem forte:**
   - Declare variáveis para: nome do produto (`string`), setor (`char`), código `"000123"` (`string`), quantidade `3` (`int`), contador grande (`long` com `L`), peso (`double`), preço `5.50m` (`decimal` com `m`) e estado ativo (`bool`).
   - Exiba todos comprovando que `"000123"` preservou os zeros.
   - **Experimento de tipagem:** tente escrever temporariamente `decimal teste = "15.99";`, registre o erro (`CS0029`) no caderno explicando por que o C# recusou e remova a linha.
4. **Bloco `A01.03` — Atribuição na memória (não é planilha):**
   - Calcule `decimal subtotal = quantidade * preco;` (`3 * 5.50m = 16,50`) e exiba-o.
   - Altere `quantidade = 5;` (sem redeclarar `int`), exiba `subtotal` **antes** de recalcular (continua `16,50`!), recalcule e exiba novamente (`27,50`). Volte `quantidade = 3;` e `subtotal = quantidade * preco;` para os próximos blocos.
5. **Bloco `A01.04` — Constante e cálculo de taxa:**
   - Declare `const decimal TAXA_SIMULADA = 0.10m;`.
   - Calcule taxa (`subtotal * TAXA_SIMULADA`, esperado `R$ 1,65`) e total com taxa (esperado `R$ 18,15`), exibindo tudo com interpolação `$"..."` e `:C`.
   - Tente alterar `TAXA_SIMULADA` em outra linha, observe o erro (`CS0131`) e apague a linha inválida.
6. **Bloco `A01.05` — Conversão explícita e `CultureInfo`:**
   - Converta `"3"` usando `int.Parse` e `"5.50"` usando `decimal.Parse(..., CultureInfo.InvariantCulture)`.
   - Multiplique ambos e exiba formatado com `.ToString("C", CultureInfo.GetCultureInfo("pt-BR"))` (esperado: `R$ 16,50`).
   - Compare no caderno `"2" + "3"` (concatenação → `"23"`) com `2 + 3` (soma → `5`).
7. **Bloco `A01.06` — Algoritmo no papel:** escreva no papel os passos para somar três notas (`6`, `7` e `8`) e calcular a média (`7`).

---

## 5. Erros Comuns no Visual Studio 2022

| Erro na Lista de Erros | Causa provável | Como resolver |
| --- | --- | --- |
| `CS1002: ; esperado` | Esqueceu `;` no fim da linha (ou na linha acima). | Confira o final da linha marcada e da linha anterior. |
| `CS0664: Literal double não pode ser convertido em decimal` | Esqueceu o sufixo `m` (ex.: `5.50` em vez de `5.50m`). | Adicione `m` no final do número decimal. |
| `CS1012: Muitos caracteres no literal de caractere` | Usou aspas simples `'Arroz'` em texto. | Aspas simples são só para 1 `char`; textos usam `"Arroz"`. |

[Próxima aula: Aula 02 — Entrada, decisões, laços e depuração](02-Entrada-Decisoes-Lacos-e-Depuracao.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 01 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-01). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
