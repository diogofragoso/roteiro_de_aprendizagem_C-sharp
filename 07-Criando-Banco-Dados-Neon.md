# 6️⃣ Criando o Banco de Dados Neon e Tabelas

Para que o nosso sistema de supermercado seja seguro e profissional, os dados (como usuários e senhas) não podem ficar guardados direto no código. Precisamos de um **Banco de Dados**.

Neste guia, utilizaremos o **Neon PostgreSQL**. O Neon é um banco de dados moderno, executado 100% na nuvem (Cloud) e que possui um plano gratuito ideal para estudantes e projetos em desenvolvimento. Você não precisa instalar servidores pesados no seu computador!

## 1. Criando a conta e o Projeto no Neon

1. Acesse o site oficial: [neon.tech](https://neon.tech/)
2. Clique no botão **Sign Up** (Registrar) ou **Start for Free**. Você pode criar a conta rapidamente usando sua conta do GitHub, Google, ou um e-mail padrão.
3. Após o login, você será direcionado para criar o seu primeiro projeto.
4. **Project Name:** Digite `SupermercadoDB`.
5. **Postgres version:** Pode deixar a versão mais recente sugerida.
6. **Region:** Escolha a região mais próxima de você (ex: `US East` ou algo similar).
7. Clique em **Create Project**.

![Neon DB Dashboard](./assets/neon_postgres_dashboard_1791481203630.jpg)
*(Dashboard moderno do Neon DB mostrando informações do seu projeto)*

## 2. A String de Conexão (Connection String)

Assim que o projeto for criado, o Neon mostrará um painel inicial (Dashboard). Lá você verá um campo chamado **Connection Details** (ou Connection String).

Certifique-se de que a linguagem selecionada seja **C#** ou `.NET` (se não tiver, escolha a string padrão `postgres://...`). Uma string de conexão para .NET costuma ter o formato:
```text
Host=ep-cool-butterfly-12345.us-east-2.aws.neon.tech;Database=neondb;Username=seu_usuario;Password=SUA_SENHA_SECRETA;SSL Mode=Require;Trust Server Certificate=true;
```

> ⚠️ **Guarde essa string!** Copie e cole em um bloco de notas. É com ela que o nosso programa C# vai pedir permissão para acessar os dados na nuvem. Ela é como o "RG e Senha" do seu banco de dados.

## 3. Criando as Tabelas no Banco de Dados

Agora que temos o banco na nuvem, ele está "vazio". Precisamos criar as "tabelas" (semelhantes a abas do Excel) para guardar os tipos de usuários e os usuários em si.

No painel esquerdo do Neon, clique em **SQL Editor**. Esse editor permite executar comandos diretamente no banco.

### Passo 3.1: Tabela de Tipo de Usuário (Cargos)
Vamos criar uma tabela que define se a pessoa é um Gerente, Caixa, etc.
Cole o código abaixo no SQL Editor e clique em **Run** (Executar):

```sql
CREATE TABLE TipoUsuario (
    Id SERIAL PRIMARY KEY,
    NomeCargo VARCHAR(50) NOT NULL
);

-- Inserindo alguns cargos iniciais
INSERT INTO TipoUsuario (NomeCargo) VALUES ('Gerente');
INSERT INTO TipoUsuario (NomeCargo) VALUES ('Operador de Caixa');
```

### Passo 3.2: Tabela de Usuários
Agora vamos criar a tabela onde os usuários farão o login, com uma chave estrangeira (Foreign Key) apontando para o cargo deles.
No mesmo SQL Editor, limpe o código anterior, cole o código abaixo e clique em **Run**:

```sql
CREATE TABLE Usuario (
    Id SERIAL PRIMARY KEY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    Senha VARCHAR(50) NOT NULL,
    TipoUsuarioId INT NOT NULL,
    FOREIGN KEY (TipoUsuarioId) REFERENCES TipoUsuario(Id)
);

-- Inserindo um usuário administrador (Gerente = Id 1)
-- IMPORTANTE: Em sistemas reais, a senha NUNCA deve ser gravada em texto puro (123456).
-- Ela deve ser criptografada (Hash). Faremos em texto puro apenas para fins didáticos iniciais.
INSERT INTO Usuario (Username, Senha, TipoUsuarioId) VALUES ('admin', '123456', 1);
```

Pronto! Nosso banco de dados está online e abastecido com um usuário "admin" de senha "123456". Agora, precisamos ensinar o Visual Studio a conversar com ele!

➡️ **[Ir para o Módulo 8: Conectando o Sistema ao Banco](./08-Conectando-Banco-Dados.md)**
