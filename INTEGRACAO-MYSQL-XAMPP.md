# Trilha Alternativa — Banco Local MySQL / MariaDB pelo XAMPP (Visual Studio 2022)

Este guia orienta quem optou por rodar o banco de dados **localmente pelo XAMPP** em vez do **Neon / PostgreSQL na nuvem**. Defina a escolha antes da [Aula 11](11-Criando-Banco-Dados-Neon.md) e mantenha a mesma escolha até a Aula 24. Todos os conceitos de C#, camadas, interfaces, modelos, serviços e critérios de avaliação continuam **exatamente os mesmos** — muda apenas a camada de infraestrutura (`Banco.cs`) e os comandos SQL/conector dentro de `Repositories/`!

> [!NOTE]
> **Por que o XAMPP chama o serviço de "MySQL" se internamente ele instala MariaDB?**
> O XAMPP para Windows distribui **MariaDB** (um derivado compatível com MySQL), conforme a [FAQ oficial do XAMPP](https://www.apachefriends.org/faq_windows.html). A biblioteca **`MySqlConnector`** funciona perfeitamente com ambos. Confira a versão com `SELECT VERSION();`: use **MariaDB 10.2.1+** ou **MySQL 8.0.16+** para garantir que restrições `CHECK` sejam realmente aplicadas.

**Escopo:** a referência docente em `src/` e o runner automatizado em `tests/SupermercadoApp.Integracao` usam PostgreSQL (`Npgsql`). Mudar apenas a string de conexão não converte SQL PostgreSQL em MySQL — este guia explica o **porquê** de cada diferença e orienta você a construir as adaptações passo a passo no seu projeto no **Visual Studio 2022**.

---

## 1. Preparação do XAMPP para a Aula 11 (E os Motivos de Segurança)

1. Instale o [XAMPP para Windows](https://www.apachefriends.org/download.html) conforme orientação do professor.
2. Abra o **XAMPP Control Panel** e clique em **Start** no serviço **MySQL** (porta padrão `3306`; se outra aplicação usar `3306`, ajuste a porta com o professor e use o mesmo número na conexão C#).
3. Para abrir a interface visual de administração no navegador (**phpMyAdmin**), clique também em **Start** no **Apache** e depois em **Admin** ao lado de MySQL (`http://localhost/phpmyadmin/`).
   - **Atenção:** o seu aplicativo C# Windows Forms conversa direto com o serviço **MySQL** via TCP — ele **não** usa Apache nem PHP!
4. Na aba **SQL** do phpMyAdmin, usando a conta administrativa local, execute os comandos abaixo (substituindo a senha):

```sql
CREATE DATABASE supermercado_lab
    CHARACTER SET utf8mb4 COLLATE utf8mb4_bin;

CREATE USER 'supermercado_app'@'127.0.0.1'
    IDENTIFIED BY 'SUBSTITUA_POR_UMA_SENHA_LOCAL_FORTE';
GRANT SELECT, INSERT, UPDATE ON supermercado_lab.*
    TO 'supermercado_app'@'127.0.0.1';
```

**Por que fazemos exatamente assim?**
- **`COLLATE utf8mb4_bin`:** por padrão no MySQL/MariaDB, comparações ignoram maiúsculas e minúsculas (então `CHECK (username = LOWER(username))` não barraria `'Admin'`). A collation binária faz o banco diferenciar `'Admin'` de `'admin'`, mantendo a integridade!
- **Por que criar o usuário `supermercado_app` em vez de usar `root`?** Princípio do menor privilégio: damos apenas `SELECT, INSERT, UPDATE` (nem `DROP TABLE` nem `DELETE`!). Em máquina compartilhada, cada dupla cria seu próprio banco e usuário.
- **Segurança local:** nunca exponha o XAMPP à internet. Desativar TLS (`SslMode=Disabled`) só é aceitável em `127.0.0.1` no laboratório local; em servidores remotos, TLS com certificado validado (`VerifyFull`) é obrigatório.

---

## 2. Scripts SQL Alternativos (Importar uma Vez na Aula Correspondente)

No phpMyAdmin, selecione o banco `supermercado_lab` na barra esquerda, clique em **Importar** e execute cada arquivo **uma única vez** na aula indicada:

| Aula | Script MySQL / MariaDB | O que cria |
| --- | --- | --- |
| 11 | [database/mysql/001-usuarios.sql](database/mysql/001-usuarios.sql) | Tabela `usuario` vazia (InnoDB, `AUTO_INCREMENT`, `UNIQUE`, `CHECK`) |
| 15 | [database/mysql/002-produtos.sql](database/mysql/002-produtos.sql) | Tabelas `produto` e `movimento_estoque` com FKs |
| 20 | [database/mysql/003-vendas.sql](database/mysql/003-vendas.sql) | Tabelas `venda` e `venda_item` com `char(36)` para UUID |

> [!WARNING]
> **Diferença importante sobre DDL no MySQL/MariaDB:** comandos `CREATE TABLE` causam commit implícito no MySQL/MariaDB (diferente do PostgreSQL). Se uma importação falhar no meio, confira quais tabelas foram criadas antes de tentar novamente. Já nossas transações de dados (`ReporAsync` e `FinalizarAsync`) rodam sobre tabelas **InnoDB** e possuem **Rollback completo**.

Na Aula 11, confira a criação com:
```sql
SELECT VERSION(), DATABASE();
SELECT id, username, perfil FROM usuario;
SHOW CREATE TABLE usuario;
```
Para testar as restrições da atividade `A11.03`, crie tabelas temporárias (`CREATE TEMPORARY TABLE`) na mesma sessão SQL do phpMyAdmin testando duplicidade, `NULL`, login em maiúsculas e perfil inválido, sem alterar a tabela real `usuario`.

---

## 3. Instalando o `MySqlConnector` e Configurando `SUPERMERCADO_DB` (Aula 12)

1. No **Visual Studio 2022**, clique com o botão direito em **`SupermercadoApp`** → **Gerenciar Pacotes do NuGet...** → procure e instale **`MySqlConnector`** (versão `2.3+` estável compatível com o SDK da turma; **não** instale `MySql.Data` nem `Npgsql`).
2. Nas **Variáveis de ambiente da sua conta Windows**, crie **`SUPERMERCADO_DB`** seguindo este modelo (e reinicie o Visual Studio 2022):
   ```text
   Server=127.0.0.1;Port=3306;Database=supermercado_lab;User ID=supermercado_app;Password=SUA_SENHA_LOCAL;SslMode=Disabled;DateTimeKind=Utc;Connection Timeout=15;Default Command Timeout=30;
   ```
   - **Por que `DateTimeKind=Utc`?** Garante que o `MySqlConnector` marque as datas lidas (`datetime(6)` gravadas com `UTC_TIMESTAMP(6)`) como `DateTimeKind.Utc`, permitindo o mesmo relatório da Aula 22!
3. **Construa `Infrastructure/Banco.cs`:**
   - Em vez de `NpgsqlDataSource`, use `using MySqlConnector;` e retorne `MySqlDataSource`.
   - Leia `Environment.GetEnvironmentVariable("SUPERMERCADO_DB")`, lance `InvalidOperationException` se estiver vazia e retorne `new MySqlDataSourceBuilder(texto).Build();`.
4. **Botão de diagnóstico da Aula 12:**
   - Injete `MySqlDataSource banco` no `FormLogin`.
   - No clique assíncrono, abra a conexão com `await using var conexao = await banco.OpenConnectionAsync();`, crie `await using var comando = new MySqlCommand("SELECT 1", conexao);` e aguarde `await comando.ExecuteScalarAsync();` dentro de `try / catch / finally`.

---

## 4. Mapa Completo de Adaptações para Construir as Aulas 13 a 23

Mantenha **intactos** todos os modelos (`Models/`), contratos (`IUsuarioRepository`, `IProdutoRepository`, `IVendaRepository`), serviços (`Services/`) e telas (`Views/`). Altere **apenas** a implementação dos repositórios e o filtro de duplicidade em `Ui.cs` seguindo este guia de equivalência:

| Conceito no PostgreSQL (`Npgsql`) | Como construir no MySQL / MariaDB (`MySqlConnector`) | Por que é diferente? |
| --- | --- | --- |
| `NpgsqlDataSource` / `NpgsqlCommand` / `NpgsqlDataReader` | `MySqlDataSource` / `MySqlCommand` / `MySqlDataReader` | Tipos equivalentes da biblioteca `MySqlConnector`. |
| `banco.CreateCommand(sql)` | `await using var conexao = await banco.OpenConnectionAsync();` + `new MySqlCommand(sql, conexao)` | Abre e devolve automaticamente a conexão ao pool a cada comando fora de transação. |
| `SELECT EXISTS(SELECT 1 FROM usuario)` (cast `(bool)`) | `Convert.ToBoolean(await cmd.ExecuteScalarAsync())` | O MySQL/MariaDB pode devolver `1`/`0` numérico em vez de `bool` direto. |
| `PostgresException` (`SqlState == "23505"`) em `Ui.cs` | `catch (MySqlException erro) when (erro.Number == 1062)` | `1062` é o código oficial de *Duplicate Entry* no MySQL/MariaDB. |
| `ILIKE @busca` (Aula 16) | `LOWER(nome) LIKE LOWER(@busca) OR LOWER(codigo) LIKE LOWER(@busca)` | Como a collation do banco é binária (`utf8mb4_bin`), `LOWER(...)` garante busca insensível a maiúsculas/minúsculas. |
| `INSERT ... RETURNING id` (Aula 20) | `await cmd.ExecuteNonQueryAsync();` seguido de `int vendaId = checked((int)cmd.LastInsertedId);` | `LastInsertedId` lê o ID gerado atomicamente naquela sessão (nunca use `MAX(id)`!). |
| Tipo `uuid` (Aula 20) | Coluna `char(36)`: envie sempre **`operacao.ToString("D")`** nos parâmetros `@op` | Formata o `Guid` no padrão de 36 caracteres com hifens. |

### Adaptação Crítica da Aula 20 — Idempotência Concorrente com `GET_LOCK` / `RELEASE_LOCK`
Como `pg_advisory_xact_lock` é exclusivo do PostgreSQL, para proteger duas chamadas simultâneas da **mesma chave `Guid`** no MySQL/MariaDB construa `VendaRepository.FinalizarAsync` seguindo exatamente este protocolo:

1. Abra uma conexão dedicada (`await using var conexao = await banco.OpenConnectionAsync();`).
2. Monte uma chave determinística de até 64 caracteres por banco e operação: `string chave = $"{conexao.Database}:{operacao:N}";`.
3. **Antes** de iniciar a transação, adquira o bloqueio nomeado de sessão executando `SELECT GET_LOCK(@chave, 10)` (timeout de 10 segundos). Converta o retorno para inteiro e **só prossiga se for exatamente `1`** (se for `0` ou `NULL`, lance exceção interrompendo a tentativa).
4. Envolva o restante da operação em um **`try / finally`**:
   - **No `try`:** inicie `BeginTransactionAsync()`, verifique idempotência (`WHERE operacao = @op AND usuario_id = @usuario` com `operacao.ToString("D")`), bloqueie os produtos em ordem crescente com `SELECT ... FOR UPDATE`, valide estoque/total, insira a venda lendo `checked((int)cmd.LastInsertedId)`, grave itens/estoque/movimentos na mesma `(conexao, transacao)` e chame `await transacao.CommitAsync();`.
   - **No `finally` (OBRIGATÓRIO!):** **Por quê?** Diferente do PostgreSQL, `GET_LOCK` pertence à sessão e **não** é liberado pelo `COMMIT` nem pelo `ROLLBACK`! Portanto, após encerrar a transação, execute **`SELECT RELEASE_LOCK(@chave)` na mesma conexão** (inclusive quando a venda já existia!) e confira retorno `1`. Se a liberação falhar por queda da conexão, limpe o pool com `MySqlConnection.ClearPool(conexao)` para não devolver uma conexão travada ao pool!

---

## 5. Diagnóstico Rápido de Problemas Frequentes no XAMPP

| Sintoma no Visual Studio 2022 | O que verificar primeiro |
| --- | --- |
| Conexão recusada (`Unable to connect`) | Verifique no XAMPP Control Panel se o serviço **MySQL** está verde (*Running*) e se a porta (`3306`) coincide com `SUPERMERCADO_DB`. |
| `Access denied for user` | Confira usuário, senha e se o host no `CREATE USER` e na string de conexão é `127.0.0.1`. |
| Login em maiúsculas sendo aceito no banco | Confira `SELECT VERSION();`, `COLLATE utf8mb4_bin` e se `SHOW CREATE TABLE usuario` mostra as restrições `CHECK`. |
| Erro de sintaxe em `RETURNING` ou `pg_advisory_xact_lock` | Restou algum trecho de SQL PostgreSQL no seu `VendaRepository.cs`; aplique as substituições da Seção 4. |

Fontes oficiais: [MySqlConnector API Básica](https://mysqlconnector.net/tutorials/basic-api/), [Opções de Conexão](https://mysqlconnector.net/connection-options/) e [GET_LOCK no MariaDB](https://mariadb.com/docs/server/reference/sql-functions/secondary-functions/miscellaneous-functions/get_lock).

[Voltar à Aula 11](11-Criando-Banco-Dados-Neon.md) · [Voltar à Aula 12](12-Conectando-Banco-Dados.md) · [Índice Geral](README.md)
