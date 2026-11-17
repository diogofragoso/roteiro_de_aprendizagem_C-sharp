# F05 — Construtores, campos e encapsulamento

**Pré-requisito:** F04. **Objetivo:** criar um objeto com estado válido e controlar suas alterações.

Crie um console `FundamentosF05`. Os arquivos desta etapa substituem o exercício anterior; não misture versões da mesma classe no projeto.

## 1. Descubra o problema antes da solução

No exemplo anterior, era possível escrever `livro.Paginas = -10`. O código compila, mas esse estado não atende ao nosso modelo de leitura. Também era possível criar um livro sem título e esquecer de preenchê-lo.

Precisamos de uma entrada de construção que exija dados iniciais e de operações que verifiquem as alterações. Esse é o problema que vamos resolver com construtor e encapsulamento.

## 2. Exija os dados na construção

Crie `Livro.cs` com:

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
            throw new ArgumentException("Informe o título.");
        if (paginas <= 0)
            throw new ArgumentException("O livro precisa ter páginas.");

        Titulo = titulo.Trim();
        Paginas = paginas;
    }

    public void Renomear(string novoTitulo)
    {
        if (string.IsNullOrWhiteSpace(novoTitulo))
            throw new ArgumentException("Informe o título.");

        Titulo = novoTitulo.Trim();
    }

    public void Ler(int quantidade)
    {
        if (quantidade <= 0 || quantidade > Paginas - paginasLidas)
            throw new ArgumentException("Quantidade de páginas inválida.");

        paginasLidas = paginasLidas + quantidade;
    }
}
```

O construtor tem o nome `Livro` e não declara tipo de retorno. Ele executa durante `new Livro("Viagem", 120)`. Como declaramos esse construtor com argumentos obrigatórios, `new Livro()` deixa de ser uma chamada válida para esta classe.

No construtor, `titulo` é um parâmetro; `Titulo` é uma propriedade do objeto. `Trim()` remove espaços nas extremidades do texto. A validação ocorre antes da atribuição: dados recusados não chegam às propriedades.

## 3. Diferencie campo de propriedade

`paginasLidas` é um **campo** privado que armazena um valor. Ele começa em 0. `PaginasLidas` é uma **propriedade** pública cujo acessor `get` permite consultar esse campo.

`Titulo` usa uma propriedade automática: o compilador fornece seu armazenamento interno. O `private set` permite atribuição dentro da classe, mas impede `livro.Titulo = "Outro"` no console. Para alterar o título, o chamador deve usar `Renomear`, que valida a entrada.

`Paginas` tem somente `get` e recebe seu valor no construtor. Neste modelo, a quantidade total de páginas não muda depois da construção.

Encapsular significa controlar o acesso ao estado e oferecer operações que preservem as regras. Apenas tornar algo privado não garante uma regra: o método público também precisa validar antes de modificar.

## 4. Observe uma recusa sem encerrar o programa

Em `Program.cs`, use:

```csharp
Livro livro = new Livro("Viagem", 120);
livro.Ler(20);
Console.WriteLine(livro.PaginasLidas);

try
{
    livro.Ler(200);
}
catch (ArgumentException erro)
{
    Console.WriteLine(erro.Message);
}

Console.WriteLine(livro.PaginasLidas);
```

Esperado: 20, a mensagem de quantidade inválida e novamente 20. `throw` interrompe o caminho normal antes da atribuição. O `catch` recebe a exceção e mostra a mensagem; o programa continua depois do bloco. Não acrescente um `catch` vazio: a recusa precisa ser compreendida pelo chamador.

Se a criação receber um título vazio, o construtor lança a exceção e não entrega um objeto construído ao chamador. Para experimentar esse caso sem encerrar o programa, coloque a própria linha `new Livro(...)` dentro de outro `try/catch`.

## 5. Métodos de instância e operações estáticas

`livro.Ler(20)` precisa saber **qual livro** terá seu estado alterado: é um método de instância. Já `string.IsNullOrWhiteSpace(titulo)` é uma operação estática chamada pelo nome do tipo, sem construir um objeto para receber a chamada. `static` não é uma instrução para criar um objeto único nem uma solução geral para compartilhar dados.

Quando encontrar `this` em outros exemplos, leia como “esta instância”. Neste código usamos nomes diferentes para parâmetros e propriedades, então não precisamos escrevê-lo.

## 6. Pratique

1. Tente construir livros com título vazio, 0 páginas e 120 páginas. Os dois primeiros casos devem ser recusados; o terceiro, aceito.
2. Leia 20 páginas de um livro de 120. Tente ler -1, 0 e 101; o estado deve continuar 20 após cada recusa.
3. Leia mais 100: o estado chega a 120. Tentar ler mais 1 deve falhar mantendo 120.
4. Renomeie para `"  Jardim  "`: o título deve ser `"Jardim"`. Tente renomear para espaços: o título deve continuar `"Jardim"`.
5. Tente atribuir diretamente a `PaginasLidas`, registre o erro de compilação e retire a linha inválida.

**Critério para avançar:** explicar por que o construtor exige dados, distinguir campo e propriedade e demonstrar que uma tentativa inválida preserva o estado anterior.

[Anterior: classes e objetos](04-Classes-e-Objetos.md) · [Próximo: composição e coleções](06-Composicao-e-Colecoes.md)
