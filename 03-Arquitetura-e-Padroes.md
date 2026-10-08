# 3️⃣ Arquitetura e Padrões de Projeto

Em projetos pequenos, é muito comum desenvolvedores iniciantes colocarem todo o código (regras de negócio, conexão com banco e manipulação da tela) diretamente dentro dos botões (ex: no `btnSalvar_Click`). 

**Isso é uma péssima prática!** O código fica bagunçado, impossível de testar e difícil de dar manutenção.

Para o nosso Sistema de Supermercado, utilizaremos uma separação em camadas.

## Arquitetura em Camadas (N-Tier)

Vamos separar nosso código em responsabilidades distintas. A estrutura ideal (e simplificada) será:

1. **Camada de Apresentação (UI - User Interface):** 
   - São as telas em Windows Forms (`.cs` e `.Designer.cs`). 
   - *Regra de ouro:* O Form **não sabe** como salvar no banco de dados. Ele só captura o que o usuário digitou e mostra as respostas.

2. **Camada de Domínio / Modelos (Models):**
   - São as classes puras do nosso sistema (ex: `Usuario.cs`, `Produto.cs`). Elas representam os dados e as regras essenciais.

3. **Camada de Acesso a Dados (Data Access Layer - Repositories):**
   - É aqui que ficam os códigos de banco de dados (os comandos `INSERT`, `SELECT`, conexão via Npgsql, etc).
   - O Form chama o Repositório, e o Repositório fala com o Neon Postgres.

## O Padrão Repository (Repositório)

O padrão de projeto *Repository* serve para abstrair a forma como acessamos os dados. Ele atua como uma coleção em memória para a camada que o está chamando.

### Por que usá-lo?
- Se um dia você quiser mudar o banco de dados de Postgres para SQL Server, você só altera o repositório. As telas (Forms) continuam intactas.
- Facilita muito a manutenção. Toda a lógica de banco está em um só lugar.

### Como funciona na prática?

Criamos uma Interface que define o contrato:

```csharp
public interface IUsuarioRepository
{
    Usuario Autenticar(string username, string senha);
    void Adicionar(Usuario usuario);
}
```

E depois criamos a classe concreta que fala com o banco:

```csharp
public class UsuarioRepository : IUsuarioRepository
{
    public Usuario Autenticar(string username, string senha)
    {
        // Aqui vai o código real do Npgsql (Postgres)
        // que abre a conexão e faz o "SELECT * FROM Usuarios WHERE..."
    }
    
    // ...
}
```

## Resumo do Fluxo do nosso Sistema

1. O usuário digita "admin" e "123" na tela (`FormLogin`).
2. O `FormLogin` chama `usuarioRepository.Autenticar("admin", "123")`.
3. O `UsuarioRepository` abre conexão com o Neon PostgreSQL, executa o SELECT e verifica.
4. O `UsuarioRepository` retorna um objeto `Usuario` (se achar) ou `null` (se errar a senha).
5. O `FormLogin` recebe o resultado e mostra a tela principal ou uma mensagem de "Senha incorreta".

Com essa base teórica sólida, estamos prontos para configurar nosso ambiente de desenvolvimento!

➡️ **[Ir para o Módulo 4: Configuração do Ambiente](./04-Configuracao-Ambiente.md)**
