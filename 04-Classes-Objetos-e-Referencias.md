# Aula 04 — Classes, Objetos, Propriedades e Referências na Memória

**Duração:** 3h30. **Pré-requisito:** [Aula 03](03-Metodos-Parametros-e-Escopo.md). **Entrega:** classes criadas em arquivos `.cs` separados no Visual Studio 2022 (`Produto.cs` e `Livro.cs`), instanciação de objetos independentes com `new` e demonstração prática/desenho de referência compartilhada na memória RAM (`A04.01`–`A04.04`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Revisar variáveis soltas e o limite de representar vários produtos assim |
| Explicação | 30 | Abstração, Classe vs Objeto, propriedades `{ get; set; } = ""` e referências na RAM |
| Construção | 60 | Criar `Livro.cs` e `Produto.cs` em arquivos separados no VS 2022 (`Shift+Alt+C`) |
| Intervalo | 15 | Pausa |
| Prática | 60 | Instanciar objetos independentes e experimentar referências compartilhadas |
| Entrega | 30 | Demonstrar código, desenho de memória (variáveis vs objetos) e registrar evidências |

---

## 1. Por Que Criar Classes? (Abstração, Molde e Instâncias)

Se um supermercado tivesse que guardar 50 produtos usando variáveis soltas (`nome1`, `preco1`, `nome2`, `preco2`...), o código ficaria impossível de organizar.

- **Abstração:** é escolher apenas os dados e comportamentos que importam para o nosso sistema (um produto real tem cor da caixa e peso da embalagem, mas para o cálculo de compra abstraímos apenas `Nome` e `Preco`).
- **Classe:** é o **molde** (a planta baixa ou receita escrita no código). Define quais propriedades e métodos aquele novo tipo possui.
- **Objeto (Instância):** é cada exemplar real criado na memória RAM usando **`new`**. Cada instância guarda seu próprio **estado** (seus valores atuais).

---

## 2. Criando Classes em Arquivos Separados no Visual Studio 2022

**Por que criar um arquivo `.cs` para cada classe?**
Mantém o projeto organizado (a classe `Produto` mora em `Produto.cs`) e evita o erro `CS8803` de misturar instruções de nível superior abaixo de declarações de classe no `Program.cs`.

### Passo a passo no Visual Studio 2022:
1. No **Gerenciador de Soluções**, clique com o botão direito sobre o projeto → **Adicionar → Classe...** (atalho: **`Shift+Alt+C`**).
2. Digite o nome do arquivo (ex.: `Produto.cs`) e clique em **Adicionar**.
3. *(Nas Aulas 04 a 06, remova temporariamente o bloco `namespace` gerado automaticamente para focar na anatomia da classe; trabalharemos `namespace` e `using` em profundidade na Aula 07.)*

### Anatomia de uma Classe (Microexemplo: `Filme.cs`)

```csharp
public class Filme
{
    public string Titulo { get; set; } = "";
    public int DuracaoMinutos { get; set; }

    public string Descrever()
    {
        return $"{Titulo}: {DuracaoMinutos} min";
    }
}
```

| Elemento | Significado | Por que escrevemos assim? |
| --- | --- | --- |
| `public class Filme` | Declara o novo tipo acessível ao resto do projeto. | Sem `public`, outras partes do código podem não enxergar a classe (`CS0122`). |
| `{ get; set; } = "";` | **Propriedade automática** de leitura e escrita inicializada com texto vazio. | Com `<Nullable>enable</Nullable>` no .NET moderno, deixar uma propriedade `string` sem `= ""` nem construtor dispara o aviso `CS8618`. |
| `public string Descrever()` | **Método de instância.** | Lê as propriedades do **próprio objeto** que recebeu a chamada (`primeiro.Descrever()`). |
| `new Filme()` | Operador de instanciação. | Cada chamada a `new` cria um objeto novo e independente na memória RAM. |

---

## 3. O Conceito Mais Importante de POO: Tipo de Valor vs Tipo de Referência

Em `Aula 03`, você viu que copiar um `int` cria uma **cópia independente**. Com **classes**, o comportamento na memória RAM é totalmente diferente!

Uma variável do tipo `Filme` **não guarda o objeto dentro dela** — ela guarda uma **referência (um endereço/seta)** apontando para onde o objeto está alocado na memória:

```csharp
Filme primeiro = new Filme(); // Cria Objeto #1
primeiro.Titulo = "Aventura";
primeiro.DuracaoMinutos = 110;

Filme outroNome = primeiro;   // NÃO tem 'new'! Copia apenas a SETA para o Objeto #1!
outroNome.DuracaoMinutos = 130;
// Agora tanto primeiro.DuracaoMinutos quanto outroNome.DuracaoMinutos valem 130!
```

```text
primeiro  ────┐
              ├──> [ Objeto #1: Titulo="Aventura", DuracaoMinutos=130 ]
outroNome ────┘

segundo   ───────> [ Objeto #2: Titulo="Comédia", DuracaoMinutos=95 ]
```

**Por que entender isso agora?** Na Aula 06 e no MarketFlow, vários itens podem referenciar o mesmo `Produto` na memória. Saber distinguir *"criei outro objeto com `new`"* de *"apontei duas variáveis para o mesmo objeto"* evita bugs misteriosos!

---

## 4. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

Crie um Aplicativo de Console `Aula04Classes` (ou continue no projeto de console) no **Visual Studio 2022**:

1. **Abstração no caderno (`A04.01`):** escolha 4 dados úteis para representar um produto no supermercado e 2 características reais que você decidiu descartar por não servirem ao sistema, justificando cada escolha.
2. **Construir `Produto.cs` e objetos independentes (`A04.02`):**
   - Adicione `Produto.cs` (`Shift+Alt+C`) declarando `public class Produto` com propriedades `string Nome { get; set; } = "";`, `decimal Preco { get; set; }` e um método `public string Resumo()` retornando nome e preço formatado com `:C`.
   - No `Program.cs`, instancie `arroz` (`"Arroz"`, `25.90m`) e `feijao` (`"Feijão"`, `8.50m`) com `new Produto()`.
   - Exiba o resumo de ambos, altere **apenas** `feijao.Preco = 9.00m` e comprove que `arroz.Preco` permaneceu `25,90`.
3. **Experimento de referência compartilhada com `Livro.cs` (`A04.03` e `A04.04`):**
   - Adicione `Livro.cs` com propriedades `Titulo` (`= ""`), `Paginas` (`int`) e método `Descrever()`.
   - No `Program.cs`, crie `primeiro` (`"Viagem"`, `120`) e `segundo` (`"Jardim"`, `80`).
   - Crie `Livro outroNome = primeiro;` (sem `new`), altere `outroNome.Paginas = 150;` e exiba as páginas de `primeiro`, `outroNome` e `segundo` (saída esperada: `150`, `150` e `80`).
   - Desenhe no caderno o diagrama de memória identificando **quantas variáveis de referência existem (3)** e **quantos objetos reais foram criados (2)**, apontando no seu código uma classe, uma propriedade, uma instanciação (`new`), um método e o estado de um objeto.

---

## 5. Erros Comuns no Visual Studio 2022

| Erro na Lista de Erros | Causa provável | Como resolver |
| --- | --- | --- |
| `CS8618: Propriedade não anulável precisa conter valor não nulo` | Declarou `public string Nome { get; set; }` sem `= ""`. | Inicialize com `= "";` (ou use construtor na Aula 05). |
| `CS8803: Instruções de nível superior devem preceder declarações de tipo` | Colocou código de execução após uma classe no `Program.cs`. | Mova cada classe para seu próprio arquivo `.cs`. |
| `CS0122: Tipo inacessível devido ao nível de proteção` | Faltou `public` na classe ou propriedade. | Adicione `public`. |

[Anterior: Aula 03](03-Metodos-Parametros-e-Escopo.md) · [Próxima: Aula 05 — Construtores, encapsulamento e exceções](05-Construtores-Encapsulamento-e-Excecoes.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 04 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-04). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
