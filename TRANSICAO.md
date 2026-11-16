# Transição para quem concluiu os capítulos antigos

A versão anterior terminava no login e usava tabelas `TipoUsuario` e `Usuario`, cargo “Gerente”, senha aberta e conexão no repositório. A continuação usa um único vocabulário: **Administrador** e **Caixa**, tabelas SQL em minúsculas e `senha_hash`.

Faça esta preparação antes de iniciar a aula 09. Ela ocupa as aulas 07 e 08 de recuperação; não deve ser somada à aula 09 como trabalho obrigatório extra.

Nesta edição o ambiente também passou para .NET 10 LTS, SDK 10.0.404 e Visual Studio 2026 18.11. Atualize a IDE e instale o SDK antes de abrir a solução; .NET 8/VS 2022 não são o ambiente de execução desta edição. Os projetos usam `net10.0-windows`, Npgsql 10.0.3 e Identity.Core 10.0.15. Consulte a aula 05 e execute as verificações após restaurar os pacotes. Os checkpoints podem ser gerados pelo [apoio ao professor](ensino/README.md).

1. Guarde uma cópia do código antigo. Remova credenciais de arquivos versionados e troque no Neon qualquer senha de banco que tenha sido publicada. Excluir uma linha de código não retira o segredo do histórico do Git.
2. Crie no Neon uma **branch didática separada** para a turma. Confira nome da branch, banco e usuário antes de executar SQL. A tabela antiga `Usuario` sem aspas vira `usuario` no PostgreSQL e conflita com o novo nome. Uma branch derivada com o esquema antigo ainda contém essa tabela: use um banco vazio nessa branch ou uma branch com esquema vazio, conforme as opções do painel.
3. Siga a aula 07 no banco vazio. Não execute `DROP TABLE` para “resolver” o conflito e não rode os scripts novos em dados reais. Esta transição é para dados descartáveis de laboratório; dados que precisem ser preservados exigem migração própria e validação antes da execução.
4. Adote o ambiente e os pacotes das aulas 05 e 08. Passe a conexão pela variável `SUPERMERCADO_DB`; tire a string do repositório.
5. Substitua o modelo antigo `Usuario` pelos tipos `Perfil`, `Credencial` e `Sessao` da aula 09. O nome `Username` permanece para reduzir mudanças; a senha aberta deixa de fazer parte do modelo retornado para a tela.
6. Substitua `FazerLogin` pelo fluxo `FormLogin → AutenticacaoService → IUsuarioRepository`. O repositório busca pelo nome; o serviço verifica o hash. Não mantenha a condição SQL `Senha = @senha`.
7. Preserve seu desenho da tela da aula 06: mantenha `FormLogin.Designer.cs`, `InitializeComponent`, `txtUsuario`, `txtSenha` e `btnLogin`. Adapte o evento explicado na aula 09. A tela da referência é construída em código e usa os mesmos conceitos, mas seu construtor completo não deve ser colado junto com o Designer.
8. Prepare o primeiro administrador conforme a aula 09. O usuário e a senha antigos não são reaproveitados automaticamente. Na aula 10 prepare um caixa de teste conforme a orientação do professor; na aula 19 o cadastro será feito pela interface.

Ao concluir, registre: branch utilizada, login correto e incorreto, conexão fora do código e ausência de senha em texto puro. Não registre a senha nem a string de conexão no caderno.
