# Aula 14 — Autenticação Segura com Hash, Salt, `record`, `enum` e SQL Parametrizado

**Duração:** 3h30. **Pré-requisito:** [Aula 13](13-Conectando-Banco-Dados.md), tabela `usuario` vazia e `SELECT 1` funcionando. **Entrega:** primeiro administrador cadastrado com hash+salt via `--preparar-admin` e `FormLogin` autenticando credenciais reais por `AutenticacaoService` e `UsuarioRepository`, sem jamais expor senhas ou hashes para as telas (`A14.01`–`A14.08`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Conferir `SELECT 1` e planejar a remoção do acesso direto ao banco na tela |
| Explicação | 30 | `enum`, `record`, `sealed`, construtor primário, `Task<T?>`, SQL parametrizado e Hash+Salt |
| Construção | 60 | Paradas 1 e 2: construir modelos e repositório SQL parametrizado de usuários |
| Intervalo | 15 | Pausa |
| Prática | 60 | Paradas 3 e 4: instalar `Identity.Core`, construir `AutenticacaoService`, `Ui` e ligar `FormLogin` |
| Entrega | 30 | Preparar 1º admin (`--preparar-admin`), testar login, normalização e conferir no SQL Editor |

---

## 1. Novos Recursos de C# e Segurança (Com Exemplos Práticos Antes do Projeto)

### 1.1 `enum`, `record`, `sealed` e Construtor Primário (`A14.01` e `A14.03`)
- **`enum`:** conjunto fechado de opções nomeadas (impede erros de digitação como `"Admnistrador"`).
- **`record`:** sintaxe moderna para criar classes imutáveis focadas em transportar dados em **uma única linha**!
- **`sealed`:** impede que outra classe herde dela *(atenção: `sealed` bloqueia herança, não é sinônimo de imutabilidade)*.
- **Construtor Primário (C# 12+):** compara exatamente estas duas formas equivalentes:

```csharp
// Forma tradicional (Aula 07–08):
public sealed class MeuServicoTradicional
{
    private readonly IUsuarioRepository usuarios;
    public MeuServicoTradicional(IUsuarioRepository usuarios) { this.usuarios = usuarios; }
}

// Forma moderna com Construtor Primário (100% equivalente e mais enxuta!):
public sealed class MeuServicoModerno(IUsuarioRepository usuarios)
{
    // 'usuarios' já pode ser usado diretamente em todos os métodos da classe!
}
```

### 1.2 SQL Parametrizado vs SQL Injection (`A14.04`)
- **❌ Vulnerável:** `$"SELECT * FROM usuario WHERE username = '{nome}'"` → digitar `' OR '1'='1` altera a lógica do comando SQL!
- **✅ Seguro:** `WHERE username = @nome` + `cmd.Parameters.AddWithValue("nome", username)`. Mesmo que o usuário digite apóstrofos (`'`), o banco trata tudo apenas como dado textual inofensivo!

### 1.3 Hash + Salt com `PasswordHasher<string>` (Experimento Prático — `A14.05`)
**Não existe "descriptografar" um hash!** O `PasswordHasher<string>` combina a senha com um **salt** (tempero aleatório diferente a cada geração). Veja o que acontece ao rodar este microteste local:

```csharp
using Microsoft.AspNetCore.Identity;

var hasher = new PasswordHasher<string>();
string hash1 = hasher.HashPassword("admin", "SenhaDidatica123");
string hash2 = hasher.HashPassword("admin", "SenhaDidatica123");

// Os dois hashes são DIFERENTES por causa do salt aleatório, mas AMBOS validam a mesma senha!
Console.WriteLine($"Hashes iguais? {hash1 == hash2}"); // False!
Console.WriteLine(hasher.VerifyHashedPassword("admin", hash1, "SenhaDidatica123")); // Success
Console.WriteLine(hasher.VerifyHashedPassword("admin", hash2, "SenhaDidatica123")); // Success
Console.WriteLine(hasher.VerifyHashedPassword("admin", hash1, "SenhaErrada99999")); // Failed
```

---

## 2. Construção Completa em 4 Paradas Compiláveis (`Ctrl+Shift+B` em cada parada!)

> [!IMPORTANT]
> **Compile (`Ctrl+Shift+B`) ao final de cada parada!** Só avance para a próxima parada quando a anterior estiver com zero erros.

### Parada 1 — Modelos (`Models/`) (`A14.01`)

Crie 3 arquivos em `Models/` (`Shift+Alt+C`). Repare na **separação de segurança**: `Credencial` carrega `SenhaHash` e nunca chega às telas; `Sessao` não tem senha nem hash!

```csharp
// Models/Perfil.cs
namespace SupermercadoApp.Models;

public enum Perfil
{
    Administrador,
    Caixa
}
```

```csharp
// Models/Credencial.cs (usado APENAS entre UsuarioRepository e AutenticacaoService!)
namespace SupermercadoApp.Models;

public sealed record Credencial(int Id, string Username, string SenhaHash, Perfil Perfil);
```

```csharp
// Models/Sessao.cs (único dado devolvido para as Views — SEM senha nem hash!)
namespace SupermercadoApp.Models;

public sealed record Sessao(int Id, string Username, Perfil Perfil);
```

---

### Parada 2 — Contrato e Repositório Parametrizado (`Repositories/`) (`A14.02` e `A14.04`)

#### `Repositories/IUsuarioRepository.cs`

```csharp
using SupermercadoApp.Models;

namespace SupermercadoApp.Repositories;

public interface IUsuarioRepository
{
    Task<Credencial?> BuscarAsync(string username); // '?' indica que pode retornar null se não existir
    Task CriarAsync(string username, string hash, Perfil perfil);
    Task AtualizarHashAsync(int id, string hash);
    Task<bool> ExisteAsync();
}
```

#### `Repositories/UsuarioRepository.cs`

```csharp
using Npgsql;
using SupermercadoApp.Models;

namespace SupermercadoApp.Repositories;

public sealed class UsuarioRepository(NpgsqlDataSource banco) : IUsuarioRepository
{
    public async Task<Credencial?> BuscarAsync(string username)
    {
        await using var cmd = banco.CreateCommand("""
            SELECT id, username, senha_hash, perfil
            FROM usuario
            WHERE username = @nome
            """);
        cmd.Parameters.AddWithValue("nome", username);

        await using var leitor = await cmd.ExecuteReaderAsync();
        if (!await leitor.ReadAsync())
            return null; // Usuário não encontrado -> serviço recusa sem estourar NullReferenceException!

        return new Credencial(
            leitor.GetInt32(0),
            leitor.GetString(1),
            leitor.GetString(2),
            Enum.Parse<Perfil>(leitor.GetString(3)));
    }

    public async Task CriarAsync(string username, string hash, Perfil perfil)
    {
        await using var cmd = banco.CreateCommand("""
            INSERT INTO usuario(username, senha_hash, perfil)
            VALUES (@nome, @hash, @perfil)
            """);
        cmd.Parameters.AddWithValue("nome", username);
        cmd.Parameters.AddWithValue("hash", hash);
        cmd.Parameters.AddWithValue("perfil", perfil.ToString());
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task AtualizarHashAsync(int id, string hash)
    {
        await using var cmd = banco.CreateCommand(
            "UPDATE usuario SET senha_hash = @hash WHERE id = @id");
        cmd.Parameters.AddWithValue("hash", hash);
        cmd.Parameters.AddWithValue("id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<bool> ExisteAsync()
    {
        await using var cmd = banco.CreateCommand("SELECT EXISTS(SELECT 1 FROM usuario)");
        return (bool)(await cmd.ExecuteScalarAsync())!;
    }
}
```

---

### Parada 3 — Instalar `Microsoft.Extensions.Identity.Core` e Criar `Services/AutenticacaoService.cs`

1. No **Visual Studio 2022**, clique direito em `SupermercadoApp` → **Gerenciar Pacotes do NuGet** → procure e instale **`Microsoft.Extensions.Identity.Core`** (versão compatível com o SDK da turma).
2. Crie **`Services/AutenticacaoService.cs`**:

```csharp
using Microsoft.AspNetCore.Identity;
using SupermercadoApp.Models;
using SupermercadoApp.Repositories;

namespace SupermercadoApp.Services;

public sealed class AutenticacaoService(IUsuarioRepository usuarios)
{
    private readonly PasswordHasher<string> hasher = new();

    public async Task<Sessao?> EntrarAsync(string username, string senha)
    {
        // Normaliza APENAS o username (nunca a senha!):
        var usuario = await usuarios.BuscarAsync(username.Trim().ToLowerInvariant());
        if (usuario is null) return null;

        var resultado = hasher.VerifyHashedPassword(usuario.Username, usuario.SenhaHash, senha);
        if (resultado == PasswordVerificationResult.Failed) return null;

        // Rehash automático se a Microsoft atualizar o algoritmo no futuro:
        if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
            await usuarios.AtualizarHashAsync(usuario.Id, hasher.HashPassword(usuario.Username, senha));

        return new Sessao(usuario.Id, usuario.Username, usuario.Perfil);
    }

    public async Task CriarAsync(Sessao administrador, string username, string senha, Perfil perfil)
    {
        Permissoes.ExigirAdministrador(administrador);
        await CadastrarAsync(username, senha, perfil);
    }

    // Só funciona com a tabela vazia (via argumento --preparar-admin):
    public async Task PrepararAdministradorAsync(string username, string senha)
    {
        if (await usuarios.ExisteAsync())
            throw new InvalidOperationException("A preparação exige a tabela de usuários vazia.");
        await CadastrarAsync(username, senha, Perfil.Administrador);
    }

    private async Task CadastrarAsync(string username, string senha, Perfil perfil)
    {
        username = username.Trim().ToLowerInvariant();
        if (username.Length is < 3 or > 50 || username.Any(c => !char.IsAsciiLetterOrDigit(c) && c != '.'))
            throw new ArgumentException("Usuário: 3 a 50 letras sem acentos, números ou ponto.");

        // Política didática: 12 a 128 caracteres, preservando espaços intencionais (sem Trim!):
        if (senha.Length is < 12 or > 128)
            throw new ArgumentException("Use uma senha de 12 a 128 caracteres.");

        if (!Enum.IsDefined(perfil))
            throw new ArgumentException("Perfil inválido.");

        await usuarios.CriarAsync(username, hasher.HashPassword(username, senha), perfil);
    }
}

public static class Permissoes
{
    public static void ExigirAdministrador(Sessao sessao)
    {
        if (sessao.Perfil != Perfil.Administrador)
            throw new UnauthorizedAccessException("Esta operação exige um administrador.");
    }

    public static void ExigirCaixa(Sessao sessao)
    {
        if (sessao.Perfil != Perfil.Caixa)
            throw new UnauthorizedAccessException("Esta operação exige um operador de caixa.");
    }
}
```

---

### Parada 4 — `Views/Ui.cs`, Adaptação de `Views/FormLogin.cs` e `Program.cs`

1. **Remova o botão temporário `btnTestarConexao`** de `FormLogin` (no Designer ou no `.Designer.cs`), pois a tela não fala mais direto com o banco!
2. Crie **`Views/Ui.cs`** centralizando o tratamento seguro de erros de tela:

```csharp
namespace SupermercadoApp.Views;

internal static class Ui
{
    public static async Task<bool> ExecutarAsync(Func<Task> acao)
    {
        try
        {
            await acao();
            return true;
        }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException or UnauthorizedAccessException)
        {
            MessageBox.Show(e.Message, "Verifique os dados");
        }
        catch (Exception e)
        {
            // Nunca registre senha, SQL com valores nem string de conexão!
            System.Diagnostics.Trace.WriteLine($"Falha: {e.GetType().Name}");
            MessageBox.Show("Não foi possível concluir. Verifique a rede e tente novamente.", "Falha de comunicação");
        }
        return false;
    }
}
```

3. Atualize **`Views/FormLogin.cs`** (mantendo `InitializeComponent()` do seu Designer!):

```csharp
using SupermercadoApp.Models;
using SupermercadoApp.Services;

namespace SupermercadoApp.Views;

public partial class FormLogin : Form
{
    private readonly AutenticacaoService autenticacao;
    private readonly bool prepararAdmin;
    public Sessao? SessaoAutenticada { get; private set; }

    public FormLogin(AutenticacaoService autenticacao, bool prepararAdmin = false)
    {
        InitializeComponent();
        this.autenticacao = autenticacao;
        this.prepararAdmin = prepararAdmin;

        if (prepararAdmin)
        {
            Text = "Preparar primeiro administrador";
            btnLogin.Text = "Criar administrador";
        }
    }

    private async void btnLogin_Click(object sender, EventArgs e)
    {
        btnLogin.Enabled = false;
        try
        {
            await Ui.ExecutarAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(txtUsuario.Text) || txtSenha.Text.Length == 0)
                    throw new ArgumentException("Preencha usuário e senha.");

                if (prepararAdmin)
                {
                    await autenticacao.PrepararAdministradorAsync(txtUsuario.Text, txtSenha.Text);
                    MessageBox.Show("Administrador criado! Remova --preparar-admin e inicie novamente.");
                    DialogResult = DialogResult.OK;
                    return;
                }

                SessaoAutenticada = await autenticacao.EntrarAsync(txtUsuario.Text, txtSenha.Text);
                if (SessaoAutenticada is null)
                    MessageBox.Show("Usuário ou senha inválidos."); // Mesma mensagem para os 2 erros!
                else
                    DialogResult = DialogResult.OK;
            });
        }
        finally
        {
            txtSenha.Clear(); // Sempre limpa a senha da tela!
            btnLogin.Enabled = true;
        }
    }
}
```

4. Atualize **`Program.cs`** conectando toda a arquitetura:

```csharp
using SupermercadoApp.Infrastructure;
using SupermercadoApp.Repositories;
using SupermercadoApp.Services;
using SupermercadoApp.Views;

namespace SupermercadoApp;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        try
        {
            using var banco = Banco.Criar();
            var autenticacao = new AutenticacaoService(new UsuarioRepository(banco));

            bool prepararAdmin = args.Contains("--preparar-admin");
            using var login = new FormLogin(autenticacao, prepararAdmin);
            if (login.ShowDialog() == DialogResult.OK && !prepararAdmin && login.SessaoAutenticada is { } sessao)
            {
                MessageBox.Show($"Bem-vindo, {sessao.Username} ({sessao.Perfil})!");
            }
        }
        catch (InvalidOperationException erro)
        {
            MessageBox.Show(erro.Message, "Configuração");
        }
    }
}
```

---

## 3. Preparando o Primeiro Administrador e Testando (`A14.06`–`A14.08`)

1. No VS 2022: botão direito em **`SupermercadoApp`** → **Propriedades** → **Depurar → Geral → Abrir interface do usuário de perfis de inicialização** → em **Argumentos da linha de comando**, digite **`--preparar-admin`**.
2. Pressione **`F5`**, teste primeiro uma senha de `11` caracteres (recusada!), depois crie o admin com `12+` caracteres.
3. **Pare o programa** e **APAGUE `--preparar-admin`** dos argumentos!
4. Pressione **`F5`** e comprove:
   - Login em **MAIÚSCULAS** com a senha correta → funciona (normalização)!
   - Senha errada e usuário inexistente (inclusive com apóstrofo `'`) → ambos mostram a mesma mensagem genérica `"Usuário ou senha inválidos."` e limpam `txtSenha`.
   - Rodar `--preparar-admin` novamente com a tabela populada → recusado!
5. Confira no SQL Editor (sem exibir o hash inteiro):
   ```sql
   SELECT username, perfil, length(senha_hash) AS tamanho_hash FROM usuario;
   ```

[Anterior: Aula 13](13-Conectando-Banco-Dados.md) · [Próxima: Aula 15 — Sessão e perfis](15-Sessao-e-Perfis.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 14 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-14). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
