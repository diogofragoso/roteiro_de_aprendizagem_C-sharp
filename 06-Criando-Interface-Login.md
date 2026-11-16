# Aula 06 — Interface de login pelo Designer

**Duração:** 3h30. **Pré-requisito:** aula 05. **Entrega:** tela de login acessível pelo teclado, com campos e validação visual; autenticação só será adicionada na aula 09.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Executar a janela vazia |
| Explicação | 30 | Name, Text, propriedades e eventos |
| Construção | 60 | Desenhar e nomear os controles |
| Intervalo | 15 | Pausa |
| Prática | 60 | Navegação por teclado, evento e validação |
| Entrega | 30 | Demonstrar Tab, Enter e senha oculta |

![Referência visual para o login](assets/winforms_login_mockup_1791481192847.jpg)

## 1. Configure o formulário

Abra `Views/FormLogin.cs` no Designer. Se aparecer código, use botão direito → **Exibir Designer**. Selecione a área vazia da janela, pressione F4 e configure:

| Propriedade | Valor | Motivo |
| --- | --- | --- |
| Text | MarketFlow — Login | Identificar a janela |
| StartPosition | CenterScreen | Facilitar a localização |
| FormBorderStyle | FixedSingle | Manter o desenho inicial estável |
| MaximizeBox | False | Evitar expansão de um layout ainda fixo |
| ClientSize | 420; 300 | Dar espaço aos campos e rótulos |
| Font | Segoe UI, 10 | Leitura consistente |

Use contraste legível. Não dependa apenas de vermelho/verde para indicar erro; a mensagem deve explicar o problema.

## 2. Arraste e nomeie os controles

Na **Caixa de Ferramentas**, arraste os itens abaixo. Diferencie `Name` (identificador usado no C#) de `Text` (texto exibido).

| Controle | Name | Text / outras propriedades |
| --- | --- | --- |
| Label | lblTitulo | MarketFlow — Supermercado |
| Label | lblUsuario | Usuário |
| TextBox | txtUsuario | Text vazio; TabIndex = 0; MaxLength = 50 |
| Label | lblSenha | Senha |
| TextBox | txtSenha | Text vazio; UseSystemPasswordChar = True; TabIndex = 1; MaxLength = 128 |
| Button | btnLogin | Entrar; TabIndex = 2 |

Selecione o formulário e ajuste **AcceptButton** para `btnLogin`: Enter aciona o botão. Rótulos permitem identificar os campos mesmo quando a caixa está vazia. Os asteriscos escondem a senha na tela, mas **não** a protegem no banco; isso será resolvido com hash.

## 3. Programe um evento pequeno

Dê duplo clique em `btnLogin` para o VS gerar o evento. Dentro dele, adicione:

```csharp
if (string.IsNullOrWhiteSpace(txtUsuario.Text) || txtSenha.Text.Length == 0)
{
    MessageBox.Show("Preencha usuário e senha.");
    return;
}
MessageBox.Show("Campos preenchidos. A autenticação será adicionada na aula 09.");
```

`return` encerra esse clique; a aplicação continua aberta. Não crie uma senha fixa `admin/123456` nem simule sucesso de login. Ainda estamos verificando os controles.

## 4. Teste e ajuste

Registre cada execução em A06.01–A06.04: preparação, ação, resultado e uma explicação individual. Anote a ordem do teclado e a quantidade de mensagens por clique. Esses registros são a entrega do teste, junto dos arquivos `.cs` e `.Designer.cs`.

1. F5: tente entrar com os dois campos vazios.
2. Digite apenas o usuário e pressione Enter: a mensagem ainda deve pedir os dois campos.
3. Preencha ambos: a tela deve informar que a autenticação ainda não existe.
4. Use Tab desde o usuário até o botão, sem mouse. Use Shift+Tab para voltar.
5. Confira que o texto de senha é oculto. Feche e reabra: campos não devem vir preenchidos.
6. Na janela de eventos do botão (ícone de raio nas Propriedades), confira que `Click` aponta uma única vez para `btnLogin_Click`. Dois eventos podem executar a ação duas vezes.

## Verifique antes de encerrar

Entregue a tela, as três verificações de campos e uma explicação de `Name`, `Text` e `Click`. Não cole o construtor completo da referência sobre esse formulário: ela constrói o layout em código. Nas próximas aulas você pode manter seu Designer e adaptar somente os campos, dependências e eventos indicados.

[Anterior: aula 05](05-Configuracao-Ambiente.md) · [Próxima: aula 07](07-Criando-Banco-Dados-Neon.md)

## Atividades por conceito

Realize as atividades da [aula 06 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-06). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
