# Aula 05 — Construtores, Campos, Encapsulamento e Exceções

**Duração:** 3h30. **Pré-requisito:** [Aula 04](04-Classes-Objetos-e-Referencias.md). **Entrega:** classes blindadas contra estados inválidos usando construtores obrigatórios, campos privados vs propriedades (`{ get; }` / `private set`), validação com `throw` / `try-catch` e comparação com retorno `bool` (`A05.01`–`A05.05`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Mostrar a falha da Aula 04: `produto.Preco = -50m` compila sem erro! |
| Explicação | 30 | Construtores, campos vs propriedades, `private set`, `throw`, `try/catch`, `static` e `nameof` |
| Construção | 60 | Construir `Usuario.cs` e `Livro.cs` encapsulados no Visual Studio 2022 |
| Intervalo | 15 | Pausa |
| Prática | 60 | Testar recusas com `try/catch` e construir o desafio independente `SessaoDeEstudo` |
| Entrega | 30 | Demonstrar preservação de estado após recusa, erro `CS0272` e registrar evidências |

---

## 1. O Problema Real: Por Que `{ get; set; }` Público É Perigoso?

Na Aula 04, qualquer linha de código podia escrever `produto.Preco = -10m;` ou criar `new Livro()` esquecendo de preencher o título. O código compilava, mas deixava o objeto em **estado corrompido**!

Para garantir que um objeto **só exista em estado válido**, usamos duas proteções juntas:
1. **Construtor:** exige os dados obrigatórios no momento do `new` e valida tudo antes de guardar.
2. **Encapsulamento:** fecha a escrita direta (`private set` ou somente `{ get; }`) e obriga alterações a passarem por métodos que validam a regra antes de mudar qualquer valor.

---

## 2. Anatomia do Encapsulamento, Exceções e `nameof` (Microexemplo: `Bateria.cs`)

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
            throw new ArgumentException("Informe o modelo.", nameof(modelo));
        if (capacidadeMaxima <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacidadeMaxima), "Capacidade deve ser positiva.");

        Modelo = modelo.Trim();
        CapacidadeMaxima = capacidadeMaxima;
    }

    public void Carregar(int quantidade)
    {
        if (quantidade <= 0 || quantidade > CapacidadeMaxima - cargaAtual)
            throw new ArgumentException("Quantidade inválida.");

        cargaAtual += quantidade; // Mesmo que: cargaAtual = cargaAtual + quantidade;
    }
}
```

### Dissecando Cada Detalhe e o "Por Quê?"

| Código | O que é? | Por que usamos dessa forma? |
| --- | --- | --- |
| `private int cargaAtual;` | **Campo privado.** | Armazena o dado internamente; código externo não pode lê-lo nem alterá-lo direto. |
| `public int CargaAtual { get { return cargaAtual; } }` | **Propriedade pública somente leitura.** | Expõe a leitura do campo com segurança. |
| `{ get; private set; }` | Propriedade automática com escrita restrita. | Qualquer classe pode ler; **somente métodos da própria classe** podem modificar. |
| `{ get; }` | Propriedade imutável pós-construção. | Só pode receber valor dentro do construtor; nunca mais muda depois. |
| `public Bateria(...)` | **Construtor.** | Mesmo nome da classe, **sem tipo de retorno**. Ao criá-lo com parâmetros, `new Bateria()` vazio deixa de compilar! |
| `string.IsNullOrWhiteSpace` | Método **estático (`static`)** de `string`. | Verifica `null`, vazio `""` e espaços `"   "`. É chamado pelo nome do tipo (`string.`), sem `new`, pois não depende de um objeto específico. |
| `nameof(modelo)` | Operador que devolve o nome `"modelo"`. | Se você renomear o parâmetro no VS 2022 (`Ctrl+R, R`), `nameof` atualiza junto automaticamente! |
| `throw new ArgumentException(...)` | Lança uma **exceção**. | Aborta o método **antes** das linhas de atribuição, preservando o estado anterior intacto! |

### Capturando Exceções com `try / catch` vs Retorno `bool`
- **`try / catch`:** envolve chamadas que podem lançar `throw` para exibir `erro.Message` sem fechar o programa:
  ```csharp
  try { bateria.Carregar(999); }
  catch (ArgumentException erro) { Console.WriteLine(erro.Message); }
  ```
  **Nunca** escreva um `catch { }` vazio — ele esconde falhas silenciosamente!
- **Quando retornar `bool` em vez de `throw`?** Em **construtores**, sempre usamos `throw` (não há retorno `bool`). Em **métodos de operação** (como `RegistrarMinutos`), você pode usar `throw` (como faremos nos serviços do MarketFlow) ou retornar `bool` (`false` em caso de recusa, mantendo o estado intacto) quando a recusa faz parte do fluxo esperado pelo chamador.

---

## 3. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

Crie um Aplicativo de Console `Aula05Encapsulamento` no **Visual Studio 2022** e construa cada classe em seu próprio arquivo `.cs` (`Shift+Alt+C`):

### Construção 1 — Classe `Usuario.cs` (`A05.01` e `A05.02`)
1. Declare `public class Usuario` com:
   - `public string Username { get; private set; } = "";`
   - Método `public void Renomear(string novoNome)`: se `string.IsNullOrWhiteSpace(novoNome)`, lance `ArgumentException`; caso contrário, atribua `Username = novoNome.Trim();`.
2. No `Program.cs`: chame `Renomear("operador")`, tente `Renomear("")` dentro de `try / catch (ArgumentException erro)` e comprove que `usuario.Username` **continuou `"operador"`**.
3. **Teste do compilador:** escreva temporariamente `usuario.Username = "hack";`, observe o erro `CS0272` na **Lista de Erros** e apague a linha.

### Construção 2 — Classe `Livro.cs` com Construtor e Campo Privado (`A05.03` e `A05.04`)
1. Declare `public class Livro` com:
   - `public string Titulo { get; private set; }`
   - `public int Paginas { get; }`
   - Campo privado `private int paginasLidas;` e propriedade pública `PaginasLidas` (apenas `get`).
   - Construtor `public Livro(string titulo, int paginas)`: valide título preenchido e `paginas > 0` lançando `ArgumentException` (com `nameof`); guarde `Titulo = titulo.Trim()` e `Paginas = paginas`.
   - Métodos `Renomear(string novoTitulo)` e `Ler(int quantidade)` (recusando `quantidade <= 0 || quantidade > Paginas - paginasLidas`).
2. No `Program.cs`, teste todas as fronteiras dentro de `try/catch`: título vazio, `0` páginas, livro válido (`120` páginas), ler `20` (vai para `20`), tentar ler `-1`, `0` e `101` (deve permanecer `20` após cada recusa!), ler `100` (vai para `120`) e tentar ler mais `1` (deve recusar mantendo `120`).

### Construção 3 — Desafio Independente `SessaoDeEstudo.cs` (`A05.05`)
Modele do zero uma classe `SessaoDeEstudo` com `Assunto` (`{ get; }`), `MetaMinutos` (`{ get; }`) e `MinutosEstudados` (`{ get; private set; }`):
- Exija assunto preenchido e meta positiva no construtor (com `throw`).
- Crie um método `public bool RegistrarMinutos(int minutos)` que retorne `false` (sem alterar estado!) se `minutos <= 0` ou ultrapassar o restante da meta, e `true` somando o tempo quando válido.
- **Testes obrigatórios:** meta `45` (começa em `0`) → registra `20` (`20`) → tenta `26` (recusa, mantém `20`) → registra `25` (`45`) → segunda sessão independente (começa em `0`) → meta `0` (recusada no construtor).

[Anterior: Aula 04](04-Classes-Objetos-e-Referencias.md) · [Próxima: Aula 06 — Composição, coleções e subtotal](06-Composicao-Colecoes-e-Subtotal.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 05 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-05). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
