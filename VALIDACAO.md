# Validação da entrega

Data: 16/10/2026. Ambiente local: Windows x64, SDK .NET 8.0.418, runtime .NET 8 instalado. Este registro distingue verificações executadas de testes que dependem do laboratório.

## Executado

| Verificação | Resultado |
| --- | --- |
| Capítulos numerados de 01 a 20 | 20 capítulos, sem lacunas |
| Tempo por capítulo | Cada tabela soma 210 minutos; 4.200 minutos no total |
| Links Markdown locais | Destinos existentes |
| Delimitadores de código nos capítulos | Sem blocos abertos |
| `git diff --check` | Sem erros de espaços ou quebras de linha |
| `dotnet build Supermercado.sln -c Release` | Sucesso, zero avisos e zero erros |
| `dotnet run --project tests/SupermercadoApp.Verificacoes -c Release --no-build` | 35 verificações aprovadas |
| Código do login com Designer da aula 09 | Compilado com Designer mínimo, zero avisos e zero erros |
| Construção das janelas | Login, administrador, caixa, produtos, relatório e usuários sem exceção no teste local |
| `dotnet publish src/SupermercadoApp/SupermercadoApp.csproj -c Release -r win-x64 --self-contained true -o artifacts/publish` | Pasta autocontida gerada com sucesso |

As verificações automáticas cobrem cálculo e alteração do carrinho, validações de cadastro/reposição, verificação de hash, separação de perfis, parâmetros de pedido e período de relatório. Utilizam repositórios em memória. O teste das janelas constrói os controles; não opera cliques, rede ou banco.

## Dependente do laboratório

Ainda não executados nesta entrega: criação remota de projeto/branch Neon, execução dos scripts em PostgreSQL, login e CRUD com conexão real, transações e concorrência contra o banco, falha de rede no intervalo após commit e uso da pasta publicada por um estudante fora da IDE.

Esses casos têm preparação, ações e resultados esperados em [VALIDACAO-MANUAL.md](VALIDACAO-MANUAL.md). Nenhum resultado de integração remota é inferido da compilação ou dos fakes.

A execução da publicação gerou arquivos em `artifacts/publish`, ignorados pelo Git. Não foi feita distribuição externa. Os arquivos preexistentes `package-lock.json` e `.vscode` não fazem parte da implementação didática e foram preservados.
