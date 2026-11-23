# F07 — Namespaces, Interfaces, Polimorfismo e Preparação para o Projeto

**Pré-requisito:** [F06](06-Composicao-e-Colecoes.md).
**Objetivo:** organizar código com `namespace` e `using`, criar contratos com `interface`, injetar dependências pelo construtor, compreender polimorfismo (por interface e por herança) e realizar o diagnóstico final antes do projeto MarketFlow.

Crie no **Visual Studio 2022** um novo **Aplicativo de Console** chamado `FundamentosF07`.

---

## 1. Organizando Nomes com `namespace` e `using`

Quando um programa cresce, podemos ter dezenas de classes (ou duas bibliotecas diferentes com uma classe de mesmo nome, como `Usuario`).

- **`namespace FundamentosF07;`**: coloca "sobrenome" em todas as classes daquele arquivo, organizando-as em um grupo lógico. **Atenção:** criar uma pasta no Visual Studio 2022 ajuda a organizar os arquivos no disco, mas o que define o namespace em C# é a declaração `namespace ...;` escrita no código!
- **`using FundamentosF07;`**: avisa ao compilador *"neste arquivo, quero usar os nomes curtos das classes que moram dentro de `FundamentosF07`"*.
  - **Importante:** `using` apenas traz um namespace que já existe no projeto para o alcance do arquivo; ele **não** baixa nem instala bibliotecas da internet (isso é papel do NuGet, que veremos na Aula 08).

---

## 2. O Que É uma `interface` e Por Que Usamos Contratos?

Imagine uma tomada elétrica na parede: a tomada define um **contrato** (dois ou três pinos e a voltagem). A tomada não sabe se você vai ligar um ventilador, um carregador ou uma geladeira — qualquer aparelho que cumpra o formato do plugue funciona!

Em C#, uma **`interface`** é exatamente esse contrato: ela lista **quais métodos** um objeto precisa oferecer (nome, parâmetros e retorno), **sem escrever o código interno** de como fazer.

### Anatomia de Interface, Implementação e Injeção pelo Construtor (Microexemplo)

Observe este modelo genérico de envio de notificações:

```csharp
// 1. O Contrato (IAlerta.cs)
namespace ExemploNotificacao;

public interface IAlerta
{
    string GerarMensagem(string texto);
}
```

```csharp
// 2. Uma Implementação Concreta (AlertaSimples.cs)
namespace ExemploNotificacao;

public class AlertaSimples : IAlerta
{
    public string GerarMensagem(string texto)
    {
        return texto;
    }
}
```

```csharp
// 3. Uma Classe que RECEBE o contrato pelo construtor (Monitor.cs)
namespace ExemploNotificacao;

public class Monitor
{
    private readonly IAlerta alerta;

    public Monitor(IAlerta alerta)
    {
        if (alerta is null)
            throw new ArgumentException("Informe o alerta.");

        this.alerta = alerta;
    }

    public string Avisar(string texto)
    {
        return alerta.GerarMensagem(texto);
    }
}
```

### Dissecando Cada Detalhe e o "Por Quê?"

| Sintaxe | Significado | Por que usamos dessa forma? |
| --- | --- | --- |
| Prefixo `I` em `IAlerta` | Convenção oficial do C# para interfaces. | Ao ler `IAlerta`, qualquer programador sabe instantaneamente que se trata de um contrato e não de uma classe concreta. |
| `;` no fim de `string GerarMensagem(string texto);` | Assinatura sem chaves `{ }`. | A interface define **o quê** deve existir, nunca **como** executar. Por isso, tentar fazer `new IAlerta()` dá erro de compilação! |
| `: IAlerta` em `class AlertaSimples : IAlerta` | A classe assina o contrato ("implementa `IAlerta`"). | O compilador **obriga** a classe a implementar `GerarMensagem` com assinatura exata. Se esquecer, não compila. |
| `private readonly IAlerta alerta;` | Campo privado e `readonly`. | `readonly` garante que o campo só pode receber atribuição no construtor e nunca mais ser trocado depois. |
| `public Monitor(IAlerta alerta)` | **Injeção de Dependência pelo Construtor.** | Em vez de `Monitor` criar internamente `new AlertaSimples()`, quem usa o `Monitor` entrega o objeto pronto. Assim podemos trocar o comportamento **sem editar uma única linha** de `Monitor`! |

### Onde aparece o Polimorfismo aqui?
**Polimorfismo** ("muitas formas") acontece quando você chama `alerta.GerarMensagem(texto)` usando uma variável do tipo do contrato (`IAlerta`), e o C# executa automaticamente o código da classe concreta real que foi entregue (`AlertaSimples`, `AlertaUrgente`, etc.).

---

## 3. Comparando Interface, Composição e Herança (`virtual` / `override`)

Muitos iniciantes acham que Orientação a Objetos significa usar herança em tudo. Não é verdade! Use este guia de decisão:

| Recurso | Pergunta-chave para escolher | Quando usar |
| --- | --- | --- |
| **Interface** | *"Que operação o chamador precisa poder pedir sem saber os detalhes?"* | Trocar implementações (ex.: repositório em memória vs banco real; formatador simples vs maiúsculo). |
| **Composição** | *"De qual objeto este objeto precisa para trabalhar?"* | Relação **"tem um"** (ex.: `Apresentador` tem um `IFormatador`; `ItemMaterial` tem um `Material`). |
| **Herança de classe** | *"A classe filha **é um** tipo especializado da classe base?"* | Relação **"é um"** genuína, quando faz sentido reaproveitar comportamento da base e especializar partes com `virtual`/`override`. |

### Como funciona Herança com Polimorfismo (`virtual` e `override`)
- Na classe base, marcar um método com **`virtual`** autoriza classes derivadas a reescreverem aquele comportamento.
- Na classe derivada (`class AvisoUrgente : Aviso`), usar **`override`** substitui o método da base pela versão especializada.
- Mesmo guardando o objeto em uma variável do tipo base (`Aviso aviso = new AvisoUrgente();`), chamar `aviso.Texto()` executa a versão `override` de `AvisoUrgente`!
- **Atenção:** membros `private` da classe base continuam invisíveis para a classe derivada — herança não quebra o encapsulamento.

---

## 4. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

### Parte A — Contrato `IFormatador` e Injeção pelo Construtor (no projeto `FundamentosF07`)

Crie cada arquivo separado no Visual Studio 2022 (`Shift+Alt+C`), colocando `namespace FundamentosF07;` no topo de todos eles:

1. **`IFormatador.cs`:**
   - Declare uma `public interface IFormatador` com um único método: `string Formatar(string texto);`.
2. **`FormatadorSimples.cs`:**
   - Crie `public class FormatadorSimples : IFormatador`.
   - Implemente `Formatar` devolvendo o próprio `texto` sem alteração.
3. **`FormatadorMaiusculo.cs`:**
   - Crie `public class FormatadorMaiusculo : IFormatador`.
   - Implemente `Formatar` devolvendo `texto.ToUpperInvariant()` (convertemos com `Invariant` para garantir comportamento padronizado em qualquer idioma do Windows).
4. **`Apresentador.cs`:**
   - Declare `public class Apresentador` com um campo `private readonly IFormatador formatador;`.
   - Crie o construtor recebendo `IFormatador formatador`: valide se `formatador is null` (lançando `ArgumentException`) e atribua com `this.formatador = formatador;`.
   - Crie o método `public string Preparar(string texto)` que delega a tarefa retornando `formatador.Formatar(texto)`.
5. **`Program.cs`:**
   - Adicione `using FundamentosF07;` na primeira linha.
   - Crie dois objetos (`IFormatador simples = new FormatadorSimples();` e `IFormatador maiusculo = new FormatadorMaiusculo();`).
   - Crie dois `Apresentador`, entregando um formatador diferente a cada um, e exiba `Preparar("Bom dia")` de ambos (saída esperada: `Bom dia` e `BOM DIA`).
   - **Depure com `F9` e `F11`:** coloque um breakpoint na chamada do segundo apresentador e acompanhe o salto `Program` → `Apresentador.Preparar` → `FormatadorMaiusculo.Formatar` → volta ao `Program`.
6. **Extensão sem alterar `Apresentador`:**
   - Crie uma terceira classe `FormatadorComPrefixo : IFormatador` que devolve `$"Mensagem: {texto}"`.
   - No `Program.cs`, crie um terceiro `Apresentador` passando o novo formatador. Comprove que `Apresentador.cs` **não precisou ser modificado**!
7. **Teste de contrato do compilador:**
   - Comente temporariamente o método `Formatar` dentro de `FormatadorSimples.cs`, leia o erro na **Lista de Erros** (`CS0535`) confirmando que o compilador fiscaliza o contrato, e restaure o método.

### Parte B — Experimento Isolado de Herança (`FundamentosHeranca`)

Para não misturar os arquivos, crie no Visual Studio 2022 um **segundo Aplicativo de Console** chamado `FundamentosHeranca`:
1. Crie `Aviso.cs` com uma classe `public class Aviso` e um método `public virtual string Texto()` retornando `"Aviso geral"`.
2. Crie `AvisoUrgente.cs` herdando de `Aviso` (`: Aviso`) e sobrescrevendo o método com `public override string Texto()` retornando `"Aviso urgente"`.
3. No `Program.cs`, declare `Aviso aviso = new AvisoUrgente();` e exiba `aviso.Texto()`. Depois troque para `new Aviso()`, preveja a saída e confirme com `Ctrl+F5`.

---

## 5. Diagnóstico Final Antes de Iniciar o Projeto MarketFlow

Antes de avançar para a [Aula 01](../01-Introducao-CSharp.md), confira se você consegue **construir e explicar com suas próprias palavras** cada item abaixo:

| Habilidade essencial | Como comprovar |
| --- | --- |
| Ler, converter e validar entrada do teclado | Recusar `"abc"` e número negativo sem derrubar o programa |
| Escrever e chamar função com retorno | Calcular dois totais com argumentos diferentes sem `Console.WriteLine` dentro da função |
| Distinguir classe, objeto e referência | Desenhar dois objetos na memória e duas variáveis apontando para o mesmo objeto |
| Construir com validação e encapsular | Recusar alteração inválida preservando o estado anterior do objeto |
| Usar composição e `List<T>` | Somar itens de uma lista vazia e de uma lista preenchida |
| Trabalhar com `interface` e polimorfismo | Trocar o formatador sem alterar o `Apresentador` |
| Usar o Depurador do Visual Studio 2022 | Pausar com `F9`, inspecionar variáveis e avançar com `F10` e `F11` |

### A Ponte para o MarketFlow
Nas aulas **01 a 03** do projeto, você aplicará esses mesmos fundamentos ao problema do supermercado. Na **Aula 04**, verá como interfaces separam a interface gráfica, as regras de negócio e o banco de dados. Windows Forms, eventos, `async/await`, hash de senha e SQL serão introduzidos um por vez em seus capítulos próprios.

**Critério para concluir F07:** construir o terceiro formatador (`FormatadorComPrefixo`), rastrear a chamada com `F11` no Visual Studio 2022 e demonstrar todos os itens da tabela de diagnóstico acima.

[Anterior: F06 — Composição e coleções](06-Composicao-e-Colecoes.md) · [Percurso preparatório](README.md) · [Iniciar Aula 01 do Projeto](../01-Introducao-CSharp.md)
