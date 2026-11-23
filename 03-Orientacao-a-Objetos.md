# Aula 03 — Orientação a Objetos Aplicada ao Supermercado

**Duração:** 3h30. **Pré-requisito:** [Aula 02](02-Pratica-Console.md) e fundamentos [F03–F07](fundamentos/README.md) demonstrados. **Entrega:** classes separadas em arquivos no Visual Studio 2022 demonstrando objetos independentes, composição com subtotal, encapsulamento com `throw`/`try-catch`, herança e polimorfismo por interface.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Revisar variáveis soltas do console e o motivo de modelar objetos |
| Explicação | 30 | Os 4 pilares, propriedades calculadas (`=>`), `private set` e exceções |
| Construção | 60 | Criar `Produto`, `ItemDeCompra` e `Usuario` em arquivos separados |
| Intervalo | 15 | Pausa |
| Prática | 60 | Construir subtotal, testar encapsulamento e exercitar herança e polimorfismo |
| Entrega | 30 | Demonstrar todos os casos (`A03.01`–`A03.06`) e registrar evidências |

> [!NOTE]
> Esta aula é uma **aplicação prática de POO ao domínio do supermercado**. Se classes, construtores, composição ou interfaces ainda são novidade para você, estude primeiro [F04](fundamentos/04-Classes-e-Objetos.md), [F05](fundamentos/05-Construtores-e-Encapsulamento.md), [F06](fundamentos/06-Composicao-e-Colecoes.md), [F07](fundamentos/07-Interfaces-e-Transicao.md) e o [Laboratório de POO](fundamentos/POO-na-Pratica.md).

---

## 1. Os 4 Pilares da POO — O Que São e Quando Realmente Usar Cada Um

No supermercado, conceitos reais viram classes no código (`Produto`, `ItemDeCompra`, `Usuario`). Os quatro pilares são ferramentas de modelagem — **não** uma obrigação de forçar herança em toda classe:

| Pilar | O que significa? | Por que usamos dessa forma no projeto? |
| --- | --- | --- |
| **1. Abstração** | Filtrar apenas os dados e ações relevantes para o sistema. | Um produto real tem cheiro, cor da embalagem e data de fabricação, mas para o cálculo de caixa abstraímos apenas `Nome` e `Preco`. |
| **2. Encapsulamento** | Proteger o estado interno (`private` / `private set`) e validar mudanças em métodos. | Impede que uma tela ou método salve `Username = ""` ou um preço negativo diretamente na propriedade. |
| **3. Herança (`:`)** | Especializar uma classe base reaproveitando membros comuns (`Cliente : Pessoa`). | Útil quando existe relação real **"é um"**. **Cuidado:** não devemos criar uma subclasse para cada tipo de produto do supermercado (`Arroz`, `Carne`, `Sabao`) — isso tornaria o sistema impossível de manter quando chegasse um produto novo! |
| **4. Polimorfismo** | Chamar uma operação por um contrato comum (`interface`) e executar comportamentos distintos conforme o objeto real. | Permite que o caixa chame `pagamento.Processar(total)` sem precisar de um `if` gigante perguntando se é dinheiro ou cartão. |

---

## 2. Detalhes de Sintaxe C# Que Você Vai Usar Nesta Aula

Antes de construir suas classes, entenda cada detalhe sintático e o seu motivo:

### Propriedade vs Campo e Inicialização de Texto (`= ""`)
- Um **campo** guarda o dado diretamente (`private int contador;`).
- Uma **propriedade** expõe acessores `get` (leitura) e `set` (escrita).
- Em `public string Nome { get; set; } = "";`, o `= ""` define o texto inicial como vazio. **Por quê?** Com `<Nullable>enable</Nullable>` ativado nos projetos modernos do Visual Studio 2022, propriedades `string` sem inicialização nem construtor geram aviso de nulabilidade (`CS8618`).

### Atalho `=>` para Propriedade Calculada (Expression-Bodied Property)
Em `F06`, você escreveu uma propriedade calculada assim:
```csharp
public decimal Subtotal
{
    get { return Produto.Preco * Quantidade; }
}
```
Em C# moderno, quando um `get` tem apenas uma expressão de retorno, podemos escrever exatamente a mesma coisa de forma mais limpa com a seta **`=>`**:
```csharp
public decimal Subtotal => Produto.Preco * Quantidade;
```
- **Por que usar propriedade calculada?** Porque `Subtotal` sempre reflete o `Produto.Preco` e a `Quantidade` atuais; ninguém consegue digitar um subtotal mentiroso manualmente!

### Encapsulamento com `private set` + `throw`
Quando declaramos `public string Username { get; private set; } = "";`:
- Qualquer classe pode **ler** `usuario.Username`.
- Somente métodos dentro de `Usuario` (como `Renomear`) podem **alterar** `Username`.
- Se `Renomear` receber texto vazio/espaços (`string.IsNullOrWhiteSpace`), executa `throw new ArgumentException("...");`. O `throw` aborta o método imediatamente **antes** da linha de atribuição, preservando o nome antigo intacto!

---

## 3. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

Continue no projeto console `TreinoSupermercado` (ou crie um novo Aplicativo de Console `Aula03Poo` no Visual Studio 2022).

> [!IMPORTANT]
> **Organização no Visual Studio 2022:** crie **cada classe em seu próprio arquivo `.cs`** clicando com o botão direito no projeto → **Adicionar → Classe...** (`Shift+Alt+C`). Deixe o `Program.cs` apenas com as instruções de teste e execução!

### Construção 1 — Classe `Produto` e Objetos Independentes (`A03.01` e `A03.02`)
1. No caderno (`A03.02`), liste 4 dados úteis para modelar um produto de supermercado e 2 características irrelevantes que você decidiu descartar (abstração), justificando cada escolha.
2. Crie o arquivo **`Produto.cs`** com a classe `public class Produto` contendo:
   - `public string Nome { get; set; } = "";`
   - `public decimal Preco { get; set; }`
3. No `Program.cs`, instancie dois produtos usando `new Produto()`: **Arroz** (`25.90m`) e **Feijão** (`8.50m`).
4. Exiba os preços iniciais, altere **apenas** o preço do Feijão para `9.00m` e exiba ambos novamente comprovando que Arroz continuou `25,90`.

### Construção 2 — Composição com `ItemDeCompra` e `List<ItemDeCompra>` (`A03.03`)
1. Crie o arquivo **`ItemDeCompra.cs`** com `public class ItemDeCompra` contendo:
   - `public Produto Produto { get; set; } = new Produto();` (composição: o item **tem um** produto);
   - `public int Quantidade { get; set; }`;
   - `public decimal Subtotal => Produto.Preco * Quantidade;` (propriedade calculada somente leitura).
2. No `Program.cs`, crie uma `List<ItemDeCompra>` contendo `2` unidades de Arroz (`25,90`) e `1` unidade de Feijão (`8,50`).
3. Percorra a lista com `foreach`, some os subtotais em uma variável `decimal total = 0m;` e exiba o total (resultado esperado: **`R$ 60,30`**).

### Construção 3 — Encapsulamento e Exceção com `Usuario` (`A03.04`)
1. Crie o arquivo **`Usuario.cs`** com `public class Usuario`:
   - Declare `public string Username { get; private set; } = "";`.
   - Declare `public void Renomear(string novoNome)`:
     - Valide com `if (string.IsNullOrWhiteSpace(novoNome))` lançando `ArgumentException`.
     - Só depois atribua `Username = novoNome.Trim();`.
2. No `Program.cs`:
   - Instancie `Usuario`, chame `Renomear("operador")` e confirme o valor.
   - Dentro de um bloco `try { ... } catch (ArgumentException erro) { ... }`, chame `Renomear("")`, exiba `erro.Message` e depois exiba `usuario.Username` comprovando que continuou `"operador"`.
   - **Teste de compilação:** tente escrever temporariamente `usuario.Username = "invasor";`, observe o erro `CS0272` na **Lista de Erros** do Visual Studio 2022 e remova a linha.

> [!NOTE]
> **Nota de segurança:** senhas nunca serão guardadas em texto puro nem com MD5; o hash seguro com biblioteca pronta será construído na [Aula 09](09-Autenticacao-Segura.md).

### Construção 4 — Exercício Isolado de Herança (`A03.05`)
*(Este exercício serve para praticar herança; não entra no modelo final de produtos)*
1. Crie os arquivos **`Pessoa.cs`**, **`Cliente.cs`** e **`Funcionario.cs`** (ou agrupe as três classes didáticas em `Pessoas.cs`):
   - `Pessoa`: propriedades `Nome` (`= ""`) e `Cpf` (`= ""`) e um método `public void ExibirCadastro()` que imprime ambos.
   - `Cliente : Pessoa`: herda de `Pessoa` e adiciona `public int PontosFidelidade { get; set; }`.
   - `Funcionario : Pessoa`: herda de `Pessoa` e adiciona `public decimal SalarioBase { get; set; }`.
2. No `Program.cs`, instancie um `Cliente` e um `Funcionario`, preencha `Nome`, `Cpf` e a propriedade específica de cada um, e chame o método herdado `ExibirCadastro()` em ambos.

### Construção 5 — Interface e Polimorfismo (`A03.06`)
*(Simulação didática de pagamentos, sem integração real)*
1. Crie o contrato **`IPagamento.cs`** declarando `public interface IPagamento` com o método `void Processar(decimal valor);`.
2. Crie duas implementações que assinam o contrato (`: IPagamento`):
   - `PagamentoDinheiro`: exibe uma mensagem simulando recebimento em dinheiro do `valor`.
   - `PagamentoCartao`: exibe uma mensagem simulando débito em cartão do `valor`.
3. No `Program.cs`, crie uma `List<IPagamento>` (ou array `IPagamento[]`) contendo uma instância de cada classe e percorra com **um único `foreach`** chamando `pagamento.Processar(total)`. Comprove que a mesma chamada produz mensagens diferentes!

---

## 4. Erros Comuns no Visual Studio 2022

| Erro na Lista de Erros | Causa provável | Como corrigir |
| --- | --- | --- |
| `CS0535: 'PagamentoCartao' não implementa membro de interface 'IPagamento.Processar(decimal)'` | O nome, o retorno ou o tipo do parâmetro do método não batem exatamente com a interface (ou faltou `public`). | Métodos que implementam uma interface devem ser `public` e ter assinatura idêntica. |
| `CS0272: O acessador set da propriedade 'Usuario.Username' está inacessível` | Você tentou alterar `Username` diretamente fora da classe `Usuario`. | Use o método `usuario.Renomear(...)`, que aplica a validação. |
| `CS8803: Instruções de nível superior...` | Colocou instruções executáveis depois de uma declaração de `class` no mesmo `Program.cs`. | Mova cada classe para seu próprio arquivo `.cs`. |

[Anterior: Aula 02](02-Pratica-Console.md) · [Próxima: Aula 04 — Arquitetura e responsabilidades](04-Arquitetura-e-Padroes.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 03 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-03). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
