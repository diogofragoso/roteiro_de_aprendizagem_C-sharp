# 2️⃣ Prática: Criando Aplicações de Console (Treinando Sintaxe)

Antes de pularmos para interfaces gráficas complexas (telas com botões) ou conectarmos um banco de dados, precisamos garantir que você dominou a **sintaxe** do C# que aprendemos no Módulo 1.

A melhor forma de aprender lógica de programação é criando **Aplicações de Console** (Console Applications) — aqueles famosos programas de "tela preta". Eles removem qualquer distração visual para que você foque 100% no código.

---

## 1. Criando seu Primeiro Projeto de Console

Se você ainda não instalou o **Visual Studio 2022**, instale a versão Community gratuita no site da Microsoft (marcando a opção "Desenvolvimento para Desktop com .NET").

1. Abra o Visual Studio 2022.
2. Clique em **Criar um novo projeto**.
3. Na barra de pesquisa, digite `Console`.
4. Escolha **Aplicativo de Console (Console App)** com a tag `C#`.
5. Dê um nome, como `TreinoSupermercado`.
6. Escolha a versão mais recente do .NET (ex: .NET 8.0) e clique em **Criar**.

O Visual Studio abrirá um arquivo chamado `Program.cs`. O código padrão se parece com isso:

```csharp
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
```

Se você apertar o botão **Iniciar** (ou F5) no topo, uma tela preta vai piscar mostrando "Hello, World!". 

---

## 2. Brincando com Variáveis (Entrada e Saída)

Vamos transformar esse "Hello World" em um sistema primitivo de caixa de supermercado.
Apague tudo no `Program.cs` e escreva o seguinte código:

```csharp
Console.WriteLine("=== BEM-VINDO AO MARKETFLOW ===");

// 1. O sistema pergunta algo (Saída de dados)
Console.Write("Digite o nome do produto: "); 

// 2. O sistema lê o que o usuário digitou (Entrada de dados)
string nomeProduto = Console.ReadLine();

Console.Write("Digite a quantidade: ");
// Console.ReadLine() sempre retorna um TEXTO (string). 
// Precisamos converter para Inteiro para fazer contas!
string quantidadeTexto = Console.ReadLine();
int quantidade = Convert.ToInt32(quantidadeTexto);

Console.Write("Digite o preço unitário (ex: 5,50): ");
decimal preco = Convert.ToDecimal(Console.ReadLine());

// 3. O sistema processa os dados
decimal total = quantidade * preco;

// 4. O sistema exibe o resultado final
Console.WriteLine("\n--- CUPOM FISCAL ---");
Console.WriteLine($"Produto: {nomeProduto}");
Console.WriteLine($"Qtd: {quantidade} x {preco:C}"); // :C formata como Moeda (Currency)
Console.WriteLine($"Total a Pagar: {total:C}");
```

> [!TIP]
> O símbolo `$` antes da string (como em `$"Produto: {nomeProduto}"`) se chama **Interpolação de String**. É a forma mais moderna e limpa de juntar textos e variáveis em C#, colocando a variável diretamente entre chaves `{}`.

---

## 3. Praticando Estruturas de Decisão

Vamos adicionar uma regra de negócio ao nosso caixa. Continue o código acima, logo abaixo do "Total a Pagar":

```csharp
Console.WriteLine("--------------------");

if (total > 100.00m)
{
    Console.WriteLine("🎉 Parabéns! O cliente ganhou um cupom de 10% para a próxima compra.");
}
else if (total > 50.00m)
{
    Console.WriteLine("👍 Cliente ganhou um brinde no balcão.");
}
else
{
    Console.WriteLine("Agradecemos a preferência.");
}
```
Aperte **F5**. Faça um teste digitando valores que passem de R$ 100,00 e veja como o sistema toma caminhos diferentes na execução!

---

## 📝 Atividades de Fixação (Console App)

Agora é a sua vez de codificar. Apague o código anterior e tente resolver este desafio no Visual Studio:

- [ ] **Desafio do Estoque:** Crie um programa que faça o seguinte:
  1. Pergunte ao usuário a "Quantidade atual de maçãs no estoque".
  2. Leia a resposta e converta para inteiro.
  3. Use um laço `while` (enquanto) que diminua o estoque em 1 até chegar a zero. Em cada repetição, imprima: `"Vendendo uma maçã. Estoque restante: X"`.
  4. Quando o `while` terminar (estoque = 0), imprima: `"ALERTA: Estoque vazio! Solicitar reposição."`.

<details>
<summary><b>💡 Ver Resposta (Tentou fazer sozinho primeiro?)</b></summary>
<br>
<pre><code>
Console.Write("Quantidade atual de maçãs no estoque: ");
int estoque = Convert.ToInt32(Console.ReadLine());

while (estoque > 0)
{
    estoque--; // O mesmo que: estoque = estoque - 1;
    Console.WriteLine($"Vendendo uma maçã. Estoque restante: {estoque}");
}

Console.WriteLine("ALERTA: Estoque vazio! Solicitar reposição.");
</code></pre>
</details>

Agora que você dominou a sintaxe e a lógica escrevendo códigos estruturados no Console, você está preparado para elevar o nível com a Orientação a Objetos.

➡️ **[Ir para o Módulo 3: Orientação a Objetos](./03-Orientacao-a-Objetos.md)**
