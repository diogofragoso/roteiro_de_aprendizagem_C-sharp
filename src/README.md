# Implementação de referência

Este é o resultado final do roteiro, em **C# / .NET 10 / Windows Forms**, para **Visual Studio 2026**. Não há arquivos de credenciais e nenhuma tabela é criada automaticamente ao abrir o programa.

Para estudantes que escolherem MySQL/MariaDB pelo XAMPP, consulte a [trilha alternativa](../INTEGRACAO-MYSQL-XAMPP.md). Esta implementação usa Npgsql e SQL PostgreSQL; siga as adaptações do complemento no projeto do estudante antes de conectar ao banco local.

## Abrir e executar

1. Abra [Supermercado.sln](../Supermercado.sln) no VS 2026 18.11 atualizado, com desenvolvimento desktop .NET e SDK 10.0.4xx. O `global.json` seleciona SDK 10.0.4xx para manter a reprodução no VS 2026; instale essa faixa se o terminal informar que não encontrou SDK compatível. O console de verificações não deve ser o projeto de inicialização; selecione **SupermercadoApp**.
2. Restaure os pacotes NuGet. O projeto fixa Npgsql 10.0.3 e Identity.Core 10.0.15. Confira atualizações compatíveis e vulnerabilidades antes de uma nova turma.
3. No ambiente Neon **vazio e separado para laboratório**, execute em ordem e uma única vez [001](../database/001-usuarios.sql), [002](../database/002-produtos.sql) e [003](../database/003-vendas.sql). Na sequência didática, cada um entra na aula correspondente; para executar a referência final agora, todos são necessários.
4. Nas variáveis de ambiente da sua conta Windows, configure `SUPERMERCADO_DB` com uma conexão Npgsql, sem publicá-la. Reabra a IDE.
5. Em propriedades do projeto → Depurar → perfil de inicialização, acrescente o argumento `--preparar-admin`. Execute, informe o nome e uma senha de 12 a 128 caracteres. Isso só funciona com a tabela de usuários vazia.
6. Pare o programa, remova o argumento e execute de novo. Entre com o administrador criado. Pelo menu de usuários, crie uma conta com perfil Caixa.
7. Cadastre produtos, selecione-os e registre reposições. Saia, entre como caixa, monte o carrinho e finalize com dinheiro simulado.

Modelo da variável, usando os valores fornecidos pelo painel Connect do Neon:

```text
Host=SEU_HOST;Database=SEU_BANCO;Username=SEU_ROLE;Password=SUA_SENHA;SSL Mode=VerifyFull;
```

Não use uma URI `postgresql://` nesse campo. Credenciais com caracteres especiais precisam ser representadas conforme o formato de conexão Npgsql; use os campos .NET fornecidos pelo painel. Não desative a verificação TLS para fazer o teste passar.

## Estrutura e localização por aula

As [versões de recuperação](../ensino/README.md) são geradas em `artifacts/checkpoints/00` a `20`. Cada uma contém apenas os membros da etapa e um inventário; início da aula N é a versão N−1. A referência final continua servindo à comparação de nomes e implementações.

| Aula | Arquivos/métodos apresentados |
| --- | --- |
| 05–06 | Projeto e login pelo Designer do estudante |
| 08 | Infrastructure/Banco.cs |
| 09 | Models/Perfil, Credencial, Sessao; IUsuarioRepository, UsuarioRepository; AutenticacaoService; FormLogin |
| 10 | Permissoes; fluxo de sessão no Program; FormAdministrador e FormCaixa mínimos |
| 11 | Produto; IProdutoRepository.SalvarAsync; ProdutoService.SalvarAsync; script 002 |
| 12 | ProdutoRepository.Ler/ListarAsync/SalvarAsync; ProdutoService.ListarAsync; FormProdutos mínimo |
| 13 | ProdutoRepository/Service.InativarAsync; campos e ações do FormProdutos |
| 14 | ProdutoRepository/Service.ReporAsync; controles de reposição |
| 15 | ItemCarrinho, Carrinho; BuscarCodigoAsync; FormCaixa sem pagamento |
| 16 | PedidoItem, VendaConfirmada, ResumoVenda; IVendaRepository; VendaRepository; VendaService; script 003 |
| 17 | Pagamento e estados de FormCaixa |
| 18 | RelatorioAsync e FormRelatorio |
| 19 | FormUsuarios e menu administrativo completo |
| 20 | Projeto de verificações e publicação |

Os formulários da referência criam o layout em código por meio de `Views/Ui.cs`, para que todos os arquivos necessários estejam no repositório. O login das aulas 05/06 ensina o Designer. Se mantiver seu Designer, não cole o construtor de layout completo: preserve `InitializeComponent` e adapte os eventos, campos e dependências conforme as aulas 08 e 09. Não mantenha dois eventos de clique para a mesma ação.

Em uma etapa intermediária, declare somente os tipos e métodos daquela aula. O código final inclui todos os recursos; não é necessário adicionar assinaturas de aulas futuras a um projeto inicial. A [orientação pedagógica](../PLANO-DE-AULAS.md) explica essa construção gradual.

## Comandos na raiz do repositório

```powershell
dotnet restore Supermercado.sln
dotnet build Supermercado.sln -c Release --no-restore
dotnet run --project tests/SupermercadoApp.Verificacoes -c Release
dotnet run --project src/SupermercadoApp -- --preparar-admin
dotnet run --project src/SupermercadoApp
```

Não execute a preparação toda vez. Depois que existe um usuário, ela deve recusar a execução. A senha é informada por campo oculto, não por argumentos de linha de comando.

Para o professor preparar uma conta Caixa nas etapas anteriores à aula 19, use `dotnet run --project src/SupermercadoApp -- --preparar-caixa`. A aplicação exige login administrativo e abre o formulário de criação; remova o argumento ao retomar a aula. Não há credencial ou acesso padrão.

## Verificações de integração isoladas

O projeto `tests/SupermercadoApp.Integracao` só conecta com `SUPERMERCADO_TEST_DB` e argumento explícito `--executar`. Configure a variável localmente em um banco didático com permissão de criar esquemas, usando conexão direta. Não publique seu valor. Execute na raiz:

```powershell
dotnet run --project tests/SupermercadoApp.Integracao -c Release -- --executar
```

Cada execução cria um esquema único `lab_<guid>`, aplica os três scripts nesse esquema, testa último item concorrente, repetição da mesma operação, rollback após escrita, histórico e fronteiras do relatório; ao terminar, remove somente esse esquema. O runner não usa `SUPERMERCADO_DB`. Sem configuração ou argumento, sai com código 2 e estado pendente, sem conexão. Falha encerra com código 1. Se o processo for interrompido à força, o esquema pode permanecer: confira o identificador no banco antes de uma limpeza manual. Não execute essa suíte em banco de operação real.

Compilar o runner ou executar sua proteção sem conexão não comprova que os testes PostgreSQL passaram. Registre sua execução remota separadamente, como no [roteiro manual](../VALIDACAO-MANUAL.md).

## Limites intencionais

O projeto é online, com quantidade inteira, dinheiro simulado e comprovante sem valor fiscal. A tentativa pendente sobrevive a novas chamadas enquanto a janela fica aberta, mas não ao encerramento forçado do processo. O Guid precisa ser consultado antes de reiniciar uma compra de resultado incerto. A arquitetura comercial deve incluir API, autorização no servidor e reconciliação durável, como descrito nas aulas 04 e 17.

A janela de produtos mostra até 200 resultados; o relatório carrega o período completo. Esses limites atendem à massa pequena de laboratório. Atualizações concorrentes de descrição/preço seguem o último salvamento confirmado; controle otimista de edição é um aprimoramento posterior. Já estoque e venda usam atualizações atômicas/bloqueios e transações.

Não há conta Neon, migração remota ou venda real executada automaticamente pelos comandos de verificação. Consulte [VALIDACAO-MANUAL.md](../VALIDACAO-MANUAL.md) para o que precisa de banco configurado.
