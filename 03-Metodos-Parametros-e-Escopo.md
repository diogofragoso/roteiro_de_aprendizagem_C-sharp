# Aula 03 — Métodos, Parâmetros, Retorno, Escopo e Depuração com `F11`

**Duração:** 3h30. **Pré-requisito:** [Aula 02](02-Entrada-Decisoes-Lacos-e-Depuracao.md). **Entrega:** funções locais reutilizáveis no console separando cálculos e regras da exibição na tela, demonstração de escopo/cópia de valor e depuração entrando em métodos com `F11` no Visual Studio 2022 (`A03.01`–`A03.05`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Observar trechos repetidos no console da Aula 02 |
| Explicação | 30 | Assinatura de métodos, retorno vs `void`, escopo, cópia de valor e `F11` |
| Construção | 60 | Construir funções puras de cálculo e validação no Visual Studio 2022 |
| Intervalo | 15 | Pausa |
| Prática | 60 | Refatorar o caixa de console separando leitura, cálculo e exibição + depurar `F11` |
| Entrega | 30 | Demonstrar chamadas, erro de escopo e rastreamento com `F11` |

---

## 1. Por Que Criar Métodos? (Separação Entre Calcular e Exibir)

Na Aula 02, escrevemos quase toda a lógica em sequência no `Program.cs`. Conforme um programa cresce, repetir fórmulas ou laços de validação em vários lugares torna qualquer mudança arriscada.

Um **método** (ou função) dá um **nome claro** a uma operação para que possamos escrevê-la **uma única vez** e reutilizá-la com argumentos diferentes.

### Anatomia Completa de um Método (Microexemplo Genérico)

```csharp
// Chamadas (onde a função realmente executa)
decimal areaSala = CalcularArea(4.5m, 3m);
ExibirArea(areaSala);

// Função com retorno (calcula, mas NÃO escreve na tela!)
decimal CalcularArea(decimal largura, decimal comprimento)
{
    decimal resultado = largura * comprimento;
    return resultado;
}

// Função void (executa uma ação, não devolve valor)
void ExibirArea(decimal valor)
{
    Console.WriteLine($"Área: {valor} m²");
}
```

| Parte | O que é? | Por que usamos dessa forma? |
| --- | --- | --- |
| `decimal` / `void` | **Tipo de retorno.** | `decimal` promete devolver um número via `return`. `void` indica que a função apenas executa uma ação e não retorna valor (não pode ser guardada em variável!). |
| `CalcularArea` | **Nome do método.** | Em C#, usamos **PascalCase** e verbos claros para facilitar a leitura. |
| `(decimal largura, decimal comprimento)` | **Parâmetros.** | Entradas exigidas pela função (variáveis locais que nascem na chamada). |
| `4.5m, 3m` | **Argumentos.** | Valores concretos entregues na chamada daquele momento. |
| `return resultado;` | **Retorno.** | Encerra a função e devolve o valor para quem chamou. |

> [!TIP]
> **Princípio de ouro que usaremos no MarketFlow inteiro:** funções de **cálculo ou validação** retornam dados (`decimal`, `bool`, `int`) e **não** chamam `Console.WriteLine` nem `MessageBox.Show`. Assim, a mesma regra funciona tanto no console hoje quanto nas telas Windows Forms e testes automatizados depois!

---

## 2. Escopo e Passagem por Cópia de Valor

- **Escopo:** cada par de chaves `{ }` delimita onde uma variável existe. Os parâmetros (`largura`) e variáveis criadas dentro de `CalcularArea` (`resultado`) **só existem enquanto a função está executando** — tentar lê-los fora gera erro `CS0103`. Isso protege o resto do programa contra alterações acidentais.
- **Cópia de Valor (`int`, `decimal`, `bool`):** quando você passa uma variável primitiva (`int numero = 3;`) para um parâmetro `int valor`, a função recebe uma **cópia independente**. Alterar `valor = valor * 2;` dentro da função **não** altera `numero` fora dela! (Na Aula 04 você verá a grande diferença quando passarmos **objetos de classes**.)

---

## 3. Depurando Chamadas no Visual Studio 2022 (`F11` vs `F10`)

Quando um Breakpoint (`F9`) para em uma linha que **chama** uma função sua (ex.: `decimal total = CalcularSubtotal(5.50m, 3);`):
- **`F10` (*Step Over* / Contornar):** executa a função inteira sem entrar nela e já pula para a próxima linha.
- **`F11` (*Step Into* / Intervir):** **entra** na primeira linha de dentro da função! Na janela **Locais**, você verá os argumentos recebidos, acompanhará o cálculo com `F10` e verá a seta amarela voltar exatamente para o ponto da chamada trazendo o retorno.

---

## 4. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

Continue evoluindo o projeto console **`TreinoSupermercado`** no **Visual Studio 2022** (mantendo as chamadas no topo de `Program.cs` e as declarações de funções locais na parte inferior):

1. **Funções puras de cálculo (`A03.01` e `A03.02`):**
   - Construa `decimal CalcularSubtotal(decimal preco, int quantidade)` retornando `preco * quantidade`. Teste com `(12.50m, 2)` → `25` e `(5.50m, 3)` → `16,50`.
   - Construa `decimal CalcularMedia(decimal primeira, decimal segunda)` retornando `(primeira + segunda) / 2m`. Teste com `(6m, 8m)` → `7` (por que `2m` e parênteses são obrigatórios?).
   - Construa `decimal CalcularDesconto(decimal valorCompra)` implementando a nova regra de desconto progressivo do supermercado: compras `>= 500.00m` ganham `50.00m` de desconto; compras `>= 200.00m` ganham `15.00m`; abaixo disso, `0m`. Retorne apenas o valor calculado **sem imprimir nada dentro da função** (teste as fronteiras `199.99m`, `200.00m`, `499.99m` e `500.00m`).
2. **Função booleana de validação (`A03.03`):**
   - Construa `bool EhQuantidadeValida(int quantidade)` retornando `quantidade > 0`.
   - Teste com `-1`, `0` e `2`, usando o retorno em um `if / else` no `Program.cs` para exibir a mensagem (mantendo a apresentação fora da função).
3. **Função reutilizável de leitura segura + ação `void` (`A03.04`):**
   - Extraia a leitura segura com `do / while` + `int.TryParse` da Aula 02 para uma função `int LerInteiroPositivo(string mensagem)`. Chame-a duas vezes (ex.: quantidade e número de vias) sem duplicar o laço!
   - Crie uma função `void ExibirComprovante(string produto, int qtd, decimal preco, decimal subtotal, decimal desconto)` responsável exclusivamente por mostrar os dados formatados com `:C`.
4. **Experimento de escopo, cópia de valor e depuração `F11` (`A03.05`):**
   - Escreva `int Dobrar(int valor)` que faz `valor = valor * 2; return valor;`. Passe `int numero = 3;`, exiba `numero` e o retorno comprovando que `numero` continuou `3` e o retorno foi `6`.
   - Tente ler uma variável interna de `CalcularSubtotal` fora dela, leia o erro `CS0103` na **Lista de Erros** do VS 2022 e apague a linha inválida.
   - Coloque um Breakpoint (`F9`) na chamada a `CalcularSubtotal`, rode com **`F5`**, entre na função com **`F11`**, inspecione os parâmetros em **Locais** e acompanhe o retorno até o `Program.cs`.

---

## 5. Erros Comuns no Visual Studio 2022

| Erro na Lista de Erros | Causa provável | Como resolver |
| --- | --- | --- |
| `CS0161: Nem todos os caminhos de código retornam um valor` | Função prometeu `decimal`/`bool`, mas falta `return` em algum `else`. | Garanta que todo caminho termine com `return`. |
| `CS0029: Não é possível converter void em ...` | Tentou atribuir o resultado de uma função `void` a uma variável. | Chame funções `void` isoladamente: `ExibirComprovante(...);`. |
| `CS7036: Nenhum argumento fornecido para o parâmetro` | Faltou passar um argumento exigido na chamada. | Confira quantidade, ordem e tipos nos parênteses. |

[Anterior: Aula 02](02-Entrada-Decisoes-Lacos-e-Depuracao.md) · [Próxima: Aula 04 — Classes, objetos e referências](04-Classes-Objetos-e-Referencias.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 03 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-03). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
