# Aula 06 — Construindo a Interface de Login pelo Designer do Visual Studio 2022

**Duração:** 3h30. **Pré-requisito:** [Aula 05](05-Configuracao-Ambiente.md). **Entrega:** tela `FormLogin` desenhada no Designer do Visual Studio 2022, navegável apenas pelo teclado (`Tab`, `Shift+Tab`, `Enter`), com senha oculta e validação visual de campos vazios (a autenticação real com banco e hash entra na Aula 09).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Executar a janela vazia da Aula 05 e abrir o Designer |
| Explicação | 30 | `Name` vs `Text`, propriedades de usabilidade e Programação Orientada a Eventos |
| Construção | 60 | Configurar o formulário e arrastar/nomear todos os controles no VS 2022 |
| Intervalo | 15 | Pausa |
| Prática | 60 | Configurar teclado (`TabIndex`/`AcceptButton`) e construir o evento `Click` |
| Entrega | 30 | Demonstrar todos os testes (`A06.01`–`A06.04`) e registrar evidências |

![Referência visual para o layout da tela de login](assets/winforms_login_mockup_1791481192847.jpg)

---

## 1. Conceitos Essenciais do Windows Forms (O Quê e Por Quê)

### A diferença mais importante no Designer: `Name` vs `Text`
Todo controle visual tem duas propriedades que iniciantes costumam confundir:

| Propriedade | Para quem serve? | Exemplo no Botão | Por que diferenciar? |
| --- | --- | --- | --- |
| **`(Name)`** | Para o **programador no código C#** (é o nome da variável do controle). | `btnLogin` | Não pode ter espaços nem acentos. Usamos prefixos curtos (`lbl` para Label, `txt` para TextBox, `btn` para Button) para o IntelliSense do VS 2022 agrupar e identificar controles rapidamente. |
| **`Text`** | Para o **usuário final na tela** (o texto visível). | `Entrar` | Pode ter espaços, acentos e emojis. Alterar `Text` nunca quebra o seu código C#; alterar `(Name)` muda o nome da variável! |

### O que é Programação Orientada a Eventos?
No Console (Aulas 01–04), o código executava de cima para baixo até acabar. Em um aplicativo desktop Windows Forms, o programa fica **parado esperando ações do usuário** (eventos): um clique do mouse (`Click`), uma tecla pressionada (`KeyDown`) ou o fechamento da janela (`FormClosing`).

Quando você dá **duplo clique** em um botão no Designer do Visual Studio 2022, duas coisas acontecem automaticamente:
1. No `FormLogin.Designer.cs`, o VS registra a inscrição do evento: `btnLogin.Click += btnLogin_Click;` (significa *"quando clicarem neste botão, chame este método"*).
2. No `FormLogin.cs`, o VS cria o método manipulador:
   ```csharp
   private void btnLogin_Click(object sender, EventArgs e)
   {
       // Seu código entra aqui
   }
   ```
   - `object sender`: informa **qual controle** disparou o evento (neste caso, o próprio `btnLogin`).
   - `EventArgs e`: carrega dados extras sobre o evento (se houver).

> [!WARNING]
> **Por que verificar a inscrição do evento na aba de raio (⚡)?** Se você der duplo clique no botão, apagar o método manualmente e clicar de novo, pode acabar associando dois manipuladores ao mesmo botão — e um único clique disparará a mensagem **duas vezes**!

---

## 2. Roteiro de Construção Guiada no Designer do Visual Studio 2022

### Passo 1 — Configurar o Formulário `FormLogin`
1. No **Gerenciador de Soluções**, abra `Views/FormLogin.cs` no modo visual (se abrir código, clique com o botão direito → **Exibir Designer** ou pressione `Shift+F7`).
2. Clique uma vez na **área cinza vazia** do formulário e pressione **`F4`** para abrir a janela **Propriedades**.
3. Configure cada propriedade abaixo entendendo o **porquê**:

| Propriedade | Valor a configurar | Por que configuramos assim? |
| --- | --- | --- |
| `Text` | `MarketFlow — Login` | Exibe um título claro na barra superior da janela. |
| `StartPosition` | `CenterScreen` | Faz a janela abrir sempre centralizada no monitor, em vez de em posições aleatórias. |
| `FormBorderStyle` | `FixedSingle` | Impede o usuário de esticar a borda e deformar um layout ainda fixo. |
| `MaximizeBox` | `False` | Desativa o botão de tela cheia pelo mesmo motivo acima. |
| `ClientSize` | `420; 300` | Define espaço interno confortável para rótulos, caixas e botão. |
| `Font` | `Segoe UI, 10` | Fonte padrão moderna do Windows, com leitura confortável e bom contraste. |

### Passo 2 — Arrastar e Configurar os 6 Controles (`A06.01` e `A06.03`)
Abra a **Caixa de Ferramentas** (`Ctrl+Alt+X`), expanda **Controles Comuns** e arraste os controles abaixo para dentro do formulário. Para cada controle, selecione-o, pressione `F4` e configure:

| Tipo do Controle | `(Name)` | Propriedades a configurar | Por que configuramos assim? |
| --- | --- | --- | --- |
| `Label` | `lblTitulo` | `Text` = `MarketFlow — Supermercado` | Identifica o sistema visualmente no topo da janela. |
| `Label` | `lblUsuario` | `Text` = `Usuário` | Mantém a identificação visível mesmo com o campo digitado (melhor acessibilidade do que texto que some ao digitar). |
| `TextBox` | `txtUsuario` | `Text` vazio; `TabIndex` = `0`; `MaxLength` = `50` | `TabIndex = 0` coloca o cursor aqui primeiro; `MaxLength = 50` já respeita o limite que teremos no banco de dados! |
| `Label` | `lblSenha` | `Text` = `Senha` | Identifica claramente o campo de senha. |
| `TextBox` | `txtSenha` | `Text` vazio; `UseSystemPasswordChar` = `True`; `TabIndex` = `1`; `MaxLength` = `128` | `UseSystemPasswordChar = True` mascara os caracteres com bolinhas/asteriscos contra curiosos olhando o monitor. **Atenção:** isso protege apenas a *tela*, **não** protege o banco de dados (o hash entra na Aula 09). |
| `Button` | `btnLogin` | `Text` = `Entrar`; `TabIndex` = `2` | `TabIndex = 2` permite alcançar o botão com `Tab` sem tirar as mãos do teclado. |

### Passo 3 — Configurar a Tecla `Enter` (`AcceptButton`)
1. Clique novamente na **área vazia** do formulário `FormLogin` e pressione `F4`.
2. Localize a propriedade **`AcceptButton`** e selecione **`btnLogin`** na lista suspensa.
   - **Por quê?** Em caixas de supermercado, operadores trabalham rápido no teclado: pressionar `Enter` em qualquer campo agora aciona automaticamente o clique de `btnLogin`!

### Passo 4 — Construir o Evento de Validação Visual (`A06.02`)
1. No Designer, dê **duplo clique** sobre o botão `btnLogin`. O Visual Studio 2022 abrirá `FormLogin.cs` já posicionado dentro de `btnLogin_Click`.
2. **Construa a lógica do evento seguindo estas orientações** (sem criar senha fixa como `"admin"/"123456"` nem fingir que o login funcionou!):
   - Escreva um `if` verificando se o campo de usuário está vazio/só espaços (`string.IsNullOrWhiteSpace(txtUsuario.Text)`) **OU (`||`)** se o comprimento da senha é zero (`txtSenha.Text.Length == 0`).
   - **Por que usar `.Length == 0` para senha em vez de `IsNullOrWhiteSpace`?** Porque espaços podem fazer parte de uma senha longa (*passphrase*) — na Aula 09 preservaremos espaços intencionais da senha sem `Trim()`!
   - Dentro do `if`: mostre um aviso claro com `MessageBox.Show("Preencha usuário e senha.");` e encerre imediatamente o método com **`return;`**.
     - **O que faz o `return;` aqui?** Interrompe apenas a execução do método `btnLogin_Click` (impedindo que as linhas de baixo rodem). O aplicativo continua aberto normalmente!
   - Depois do `if` (quando ambos os campos foram preenchidos): exiba uma `MessageBox.Show` honesta informando que os campos foram validados visualmente e que a autenticação real será implementada na Aula 09.

---

## 3. Roteiro de Testes e Verificação no Visual Studio 2022 (`A06.01`–`A06.04`)

Execute com **`F5`** e registre no [Caderno de Evidências](CADERNO-DE-EVIDENCIAS.md) o resultado esperado e observado de cada teste:

1. **Ambos vazios:** clique em **Entrar** sem digitar nada → deve exibir a mensagem pedindo usuário e senha.
2. **Apenas usuário preenchido:** digite um usuário, deixe a senha vazia e pressione **`Enter`** → o `AcceptButton` deve disparar o botão e ainda recusar porque falta a senha.
3. **Ambos preenchidos:** digite usuário e senha e pressione `Enter` → deve confirmar o preenchimento avisando que a autenticação entra na Aula 09.
4. **Navegação 100% pelo teclado:** sem usar o mouse, navegue com **`Tab`** (`txtUsuario` → `txtSenha` → `btnLogin`) e volte com **`Shift+Tab`**. Confirme que a ordem está lógica.
5. **Segurança visual e limpeza inicial:** confirme que a senha aparece oculta com bolinhas/asteriscos. Feche e abra a janela novamente confirmando que nenhum campo vem pré-preenchido.
6. **Auditoria de inscrição única de evento (`A06.04`):** no Designer, selecione `btnLogin`, pressione `F4`, clique no **ícone de raio ⚡ (Eventos)** no topo da janela de Propriedades e confirme que o evento **`Click`** aponta **uma única vez** para `btnLogin_Click`.

> [!TIP]
> **Acessibilidade:** nunca dependa apenas de cor vermelha/verde para avisar erro — sempre apresente uma mensagem textual clara explicando o que o usuário precisa corrigir.

---

## 4. Erros Comuns no Visual Studio 2022

| Sintoma / Erro | Causa provável | Como corrigir |
| --- | --- | --- |
| `CS0103: O nome 'txtUsuario' não existe` | Você alterou a propriedade `Text` em vez da propriedade `(Name)`. | Selecione o `TextBox` no Designer, pressione `F4` e confira **`(Name)`** no topo da lista. |
| Um clique abre a `MessageBox` duas vezes | O evento `Click` foi associado duas vezes no `.Designer.cs` ou no construtor. | Verifique a aba de raio ⚡ em Propriedades e confirme que `btnLogin.Click +=` aparece uma única vez no `.Designer.cs`. |
| Ao apagar um evento no `.cs`, o Designer quebra com tela de erro vermelha | A linha `btnLogin.Click += ...` continuou existindo no `.Designer.cs` apontando para o método apagado. | Abra `FormLogin.Designer.cs`, remova apenas a linha órfã do evento apagado (ou desfaça com `Ctrl+Z`) e recompile (`Ctrl+Shift+B`). |

[Anterior: Aula 05](05-Configuracao-Ambiente.md) · [Próxima: Aula 07 — Banco de dados e usuários](07-Criando-Banco-Dados-Neon.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 06 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-06). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
