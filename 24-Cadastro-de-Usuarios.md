# Aula 24 — Cadastro de Usuários, Reuso de Serviços e `ComboBox` com `Enum`

**Duração:** 3h30. **Pré-requisito:** [Aula 23](23-Relatorio-de-Vendas.md) e administrador autenticado. **Entrega:** tela definitiva `FormUsuarios` conectada ao menu do administrador, reutilizando `AutenticacaoService.CriarAsync` sem duplicar lógica de hash nem SQL, permitindo criar novos administradores e caixas com validação completa.

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Revisar `AutenticacaoService.CriarAsync` criado na Aula 14 |
| Explicação | 30 | Reuso da camada de serviço, `ComboBox` vinculado a `Enum`, unboxing e limites de sessão |
| Construção | 60 | Construir `FormUsuarios.cs` e finalizar o menu de `FormAdministrador.cs` |
| Intervalo | 15 | Pausa |
| Prática | 60 | Cadastrar novas contas, testar fronteiras de username/senha e login real |
| Entrega | 30 | Demonstrar criação, normalização, duplicidade e bloqueio de `Caixa` (`A24.01`–`A24.06`) |

---

## 1. Por Que a Tela de Cadastro É Tão Pequena? (O Poder da Arquitetura) (`A24.01`)

Observe o retorno do investimento que você fez na Aula 08 e na Aula 14:
- **Todo o trabalho pesado já está pronto e protegido em `AutenticacaoService.CriarAsync`:** verificação de permissão administrativa, normalização do username para minúsculas, validação de caracteres ASCII (`3` a `50`), validação de senha (`12` a `128`, preservando espaços!), validação de enum (`Enum.IsDefined`) e geração segura de **Hash + Salt** com `PasswordHasher`.
- **A tela `FormUsuarios` NUNCA importa `PasswordHasher` nem escreve SQL!** Ela apenas coleta os 3 valores da interface, chama `await usuarios.CriarAsync(...)` dentro de `Ui.ExecutarAsync` e mostra a confirmação.

### Vinculando um `ComboBox` Diretamente a um `Enum` (`A24.04`)
*(Aplicação prática direta do padrão de `ComboBox` fechado estudado na [Aula 10 — Seção 2.6](10-Componentes-Windows-Forms.md).)*
Como evitar que o administrador digite livremente `"Gerente"` ou `"Caixaa"` no perfil?
Usamos um `ComboBox` configurado assim:
```csharp
var perfil = new ComboBox
{
    DropDownStyle = ComboBoxStyle.DropDownList,
    DataSource = Enum.GetValues<Perfil>(),
    Width = 260
};
```
- **`DropDownStyle = ComboBoxStyle.DropDownList`:** trava o campo para aceitar **apenas** escolhas da lista (não permite digitar texto livre).
- **`Enum.GetValues<Perfil>()`:** lê automaticamente todos os valores declarados no `enum Perfil` (`Administrador`, `Caixa`). Se um novo perfil entrar no futuro, o ComboBox se atualiza sozinho!
- **`(Perfil)perfil.SelectedItem!`:** como `SelectedItem` retorna `object?` (genérico), fazemos a conversão explícita (*cast*) para `Perfil`.
- **Por que o serviço ainda testa `Enum.IsDefined(perfil)` se o ComboBox já trava a UI?** Porque outro código (ou um teste unitário) pode chamar `CriarAsync(..., (Perfil)999)` passando por cima da tela! A camada de serviço nunca confia cegamente na interface gráfica.

---

## 2. Roteiro de Construção Guiada (Sua Vez no Visual Studio 2022!)

### Passo 1 — Construir `Views/FormUsuarios.cs`
Crie **`Views/FormUsuarios.cs`** em `namespace SupermercadoApp.Views;` (`public sealed class FormUsuarios : Form`):
1. Receba **`(Sessao sessao, AutenticacaoService usuarios)`** no construtor.
2. **Primeira linha obrigatória:** `Permissoes.ExigirAdministrador(sessao);`.
3. Prepare a janela com `Ui.Preparar(this, "Administração — Cadastrar usuário", 440, 400)`.
4. Crie os controles no painel:
   - `nome`: `Ui.Texto(painel, "Usuário (letras sem acentos, números ou ponto)")`;
   - `senha`: `Ui.Texto(painel, "Senha (12 a 128 caracteres)", senha: true)` *(com `UseSystemPasswordChar = true`)*;
   - Rótulo `painel.Controls.Add(Ui.Rotulo("Perfil de acesso"));` e o `ComboBox perfil` configurado com `DropDownStyle.DropDownList`, `DataSource = Enum.GetValues<Perfil>()` e `Width = 260`, **adicionando-o ao painel com `painel.Controls.Add(perfil);`**.
5. Crie o botão **`"Cadastrar"`** com evento `async (sender, _) =>`:
   - Desabilite o botão antes do `try` e reabilite no `finally`.
   - Dentro de `Ui.ExecutarAsync`:
     1. Chame `await usuarios.CriarAsync(sessao, nome.Text, senha.Text, (Perfil)perfil.SelectedItem!);`.
     2. **Somente após sucesso** (depois do `await` sem exceção): limpe `senha.Clear();`, `nome.Clear();` e exiba `MessageBox.Show("Usuário cadastrado.");`.
     3. *(Por quê? Se der erro de usuário duplicado `23505` ou senha curta, os dados ficam disponíveis para correção, sem anunciar falso sucesso nem expor a senha na mensagem!)*

### Passo 2 — Finalizar `Views/FormAdministrador.cs` e `Program.cs`
1. Atualize o construtor de **`FormAdministrador.cs`** para receber as 4 dependências completas do sistema:
   `(Sessao sessao, ProdutoService produtos, AutenticacaoService usuarios, VendaService vendas)`.
2. Adicione o botão **`"Cadastrar usuário"`** abrindo `using var f = new FormUsuarios(sessao, usuarios); f.ShowDialog(this);`.
3. Confira a composição final no **`Program.cs`**:
   - Criação única de `using var banco = Banco.Criar();`;
   - Repositórios (`UsuarioRepository`, `ProdutoRepository`, `VendaRepository`);
   - Serviços (`AutenticacaoService`, `ProdutoService`, `VendaService`);
   - Passagem de todas as dependências para `FormAdministrador` e `FormCaixa` dentro do laço `while (true)` de sessão.
   - Certifique-se de que não restou nenhum botão temporário de aulas anteriores.

---

## 3. Verificação das Regras de Fronteira e Fluxo Completo (`A24.01`–`A24.06`)

Execute com **`F5`** no Visual Studio 2022 e comprove cada cenário (registrando evidências **sem jamais anotar senhas reais**):

1. **Fronteiras de `username` (`A24.02`):** teste `2` caracteres (recusa), `3` caracteres (aceita), `51` caracteres (recusa), acentos como `"joão"` (recusa) e ponto como `"caixa.novo"` (aceita). Em seguida tente cadastrar `"CAIXA.NOVO"` → deve ser recusado por unicidade (`23505`), sem criar conta duplicada!
2. **Fronteiras de `senha` e espaços (`A24.03`):** teste `11` caracteres (recusa), `12` caracteres (aceita) e `129` caracteres (recusa). Cadastre uma conta de teste usando uma senha fictícia com espaços nas bordas e comprove que o login só funciona com os espaços preservados!
3. **Login real das novas contas (`A24.05`):** crie `caixa.novo` (perfil `Caixa`), clique em **Sair / trocar usuário**, entre com `caixa.novo` e confirme que abre a janela de **Caixa** (e tente senha incorreta para confirmar recusa). Volte como Administrador e crie uma segunda conta `Administrador`.
4. **Conferência no SQL Editor:** execute `SELECT username, perfil FROM usuario ORDER BY id;` e confirme que bate com seus cadastros.
5. **Reflexão de arquitetura (`A24.06`):** registre no caderno por que sair da janela renova a sessão no nosso aplicativo e quais mecanismos seriam necessários em uma versão comercial para suportar *revogação imediata de sessão* ou *recuperação de senha esquecida* (funcionalidades de unidades futuras).

[Anterior: Aula 23](23-Relatorio-de-Vendas.md) · [Próxima: Aula 25 — Testes e publicação](25-Testes-e-Publicacao.md) · [Índice](README.md)

## Atividades por conceito

Realize as atividades da [aula 24 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-24). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
