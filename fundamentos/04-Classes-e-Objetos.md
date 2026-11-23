# F04 — Classes, Objetos, Propriedades e Referências na Memória

**Pré-requisito:** [F03](03-Metodos.md).
**Objetivo:** agrupar dados e comportamentos em classes em arquivos separados no Visual Studio 2022, criar múltiplos objetos independentes e compreender na prática como funcionam as **referências na memória**.

Crie no **Visual Studio 2022** um novo **Aplicativo de Console** chamado `FundamentosF04`.

---

## 1. Por Que Agrupar Dados em uma Classe? (Abstração)

Suponha que seu programa precisa gerenciar livros em uma biblioteca. Sem classes, você teria de criar variáveis soltas:

```csharp
// ❌ Difícil de manter quando há 50 livros!
string titulo1 = "Viagem";
int paginas1 = 120;
string titulo2 = "Jardim";
int paginas2 = 80;
```

Se amanhã precisar enviar um livro para um método, teria que passar vários parâmetros soltos e torcer para não trocar `titulo1` com `paginas2`.

Uma **classe** cria um **novo tipo personalizado** que reúne os dados (**propriedades**) e as ações (**métodos**) daquele conceito em uma única unidade.
Note também que um livro real tem peso, cor da capa, tipo de papel e cheiro, mas para um app de controle de leitura só escolhemos `Titulo` e `Paginas`. Escolher apenas os detalhes que importam para o problema se chama **Abstração**.

### Classe vs Objeto (A Diferença Fundamental)
- **Classe:** é o **molde** ou a **planta baixa** (existe no código-fonte, define quais propriedades e métodos aquele tipo possui).
- **Objeto (Instância):** é cada **exemplar real construído na memória RAM** usando a palavra-chave `new`. A partir de uma única classe `Livro`, podemos criar 1, 2 ou 10.000 objetos independentes, cada um com seu próprio **estado** (o conjunto dos seus valores naquele momento).

---

## 2. Como Criar uma Classe em Arquivo Separado no Visual Studio 2022

**Por que colocar cada classe em seu próprio arquivo `.cs`?**
Quando o projeto cresce, encontrar `Livro` em um arquivo chamado `Livro.cs` é imediato. Além disso, em projetos com instruções de nível superior no `Program.cs`, misturar código solto depois de declarações de classe gera erro de compilação (`CS8803`).

### Passo a passo no Visual Studio 2022:
1. No **Gerenciador de Soluções** (painel à direita), clique com o botão direito sobre o nome do projeto `FundamentosF04`.
2. Selecione **Adicionar → Classe...** (atalho rápido: **`Shift+Alt+C`**).
3. Na janela que abrir, confirme que **Classe** está selecionado, digite o nome do arquivo na caixa inferior (ex.: `Filme.cs`) e clique em **Adicionar**.
4. O Visual Studio cria o arquivo com um `namespace` padrão. **Apenas nesta etapa F04 (e em F05–F06)**, remova as linhas `using` desnecessárias e o `namespace` para focar só na anatomia da classe (introduziremos `namespace` com calma em `F07`).

### Anatomia de uma Classe (Microexemplo: `Filme.cs`)

```csharp
public class Filme
{
    public string Titulo { get; set; } = "";
    public int DuracaoMinutos { get; set; }

    public string Resumo()
    {
        return $"{Titulo} ({DuracaoMinutos} min)";
    }
}
```

Dissecando cada elemento:

| Elemento | O que significa? | Por que usamos assim? |
| --- | --- | --- |
| `public` | Modificador de acesso público. | Permite que o `Program.cs` (e outras classes) enxerguem e utilizem `Filme`. |
| `class Filme` | Declara o novo tipo `Filme`. | O nome da classe deve coincidir com o nome do arquivo (`Filme.cs`) em PascalCase. |
| `public string Titulo { get; set; } = "";` | **Propriedade automática.** | `get` permite ler o valor; `set` permite alterar o valor. O `= ""` define um texto inicial vazio para evitar o aviso de nulabilidade `CS8618` do compilador moderno. |
| `public int DuracaoMinutos { get; set; }` | Propriedade numérica. | Tipos numéricos começam automaticamente em `0`, sem precisar de `= 0`. |
| `public string Resumo()` | **Método de instância.** | Trabalha diretamente com as propriedades *do objeto específico* que recebeu a chamada (se chamarmos `filme1.Resumo()`, usa o título e a duração do `filme1`). |

### Como instanciar e usar objetos (`new` e ponto `.`)

```csharp
Filme primeiro = new Filme(); // 'new' aloca um objeto novo na memória RAM
primeiro.Titulo = "Aventura";
primeiro.DuracaoMinutos = 110;

Filme segundo = new Filme();  // Outro objeto, totalmente separado na memória
segundo.Titulo = "Comédia";
segundo.DuracaoMinutos = 95;
```

- Cada uso de `new Filme()` cria uma nova instância independente. Alterar `segundo.DuracaoMinutos = 100;` **não** afeta `primeiro.DuracaoMinutos`.

---

## 3. O Conceito Que Mais Causa Bugs em Iniciantes: Referência na Memória

Compare o que acontece quando copiamos um `int` (F03) vs quando copiamos uma variável de classe (`Filme`):

```csharp
Filme outroNome = primeiro; // NÃO existe 'new' aqui!
outroNome.DuracaoMinutos = 130;
```

**O que aconteceu na memória RAM?**
Variáveis de classes não guardam o objeto inteiro "dentro da gaveta" — elas guardam um **endereço (uma seta/controle remoto)** apontando para o objeto na memória.

```text
primeiro  ────┐
              ├──> [ Objeto Filme: Titulo="Aventura", DuracaoMinutos=130 ]
outroNome ────┘

segundo   ───────> [ Objeto Filme: Titulo="Comédia", DuracaoMinutos=95 ]
```

Como não houve um novo `new Filme()`, `primeiro` e `outroNome` apontam para o **mesmo objeto**. Alterar por um lado aparece imediatamente ao ler pelo outro!
No sistema do supermercado, entender isso será vital para saber quando dois itens compartilham o mesmo objeto `Produto`.

---

## 4. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

Construa os dois exercícios abaixo no projeto `FundamentosF04`, seguindo as orientações passo a passo:

### Construção 1 — Classe `Livro` e Teste de Referência Compartilhada
1. No **Visual Studio 2022**, adicione a classe `Livro.cs` (`Shift+Alt+C`).
2. Declare a classe `public class Livro` (sem `namespace` por enquanto) contendo:
   - Propriedade pública `string Titulo` com `{ get; set; }` inicializada com `""`.
   - Propriedade pública `int Paginas` com `{ get; set; }`.
   - Método público `Descrever()` que não recebe parâmetros e retorna uma `string` interpolada no formato `"{Titulo}: {Paginas} páginas"`.
3. No `Program.cs`:
   - Instancie um objeto `primeiro` (`"Viagem"`, `120` páginas) e um objeto `segundo` (`"Jardim"`, `80` páginas) usando `new Livro()`.
   - Exiba o retorno de `Descrever()` para ambos.
   - Altere `segundo.Paginas` para `90` e exiba as páginas de `primeiro` e `segundo` para comprovar que são independentes (`120` e `90`).
   - Agora crie uma variável `Livro outroNome = primeiro;` (sem `new`), altere `outroNome.Paginas` para `150` e exiba `primeiro.Paginas`, `outroNome.Paginas` e `segundo.Paginas`.
   - **Preveja antes de executar com `Ctrl+F5`:** qual será a saída das 3 linhas? Desenhe no caderno quantas **variáveis** existem (3) e quantos **objetos** existem na memória (2).

### Construção 2 — Sua Própria Classe `Aluno`
1. Adicione um segundo arquivo de classe no projeto chamado `Aluno.cs`.
2. Modele a classe `public class Aluno` com:
   - Propriedade `Nome` (`string`, inicializada como `""`);
   - Método `Apresentar()` que devolve uma frase de apresentação usando o `Nome` do aluno.
3. No `Program.cs`, crie dois objetos `Aluno` diferentes, exiba a apresentação de ambos, altere apenas um deles e comprove que o outro não mudou.
4. Em seguida, crie uma segunda referência apontando para o primeiro aluno, altere o nome por ela e explique no caderno por que a primeira variável enxergou a mudança.

---

## 5. Erros Comuns no Visual Studio 2022

| Erro na Lista de Erros | Causa provável | Como resolver |
| --- | --- | --- |
| `CS8618: Propriedade não anulável 'Titulo' precisa conter um valor não nulo` | Declarou `public string Titulo { get; set; }` sem valor inicial. | Inicialize com `= "";` (em `F05` aprenderemos a inicializar pelo construtor). |
| `CS8803: Instruções de nível superior devem preceder declarações de namespace e tipo` | Colocou código de execução abaixo de uma classe no `Program.cs`. | Mantenha cada classe em seu próprio arquivo `.cs` e deixe apenas instruções de execução no `Program.cs`. |
| `CS0122: 'Livro' é inacessível devido ao seu nível de proteção` | Faltou escrever `public` antes de `class` ou da propriedade/método. | Adicione `public` na declaração. |

**Critério para avançar para F05:** criar uma classe em arquivo separado no Visual Studio 2022, instanciar dois objetos independentes e explicar no desenho a diferença entre criar com `new` e copiar uma referência.

[Anterior: F03 — Métodos](03-Metodos.md) · [Próximo: F05 — Construtores e encapsulamento](05-Construtores-e-Encapsulamento.md)
