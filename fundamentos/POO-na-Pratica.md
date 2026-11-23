# Laboratório Complementar — POO na Prática com C# (Visual Studio 2022)

Use este laboratório como reforço prático a partir da etapa [F03 (Métodos)](03-Metodos.md) e em paralelo às etapas `F04`–`F07`. O objetivo é ler, prever, depurar e **construir** modelos orientados a objetos no console antes de aplicá-los ao supermercado.

Crie no **Visual Studio 2022** um **Aplicativo de Console** chamado `PooNaPratica`.

---

## 1. Revisão dos Conceitos-Chave (O Quê e Por Quê)

| Conceito | O que significa? | Por que é importante na prática? |
| --- | --- | --- |
| **Abstração** | Escolher apenas os dados e comportamentos relevantes para o sistema. | Evita poluir o código com propriedades inúteis (ex.: para controlar leitura de livros, importam título e páginas; não importa o peso da capa). |
| **Classe e Objeto** | A classe é o molde; cada `new` cria um objeto (instância) com estado próprio. | Permite representar dezenas de registros independentes sem criar variáveis numeradas (`livro1`, `livro2`...). |
| **Encapsulamento** | Controlar alterações de estado via construtor e métodos validadores (`private set`). | Garante que nenhuma parte do código coloque o objeto em um estado impossível (ex.: páginas lidas maior que o total). |
| **Polimorfismo** | Chamar o mesmo método por um tipo comum (interface ou classe base) e cada objeto responder à sua maneira. | Permite adicionar novos tipos ao sistema sem reescrever o laço ou o código que os consome. |

---

## 2. Retornar `bool` vs Lançar Exceção (`throw`)

Em `F05`, você usou `throw new ArgumentException(...)` para recusar dados inválidos. Existe uma segunda estratégia muito usada em C# para operações que podem falhar de forma esperada no dia a dia: **retornar `bool`** (`true` se conseguiu aplicar a operação, `false` se recusou mantendo o estado intacto).

- **No Construtor:** como construtores não têm retorno `bool`, **sempre** usamos `throw` (ex.: `ArgumentException` ou `ArgumentOutOfRangeException`) para impedir que um objeto nasça inválido.
- **Em um Método de Ação (como `RegistrarLeitura`):** você pode optar por retornar `bool` para que o chamador decida em um `if (!objeto.RegistrarLeitura(x))` como avisar o usuário, sem precisar de `try/catch`.

> [!NOTE]
> **O que é `nameof(parametro)`?** Em `throw new ArgumentException("Informe o título.", nameof(titulo));`, o operador `nameof(titulo)` gera o texto `"titulo"` automaticamente. **Por que usar `nameof` em vez de escrever `"titulo"` na mão?** Se um dia você renomear o parâmetro no Visual Studio 2022 (`Ctrl+R, R`), o `nameof` é renomeado junto e nunca fica desatualizado!

---

## 3. Estudo de Caso Guiado 1 — Construtor, Referências e Retorno `bool`

Construa no projeto `PooNaPratica` a classe `LivroLeitura.cs` (`Shift+Alt+C`) seguindo as orientações abaixo:

1. **Propriedades:**
   - `Titulo` (`string`, `{ get; }`);
   - `TotalPaginas` (`int`, `{ get; }`);
   - `PaginasLidas` (`int`, `{ get; private set; }`).
2. **Construtor `public LivroLeitura(string titulo, int totalPaginas)`:**
   - Se `string.IsNullOrWhiteSpace(titulo)` → lance `ArgumentException` passando `nameof(titulo)`.
   - Se `totalPaginas <= 0` → lance `ArgumentOutOfRangeException` passando `nameof(totalPaginas)`.
   - Guarde `Titulo = titulo.Trim();` e `TotalPaginas = totalPaginas;`.
3. **Método `public bool RegistrarLeitura(int quantidade)`:**
   - Se `quantidade <= 0` ou `quantidade > TotalPaginas - PaginasLidas` → retorne `false` **sem alterar `PaginasLidas`**.
   - Caso contrário, some `quantidade` em `PaginasLidas` (usando o atalho `PaginasLidas += quantidade;`) e retorne `true`.
4. **Método `public string Resumo()`:**
   - Retorne `$"{Titulo} | {PaginasLidas}/{TotalPaginas} páginas"`.
5. **Testes no `Program.cs`:**
   - Instancie `viagem` (`120` páginas) e `jardim` (`80` páginas).
   - Registre `20` páginas em `viagem` e imprima o resumo dos dois.
   - Crie `LivroLeitura leituraAtual = viagem;`, registre mais `10` páginas por `leituraAtual` e imprima `viagem.Resumo()` (esperado: `30/120`, comprovando referência compartilhada!).
   - Teste `if (!viagem.RegistrarLeitura(100))` e exiba `"Quantidade recusada."`, conferindo que `viagem` continua em `30/120`.
   - Registre mais `90` páginas (chegando a `120/120`) e depois tente registrar mais `1` (deve ser recusada).

---

## 4. Estudo de Caso Guiado 2 — Interface, Herança (`base`) e Polimorfismo

### Como funciona `: base(...)` e `base.Metodo()` em Herança?
Quando uma classe filha (`AvisoUrgente : Aviso`) herda de uma classe pai que exige um parâmetro no construtor (`public Aviso(string titulo)`):
- O construtor da filha repassa esse dado para o construtor da pai usando **`: base(titulo)`**:
  ```csharp
  public AvisoUrgente(string titulo) : base(titulo)
  {
  }
  ```
- Dentro de um método sobrescrito (`override`), você pode reaproveitar o texto gerado pela classe pai chamando **`base.Exibir()`** e acrescentar um prefixo na frente (ex.: `$"URGENTE | {base.Exibir()}"`).

### Sua vez de construir o Painel Polimórfico (em arquivos separados):
1. Crie `IExibivel.cs` com a interface `public interface IExibivel` exigindo `string Exibir();`.
2. Crie `Aviso.cs` implementando `IExibivel`, com propriedade `Titulo { get; }`, construtor exigindo título e método `public virtual string Exibir()` retornando `$"Aviso | {Titulo}"`.
3. Crie `AvisoUrgente.cs` herdando de `Aviso`, repassando o título com `: base(titulo)` e sobrescrevendo `Exibir()` com `override` para retornar `$"URGENTE | {base.Exibir()}"`.
4. Crie `Rodape.cs` implementando `IExibivel` (sem herdar de `Aviso`), retornando uma mensagem fixa de atendimento.
5. Crie também uma quarta classe **`Horario.cs`** implementando `IExibivel` que informe o horário de funcionamento.
6. No `Program.cs`, monte uma coleção `IExibivel[]` ou `List<IExibivel>` contendo instâncias das quatro classes e percorra tudo com **um único `foreach`** chamando `item.Exibir()`. Observe o polimorfismo em ação!

---

## 5. Desafio Independente de Modelagem: `SessaoDeEstudo`

Construa do zero (sem consultar os exemplos anteriores) a classe `SessaoDeEstudo.cs` que controla uma sessão de estudo cronometrada:

- **O que deve guardar:** assunto (`string`), meta em minutos (`int`) e minutos estudados (`int`, começando em `0`).
- **Regras obrigatórias:**
  - Exija assunto preenchido e meta positiva no construtor (lance exceção em caso contrário).
  - Crie um método para registrar minutos estudados que recuse valores `<= 0` ou que ultrapassem o tempo restante da meta, preservando o progresso anterior.
  - Crie um método `Resumo()` devolvendo o texto do progresso.

### Tabela de Verificação (Teste todos os casos no `Program.cs`)

| Caso testado | Resultado esperado |
| --- | --- |
| Criar sessão `"C#"` com meta de `45` minutos | Progresso inicial: `0` |
| Registrar `20` minutos | Aceito; progresso passa para `20` |
| Tentar registrar `26` minutos | Recusado; progresso permanece `20` |
| Registrar `25` minutos | Aceito; progresso chega a `45` |
| Criar uma segunda sessão `"SQL"` com meta `30` | Nova sessão começa independente em `0` |
| Tentar criar sessão com meta `0` ou assunto vazio | Construção recusada por exceção |

Ao concluir, explique em voz alta (ou no caderno): quais propriedades você escolheu abstrair, onde a regra de progresso é protegida e por que as duas sessões mantêm estados independentes.

Continue com [F04 — Classes e objetos](04-Classes-e-Objetos.md), [F05 — Encapsulamento](05-Construtores-e-Encapsulamento.md), [F06 — Composição](06-Composicao-e-Colecoes.md) e [F07 — Interfaces](07-Interfaces-e-Transicao.md), ou avance para a [Aula 03 do Projeto](../03-Orientacao-a-Objetos.md) se já concluiu os fundamentos.
