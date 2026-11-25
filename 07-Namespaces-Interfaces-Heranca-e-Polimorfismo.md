# Aula 07 — Namespaces, Interfaces, Herança e Polimorfismo

**Duração:** 3h30. **Pré-requisito:** [Aula 06](06-Composicao-Colecoes-e-Subtotal.md). **Entrega:** projeto organizado em `namespace` demonstrando contratos com `interface`, injeção de dependência pelo construtor (`readonly`), herança com `virtual`/`override`/`: base(...)` e polimorfismo executando comportamentos distintos pela mesma chamada (`A07.01`–`A07.05`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Revisar composição da Aula 06 e o problema de trocar comportamentos |
| Explicação | 30 | `namespace`/`using`, `interface`, `readonly`, injeção pelo construtor, herança e polimorfismo |
| Construção | 60 | Construir `IFormatador`, implementações e `Apresentador` com injeção no VS 2022 |
| Intervalo | 15 | Pausa |
| Prática | 60 | Construir `IPagamento` polimórfico e exercício isolado de herança (`Pessoa`/`Aviso`) |
| Entrega | 30 | Demonstrar troca de implementação sem editar o chamador e diagnóstico final de POO |

---

## 1. Organizando Nomes com `namespace` e `using` (`A07.01`)

A partir desta aula e em todo o projeto MarketFlow, todas as classes pertencem a um **`namespace`**:
- **`namespace Aula07Contratos;`** dá um "sobrenome" ao arquivo para evitar conflitos de nomes quando o sistema cresce. **Atenção:** criar pastas no Visual Studio 2022 organiza os arquivos no disco, mas o que define o namespace em C# é a declaração `namespace ...;` no código!
- **`using Aula07Contratos;`** permite usar os nomes curtos daquele namespace no `Program.cs`. Ele **não** baixa pacotes da internet (isso é feito pelo NuGet na Aula 13).

---

## 2. Interfaces, Injeção pelo Construtor e Polimorfismo (`A07.02` e `A07.03`)

Uma **`interface`** é um **contrato** que define *quais operações* uma classe precisa oferecer, sem dizer *como* executá-las (por isso seus métodos terminam em `;` e não podemos fazer `new IContrato()`). Por convenção C#, toda interface começa com a letra **`I`**.

### Anatomia da Injeção pelo Construtor (Microexemplo: `Monitor.cs`)

```csharp
namespace ExemploNotificacao;

public interface IAlerta { string GerarMensagem(string texto); }

public class Monitor
{
    private readonly IAlerta alerta; // 'readonly': só pode ser atribuído no construtor!

    public Monitor(IAlerta alerta)
    {
        if (alerta is null) throw new ArgumentException("Informe o alerta.");
        this.alerta = alerta; // 'this' diferencia o campo da classe do parâmetro
    }

    public string Avisar(string texto) => alerta.GerarMensagem(texto);
}
```

**Por que `Monitor` recebe `IAlerta` pelo construtor em vez de dar `new AlertaSimples()` lá dentro?**
Porque assim o `Monitor` não fica preso a uma única implementação! Quem cria o `Monitor` (no `Program.cs`) decide qual classe concreta entregar, e quando `Monitor` chama `alerta.GerarMensagem(texto)`, o C# executa o código específico daquele objeto (**Polimorfismo**).

---

## 3. Herança (`virtual`, `override`, `base`) vs Interface vs Composição (`A07.04` e `A07.05`)

Muitos iniciantes forçam herança em tudo (ex.: criar subclasses `Arroz : Produto`, `Carne : Produto`). **Não faça isso!** Use este critério de engenharia:

| Recurso | Pergunta para escolher | Exemplo certo |
| --- | --- | --- |
| **Interface** | *"Que operação o chamador precisa pedir sem depender da implementação?"* | `IPagamento.Processar`, `IFormatador.Formatar`, `IProdutoRepository` |
| **Composição** | *"De qual objeto este objeto precisa ('tem um')?"* | `ItemDeCompra` tem um `Produto`; `Apresentador` recebe um `IFormatador` |
| **Herança (`:`)** | *"A classe derivada **é um** tipo especializado da base?"* | `Cliente : Pessoa`; `AvisoUrgente : Aviso`; `FormLogin : Form` |

### Como funciona Herança com Polimorfismo em C#?
- **`virtual`** na classe base autoriza subclasses a especializarem aquele método.
- **`override`** na subclasse substitui a implementação; **`base.Metodo()`** reaproveita o comportamento da base; **`: base(arg)`** no construtor da subclasse repassa argumentos obrigatórios para o construtor da base.
- Membros **`private`** da base continuam inacessíveis na subclasse (herança não quebra encapsulamento!).

---

## 4. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

Crie um Aplicativo de Console `Aula07Polimorfismo` no **Visual Studio 2022**, colocando `namespace Aula07Polimorfismo;` em todos os arquivos de classe/interface (e `using Aula07Polimorfismo;` no `Program.cs`):

### Parte 1 — Contrato `IFormatador` e Injeção pelo Construtor (`A07.01` e `A07.02`)
1. Crie **`IFormatador.cs`** declarando `public interface IFormatador` com `string Formatar(string texto);`.
2. Crie três implementações em arquivos separados (`: IFormatador`):
   - **`FormatadorSimples.cs`:** devolve `texto` sem alteração;
   - **`FormatadorMaiusculo.cs`:** devolve `texto.ToUpperInvariant()`;
   - **`FormatadorComPrefixo.cs`:** devolve `$"Mensagem: {texto}"`.
3. Crie **`Apresentador.cs`** recebendo `IFormatador formatador` no construtor (validando `is null`, guardando em campo `private readonly` com `this.formatador`) e oferecendo `public string Preparar(string texto)` que chama `formatador.Formatar(texto)`.
4. No `Program.cs`, crie três `Apresentador` (um com cada formatador) e chame `Preparar("Bom dia")` → esperado: `Bom dia`, `BOM DIA` e `Mensagem: Bom dia`, **sem alterar `Apresentador.cs`!**
5. **Depuração (`F11`) e teste de contrato:** entre em `Preparar` com `F11` no VS 2022 acompanhando o salto até `FormatadorMaiusculo`. Comente temporariamente `Formatar` em uma das classes, observe o erro `CS0535` do compilador e restaure.

### Parte 2 — Polimorfismo de Pagamentos Simulados (`A07.03`)
1. Crie **`IPagamento.cs`** com `void Processar(decimal valor);`.
2. Crie **`PagamentoDinheiro.cs`** e **`PagamentoCartao.cs`** implementando `IPagamento` com mensagens simuladas distintas (nenhuma cobrança real).
3. No `Program.cs`, coloque instâncias de ambos dentro de uma lista `List<IPagamento>` e percorra com **um único `foreach`** chamando `pagamento.Processar(60.30m)`.

### Parte 3 — Herança com `virtual`, `override` e `: base(...)` (`A07.04` e `A07.05`)
*(Exercício didático de herança — fora do modelo final de produtos)*
1. Crie **`Pessoa.cs`** (`Nome`, `Cpf` inicializados com `""` e método `ExibirCadastro()`), **`Cliente.cs`** (`: Pessoa`, com `PontosFidelidade`) e **`Funcionario.cs`** (`: Pessoa`, com `SalarioBase`). Instancie ambos no `Program.cs` e execute o método herdado `ExibirCadastro()`.
2. Crie **`Aviso.cs`** (construtor exigindo `string titulo`, propriedade `Titulo { get; }`, método `public virtual string Exibir()` retornando `$"Aviso | {Titulo}"`) e **`AvisoUrgente.cs`** (`: Aviso`, construtor `: base(titulo)`, método `public override string Exibir()` retornando `$"URGENTE | {base.Exibir()}"`).
3. No `Program.cs`, teste `Aviso a = new AvisoUrgente("Fechamento 18h");` e depois troque para `new Aviso("Fechamento 18h");` comprovando o polimorfismo por herança.

---

## 5. Checkpoint de Prontidão Antes de Entrar no Projeto MarketFlow

Com esta aula, você concluiu toda a base de **C# e Orientação a Objetos**! Na próxima aula (**Aula 08**), aplicaremos interfaces e injeção de dependência para estruturar a **Arquitetura em Camadas do MarketFlow**, e na **Aula 09** abriremos o primeiro aplicativo visual **Windows Forms** no Visual Studio 2022.

[Anterior: Aula 06](06-Composicao-Colecoes-e-Subtotal.md) · [Próxima: Aula 08 — Arquitetura em camadas e contratos](08-Arquitetura-e-Padroes.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 07 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-07). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
