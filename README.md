# C# na prática: supermercado desktop

Roteiro para estudantes iniciantes construírem o **MarketFlow**, um aplicativo Windows Forms com áreas separadas para **Administrador** e **Caixa**, utilizando **Visual Studio 2026** e **Neon PostgreSQL**.

## Por onde começar

Para preparar as entregas da turma, consulte o [caderno de atividades por conceito](ATIVIDADES-POR-CONCEITO.md), com enunciados e critérios de aceite para as 20 aulas. A [análise pedagógica](ANALISE-PEDAGOGICA.md) apresenta lacunas, prioridades e recomendações para o professor.

O [apoio ao professor](ensino/README.md) traz preparação do laboratório e geração de 21 checkpoints, do console inicial à entrega final. Para conferir documentação, compilar as etapas e executar as verificações locais, use `tools/Validar-Material.ps1`.

1. Leia o [plano das aulas](PLANO-DE-AULAS.md): objetivos, tempos, entregas e avaliação.
2. Faça as aulas na ordem abaixo. Se já concluiu a versão antiga, siga primeiro a [transição](TRANSICAO.md).
3. Consulte o [código de referência e a execução](src/README.md) para comparar seu trabalho; ele representa o resultado final, não um projeto inicial que deve ser copiado inteiro na primeira aula.
4. Registre as verificações no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Consulte também o [registro de validação das melhorias](VALIDACAO-MELHORIAS.md); o [registro anterior](VALIDACAO.md) foi preservado como histórico.

## Sequência das práticas

| Aula | Prática | Entrega ao final de 3h30 |
| --- | --- | --- |
| 01 | [Lógica e tipos](01-Introducao-CSharp.md) | Cálculo e decisões com valores de uma compra |
| 02 | [Console e depuração](02-Pratica-Console.md) | Caixa de console com entrada validada |
| 03 | [Orientação a objetos](03-Orientacao-a-Objetos.md) | Produtos e carrinho em memória |
| 04 | [Arquitetura e contratos](04-Arquitetura-e-Padroes.md) | Responsabilidades e repositório em memória |
| 05 | [Ambiente e solução](05-Configuracao-Ambiente.md) | Aplicativo Windows Forms compilando |
| 06 | [Interface de login](06-Criando-Interface-Login.md) | Tela navegável pelo teclado |
| 07 | [Banco Neon](07-Criando-Banco-Dados-Neon.md) | Tabela de usuários sem senha em texto puro |
| 08 | [Conexão assíncrona](08-Conectando-Banco-Dados.md) | Consulta ao banco sem congelar a tela |
| 09 | [Autenticação](09-Autenticacao-Segura.md) | Primeiro administrador e login por hash |
| 10 | [Sessão e perfis](10-Sessao-e-Perfis.md) | Menus separados e troca de usuário |
| 11 | [Modelo de produtos](11-Modelo-de-Produtos.md) | Tabelas e validação de produto |
| 12 | [Consulta de produtos](12-Consulta-de-Produtos.md) | Busca parametrizada em uma grade |
| 13 | [Cadastro de produtos](13-Cadastro-de-Produtos.md) | Inclusão, edição e inativação |
| 14 | [Reposição de estoque](14-Reposicao-de-Estoque.md) | Saldo e histórico atualizados juntos |
| 15 | [Carrinho do caixa](15-Carrinho-do-Caixa.md) | Inclusão, remoção e total em memória |
| 16 | [Persistência da venda](16-Venda-com-Transacao.md) | Venda atômica e proteção de estoque |
| 17 | [Finalização e troco](17-Finalizacao-e-Troco.md) | Fluxo completo com recuperação de tentativa |
| 18 | [Relatório de vendas](18-Relatorio-de-Vendas.md) | Consulta por período e total vendido |
| 19 | [Cadastro de usuários](19-Cadastro-de-Usuarios.md) | Administrador cadastra caixas e administradores |
| 20 | [Testes e publicação](20-Testes-e-Publicacao.md) | Evidências de testes e pasta executável |

## Decisões do projeto

Usamos **um projeto com pastas por responsabilidade**, injeção manual pelo construtor, SQL parametrizado e Npgsql. Uma solução com muitos projetos, um contêiner de dependências e um ORM acrescentaria conceitos antes de o aluno compreender o problema. A arquitetura permite evoluir essas escolhas depois. Veja a [aula 04](04-Arquitetura-e-Padroes.md).

O escopo trabalha com produtos em **unidades inteiras** e **dinheiro simulado**. Não inclui integração com balança, TEF/Pix, emissão fiscal, vendas offline, abertura/fechamento de turno ou devoluções. O comprovante é didático, sem valor fiscal. O objetivo é concluir um fluxo de supermercado consistente antes dessas integrações.

A ligação direta do desktop ao Neon é um recorte de laboratório: credenciais no computador e verificações de perfil em C# não constituem uma barreira contra alguém que modifique o aplicativo. Para distribuição comercial, uma API deve guardar as credenciais e aplicar autenticação e autorização no servidor. Isso é explicado na [arquitetura](04-Arquitetura-e-Padroes.md), sem exigir uma API nesta primeira sequência.

## Versões e referências

O ambiente desta edição é **.NET 10 LTS**, **Visual Studio 2026 18.11** e **SDK 10.0.404** (faixa 10.0.4xx, selecionada por `global.json`). Atualize os patches dessa faixa antes de cada turma. O .NET 8 usado na edição anterior encerrou suporte em 10/11/2026; código, pacotes e instruções foram atualizados em conjunto nesta edição. Consulte a [compatibilidade oficial](https://learn.microsoft.com/en-us/dotnet/core/porting/versioning-sdk-msbuild-vs) e a [política de suporte](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core).

O exemplo fixa Npgsql 10.0.3 e Microsoft.Extensions.Identity.Core 10.0.15 para reprodução. Não significam “últimas versões”; o professor deve verificar atualizações e vulnerabilidades compatíveis antes de cada turma. As [fontes oficiais](REFERENCIAS.md) sustentam as decisões técnicas.
