# Transição para Quem Concluiu uma Versão Antiga do Roteiro

A versão histórica antiga terminava no login e usava tabelas `TipoUsuario` e `Usuario`, cargo "Gerente", senha aberta e conexão dentro do repositório. A trilha unificada atual (**25 aulas** no **Visual Studio 2022**) usa um único vocabulário (**Administrador** e **Caixa**), tabelas SQL em minúsculas e `senha_hash`.

Faça esta preparação antes de retomar na [Aula 14](14-Autenticacao-Segura.md) (ela equivale à recuperação das Aulas 12 e 13).

1. Guarde uma cópia do código antigo. Remova credenciais de arquivos versionados e troque no Neon qualquer senha de banco que tenha sido publicada. Excluir uma linha de código não retira o segredo do histórico do Git.
2. Crie no Neon uma **branch didática separada** para a turma (ou um banco local limpo no XAMPP). Confira nome da branch, banco e usuário antes de executar SQL. A tabela antiga `Usuario` sem aspas vira `usuario` no PostgreSQL e conflita com o novo nome.
3. Siga a [Aula 12](12-Criando-Banco-Dados-Neon.md) no banco vazio. Não execute `DROP TABLE` em dados reais.
4. Adote a estrutura de projeto da [Aula 09](09-Configuracao-Ambiente.md) e a conexão via variável `SUPERMERCADO_DB` da [Aula 13](13-Conectando-Banco-Dados.md), retirando qualquer string de conexão do repositório.
5. Substitua o modelo antigo `Usuario` pelos tipos `Perfil`, `Credencial` e `Sessao` da [Aula 14](14-Autenticacao-Segura.md). A senha aberta deixa de existir no modelo retornado para a tela.
6. Substitua `FazerLogin` pelo fluxo `FormLogin → AutenticacaoService → IUsuarioRepository`. O repositório busca pelo `username`; o serviço verifica o hash com `PasswordHasher`. Nunca mantenha `WHERE Senha = @senha`.
7. Preserve o desenho do Designer da [Aula 11](11-Criando-Interface-Login.md): mantenha `FormLogin.Designer.cs`, `InitializeComponent`, `txtUsuario`, `txtSenha` e `btnLogin`, adaptando o construtor e o evento conforme a [Aula 14](14-Autenticacao-Segura.md).
8. Prepare o primeiro administrador conforme a [Aula 14](14-Autenticacao-Segura.md) (`--preparar-admin`). Na [Aula 15](15-Sessao-e-Perfis.md) prepare uma conta `Caixa` de teste; na [Aula 24](24-Cadastro-de-Usuarios.md) você construirá a interface definitiva de cadastro de usuários.

Ao concluir, registre: branch utilizada, login correto e incorreto, conexão fora do código e ausência de senha em texto puro. Não registre a senha nem a string de conexão no caderno.
