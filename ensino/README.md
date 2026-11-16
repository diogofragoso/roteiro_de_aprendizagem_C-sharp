# Apoio ao professor e pontos de recuperação

Este material acompanha as [atividades dos estudantes](../ATIVIDADES-POR-CONCEITO.md). Prepare os checkpoints antes da turma. Eles contêm somente os tipos, métodos e controles da etapa correspondente; não são a aplicação final com menus escondidos.

## Gerar as versões de início e fim

Na raiz, com PowerShell e o SDK indicado em `global.json`:

```powershell
powershell -ExecutionPolicy Bypass -File tools/Gerar-Checkpoints.ps1
powershell -ExecutionPolicy Bypass -File tools/Validar-Material.ps1
```

As versões ficam em `artifacts/checkpoints/00` a `20`. A aula N começa com a versão N−1 e termina com N. A versão 00 é o console preparado para a primeira aula. Cada pasta tem projeto independente, fontes, scripts disponíveis naquele momento, inventário de arquivos, assinaturas em `METODOS.txt` e instruções. O gerador recusa reutilizar uma pasta existente: para gerar novamente, escolha outro `-Destino` dentro de `artifacts`. Assim ele não apaga trabalhos de recuperação.

Abra o `.csproj` da pasta para trabalhar. Ao recuperar uma etapa, copie sua pasta para a área do estudante e faça nela as atividades da aula seguinte. Compare os inventários de duas etapas para identificar o que entrou. Aulas 05–08 mantêm login com Designer; a partir da 09 o checkpoint usa layout em código como alternativa de recuperação, preservando nomes dos controles. Quem usa Designer segue o adaptador da aula 09.

`Validar-Material.ps1` confere links/IDs/tempos, gera uma nova série, compila as 21 etapas e executa os consoles 00–04 com `--verificar`. Não abre janelas nem consulta banco. Não precisa de credenciais. O relatório distingue essas verificações das práticas de integração.

## Preparação antes dos encontros

| Antes da aula | Preparação do professor | Evidência da preparação |
| --- | --- | --- |
| 01 | Executar diagnóstico sem nota: criar pasta, abrir checkpoint 00, executar e reconhecer entrada/saída. | Registrar quem precisa de apoio para arquivos/IDE antes de avaliar lógica. |
| 05 | Conferir Visual Studio e SDK compatíveis, workload desktop e acesso NuGet. | Compilar checkpoint 05 em computador representativo do laboratório. |
| 07 | Separar banco/branch vazio por dupla; usar conexão direta para exercícios SQL com estado de sessão. | Identificar ambiente sem publicar credenciais. |
| 09 | Scripts 001 e conexão prontos; distribuir checkpoint 08 e roteiro de leitura abaixo. | Diagnóstico SELECT 1 e tipos compilando antes do evento de login. |
| 10 | Preparar admin e caixa no banco da dupla pela ferramenta abaixo. | Cada perfil entra; nenhuma senha no código ou relatório. |
| 11 | Script 002 e checkpoint com fake que conta chamadas. | Verificação de preço inválido não incrementa chamadas. |
| 12 | Cadastrar arroz e feijão pelo cadastro da referência em ambiente preparado; não pedir que estudante improvise botão temporário. | Busca retorna os produtos fictícios esperados. |
| 16 | Script 003, dois caixas, produto com saldo 1 e branch isolada para falha controlada. | Contagens e saldos iniciais anotados; gatilho de falha ausente antes do teste. |
| 18 | Preparar massa de fronteira de datas; configurar fuso do Windows. | Resultados de início/fim conferidos com o SQL de laboratório. |

Para cadastrar o caixa de laboratório depois do primeiro administrador, abra a referência com `--preparar-caixa`. Autentique o administrador e preencha o formulário de cadastro que se abre. O argumento é uma ferramenta do professor: exige autenticação e permissão administrativa, não contém senha padrão nem amplia privilégios. Nos checkpoints 10–18 a mesma ferramenta está disponível; o formulário de cadastro é o único recurso antecipado, identificado como ferramenta de preparação. No fluxo normal ele só entra no menu na aula 19.

## Checkpoints dentro das aulas densas

Na aula 09, use quatro paradas: (1) Perfil/Credencial/Sessao e ausência de usuário; (2) repositório parametrizado; (3) hash/verificação e preparação; (4) evento da tela. Nos 60 minutos guiados, trabalhe 1 e 2; nos 60 de prática, 3 e 4; reserve explicação e entrega para rastreamento individual e evidências. Compile após cada parada e só avance quando a dupla conseguir explicar o retorno.

Na aula 16, o professor fornece os blocos técnicos da referência para leitura e alteração orientada. Nos 30 minutos de explicação, trace duas vendas concorrentes no papel. Nos 60 guiados, trabalhe esquema, contrato e transação; nos 60 de prática, execute último item, repetição e rollback após escrita. Reserve 30 de entrega para confrontar contagens e explicar o commit. Os demais 30 minutos são retomada e intervalo. Se o ambiente ou ritmo não permitir algum teste, registre pendência e agende recuperação; não considere a previsão como execução.

## Devolutiva e recuperação

Em cada aula, faça uma pergunta de recuperação sobre o encontro anterior. Use os IDs do caderno para localizar a evidência e a rubrica do plano para avaliar. Peça que cada estudante explique uma entrada diferente da copiada. Se não demonstrar, registre uma ação específica: por exemplo, “refazer A02.03 com número negativo e explicar a diferença entre conversão e regra”. Nos marcos 06, 10, 14, 17 e 20, cada integrante explica uma atividade escolhida pelo professor.

As 114 atividades integram construção e verificação existentes. Não distribua uma segunda carga de dever obrigatório. Registre os tempos observados na primeira turma e ajuste o calendário quando necessário.
