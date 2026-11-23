# F05 — Construtores, Campos, Encapsulamento e Exceções

**Pré-requisito:** [F04](04-Classes-e-Objetos.md).
**Objetivo:** impedir que um objeto nasça ou fique em estado inválido usando **construtores**, **encapsulamento** (`private`, `private set`), lançamento de exceções (`throw`) e tratamento seguro (`try/catch`).

Crie no **Visual Studio 2022** um novo **Aplicativo de Console** chamado `FundamentosF05`. Não misture os arquivos antigos de `F04` aqui.

---

## 1. Qual Problema Estamos Resolvendo?

Na etapa `F04`, todas as propriedades tinham `{ get; set; }` públicos. Isso abria duas falhas sérias:
1. Era possível fazer `new Livro()` e **esquecer** de preencher `Titulo` e `Paginas`.
2. Qualquer trecho do código podia escrever `livro.Paginas = -10;` — o programa compilava, mas o objeto ficava corrompido!

**Por que precisamos de Construtor e Encapsulamento?**
- O **Construtor** é a "porta de entrada" obrigatória: garante que o objeto **só nasce se receber dados válidos**.
- O **Encapsulamento** fecha o acesso direto às propriedades e exige que toda alteração passe por métodos que verificam as regras de negócio antes de modificar o estado.

---

## 2. Anatomia do Construtor, Campos, Propriedades e `throw` (Microexemplo)

Observe este modelo genérico de uma **Bateria de Celular** (`Bateria.cs`):

```csharp
public class Bateria
{
    public string Modelo { get; private set; }
    public int CapacidadeMaxima { get; }
    private int cargaAtual;

    public int CargaAtual
    {
        get { return cargaAtual; }
    }

    public Bateria(string modelo, int capacidadeMaxima)
    {
        if (string.IsNullOrWhiteSpace(modelo))
            throw new ArgumentException("Informe o modelo.");
        if (capacidadeMaxima <= 0)
            throw new ArgumentException("A capacidade deve ser positiva.");

        Modelo = modelo.Trim();
        CapacidadeMaxima = capacidadeMaxima;
    }

    public void Carregar(int quantidade)
    {
        if (quantidade <= 0 || quantidade > CapacidadeMaxima - cargaAtual)
            throw new ArgumentException("Quantidade de carga inválida.");

        cargaAtual = cargaAtual + quantidade;
    }
}
```

### Dissecando Cada Detalhe e o "Por Quê?"

| Código | O que é? | Por que escrevemos dessa forma? |
| --- | --- | --- |
| `private int cargaAtual;` | **Campo privado** (variável interna da classe). | Armazena o valor internamente. Por ser `private`, nenhum código externo consegue fazer `bateria.cargaAtual = 9999;`. |
| `public int CargaAtual { get { return cargaAtual; } }` | **Propriedade pública somente leitura** apoiada em campo. | Permite que o mundo externo **leia** a carga atual, mas não altere diretamente. |
| `public string Modelo { get; private set; }` | **Propriedade automática com `private set`.** | O compilador cria o campo oculto sozinho. Todos podem ler (`get`), mas só métodos **de dentro da própria classe** podem modificar (`private set`). |
| `public int CapacidadeMaxima { get; }` | **Propriedade somente `get`.** | Só pode receber valor dentro do **construtor**; depois que o objeto nasce, nunca mais muda. |
| `public Bateria(string modelo, int capacidadeMaxima)` | **Construtor.** | Tem exatamente o mesmo nome da classe e **não** tem tipo de retorno (nem `void`). Executa automaticamente durante `new Bateria(...)`. Ao criar um construtor com parâmetros, `new Bateria()` sem argumentos deixa de compilar! |
| `string.IsNullOrWhiteSpace(modelo)` | Método estático da classe `string`. | Retorna `true` se o texto for `null`, vazio (`""`) ou só espaços (`"   "`). Muito mais seguro que testar apenas `modelo == ""`. |
| `modelo.Trim()` | Remove espaços no início e no fim do texto. | Evita salvar `"  Lítio  "` com espaços acidentais nas bordas. |
| `throw new ArgumentException("...");` | **Lança uma exceção.** | Interrompe imediatamente o método/construtor **antes** de chegar nas linhas de atribuição. Assim o dado inválido nunca contamina o objeto! |

> [!TIP]
> **Atenção:** apenas colocar `private` não é encapsulamento completo. Encapsular significa esconder a escrita direta **e** oferecer métodos públicos (como `Carregar` ou `Renomear`) que validam a regra antes de alterar o estado.

---

## 3. Capturando uma Recusa Sem Derrubar o Programa (`try / catch`)

Quando um método executa `throw new ArgumentException(...)`, se ninguém tratar esse erro, o programa fecha com erro. Para capturar a recusa de maneira controlada, envolvemos a chamada em um bloco `try / catch`:

```csharp
try
{
    // Código que pode lançar uma exceção
    bateria.Carregar(500);
}
catch (ArgumentException erro)
{
    // Só executa se o 'try' lançou uma ArgumentException
    Console.WriteLine($"Operação recusada: {erro.Message}");
}
// O programa continua normalmente a partir daqui!
```

- `try` ("tente"): executa o caminho normal. Se uma linha lançar exceção, as linhas restantes dentro do `try` são **puladas** imediatamente.
- `catch (ArgumentException erro)`: captura o objeto de erro na variável `erro` e permite exibir `erro.Message` (a frase que você escreveu no `throw`).
- **Por que nunca escrever `catch { }` vazio?** Um `catch` vazio engole o erro em silêncio e faz o usuário achar que deu certo quando falhou.

---

## 4. Membros de Instância vs `static` e a Palavra `this`

- **Método de instância (`bateria.Carregar(20)`):** precisa de um objeto criado com `new` para saber *qual* bateria terá seu estado alterado.
- **Método estático (`string.IsNullOrWhiteSpace(texto)`):** pertence ao **tipo** em si, não a um objeto específico; por isso é chamado pelo nome do tipo (`string.`), sem usar `new`.
- **Palavra `this`:** significa *"esta instância atual"*. Quando um parâmetro tem o mesmo nome de um campo (ex.: `this.modelo = modelo;`), `this` desfaz a ambiguidade. Quando usamos nomes com capitalização diferente (`Titulo = titulo;`), o compilador já distingue sem precisar de `this`.

---

## 5. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

Agora você vai construir a sua própria classe `Livro` blindada em `FundamentosF05`, **seguindo as orientações abaixo** (sem copiar a classe `Bateria`):

### Passo 1 — Criar `Livro.cs` (`Shift+Alt+C` no VS 2022)
Declare `public class Livro` (sem `namespace` por enquanto) seguindo estas especificações exatas:
1. **Propriedades e campos:**
   - `Titulo`: `string`, leitura pública e escrita privada (`public string Titulo { get; private set; }`).
   - `Paginas`: `int`, somente leitura (`public int Paginas { get; }`) — o total de páginas de um livro impresso não muda após nascer.
   - Um campo privado `private int paginasLidas;` (começa em 0) e uma propriedade pública `PaginasLidas` apenas com `get` retornando esse campo.
2. **Construtor `public Livro(string titulo, int paginas)`:**
   - Antes de atribuir qualquer coisa, valide:
     - Se `string.IsNullOrWhiteSpace(titulo)` → lance `ArgumentException` com mensagem pedindo o título.
     - Se `paginas <= 0` → lance `ArgumentException` informando que o livro precisa ter páginas.
   - Só após passar pelas validações, atribua `Titulo = titulo.Trim();` e `Paginas = paginas;`.
3. **Método `public void Renomear(string novoTitulo)`:**
   - Recuse com `ArgumentException` se `novoTitulo` for nulo, vazio ou apenas espaços.
   - Caso contrário, atualize `Titulo` com `novoTitulo.Trim()`.
4. **Método `public void Ler(int quantidade)`:**
   - Calcule mentalmente: quantas páginas ainda faltam ler? (`Paginas - paginasLidas`).
   - Recuse com `ArgumentException` se `quantidade <= 0` **ou (`||`)** `quantidade > Paginas - paginasLidas`.
   - Se for válida, some `quantidade` ao campo `paginasLidas`.

### Passo 2 — Construir os Testes no `Program.cs`
No `Program.cs`, escreva um roteiro de testes usando `try / catch (ArgumentException erro)` para comprovar cada regra:
1. Tente criar um `Livro` com título vazio `""` (dentro de `try/catch`) e outro com `0` páginas (dentro de `try/catch`). Ambos devem exibir a mensagem de recusa.
2. Crie um livro válido `"Viagem"` com `120` páginas.
3. Chame `Ler(20)` e exiba `PaginasLidas` (esperado: `20`).
4. Dentro de blocos `try/catch`, tente chamar `Ler(-1)`, `Ler(0)` e `Ler(101)`. Depois de cada recusa, exiba `PaginasLidas` comprovando que o estado **permaneceu intacto em 20**.
5. Chame `Ler(100)` (deve chegar a `120`) e depois tente `Ler(1)` em um `try/catch` (deve recusar mantendo `120`).
6. Chame `Renomear("  Jardim  ")` e comprove que `Titulo` virou `"Jardim"`. Tente `Renomear("   ")` dentro de `try/catch` e comprove que o título continuou `"Jardim"`.
7. **Teste de blindagem do compilador:** escreva temporariamente `livro.PaginasLidas = 50;` ou `livro.Titulo = "Hack";`, leia o erro na **Lista de Erros** do VS 2022 (`CS0200` / `CS0272`) e apague a linha inválida.

---

## 6. Erros Comuns no Visual Studio 2022

| Erro na Lista de Erros | Causa provável | Como corrigir |
| --- | --- | --- |
| `CS7036: Não há nenhum argumento... para o parâmetro 'titulo'` | Você tentou chamar `new Livro()` vazio depois de criar um construtor que exige parâmetros. | Passe os argumentos obrigatórios: `new Livro("Viagem", 120)`. |
| `CS0200 / CS0272: A propriedade é somente leitura / acessador set inacessível` | Tentou atribuir diretamente de fora da classe a uma propriedade com `{ get; }` ou `{ get; private set; }`. | Isso prova que seu encapsulamento funcionou! Use os métodos públicos (`Renomear`, `Ler`). |
| Exceção sem tratamento fecha a janela no VS 2022 | Chamou um método que lançou `throw` fora de um bloco `try / catch`. | Envolva a chamada de teste em `try { ... } catch (ArgumentException erro) { ... }`. |

**Critério para avançar para F06:** explicar por que o construtor exige dados válidos na criação, diferenciar campo privado de propriedade pública e demonstrar no código que uma tentativa inválida preserva o estado anterior do objeto.

[Anterior: F04 — Classes e objetos](04-Classes-e-Objetos.md) · [Próximo: F06 — Composição e coleções](06-Composicao-e-Colecoes.md)
