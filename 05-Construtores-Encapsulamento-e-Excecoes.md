# Aula 05 — Construtores, Campos Privados, Encapsulamento e Exceções

**Duração:** 3h30. **Pré-requisito:** [Aula 04](04-Classes-Objetos-e-Referencias.md). **Entrega:** classes blindadas contra estados inválidos usando construtores obrigatórios, campos privados vs propriedades (`{ get; }` e `private set`), validação com `throw` / `try-catch` e comparação prática com retorno `bool` (`A05.01`–`A05.05`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Demonstrar a falha da Aula 04: `produto.Preco = -50m` compila sem reclamar! |
| Explicação | 30 | Construtores, campos privados vs propriedades, `private set`, `throw`, `try/catch` e `nameof` |
| Construção | 60 | Executar os Exemplos Completos #1 e #2 (`Bateria`, `Usuario` e `Livro` blindados) |
| Intervalo | 15 | Pausa |
| Prática | 60 | Executar o Exemplo #3 (`throw` vs `bool`) e construir o desafio autônomo `SessaoDeEstudo` |
| Entrega | 30 | Demonstrar preservação de estado após recusa, erro `CS0272` e registrar evidências |

---

## 1. O Problema Real da Aula 04: Por Que `{ get; set; }` Público É Perigoso?

Na [Aula 04](04-Classes-Objetos-e-Referencias.md), qualquer código podia fazer isto com a nossa classe `Produto`:

```csharp
// ❌ Código que COMPILAVA na Aula 04, mas deixa o sistema corrompido:
Produto p = new Produto(); // Nasceu sem nome e com preço R$ 0,00!
p.Nome = "   ";            // Nome só com espaços!
p.Preco = -50.00m;         // Preço NEGATIVO!
p.Estoque = -999;          // Estoque NEGATIVO!
```

Em um sistema comercial real (como o **MarketFlow**), um objeto **nunca pode existir em estado inválido**. Para blindar nossas classes, usamos **dois pilares juntos**:

1. **Construtor obrigatório:** método especial que roda no instante do `new`. Exige os dados essenciais na criação e **recusa criar o objeto** se algum dado for inválido!
2. **Encapsulamento:** fecha a escrita direta (`private set`, `{ get; }` ou campo `private`) para que qualquer mudança posterior passe obrigatoriamente por um método que valida a regra antes de alterar a memória.

---

## 2. Exemplo Prático Completo #1 — Construtor, Campo Privado, `private set`, `nameof` e `throw` (`Bateria.cs`)

Crie um novo **Aplicativo de Console** no Visual Studio 2022 chamado **`Aula05Encapsulamento`**. Adicione **`Bateria.cs`** (`Shift+Alt+C`) e digite ambos os arquivos abaixo:

### Arquivo 1: `Bateria.cs`

```csharp
public class Bateria
{
    // 1. Propriedade com escrita restrita: qualquer um LÊ, só a classe Bateria ALTERA:
    public string Modelo { get; private set; }

    // 2. Propriedade imutável pós-construção: só pode receber valor no Construtor!
    public int CapacidadeMaxima { get; }

    // 3. Campo privado (invisível fora da classe) + Propriedade pública somente leitura:
    private int cargaAtual;
    public int CargaAtual
    {
        get { return cargaAtual; }
    }

    // 4. CONSTRUTOR: mesmo nome da classe, SEM tipo de retorno!
    // Ao criar este construtor, 'new Bateria()' vazio DEIXA DE COMPILAR!
    public Bateria(string modelo, int capacidadeMaxima)
    {
        if (string.IsNullOrWhiteSpace(modelo))
            throw new ArgumentException("O modelo da bateria é obrigatório.", nameof(modelo));

        if (capacidadeMaxima <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacidadeMaxima), "A capacidade deve ser positiva.");

        Modelo = modelo.Trim();
        CapacidadeMaxima = capacidadeMaxima;
        cargaAtual = 0; // Nasce descarregada de forma explícita
    }

    // 5. Método controlado para alterar o modelo com validação:
    public void RenomearModelo(string novoModelo)
    {
        if (string.IsNullOrWhiteSpace(novoModelo))
            throw new ArgumentException("O novo modelo não pode ser vazio.", nameof(novoModelo));

        Modelo = novoModelo.Trim();
    }

    // 6. Método controlado para alterar o campo privado 'cargaAtual':
    public void Carregar(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade para carregar deve ser positiva.", nameof(quantidade));

        if (quantidade > CapacidadeMaxima - cargaAtual)
            throw new ArgumentException($"Excede a capacidade máxima! Espaço restante: {CapacidadeMaxima - cargaAtual} mAh.");

        // Só executa se PASSAR por todas as validações acima!
        cargaAtual += quantidade;
    }
}
```

### Arquivo 2: `Program.cs`

```csharp
// 1. Criando uma bateria válida:
Bateria bat = new Bateria("  PowerBank Pro  ", 100);
Console.WriteLine($"Criada: '{bat.Modelo}' | Carga: {bat.CargaAtual}/{bat.CapacidadeMaxima}");

// 2. Carregamento válido:
bat.Carregar(40);
Console.WriteLine($"Após carregar 40: {bat.CargaAtual}/{bat.CapacidadeMaxima}");

// 3. Tentando carregar acima do limite dentro de try / catch (o programa NÃO trava!):
try
{
    bat.Carregar(80); // 40 + 80 = 120 > 100 -> lança ArgumentException!
    Console.WriteLine("Esta linha NUNCA executa se a linha de cima falhar!");
}
catch (ArgumentException erro)
{
    Console.WriteLine($"Recusado: {erro.Message}");
}

// PROVA DE PRESERVAÇÃO DE ESTADO: cargaAtual continuou exatamente em 40!
Console.WriteLine($"Estado após recusa: {bat.CargaAtual}/{bat.CapacidadeMaxima}");

// 4. Tentando criar objeto inválido no construtor:
try
{
    Bateria invalida = new Bateria("", -10);
}
catch (ArgumentException erro)
{
    Console.WriteLine($"Construtor recusou: {erro.Message}");
}

// ❌ TESTE DO COMPILADOR (descomente para ver o erro CS0272 e depois remova):
// bat.Modelo = "Hack";
// bat.CapacidadeMaxima = 9999; // Erro CS0200 (somente leitura!)
```

### Saída Esperada no Console (`Ctrl+F5`)

```text
Criada: 'PowerBank Pro' | Carga: 0/100
Após carregar 40: 40/100
Recusado: Excede a capacidade máxima! Espaço restante: 60 mAh.
Estado após recusa: 40/100
Construtor recusou: O modelo da bateria é obrigatório. (Parameter 'modelo')
```

### Dissecando Cada Palavra-Chave Nova

| Sintaxe | O que é? | Por que usamos assim? |
| --- | --- | --- |
| `private int cargaAtual;` | **Campo privado** (*field*). | Guarda o dado internamente; código de fora não consegue enxergar nem adulterar. |
| `public int CargaAtual { get { return cargaAtual; } }` | **Propriedade somente leitura.** | Expõe o valor do campo privado para consulta segura sem permitir escrita direta. |
| `{ get; private set; }` | **Propriedade encapsulada.** | Leitura pública; escrita permitida **apenas dentro da própria classe** (`CS0272` se tentar de fora!). |
| `{ get; }` (sem `set`) | **Propriedade imutável.** | Só pode receber valor dentro do construtor; nunca mais muda durante toda a vida do objeto. |
| `string.IsNullOrWhiteSpace(x)` | Método **estático (`static`)**. | Chamado direto no tipo `string.` (sem `new`), detecta `null`, `""` e espaços `"   "`. |
| `nameof(modelo)` | Operador de nome seguro. | Gera a string `"modelo"` em tempo de compilação — se você renomear o parâmetro no VS 2022 (`Ctrl+R, R`), `nameof` acompanha automaticamente! |
| `throw new ArgumentException(...)` | **Lança uma exceção.** | Aborta o método **antes** de alterar qualquer dado, garantindo que o estado anterior fique 100% preservado! |
| `try { ... } catch (ArgumentException erro)` | **Captura a exceção.** | Impede que o programa feche e permite exibir `erro.Message` amigavelmente. **Nunca** deixe um `catch { }` vazio! |

---

## 3. Exemplo Prático Completo #2 — `Usuario.cs` e `Livro.cs` com Testes de Fronteira

Veja agora a implementação completa e executável de `Usuario` e `Livro` testando **todas** as fronteiras (`A05.01`–`A05.04`):

### Arquivo 1: `Usuario.cs`

```csharp
public class Usuario
{
    public string Username { get; private set; } = "";

    public void Renomear(string novoNome)
    {
        if (string.IsNullOrWhiteSpace(novoNome))
            throw new ArgumentException("O nome de usuário é obrigatório.", nameof(novoNome));

        Username = novoNome.Trim();
    }
}
```

### Arquivo 2: `Livro.cs`

```csharp
public class Livro
{
    public string Titulo { get; private set; }
    public int Paginas { get; }
    private int paginasLidas;

    public int PaginasLidas
    {
        get { return paginasLidas; }
    }

    public Livro(string titulo, int paginas)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("O título é obrigatório.", nameof(titulo));
        if (paginas <= 0)
            throw new ArgumentException("A quantidade de páginas deve ser positiva.", nameof(paginas));

        Titulo = titulo.Trim();
        Paginas = paginas;
    }

    public void Renomear(string novoTitulo)
    {
        if (string.IsNullOrWhiteSpace(novoTitulo))
            throw new ArgumentException("O novo título é obrigatório.", nameof(novoTitulo));

        Titulo = novoTitulo.Trim();
    }

    public void Ler(int quantidade)
    {
        if (quantidade <= 0 || quantidade > Paginas - paginasLidas)
            throw new ArgumentException($"Leitura inválida ({quantidade}). Restam {Paginas - paginasLidas} páginas.");

        paginasLidas += quantidade;
    }
}
```

### Arquivo 3: `Program.cs` (Testando Todas as Fronteiras)

```csharp
// === TESTE 1: USUÁRIO ===
Console.WriteLine("=== TESTE DE USUÁRIO ===");
var usuario = new Usuario();
usuario.Renomear("  operador  ");
Console.WriteLine($"Nome válido: '{usuario.Username}'");

try { usuario.Renomear("   "); }
catch (ArgumentException erro) { Console.WriteLine($"Recusa: {erro.Message}"); }

Console.WriteLine($"Preservou nome anterior? '{usuario.Username}'");

// === TESTE 2: LIVRO E FRONTEIRAS DE LEITURA ===
Console.WriteLine("\n=== TESTE DE LIVRO (120 páginas) ===");
var livro = new Livro("C# Essencial", 120);

int[] tentativasLeitura = { 20, -1, 0, 101, 100, 1 };
foreach (int qtd in tentativasLeitura)
{
    try
    {
        livro.Ler(qtd);
        Console.WriteLine($"Ler({qtd,3}) -> ACEITO   | PaginasLidas = {livro.PaginasLidas}/{livro.Paginas}");
    }
    catch (ArgumentException erro)
    {
        Console.WriteLine($"Ler({qtd,3}) -> RECUSADO | PaginasLidas mantido = {livro.PaginasLidas} ({erro.Message})");
    }
}
```

### Saída Esperada no Console

```text
=== TESTE DE USUÁRIO ===
Nome válido: 'operador'
Recusa: O nome de usuário é obrigatório. (Parameter 'novoNome')
Preservou nome anterior? 'operador'

=== TESTE DE LIVRO (120 páginas) ===
Ler( 20) -> ACEITO   | PaginasLidas = 20/120
Ler( -1) -> RECUSADO | PaginasLidas mantido = 20 (Leitura inválida (-1). Restam 100 páginas.)
Ler(  0) -> RECUSADO | PaginasLidas mantido = 20 (Leitura inválida (0). Restam 100 páginas.)
Ler(101) -> RECUSADO | PaginasLidas mantido = 20 (Leitura inválida (101). Restam 100 páginas.)
Ler(100) -> ACEITO   | PaginasLidas = 120/120
Ler(  1) -> RECUSADO | PaginasLidas mantido = 120 (Leitura inválida (1). Restam 0 páginas.)
```

---

## 4. Exemplo Prático Completo #3 — Quando Lançar Exceção (`throw`) vs Retornar `bool`?

Uma decisão importante de design em C#:
- **Em Construtores:** **sempre use `throw`** — construtores não têm tipo de retorno `bool`; se os dados iniciais forem inválidos, o objeto não pode nascer!
- **Em Métodos de Operação:**
  - Use **`throw`** quando a falha precisa carregar uma **mensagem explicativa detalhada** ou interromper uma cadeia de operações (é o padrão dos Serviços no **MarketFlow**: `ProdutoService`, `VendaService`, `AutenticacaoService`).
  - Use **`bool`** (`true` = sucesso, `false` = recusado sem alterar estado) quando a recusa é um resultado simples e esperado que o chamador quer testar num `if` (exatamente como `int.TryParse`!).

Veja este exemplo completo comparando os dois na mesma classe (`Cofrinho.cs`):

### Arquivo 1: `Cofrinho.cs`

```csharp
public class Cofrinho
{
    public string Dono { get; }
    public decimal Saldo { get; private set; }

    // Construtor SEMPRE usa throw (não existe retorno bool em construtor!):
    public Cofrinho(string dono)
    {
        if (string.IsNullOrWhiteSpace(dono))
            throw new ArgumentException("Informe o dono do cofrinho.", nameof(dono));
        Dono = dono.Trim();
    }

    // Método retornando bool: recusa sem precisar de try/catch, preservando o saldo!
    public bool Depositar(decimal valor)
    {
        if (valor <= 0m) return false;
        Saldo += valor;
        return true;
    }

    public bool Retirar(decimal valor)
    {
        if (valor <= 0m || valor > Saldo) return false;
        Saldo -= valor;
        return true;
    }
}
```

### Arquivo 2: `Program.cs`

```csharp
using System.Globalization;

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");

var cofre = new Cofrinho("Maria");
Console.WriteLine($"Depositar 50,00: {cofre.Depositar(50m)} | Saldo: {cofre.Saldo:C}");
Console.WriteLine($"Retirar 80,00  : {cofre.Retirar(80m)}   | Saldo: {cofre.Saldo:C}");
Console.WriteLine($"Retirar 20,00  : {cofre.Retirar(20m)}   | Saldo: {cofre.Saldo:C}");
```

### Saída Esperada no Console

```text
Depositar 50,00: True | Saldo: R$ 50,00
Retirar 80,00  : False | Saldo: R$ 50,00
Retirar 20,00  : True | Saldo: R$ 30,00
```

---

## 5. Roteiro de Prática e Desafio Autônomo (`A05.01`–`A05.05`)

No projeto **`Aula05Encapsulamento`**, complete e entregue:

1. **`A05.01` e `A05.02` (`Usuario.cs`):** demonstre `Renomear` válido e vazio dentro de `try/catch`, provoque **`CS0272`** tentando `usuario.Username = "hack";`, registre o erro e remova a linha.
2. **`A05.03` e `A05.04` (`Livro.cs`):** demonstre que `new Livro()` vazio não compila, teste todas as 6 leituras (`20`, `-1`, `0`, `101`, `100`, `1`) comprovando que `PaginasLidas` só muda quando válido.
3. **`A05.05` — Desafio Independente (`SessaoDeEstudo.cs`):** seguindo o padrão do `Cofrinho` acima, modele do zero a classe `SessaoDeEstudo`:
   - Propriedades: `Assunto { get; }`, `MetaMinutos { get; }`, `MinutosEstudados { get; private set; }`.
   - Construtor exigindo assunto preenchido e `metaMinutos > 0` (com **`throw`**).
   - Método `public bool RegistrarMinutos(int minutos)` retornando `false` (sem mudar estado!) se `minutos <= 0` ou ultrapassar `MetaMinutos - MinutosEstudados`, e `true` somando quando válido.
   - Comprove no `Program.cs` os **6 testes obrigatórios**: meta `45` (nasce `0`) → registra `20` (`20`) → tenta `26` (`false`, mantém `20`) → registra `25` (`45`) → segunda sessão independente (nasce `0`) → meta `0` (recusada no construtor com `throw`).

---

## 6. Erros Comuns de Iniciantes (❌ Errado vs ✅ Certo)

| Erro / Sintoma | ❌ Código que quebra | ✅ Como corrigir |
| --- | --- | --- |
| **`CS0272`** (`set` inacessível) | `usuario.Username = "ana";` (fora da classe com `private set`) | Chame o método público: `usuario.Renomear("ana");`. |
| **`CS7036`** (faltou argumento no `new`) | `var l = new Livro();` (quando já existe `Livro(string, int)`) | Passe os dados obrigatórios: `new Livro("C#", 120)`. |
| **`CS0542` / construtor com `void`** | `public void Livro(string t)` *(virou método comum, não construtor!)* | Construtor **não tem** tipo de retorno: `public Livro(string t)`. |
| **Alterar antes de validar** | `paginasLidas += q; if (q <= 0) throw ...;` | Valide **SEMPRE primeiro** e só altere o campo na última linha! |
| **`catch { }` vazio** | `try { ... } catch { }` *(esconde o bug!)* | Capture o tipo específico (`catch (ArgumentException erro)`) e trate ou exiba `erro.Message`. |

[Anterior: Aula 04](04-Classes-Objetos-e-Referencias.md) · [Próxima: Aula 06 — Composição, coleções e subtotal](06-Composicao-Colecoes-e-Subtotal.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 05 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-05). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
