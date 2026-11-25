# Aula 07 — Namespaces, Interfaces, Injeção pelo Construtor, Herança e Polimorfismo

**Duração:** 3h30. **Pré-requisito:** [Aula 06](06-Composicao-Colecoes-e-Subtotal.md). **Entrega:** projeto organizado em `namespace` demonstrando contratos com `interface`, injeção de dependência pelo construtor (`private readonly` + `this`), polimorfismo via interface (`List<IPagamento>`) e herança com `virtual`, `override` e `: base(...)` (`A07.01`–`A07.05`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Revisar composição da Aula 06 e o problema de trocar comportamentos sem editar o chamador |
| Explicação | 30 | `namespace`/`using`, `interface`, injeção, polimorfismo, herança (`virtual`/`override`/`base`) |
| Construção | 60 | Executar os Exemplos Completos #1 e #2 (`IFormatador`/`Apresentador` e `IPagamento`) |
| Intervalo | 15 | Pausa |
| Prática | 60 | Executar o Exemplo Completo #3 (Herança `Pessoa`/`Aviso`/`AvisoUrgente`) e depurar com `F11` |
| Entrega | 30 | Demonstrar erro `CS0535`, polimorfismo, justificar interface vs composição vs herança |

---

## 1. Organizando Seu Código com `namespace` e `using` (`A07.01`)

Até a Aula 06, criamos poucas classes. Em projetos reais (como o **MarketFlow**, que terá `Models`, `Services`, `Repositories`, `Infrastructure` e `Views`), dois arquivos podem até ter o mesmo nome em áreas diferentes.

- **`namespace Aula07Polimorfismo;`** dá um "sobrenome" oficial a todas as classes daquele arquivo (sintaxe *file-scoped*, válida para o arquivo inteiro).
  - **Atenção:** criar pastas no Visual Studio 2022 organiza os arquivos no disco, mas o que realmente define o namespace em C# é a linha `namespace ...;` no código!
- **`using Aula07Polimorfismo;`** avisa ao arquivo `Program.cs`: *"quero usar os nomes curtos das classes desse namespace aqui"*.
  - **Mito comum:** `using` **NÃO** baixa nem instala bibliotecas da internet (quem faz isso é o **NuGet**, que veremos na Aula 13!).

---

## 2. Exemplo Prático Completo #1 — `interface`, Injeção pelo Construtor e `readonly` (`A07.01` e `A07.02`)

Uma **`interface`** é um **contrato**: ela define **o que** uma classe precisa saber fazer (nomes, parâmetros e retornos dos métodos), mas **não escreve o código de como fazer**.
- Por convenção em C#, toda interface começa com a letra **`I`** (`IFormatador`, `IPagamento`, `IProdutoRepository`).
- Você **nunca** pode fazer `new IFormatador()` — só podemos instanciar classes concretas que **assinam o contrato** com `: IFormatador`.

Crie um novo **Aplicativo de Console** chamado **`Aula07Polimorfismo`** no Visual Studio 2022 e adicione estes 5 arquivos (mais o `Program.cs`):

### Arquivo 1: `IFormatador.cs` (O Contrato)

```csharp
namespace Aula07Polimorfismo;

public interface IFormatador
{
    string Formatar(string texto); // Termina em ';' — sem corpo { }!
}
```

### Arquivo 2: `FormatadorSimples.cs` (Implementação 1)

```csharp
namespace Aula07Polimorfismo;

public class FormatadorSimples : IFormatador
{
    public string Formatar(string texto) => texto;
}
```

### Arquivo 3: `FormatadorMaiusculo.cs` (Implementação 2)

```csharp
namespace Aula07Polimorfismo;

public class FormatadorMaiusculo : IFormatador
{
    public string Formatar(string texto) => texto.ToUpperInvariant();
}
```

### Arquivo 4: `FormatadorComPrefixo.cs` (Implementação 3)

```csharp
namespace Aula07Polimorfismo;

public class FormatadorComPrefixo : IFormatador
{
    public string Formatar(string texto) => $"Mensagem: {texto}";
}
```

### Arquivo 5: `Apresentador.cs` (Quem RECEBE o Contrato pelo Construtor!)

```csharp
namespace Aula07Polimorfismo;

public class Apresentador
{
    // 'readonly': só pode ser atribuído no construtor (nunca trocado por acidente depois!)
    private readonly IFormatador formatador;

    // INJEÇÃO PELO CONSTRUTOR: recebe QUALQUER classe que assine IFormatador!
    public Apresentador(IFormatador formatador)
    {
        if (formatador is null)
            throw new ArgumentException("Informe um formatador.", nameof(formatador));

        // 'this.formatador' é o campo da classe; 'formatador' é o parâmetro do construtor:
        this.formatador = formatador;
    }

    public string Preparar(string texto)
    {
        // Apresentador NÃO sabe qual classe concreta está aqui dentro!
        return formatador.Formatar(texto);
    }
}
```

### Arquivo 6: `Program.cs`

```csharp
using Aula07Polimorfismo;

var simples = new Apresentador(new FormatadorSimples());
var gritando = new Apresentador(new FormatadorMaiusculo());
var rotulado = new Apresentador(new FormatadorComPrefixo());

Console.WriteLine(simples.Preparar("Bom dia"));
Console.WriteLine(gritando.Preparar("Bom dia"));
Console.WriteLine(rotulado.Preparar("Bom dia"));
```

### Saída Esperada no Console (`Ctrl+F5`)

```text
Bom dia
BOM DIA
Mensagem: Bom dia
```

> [!TIP]
> **Por que isso é revolucionário?** Mudamos completamente o comportamento de `Apresentador` **três vezes** sem tocar em **uma única linha** de `Apresentador.cs`! É exatamente assim que, no MarketFlow, `ProdutoService` funcionará tanto com o banco PostgreSQL quanto com um repositório Fake em memória nos testes!

---

## 3. Exemplo Prático Completo #2 — Polimorfismo via `List<IPagamento>` (`A07.03`)

**Polimorfismo** significa *"muitas formas"*: um mesmo código chama `pagamento.Processar(60.30m)`, e cada objeto reage do seu próprio jeito!

### Arquivo 1: `IPagamento.cs`

```csharp
namespace Aula07Polimorfismo;

public interface IPagamento
{
    void Processar(decimal valor);
}
```

### Arquivo 2: `PagamentoDinheiro.cs`

```csharp
namespace Aula07Polimorfismo;

public class PagamentoDinheiro : IPagamento
{
    public void Processar(decimal valor)
    {
        Console.WriteLine($"[Dinheiro simulado] Recebimento de {valor:C} em espécie (conferir troco).");
    }
}
```

### Arquivo 3: `PagamentoCartao.cs`

```csharp
namespace Aula07Polimorfismo;

public class PagamentoCartao : IPagamento
{
    public void Processar(decimal valor)
    {
        Console.WriteLine($"[Cartão simulado]   Transação fictícia aprovada no valor de {valor:C}.");
    }
}
```

### Uso no `Program.cs`

```csharp
using System.Globalization;
using Aula07Polimorfismo;

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");

// Uma única lista do tipo da INTERFACE guardando objetos de classes DIFERENTES:
List<IPagamento> pagamentos = new List<IPagamento>
{
    new PagamentoDinheiro(),
    new PagamentoCartao()
};

Console.WriteLine("=== POLIMORFISMO POR INTERFACE ===");
foreach (IPagamento pagamento in pagamentos)
{
    // A MESMA linha executa comportamentos completamente diferentes em cada volta!
    pagamento.Processar(60.30m);
}
```

### Saída Esperada no Console

```text
=== POLIMORFISMO POR INTERFACE ===
[Dinheiro simulado] Recebimento de R$ 60,30 em espécie (conferir troco).
[Cartão simulado]   Transação fictícia aprovada no valor de R$ 60,30.
```

---

## 4. Exemplo Prático Completo #3 — Herança (`:`), `virtual`, `override` e `: base(...)` (`A07.04` e `A07.05`)

**Herança** permite criar uma classe derivada (filha) que **herda** propriedades e métodos de uma classe base (pai) usando dois-pontos (`:`), acrescentando ou especializando comportamentos.

### Parte A — Reaproveitando Estrutura Comum (`Pessoa`, `Cliente` e `Funcionario`)

#### `Pessoa.cs`, `Cliente.cs` e `Funcionario.cs` (em arquivos separados!)

```csharp
// Pessoa.cs (classe base / pai)
namespace Aula07Polimorfismo;

public class Pessoa
{
    public string Nome { get; set; } = "";
    public string Cpf { get; set; } = "";

    public void ExibirCadastro()
    {
        Console.WriteLine($"Cadastro -> Nome: {Nome} | CPF: {Cpf}");
    }
}
```

```csharp
// Cliente.cs (herda Nome, Cpf e ExibirCadastro de Pessoa, e adiciona PontosFidelidade!)
namespace Aula07Polimorfismo;

public class Cliente : Pessoa
{
    public int PontosFidelidade { get; set; }
}
```

```csharp
// Funcionario.cs (herda de Pessoa e adiciona SalarioBase!)
namespace Aula07Polimorfismo;

public class Funcionario : Pessoa
{
    public decimal SalarioBase { get; set; }
}
```

### Parte B — Especializando Comportamento com `virtual`, `override` e `: base(...)`

Quando a classe filha quer **personalizar** um método da classe pai (em vez de usar exatamente o mesmo), usamos:
1. **`virtual`** na classe base: autoriza subclasses a sobrescreverem aquele método.
2. **`override`** na classe filha: fornece a nova implementação.
3. **`: base(titulo)`** no construtor da filha: repassa argumentos obrigatórios ao construtor da classe base.
4. **`base.Exibir()`**: reaproveita o resultado do método original da base dentro da nova versão!

#### `Aviso.cs` (Classe Base)

```csharp
namespace Aula07Polimorfismo;

public class Aviso
{
    public string Titulo { get; }

    public Aviso(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("Informe o título do aviso.");
        Titulo = titulo.Trim();
    }

    // 'virtual' autoriza classes derivadas a sobrescreverem Exibir():
    public virtual string Exibir()
    {
        return $"Aviso | {Titulo}";
    }
}
```

#### `AvisoUrgente.cs` (Classe Derivada)

```csharp
namespace Aula07Polimorfismo;

public class AvisoUrgente : Aviso
{
    // ': base(titulo)' chama o construtor da classe Aviso (reaproveitando a validação!):
    public AvisoUrgente(string titulo) : base(titulo)
    {
    }

    // 'override' substitui o comportamento de Exibir(), reutilizando base.Exibir():
    public override string Exibir()
    {
        return $"URGENTE | {base.Exibir()}";
    }
}
```

#### Testando Tudo no `Program.cs`

```csharp
using System.Globalization;
using Aula07Polimorfismo;

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");

Console.WriteLine("=== HERANÇA SIMPLES (Pessoa) ===");
var cliente = new Cliente { Nome = "Ana Paula", Cpf = "000.111.222-33", PontosFidelidade = 120 };
var func = new Funcionario { Nome = "Carlos Silva", Cpf = "444.555.666-77", SalarioBase = 2500.00m };

cliente.ExibirCadastro(); // Método herdado de Pessoa!
Console.WriteLine($"  Pontos: {cliente.PontosFidelidade}");

func.ExibirCadastro();    // Mesmo método herdado de Pessoa!
Console.WriteLine($"  Salário: {func.SalarioBase:C}");

Console.WriteLine("\n=== POLIMORFISMO POR HERANÇA (virtual / override) ===");
// A variável é do tipo base 'Aviso', mas aponta para objetos de tipos diferentes!
Aviso aviso1 = new Aviso("Promoção de hortifrúti até 18h");
Aviso aviso2 = new AvisoUrgente("Fechamento do caixa em 10 minutos");

Console.WriteLine(aviso1.Exibir()); // Chama Aviso.Exibir()
Console.WriteLine(aviso2.Exibir()); // Chama AvisoUrgente.Exibir()!
```

### Saída Esperada no Console (`Ctrl+F5`)

```text
=== HERANÇA SIMPLES (Pessoa) ===
Cadastro -> Nome: Ana Paula | CPF: 000.111.222-33
  Pontos: 120
Cadastro -> Nome: Carlos Silva | CPF: 444.555.666-77
  Salário: R$ 2.500,00

=== POLIMORFISMO POR HERANÇA (virtual / override) ===
Aviso | Promoção de hortifrúti até 18h
URGENTE | Aviso | Fechamento do caixa em 10 minutos
```

---

## 5. Decisão de Engenharia: Quando Usar Interface, Composição ou Herança?

Muitos iniciantes forçam herança em tudo (ex.: criar subclasses `Arroz : Produto` ou `Feijao : Produto` — **errado**, porque Arroz e Feijão são apenas instâncias com `Nome` diferentes da mesma classe `Produto`!). Use esta tabela decisória:

| Recurso | Pergunta para escolher | Exemplo certo no curso |
| --- | --- | --- |
| **Interface (`interface`)** | *"Preciso definir um contrato de ações trocáveis sem acoplar à implementação?"* | `IFormatador`, `IPagamento`, `IProdutoRepository` |
| **Composição ("tem um")** | *"Este objeto colabora com / possui outro objeto?"* | `ItemDeCompra` tem um `Produto`; `Apresentador` recebe `IFormatador` |
| **Herança (`:`)** | *"A classe derivada **é um** tipo especializado da base?"* | `Cliente : Pessoa`; `AvisoUrgente : Aviso`; **`FormLogin : Form`** (Windows Forms!) |

*(Lembre-se: membros `private` da classe base continuam inacessíveis na subclasse — herança nunca quebra o encapsulamento!)*

---

## 6. Roteiro de Prática e Entregas no Visual Studio 2022 (`A07.01`–`A07.05`)

No projeto **`Aula07Polimorfismo`**, com `namespace Aula07Polimorfismo;` em todos os arquivos:

1. **`A07.01` e `A07.02`:** construa `IFormatador`, as 3 implementações e `Apresentador`, execute comprovando `Bom dia`, `BOM DIA` e `Mensagem: Bom dia`, **entre em `Preparar` com `F11`** no Depurador observando o salto polimórfico e comente temporariamente `Formatar` em uma classe para registrar o erro **`CS0535`** do compilador.
2. **`A07.03`:** construa `IPagamento`, `PagamentoDinheiro` e `PagamentoCartao` e percorra `List<IPagamento>` em um único `foreach`.
3. **`A07.04` e `A07.05`:** construa `Pessoa`/`Cliente`/`Funcionario` e `Aviso`/`AvisoUrgente` (com `: base(titulo)`, `virtual`, `override` e `base.Exibir()`), teste trocar `new AvisoUrgente` por `new Aviso` e justifique no caderno por que não usamos herança para cada produto do supermercado.

---

## 7. Erros Comuns de Iniciantes (❌ Errado vs ✅ Certo)

| Código do Erro | ❌ Código que quebra | ✅ Como corrigir |
| --- | --- | --- |
| **`CS0535`** (contrato incompleto) | `class X : IFormatador { }` *(esqueceu o método `Formatar`!)* | Implemente todos os métodos exigidos pela interface com assinatura idêntica e `public`. |
| **`CS0144`** (`new` em interface) | `var f = new IFormatador();` | Instancie uma classe concreta: `IFormatador f = new FormatadorSimples();`. |
| **`CS0506`** (`override` sem `virtual`) | Escrever `override` na filha sem `virtual` no método da classe pai. | Marque o método da classe base como `public virtual`. |
| **`CS7036`** (faltou `: base(...)`) | Herdar de `Aviso` (que exige `titulo`) sem chamar `: base(titulo)`. | Declare `public AvisoUrgente(string titulo) : base(titulo)`. |
| **`CS0246`** (faltou `using`) | Usar `Apresentador` no `Program.cs` sem `using Aula07Polimorfismo;`. | Adicione `using Aula07Polimorfismo;` na primeira linha de `Program.cs`. |

[Anterior: Aula 06](06-Composicao-Colecoes-e-Subtotal.md) · [Próxima: Aula 08 — Arquitetura em camadas e contratos](08-Arquitetura-e-Padroes.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 07 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-07). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
