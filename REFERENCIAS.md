# Referências técnicas para o professor

Referências iniciais em 16/10/2026; suporte, SDK, IDE e versões NuGet conferidos na revisão de 16/11/2026. Os textos do roteiro e a implementação são adaptações didáticas; a documentação oficial pode evoluir.

- [Compatibilidade entre SDK e Visual Studio](https://learn.microsoft.com/en-us/dotnet/core/porting/versioning-sdk-msbuild-vs): consulte antes de alterar o framework da turma.
- [Suporte do .NET](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core): datas de manutenção e atualização do laboratório.
- [Npgsql: uso básico](https://www.npgsql.org/doc/basic-usage.html): fonte de dados reutilizável, parâmetros, chamadas assíncronas e transações.
- [Npgsql: segurança e criptografia](https://www.npgsql.org/doc/security.html): verificação de certificado TLS.
- [Neon com .NET/Npgsql](https://neon.com/docs/guides/dotnet-npgsql): configuração da conexão do cliente .NET.
- [PasswordHasher<TUser>](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.passwordhasher-1?view=aspnetcore-10.0): criação e verificação de hashes com implementação mantida pela Microsoft.
- [Download .NET 10](https://dotnet.microsoft.com/en-us/download/dotnet/10.0): SDK 10.0.404 e Visual Studio 2026 18.11 usados nesta revisão.
- [Npgsql no NuGet](https://www.nuget.org/packages/Npgsql/10.0.3) e [Identity.Core no NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Identity.Core/10.0.15): pacotes fixados e restaurados na migração.
- [Bloqueios do PostgreSQL](https://www.postgresql.org/docs/current/explicit-locking.html): bloqueios de linha e advisory locks usados na aula de concorrência.
- [Publicar aplicativos .NET](https://learn.microsoft.com/en-us/dotnet/core/deploying/): diferenças entre publicação dependente de runtime e autocontida.

Evite apresentar hash como criptografia reversível. O `PasswordHasher` gera um formato com parâmetros e salt; o aplicativo verifica a senha com a biblioteca. Não implemente seu próprio esquema de senhas e não use MD5.

## Alternativa MySQL/MariaDB com XAMPP

- [XAMPP: FAQ para Windows](https://www.apachefriends.org/faq_windows.html): serviço MySQL do painel, distribuição MariaDB e uso local.
- [MySqlConnector: API básica](https://mysqlconnector.net/tutorials/basic-api/): conexões, parâmetros e consultas assíncronas.
- [MySqlConnector: opções de conexão](https://mysqlconnector.net/connection-options/): porta, TLS, pool e DateTimeKind.
- [MySqlConnector: MySqlDataSourceBuilder](https://mysqlconnector.net/api/mysqlconnector/mysqldatasourcebuildertype/): construção da fonte de dados.
- [MySqlConnector: LastInsertedId](https://mysqlconnector.net/api/mysqlconnector/mysqlcommand/lastinsertedid/): obtenção do identificador após INSERT.
- [MariaDB: restrições](https://mariadb.com/docs/server/reference/sql-statements/data-definition/constraint): chaves e CHECK.
- [MariaDB: GET_LOCK](https://mariadb.com/docs/server/reference/sql-functions/secondary-functions/miscellaneous-functions/get_lock): bloqueio de sessão e necessidade de liberação explícita.