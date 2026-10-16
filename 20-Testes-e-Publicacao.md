# Aula 20 — Verificação e publicação local

**Duração:** 3h30. **Pré-requisito:** aula 19. **Entrega:** aplicação compilada em Release, evidências dos fluxos essenciais e pasta executável para demonstração no laboratório.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Conferir o fluxo completo e o escopo da entrega |
| Explicação | 30 | Teste de regra, integração, interface e publicação |
| Construção | 60 | Executar verificações e corrigir problemas encontrados |
| Intervalo | 15 | Pausa |
| Prática | 60 | Publicar e executar fora da IDE |
| Entrega | 30 | Demonstrar, apresentar evidências e registrar limites |

## 1. Separe o que cada verificação comprova

Uma compilação encontra tipos e chamadas inconsistentes, mas não comprova venda correta. Um teste com repositório em memória comprova uma regra, mas não comprova transação PostgreSQL. Um teste no banco não comprova navegação por teclado. Precisamos de evidências complementares.

Na solução de referência, abra `Supermercado.sln` e execute no terminal da raiz:

```powershell
dotnet build Supermercado.sln -c Release
dotnet run --project tests/SupermercadoApp.Verificacoes -c Release
```

O projeto de verificações é um console para reduzir dependências didáticas; falhas encerram com código diferente de zero. Leia os casos em [Program.cs](tests/SupermercadoApp.Verificacoes/Program.cs). Eles verificam carrinho, validação, hash, perfis e encaminhamento de pedidos. Não são testes de PostgreSQL.

O roteiro opcional de integração fica em [VALIDACAO-MANUAL.md](VALIDACAO-MANUAL.md), incluindo rollback após escrita, duas vendas concorrentes e repetição de chave. Rode apenas em ambiente didático isolado e registre saldos antes/depois. Sem executar esses casos, registre “pendente”; não conclua que os fakes testaram transações.

## 2. Verifique a interface e os dados

Siga o [roteiro manual completo](VALIDACAO-MANUAL.md). Use os dados conhecidos da aula 15 e uma conta de cada perfil. Confira especialmente login incorreto, troca de usuário, código duplicado, estoque insuficiente, recebido insuficiente, histórico e relatório.

Em dupla, uma pessoa opera a aplicação e outra confere os resultados esperados. Troquem os papéis para o segundo perfil. Se surgir um erro, descreva entrada e resultado antes de alterar o código; assim será possível verificar a correção.

## 3. Publique pelo Visual Studio

1. No Gerenciador de Soluções, botão direito no projeto **SupermercadoApp**, não na solução → **Publicar**.
2. Escolha **Pasta** e destino local `artifacts/publish` na raiz do seu projeto.
3. Nas configurações, selecione **Release**, destino `win-x64` e implantação **autocontida**.
4. Deixe arquivo único e trimming desabilitados nesta primeira publicação.
5. Publique e abra a pasta. Não copie apenas o `.exe`: mantenha todos os arquivos gerados.

Alternativa pelo terminal, na raiz deste repositório:

```powershell
dotnet publish src/SupermercadoApp/SupermercadoApp.csproj -c Release -r win-x64 --self-contained true -o artifacts/publish
```

A publicação autocontida inclui runtime, mas continua exigindo Windows compatível, arquitetura correta e internet para acessar o Neon. Ela é maior e precisa ser republicada quando houver atualização de segurança do runtime. A publicação pode precisar baixar pacotes do runtime pelo NuGet. Veja [implantação .NET](https://learn.microsoft.com/en-us/dotnet/core/deploying/).

## 4. Execute fora da IDE

Feche o Visual Studio, confirme que a conta Windows possui `SUPERMERCADO_DB` e abra `SupermercadoApp.exe`. Não entregue credenciais dentro da pasta publicada. Em outra máquina do laboratório, configure a variável localmente antes de testar. As contas do supermercado já estão no banco; não execute novamente a preparação de administrador.

Demonstre um login de cada perfil e uma venda de teste. Se o computador não puder acessar o Neon, registre essa limitação e não apresente a publicação como validada nesse computador. Não distribua o aplicativo para operação real com a credencial direta do banco; a arquitetura de produção exige a API discutida na aula 04.

## Critérios de entrega

- Solução e scripts organizados; sem credenciais versionadas.
- Compilação Release sem erro e resultado das verificações automáticas registrado.
- Evidências manuais dos fluxos de administrador e caixa, incluindo falhas.
- Pasta publicada abrindo fora da IDE em máquina de laboratório configurada.
- Explicação de uma transação, de autorização e do caminho tela → serviço → repositório.
- Limites conhecidos registrados: dinheiro simulado, comprovante não fiscal, conexão direta didática, sem modo offline ou recuperação durável de tentativa após encerramento forçado.

Os desafios futuros — API, recuperação de senha, turnos de caixa, devoluções, paginação, persistência local de tentativas, testes com framework e integrações fiscais — formam novas unidades. Eles não são trabalho obrigatório escondido nesta aula.

[Anterior: aula 19](19-Cadastro-de-Usuarios.md) · [Índice](README.md)
