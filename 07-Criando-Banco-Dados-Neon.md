# Aula 07 — Banco Neon e usuários

**Duração:** 3h30. **Pré-requisito:** aula 06, internet e conta de laboratório. **Entrega:** banco com a tabela `usuario` criada, sem contas com senha aberta.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Diferenciar campos de tela e persistência |
| Explicação | 30 | Tabela, chave, restrições e credenciais |
| Construção | 60 | Criar projeto/branch e executar script 001 |
| Intervalo | 15 | Pausa |
| Prática | 60 | Consultar estrutura e conferir conexão |
| Entrega | 30 | Explicar o esquema e identificar a branch correta |

## 1. Prepare um ambiente só da turma

1. Entre no [console do Neon](https://console.neon.tech/). Os nomes dos botões podem mudar; procure a criação de projeto.
2. Crie um projeto `SupermercadoDB`, escolha uma região disponível adequada ao laboratório e anote o nome do banco e da branch.
3. Cada estudante/dupla deve usar um ambiente separado. Se o professor usar um projeto compartilhado, distribua branches/bancos diferentes; executar os mesmos scripts e testes no mesmo esquema provoca conflitos.
4. Confira os limites e as condições do plano usado antes da turma. Não presuma gratuidade ou cotas ilimitadas.
5. Abra o **SQL Editor** e confira banco e branch antes de executar qualquer comando.

![Exemplo ilustrativo do painel Neon](assets/neon_postgres_dashboard_1791481203630.jpg)

**Por quê?** Uma branch de laboratório permite errar sem atingir outro grupo. Uma branch derivada de um banco existente pode copiar o esquema; para esta sequência, escolha um banco vazio. Quem fez o material antigo deve ler a [transição](TRANSICAO.md).

## 2. Execute o primeiro script

Abra [database/001-usuarios.sql](database/001-usuarios.sql), copie seu conteúdo inteiro para o SQL Editor e execute **uma vez**. Ele contém:

```sql
CREATE TABLE usuario (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    username varchar(50) NOT NULL UNIQUE CHECK (username = lower(username)),
    senha_hash text NOT NULL,
    perfil varchar(20) NOT NULL CHECK (perfil IN ('Administrador', 'Caixa'))
);
```

O arquivo também envolve a criação em `BEGIN` e `COMMIT`. Se houver erro e a sessão ficar em transação abortada, execute `ROLLBACK` antes de corrigir e tentar novamente.

- `id` identifica o registro, independentemente do nome de login.
- `IDENTITY` faz o banco gerar o número; não suponha que o próximo será sempre o anterior mais um.
- `UNIQUE` impede dois logins iguais, inclusive se duas pessoas tentarem cadastrar ao mesmo tempo.
- `NOT NULL` impede ausência de dado; não equivale a validar texto vazio.
- `CHECK` restringe os perfis e exige nome em minúsculas.
- `senha_hash` guarda a representação produzida por uma biblioteca. Não execute um `INSERT` com senha aberta nessa coluna.

Usamos um conjunto pequeno de perfis em vez de uma tabela separada de cargos. Isso evita dependência em IDs mágicos como “gerente = 1” e combina com o `enum Perfil` do C#.

| Coluna | Tipo | Significado e integridade |
| --- | --- | --- |
| id | integer identity | Identificador interno e chave primária; não é o login. |
| username | varchar(50) | Login obrigatório, único e em minúsculas. |
| senha_hash | text | Representação produzida pela biblioteca; nunca senha aberta. |
| perfil | varchar(20) | Administrador ou Caixa; CHECK restringe o conjunto. |

Use a tabela como início do seu dicionário; acrescente exemplos fictícios sem incluir hash ou senha nas evidências.

Execute:

```sql
SELECT id, username, perfil FROM usuario;
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_schema = 'public' AND table_name = 'usuario';
```

A primeira consulta retorna **zero usuários**, o que é correto. O administrador será criado pelo aplicativo na aula 09. O script não deve ser reaplicado como se fosse uma consulta: se a tabela já existe, confira o ambiente e o histórico em vez de apagá-la.

## 3. Prepare os dados de conexão

### Exercite as restrições sem criar contas inválidas

Use [o laboratório de restrições](database/laboratorio/07-restricoes.sql) em uma sessão com conexão direta. Ele usa tabelas temporárias e testa duplicidade, ausência, maiúsculas e perfil inválido sem tocar em `usuario`. Execute o arquivo inteiro para manter a mesma sessão; tabelas temporárias não são compartilhadas entre conexões.

Entregue um dicionário com coluna, tipo, significado e restrição. Diferencie `NULL` de texto vazio: `NOT NULL` sozinho permite `''`. O resultado de cada violação deve apontar a restrição correspondente. Ao final, compare inserção revertida e confirmada; não espere sequência de IDs sem lacunas.

No painel **Connect**, selecione **.NET/C#**, branch, banco e role. Copie os campos adequados ao Npgsql. Uma URI `postgresql://...` não deve ser colada diretamente no construtor que espera os pares abaixo:

```text
Host=SEU_HOST;Database=SEU_BANCO;Username=SEU_ROLE;Password=SUA_SENHA;SSL Mode=VerifyFull;
```

O hostname deve ser exatamente o fornecido pelo Neon. `VerifyFull` solicita criptografia e validação da identidade do servidor. Não use `Trust Server Certificate=true` para contornar erros. Confirme rede, data do Windows, hostname e cadeia de certificados.

Guarde a conexão apenas no ambiente local conforme a aula 08. A senha do **role do PostgreSQL** é diferente da senha de um **usuário do supermercado**. Não inclua credenciais em Git, capturas ou entregas.

## Verifique antes de encerrar

- `usuario` existe e contém `senha_hash`, sem coluna de senha aberta.
- A consulta retorna zero contas; não inventamos uma senha padrão.
- Você explica chave primária, unicidade e a diferença entre role e perfil do aplicativo.
- Registrou nome da branch e resultado das consultas, sem credenciais.

Fontes: [conexão Neon/.NET](https://neon.com/docs/guides/dotnet-npgsql) e [TLS no Npgsql](https://www.npgsql.org/doc/security.html).

[Anterior: aula 06](06-Criando-Interface-Login.md) · [Próxima: aula 08](08-Conectando-Banco-Dados.md)

## Atividades por conceito

Realize as atividades da [aula 07 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-07). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
