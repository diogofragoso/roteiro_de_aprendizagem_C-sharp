# F00 — Do enunciado ao primeiro programa

**Pré-requisito:** nenhum conhecimento de programação. **Objetivo:** criar um programa pequeno e explicar a ordem de execução.

## 1. O que é um programa?

Um programa contém instruções que o computador executa. Antes de escrever C#, precisamos dizer o que queremos que aconteça. Por exemplo, para descobrir o total de dois cadernos de 12 reais:

```text
Conhecer a quantidade: 2.
Conhecer o preço de cada caderno: 12.
Multiplicar quantidade pelo preço.
Mostrar o resultado: 24.
```

Essa sequência é um algoritmo. Uma instrução como “calcule a compra corretamente” ainda não explica quais dados serão usados nem qual operação deve acontecer.

**Faça no papel:** descreva como calcular o dobro de um número. Marque os dados necessários, a operação e o resultado que será mostrado. Teste com 3 e com 0.

## 2. Reconheça as ferramentas

| Termo | Para que serve neste percurso |
| --- | --- |
| C# | Linguagem em que escrevemos as instruções |
| .NET SDK | Ferramentas para compilar e executar nossos projetos |
| Visual Studio | Editor e ferramentas para criar arquivos, compilar e depurar |
| Projeto | Configuração e arquivos de um programa; o `.csproj` descreve sua configuração |
| Solução | Organização que pode reunir vários projetos |
| Console | Janela de texto para a entrada e a saída do exercício |

O professor deve preparar o laboratório antes deste encontro, seguindo a [configuração de ambiente](../05-Configuracao-Ambiente.md). Nesta etapa, apenas a preparação da IDE e do SDK dessa página é necessária. Criar uma conta de banco ou instalar XAMPP ainda não faz parte da atividade.

## 3. Crie o projeto

1. Abra o Visual Studio e escolha **Criar um novo projeto**.
2. Procure **Aplicativo de Console**, com linguagem **C#**. Use o modelo .NET, evitando o modelo antigo `.NET Framework`.
3. Nomeie o projeto `FundamentosF00` e salve na pasta de estudos.
4. Selecione **.NET 10**, conforme o ambiente deste material. Mantenha as instruções de nível superior do modelo, isto é, o `Program.cs` sem uma classe `Program` escrita manualmente.
5. Abra `Program.cs`, substitua seu conteúdo pelo exemplo abaixo e salve.

```csharp
Console.WriteLine("Começo do programa");
Console.WriteLine("Estou aprendendo C#");
Console.WriteLine("Fim do programa");
```

Execute com **Ctrl+F5**. Leia as três linhas na ordem. Encerre a execução antes de editar e executar novamente.

`Console.WriteLine` é uma operação pronta para mostrar uma linha. Os parênteses contêm a informação entregue a essa operação. As aspas delimitam o texto; não aparecem na saída. O ponto e vírgula encerra cada instrução do exemplo. Os detalhes de classe e método serão estudados depois: por enquanto, use essa operação para observar resultados.

Um comentário começa com `//` e serve para quem lê o código:

```csharp
// Esta linha explica a intenção e não é uma mensagem na tela.
Console.WriteLine("Somente esta mensagem será mostrada");
```

## 4. Investigue um erro de cada vez

No primeiro exemplo, retire temporariamente a aspa que fecha um dos textos. Tente compilar. Localize a mensagem na **Lista de Erros**, abra a linha indicada e recoloque a aspa. Uma falha de sintaxe pode gerar várias mensagens; comece pela primeira e confira também a linha anterior.

Compilar verifica e transforma o código para execução. Executar é fazer o programa funcionar. Um programa que compila ainda pode produzir um resultado diferente do que o enunciado pediu.

Se a IDE oferecer executar uma compilação anterior após uma falha, cancele: corrija o código antes de conferir sua saída.

## 5. Pratique

1. Antes de executar, escreva qual será a saída se trocar a primeira e a última instrução.
2. Mostre seu primeiro nome fictício, uma atividade e uma meta, cada informação em uma linha.
3. Escreva no papel os passos para somar três notas e dividir o resultado por três. Faça a conta com 6, 7 e 8; ainda não precisa programá-la.
4. Provoque um erro de aspa, registre a mensagem e entregue o programa corrigido.

**Critério para avançar:** criar e executar o próprio console, prever a ordem das mensagens e explicar a diferença entre código, mensagem de saída e erro de compilação. A média do exercício no papel deve ser 7.

[Anterior: percurso](README.md) · [Próximo: valores e entrada](01-Valores-e-Entrada.md)
