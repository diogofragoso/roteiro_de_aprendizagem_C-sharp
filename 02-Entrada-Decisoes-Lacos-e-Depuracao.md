# Aula 02 — Entrada Segura, Decisões, Repetições e Depuração no VS 2022

**Duração:** 3h30. **Pré-requisito:** [Aula 01](01-Primeiro-Programa-e-Tipos.md). **Entrega:** aplicativo de console interativo do caixa com validação blindada (`TryParse` + `do/while`), regras de desconto (`if`/`else if`/`else`), menu simulado (`switch/case`), laços (`for`, `while`, `foreach`) e depuração demonstrada com Breakpoint no Visual Studio 2022 (`A02.01`–`A02.08`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Revisar tipos da Aula 01 e o perigo de entradas inválidas do usuário |
| Explicação | 30 | `ReadLine`, `??`, `TryParse`/`out`, operadores, `if`/`switch`, laços e Depurador |
| Construção | 60 | Construir leitura validada de produto, quantidade e preço com `do/while` |
| Intervalo | 15 | Pausa |
| Prática | 60 | Construir descontos, menu `switch`, laços (`for`/`while`/`foreach`) e depurar |
| Entrega | 30 | Demonstrar entradas inválidas (`abc`, `0`, `-1`), Breakpoint (`F9`/`F10`) e evidências |

---

## 1. Lendo Dados do Teclado com Segurança (`ReadLine`, `??`, `TryParse` e `out`)

Tudo que o usuário digita no console entra como **texto** por `Console.ReadLine()`, cujo tipo de retorno é **`string?`** (pode retornar `null` se o fluxo de entrada for encerrado com `Ctrl+Z`).

- **Operador `??` (coalescência nula):** em `string nome = Console.ReadLine() ?? "";`, se vier `null`, usa `""`. **Atenção:** `?? ""` trata `null`, mas **não** impede que o usuário apenas aperte Enter deixando texto vazio!
- **Por que NÃO usar `Convert.ToInt32` nem `int.Parse` direto no que o usuário digita?** Se o caixa esbarrar na letra `"a"`, `Convert.ToInt32` lança `FormatException` e **fecha o programa inteiro na cara do cliente!**
- **A solução profissional (`TryParse` + `out` + `do/while`):** observe este microexemplo genérico de leitura de **parcelas**:

```csharp
int parcelas;
string? entrada;

do
{
    Console.Write("Número de parcelas (maior que zero): ");
    entrada = Console.ReadLine();
    if (entrada is null)
    {
        Console.WriteLine("Entrada encerrada.");
        return;
    }
} while (!int.TryParse(entrada, out parcelas) || parcelas <= 0);
```

Dissecando **cada peça** e o seu motivo:
- **`do { ... } while (...);`**: executa a pergunta ao menos uma vez e testa no final se precisa repetir (note o `;` obrigatório após `while(...)`).
- **`if (entrada is null) return;`**: impede laço infinito caso a entrada do terminal seja fechada.
- **`int.TryParse(entrada, out parcelas)`**: tenta converter sem nunca travar. Retorna `true`/`false` e, pelo modificador **`out`**, entrega o número convertido dentro da variável `parcelas`.
- **`!` e `||` (curto-circuito):** repete se **NÃO** conseguiu converter (`!TryParse`) **OU** se o número violou a regra (`parcelas <= 0`). Se digitarem `"abc"`, o lado esquerdo já dá `true` e o C# nem testa `<= 0`!
- **Diferença fundamental:** `"abc"` é **falha de conversão**; `0` e `-1` são **conversões válidas** (são inteiros!), mas **reprovadas pela regra de negócio**.

---

## 2. Operadores, Decisões (`if` vs `switch`) e Repetições (`for`, `while`, `foreach`)

### Operadores Relacionais e Lógicos
- **Relacionais:** `==` (igual — dois sinais! Nunca confunda com `=` de atribuição), `!=` (diferente), `>`, `>=`, `<`, `<=`.
- **Lógicos:** `&&` (E: ambas verdadeiras), `||` (OU: ao menos uma verdadeira), `!` (NÃO: inverte `true`/`false`).

### Quando escolher `if / else if / else` vs `switch / case`?
- **`if / else if / else`:** use para **faixas de valores** (`>= 500`, `>= 200`, `> 100`) ou condições combinadas com `&&`/`||`. Teste sempre da faixa mais alta para a mais baixa!
- **`switch / case`:** use quando compara **uma única variável** contra opções exatas (ex.: menu `1`, `2`, `3`). Todo `case` termina com `break;`, e `default:` trata opções inválidas.

### Quando escolher `for`, `while` ou `foreach`?
- **`for (int i = 1; i <= 3; i++)`**: quando sabe **quantas vezes exatas** vai repetir (inicialização; condição; incremento `i++` ou decremento `i--`).
- **`while (condicao)`**: quando **não sabe** quantas voltas serão, mas conhece a **condição de parada** (ex.: enquanto `!estavel && tentativas < 5`, ou enquanto `estoque > 0`).
- **`foreach (string item in colecao)`**: ideal para percorrer todos os elementos de um array ou lista um por um, sem risco de errar índices.

---

## 3. Como Usar o Depurador (Debugger) do Visual Studio 2022 (`F9`, `F5`, `F10`)

Um programa pode compilar com zero erros vermelhos e ainda ter um **erro de lógica** (ex.: escrever `< 3` em vez de `<= 3`). Para investigar sem "chutar":

1. Clique na margem esquerda da linha desejada (ou pressione **`F9`**) para colocar a bolinha vermelha (**Breakpoint**).
2. Execute com **`F5`** (*Iniciar com Depuração* — **não** `Ctrl+F5`!).
3. Quando pausar na **linha amarela**, abra **Depurar → Janelas → Locais** (`Ctrl+Alt+V, L`) ou passe o mouse sobre as variáveis para ver os valores na memória RAM.
4. Pressione **`F10`** (*Contornar / Step Over*) para executar uma linha por vez e ver os valores mudarem.
5. Pressione **`F5`** para continuar; se criar um laço infinito por engano, clique no botão quadrado vermelho **Parar Depuração** (`Shift+F5`).

---

## 4. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

No projeto `TreinoSupermercado` (mantendo `using System.Globalization;` e `CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");` no topo de `Program.cs`), construa o seu **Caixa de Console Validado** cobrindo **`A02.01` a `A02.08`**:

1. **Entrada do produto e leitura validada (`A02.01`, `A02.02`, `A02.03`, `A02.04`):**
   - Pergunte o nome do produto (`Console.Write` + `Console.ReadLine() ?? ""`).
   - Construa um laço `do / while` com `int.TryParse` para ler uma **quantidade inteira maior que zero** (encerrando se `entrada is null`). Teste digitando `abc`, `0`, `-1` e `3` — só `3` deve sair do laço!
   - Construa um segundo laço `do / while` com `decimal.TryParse` para ler um **preço unitário positivo** (teste `abc`, `0` e `5,50`).
   - Calcule `decimal total = quantidade * preco;` e imprima um **Comprovante Didático — Sem Valor Fiscal** com interpolação e `:C` (`3 × 5,50 = R$ 16,50`).
2. **Regras de brinde/cupom e fronteiras (`A02.05`):**
   - Logo abaixo do comprovante, crie um `if / else if / else` sobre `total`: maior que `100.00m` ganha cupom de 10%; maior que `50.00m` ganha brinde; caso contrário, agradecimento.
   - Teste fronteiras exatas que gerem `50,00`, `50,01`, `100,00` e `100,01` (explicando `>` vs `>=`).
3. **Menu de pagamento simulado (`A02.06`):**
   - Peça uma opção inteira (`1`-Dinheiro, `2`-Cartão, `3`-Pix) lendo com segurança via `int.TryParse(Console.ReadLine(), out int opcao)` *(se o usuário digitar texto inválido, `opcao` recebe `0` e cai automaticamente no `default:` sem travar o programa!)*.
   - Use `switch (opcao)` com `case 1:`, `case 2:`, `case 3:`, `break;` em cada ramo e `default:` para opção inválida. Comprove as quatro saídas.
4. **Três vias com `for`, balança com `while` e catálogo com `foreach` (`A02.07`):**
   - Use `for` para imprimir exatamente `"Via 1"`, `"Via 2"` e `"Via 3"`.
   - Use `while (!balancaEstavel && tentativas < 5)` simulando 5 verificações de balança (sem laço infinito!).
   - Percorra um `string[]` com 4 nomes de produtos usando `foreach`.
5. **Simulador de estoque com decremento e Depuração (`A02.08`):**
   - Leia o estoque inicial de maçãs com `do / while` aceitando **`>= 0`** (zero é estoque vazio válido; negativo é recusado!).
   - Enquanto `estoque > 0`, faça `estoque--` e imprima o restante; ao encerrar, exiba `"ALERTA: Estoque vazio! Solicitar reposição."`. Teste também com estoque inicial `0`.
   - **Depuração obrigatória no VS 2022:** coloque Breakpoints (`F9`) na multiplicação `quantidade * preco` e na linha `estoque--`, rode com `F5`, avance com `F10` e registre no caderno os valores observados na janela **Locais** antes e depois de cada passo!

---

## 5. Erros Comuns no Visual Studio 2022

| Sintoma / Erro | Causa provável | Como resolver |
| --- | --- | --- |
| Breakpoint não pausa ao executar | Usou `Ctrl+F5` em vez de `F5`. | Pressione `F5` (botão verde preenchido *Iniciar*). |
| `CS0163: O controle não pode avançar de um rótulo de case` | Faltou `break;` em um `case` do `switch`. | Encerre todo `case` e o `default` com `break;`. |
| Digitar `5,50` falha no `decimal.TryParse` | Faltou configurar `CultureInfo.CurrentCulture` para `"pt-BR"`. | Configure no início do `Program.cs` antes de ler dados. |

[Anterior: Aula 01](01-Primeiro-Programa-e-Tipos.md) · [Próxima: Aula 03 — Métodos, parâmetros e escopo](03-Metodos-Parametros-e-Escopo.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 02 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-02). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
