# Aula 04 — Arquitetura e responsabilidades

**Duração:** 3h30. **Pré-requisito:** aula 03. **Entrega:** uma consulta a produtos em memória em que a tela/console chama um serviço, que chama uma interface de repositório.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Identificar cálculo, entrada e armazenamento no console |
| Explicação | 30 | Camadas, contratos e composição |
| Construção | 60 | Separar produto, contrato e repositório em memória |
| Intervalo | 15 | Pausa |
| Prática | 60 | Colocar uma regra no serviço e chamar pelo console |
| Entrega | 30 | Trocar a implementação e explicar o fluxo |

## 1. Parta de um problema concreto

Abra o console anterior. Imagine que a mesma regra de preço positivo será usada no cadastro, na importação e numa futura API. Se a regra estiver no clique de um botão, cada entrada precisará copiá-la. Vamos colocá-la num serviço, independente do controle visual.

Nossa arquitetura é uma **separação lógica em um projeto**. Pastas ajudam a localizar código, mas não impedem dependências indevidas; cabe à equipe respeitar o sentido das chamadas. Não chamaremos essa organização de uma implementação completa de Clean Architecture.

```text
Views → Services → interfaces de Repositories
                      ↑
             implementações Npgsql → Neon
Models são usados nos contratos e serviços.
Program cria os objetos e conecta as dependências.
```

| Pasta | Responsabilidade | Exemplo |
| --- | --- | --- |
| Models | Dados e conceitos do supermercado | Produto, Sessao, ItemCarrinho |
| Services | Validações, autorização e casos de uso | ProdutoService |
| Repositories | Consultas e persistência; nenhum MessageBox | ProdutoRepository |
| Infrastructure | Configuração técnica | Banco |
| Views | Ler controles, chamar serviço e apresentar resultado | FormProdutos |

## 2. Faça um contrato pequeno

No console `TreinoSupermercado`, adicione uma classe `Produto` (se ainda não a criou) com propriedades `Nome` e `Preco`. Depois adicione o arquivo `IConsultaProdutos.cs`:

```csharp
public interface IConsultaProdutos
{
    IReadOnlyList<Produto> Listar();
}
```

`IReadOnlyList` permite consultar a lista sem oferecer `Add` nesse contrato. Uma interface descreve **o que** podemos pedir; não decide **como** buscar.

Adicione `ProdutosEmMemoria.cs`:

```csharp
public class ProdutosEmMemoria : IConsultaProdutos
{
    public IReadOnlyList<Produto> Listar() => new List<Produto>
    {
        new Produto { Nome = "Arroz", Preco = 25.90m },
        new Produto { Nome = "Feijão", Preco = 8.50m }
    };
}
```

Esse exemplo cria a lista em cada consulta. O banco virá depois; hoje verificamos a organização sem depender de internet.

## 3. Receba a dependência pelo construtor

Adicione `ConsultaProdutosService.cs`:

```csharp
public class ConsultaProdutosService
{
    private readonly IConsultaProdutos produtos;
    public ConsultaProdutosService(IConsultaProdutos produtos)
    {
        this.produtos = produtos;
    }
    public IReadOnlyList<Produto> Executar() => produtos.Listar();
    public void ValidarPreco(decimal preco)
    {
        if (preco <= 0) throw new ArgumentException("Preço deve ser positivo.");
    }
}
```

No `Program.cs`, deixe apenas as instruções de execução; as classes ficam em seus próprios arquivos:

```csharp
IConsultaProdutos repositorio = new ProdutosEmMemoria();
var servico = new ConsultaProdutosService(repositorio);
foreach (var produto in servico.Executar())
    Console.WriteLine($"{produto.Nome}: {produto.Preco:C}");
try { servico.ValidarPreco(0); }
catch (ArgumentException erro) { Console.WriteLine(erro.Message); }
```

Isso é **injeção de dependência manual**: quem cria o serviço entrega o objeto de que ele precisa. Não precisamos de uma biblioteca de injeção para aprender o conceito.

## 4. Entenda o alcance das escolhas

Antes de continuar, crie uma segunda implementação do contrato:

```csharp
public class ProdutosPromocionais : IConsultaProdutos
{
    public IReadOnlyList<Produto> Listar() => new List<Produto>
    {
        new Produto { Nome = "Café", Preco = 12.90m }
    };
}
```

No Program, substitua apenas `new ProdutosEmMemoria()` por `new ProdutosPromocionais()`. O serviço deve continuar igual e a saída deve mudar. Essa é a evidência da substituição da dependência; alterar o preço dentro da mesma lista não demonstra a troca de implementação.

`ValidarPreco` está separado da consulta para exercitar a localização de uma regra sem interface gráfica. A partir da aula 11, a validação fará parte do próprio caso de uso `SalvarAsync`, antes da gravação. No `try`, execute a operação; no `catch`, apresente a recusa. O código depois do `throw` não continua no mesmo caminho.

O Repository concentra o SQL, mas trocar PostgreSQL por outro banco pode exigir também mudar transações, tipos e migrações. A interface reduz o impacto; não torna bancos intercambiáveis automaticamente.

No aplicativo final, o serviço decide se o usuário pode salvar e se o preço é válido. O repositório executa o SQL. Na venda, o repositório mantém os comandos relacionados numa mesma transação: dividir cada comando em uma conexão diferente destruiria essa garantia.

A conexão direta do desktop ao Neon serve ao laboratório. Um usuário com acesso à máquina pode obter a credencial de banco ou modificar uma sessão em memória. Em uma implantação comercial, coloque as regras de autorização numa API autenticada; o desktop não deve receber a credencial compartilhada do banco.

## Verifique antes de encerrar

- Troque o preço da lista em memória e mostre a mudança no console.
- Chame `ValidarPreco(-1)` sem nenhuma tela: a regra continua funcionando.
- Explique por que um repositório não deve mostrar `MessageBox`.
- Registre no caderno quais responsabilidades mudarão quando adicionarmos o Neon.

[Anterior: aula 03](03-Orientacao-a-Objetos.md) · [Próxima: aula 05](05-Configuracao-Ambiente.md)

## Atividades por conceito

Realize as atividades da [aula 04 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-04). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
