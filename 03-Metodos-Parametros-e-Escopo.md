# Aula 03 — Métodos, Parâmetros, Retorno, Escopo e Depuração com `F11`

**Duração:** 3h30. **Pré-requisito:** [Aula 02](02-Entrada-Decisoes-Lacos-e-Depuracao.md). **Entrega:** funções locais reutilizáveis no console separando cálculos e regras da exibição na tela, demonstração prática de escopo e cópia de valor, e depuração entrando dentro de métodos com `F11` no Visual Studio 2022 (`A03.01`–`A03.05`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Identificar trechos duplicados no console da Aula 02 |
| Explicação | 30 | Assinatura de métodos, retorno vs `void`, funções puras, escopo, cópia de valor e `F11` |
| Construção | 60 | Executar os Exemplos Completos #1 e #2 (cálculos puros, `bool`, `void` e reutilização) |
| Intervalo | 15 | Pausa |
| Prática | 60 | Executar o Exemplo #3 (escopo e cópia de valor), depurar com `F11` e refatorar o caixa |
| Entrega | 30 | Demonstrar chamadas, erro `CS0103`, rastreamento `F11` e registrar evidências |

---

## 1. Por Que Criar Métodos? (Reuso + Separação Entre Calcular e Exibir)

Na Aula 02, escrevemos toda a lógica em sequência dentro do `Program.cs`. Agora imagine que você precise ler 3 números positivos diferentes no mesmo programa (quantidade, número de vias e estoque): sem métodos, você teria que copiar e colar o mesmo laço `do / while` três vezes!

Um **método** (ou **função**) empacota uma tarefa com um **nome claro** para que você escreva a lógica **uma única vez** e a chame quantas vezes quiser com valores diferentes.

### Anatomia Completa de um Método

```text
TipoDeRetorno NomeEmPascalCase(Tipo1 parametro1, Tipo2 parametro2)
{
    // Corpo do método (código que executa quando ele é chamado)
    return valorDoMesmoTipo; // Obrigatório quando TipoDeRetorno NÃO é void
}
```

| Parte | Exemplo | O que significa? |
| --- | --- | --- |
| **Tipo de retorno** | `decimal`, `bool`, `int` ou `void` | Promete o tipo de resposta que a função devolve. **`void`** significa *"apenas faz uma ação e não devolve nenhum valor"*. |
| **Nome do método** | `CalcularSubtotal`, `ExibirComprovante` | Em C#, usa **PascalCase** (primeira letra de cada palavra maiúscula) e começa com verbo. |
| **Parâmetros** | `(decimal preco, int quantidade)` | Entradas que a função exige para trabalhar (variáveis locais que nascem na chamada). |
| **Argumentos** | `(5.50m, 3)` | Os valores concretos passados no momento da chamada. |
| **`return`** | `return preco * quantidade;` | Encerra a função imediatamente e devolve o resultado para quem chamou. |

> [!TIP]
> **Princípio de ouro da engenharia de software (usado no MarketFlow inteiro):**
> Métodos de **cálculo ou validação** retornam dados (`decimal`, `bool`, `int`) e **NUNCA** chamam `Console.WriteLine` nem `MessageBox.Show` lá dentro! Assim a mesma regra funciona no Console, no Windows Forms e nos testes automatizados.

---

## 2. Exemplo Prático Completo #1 — Funções Puras de Cálculo vs Função `void`

Digite e execute este programa completo no `Program.cs` do projeto `TreinoSupermercado` (no C# moderno com instruções de nível superior, **as chamadas ficam na parte de cima** e **as declarações dos métodos locais ficam na parte de baixo**):

```csharp
using System.Globalization;

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");

// === CHAMADAS DOS MÉTODOS (aqui o código realmente executa) ===
decimal subtotal1 = CalcularSubtotal(12.50m, 2); // Esperado: 25,00
decimal subtotal2 = CalcularSubtotal(5.50m, 3);  // Esperado: 16,50
decimal media = CalcularMedia(6m, 8m);           // Esperado: 7

Console.WriteLine($"Subtotal 1: {subtotal1:C}");
Console.WriteLine($"Subtotal 2: {subtotal2:C}");
Console.WriteLine($"Média (6 e 8): {media}");

Console.WriteLine("\n=== TESTANDO FRONTEIRAS DE DESCONTO ===");
decimal[] compras = { 199.99m, 200.00m, 499.99m, 500.00m };
foreach (decimal valor in compras)
{
    decimal desconto = CalcularDesconto(valor);
    decimal liquido = valor - desconto;
    Console.WriteLine($"Compra {valor:C} | Desconto: {desconto:C} | A pagar: {liquido:C}");
}

// Chamando uma função void (não pode ser guardada em variável!):
Console.WriteLine();
ExibirComprovante("Arroz 5kg", 3, 5.50m, subtotal2, CalcularDesconto(subtotal2));

// === DECLARAÇÕES DOS MÉTODOS LOCAIS ===

// Função pura: recebe dados e retorna decimal (ZERO Console.WriteLine aqui dentro!)
decimal CalcularSubtotal(decimal precoUnitario, int quantidade)
{
    return precoUnitario * quantidade;
}

// Parênteses garantem que a soma aconteça antes da divisão; '2m' garante divisão decimal exata:
decimal CalcularMedia(decimal primeiraNota, decimal segundaNota)
{
    return (primeiraNota + segundaNota) / 2m;
}

// Regra de negócio isolada: >= 500 ganha 50; >= 200 ganha 15; abaixo disso, 0.
decimal CalcularDesconto(decimal valorCompra)
{
    if (valorCompra >= 500.00m)
        return 50.00m;

    if (valorCompra >= 200.00m)
        return 15.00m;

    return 0m;
}

// Função void: responsabilidade exclusiva de exibir na tela!
void ExibirComprovante(string produto, int qtd, decimal preco, decimal subtotal, decimal desconto)
{
    Console.WriteLine("==========================================");
    Console.WriteLine(" COMPROVANTE DIDÁTICO — SEM VALOR FISCAL");
    Console.WriteLine("==========================================");
    Console.WriteLine($"Produto  : {produto}");
    Console.WriteLine($"Itens    : {qtd} x {preco:C}");
    Console.WriteLine($"Subtotal : {subtotal:C}");
    Console.WriteLine($"Desconto : {desconto:C}");
    Console.WriteLine($"TOTAL    : {(subtotal - desconto):C}");
    Console.WriteLine("==========================================");
}
```

### Saída Esperada no Console (`Ctrl+F5`)

```text
Subtotal 1: R$ 25,00
Subtotal 2: R$ 16,50
Média (6 e 8): 7

=== TESTANDO FRONTEIRAS DE DESCONTO ===
Compra R$ 199,99 | Desconto: R$ 0,00 | A pagar: R$ 199,99
Compra R$ 200,00 | Desconto: R$ 15,00 | A pagar: R$ 185,00
Compra R$ 499,99 | Desconto: R$ 15,00 | A pagar: R$ 484,99
Compra R$ 500,00 | Desconto: R$ 50,00 | A pagar: R$ 450,00

==========================================
 COMPROVANTE DIDÁTICO — SEM VALOR FISCAL
==========================================
Produto  : Arroz 5kg
Itens    : 3 x R$ 5,50
Subtotal : R$ 16,50
Desconto : R$ 0,00
TOTAL    : R$ 16,50
==========================================
```

---

## 3. Exemplo Prático Completo #2 — Função Booleana e Reutilização de Leitura Segura

Observe como extrair o laço `do / while` da Aula 02 para **`LerInteiroPositivo(string mensagem)`** elimina completamente código duplicado: escrevemos o laço **1 vez** e o reutilizamos para ler quantidade, número de vias ou qualquer outro número positivo!

```csharp
// Testando a função booleana de validação (sem imprimir nada dentro dela!):
int[] testesQuantidade = { -1, 0, 2 };
foreach (int q in testesQuantidade)
{
    if (EhQuantidadeValida(q))
        Console.WriteLine($"Quantidade {q}: VÁLIDA.");
    else
        Console.WriteLine($"Quantidade {q}: RECUSADA (precisa ser maior que zero).");
}

// Reutilizando o MESMO método de leitura segura duas vezes com mensagens diferentes!
Console.WriteLine();
int quantidadeItens = LerInteiroPositivo("Digite a quantidade de itens: ");
int numeroVias = LerInteiroPositivo("Digite quantas vias deseja imprimir: ");

Console.WriteLine($"\nConfirmado: {quantidadeItens} item(ns) e {numeroVias} via(s).");

// --- MÉTODOS ---

bool EhQuantidadeValida(int quantidade)
{
    return quantidade > 0;
}

int LerInteiroPositivo(string mensagem)
{
    int valor;
    string? entrada;

    do
    {
        Console.Write(mensagem);
        entrada = Console.ReadLine();
        if (entrada is null) return 1;
    } while (!int.TryParse(entrada, out valor) || !EhQuantidadeValida(valor));

    return valor;
}
```

### Exemplo de Execução no Console

```text
Quantidade -1: RECUSADA (precisa ser maior que zero).
Quantidade 0: RECUSADA (precisa ser maior que zero).
Quantidade 2: VÁLIDA.

Digite a quantidade de itens: abc
Digite a quantidade de itens: 0
Digite a quantidade de itens: 4
Digite quantas vias deseja imprimir: -2
Digite quantas vias deseja imprimir: 2

Confirmado: 4 item(ns) e 2 via(s).
```

*(Repare na elegância: `LerInteiroPositivo` chamou `EhQuantidadeValida(valor)` — um método reutilizando outro método!)*

---

## 4. Exemplo Prático Completo #3 — Escopo de Variáveis e Passagem por Cópia de Valor

Dois conceitos essenciais que você precisa provar executando no VS 2022:

1. **Escopo (`{ }`):** toda variável criada dentro de um método (incluindo seus parâmetros) **só existe dentro das chaves `{ }` daquele método**. Quando o método termina, ela é destruída da memória.
2. **Passagem por Cópia de Valor (`int`, `decimal`, `bool`, `char`):** quando você passa `numero` para `Dobrar(int valor)`, o C# faz uma **cópia independente** do número `3`. Alterar `valor` lá dentro **não altera** `numero` fora!

```csharp
int numero = 3;
int resultado = Dobrar(numero);

Console.WriteLine($"Fora do método -> numero original = {numero}");     // Continua 3!
Console.WriteLine($"Fora do método -> resultado       = {resultado}");  // Vale 6!

// ❌ TESTE DE ESCOPO (descomente para observar o erro CS0103 e depois apague):
// Console.WriteLine(valorInterno); // Erro CS0103: 'valorInterno' só existe dentro de Dobrar!

int Dobrar(int valor)
{
    int valorInterno = valor * 2;
    valor = valorInterno; // Altera APENAS a cópia local chamada 'valor'!
    Console.WriteLine($"Dentro de Dobrar -> cópia 'valor' = {valor}");
    return valor;
}
```

### Saída Esperada no Console

```text
Dentro de Dobrar -> cópia 'valor' = 6
Fora do método -> numero original = 3
Fora do método -> resultado       = 6
```

> [!NOTE]
> **Gancho para a Aula 04:** hoje você viu que tipos primitivos (`int`, `decimal`, `bool`) são copiados por **valor**. Na [Aula 04](04-Classes-Objetos-e-Referencias.md), você descobrirá que **objetos de Classes** funcionam por **referência** — alterar uma propriedade dentro de um método altera o objeto original!

---

## 5. Depurando Chamadas de Método no VS 2022 (`F11` vs `F10`)

Quando o Depurador pausa (`F9` + `F5`) em uma linha que **chama um método seu**, como `decimal subtotal = CalcularSubtotal(5.50m, 3);`:

- **`F10` (*Step Over* / Contornar):** executa `CalcularSubtotal` inteiro de uma vez e pula direto para a linha seguinte.
- **`F11` (*Step Into* / Intervir):** **salta para dentro** da primeira linha de `CalcularSubtotal`!
  - Olhe na janela **Locais** (`Ctrl+Alt+V, L`): você verá `precoUnitario = 5,50` e `quantidade = 3` nascendo dentro do escopo do método.
  - Pressione `F10` até passar pelo `return` e veja a seta amarela voltar ao ponto exato da chamada entregando `16,50` para `subtotal`!

---

## 6. Roteiro de Prática e Entregas no Visual Studio 2022 (`A03.01`–`A03.05`)

No projeto **`TreinoSupermercado`**, construa e demonstre:

1. **`A03.01` e `A03.02`:** construa `CalcularSubtotal(decimal, int)`, `CalcularMedia(decimal, decimal)` (com `2m`) e `CalcularDesconto(decimal)` sem nenhum `Console.WriteLine` interno, testando `(12.50m, 2)`, `(5.50m, 3)`, `(6m, 8m)` e as fronteiras `199,99`, `200,00`, `499,99` e `500,00`.
2. **`A03.03`:** construa `EhQuantidadeValida(int)` retornando `bool` e teste `-1`, `0` e `2` com `if / else` externo.
3. **`A03.04`:** extraia `LerInteiroPositivo(string)` reutilizando-a duas vezes e crie `ExibirComprovante` como `void`.
4. **`A03.05`:** demonstre `Dobrar(int)` comprovando que `numero` permanece `3` enquanto o retorno é `6`, provoque e registre o erro de escopo **`CS0103`** e entre em `CalcularSubtotal` usando **`F11`** no Depurador.

---

## 7. Erros Comuns de Iniciantes (❌ Errado vs ✅ Certo)

| Código do Erro | ❌ Código que quebra | ✅ Como corrigir |
| --- | --- | --- |
| **`CS0161`** (nem todos os caminhos retornam) | `decimal Desc(decimal v) { if (v >= 200m) return 15m; }` | Garanta `return` também quando o `if` for falso (`return 0m;`). |
| **`CS0029`** (atribuir `void` a variável) | `string s = ExibirComprovante(...);` | Funções `void` são chamadas sozinhas: `ExibirComprovante(...);`. |
| **`CS7036`** (faltou argumento) | `CalcularSubtotal(5.50m);` | Passe todos os parâmetros na ordem certa: `CalcularSubtotal(5.50m, 3);`. |
| **`CS0103`** (fora de escopo) | Ler `valorInterno` fora do método onde foi criado. | Devolva o resultado com `return` para quem chamou. |
| **Divisão inteira truncada** | `decimal media = (6 + 7) / 2;` *(dá `6`, não `6,5`!)* | Use `2m`: `decimal media = (6m + 7m) / 2m;` *(dá `6,5`!)*. |

[Anterior: Aula 02](02-Entrada-Decisoes-Lacos-e-Depuracao.md) · [Próxima: Aula 04 — Classes, objetos e referências](04-Classes-Objetos-e-Referencias.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 03 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-03). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
