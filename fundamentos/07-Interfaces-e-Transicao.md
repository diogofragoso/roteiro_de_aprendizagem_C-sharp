# F07 — Contratos, polimorfismo e preparação para o projeto

**Pré-requisito:** F06. **Objetivo:** fazer o mesmo chamador trabalhar com comportamentos diferentes e explicar como os objetos são conectados.

Crie o console `FundamentosF07`. Use arquivos separados e o namespace `FundamentosF07` em todos os tipos desta página. Namespace organiza nomes; uma pasta sozinha não define o namespace.

## 1. Parta de uma mudança pequena

Queremos receber uma frase e apresentá-la de duas maneiras: como foi escrita ou em letras maiúsculas. O código que pede a formatação pode fazer o mesmo pedido, independentemente da forma escolhida.

Uma interface define as operações de um contrato. Neste exercício, o contrato exige uma operação que recebe texto e devolve texto. Não vamos conectar banco nem criar repositórios ainda.

Em `IFormatador.cs`:

```csharp
namespace FundamentosF07;

public interface IFormatador
{
    string Formatar(string texto);
}
```

O prefixo `I` é uma convenção de nomes de interfaces. No nosso contrato, o ponto e vírgula encerra a assinatura; não há corpo de implementação. Não podemos criar uma instância com `new IFormatador()`.

## 2. Implemente o contrato de duas formas

Em `FormatadorSimples.cs`:

```csharp
namespace FundamentosF07;

public class FormatadorSimples : IFormatador
{
    public string Formatar(string texto)
    {
        return texto;
    }
}
```

Em `FormatadorMaiusculo.cs`:

```csharp
namespace FundamentosF07;

public class FormatadorMaiusculo : IFormatador
{
    public string Formatar(string texto)
    {
        return texto.ToUpperInvariant();
    }
}
```

Os dois-pontos indicam aqui que a classe implementa `IFormatador`: ela precisa cumprir a assinatura. Ter métodos com o mesmo nome em duas classes, por si só, não explica o mecanismo; a referência pelo contrato comum permite fazer a mesma chamada sem escolher uma classe concreta no chamador.

## 3. Faça um objeto receber sua dependência

Em `Apresentador.cs`:

```csharp
namespace FundamentosF07;

public class Apresentador
{
    private readonly IFormatador formatador;

    public Apresentador(IFormatador formatador)
    {
        if (formatador is null)
            throw new ArgumentException("Informe o formatador.");

        this.formatador = formatador;
    }

    public string Preparar(string texto)
    {
        return formatador.Formatar(texto);
    }
}
```

`this.formatador` é o campo da instância; `formatador`, sem `this` no construtor, identifica o parâmetro de mesmo nome. `readonly` impede reatribuir esse campo depois da construção; não torna automaticamente imutável o objeto referenciado.

O `Apresentador` recebe o objeto de que precisa em vez de criar internamente um `FormatadorSimples`. Isso é injeção pelo construtor. Estamos apenas passando um objeto como argumento, como em F06; não precisamos de uma biblioteca para fazer isso.

## 4. Faça a troca no ponto de criação

Em `Program.cs`:

```csharp
using FundamentosF07;

IFormatador simples = new FormatadorSimples();
IFormatador maiusculo = new FormatadorMaiusculo();

Apresentador primeiro = new Apresentador(simples);
Apresentador segundo = new Apresentador(maiusculo);

Console.WriteLine(primeiro.Preparar("Bom dia"));
Console.WriteLine(segundo.Preparar("Bom dia"));
```

Esperado: `Bom dia` e `BOM DIA`. `using FundamentosF07;` permite usar os nomes declarados naquele namespace. Ele não instala um pacote NuGet.

Os dois apresentadores executam o mesmo método `Preparar`. A implementação do formatador recebido determina a resposta de `Formatar`: esse é o polimorfismo demonstrado aqui.

Rastreie a segunda chamada no depurador:

```text
Program chama segundo.Preparar("Bom dia")
Apresentador pede formatador.Formatar("Bom dia")
FormatadorMaiusculo devolve "BOM DIA"
Apresentador devolve esse texto
Program mostra o resultado
```

## 5. Diferencie interface, herança e composição

| Recurso | Pergunta que ajuda a escolher | Exemplo |
| --- | --- | --- |
| Interface | Que operação o chamador precisa poder pedir? | `IFormatador.Formatar` |
| Composição | De qual objeto este objeto precisa? | `Apresentador` recebe um formatador |
| Herança de classe | O tipo derivado pode ser usado como o tipo base, respeitando seu comportamento? | Um tipo de aviso pode especializar uma base de aviso |

Herança de classe usa uma classe base. Para observar o mecanismo sem misturar os arquivos anteriores, crie **outro console**, `FundamentosHeranca`. Em `Aviso.cs`:

```csharp
public class Aviso
{
    public virtual string Texto()
    {
        return "Aviso geral";
    }
}
```

Em `AvisoUrgente.cs`:

```csharp
public class AvisoUrgente : Aviso
{
    public override string Texto()
    {
        return "Aviso urgente";
    }
}
```

Em `Program.cs` desse novo console:

```csharp
Aviso aviso = new AvisoUrgente();
Console.WriteLine(aviso.Texto());
```

`virtual` permite especializar esse método da base; `override` fornece a implementação na classe derivada. Mesmo usando a variável de tipo `Aviso`, a saída é `Aviso urgente`. Esse é outro exemplo de polimorfismo. Membros privados da base continuam privados; herdar não libera acesso direto a eles.

Conhecer herança não significa usá-la em toda classe. Nos materiais de F06, “um item tem um material” descreve a relação; dizer “um item é um material” mudaria o significado. Na primeira aplicação, composição e interfaces serão suficientes em muitos pontos.

## 6. Pratique e faça o diagnóstico final

1. No projeto F07, adicione `FormatadorComPrefixo : IFormatador`, cuja saída para `"Bom dia"` seja `"Mensagem: Bom dia"`. Crie outro apresentador sem mudar seu método `Preparar`.
2. Explique a diferença entre o tipo declarado `IFormatador` e a classe do objeto criado com `new FormatadorMaiusculo()`.
3. Remova temporariamente `Formatar` de uma implementação, leia a falha de compilação e restaure o método.
4. Explique por que `using` não substitui instalar uma biblioteca e por que passar um argumento não exige um contêiner de dependências.
5. No exemplo de herança, preveja a saída ao trocar `new AvisoUrgente()` por `new Aviso()` e confirme.

Antes do projeto, cada estudante deve demonstrar:

| Consigo fazer e explicar | Evidência sugerida |
| --- | --- |
| Ler, converter e validar entrada | Recusar `abc` e quantidade negativa |
| Escrever e chamar uma função com retorno | Calcular dois totais com argumentos diferentes |
| Distinguir classe, objeto e referência | Desenhar dois objetos e uma referência compartilhada |
| Usar construtor e preservar uma regra | Recusar leitura acima do total sem alterar o estado |
| Percorrer uma lista de objetos relacionados | Calcular materiais e explicar o subtotal |
| Usar um contrato com duas implementações | Trocar o formatador sem alterar o apresentador |
| Acompanhar chamadas e localizar erros | Demonstrar uma passagem no depurador |

Se algum item estiver pendente, retome o capítulo correspondente com uma entrada diferente. A cópia de um exemplo completo não substitui essa demonstração.

## 7. Faça a ponte para o MarketFlow

No projeto, `Produto` e `ItemCarrinho` serão novos modelos com regras próprias. Não substitua suas implementações pelos exercícios daqui. A responsabilidade de `Program` em construir e conectar objetos volta a aparecer; interfaces permitirão ao serviço pedir consultas sem conhecer seus detalhes técnicos.

As aulas 01–03 retomam os fundamentos com o problema do supermercado. Na aula 04 você aprenderá a separar apresentação, regras e armazenamento. Windows Forms, eventos, `Task`, `async/await`, hash e SQL entram em seus capítulos próprios; não são pré-requisitos ocultos deste percurso.

**Critério para concluir:** implementar o terceiro formatador, rastrear a chamada e demonstrar os itens do diagnóstico com uma variação escolhida pelo professor.

[Anterior: composição](06-Composicao-e-Colecoes.md) · [Percurso preparatório](README.md) · [Iniciar a aplicação dos fundamentos](../01-Introducao-CSharp.md)
