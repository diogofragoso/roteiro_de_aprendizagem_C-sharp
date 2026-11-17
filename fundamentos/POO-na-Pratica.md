# POO na prática com C#

Use este laboratório depois de [métodos (F03)](03-Metodos.md). Você vai criar objetos no console, acompanhar as mudanças de estado e observar como a mesma chamada pode executar comportamentos diferentes.

Crie um projeto de **Aplicativo de Console** chamado `PooNaPratica`. Em cada exemplo, substitua **todo** o conteúdo de `Program.cs` pelo bloco correspondente. As instruções de execução ficam no início do arquivo e as declarações dos tipos ficam no final.

## 1. Abstração, classes e objetos

Imagine um aplicativo para acompanhar leituras. Ele precisa do título, da quantidade de páginas e do progresso. A cor da capa só entraria no modelo se houvesse uma função que usasse essa informação. **Abstração** é escolher os dados e as operações que fazem sentido para o problema.

`Livro` é uma **classe** que define o tipo. Cada chamada a `new Livro(...)` cria um **objeto**, também chamado de instância. Os valores que ele guarda em um dado momento são seu **estado**. Os métodos expressam seu **comportamento**.

## 2. Construtor e encapsulamento

Um livro começa com zero páginas lidas. Depois, o progresso só pode aumentar até o total de páginas. O **construtor** recebe e valida os dados necessários na criação. O **encapsulamento** faz com que alterações de progresso passem por uma operação que verifica a regra.

```csharp
Livro viagem = new Livro("Viagem", 120);
Livro jardim = new Livro("Jardim", 80);

viagem.RegistrarLeitura(20);
Console.WriteLine(viagem.Resumo());
Console.WriteLine(jardim.Resumo());

Livro leituraAtual = viagem;
leituraAtual.RegistrarLeitura(10);
Console.WriteLine(viagem.Resumo());

if (!viagem.RegistrarLeitura(100))
{
    Console.WriteLine("Quantidade recusada.");
}

Console.WriteLine(viagem.Resumo());

public class Livro
{
    public string Titulo { get; }
    public int TotalPaginas { get; }
    public int PaginasLidas { get; private set; }

    public Livro(string titulo, int totalPaginas)
    {
        if (string.IsNullOrWhiteSpace(titulo))
        {
            throw new ArgumentException("Informe o título.", nameof(titulo));
        }

        if (totalPaginas <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(totalPaginas), "O total de páginas deve ser positivo.");
        }

        Titulo = titulo.Trim();
        TotalPaginas = totalPaginas;
    }

    public bool RegistrarLeitura(int quantidade)
    {
        if (quantidade <= 0 || quantidade > TotalPaginas - PaginasLidas)
        {
            return false;
        }

        PaginasLidas += quantidade;
        return true;
    }

    public string Resumo()
    {
        return $"{Titulo} | {PaginasLidas}/{TotalPaginas} páginas";
    }
}
```

Saída esperada

```text
Viagem | 20/120 páginas
Jardim | 0/80 páginas
Viagem | 30/120 páginas
Quantidade recusada.
Viagem | 30/120 páginas
```

- `Titulo`, `TotalPaginas` e `PaginasLidas` são **propriedades**. `get` permite a leitura. O `private set` permite escrever em `PaginasLidas` dentro da própria classe. São propriedades automáticas, cujo armazenamento fica em campos gerados pelo compilador.
- Em `Livro leituraAtual = viagem`, as duas variáveis passam a referenciar a mesma instância. Por isso, a chamada feita por `leituraAtual` aparece no resumo obtido por `viagem`. O objeto referenciado por `jardim` mantém seu próprio estado.
- O construtor lança uma exceção quando recebe dados inválidos. Já `RegistrarLeitura` devolve `false` quando a quantidade é recusada e deixa o estado como estava. Quem chama o método decide como mostrar a recusa.

**Experimente.** Depois da última chamada a `Resumo`, registre mais 90 páginas em `viagem` e tente registrar mais uma. O progresso deve chegar a 120 e permanecer nesse valor. Em seguida, tente escrever `viagem.PaginasLidas = 500;` e identifique por que a compilação falha. Retire a linha antes de continuar.

## 3. Herança, interfaces e polimorfismo

Neste exemplo, um painel mostra avisos da biblioteca. A **interface** `IExibivel` descreve a operação que o painel pede. A **herança** permite que `AvisoUrgente` especialize `Aviso` e reutilize o título e a formatação da classe base. O painel também exibe um `Rodape`, que implementa o mesmo contrato.

```csharp
IExibivel[] painel =
{
    new Aviso("Devolução até sexta"),
    new AvisoUrgente("Biblioteca fecha às 16h"),
    new Rodape()
};

foreach (IExibivel item in painel)
{
    Console.WriteLine(item.Exibir());
}

public interface IExibivel
{
    string Exibir();
}

public class Aviso : IExibivel
{
    public string Titulo { get; }

    public Aviso(string titulo)
    {
        Titulo = titulo;
    }

    public virtual string Exibir()
    {
        return $"Aviso | {Titulo}";
    }
}

public class AvisoUrgente : Aviso
{
    public AvisoUrgente(string titulo) : base(titulo)
    {
    }

    public override string Exibir()
    {
        return $"URGENTE | {base.Exibir()}";
    }
}

public class Rodape : IExibivel
{
    public string Exibir()
    {
        return "Consulte a recepção para mais informações.";
    }
}
```

Saída esperada

```text
Aviso | Devolução até sexta
URGENTE | Aviso | Biblioteca fecha às 16h
Consulte a recepção para mais informações.
```

- Em `Aviso : IExibivel`, a classe implementa uma interface. Em `AvisoUrgente : Aviso`, a classe deriva de outra classe. O compilador exige que os tipos exibíveis ofereçam a operação `Exibir()`.
- `: base(titulo)` chama o construtor de `Aviso` para inicializar o título. `virtual` autoriza a especialização do método e `override` declara a especialização. Dentro dela, `base.Exibir()` chama explicitamente a implementação da classe base.
- No laço, `item` tem o tipo declarado `IExibivel`. A chamada `item.Exibir()` executa a implementação correspondente a cada objeto. Essa é a manifestação do **polimorfismo** no exemplo.

**Experimente.** Troque `new AvisoUrgente(...)` por `new Aviso(...)` mantendo o mesmo título. Preveja qual parte da segunda linha muda. Depois, implemente uma classe `Horario : IExibivel` e inclua uma instância no painel. O mesmo laço deve exibir o quarto item.

## 4. Prática independente

Modele uma `SessaoDeEstudo` com assunto, meta em minutos e tempo estudado. Exija assunto preenchido e meta positiva na construção. Crie um método para registrar minutos e recusar valores que ultrapassem o tempo restante.

| Caso para demonstrar | Resultado esperado |
| --- | --- |
| Criar uma sessão de 45 minutos | Progresso inicial de 0 |
| Registrar 20 minutos | Progresso de 20 |
| Tentar registrar 26 minutos | Recusa e progresso de 20 |
| Registrar 25 minutos | Progresso de 45 |
| Criar outra sessão | A nova sessão começa em 0 |
| Criar uma sessão com meta zero | Construção recusada |

Ao terminar, explique quais detalhes você escolheu para representar uma sessão, onde a regra de progresso é aplicada e por que as duas instâncias guardam estados independentes.

Continue com [classes e referências](04-Classes-e-Objetos.md), [encapsulamento](05-Construtores-e-Encapsulamento.md), [composição](06-Composicao-e-Colecoes.md) e [interfaces](07-Interfaces-e-Transicao.md), ou siga para a [revisão aplicada ao supermercado](../03-Orientacao-a-Objetos.md) se já concluiu os fundamentos.
