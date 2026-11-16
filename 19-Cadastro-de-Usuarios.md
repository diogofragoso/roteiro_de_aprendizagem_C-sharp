# Aula 19 — Cadastro de usuários por administrador

**Duração:** 3h30. **Pré-requisito:** aula 18 e administrador autenticado. **Entrega:** formulário para criar novas contas de administrador ou caixa, sem armazenar senha aberta.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Rever a preparação temporária de caixa |
| Explicação | 30 | Perfil, normalização e unicidade |
| Construção | 60 | Formulário e chamada ao serviço existente |
| Intervalo | 15 | Pausa |
| Prática | 60 | Integração ao menu e verificações de acesso |
| Entrega | 30 | Novo usuário entra e respeita seu perfil |

## 1. Reutilize o caso de uso

`AutenticacaoService.CriarAsync` foi implementado na aula 09 e usado pelo professor na preparação. Agora criaremos sua interface definitiva. Não duplique a geração de hash na janela: a tela nunca precisa conhecer `PasswordHasher`.

Antes de criar a conta, o serviço exige administrador, normaliza username, valida tamanho e perfil e pede ao repositório para gravar o hash. O banco resolve a concorrência pela restrição de unicidade. A enumeração do ComboBox evita digitação livre de nomes de perfil.

## 2. Construa a tela

Crie [FormUsuarios.cs](src/SupermercadoApp/Views/FormUsuarios.cs), herdando de `Form`, com:

- Username de 3 a 50 caracteres: letras ASCII, números ou ponto.
- Senha de 12 a 128 caracteres, `UseSystemPasswordChar = true`.
- `ComboBox` com `DropDownStyle.DropDownList` e `Enum.GetValues<Perfil>()`.
- Botão Cadastrar, desabilitado durante a chamada.

A janela recebe sessão e serviço pelo construtor e chama `Permissoes.ExigirAdministrador`. O clique aguarda `CriarAsync`; só depois de sucesso limpa os campos e mostra confirmação. Falha de duplicidade mantém os dados para correção; a senha não deve aparecer em mensagens nem logs.

## 3. Ligue ao menu e teste a conta criada

Atualize o construtor de `FormAdministrador` para receber também `AutenticacaoService` e adicione o botão “Cadastrar usuário”. Confira agora a composição completa de [Program.cs](src/SupermercadoApp/Program.cs) e [FormAdministrador.cs](src/SupermercadoApp/Views/FormAdministrador.cs): todos os tipos necessários já foram apresentados.

Crie `caixa.novo` com perfil Caixa e uma senha escolhida na tela. Saia, entre com a conta e confirme que o caixa abre. Depois saia e volte com administrador, criando uma segunda conta administrativa. Cada conta deve receber o perfil selecionado.

Se você antecipou `FormUsuarios` para preparar o laboratório na aula 10, revise agora as regras e remova qualquer botão de preparação remanescente. O modo `--preparar-admin` continua recusando tabela não vazia.

## Verifique antes de encerrar

Reaproveite os testes por conceito A19.01–A19.06: fronteiras de tamanho, espaços da senha e valor de enum inválido são verificações de serviço, mesmo quando o controle visual impede digitação equivalente. Registre quais chamadas chegaram ao repositório e confirme o login da conta criada. Não basta fotografar o cadastro com mensagem de sucesso.

1. `CAIXA.NOVO` e `caixa.novo` não podem criar duas contas diferentes.
2. Senha curta é recusada; senha com espaços não é recortada silenciosamente.
3. Conta nova com senha incorreta não entra.
4. Caixa não consegue cadastrar usuários nem selecionar um perfil maior para si.
5. A consulta `SELECT username, perfil FROM usuario` confere com os cadastros.

Este encontro cobre criação, não recuperação de senha, desativação de usuários ou mudança de perfil durante uma sessão. Esses casos precisam de políticas próprias. Uma sessão deste laboratório mantém seu perfil até sair; no cenário com API, revogação e validade de sessão devem ser aplicadas no servidor.

[Anterior: aula 18](18-Relatorio-de-Vendas.md) · [Próxima: aula 20](20-Testes-e-Publicacao.md)

## Atividades por conceito

Realize as atividades da [aula 19 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-19). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
