# F02 — Decisões, Repetições e Depuração no Visual Studio 2022

**Pré-requisito:** [F01](01-Valores-e-Entrada.md).
**Objetivo:** controlar quais trechos de código executam (decisões), repetir instruções com segurança (laços) e aprender a inspecionar a execução passo a passo com o **Depurador do Visual Studio 2022**.

Crie um novo **Aplicativo de Console** no Visual Studio 2022 chamado `FundamentosF02`.

---

## 1. Expressando Condições: Operadores Relacionais e Lógicos

Um `if` sempre avalia uma pergunta cuja resposta precisa ser um `bool` (`true` ou `false`). Para montar essas perguntas, usamos operadores:

### Operadores de Comparação (Relacionais)

| Operador | Significado | Exemplo | Resultado |
| --- | --- | --- | --- |
| `==` | É igual a? (**dois** sinais de igual!) | `5 == 5` | `true` |
| `!=` | É diferente de? | `5 != 3` | `true` |
| `>` / `>=` | Maior que / Maior ou igual a | `10 >= 10` | `true` |
| `<` / `<=` | Menor que / Menor ou igual a | `10 < 10` | `false` |

> [!WARNING]
> **Armadilha clássica:** `=` (um sinal) **atribui** valor (`x = 5`); `==` (dois sinais) **compara** valores (`x == 5`). No C#, se você escrever `if (x = 5)`, o Visual Studio 2022 avisa imediatamente com erro de compilação porque `x = 5` não é uma condição booleana.

### Operadores Lógicos (Combinando Perguntas)

| Operador | Nome | Quando resulta em `true`? | Exemplo prático |
| --- | --- | --- | --- |
| `&&` | **E** (*AND*) | Somente se **ambas** as condições forem verdadeiras. | `nota >= 0 && nota <= 10` (aceita todo o intervalo de 0 a 10) |
| `\|\|` | **OU** (*OR*) | Se **pelo menos uma** das condições for verdadeira. | `dia == 1 \|\| dia == 7` (aceita apenas os extremos 1 ou 7) |
| `!` | **NÃO** (*NOT*) | **Inverte** o resultado (`!true` vira `false`; `!false` vira `true`). | `!ativo` (verdadeiro quando `ativo` for `false`) |

**Por que existe "curto-circuito" em `&&` e `||`?**
Em `int.TryParse(entrada, out nota) && nota >= 0`:
- Se o lado esquerdo (`TryParse`) der `false`, o C# **nem executa** o lado direito (`nota >= 0`), porque em um **E (`&&`)** basta um lado ser falso para o resultado inteiro ser falso. Isso evita testar um número que nem conseguiu ser convertido!

---

## 2. Repetindo Até o Usuário Digitar Direito (`while`)

Em `F01`, se o usuário digitava `"abc"`, o programa exibia um aviso e **encerrava**. Em um sistema real, o programa precisa **repetir a pergunta** até receber um valor válido.

Observe a anatomia deste modelo de validação de temperatura positiva:

```csharp
int graus;

while (true)
{
    Console.Write("Informe a temperatura positiva: ");
    string? entrada = Console.ReadLine();
    if (entrada is null)
    {
        return;
    }

    if (int.TryParse(entrada, out graus) && graus > 0)
    {
        break;
    }

    Console.WriteLine("Valor inválido. Tente novamente.");
}

Console.WriteLine($"Temperatura registrada: {graus}");
```

Dissecando cada detalhe e **por que usamos assim**:
- `int graus;` foi declarada **fora** do `while`. **Por quê?** Porque uma variável declarada dentro das chaves `{ }` do `while` deixa de existir quando o laço termina — e precisamos usar `graus` na última linha, depois do laço!
- `while (true)`: cria um laço que continua repetindo até encontrar uma ordem explícita de saída.
- `string? entrada = Console.ReadLine();`: usamos `string?` (texto que pode ser `null`) para testar `if (entrada is null) return;`. **Por quê?** Se o fluxo de entrada for encerrado (fim de arquivo ou `Ctrl+Z`), repetir para sempre causaria um laço infinito; o `return;` encerra o programa de forma limpa.
- `break;`: comando que **quebra/sai imediatamente** do laço `while`, pulando para a linha após o fechamento `}`.
- **Perigo do laço infinito:** todo laço precisa ter uma condição de parada alcançável. Se um programa seu travar repetindo sem parar no Visual Studio 2022, clique no botão quadrado vermelho **Parar Depuração** (`Shift+F5`) ou feche a janela do console.

---

## 3. Contando Repetições Exatas (`for`)

Quando sabemos exatamente de onde começar, até onde ir e quanto avançar em cada passo, o laço `for` deixa tudo visível em uma única linha.

Anatomia do `for`:

```csharp
for (int passo = 1; passo <= 3; passo++)
{
    Console.WriteLine($"Passo atual: {passo}");
}
```

As **3 partes dentro dos parênteses**, separadas por ponto e vírgula `;`:
1. **Inicialização (`int passo = 1`):** executa **apenas uma vez**, antes do laço começar.
2. **Condição (`passo <= 3`):** verificada **antes** de cada repetição. Enquanto for `true`, o bloco `{ }` executa; quando vira `false` (quando `passo` chega a `4`), o laço termina.
3. **Atualização (`passo++`):** executa **ao final** de cada repetição. `passo++` é um atalho para `passo = passo + 1;` (e `passo--` subtrai 1).

**Teste de Mesa (rastreando no papel):**
Para acumular uma soma (`soma = soma + passo`) de 1 até 3:

| Passagem | `passo` | `soma` antes da linha | `soma` depois da linha |
| --- | ---: | ---: | ---: |
| 1ª | 1 | 0 | 1 |
| 2ª | 2 | 1 | 3 |
| 3ª | 3 | 3 | 6 |

---

## 4. Como Usar o Depurador (Debugger) do Visual Studio 2022

O depurador permite **congelar o tempo** enquanto seu código roda e olhar dentro das variáveis na memória RAM.

### Passo a passo no Visual Studio 2022:
1. Clique na **margem cinza à esquerda** do número da linha onde deseja pausar (ou posicione o cursor na linha e pressione **`F9`**). Uma **bolinha vermelha** (*Breakpoint*) aparecerá.
2. Execute o programa usando **`F5`** (*Iniciar com Depuração* — **não** use `Ctrl+F5` quando quiser depurar!).
3. Quando a execução alcançar a bolinha vermelha, a linha ficará **destacada em amarelo**. Isso significa que o programa pausou **imediatamente antes** de executar aquela linha.
4. **Inspecione as variáveis:**
   - Passe o mouse por cima do nome de qualquer variável para ver o valor atual; ou
   - Abra a janela **Depurar → Janelas → Locais** (`Ctrl+Alt+V, L`) para ver todas as variáveis do momento em tabela.
5. Pressione **`F10`** (*Contornar / Step Over*) para executar **somente a linha amarela** e avançar para a próxima. Observe o valor atualizado mudar de cor na janela **Locais**!
6. Pressione **`F5`** para soltar a execução até o próximo breakpoint (ou até o fim). Clique novamente na bolinha vermelha (ou `F9`) para removê-la quando terminar.

**Por que depurar em vez de chutar alterações?**
Se você escrever `passo < 3` em vez de `passo <= 3`, o código compila sem nenhum erro vermelho, mas o resultado sai errado (erro de lógica). Com `F9` e `F10`, você vê exatamente em qual passagem o laço parou cedo demais.

---

## 5. Roteiro de Construção Guiada (Sua Vez!)

Construa os exercícios abaixo no projeto `FundamentosF02` seguindo as orientações (sem copiar respostas prontas):

### Construção 1 — Validador de Idade em Faixa (`while`)
1. Declare uma variável `int idade;` antes do laço.
2. Crie um laço `while (true)` que peça ao usuário uma idade inteira entre `0` e `120`.
3. Leia a entrada como `string?` e encerre o programa (`return;`) se ela for `null`.
4. Monte uma condição `if` combinando `int.TryParse` com o intervalo válido (`idade >= 0 && idade <= 120`) usando `&&`.
5. Se tudo for verdadeiro, saia do laço com `break;`. Caso contrário, informe que a idade é inválida e deixe o laço repetir.
6. Após o laço, mostre a idade aceita.
7. **Testes obrigatórios:** digite `abc`, `-1`, `121`, `0` e `120`. Apenas `0` e `120` devem encerrar o laço!

### Construção 2 — Contagem e Soma com `for` + Depuração
1. Crie um laço `for` que mostre os números de `1` a `5` (não pode mostrar `0` nem `6`) e, ao terminar, exiba uma mensagem de encerramento.
2. Em seguida, crie uma variável `int soma = 0;` fora de um laço `for` de `1` até `5`, acumulando `soma = soma + numero;` a cada volta. Ao final, exiba a soma (resultado esperado: `15`).
3. **Experimento obrigatório no Depurador do VS 2022:**
   - Coloque um breakpoint (`F9`) na linha `soma = soma + numero;`.
   - Rode com `F5`, avance com `F10` e anote no caderno os valores de `numero` e `soma` antes e depois de cada passagem.

### Construção 3 — Reflexão sobre Fronteiras
- Escreva no seu caderno o que mudaria no comportamento do programa se uma condição de quantidade usasse `>= 0` em vez de `> 0`. Dê um exemplo de valor que seria aceito em uma e recusado na outra.

---

## 6. Erros Comuns no Visual Studio 2022

| Sintoma / Erro | Causa provável | Como resolver |
| --- | --- | --- |
| `CS0165: Uso de variável local não atribuída` | Tentou ler uma variável após um laço em que o compilador não tem certeza de que ela recebeu valor. | Garanta que todos os caminhos que saem do laço (`break`) atribuam valor à variável (ou inicialize-a com `= 0`). |
| O Breakpoint não pausa ao rodar | Você apertou `Ctrl+F5` (sem depuração) em vez de `F5`. | Pressione `F5` (botão verde preenchido *Iniciar* no topo do VS 2022). |
| O `for` executou uma vez a menos | Usou `<` em vez de `<=` na condição de parada. | Revise o valor limite e confirme com o depurador (`F10`). |

**Critério para avançar para F03:** construir um laço que recuse entradas fora de formato e fora da faixa, rastrear um `for` no papel e no depurador (`F9`/`F10`) e explicar com clareza a diferença entre `>` e `>=`, `&&` e `||`.

[Anterior: F01 — Valores e entrada](01-Valores-e-Entrada.md) · [Próximo: F03 — Métodos, parâmetros e retorno](03-Metodos.md)
