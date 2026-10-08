# 1️⃣ Introdução ao C# e Lógica de Programação

O **C# (C-Sharp)** é uma linguagem de programação moderna, fortemente tipada e orientada a objetos, criada pela Microsoft. Ele faz parte do ecossistema .NET, sendo uma das linguagens mais utilizadas no mundo corporativo para desenvolvimento de aplicações web, desktop (como a que faremos) e até mobile/games.

Nesta etapa, vamos rever os conceitos básicos que dão base para a construção do nosso Sistema de Supermercado.

## Variáveis e Tipos de Dados

Em C#, toda variável precisa ter um tipo definido (isso previne muitos erros no futuro).

```csharp
// Exemplos no contexto do nosso Supermercado:

string nomeProduto = "Arroz 5kg"; // Textos
int quantidadeEstoque = 150;      // Números inteiros
decimal preco = 25.99m;           // Dinheiro/Valores exatos (usamos 'm' no final)
double peso = 5.0;                // Números com ponto flutuante
bool produtoAtivo = true;         // Verdadeiro ou falso
```

> **Dica de Boas Práticas:** Para valores monetários (como preço de produtos), sempre utilize `decimal` no lugar de `double` ou `float`, pois o `decimal` possui maior precisão e evita erros de arredondamento.

## Estruturas de Decisão

O nosso sistema vai precisar tomar decisões. Por exemplo, "Se o usuário errar a senha, mostre erro", ou "Se o estoque for menor que 10, avise o gerente".

### If / Else
```csharp
int estoqueAtual = 5;
int estoqueMinimo = 10;

if (estoqueAtual < estoqueMinimo)
{
    Console.WriteLine("Alerta: Necessário repor o estoque!");
}
else
{
    Console.WriteLine("Estoque regular.");
}
```

### Switch / Case
Ótimo para menus ou verificação de status (ex: status de um pedido).
```csharp
int statusPedido = 2; // 1-Pendente, 2-Pago, 3-Cancelado

switch (statusPedido)
{
    case 1:
        Console.WriteLine("Aguardando pagamento.");
        break;
    case 2:
        Console.WriteLine("Pagamento aprovado. Liberar mercadoria.");
        break;
    default:
        Console.WriteLine("Status desconhecido.");
        break;
}
```

## Estruturas de Repetição

Precisaremos percorrer listas (como a lista de produtos no carrinho do cliente).

### For e Foreach
```csharp
// Usando o FOR para repetir um número de vezes
for (int i = 0; i < 5; i++)
{
    Console.WriteLine($"Imprimindo cupom via {i+1}");
}

// Usando o FOREACH para percorrer coleções (Muito útil em Listas!)
string[] carrinhoDeCompras = { "Leite", "Pão", "Manteiga" };

foreach (string item in carrinhoDeCompras)
{
    Console.WriteLine($"Produto no carrinho: {item}");
}
```

## Resumo e Próximos Passos
Neste módulo, você aprendeu como armazenar dados e controlar o fluxo do código usando as bases do C#. No mundo real, um sistema de supermercado é composto por centenas de arquivos e regras complexas. Para organizar tudo isso, usamos o conceito do próximo módulo.

➡️ **[Ir para o Módulo 2: Orientação a Objetos](./02-Orientacao-a-Objetos.md)**
