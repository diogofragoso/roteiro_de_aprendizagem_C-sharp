# Aula 13 — Autenticação Segura com Hash, Salt e SQL Parametrizado

**Duração:** 3h30. **Pré-requisito:** [Aula 12](12-Conectando-Banco-Dados.md), tabela `usuario` vazia e `SELECT 1` funcionando. **Entrega:** primeiro administrador cadastrado com hash seguro e tela de login autenticando credenciais reais via `AutenticacaoService` e `UsuarioRepository`, sem jamais expor senhas ou hashes para a interface.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Conferir `SELECT 1` e planejar a remoção do acesso direto ao banco na tela |
| Explicação | 30 | `enum`, `record`, `Task<T?>`, SQL parametrizado, Hash+Salt e construtor primário |
| Construção | 60 | Paradas 1 e 2: construir modelos e repositório parametrizado de usuários |
| Intervalo | 15 | Pausa |
| Prática | 60 | Paradas 3 e 4: instalar Identity.Core, construir serviço de hash e ligar `FormLogin` |
| Entrega | 30 | Preparar primeiro admin (`--preparar-admin`), testar login e conferir o banco |

---

## 1. Conceitos Novos de C# e Segurança (Entenda Cada Detalhe e o "Por Quê")

Esta é uma das aulas mais importantes do curso. Trabalharemos em **4 paradas com compilação intermediária (`Ctrl+Shift+B`)** para que você domine cada peça antes de avançar.

### 1.1 `enum`, `record` e Separação de Dados Sensíveis (`A13.01`)
- **`enum` (Enumeração):** define um conjunto fixo e fechado de opções nomeadas (ex.: `public enum Turno { Manha, Tarde }`). **Por que usar `enum Perfil` em vez de `string`?** Com `string`, alguém poderia digitar `"Admnistrador"` errado e só descobrir um bug depois. Com `enum`, o compilador do Visual Studio 2022 valida cada uso!
- **`record` (Registro):** uma forma moderna e concisa de criar classes focadas em carregar dados imutáveis. Exemplo genérico:
  ```csharp
  public sealed record Cracha(int Id, string Nome);
  ```
  Essa única linha gera automaticamente o construtor e as propriedades somente leitura `Id` e `Nome`!
- **`sealed`:** impede que outras classes herdem dessa classe/record. **Atenção:** `sealed` bloqueia herança, não é sinônimo de imutabilidade.
- **Por que criar DOIS records separados — `Credencial` e `Sessao`?**
  - `Credencial` carrega `SenhaHash` e existe **apenas** entre o `UsuarioRepository` e o `AutenticacaoService` para conferir a senha.
  - `Sessao` carrega **apenas** `Id`, `Username` e `Perfil` (**sem `SenhaHash`**!) e é o único objeto devolvido para as telas (`Views`). Assim, nenhuma tela consegue vazar um hash por acidente!

### 1.2 Construtor Primário e Nulabilidade Assíncrona (`A13.02` e `A13.03`)
- **Construtor Primário (C# 12+):** em vez de escrever campo + construtor longo como na Aula 08, você pode declarar o parâmetro direto ao lado do nome da classe:
  ```csharp
  public sealed class MeuServico(IMeuRepositorio repo)
  {
      // 'repo' já fica disponível dentro dos métodos da classe!
  }
  ```
  Ele cumpre exatamente o mesmo papel de injeção de dependência pelo construtor tradicional (você pode usar qualquer uma das duas formas).
- **`Task<Credencial?>`:** significa *"esta operação assíncrona vai entregar um objeto `Credencial` **ou `null`** (se o usuário não existir no banco)"*. O ponto de interrogação `?` obriga o chamador a testar `if (usuario is null)` antes de acessar propriedades.

### 1.3 SQL Parametrizado vs SQL Injection (`A13.04`)
**Por que JAMAIS concatenar texto digitado pelo usuário dentro de uma string SQL?**
- **❌ Vulnerável (SQL Injection):** `$"SELECT ... WHERE username = '{username}'"` → Se um invasor digitar `' OR '1'='1`, a string altera a lógica do comando SQL no banco!
- **✅ Seguro (Parametrizado):**
  ```csharp
  await using var cmd = banco.CreateCommand("""
      SELECT id, titulo FROM livro WHERE codigo = @cod
      """);
  cmd.Parameters.AddWithValue("cod", codigoDigitado);
  ```
  - As três aspas (`"""`) delimitam uma **string bruta multilinha**, mantendo o SQL legível sem concatenações.
  - `@cod` é um marcador de parâmetro: o driver envia o SQL e o valor **separadamente** ao PostgreSQL. Mesmo que alguém digite um apóstrofo `'` ou `OR 1=1`, o banco trata aquilo puramente como texto comum de busca!

### 1.4 Hash, Salt e `PasswordHasher<string>` (`A13.05` e `A13.06`)
**Por que buscar no SQL somente pelo `username` e verificar a senha em C#?**
Porque **não existe "descriptografar" um hash!**
- Um **hash criptográfico com salt** transforma a senha em uma assinatura irreversível combinada com um tempero aleatório (**salt**) diferente a cada usuário.
- Por isso, duas contas com a **mesma senha** geram **hashes completamente diferentes** no banco! (Logo, um `WHERE senha_hash = ...` no SQL nunca funcionaria.)
- Usamos a classe pronta **`PasswordHasher<string>`** (do pacote oficial `Microsoft.Extensions.Identity.Core`) em vez de inventar criptografia própria ou usar MD5:
  - `hasher.HashPassword(username, senha)`: gera o hash + salt pronto para salvar no banco.
  - `hasher.VerifyHashedPassword(username, hashSalvo, senhaDigitada)`: recalcula usando o salt embutido no hash salvo e retorna um dos 3 valores de `PasswordVerificationResult`:
    1. `Failed` → senha incorreta (devolva `null`);
    2. `Success` → senha correta;
    3. `SuccessRehashNeeded` → senha correta, mas gravada com parâmetros antigos; atualizamos o hash no banco chamando `AtualizarHashAsync`!
- **Política didática de normalização e senha:** normalizamos `username` com `.Trim().ToLowerInvariant()` (assim `ADMIN` e `admin` são a mesma conta), mas **nunca** aplicamos `.Trim()` na senha (espaços podem fazer parte de uma senha longa de 12 a 128 caracteres!).

### 1.5 Centralizando Tratamento de Erros de Tela (`Ui.ExecutarAsync`)
Para não repetir 3 blocos `catch` diferentes em todos os botões de todas as telas do sistema, criaremos uma classe auxiliar `Views/Ui.cs` com um método estático `ExecutarAsync(Func<Task> acao)`:
- `Func<Task> acao` representa "qualquer bloco assíncrono que a tela quiser executar".
- Ele captura erros esperados de regra (`ArgumentException`, `InvalidOperationException`, `UnauthorizedAccessException`) mostrando `e.Message` ao usuário, e captura erros técnicos/de rede mostrando uma mensagem genérica sem expor credenciais.

---

## 2. Roteiro de Construção Guiada em 4 Paradas (Sua Vez no VS 2022!)

> [!IMPORTANT]
> **Compilação em cada parada:** pressione **`Ctrl+Shift+B`** no Visual Studio 2022 ao final de cada uma das 4 paradas abaixo. Só avance quando a parada anterior compilar com zero erros!

### Parada 1 — Modelos (`Models/`)
Crie três arquivos em `Models/` (`Shift+Alt+C`), todos em `namespace SupermercadoApp.Models;`:
1. **`Perfil.cs`:** declare `public enum Perfil` com dois valores: `Administrador` e `Caixa`.
2. **`Credencial.cs`:** declare `public sealed record Credencial(int Id, string Username, string SenhaHash, Perfil Perfil);`.
3. **`Sessao.cs`:** declare `public sealed record Sessao(int Id, string Username, Perfil Perfil);` *(observe: sem `SenhaHash`!)*.

### Parada 2 — Contrato e Repositório de Usuários (`Repositories/`)
Crie dois arquivos em `Repositories/`, ambos em `namespace SupermercadoApp.Repositories;`:
1. **`IUsuarioRepository.cs`:** declare `public interface IUsuarioRepository` com 4 métodos assíncronos:
   - `Task<Credencial?> BuscarAsync(string username);`
   - `Task CriarAsync(string username, string hash, Perfil perfil);`
   - `Task AtualizarHashAsync(int id, string hash);`
   - `Task<bool> ExisteAsync();`
2. **`UsuarioRepository.cs`:** declare `public sealed class UsuarioRepository(NpgsqlDataSource banco) : IUsuarioRepository` e implemente os 4 métodos usando SQL parametrizado e `await using`:
   - **`BuscarAsync`:** faça `SELECT id, username, senha_hash, perfil FROM usuario WHERE username = @nome`, passe `@nome`, abra `await using var leitor = await cmd.ExecuteReaderAsync();`. Se `!await leitor.ReadAsync()`, retorne `null`. Caso encontre, construa `new Credencial(...)` lendo colunas `0` (`GetInt32`), `1` (`GetString`), `2` (`GetString`) e `3` (`Enum.Parse<Perfil>(leitor.GetString(3))`).
   - **`CriarAsync`:** faça `INSERT INTO usuario(username, senha_hash, perfil) VALUES (@nome, @hash, @perfil)`, adicionando os 3 parâmetros (`perfil.ToString()`) e chamando `await cmd.ExecuteNonQueryAsync()`.
   - **`AtualizarHashAsync`:** faça `UPDATE usuario SET senha_hash = @hash WHERE id = @id` com os 2 parâmetros e `ExecuteNonQueryAsync()`.
   - **`ExisteAsync`:** faça `SELECT EXISTS(SELECT 1 FROM usuario)` com `ExecuteScalarAsync()` convertendo o retorno para `bool`.

### Parada 3 — Pacote NuGet, `AutenticacaoService` e `Permissoes` (`Services/`)
1. **Instalar NuGet no Visual Studio 2022:** botão direito em `SupermercadoApp` → **Gerenciar Pacotes do NuGet** → procure e instale **`Microsoft.Extensions.Identity.Core`** (versão compatível com o SDK da turma; apesar do namespace `Microsoft.AspNetCore.Identity`, ele funciona perfeitamente em aplicativos desktop!).
2. Crie **`Services/AutenticacaoService.cs`** em `namespace SupermercadoApp.Services;` (com `using Microsoft.AspNetCore.Identity;`, `Models` e `Repositories`):
   - **Classe `AutenticacaoService(IUsuarioRepository usuarios)`:**
     - Campo interno: `private readonly PasswordHasher<string> hasher = new();`.
     - **`EntrarAsync(string username, string senha)` → `Task<Sessao?>`:**
       1. Busque o usuário passando `username.Trim().ToLowerInvariant()`.
       2. Se retornar `null`, devolva `null`.
       3. Verifique a senha com `hasher.VerifyHashedPassword(usuario.Username, usuario.SenhaHash, senha)`.
       4. Se o resultado for `PasswordVerificationResult.Failed`, devolva `null`.
       5. Se for `PasswordVerificationResult.SuccessRehashNeeded`, chame `await usuarios.AtualizarHashAsync(usuario.Id, hasher.HashPassword(usuario.Username, senha))`.
       6. Retorne `new Sessao(usuario.Id, usuario.Username, usuario.Perfil)`.
     - **Método privado auxiliar `CadastrarAsync(string username, string senha, Perfil perfil)`:**
       1. Normalize `username = username.Trim().ToLowerInvariant();`.
       2. Valide que `username.Length` está entre `3` e `50` e que todo caractere `c` atende `char.IsAsciiLetterOrDigit(c) || c == '.'` (se falhar, lance `ArgumentException`).
       3. Valide que `senha.Length` está entre `12` e `128` (**sem `Trim()`**! Se falhar, lance `ArgumentException`).
       4. Valide `Enum.IsDefined(perfil)` (se falhar, lance `ArgumentException`).
       5. Chame `await usuarios.CriarAsync(username, hasher.HashPassword(username, senha), perfil)`.
     - **`PrepararAdministradorAsync(string username, string senha)`:** verifique `if (await usuarios.ExisteAsync())` → lance `InvalidOperationException` (*"A preparação exige a tabela de usuários vazia."*). Só se estiver vazia, chame `await CadastrarAsync(username, senha, Perfil.Administrador)`.
     - **`CriarAsync(Sessao administrador, string username, string senha, Perfil perfil)`:** chame `Permissoes.ExigirAdministrador(administrador);` e depois `await CadastrarAsync(username, senha, perfil)`.
   - **Classe estática `Permissoes`** (no mesmo arquivo ou em `Services/Permissoes.cs`, usada na Aula 14):
     - `ExigirAdministrador(Sessao sessao)`: se `sessao.Perfil != Perfil.Administrador`, lance `UnauthorizedAccessException`.
     - `ExigirCaixa(Sessao sessao)`: se `sessao.Perfil != Perfil.Caixa`, lance `UnauthorizedAccessException`.

### Parada 4 — Helper `Views/Ui.cs`, Adaptação do `FormLogin` e `Program.cs`
1. Crie **`Views/Ui.cs`** (`internal static class Ui` em `namespace SupermercadoApp.Views;`) e construa por enquanto apenas o método **`public static async Task<bool> ExecutarAsync(Func<Task> acao)`**:
   - Dentro de `try`: `await acao(); return true;`.
   - `catch (Exception e) when (e is ArgumentException or InvalidOperationException or UnauthorizedAccessException)`: mostre `MessageBox.Show(e.Message, "Verifique os dados");`.
   - `catch (Exception e)`: registre apenas `System.Diagnostics.Trace.WriteLine($"Falha: {e.GetType().Name}");` (nunca a senha ou conexão!) e mostre mensagem amigável de falha de comunicação.
   - Retorne `false` ao final em caso de erro.
2. **Adaptar seu `Views/FormLogin.cs` (mantendo o Designer da Aula 10!):**
   - No Designer (`Shift+F7`), **selecione e apague o botão temporário `btnTestarConexao`** (`Delete`). *(Dica: se o Designer do VS 2022 reclamar do construtor com parâmetro da Aula 12, você pode apagar o campo/controle `btnTestarConexao` diretamente no `FormLogin.Designer.cs` e apagar o método `btnTestarConexao_Click` em `FormLogin.cs`.)*
   - No código (`F7`): remova `using Npgsql;`, remova o método `btnTestarConexao_Click` e remova o campo `NpgsqlDataSource`! Adicione `using SupermercadoApp.Models;` e `using SupermercadoApp.Services;`: agora a tela depende **apenas** de `AutenticacaoService`.
   - Declare os campos `private readonly AutenticacaoService autenticacao;`, `private readonly bool prepararAdmin;` e a propriedade pública `public Sessao? SessaoAutenticada { get; private set; }`.
   - Atualize o construtor para `public FormLogin(AutenticacaoService autenticacao, bool prepararAdmin = false)` mantendo `InitializeComponent();`, guardando os campos e, se `prepararAdmin` for `true`, mudando `btnLogin.Text = "Criar administrador";`.
   - No evento **`private async void btnLogin_Click(object sender, EventArgs e)`**:
     1. Desabilite `btnLogin.Enabled = false;`.
     2. Em um bloco `try`, chame `await Ui.ExecutarAsync(async () => { ... });`:
        - Valide os campos vazios lançando `ArgumentException`.
        - Se `prepararAdmin` for verdadeiro: chame `await autenticacao.PrepararAdministradorAsync(txtUsuario.Text, txtSenha.Text);`, avise para remover o argumento de preparação, defina `DialogResult = DialogResult.OK;` e `return;`.
        - Caso contrário (login normal): atribua `SessaoAutenticada = await autenticacao.EntrarAsync(txtUsuario.Text, txtSenha.Text);`. Se for `null`, exiba `"Usuário ou senha inválidos."` (mensagem genérica igual para usuário inexistente ou senha errada!); caso contrário, defina `DialogResult = DialogResult.OK;`.
     3. No bloco **`finally`**: limpe a senha (`txtSenha.Clear();`) e reabilite `btnLogin.Enabled = true;`.
3. **Atualizar `Program.cs`:**
   - Adicione no topo do arquivo os `using` das suas camadas: `SupermercadoApp.Infrastructure`, `SupermercadoApp.Repositories`, `SupermercadoApp.Services` e `SupermercadoApp.Views`.
   - Altere `Main()` para `Main(string[] args)` para ler argumentos de linha de comando.
   - Dentro do `try`: crie `using var banco = Banco.Criar();`, instancie `var autenticacao = new AutenticacaoService(new UsuarioRepository(banco));`, verifique `bool prepararAdmin = args.Contains("--preparar-admin");`, abra `using var login = new FormLogin(autenticacao, prepararAdmin);` com `login.ShowDialog();`.
   - Se `!prepararAdmin && login.SessaoAutenticada is { } sessao` (o padrão `is { } sessao` significa *"não é `null` e já guarda na variável `sessao`"*), exiba `MessageBox.Show($"Bem-vindo, {sessao.Username} ({sessao.Perfil})");`.

---

## 3. Como Preparar o Primeiro Administrador no Visual Studio 2022 (`A13.07` e `A13.08`)

**Por que não criamos um admin com senha padrão no SQL?** Porque senhas padrão hardcoded são a causa número 1 de invasões em sistemas reais!

1. No **Gerenciador de Soluções** do Visual Studio 2022, clique com o botão direito em **`SupermercadoApp`** → **Propriedades**.
2. No menu esquerdo, clique em **Depurar → Geral → Abrir interface do usuário de perfis de inicialização de depuração**.
3. No campo **Argumentos da linha de comando**, digite exatamente: **`--preparar-admin`**. Feche a janela.
4. Pressione **`F5`**: o botão aparecerá como **"Criar administrador"**. Digite um username (3 a 50 chars) e uma senha de **12 a 128 caracteres** e clique para criar.
5. **Pare o programa**, volte aos perfis de inicialização e **APAGUE `--preparar-admin`**!
6. Pressione **`F5`** novamente e teste:
   - Login com o username em **MAIÚSCULAS** e a senha correta → deve dar boas-vindas (comprovando normalização!).
   - Senha errada e usuário inexistente → **ambos** devem exibir a mesma mensagem genérica `"Usuário ou senha inválidos."`.
   - Tente rodar com `--preparar-admin` de novo com a tabela já populada → deve recusar!
7. **Confira no SQL Editor** (sem projetar o hash inteiro na tela da turma):
   ```sql
   SELECT username, perfil, length(senha_hash) AS tamanho_hash FROM usuario;
   ```

[Anterior: Aula 12](12-Conectando-Banco-Dados.md) · [Próxima: Aula 14 — Sessão e perfis](14-Sessao-e-Perfis.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 13 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-13). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
