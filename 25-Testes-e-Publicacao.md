# Aula 25 — Verificação Automatizada com Fakes, Roteiro Manual e Publicação no VS 2022

**Duração:** 3h30. **Pré-requisito:** [Aula 24](24-Cadastro-de-Usuarios.md). **Entrega:** projeto console de verificações automatizadas com Fakes rodando suas próprias asserções (com prova de regressão), compilação `Release` limpa, roteiro manual conferido em dupla e executável autocontido (`win-x64`) funcionando fora do Visual Studio 2022 (`A25.01`–`A25.06`).

| Bloco | Minutos | Atividade |
| --- | ---: | --- |
| Retomada | 15 | Conferir o fluxo completo do MarketFlow e o escopo final da entrega |
| Explicação | 30 | Pirâmide de testes (Build, Fakes, Integração, UI), `Release` vs `Debug` e publicação |
| Construção | 60 | Construir e executar o projeto `SupermercadoApp.Verificacoes` + teste de regressão |
| Intervalo | 15 | Pausa |
| Prática | 60 | Executar roteiro manual em dupla, publicar pelo VS 2022 e rodar fora da IDE |
| Entrega | 30 | Demonstrar executável, explicar transação/camadas e registrar limites conhecidos |

---

## 1. O Que Cada Verificação Comprova — E o Que NÃO Comprova (`A25.01`)

| Camada de Verificação | O que comprova | O que **NÃO** comprova |
| --- | --- | --- |
| **Build `Release`** | Sintaxe, tipos e contratos compatíveis. | Não comprova regras de negócio nem SQL. |
| **Testes de Regra com Fakes** | Validações de `Carrinho`, `ProdutoService`, `AutenticacaoService`, `VendaService` e `Permissoes` em milissegundos, offline! | Não comprova `FOR UPDATE`, rollback PostgreSQL nem constraints SQL. |
| **Integração no Banco** | Atomicidade (rollback após escrita), disputa concorrente, idempotência e intervalo UTC. | Não comprova teclado (`Tab`/`Enter`) nem layout visual. |
| **Teste Manual de UI** | Usabilidade, teclado, mensagens amigáveis e execução fora da IDE. | Lento para repetir dezenas de fronteiras a cada alteração. |

---

## 2. `Debug` vs `Release`, RID (`win-x64`) e Publicação Autocontida (`A25.04`)

- **`Release`:** ativa otimizações do compilador para entrega real.
- **`win-x64` (RID):** Windows 64 bits.
- **Autocontido (`Self-Contained`):** inclui o motor do .NET dentro da pasta publicada — roda em máquinas sem .NET Runtime instalado, mas a pasta inteira precisa ir junta (**nunca copie só o `.exe`!**) e exige republicação em atualizações de segurança do .NET.

---

## 3. Código Completo e Compilável do Projeto `SupermercadoApp.Verificacoes` (`A25.02`)

### Passo 1 — Criar o Projeto e Ajustar o `.csproj` no Visual Studio 2022
1. Botão direito na **Solução `Supermercado`** → **Adicionar → Novo Projeto...** → **Aplicativo de Console** (C#) → nomeie **`SupermercadoApp.Verificacoes`**.
2. Dê duplo clique em `SupermercadoApp.Verificacoes` para editar o `.csproj` (para evitar erro `NU1201` ao referenciar um projeto Windows Forms!):
   ```xml
   <Project Sdk="Microsoft.NET.Sdk">
     <PropertyGroup>
       <OutputType>Exe</OutputType>
       <TargetFramework>net8.0-windows</TargetFramework>
       <UseWindowsForms>true</UseWindowsForms>
       <ImplicitUsings>enable</ImplicitUsings>
       <Nullable>enable</Nullable>
     </PropertyGroup>
   </Project>
   ```
   *(Use a mesma versão `.NET` do seu `SupermercadoApp`!)*
3. Clique direito em **Dependências** de `SupermercadoApp.Verificacoes` → **Adicionar Referência de Projeto...** → marque **`SupermercadoApp`** → **OK**. *(Mantenha `SupermercadoApp` como Projeto de Inicialização padrão.)*

### Passo 2 — `Program.cs` Completo de Verificações com Fakes (Compila e Executa Offline!)

```csharp
using SupermercadoApp.Models;
using SupermercadoApp.Repositories;
using SupermercadoApp.Services;

int aprovadas = 0;

void Conferir(bool condicao, string descricao)
{
    if (!condicao) throw new Exception($"FALHOU: {descricao}");
    aprovadas++;
    Console.WriteLine($"[OK] {descricao}");
}

async Task ExigirErroAsync<TEx>(Func<Task> acao, string descricao) where TEx : Exception
{
    try { await acao(); throw new Exception($"FALHOU (não lançou {typeof(TEx).Name}): {descricao}"); }
    catch (TEx) { aprovadas++; Console.WriteLine($"[OK] {descricao}"); }
}

var admin = new Sessao(1, "admin", Perfil.Administrador);
var caixa = new Sessao(2, "caixa", Perfil.Caixa);

// 1. Verificação do Carrinho (agrupamento e total R$ 60,30):
var carrinho = new Carrinho();
var arroz = new Produto(1, "000123", "Arroz", 25.90m, 10, true);
var feijao = new Produto(2, "000456", "Feijão", 8.50m, 10, true);
carrinho.Adicionar(arroz, 1);
carrinho.Adicionar(arroz, 1);
carrinho.Adicionar(feijao, 1);
Conferir(carrinho.Itens.Count == 2 && carrinho.Itens[0].Quantidade == 2 && carrinho.Total == 60.30m,
    "Carrinho agrupa arroz (2) + feijão (1) = R$ 60,30");

// 2. Verificação de ProdutoService (regras de fronteira + autorização):
var fakeProd = new FakeProdutoRepo();
var prodSvc = new ProdutoService(fakeProd);
await prodSvc.SalvarAsync(admin, 0, " 000123 ", " Arroz ", 25.90m);
Conferir(fakeProd.ChamadasSalvar == 1, "Admin salva produto válido");
await ExigirErroAsync<ArgumentException>(() => prodSvc.SalvarAsync(admin, 0, "000123", "Arroz", 0m),
    "Recusa preço zero");
await ExigirErroAsync<ArgumentException>(() => prodSvc.SalvarAsync(admin, 0, "000123", "Arroz", 1.999m),
    "Recusa preço com 3 casas decimais");
await ExigirErroAsync<UnauthorizedAccessException>(() => prodSvc.SalvarAsync(caixa, 0, "000123", "Arroz", 10m),
    "Caixa é recusado em ProdutoService");
Conferir(fakeProd.ChamadasSalvar == 1, "Nenhuma chamada inválida chegou ao repositório");

// 3. Verificação de AutenticacaoService (hash+salt e preservação de espaços na senha):
var fakeUser = new FakeUsuarioRepo();
var authSvc = new AutenticacaoService(fakeUser);
await authSvc.PrepararAdministradorAsync("Admin.Lab", "  Senha Com Espaco  ");
var sessaoOk = await authSvc.EntrarAsync("admin.lab", "  Senha Com Espaco  ");
var sessaoSemEspaco = await authSvc.EntrarAsync("admin.lab", "Senha Com Espaco");
Conferir(sessaoOk is { Perfil: Perfil.Administrador }, "Login com normalização e espaços preservados");
Conferir(sessaoSemEspaco is null, "Senha sem os espaços originais é recusada");

// 4. Verificação de VendaService (intervalo invertido e permissão):
var vendaSvc = new VendaService(new FakeVendaRepo(), fakeProd);
var hojeUtc = DateTime.UtcNow;
await ExigirErroAsync<ArgumentException>(() => vendaSvc.RelatorioAsync(admin, hojeUtc, hojeUtc),
    "Relatório recusa intervalo vazio/invertido");
await ExigirErroAsync<UnauthorizedAccessException>(() => vendaSvc.RelatorioAsync(caixa, hojeUtc.AddDays(-1), hojeUtc),
    "Caixa é recusado no relatório financeiro");

Console.WriteLine($"\nTotal de verificações aprovadas: {aprovadas}");

// --- FAKES EM MEMÓRIA ---
sealed class FakeProdutoRepo : IProdutoRepository
{
    public int ChamadasSalvar { get; private set; }
    public Task<IReadOnlyList<Produto>> ListarAsync(string busca) => Task.FromResult<IReadOnlyList<Produto>>([]);
    public Task<Produto?> BuscarCodigoAsync(string codigo) => Task.FromResult<Produto?>(null);
    public Task SalvarAsync(int id, string codigo, string nome, decimal preco) { ChamadasSalvar++; return Task.CompletedTask; }
    public Task InativarAsync(int id) => Task.CompletedTask;
    public Task ReporAsync(int id, int quantidade, int usuarioId) => Task.CompletedTask;
}

sealed class FakeUsuarioRepo : IUsuarioRepository
{
    private readonly List<Credencial> lista = [];
    public Task<Credencial?> BuscarAsync(string username) => Task.FromResult(lista.FirstOrDefault(u => u.Username == username));
    public Task CriarAsync(string username, string hash, Perfil perfil) { lista.Add(new(lista.Count + 1, username, hash, perfil)); return Task.CompletedTask; }
    public Task AtualizarHashAsync(int id, string hash) => Task.CompletedTask;
    public Task<bool> ExisteAsync() => Task.FromResult(lista.Count > 0);
}

sealed class FakeVendaRepo : IVendaRepository
{
    public Task<VendaConfirmada?> BuscarAsync(Guid operacao) => Task.FromResult<VendaConfirmada?>(null);
    public Task<VendaConfirmada> FinalizarAsync(Guid op, int u, IReadOnlyList<PedidoItem> itens, decimal rec) => Task.FromResult(new VendaConfirmada(1, 10m, rec));
    public Task<IReadOnlyList<ResumoVenda>> RelatorioAsync(DateTime i, DateTime f) => Task.FromResult<IReadOnlyList<ResumoVenda>>([]);
}
```

### Prova de Regressão Obrigatória (`A25.02`)
Altere temporariamente `if (preco <= 0 ...)` em `ProdutoService.cs` para `if (preco < 0 ...)`, execute `SupermercadoApp.Verificacoes` e comprove que `"Recusa preço zero"` **falha detectando o bug**. Restaure `<= 0` e execute novamente comprovando aprovação!

---

## 4. Roteiro Manual em Dupla, Publicação e Execução Fora da IDE (`A25.03`–`A25.06`)

1. **Roteiro manual (`A25.03`):** siga [VALIDACAO-MANUAL.md](VALIDACAO-MANUAL.md) em dupla invertendo os papéis de Administrador e Caixa. Marque honestamente como *pendente* o que depender de ambiente ausente.
2. **Publicar pelo VS 2022 (`A25.04`):** botão direito em **`SupermercadoApp`** → **Publicar...** → **Pasta** (`artifacts\publish`) → **Mostrar todas as configurações**:
   - **Configuração:** `Release` | **Modo:** `Autocontido` (*Self-contained*) | **Runtime:** `win-x64` | deixe *Arquivo único* e *Trimming* desmarcados → **Publicar**.
3. **Executar fora do VS 2022 (`A25.05`):** feche o Visual Studio, confirme `SUPERMERCADO_DB` na conta Windows, abra `artifacts\publish` e rode **`SupermercadoApp.exe`** demonstrando login por perfil e uma venda fictícia completa.
4. **Entrega final (`A25.06`):** fontes sem `bin`/`obj`/`.vs`/credenciais, evidências por ID, versão entregue e registro explícito dos limites (dinheiro simulado, sem valor fiscal, conexão direta didática, tentativa pendente volátil na RAM).

Consulte também: [Implantação de Aplicativos .NET](https://learn.microsoft.com/en-us/dotnet/core/deploying/).

[Anterior: Aula 24](24-Cadastro-de-Usuarios.md) · [Índice Geral](README.md)

## Atividades por conceito

Realize as atividades da [aula 25 no caderno complementar](ATIVIDADES-POR-CONCEITO.md#aula-25). Cada conceito tem enunciado e critério de aceite. Reúna as evidências em uma entrega por aula, usando os IDs no [caderno de evidências](CADERNO-DE-EVIDENCIAS.md). Essas tarefas integram a prática e a verificação já previstas no encontro.
