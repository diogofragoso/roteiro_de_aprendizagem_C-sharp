# Aula 02 — Console Interativo, Validação de Entrada e Depuração no VS 2022

**Duração:** 3h30. **Pré-requisito:** [Aula 01](01-Introducao-CSharp.md). **Entrega:** aplicativo de console de caixa com todas as entradas validadas (`TryParse` e `do/while`), regras de desconto por fronteira e depuração demonstrada com Breakpoint no Visual Studio 2022.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Revisar tipos da Aula 01 e o desafio da entrada do usuário |
| Explicação | 30 | `ReadLine`, `??`, `TryParse`, `out`, `do/while` e cultura `pt-BR` |
| Construção | 60 | Criar o projeto `TreinoSupermercado` no Visual Studio 2022 e montar o fluxo |
| Intervalo | 15 | Pausa |
| Prática | 60 | Blindar quantidade/preço com `TryParse`, implementar baixa de estoque e depurar |
| Entrega | 30 | Demonstrar casos normal/inválido (`abc`, `0`, `-1`, `3`) e registrar evidências |

---

## 1. Por Que Ainda Trabalhar no Console Antes de Criar Telas?

Aplicativos de console eliminam a complexidade de janelas, cliques e eventos para que você domine 100% o ciclo fundamental de qualquer sistema:

```text
Entrada de dados (ReadLine) → Validação e Conversão (TryParse) → Processamento (Regras) → Saída (WriteLine)
```

---

## 2. Criando o Projeto `TreinoSupermercado` no Visual Studio 2022

1. Abra o **Visual Studio 2022 Community**.
2. Clique em **Criar um novo projeto**.
3. Pesquise por `Console`, filtre pela linguagem **C#** e selecione **Aplicativo de Console** (modelo .NET moderno — **nunca** o modelo `.NET Framework`).
4. Clique em **Próximo**, informe o nome **`TreinoSupermercado`** e clique em **Próximo**.
5. Escolha a versão moderna do **.NET** adotada no laboratório (ex.: `.NET 8.0`), mantenha desmarcada a opção *"Não usar instruções de nível superior"* e clique em **Criar**.
6. Teste rapidamente com **`Ctrl+F5`** para confirmar que o projeto compila e executa.

---

## 3. O Perigo de Confiar no Que o Usuário Digita (`Convert` vs `TryParse`)

### Por que `Convert.ToInt32(Console.ReadLine())` é perigoso em um caixa?
Se o operador de caixa esbarrar na letra `"a"` ao digitar a quantidade e seu código usar `Convert.ToInt32` ou `int.Parse`, o programa lança uma **`FormatException`** e **fecha abruptamente no meio do atendimento**, perdendo toda a compra do cliente!

### Anatomia da Leitura Segura: `string?`, `??`, `do/while`, `TryParse`, `out`, `!` e `||`

Observe como cada peça trabalha em conjunto em um exemplo genérico de leitura de **número de parcelas**:

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

Dissecando **cada detalhe** e **por que usamos assim**:

| Elemento | O que faz? | Por que é necessário? |
| --- | --- | --- |
| `string? entrada` | Declara uma string que aceita `null`. | `Console.ReadLine()` retorna `null` quando a entrada do terminal é fechada (ex.: `Ctrl+Z`). Testar `if (entrada is null) return;` impede um laço infinito se a entrada acabar. |
| `?? ""` (quando usado em textos) | Operador de coalescência nula. | Em `string nome = Console.ReadLine() ?? "";`, garante que `nome` nunca fique `null`, virando `""` se vier nulo. **Atenção:** `?? ""` trata `null`, mas **não** impede texto vazio! |
| `do { ... } while (...);` | Laço que testa a condição **no final**. | Diferente do `while` comum, o `do/while` garante que a pergunta ao usuário seja feita **pelo menos uma vez** antes de decidir se precisa repetir. Note o ponto e vírgula `;` obrigatório no final do `while (...) ;`. |
| `int.TryParse(entrada, out parcelas)` | Tenta converter e entrega o resultado em `parcelas` (`out`). | Devolve `true` se o texto era um número inteiro e `false` se não era — sem nunca derrubar o programa. |
| `!` (negação) | Inverte o booleano do `TryParse`. | Queremos repetir quando **NÃO** conseguiu converter (`!int.TryParse(...)`). |
| `\|\|` (OU lógico com curto-circuito) | Repete se falhou a conversão **OU** se falhou a regra (`parcelas <= 0`). | Se o lado esquerdo for `true` (não converteu `"abc"`), o C# nem testa o lado direito e já repete o laço! |

### Tabela de Rastreamento: Conversão vs Regra de Negócio

| Texto digitado | `TryParse` converteu? | Atende `> 0`? | Resultado de `!TryParse \|\| <= 0` | Ação do laço |
| --- | --- | --- | --- | --- |
| `abc` | Não (`false`) | Não avaliado (curto-circuito) | `true` | Repete a pergunta |
| `0` | Sim (`true`) | Não (`0 <= 0` é `true`) | `true` | Repete a pergunta |
| `-1` | Sim (`true`) | Não (`-1 <= 0` é `true`) | `true` | Repete a pergunta |
| `3` | Sim (`true`) | Sim (`3 <= 0` é `false`) | `false` | **Sai do laço e aceita!** |

### Por que configurar `CultureInfo.CurrentCulture` no início do `Program.cs`?
Se um computador do laboratório estiver com o Windows em inglês, `decimal.TryParse("5,50", out preco)` falha ou lê errado porque espera ponto. Adicionando no topo do `Program.cs`:
- `using System.Globalization;`
- `CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");`

garantimos que **todos** os computadores aceitem `"5,50"` com vírgula na digitação e exibam `R$ 5,50` com `:C` — mesmo que dentro do código C# os literais continuem sendo escritos com ponto (`5.50m`).

---

## 4. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

Construa no `Program.cs` do projeto `TreinoSupermercado` o seu **Caixa de Console Validado** seguindo as etapas abaixo (não copie blocos prontos — monte cada peça entendendo o motivo):

### Etapa 1 — Cultura e Identificação do Produto (`A02.01` e `A02.02`)
1. No topo de `Program.cs`, adicione `using System.Globalization;` e configure `CultureInfo.CurrentCulture` para `"pt-BR"`.
2. Exiba um cabeçalho `"=== BEM-VINDO AO MARKETFLOW ==="`.
3. Peça o nome do produto com `Console.Write` e leia com `Console.ReadLine() ?? ""`.
4. **Experimento mental / no caderno (`A02.02`):** registre qual será o valor final de `string nome = entrada ?? "";` nos três cenários: `entrada` sendo `null`, `""` e `"Arroz"`.

### Etapa 2 — Validação de Quantidade e Preço (`A02.03` e `A02.04`)
1. *(Opcional de contraste)* Antes de usar `TryParse`, faça um teste rápido lendo com `Convert.ToInt32`, digite `"abc"`, observe a exceção `FormatException` no Visual Studio 2022 e entenda por que isso é inaceitável em produção.
2. Substitua a leitura da quantidade por um laço `do / while` usando `int.TryParse(entrada, out quantidade)`:
   - Encerre o programa com mensagem se `entrada is null`.
   - Repita enquanto a conversão falhar **OU** `quantidade <= 0`.
3. Construa um segundo laço `do / while` para o **preço unitário** (`decimal preco`) usando `decimal.TryParse(entrada, out preco)`:
   - Encerre se `entrada is null`.
   - Repita enquanto a conversão falhar **OU** `preco <= 0`.
4. Calcule `decimal total = quantidade * preco;`.
5. Exiba um **Comprovante Didático — Sem Valor Fiscal** usando interpolação de string (`$"..."`) e o formatador monetário `:C` (ex.: `$"Total a Pagar: {total:C}"`).

### Etapa 3 — Regra de Brinde/Cupom e Teste de Fronteiras (`A02.05`)
Logo abaixo do comprovante, construa uma estrutura `if / else if / else` sobre `total`:
- Se `total > 100.00m`: informe que o cliente ganhou cupom de 10% para a próxima compra.
- Senão, se `total > 50.00m`: informe que o cliente ganhou um brinde no balcão.
- Senão: exiba mensagem de agradecimento.
- **Teste obrigatório de fronteiras:** rode com valores que gerem totais exatos de `50,00`, `50,01`, `100,00` e `100,01` e explique no caderno o papel do operador `>` em vez de `>=`.

### Etapa 4 — Simulador de Baixa de Estoque com Decremento (`A02.06`)
Ao final do programa (ou em uma segunda seção clara), construa o controle de estoque de maçãs:
1. Peça ao usuário a quantidade inicial de maçãs no estoque usando `do / while` e `int.TryParse`:
   - **Atenção à diferença da regra:** estoque pode começar em **zero** (estoque vazio), mas **não** pode ser negativo (`estoque < 0`)!
2. Enquanto `estoque > 0` (`while`), decremente uma unidade (`estoque--`) e exiba `"Vendendo uma maçã. Estoque restante: {estoque}"`.
3. Quando o laço encerrar, exiba `"ALERTA: Estoque vazio! Solicitar reposição."`.
4. **Teste de fronteira:** informe `0` como estoque inicial e confirme que nenhuma venda é executada e o alerta aparece direto (sem gerar saldo negativo).

### Etapa 5 — Depuração Obrigatória no Visual Studio 2022 (`F9`, `F5`, `F10`)
1. Coloque um **Breakpoint (`F9`)** na linha `decimal total = quantidade * preco;` e outro na linha `estoque--;`.
2. Execute com **`F5`** (Iniciar com Depuração).
3. Digite entradas inválidas (`abc`, `0`, `-1`) e depois válidas (`3` unidades a `5,50`).
4. Quando o Visual Studio 2022 pausar na linha amarela, abra a janela **Locais**, inspecione `quantidade` e `preco`, avance com **`F10`** e confirme o cálculo `16,50`. Repita o acompanhamento em cada volta do laço `estoque--` com estoque inicial `3`.

---

## 5. Erros Comuns no Visual Studio 2022

| Sintoma / Erro | Causa provável | Como corrigir |
| --- | --- | --- |
| `CS1002: ; esperado` após `while (...)` | Esqueceu o ponto e vírgula ao final do `do { ... } while (...);`. | Coloque `;` logo depois do parêntese do `while`. |
| Digitar `5,50` falha na conversão | Não configurou `CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");` no início do `Program.cs`. | Adicione `using System.Globalization;` e configure a cultura `pt-BR` antes das leituras. |
| Laço repete mesmo digitando número válido | Inverteu a lógica do `while` (usou `&&` em vez de `\|\|` ou esqueceu o `!` no `TryParse`). | Revise a tabela de rastreamento na Seção 3: queremos repetir se **NÃO** converteu **OU** estiver fora da regra. |

[Anterior: Aula 01](01-Introducao-CSharp.md) · [Próxima: Aula 03 — Orientação a objetos](03-Orientacao-a-Objetos.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 02 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-02). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
