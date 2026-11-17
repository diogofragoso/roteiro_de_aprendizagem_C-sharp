# Antes do projeto — fundamentos de C# e orientação a objetos

Este é o ponto de partida para quem nunca programou ou ainda não entende orientação a objetos. Você vai criar programas pequenos no console, aprender a acompanhar sua execução e só depois organizar dados e comportamentos em objetos. Não é necessário conhecer banco de dados, telas, SQL ou arquitetura.

Ao concluir, você deve conseguir criar uma classe, construir dois objetos, explicar seu estado, chamar métodos, proteger alterações e fazer objetos colaborarem. O projeto MarketFlow será a etapa seguinte, em que esses conhecimentos serão aplicados.

## Como estudar

Depois de F03, use o [laboratório de POO em C#](POO-na-Pratica.md) como prática complementar aos encontros F04–F07. Os exemplos de leitura e avisos incluem saídas esperadas e um exercício de modelagem independente.

Em cada encontro: leia o problema, preveja a saída no papel, digite o exemplo, execute, altere uma entrada e explique o resultado. Em seguida, resolva a atividade sem copiar a solução do exemplo. Erros fazem parte da prática: leia a mensagem, localize a linha e altere uma coisa por vez.

Cada capítulo indica exatamente quais arquivos criar e quando substituir o código. Use um projeto de console separado por capítulo. Os exemplos são exercícios independentes da aplicação final: as classes de treino não devem ser copiadas sobre os modelos do supermercado.

## Sequência e tempo

Sugestão inicial: **8 encontros de 3h30, total de 28 horas**, antes das 70 horas do projeto. O percurso completo passa a ter **98 horas**. Esses tempos são uma previsão para planejamento; acrescente recuperação quando a turma precisar. Não comprima esses fundamentos na antiga aula 03.

| Etapa | Tema e leitura | Entrega para avançar |
| --- | --- | --- |
| F00 | [Primeiro programa e execução](00-Primeiro-Programa.md) | Criar, executar e corrigir um programa |
| F01 | [Valores, variáveis e entrada](01-Valores-e-Entrada.md) | Ler texto e número, calcular e apresentar |
| F02 | [Decisões, repetições e depuração](02-Logica-e-Depuracao.md) | Recusar entrada inválida e rastrear um laço |
| F03 | [Métodos, parâmetros e retorno](03-Metodos.md) | Separar cálculo de leitura e apresentação |
| F04 | [Classes, objetos e referências](04-Classes-e-Objetos.md) | Criar dois objetos e explicar uma referência compartilhada |
| F05 | [Construtores e encapsulamento](05-Construtores-e-Encapsulamento.md) | Construir estado válido e impedir alteração inválida |
| F06 | [Composição e coleções](06-Composicao-e-Colecoes.md) | Fazer um objeto usar outros e calcular um total |
| F07 | [Interfaces, polimorfismo e transição](07-Interfaces-e-Transicao.md) | Trocar um comportamento pelo mesmo contrato e explicar o fluxo |

Use em cada encontro a seguinte distribuição, ajustando o ritmo a partir das evidências:

| Momento | Minutos | Ação |
| --- | ---: | --- |
| Retomada | 15 | Explicar e alterar um exemplo anterior |
| Explicação | 30 | Introduzir o problema e os termos novos |
| Construção acompanhada | 60 | Digitar, prever, executar e depurar |
| Intervalo | 15 | Pausa |
| Prática | 60 | Resolver os exercícios e comparar resultados |
| Verificação | 30 | Demonstrar individualmente e registrar dificuldades |

## Evidências e recuperação

Mantenha um registro por etapa, identificado de F00 a F07:

```text
Etapa e estudante:
Arquivo e trecho que consigo explicar:
Entrada ou alteração realizada:
Resultado previsto:
Resultado observado:
Por que o resultado aconteceu:
Erro encontrado e como investiguei:
Apoio utilizado:
Pendência e próxima tentativa:
```

Uma captura sozinha não demonstra compreensão. Cada estudante deve explicar uma alteração escolhida pelo professor, mesmo quando programou em dupla. O critério de avanço está no fim de cada capítulo. Se ainda não o demonstrar, refaça uma variação pequena do exercício antes de acrescentar o conceito seguinte.

Não é necessário acertar de memória todos os símbolos. É necessário localizar a declaração, identificar a chamada, acompanhar os valores e justificar o comportamento observado.

## Depois dos fundamentos

Use as aulas [01](../01-Introducao-CSharp.md), [02](../02-Pratica-Console.md) e [03](../03-Orientacao-a-Objetos.md) como revisão aplicada ao supermercado, com as entregas próprias do projeto. A [aula 04](../04-Arquitetura-e-Padroes.md) introduz a separação de responsabilidades; a aula 05 inicia a aplicação Windows Forms. Banco de dados entra somente depois.

Para uma turma com experiência, o professor pode aplicar os critérios F00–F07 como diagnóstico e registrar quais etapas já foram demonstradas. Para iniciantes, siga a sequência inteira.

[Começar em F00](00-Primeiro-Programa.md) · [Índice geral](../README.md)
