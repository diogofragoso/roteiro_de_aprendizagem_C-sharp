# Aula 03 — Orientação a objetos

**Duração:** 3h30. **Pré-requisito:** Aula 02. **Entrega:** Classes Produto e ItemDeCompra instanciadas, com subtotal calculado.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Contexto e problema do supermercado |
| Explicação | 30 | Conceitos dos exemplos abaixo |
| Construção | 60 | Criar e comparar dois produtos |
| Intervalo | 15 | Pausa |
| Prática | 60 | Implementar subtotal e percorrer objetos |
| Entrega | 30 | Demonstrar casos normal/inválido e registrar evidências |

A Programação Orientada a Objetos (POO) não é apenas uma funcionalidade da linguagem; é uma **forma de pensar**. No início da programação de computadores, o código era uma lista gigante de comandos descendo a tela (programação procedural). Com a POO, nós modelamos o nosso software para se parecer com o mundo real.

No nosso **Sistema de Supermercado**, coisas do mundo real se transformam em partes do código: `Produto`, `Usuario`, `CarrinhoDeCompra`, `Pagamento`.

## 1. Classes vs Objetos

Esses são os dois conceitos que mais confundem iniciantes, mas a diferença é simples:
- **Classe:** É o "Molde" (a teoria). É a planta baixa de uma casa. Ela define quais características algo *vai ter*, mas não possui dados reais.
- **Objeto (Instância):** É o "Produto final" (a prática). É a casa já construída com base na planta. Ele ocupa espaço na memória e contém os dados reais de um caso específico.

```csharp
// A CLASSE (O Molde)
public class Produto
{
    // Atributos / Propriedades
    public string Nome { get; set; }
    public decimal Preco { get; set; }
}

// ... em outro lugar do código ...

// OS OBJETOS (A Prática em memória)
Produto prod1 = new Produto(); // 'new' cria a casa a partir da planta!
prod1.Nome = "Café";
prod1.Preco = 18.50m;

Produto prod2 = new Produto(); // Outro objeto, totalmente independente
prod2.Nome = "Açúcar";
prod2.Preco = 4.00m;
```

---

## 2. Os 4 Pilares da Orientação a Objetos

Todo sistema profissional (especialmente em .NET) se baseia nestes quatro conceitos. Ignorá-los resulta em códigos difíceis de dar manutenção.

### Pilar 1: Abstração
É a capacidade de ignorar detalhes que não importam e focar apenas no essencial para o seu sistema.
- *Exemplo:* Um `Funcionario` humano possui tipo sanguíneo, altura e cor dos olhos. Mas para o Sistema de Supermercado, abstraímos isso! Nos importam apenas: `Nome`, `CPF`, `Cargo` e `DataAdmissao`.

### Pilar 2: Encapsulamento (Proteção)
É a prática de esconder os detalhes internos da classe e proteger os dados contra mudanças indevidas de fora. Usamos os modificadores de acesso:
- `public`: O membro pode ser acessado externamente; a possibilidade de alteração depende também do setter.
- `private`: Só a própria classe pode ver e alterar.

```csharp
public class Usuario
{
    public string Username { get; private set; } = "";
    public void Renomear(string novoNome)
    {
        if (string.IsNullOrWhiteSpace(novoNome))
            throw new ArgumentException("O nome é obrigatório.");
        Username = novoNome.Trim();
    }
}
```

### Pilar 3: Herança (Reaproveitamento)
Permite que uma classe "filha" herde todas as características da classe "pai", evitando repetição de código.

```csharp
// Classe Pai (Base)
public class Pessoa
{
    public string Nome { get; set; }
    public string Cpf { get; set; }

    public void ExibirCadastro()
    {
        Console.WriteLine($"Nome: {Nome} | CPF: {Cpf}");
    }
}

// Cliente herda de Pessoa (Usa-se os dois pontos :)
// Cliente ganha Nome, Cpf e ExibirCadastro automaticamente!
public class Cliente : Pessoa
{
    public int PontosFidelidade { get; set; }
}

// Funcionario também herda de Pessoa
public class Funcionario : Pessoa
{
    public decimal SalarioBase { get; set; }
}
```

### Pilar 4: Polimorfismo ("Muitas Formas")
Permite que classes diferentes tenham o mesmo nome de método, mas executem lógicas diferentes de acordo com a sua classe. Muito usado com **Interfaces** (contratos).

```csharp
// A Interface é um CONTRATO. Ela obriga quem herdar a ter o método "Processar"
public interface IPagamento
{
    void Processar(decimal valor);
}

// Classe que cumpre o contrato do jeito do Dinheiro
public class PagamentoDinheiro : IPagamento
{
    public void Processar(decimal valor)
    {
        Console.WriteLine($"Abrindo gaveta, esperando R$ {valor}...");
    }
}

// Classe que cumpre o contrato do jeito do Cartão
public class PagamentoCartao : IPagamento
{
    public void Processar(decimal valor)
    {
        Console.WriteLine($"Conectando Cielo... Debitando R$ {valor} do cartão.");
    }
}
```
> [!TIP]
> A vantagem do Polimorfismo é que a tela do Caixa não precisa saber **como** funciona o Cartão ou o Dinheiro. A tela apenas manda o comando genérico `pagamento.Processar(valor)` e cada objeto se vira para executar sua regra específica!

---

## 📝 Atividades de Fixação

Teste sua compreensão sobre POO com os exercícios abaixo:

- [ ] **Exercício 1:** Explique a diferença entre `Classe` e `Objeto` usando uma analogia diferente da "planta baixa" usada na aula.
<details>
<summary><b>💡 Ver Resposta</b></summary>
<br>
<i>Analogia da Receita de Bolo:</i> A <b>Classe</b> é a receita escrita no papel (diz quais ingredientes vão e como fazer, mas você não pode comê-la). O <b>Objeto</b> é o bolo físico e assado saindo do forno (a instância real e utilizável baseada na receita).
</details>

- [ ] **Exercício 2:** Qual dos 4 pilares da POO está sendo aplicado quando criamos atributos `private` e exigimos que eles sejam alterados apenas através de métodos de validação?
<details>
<summary><b>💡 Ver Resposta</b></summary>
<br>
O <b>Encapsulamento</b>. Estamos encapsulando (escondendo e protegendo) o estado interno do objeto para que ele não seja corrompido ou alterado indevidamente por outras partes do código.
</details>

- [ ] **Exercício 3:** Crie mentalmente (ou escreva) uma classe pai chamada `ProdutoGeral` e duas classes filhas que herdam dela, representando produtos específicos de um supermercado que tenham características próprias.
<details>
<summary><b>💡 Ver Resposta</b></summary>
<br>
<pre><code>
public class ProdutoGeral
{
    public string Nome { get; set; }
    public decimal Preco { get; set; }
}

public class Carne : ProdutoGeral
{
    public int DiasValidade { get; set; }
    public string TipoCorte { get; set; } // Picanha, Alcatra
}

public class Eletrodomestico : ProdutoGeral
{
    public int MesesGarantia { get; set; }
    public string Voltagem { get; set; } // 110v, 220v
}
</code></pre>
</details>


## Entrega prática: objetos e subtotal

Crie as classes em arquivos separados. No console, mantenha apenas instruções de execução: não coloque instruções soltas depois da declaração de uma classe no mesmo `Program.cs`.

```csharp
public class ItemDeCompra
{
    public Produto Produto { get; set; } = new Produto();
    public int Quantidade { get; set; }
    public decimal Subtotal => Produto.Preco * Quantidade;
}
```

Instancie arroz de R$ 25,90 e feijão de R$ 8,50. Crie uma `List<ItemDeCompra>` com duas unidades de arroz e uma de feijão; percorra a lista com `foreach` e some os subtotais. Resultado: R$ 60,30. Mude apenas o preço do feijão e confira que o arroz permanece igual. Chame `Renomear("")` e observe a validação. Na arquitetura final, regras de acesso ficarão em serviços; esse exemplo serve para compreender encapsulamento.

Hash de senha será ensinado na aula 09 com uma biblioteca pronta. MD5 não é uma solução adequada para senhas. Herança é um recurso a conhecer; não precisamos criar uma subclasse para cada produto do supermercado.

[Próxima aula](04-Arquitetura-e-Padroes.md) · [Índice](README.md)
