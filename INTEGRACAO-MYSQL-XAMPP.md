# Opção 2 — MySQL/MariaDB pelo XAMPP

Esta trilha complementa as aulas 07 e 08 e orienta a adaptação das aulas seguintes. Escolha **Neon/PostgreSQL com Npgsql** ou **MySQL/MariaDB local com MySqlConnector** antes da aula 07 e mantenha a escolha ao longo do projeto. Os objetivos, os modelos, os serviços e os critérios de entrega continuam os mesmos.

O XAMPP distribui **MariaDB**, embora o painel use o nome **MySQL**, conforme a [FAQ oficial](https://www.apachefriends.org/faq_windows.html). O MySqlConnector atende aos dois servidores. Confira a versão instalada com `SELECT VERSION();`: para esta trilha, use MariaDB 10.2.1 ou superior, ou MySQL 8.0.16 ou superior, com restrições `CHECK` efetivamente habilitadas.

**Escopo:** os arquivos em `src`, os checkpoints e o runner de integração existentes continuam sendo a referência PostgreSQL. Este complemento fornece os scripts alternativos e os trechos de integração para o projeto do estudante; mudar apenas a string de conexão não converte a referência final para MySQL. Faça as adaptações abaixo conforme cada aula.

## 1. Prepare o XAMPP para a aula 07

1. Instale o [XAMPP para Windows](https://www.apachefriends.org/download.html), com orientação do professor.
2. No **XAMPP Control Panel**, inicie **MySQL**. Confira a porta configurada; normalmente é `3306`. Se outro servidor já ocupar essa porta, identifique o serviço com o professor ou configure outra porta e use o mesmo valor na conexão C#.
3. Para administrar pelo navegador, inicie também **Apache** e abra o phpMyAdmin pelo botão **Admin**, normalmente em `http://localhost/phpmyadmin/`. O aplicativo Windows Forms precisa somente do serviço MySQL; Apache e PHP são usados aqui pela ferramenta de administração.
4. Cada estudante/dupla deve ter um banco separado. Nos exemplos usamos `supermercado_lab`; em máquina compartilhada, o professor deve fornecer nomes e contas diferentes e ajustar todos os exemplos.
5. Na aba **SQL**, usando a conta administrativa local, execute o modelo abaixo após substituir a senha. A conta administrativa serve para preparar a estrutura; a aplicação usa uma conta própria.

```sql
CREATE DATABASE supermercado_lab
    CHARACTER SET utf8mb4 COLLATE utf8mb4_bin;

CREATE USER 'supermercado_app'@'127.0.0.1'
    IDENTIFIED BY 'SUBSTITUA_POR_UMA_SENHA_LOCAL_FORTE';
GRANT SELECT, INSERT, UPDATE ON supermercado_lab.*
    TO 'supermercado_app'@'127.0.0.1';
```

Não salve nem entregue o comando com a senha real. Não utilize `root` na aplicação. A conta acima corresponde à conexão TCP com `127.0.0.1`; mantenha esse endereço no C#. A senha dessa conta é diferente da senha dos usuários Administrador/Caixa do supermercado.

Esse ambiente é de laboratório local. Não exponha o XAMPP à internet. A conexão sem TLS descrita adiante é exclusiva do computador local; servidores remotos precisam de TLS com certificado verificado.

## 2. Crie as tabelas no momento de cada aula

Selecione `supermercado_lab` no phpMyAdmin e use **Importar** para executar cada arquivo uma única vez, nesta ordem:

| Aula | Script MySQL/MariaDB | Resultado |
| --- | --- | --- |
| 07 | [001-usuarios.sql](database/mysql/001-usuarios.sql) | Tabela `usuario` vazia |
| 11 | [002-produtos.sql](database/mysql/002-produtos.sql) | Produto e movimento de estoque |
| 16 | [003-vendas.sql](database/mysql/003-vendas.sql) | Venda e itens |

Use esses arquivos em lugar dos scripts PostgreSQL de mesmo número. Eles usam `AUTO_INCREMENT`, tabelas **InnoDB**, chaves estrangeiras e restrições. A collation binária diferencia maiúsculas e minúsculas, permitindo verificar a regra de login em minúsculas. O aplicativo continua normalizando e validando os dados.

Comandos de criação de tabela podem provocar commit implícito no MySQL/MariaDB. Não espere que `ROLLBACK` desfaça uma importação parcial: se ocorrer falha, confira quais tabelas foram criadas e corrija com o professor antes de continuar. As transações de reposição e venda usam operações de dados sobre InnoDB, sem comandos de criação de tabela.

Na aula 07, execute:

```sql
SELECT VERSION(), DATABASE();
SELECT id, username, perfil FROM usuario;
SHOW CREATE TABLE usuario;
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_schema = DATABASE() AND table_name = 'usuario';
```

A consulta de usuários deve retornar zero linhas. Crie o primeiro administrador pela aplicação na aula 09, usando o mesmo serviço de hash; nunca insira senha aberta em `senha_hash`. O laboratório `database/laboratorio/07-restricoes.sql` é PostgreSQL: para esta opção, reproduza seus casos em tabelas temporárias com sintaxe MySQL, na mesma sessão, sem alterar `usuario`. Confira rejeição de duplicidade, `NULL`, login em maiúsculas e perfil inválido.

## 3. Instale o conector na aula 08

No projeto Windows Forms do estudante, abra **Gerenciar Pacotes NuGet**, procure **MySqlConnector** na origem `nuget.org` e instale uma versão estável compatível com .NET 10. Registre a versão escolhida no `.csproj` e use a mesma em toda a turma. Confira compatibilidade e alertas antes da aula. Este exemplo usa a API `MySqlDataSourceBuilder` do MySqlConnector 2.3 ou superior.

Use o namespace `MySqlConnector`. Os exemplos desta trilha não usam `MySql.Data` nem Npgsql. O pacote Identity.Core e o mecanismo de hash da aula 09 continuam necessários.

## 4. Configure a conexão fora do código

Nas variáveis de ambiente da conta Windows, configure `SUPERMERCADO_DB` com os dados locais. Modelo para preencher na interface do Windows:

```text
Server=127.0.0.1;Port=3306;Database=supermercado_lab;User ID=supermercado_app;Password=SUA_SENHA_LOCAL;SslMode=Disabled;DateTimeKind=Utc;Connection Timeout=15;Default Command Timeout=30;
```

Feche e reabra o Visual Studio. Não publique esse valor em Git, capturas ou evidências. Para senhas com caracteres especiais, respeite o formato de conexão do MySqlConnector; consulte as [opções oficiais](https://mysqlconnector.net/connection-options/).

`SslMode=Disabled` atende somente a este laboratório em `127.0.0.1`, onde o XAMPP pode não ter TLS configurado. Em servidor remoto, configure TLS e use `SslMode=VerifyFull` com hostname e certificado válidos. Não altere o `Banco.cs` da trilha Neon para desativar TLS.

`DateTimeKind=Utc` define como o conector interpreta os valores lidos; não converte automaticamente o horário do servidor. Os scripts alternativos usam `UTC_TIMESTAMP(6)` e o aplicativo deve enviar datas UTC no relatório.

## 5. Crie Banco.cs e teste a conexão

No projeto do estudante, substitua a implementação PostgreSQL de `Infrastructure/Banco.cs` por:

```csharp
using MySqlConnector;

namespace SupermercadoApp.Infrastructure;

public static class Banco
{
    public static MySqlDataSource Criar()
    {
        string? texto = Environment.GetEnvironmentVariable("SUPERMERCADO_DB");
        if (string.IsNullOrWhiteSpace(texto))
            throw new InvalidOperationException(
                "Configure a variável SUPERMERCADO_DB e reabra o Visual Studio.");

        return new MySqlDataSourceBuilder(texto).Build();
    }
}
```

Crie a fonte de dados uma vez no `Program.cs`, com `using var banco = Banco.Criar()`, e injete-a pelo construtor como na aula 08. No `FormLogin`, use `using MySqlConnector;`, declare `private readonly MySqlDataSource banco;` e receba `MySqlDataSource banco` no construtor. Preserve `InitializeComponent()`.

No evento do botão de diagnóstico:

```csharp
private async void btnTestarConexao_Click(object sender, EventArgs e)
{
    btnTestarConexao.Enabled = false;
    try
    {
        await using var conexao = await banco.OpenConnectionAsync();
        await using var comando = new MySqlCommand("SELECT 1", conexao);
        var resultado = await comando.ExecuteScalarAsync();
        MessageBox.Show($"Conexão confirmada: {resultado}");
    }
    catch (Exception)
    {
        MessageBox.Show("Não foi possível consultar. Verifique o XAMPP e a configuração.");
    }
    finally
    {
        btnTestarConexao.Enabled = true;
    }
}
```

O descarte devolve a conexão ao pool. Não mantenha uma conexão aberta global. O [exemplo oficial da API](https://mysqlconnector.net/tutorials/basic-api/) mostra consultas assíncronas e parâmetros; o [builder da fonte de dados](https://mysqlconnector.net/api/mysqlconnector/mysqldatasourcebuildertype/) centraliza a configuração.

## 6. Adapte a persistência das aulas 09 a 19

Mantenha as interfaces `IUsuarioRepository`, `IProdutoRepository` e `IVendaRepository`, os modelos, os serviços e as regras de perfil. Na aula 09, retire o diagnóstico da tela e injete `AutenticacaoService`, conforme o capítulo principal. Os repositórios passam a receber `MySqlDataSource`.

| Referência PostgreSQL | Alternativa MySQL/MariaDB |
| --- | --- |
| `using Npgsql;` | `using MySqlConnector;` |
| `NpgsqlDataSource` | `MySqlDataSource` |
| `NpgsqlCommand` / `NpgsqlDataReader` | `MySqlCommand` / `MySqlDataReader` |
| `banco.CreateCommand(sql)` | Abra a conexão e use `new MySqlCommand(sql, conexao)` |
| `GENERATED ALWAYS AS IDENTITY` | `AUTO_INCREMENT` nos scripts alternativos |
| `INSERT ... RETURNING id` | `ExecuteNonQueryAsync()` e `cmd.LastInsertedId` |
| `uuid` | `char(36)`; envie `operacao.ToString("D")` em todas as consultas |
| `timestamptz` | `datetime(6)` com convenção UTC |
| `ILIKE` | `LIKE`; nesta collation a busca diferencia maiúsculas; use `LOWER(nome) LIKE LOWER(@busca)` para ignorar essa diferença |
| `PostgresException` por unicidade | `MySqlException` com `Number == 1062` |

Nos comandos fora de transação, abra e descarte uma conexão por operação. Exemplo para `UsuarioRepository.BuscarAsync`, mantendo o retorno de `Credencial` já apresentado na aula 09:

```csharp
await using var conexao = await banco.OpenConnectionAsync();
await using var cmd = new MySqlCommand(
    "SELECT id, username, senha_hash, perfil FROM usuario WHERE username = @nome",
    conexao);
cmd.Parameters.AddWithValue("nome", username);
await using var leitor = await cmd.ExecuteReaderAsync();
if (!await leitor.ReadAsync()) return null;
return new Credencial(leitor.GetInt32(0), leitor.GetString(1), leitor.GetString(2),
    Enum.Parse<Perfil>(leitor.GetString(3)));
```

Repita o padrão em `CriarAsync` e `AtualizarHashAsync`, preservando os parâmetros. Em `ExisteAsync`, use `Convert.ToBoolean(await cmd.ExecuteScalarAsync())` para o resultado de `SELECT EXISTS(SELECT 1 FROM usuario)`: o servidor pode devolver um número em vez do `bool` esperado pelo cast da referência PostgreSQL.

Em `Views/Ui.cs`, substitua o tratamento específico de duplicidade por:

```csharp
catch (MySqlException erro) when (erro.Number == 1062)
{
    MessageBox.Show("Código ou usuário já cadastrado. Use outro valor.");
}
```

Mantenha o tratamento genérico sem expor credenciais. Faça a mesma troca em outros pontos que tratem exceções do PostgreSQL.

### Produtos e reposição — aulas 11 a 14

As validações, os parâmetros `@nome`, `@codigo` e os comandos `INSERT`, `UPDATE` e `LIMIT` continuam. Use os tipos MySqlConnector e ajuste a pesquisa conforme a tabela acima. Na reposição, abra uma conexão, inicie `BeginTransactionAsync()` e passe **a mesma conexão e a mesma transação** ao comando que altera estoque e ao que registra o movimento:

```csharp
await using var conexao = await banco.OpenConnectionAsync();
await using var transacao = await conexao.BeginTransactionAsync();
await using var cmd = new MySqlCommand(sql, conexao, transacao);
// Adicione parâmetros, execute os dois comandos e só depois confirme.
// Cada comando da operação deve receber conexao e transacao.
```

O trecho mostra a estrutura, não substitui o método completo. Preserve a verificação da quantidade de linhas afetadas, o `CommitAsync()` ao final e o descarte da transação sem commit em caso de falha.

### Venda e repetição da operação — aulas 16 e 17

Não substitua apenas os nomes dos tipos em `VendaRepository`: `pg_advisory_xact_lock` é exclusivo do PostgreSQL. Para manter a mesma garantia nesta trilha:

1. Abra uma conexão e obtenha um bloqueio nomeado com `SELECT GET_LOCK(@chave, 10)`, antes de iniciar a transação. Use uma chave determinística por banco e operação, por exemplo `$"{conexao.Database}:{operacao:N}"`, de até 64 caracteres. Só continue se o retorno convertido para inteiro for `1`; timeout/`NULL` interrompem a tentativa.
2. Dentro de `try/finally`, inicie a transação e consulte se aquela operação já existe, conferindo também `usuario_id`. Se existir, devolva o mesmo resultado sem descontar estoque novamente. Envie o Guid como string no formato `D` tanto nessa consulta quanto em `BuscarAsync` e no `INSERT`.
3. Preserve `SELECT ... FOR UPDATE` nos produtos, em ordem de ID, a conferência do estoque e os comandos de venda, itens e movimentos na mesma transação InnoDB.
4. No `INSERT` da venda, remova `RETURNING id`, execute `await cmd.ExecuteNonQueryAsync()` e leia `int vendaId = checked((int)cmd.LastInsertedId);` antes de descartar o comando. Não use `MAX(id)`.
5. Confirme com `CommitAsync()` antes de anunciar sucesso. No `finally`, depois de encerrar a transação, execute `SELECT RELEASE_LOCK(@chave)` **na mesma conexão** e confira retorno `1`, antes de devolvê-la ao pool. O caminho que encontra uma venda existente também precisa liberar o bloqueio. Se a liberação falhar, descarte a conexão física com `MySqlConnection.ClearPool(conexao)` e trate a falha sem repetir a venda com outro Guid.

O bloqueio nomeado pertence à sessão e **não é liberado pelo commit ou rollback**. Todos os caminhos de escrita de venda precisam seguir esse protocolo, além da restrição `UNIQUE(operacao)`. Consulte [GET_LOCK no MariaDB](https://mariadb.com/docs/server/reference/sql-functions/secondary-functions/miscellaneous-functions/get_lock) e [LastInsertedId no conector](https://mysqlconnector.net/api/mysqlconnector/mysqlcommand/lastinsertedid/).

### Relatório e usuários — aulas 18 e 19

Continue enviando `inicioUtc` e `fimUtc` e usando o intervalo `>= @inicio AND < @fim`. Com `DateTimeKind=Utc`, leia `criado_em` como UTC e converta para o horário local apenas na apresentação. Preserve o hash de senha e a verificação de administrador no serviço de cadastro.

## 7. Confira e registre as evidências

- Aula 07: versão do servidor, banco escolhido, tabelas e restrições, sem senhas ou hashes nas capturas.
- Aula 08: `SELECT 1` retorna `1`; variável ausente apresenta orientação; parar o MySQL no XAMPP provoca falha tratada; o botão volta a habilitar e a janela responde durante a espera.
- Aula 09: administrador criado pela aplicação, login correto aceito e senha incorreta recusada.
- Aulas 11–14: código duplicado recusado, estoque não negativo e reposição com movimento na mesma transação.
- Aulas 16–17: falha antes do commit não deixa venda parcial; dois caixas disputando o último item geram uma única venda; repetir o mesmo Guid, inclusive simultaneamente, não duplica venda nem desconto de estoque.
- Aula 18: relatório respeita as fronteiras das datas e o horário local apresentado.
- Aula 20: publique e execute com o MySQL do XAMPP iniciado e a variável local configurada. O banco local precisa estar acessível em cada computador; copiar o executável não copia os dados.

Use o mesmo [caderno de evidências](CADERNO-DE-EVIDENCIAS.md), identificando a opção escolhida. O runner `tests/SupermercadoApp.Integracao` usa recursos PostgreSQL e não valida esta trilha: faça os cenários acima em um banco MySQL/MariaDB separado de testes. Não aponte o runner atual para o XAMPP.

## Problemas frequentes

| Sintoma | O que conferir |
| --- | --- |
| Conexão recusada | MySQL iniciado no XAMPP, endereço e porta |
| `Access denied` | Conta, senha e host `127.0.0.1` usados no `CREATE USER` |
| Banco/tabela inexistente | Nome do banco e importação dos scripts na ordem da aula |
| Erro de TLS no teste local | String da trilha local e ausência de código Npgsql que force TLS |
| Login em maiúsculas aceito pelo banco | Versão, collation binária e restrições `CHECK` habilitadas |
| Erro com `RETURNING` ou `pg_advisory_xact_lock` | SQL PostgreSQL ainda presente; aplicar as adaptações da venda |

[Voltar à aula 07](07-Criando-Banco-Dados-Neon.md) · [Voltar à aula 08](08-Conectando-Banco-Dados.md) · [Página inicial](README.md)
