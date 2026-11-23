# Antes do Projeto — Fundamentos de C# e Orientação a Objetos no Visual Studio 2022

Este é o ponto de partida oficial para quem **nunca programou** ou ainda está dando os primeiros passos em **C#** e **Orientação a Objetos**. Aqui você vai construir programas pequenos no **Console** usando o **Visual Studio 2022**, entender exatamente o que cada símbolo faz na memória do computador e descobrir **por que** escrevemos código de determinadas maneiras — sem precisar saber banco de dados, telas gráficas ou arquitetura ainda.

Ao terminar as oito etapas (`F00` a `F07`), você será capaz de ler dados do teclado com segurança, depurar seu código linha por linha, criar métodos reutilizáveis, modelar classes com encapsulamento, conectar objetos por composição e trabalhar com interfaces e polimorfismo. Só depois disso você iniciará a construção do sistema de supermercado **MarketFlow**.

---

## Como Estudar com Este Guia (Sem Copiar Código)

Em cada etapa de `F00` a `F07`, você vai encontrar:

1. **Explicação do zero e o "Por quê?":** entenda qual problema cada recurso da linguagem C# resolve antes de escrever código.
2. **Anatomia da sintaxe com microexemplos:** exemplos curtos e comentados em detalhe para você entender o significado de cada palavra-chave e símbolo.
3. **Passo a passo no Visual Studio 2022:** exatamente onde clicar, quais atalhos usar e como interpretar a **Lista de Erros** e o **Depurador**.
4. **Roteiro de construção própria:** orientações claras para você **construir seus próprios programas** a partir do zero, sem respostas prontas para copiar.

> [!TIP]
> **Regra de ouro para aprender de verdade:** antes de apertar `Ctrl+F5` ou `F5` no Visual Studio 2022, escreva no papel o que você espera que apareça na tela. Se o resultado for diferente, use o depurador para investigar o motivo — nunca fique "chutando" alterações aleatórias até funcionar.

- Crie um projeto de **Aplicativo de Console** separado para cada etapa (`FundamentosF00`, `FundamentosF01`, ..., `FundamentosF07`).
- A partir da etapa `F04`, utilize também o [**Laboratório de POO na Prática**](POO-na-Pratica.md) como reforço extra de modelagem e leitura de código.
- Os projetos desta pasta são exercícios de treino independentes: não copie essas classes para dentro do projeto do supermercado depois.

---

## Sequência e Tempo Sugerido

Planejamento recomendado: **8 encontros de 3h30 (210 minutos), totalizando 28 horas** antes das 70 horas do projeto MarketFlow (**98 horas no total**). Use os critérios de avanço ao final de cada capítulo para saber se está pronto para a etapa seguinte.

| Etapa | Tema e Leitura | O que você precisa conseguir construir e explicar |
| --- | --- | --- |
| F00 | [Primeiro programa e Visual Studio 2022](00-Primeiro-Programa.md) | Criar um projeto Console, executar, prever a ordem de saída e corrigir erro de sintaxe |
| F01 | [Valores, variáveis, tipos e entrada](01-Valores-e-Entrada.md) | Declarar variáveis tipadas, acompanhar atribuições e converter entrada com `TryParse` |
| F02 | [Decisões, repetições e depuração](02-Logica-e-Depuracao.md) | Validar entradas em laço, construir `for`/`while` e usar Breakpoint (`F9`/`F10`) |
| F03 | [Métodos, parâmetros, retorno e escopo](03-Metodos.md) | Separar cálculos em funções com retorno, entender escopo e depurar com `F11` |
| F04 | [Classes, objetos, propriedades e referências](04-Classes-e-Objetos.md) | Criar classes em arquivos separados, instanciar objetos e explicar referências na memória |
| F05 | [Construtores, campos e encapsulamento](05-Construtores-e-Encapsulamento.md) | Proteger o estado do objeto com construtor, `private set`, `throw` e `try/catch` |
| F06 | [Composição e coleções (`List<T>`)](06-Composicao-e-Colecoes.md) | Conectar objetos na relação "tem um", percorrer listas e calcular totais |
| F07 | [Interfaces, polimorfismo e transição](07-Interfaces-e-Transicao.md) | Criar contratos, injetar dependências pelo construtor e trocar comportamentos |

### Distribuição sugerida de cada encontro (210 minutos)

| Momento | Minutos | Ação |
| --- | ---: | --- |
| Retomada | 15 | Revisar e alterar um detalhe de um programa da etapa anterior |
| Explicação guiada | 30 | Entender o problema real, os novos termos e os motivos técnicos |
| Construção guiada | 60 | Explorar a anatomia da sintaxe no Visual Studio 2022, prever e depurar |
| Intervalo | 15 | Pausa |
| Prática autônoma | 60 | Construir os exercícios seguindo as orientações do capítulo |
| Verificação | 30 | Demonstrar individualmente, explicar escolhas e registrar evidências |

---

## Registro de Evidências (F00 a F07)

Ao concluir cada etapa, preencha um registro curto para acompanhar sua própria evolução:

```text
Etapa e estudante:
Projeto / arquivo construído:
Entrada ou teste realizado:
Resultado previsto no papel:
Resultado observado no console:
Por que esse resultado aconteceu:
Erro encontrado no VS 2022 e como investiguei:
O que ainda precisa de revisão:
```

Não é necessário decorar todos os símbolos de cabeça no primeiro dia. O objetivo é **entender o que cada peça significa**, saber localizar declarações e chamadas no Visual Studio 2022 e conseguir justificar o comportamento do seu programa.

---

## O Que Vem Depois dos Fundamentos

Concluindo o diagnóstico final de `F07`, você estará pronto para iniciar a trilha principal do projeto MarketFlow:

- Aulas [**01**](../01-Introducao-CSharp.md), [**02**](../02-Pratica-Console.md) e [**03**](../03-Orientacao-a-Objetos.md): aplicação dos fundamentos no contexto do supermercado.
- Aula [**04**](../04-Arquitetura-e-Padroes.md): separação de responsabilidades e arquitetura em camadas.
- Aulas [**05**](../05-Configuracao-Ambiente.md) em diante: criação do aplicativo visual Windows Forms no Visual Studio 2022 e integração com banco de dados.

[Começar em F00 — Primeiro Programa](00-Primeiro-Programa.md) · [Índice Geral](../README.md)
