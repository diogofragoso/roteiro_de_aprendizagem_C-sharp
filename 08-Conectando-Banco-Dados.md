# Aula 08 — Conexão assíncrona com o banco

**Opção MySQL/MariaDB:** se você escolheu XAMPP, use as etapas 3 a 5 da [trilha alternativa](INTEGRACAO-MYSQL-XAMPP.md) no lugar do Npgsql e do Banco.cs abaixo. O diagnóstico e os critérios de entrega continuam iguais.

**Duração:** 3h30. **Pré-requisito:** aula 07 e login visual. **Entrega:** botão de diagnóstico que executa `SELECT 1` sem bloquear a interface, sem credenciais no código.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Conferir banco/branch e projeto .NET 10 |
| Explicação | 30 | NuGet, conexão, await e descarte |
| Construção | 60 | Instalar pacote, configurar variável e Banco |
| Intervalo | 15 | Pausa |
| Prática | 60 | Criar botão de diagnóstico e tratar falha |
| Entrega | 30 | Demonstrar conexão normal e configuração ausente |

## 1. Instale o Npgsql

1. No Gerenciador de Soluções, botão direito em `SupermercadoApp` → **Gerenciar Pacotes NuGet**.
2. Abra **Procurar**, selecione a origem `nuget.org` e pesquise `Npgsql`.
3. Confira o pacote **Npgsql** do projeto Npgsql, selecione **10.0.3** e instale.
4. Em **Dependências → Pacotes**, confira a referência. É uma biblioteca do projeto; não é uma extensão da IDE.
5. Compile. Se o laboratório bloquear o NuGet, o professor precisa liberar a origem ou preparar o cache antes da aula.

Fixamos versões para reproduzir o material; confira atualizações compatíveis e alertas de vulnerabilidade antes de uma nova turma.

## 2. Configure a conexão fora do código

No Windows, pesquise **Editar as variáveis de ambiente da sua conta**. Em **Variáveis do usuário**, crie `SUPERMERCADO_DB` com a string Npgsql da aula 07. Feche e reabra o Visual Studio: processos já abertos não recebem automaticamente a variável nova.

Para um teste somente no terminal PowerShell atual, use:

```powershell
$env:SUPERMERCADO_DB = 'Host=SEU_HOST;Database=SEU_BANCO;Username=SEU_ROLE;Password=SUA_SENHA;SSL Mode=VerifyFull;'
```

Essa linha é um **modelo**. Prefira a interface do Windows para inserir o valor real e não registrar a senha no histórico de comandos. Variável de ambiente evita versionar a credencial; não a torna inacessível a outras pessoas com acesso à mesma conta Windows.

## 3. Crie a infraestrutura

Na pasta `Infrastructure`, **Adicionar → Classe → Banco.cs**. Use o conteúdo de [Banco.cs](src/SupermercadoApp/Infrastructure/Banco.cs). Ele lê a variável, rejeita configuração ausente, configura TLS e cria um `NpgsqlDataSource`.

**Por quê?** A fonte de dados é criada uma vez e reutiliza um pool de conexões. Uma conexão aberta para cada operação é devolvida ao pool quando descartada. Não guarde uma conexão aberta global nem recrie o pool a cada clique. Veja o [uso oficial do Npgsql](https://www.npgsql.org/doc/basic-usage.html).

## 4. Acrescente o diagnóstico temporário

No Designer do login, adicione um botão `btnTestarConexao`, texto “Testar conexão”. Em `FormLogin.cs`, adicione os `using` de Npgsql e Infrastructure e um campo:

```csharp
private readonly NpgsqlDataSource banco;
```

Mude o construtor existente para receber a fonte de dados, mantendo o Designer:

```csharp
public FormLogin(NpgsqlDataSource banco)
{
    InitializeComponent();
    this.banco = banco;
}
```

No `Program.cs`, após `ApplicationConfiguration.Initialize()`, substitua a linha que abria o login por:

```csharp
try
{
    using var banco = SupermercadoApp.Infrastructure.Banco.Criar();
    Application.Run(new SupermercadoApp.Views.FormLogin(banco));
}
catch (InvalidOperationException erro)
{
    MessageBox.Show(erro.Message, "Configuração");
}
```

Duplo clique no botão de diagnóstico e use:

```csharp
private async void btnTestarConexao_Click(object sender, EventArgs e)
{
    btnTestarConexao.Enabled = false;
    try
    {
        await using var comando = banco.CreateCommand("SELECT 1");
        var resultado = await comando.ExecuteScalarAsync();
        MessageBox.Show($"Conexão confirmada: {resultado}");
    }
    catch (Exception)
    {
        MessageBox.Show("Não foi possível consultar. Verifique rede e configuração.");
    }
    finally
    {
        btnTestarConexao.Enabled = true;
    }
}
```

`async void` é reservado aqui ao evento exigido pelo Windows Forms. Outros métodos assíncronos retornarão `Task` para que o chamador consiga aguardar e tratar falhas. `await` entrega o controle à interface enquanto a consulta aguarda a rede; não use `.Result` nem `.Wait()`. `await using` descarta recursos inclusive quando ocorre uma exceção.

Não mostre `erro.ToString()` ao usuário: detalhes técnicos podem revelar infraestrutura. Para investigar no laboratório, use o depurador sem publicar valores de credenciais.

## Verifique antes de encerrar

Antes de executar, trace três caminhos: sucesso passa pelo `try` e pelo `finally`; falha na consulta passa pelo `catch` e pelo `finally`; configuração ausente falha ao criar Banco, antes do clique. Em cada caminho, identifique quais recursos já existem e precisam de descarte. O `finally` reabilita o botão mesmo quando a operação falha.

Para observar a tela respondendo sem depender da velocidade da rede, acrescente temporariamente `await Task.Delay(2000);` antes da consulta e mova a janela durante a espera. Retire a linha ao concluir. Esse ensaio comprova o comportamento da espera assíncrona; registre separadamente a execução real de `SELECT 1` e o teste de indisponibilidade.

1. Com a conexão correta, o botão mostra `1` e volta a ficar habilitado.
2. Sem a variável, a aplicação explica a configuração ausente. Restaure a variável e reabra a IDE.
3. Com rede indisponível, a mensagem não expõe a conexão nem a senha. A tela volta a responder após o timeout.
4. Enquanto aguarda, a janela pode ser movida. O botão não permite uma segunda consulta simultânea.
5. Não registre os valores secretos no caderno.

O teste de conexão não autentica usuários. Na aula 09 removeremos esse botão e o banco deixará de ser dependência direta da tela.

[Anterior: aula 07](07-Criando-Banco-Dados-Neon.md) · [Próxima: aula 09](09-Autenticacao-Segura.md)

## Atividades por conceito

Realize as atividades da [aula 08 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-08). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
