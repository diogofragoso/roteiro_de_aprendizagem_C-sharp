# 1️⃣ Introdução ao C# e Lógica de Programação

O **C# (C-Sharp)** é uma linguagem de programação moderna, fortemente tipada e orientada a objetos, desenvolvida e mantida pela Microsoft. Ele faz parte do ecossistema .NET, sendo incrivelmente versátil: com C# você pode criar aplicações web, sistemas desktop (como o nosso supermercado), aplicativos mobile e até mesmo jogos (utilizando a Unity).

> [!NOTE]
> **O que significa ser "fortemente tipada"?**
> Significa que o C# é rigoroso com o tipo de dado que uma variável pode armazenar. Se você criar uma variável para guardar números inteiros, você não poderá guardar um texto nela. Isso evita muitos "bugs" invisíveis que ocorrem em linguagens mais flexíveis (como JavaScript).

---

## 1. Variáveis e Tipos de Dados

Pense em variáveis como "caixas" na memória do computador, onde cada caixa possui um rótulo (nome) e um formato específico (tipo) que define o que cabe ali dentro.

### Os principais tipos primitivos:
```csharp
// Texto (String e Char)
string nomeProduto = "Arroz Parboilizado 5kg";
char setor = 'A'; // Char usa aspas simples e guarda um único caractere

// Números Inteiros
int quantidadeEstoque = 150;
long codigoDeBarras = 7891020304050L; // Usado para números inteiros muito grandes

// Números Decimais (Com vírgula/ponto flutuante)
double pesoBanca = 5.45; // Mais comum para medidas matemáticas e pesos
decimal preco = 25.99m;  // OBRIGATÓRIO para dinheiro/valores exatos (nota-se o 'm' no final)

// Lógico (Verdadeiro ou Falso)
bool produtoAtivo = true;
bool precisaDeRefrigeracao = false;
```

> [!TIP]
> **Por que usar `decimal` no lugar de `double` no supermercado?**
> O tipo `double` armazena números usando base binária, o que pode causar erros de arredondamento em cálculos financeiros (ex: 0.1 + 0.2 resultando em 0.30000000000000004). O `decimal` trabalha na base 10, garantindo precisão absoluta em centavos.

### Constantes
Se um valor nunca deve mudar durante a execução do programa, declaramos como constante usando `const`:
```csharp
const decimal IMPOSTO_PADRAO = 0.18m; // 18%
```

---

## 2. Conversão de Tipos (Casting)

No mundo real (e nas telas do Windows Forms que criaremos), tudo que o usuário digita no teclado vem como **Texto (string)**. Precisamos converter isso para o tipo correto antes de fazer contas.

```csharp
string entradaDigitada = "15"; // Digamos que veio da tela

// Convertendo String para Inteiro
int quantidade = Convert.ToInt32(entradaDigitada);
// ou
int quantidade2 = int.Parse(entradaDigitada);

// Convertendo Número de volta para String (para mostrar na tela)
decimal troco = 12.50m;
string textoTroco = troco.ToString("C"); // O "C" formata como Moeda (R$ 12,50)
```

---

## 3. Estruturas de Decisão

O nosso sistema vai precisar tomar decisões com base nas ações do cliente.

### If / Else If / Else
A estrutura mais clássica para verificar condições múltiplas.
```csharp
decimal valorCompra = 250.00m;
decimal desconto = 0m;

if (valorCompra >= 500.00m)
{
    desconto = 50.00m;
    Console.WriteLine("Cliente Ouro! Ganhou R$ 50 de desconto.");
}
else if (valorCompra >= 200.00m)
{
    desconto = 15.00m;
    Console.WriteLine("Cliente Prata! Ganhou R$ 15 de desconto.");
}
else
{
    Console.WriteLine("Sem descontos aplicáveis nesta compra.");
}
```

### Switch / Case
Muito mais limpo do que vários "Ifs" quando você está avaliando o valor exato de uma única variável. Ideal para menus ou métodos de pagamento.

```csharp
int metodoPagamento = 2; // 1-Dinheiro, 2-Cartão, 3-Pix

switch (metodoPagamento)
{
    case 1:
        Console.WriteLine("Abrindo a gaveta de dinheiro...");
        break;
    case 2:
        Console.WriteLine("Enviando valor para a maquininha de cartão...");
        break;
    case 3:
        Console.WriteLine("Gerando QR Code na tela...");
        break;
    default:
        // O "default" cai quando nenhuma das opções acima foi escolhida
        Console.WriteLine("Método de pagamento inválido.");
        break;
}
```

---

## 4. Estruturas de Repetição (Loops)

Imagine que o cliente tem 30 produtos no carrinho. O sistema não vai escrever 30 linhas de código para ler cada um; ele usa um *Loop*!

### For
Usado quando sabemos **exatamente** quantas vezes queremos repetir o bloco de código.
```csharp
// Imprimir 3 cupons iguais
for (int via = 1; via <= 3; via++)
{
    Console.WriteLine($"Imprimindo via de número {via}");
}
```

### While
Usado quando **não sabemos** quantas vezes vamos repetir, mas sabemos a condição de parada.
```csharp
bool balancaEstavel = false;
int tentativas = 0;

while (!balancaEstavel && tentativas < 5)
{
    Console.WriteLine("Aguardando estabilização do peso...");
    // Código para ler balança real viria aqui
    tentativas++;
}
```

### Foreach (O Rei das Coleções)
O mais utilizado no dia a dia. Ele pega uma lista e diz: "Para cada item desta lista, faça isso:"
```csharp
string[] carrinho = { "Arroz", "Feijão", "Macarrão", "Molho" };

foreach (string produto in carrinho)
{
    Console.WriteLine($"Passando no caixa: {produto}");
}
```

---

## 📝 Atividades de Fixação

Chegou a hora de testar seus conhecimentos! 

> [!IMPORTANT]
> Tente resolver mentalmente ou escrevendo em um bloco de notas antes de abrir a resposta. Isso é crucial para fixar a lógica de programação.

- [ ] **Exercício 1:** Qual tipo de variável você utilizaria para armazenar a resposta da pergunta: *"O cliente quer incluir o CPF na nota fiscal?"*
<details>
<summary><b>💡 Ver Resposta</b></summary>
<br>
O tipo correto é <code>bool</code> (Booleano), pois ele armazena apenas verdadeiro (sim) ou falso (não).
<pre><code>bool incluiCpfNaNota = true;</code></pre>
</details>

- [ ] **Exercício 2:** Escreva um código usando `if/else` que leia uma variável `idade`. Se a idade for menor que 18, exiba "Venda de bebidas alcoólicas proibida". Caso contrário, exiba "Venda liberada".
<details>
<summary><b>💡 Ver Resposta</b></summary>
<br>
<pre><code>int idade = 20;

if (idade < 18)
{
    Console.WriteLine("Venda de bebidas alcoólicas proibida");
}
else
{
    Console.WriteLine("Venda liberada");
}</code></pre>
</details>

- [ ] **Exercício 3:** Encontre o Erro: O código abaixo não compila. Por quê?
  ```csharp
  string precoTexto = "15.99";
  decimal preco = precoTexto;
  ```
<details>
<summary><b>💡 Ver Resposta</b></summary>
<br>
O C# é fortemente tipado. Você não pode jogar um texto (string) diretamente dentro de uma variável numérica (decimal) sem convertê-lo explicitamente. O correto seria usar o <code>Convert.ToDecimal()</code> ou <code>decimal.Parse()</code>:
<pre><code>decimal preco = decimal.Parse(precoTexto);</code></pre>
</details>

➡️ **[Ir para o Módulo 2: Prática com Console](./02-Pratica-Console.md)**
