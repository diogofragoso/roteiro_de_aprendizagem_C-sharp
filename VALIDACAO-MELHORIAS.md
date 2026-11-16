# Validação da implementação das melhorias

Data: 16/11/2026. Ambiente: Windows x64; SDK **10.0.404** extraído localmente em `artifacts/sdk`, sem substituir a instalação global. O ambiente da turma é .NET 10 LTS com Visual Studio 2026 18.11; a IDE não foi instalada nem operada nesta revisão.

## Executado

| Verificação | Resultado |
| --- | --- |
| Documentação | 114 IDs únicos em 20 aulas; links locais existentes, âncoras de aula presentes e blocos de código fechados. |
| Carga horária | 210 minutos por capítulo; 4.200 minutos no total, mantendo o intervalo. |
| Geração dos checkpoints | 21 projetos independentes, de 00 a 20; inventário por etapa. |
| Compilação dos checkpoints | Todos compilados em Release com avisos tratados como erros; zero avisos/erros. |
| Consoles 00–04 | Executados com `--verificar`, incluindo entrada inválida, conta conhecida, objetos e substituição de dependência. |
| Solução principal | Três projetos compilados em Release; zero avisos/erros. |
| Verificações locais da referência | 56 verificações aprovadas, com fakes e construção das seis janelas. |
| Publicação | Pasta autocontida win-x64 gerada em `artifacts/publish` com .NET 10. |
| Consulta de vulnerabilidades NuGet | Nenhum pacote vulnerável reportado para o aplicativo, incluindo transitivos, nas fontes consultadas nesta execução. |
| Proteção do runner PostgreSQL | Sem configuração explícita, retorna código 2 e informa pendência sem abrir conexão. |
| `git diff --check` | Sem erros de espaços nas alterações. |

As verificações locais incluem fronteiras monetárias e de texto, autorização, carrinho, preservação de estado, hash/salt, rehash, senha com espaços e simulação de resposta perdida. A construção das janelas não opera os cliques. A simulação em memória não comprova perda de resposta real depois do commit.

## Preparado, dependente de laboratório

- SQL de restrições com tabelas temporárias e massa fixa de fronteiras do relatório.
- Runner PostgreSQL com esquema exclusivo: último item concorrente, mesma chave concorrente, rollback após escrita, repetição, histórico e intervalo semiaberto.
- Fluxos manuais de teclado, login, permissões, cadastro, estoque, estados pendentes e execução publicada fora da IDE.

Essas práticas **não foram executadas contra PostgreSQL nesta revisão**: não há `SUPERMERCADO_TEST_DB` configurada. O runner foi compilado e sua proteção sem conexão foi executada. Não foram criados projetos, branches ou contas remotas, nem aplicados scripts a banco existente. O [roteiro manual](VALIDACAO-MANUAL.md) e as [instruções do runner](src/README.md#verificações-de-integração-isoladas) permitem realizar e registrar os testes no laboratório.

## Reproduzir

Com o SDK instalado para a turma, execute na raiz:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/Validar-Material.ps1
dotnet publish src/SupermercadoApp/SupermercadoApp.csproj -c Release -r win-x64 --self-contained true -o artifacts/publish
dotnet list src/SupermercadoApp/SupermercadoApp.csproj package --vulnerable --include-transitive
```

Para usar o SDK local desta execução, passe `-Dotnet .\artifacts\sdk\dotnet.exe` ao script. Para conferir apenas os documentos, use `-SomenteDocumentacao`. Cada validação gera uma nova pasta para preservar checkpoints e escreve `VALIDACAO.txt` quando conclui.

O [registro anterior](VALIDACAO.md), datado de 16/10/2026, foi preservado como histórico do .NET 8. Os artefatos de compilação, SDK e publicação são locais e ignorados pelo Git. O ritmo de 70 horas ainda precisa ser observado na aplicação piloto da turma.
