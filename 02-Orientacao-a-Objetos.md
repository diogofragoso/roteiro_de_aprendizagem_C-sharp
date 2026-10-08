# 2️⃣ Programação Orientada a Objetos (POO)

A Programação Orientada a Objetos (POO) é o coração do C#. Em vez de escrevermos um amontoado de funções soltas, nós organizamos o código em "Objetos" que representam coisas do mundo real.

No nosso **Sistema de Supermercado**, coisas do mundo real seriam: `Produto`, `Usuario`, `Venda`, `Cliente`.

## Classes e Objetos

- **Classe:** É o "molde" ou a "planta baixa". Ela define quais propriedades e ações algo deve ter.
- **Objeto:** É a "casa construída" a partir da planta. É a instância real em memória.

```csharp
// A Classe (O Molde)
public class Produto 
{
    // Propriedades (Características)
    public int Id { get; set; }
    public string Nome { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }

    // Métodos (Ações/Comportamentos)
    public void AdicionarEstoque(int quantidade) 
    {
        Estoque += quantidade; // Mesmo que: Estoque = Estoque + quantidade;
    }

    public bool TemEstoque() 
    {
        return Estoque > 0;
    }
}
```

Para usar essa classe no nosso sistema:

```csharp
// Criando Objetos (A Instância)
Produto produto1 = new Produto();
produto1.Id = 1;
produto1.Nome = "Café Tradicional";
produto1.Preco = 18.50m;
produto1.Estoque = 50;

// Utilizando um método
produto1.AdicionarEstoque(10); 
// Agora o estoque é 60.
```

## Os 4 Pilares da POO

Para criarmos um software sustentável e profissional, precisamos entender os 4 pilares:

### 1. Encapsulamento
É esconder os detalhes internos e proteger os dados, liberando acesso apenas através de métodos ou propriedades controladas. No C#, fazemos isso com modificadores de acesso (`public`, `private`, `protected`).

```csharp
public class Usuario 
{
    public string Nome { get; set; }
    
    // O set é private! A senha só pode ser definida através de um método específico.
    public string SenhaHash { get; private set; } 

    public void DefinirSenha(string senhaAberta) 
    {
        // Lógica para criptografar a senha antes de salvar
        SenhaHash = Criptografar(senhaAberta);
    }
}
```

### 2. Herança
Permite criar novas classes reaproveitando propriedades e métodos de uma classe existente.

```csharp
public class Pessoa 
{
    public string Nome { get; set; }
    public string Cpf { get; set; }
}

// Cliente herda de Pessoa (Cliente É UMA Pessoa)
public class Cliente : Pessoa 
{
    public string CartaoFidelidade { get; set; }
}

// Funcionario também herda de Pessoa
public class Funcionario : Pessoa 
{
    public decimal Salario { get; set; }
}
```

### 3. Polimorfismo
Permite que objetos de classes diferentes sejam tratados de forma semelhante, ou que métodos com o mesmo nome se comportem de maneira diferente dependendo do objeto.

### 4. Abstração (Interfaces)
Interfaces definem um "contrato". Elas dizem *o que* a classe deve fazer, mas não *como* fazer. Isso será essencial na nossa arquitetura!

```csharp
public interface IRepositorioUsuario 
{
    // Qualquer classe que implementar esta interface OBRIGATORIAMENTE
    // terá que criar estes dois métodos.
    void Salvar(Usuario usuario);
    Usuario BuscarPorEmail(string email);
}
```

➡️ **[Ir para o Módulo 3: Arquitetura e Padrões](./03-Arquitetura-e-Padroes.md)**
