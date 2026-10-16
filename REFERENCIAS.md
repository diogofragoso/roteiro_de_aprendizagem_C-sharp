# Referências técnicas para o professor

Consulta realizada em 16/10/2026. Os textos do roteiro e a implementação são adaptações didáticas; a documentação oficial pode evoluir.

- [Compatibilidade entre SDK e Visual Studio](https://learn.microsoft.com/en-us/dotnet/core/porting/versioning-sdk-msbuild-vs): consulte antes de alterar o framework da turma.
- [Suporte do .NET](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core): datas de manutenção e atualização do laboratório.
- [Npgsql: uso básico](https://www.npgsql.org/doc/basic-usage.html): fonte de dados reutilizável, parâmetros, chamadas assíncronas e transações.
- [Npgsql: segurança e criptografia](https://www.npgsql.org/doc/security.html): verificação de certificado TLS.
- [Neon com .NET/Npgsql](https://neon.com/docs/guides/dotnet-npgsql): configuração da conexão do cliente .NET.
- [PasswordHasher<TUser>](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.passwordhasher-1?view=aspnetcore-8.0): criação e verificação de hashes com implementação mantida pela Microsoft.
- [Bloqueios do PostgreSQL](https://www.postgresql.org/docs/current/explicit-locking.html): bloqueios de linha e advisory locks usados na aula de concorrência.
- [Publicar aplicativos .NET](https://learn.microsoft.com/en-us/dotnet/core/deploying/): diferenças entre publicação dependente de runtime e autocontida.

Evite apresentar hash como criptografia reversível. O `PasswordHasher` gera um formato com parâmetros e salt; o aplicativo verifica a senha com a biblioteca. Não implemente seu próprio esquema de senhas e não use MD5.
