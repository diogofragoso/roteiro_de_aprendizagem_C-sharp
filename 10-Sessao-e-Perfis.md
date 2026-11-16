# Aula 10 — Sessão, administrador e caixa

**Duração:** 3h30. **Pré-requisito:** aula 09. **Entrega:** cada perfil abre uma janela diferente e sair volta ao login, sem reaproveitar a sessão anterior.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Fazer login com administrador |
| Explicação | 30 | Autenticação, autorização e ciclo de vida de janelas |
| Construção | 60 | Criar janelas e selecionar por perfil |
| Intervalo | 15 | Pausa |
| Prática | 60 | Troca de usuário e teste com caixa |
| Entrega | 30 | Demonstrar acesso permitido e negado |

## 1. Escreva a regra antes da interface

| Ação | Administrador | Caixa |
| --- | --- | --- |
| Gerenciar produtos e estoque | Sim | Não |
| Consultar relatório e criar usuários | Sim | Não |
| Montar carrinho e registrar venda | Não | Sim |
| Sair / trocar usuário | Sim | Sim |

Este curso separa as funções: um administrador não vende com sua conta de administração. Uma pessoa que exerça ambas as funções utiliza contas com perfis próprios neste exemplo. A restrição é uma decisão explícita do projeto, não uma regra universal de supermercados.

Abra `Permissoes`, criado na aula 09. Cada método lança `UnauthorizedAccessException` quando o perfil não atende à operação. Usaremos a verificação no construtor da janela e no serviço. Esconder um botão sozinho não protege uma regra.

## 2. Crie as janelas mínimas

Em `Views`, **Adicionar → Classe**: `FormAdministrador.cs` e `FormCaixa.cs`. Para estas telas vamos construir o layout em C#, usando `Ui`, para conseguir reproduzir todos os arquivos pelo roteiro. A classe herda de `Form`:

```csharp
public sealed class FormAdministrador : Form
{
    public FormAdministrador(Sessao sessao)
    {
        Permissoes.ExigirAdministrador(sessao);
        var painel = Ui.Preparar(this, "Administração", 480, 300);
        painel.Controls.Add(Ui.Rotulo($"Olá, {sessao.Username}"));
        Ui.Botao(painel, "Sair / trocar usuário", (_, _) => Close());
    }
}
```

Adicione os `using` de `Models` e `Services` e o namespace `SupermercadoApp.Views`. Faça uma classe equivalente `FormCaixa`, chamando `ExigirCaixa`, com título “Caixa”. Não copie ainda os construtores finais de referência, pois exigem os serviços de aulas futuras.

`Ui.Preparar` cria um `FlowLayoutPanel` que organiza os controles de cima para baixo e permite rolagem. `(_, _) => Close()` é uma função curta de evento cujos argumentos não precisamos usar. Se preferir o Designer, crie os mesmos controles e eventos, mantendo as verificações e o construtor que recebe sessão.

## 3. Controle o fluxo no Program

### Do Designer ao layout em código

| No Designer | Em código | Significado |
| --- | --- | --- |
| Arrastar Button | `new Button()` | Criar controle |
| Configurar Text | `botao.Text = "Sair"` | Definir o texto visível |
| Inserir no formulário/painel | `painel.Controls.Add(botao)` | Inserir no contêiner |
| Associar evento Click | `botao.Click += AoSair` | Registrar comportamento uma vez |

`(_, _) => Close()` pode ser reescrito como `void AoSair(object? sender, EventArgs e) { Close(); }`. Os argumentos existem mesmo quando não são usados. Faça essa troca em A10.03 e mantenha uma única associação do evento. Layout e regras continuam sendo responsabilidades diferentes.

Substitua o trecho que abre o login uma única vez por um laço. Mantenha o modo `--preparar-admin` separado e saindo após a preparação. No modo normal:

```csharp
while (true)
{
    using var login = new FormLogin(autenticacao);
    if (login.ShowDialog() != DialogResult.OK || login.SessaoAutenticada is not { } sessao)
        break;
    using Form principal = sessao.Perfil == Perfil.Administrador
        ? new FormAdministrador(sessao)
        : new FormCaixa(sessao);
    principal.ShowDialog();
}
```

**Por quê `ShowDialog`?** Ele aguarda o fechamento da janela antes de seguir. `using` descarta a janela ao final. Um novo login gera uma nova sessão; não usamos um usuário global estático. Fechar o login encerra o laço. Fechar a janela principal retorna ao login.

No evento de autenticação, `DialogResult = DialogResult.OK` só ocorre quando o login dá certo. Sem essa atribuição, o laço não sabe que deve abrir a janela principal.

## 4. Prepare uma conta de caixa para os testes

A interface de cadastro será estudada na aula 19. Antes deste encontro, o professor executa a referência ou o checkpoint com `--preparar-caixa`, autentica um administrador e cadastra uma conta Caixa pelo formulário de preparação. A ferramenta usa o serviço existente e exige permissão administrativa; não contém senha padrão. Veja o [roteiro do professor](ensino/README.md). A turma recebe a conta preparada localmente, sem senha no código ou nas evidências.

Não acrescente um botão provisório de cadastro ao menu da turma. O formulário antecipado é identificado nos checkpoints como ferramenta do professor; seu acesso normal só entra no menu na aula 19.

## Verifique antes de encerrar

1. Admin abre “Administração”; caixa abre “Caixa”.
2. Caixa não vê funções de produtos; admin não vê carrinho.
3. Chame `Permissoes.ExigirAdministrador` com a sessão de caixa no depurador: deve recusar. Não altere o perfil para “passar”.
4. Sair da janela principal volta ao login com senha vazia; fechar login encerra o programa.
5. Repetir o ciclo três vezes não deixa janelas escondidas nem processos extras.

A autorização em C# organiza e protege os caminhos normais do aplicativo. Contra modificação maliciosa do cliente, precisamos da fronteira de servidor discutida na aula 04.

[Anterior: aula 09](09-Autenticacao-Segura.md) · [Próxima: aula 11](11-Modelo-de-Produtos.md)

## Atividades por conceito

Realize as atividades da [aula 10 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-10). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
