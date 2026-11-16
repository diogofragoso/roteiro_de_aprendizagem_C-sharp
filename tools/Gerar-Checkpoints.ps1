param([string]$Destino = 'artifacts/checkpoints')
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
$saida = [IO.Path]::GetFullPath((Join-Path $repo $Destino))
$permitido = [IO.Path]::GetFullPath((Join-Path $repo 'artifacts')) + [IO.Path]::DirectorySeparatorChar
if (-not $saida.StartsWith($permitido, [StringComparison]::OrdinalIgnoreCase)) { throw 'Destino deve ficar dentro de artifacts.' }
if (Test-Path -LiteralPath $saida) { throw 'Destino ja existe. Escolha outra pasta para preservar trabalhos anteriores.' }
$utf8 = New-Object Text.UTF8Encoding($false)
function Escrever([string]$relativo, [string]$texto) {
    $arquivo = Join-Path $script:etapa $relativo
    [IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($arquivo)) | Out-Null
    [IO.File]::WriteAllText($arquivo, $texto.Replace("`r`n", "`n").TrimEnd() + "`n", $utf8)
}
function Fonte([string]$relativo) { [IO.File]::ReadAllText((Join-Path $repo ('src/SupermercadoApp/' + $relativo))).Replace("`r`n", "`n") }
function Copiar([string]$relativo) { Escrever $relativo (Fonte $relativo) }
# Os membros de primeiro nivel das classes da referencia usam quatro espacos.
# Falha explicitamente se um metodo esperado deixar de existir.
function Membros([string]$texto, [string[]]$nomes) {
    $posicoes = [regex]::Matches($texto, '(?m)^    (?:public|private) [^\n]*\(')
    if ($posicoes.Count -eq 0) { throw 'Classe sem membros reconhecidos' }
    $fim = $texto.LastIndexOf("`n}")
    $resultado = $texto.Substring(0, $posicoes[0].Index)
    $encontrados = @()
    for ($i = 0; $i -lt $posicoes.Count; $i++) {
        $inicio = $posicoes[$i].Index
        $limite = if ($i + 1 -lt $posicoes.Count) { $posicoes[$i+1].Index } else { $fim }
        $trecho = $texto.Substring($inicio, $limite - $inicio)
        foreach ($nome in $nomes) {
            if ($posicoes[$i].Value -match ('\b' + [regex]::Escape($nome) + '\(')) {
                $resultado += $trecho; $encontrados += $nome; break
            }
        }
    }
    foreach ($nome in $nomes) { if ($nome -notin $encontrados) { throw "Membro nao encontrado: $nome" } }
    return $resultado.TrimEnd() + "`n}`n"
}
function Contrato([string]$nome, [string[]]$metodos) {
    $texto = Fonte "Repositories/$nome.cs"
    $declaracoes = [regex]::Matches($texto, '(?m)^    Task[^;]+;')
    foreach ($decl in $declaracoes) {
        if (-not ($metodos | Where-Object { $decl.Value -match ('\b' + $_ + '\(') })) { $texto = $texto.Replace($decl.Value, '') }
    }
    Escrever "Repositories/$nome.cs" $texto
}
$console = @{}
$console[0] = 'Console.WriteLine("Laboratorio pronto. Entrada: valores; processamento: regras; saida: mensagens.");'
$console[1] = @'
using System.Globalization;
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
string nome = "Arroz", codigo = "000123"; char setor = 'A';
int quantidade = 3; long contador = 7891020304050L; double peso = 5.45;
decimal preco = 5.50m; bool ativo = true;
const decimal TAXA_SIMULADA = 0.10m;
Console.WriteLine($"{nome} {codigo} {setor} {contador} {peso} {ativo}");
decimal subtotal = quantidade * preco;
Console.WriteLine($"Subtotal {subtotal:C}; taxa {subtotal*TAXA_SIMULADA:C}; total {subtotal*(1+TAXA_SIMULADA):C}");
int convertido = int.Parse("3");
Console.WriteLine((convertido * decimal.Parse("5.50", CultureInfo.InvariantCulture)).ToString("C"));
foreach (decimal compra in new[] { 199.99m, 200m, 499.99m, 500m })
{
    decimal desconto;
    if (compra >= 500) desconto = 50;
    else if (compra >= 200) desconto = 15;
    else desconto = 0;
    Console.WriteLine($"Compra {compra:C}, desconto {desconto:C}");
}
quantidade = 0;
if (quantidade <= 0) Console.WriteLine("Compra recusada");
foreach (int metodo in new[] { 1, 2, 3, 9 })
    switch (metodo)
    {
        case 1: Console.WriteLine("Dinheiro simulado"); break;
        case 2: Console.WriteLine("Cartao simulado"); break;
        case 3: Console.WriteLine("Pix simulado"); break;
        default: Console.WriteLine("Opcao invalida"); break;
    }
for (int via = 1; via <= 3; via++) Console.WriteLine($"Via {via}");
bool estavel = false; int tentativas = 0;
while (!estavel && tentativas < 5) { tentativas++; Console.WriteLine($"Tentativa {tentativas}"); }
foreach (string produto in new[] { "Arroz", "Feijao", "Macarrao", "Molho" }) Console.WriteLine(produto);
if (subtotal != 16.50m || tentativas != 5 || codigo != "000123") throw new Exception("Falha no checkpoint 01");
'@
$console[2] = @'
using System.Globalization;
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
if (args.Contains("--verificar"))
{
    var leitor = new StringReader("Arroz\nabc\n0\n-1\n3\nabc\n0\n5,50\n");
    Console.SetIn(leitor);
}
Console.Write("Produto: "); string nome = Console.ReadLine() ?? "";
int quantidade;
string? entrada;
do
{
    Console.Write("Quantidade positiva: "); entrada = Console.ReadLine();
    if (entrada is null) { Console.WriteLine("Entrada encerrada."); return; }
} while (!int.TryParse(entrada, out quantidade) || quantidade <= 0);
decimal preco;
do
{
    Console.Write("Preco positivo: "); entrada = Console.ReadLine();
    if (entrada is null) { Console.WriteLine("Entrada encerrada."); return; }
} while (!decimal.TryParse(entrada, out preco) || preco <= 0);
decimal total = quantidade * preco; // Coloque breakpoint nesta linha.
Console.WriteLine($"{nome}: {quantidade} x {preco:C} = {total:C}");
if (args.Contains("--verificar") && total != 16.50m) throw new Exception("Falha na leitura validada");
int estoque = 3;
while (estoque > 0) { estoque--; Console.WriteLine($"Estoque: {estoque}"); }
Console.WriteLine("Estoque vazio");
'@
$console[3] = @'
var arroz = new Produto { Nome = "Arroz", Preco = 25.90m };
var feijao = new Produto { Nome = "Feijao", Preco = 8.50m };
var itens = new List<ItemDeCompra> { new() { Produto = arroz, Quantidade = 2 }, new() { Produto = feijao, Quantidade = 1 } };
decimal total = 0; foreach (var item in itens) total += item.Subtotal;
if (total != 60.30m) throw new Exception("Subtotal incorreto");
feijao.Preco = 9;
if (arroz.Preco != 25.90m) throw new Exception("Objetos deveriam ser independentes");
var usuario = new Usuario(); usuario.Renomear("operador");
try { usuario.Renomear(""); throw new Exception("Nome vazio aceito"); }
catch (ArgumentException) { Console.WriteLine(usuario.Username); }
new Cliente { Nome = "Cliente ficticio", Pontos = 2 }.Exibir();
new Funcionario { Nome = "Funcionario ficticio", Salario = 1000 }.Exibir();
foreach (IPagamento pagamento in new IPagamento[] { new Dinheiro(), new Cartao() }) pagamento.Processar(total);
Console.WriteLine($"Total: {total}");
'@
$console[4] = @'
IConsultaProdutos repositorio = new ProdutosEmMemoria();
var servico = new ConsultaProdutosService(repositorio);
Console.WriteLine(servico.Executar()[0].Nome);
var alternativo = new ConsultaProdutosService(new ProdutosPromocionais());
if (alternativo.Executar()[0].Nome != "Cafe") throw new Exception("Dependencia nao foi substituida");
try { servico.ValidarPreco(-1); throw new Exception("Preco invalido aceito"); }
catch (ArgumentException erro) { Console.WriteLine(erro.Message); }
'@
for ($n = 0; $n -le 20; $n++) {
    $num = '{0:D2}' -f $n
    $script:etapa = Join-Path $saida $num
    [IO.Directory]::CreateDirectory($script:etapa) | Out-Null
    if ($n -ge 5) {
        foreach ($pasta in @('Models','Services','Repositories','Infrastructure','Views')) {
            [IO.Directory]::CreateDirectory((Join-Path $script:etapa $pasta)) | Out-Null
        }
    }
    Escrever 'global.json' ([IO.File]::ReadAllText((Join-Path $repo 'global.json')))
    $projeto = [IO.File]::ReadAllText((Join-Path $repo 'src/SupermercadoApp/SupermercadoApp.csproj'))
    if ($n -lt 5) { $projeto = $projeto.Replace('WinExe','Exe').Replace('net10.0-windows','net10.0').Replace('<UseWindowsForms>true</UseWindowsForms>','') }
    if ($n -lt 8) { $projeto = [regex]::Replace($projeto, '(?s)  <ItemGroup>.*?</ItemGroup>', '') }
    elseif ($n -lt 9) { $projeto = [regex]::Replace($projeto, '    <PackageReference Include="Microsoft.Extensions.Identity.Core"[^\n]+\n', '') }
    Escrever 'SupermercadoApp.csproj' $projeto
    if ($n -lt 5) {
        Escrever 'Program.cs' $console[$n]
        if ($n -ge 3) {
            Escrever 'Produto.cs' 'public class Produto { public string Nome { get; set; } = ""; public decimal Preco { get; set; } }'
            Escrever 'ItemDeCompra.cs' 'public class ItemDeCompra { public Produto Produto { get; set; } = new(); public int Quantidade { get; set; } public decimal Subtotal => Produto.Preco * Quantidade; }'
            Escrever 'Usuario.cs' @'
public class Usuario
{
    public string Username { get; private set; } = "";
    public void Renomear(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome obrigatorio");
        Username = nome.Trim();
    }
}
'@
            Escrever 'Pessoas.cs' @'
public class Pessoa { public string Nome { get; set; } = ""; public void Exibir() => Console.WriteLine(Nome); }
public class Cliente : Pessoa { public int Pontos { get; set; } }
public class Funcionario : Pessoa { public decimal Salario { get; set; } }
'@
            Escrever 'Pagamentos.cs' @'
public interface IPagamento { void Processar(decimal valor); }
public class Dinheiro : IPagamento { public void Processar(decimal valor) => Console.WriteLine($"Dinheiro simulado {valor}"); }
public class Cartao : IPagamento { public void Processar(decimal valor) => Console.WriteLine($"Cartao simulado {valor}"); }
'@
        }
        if ($n -eq 4) {
            Escrever 'IConsultaProdutos.cs' 'public interface IConsultaProdutos { IReadOnlyList<Produto> Listar(); }'
            Escrever 'ProdutosEmMemoria.cs' 'public class ProdutosEmMemoria : IConsultaProdutos { public IReadOnlyList<Produto> Listar() => new List<Produto> { new() { Nome = "Arroz", Preco = 25.90m } }; }'
            Escrever 'ProdutosPromocionais.cs' 'public class ProdutosPromocionais : IConsultaProdutos { public IReadOnlyList<Produto> Listar() => new List<Produto> { new() { Nome = "Cafe", Preco = 12.90m } }; }'
            Escrever 'ConsultaProdutosService.cs' @'
public class ConsultaProdutosService(IConsultaProdutos produtos)
{
    public IReadOnlyList<Produto> Executar() => produtos.Listar();
    public void ValidarPreco(decimal preco) { if (preco <= 0) throw new ArgumentException("Preco positivo obrigatorio"); }
}
'@
        }
    }
    elseif ($n -le 8) {
        $comportamento = if ($n -ge 6) { @'
    private void Entrar(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtUsuario.Text) || txtSenha.Text.Length == 0)
        { MessageBox.Show("Preencha usuario e senha."); return; }
        MessageBox.Show("Campos preenchidos. Autenticacao entra na aula 09.");
    }
'@ } else { '' }
        $campo = ''; $parametro = ''; $atribuicao = ''; $diagnostico = ''
        if ($n -eq 8) {
            Copiar 'Infrastructure/Banco.cs'
            $campo = 'private readonly Npgsql.NpgsqlDataSource banco;'
            $parametro = 'Npgsql.NpgsqlDataSource banco'
            $atribuicao = 'this.banco = banco;'
            $diagnostico = @'
        var teste = new Button { Text = "Testar conexao", Top = 210, Width = 180 };
        Controls.Add(teste);
        teste.Click += async (_, _) =>
        {
            teste.Enabled = false;
            try { await using var cmd = banco.CreateCommand("SELECT 1"); MessageBox.Show($"Resultado: {await cmd.ExecuteScalarAsync()}"); }
            catch (Exception) { MessageBox.Show("Verifique rede e configuracao."); }
            finally { teste.Enabled = true; }
        };
'@
        }
        Escrever 'Views/FormLogin.cs' @"
namespace SupermercadoApp.Views;
public partial class FormLogin : Form
{
    $campo
    public FormLogin($parametro) { InitializeComponent(); $atribuicao
    $diagnostico
    }
    $comportamento
}
"@
        $controles = if ($n -ge 6) { @'
        txtUsuario = new TextBox { Name = "txtUsuario", Top = 50, Width = 240, TabIndex = 0, MaxLength = 50 };
        txtSenha = new TextBox { Name = "txtSenha", Top = 110, Width = 240, TabIndex = 1, MaxLength = 128, UseSystemPasswordChar = true };
        btnLogin = new Button { Name = "btnLogin", Text = "Entrar", Top = 160, TabIndex = 2 };
        Controls.Add(new Label { Text = "Usuario", Top = 25 }); Controls.Add(txtUsuario);
        Controls.Add(new Label { Text = "Senha", Top = 85 }); Controls.Add(txtSenha); Controls.Add(btnLogin);
        AcceptButton = btnLogin; btnLogin.Click += Entrar;
'@ } else { '' }
        $declaracoes = if ($n -ge 6) { 'private TextBox txtUsuario = null!; private TextBox txtSenha = null!; private Button btnLogin = null!;' } else { '' }
        Escrever 'Views/FormLogin.Designer.cs' @"
namespace SupermercadoApp.Views;
partial class FormLogin
{
    $declaracoes
    private void InitializeComponent()
    {
        Text = "MarketFlow - Login"; ClientSize = new Size(420, 300);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle; MaximizeBox = false;
        $controles
    }
}
"@
        $abertura = if ($n -eq 8) { 'using var banco = SupermercadoApp.Infrastructure.Banco.Criar(); Application.Run(new SupermercadoApp.Views.FormLogin(banco));' } else { 'Application.Run(new SupermercadoApp.Views.FormLogin());' }
        Escrever 'Program.cs' @"
internal static class Program
{
    [STAThread] private static void Main()
    {
        ApplicationConfiguration.Initialize();
        try { $abertura }
        catch (InvalidOperationException erro) { MessageBox.Show(erro.Message); }
    }
}
"@
    }
    else {
        foreach ($arquivo in @('Infrastructure/Banco.cs','Models/Perfil.cs','Models/Credencial.cs','Models/Sessao.cs','Repositories/IUsuarioRepository.cs','Repositories/UsuarioRepository.cs','Services/AutenticacaoService.cs','Views/FormLogin.cs','Views/Ui.cs')) { Copiar $arquivo }
        if ($n -ge 10) { Copiar 'Views/FormUsuarios.cs' }
        if ($n -ge 11) {
            Copiar 'Models/Produto.cs'
            $metodos = @('SalvarAsync')
            if ($n -ge 12) { $metodos += 'ListarAsync' }
            if ($n -ge 13) { $metodos += 'InativarAsync' }
            if ($n -ge 14) { $metodos += 'ReporAsync' }
            $servico = Membros (Fonte 'Services/ProdutoService.cs') $metodos
            Escrever 'Services/ProdutoService.cs' $servico
            if ($n -ge 15) { $metodos += 'BuscarCodigoAsync' }
            Contrato 'IProdutoRepository' $metodos
            if ($n -eq 11) {
                Escrever 'Repositories/ProdutoEmMemoriaRepository.cs' @'
using SupermercadoApp.Repositories;
namespace SupermercadoApp.Repositories;
public sealed class ProdutoEmMemoriaRepository : IProdutoRepository
{
    public int Chamadas { get; private set; }
    public Task SalvarAsync(int id, string codigo, string nome, decimal preco)
    { Chamadas++; return Task.CompletedTask; }
}
'@
            } else {
                Escrever 'Repositories/ProdutoRepository.cs' (Membros (Fonte 'Repositories/ProdutoRepository.cs') (@('Ler') + $metodos))
                if ($n -eq 12) {
                    Escrever 'Views/FormProdutos.cs' @'
using SupermercadoApp.Models;
using SupermercadoApp.Services;
namespace SupermercadoApp.Views;
public sealed class FormProdutos : Form
{
    public FormProdutos(Sessao sessao, ProdutoService produtos)
    {
        Permissoes.ExigirAdministrador(sessao);
        var p = Ui.Preparar(this, "Produtos");
        var busca = Ui.Texto(p, "Nome ou codigo - ate 200 resultados");
        var grade = Ui.Grade(p);
        async Task Carregar() => grade.DataSource = (await produtos.ListarAsync(sessao, busca.Text)).ToList();
        Ui.Botao(p, "Buscar", async (_, _) => await Ui.ExecutarAsync(Carregar));
        Shown += async (_, _) => await Ui.ExecutarAsync(Carregar);
    }
}
'@
                } elseif ($n -eq 13) {
                    $form = Fonte 'Views/FormProdutos.cs'
                    $a = $form.IndexOf('        p.Controls.Add(Ui.Rotulo("Quantidade para repor')
                    $b = $form.IndexOf('        Shown +=', $a)
                    if ($a -lt 0 -or $b -lt 0) { throw 'Bloco de reposicao nao localizado' }
                    Escrever 'Views/FormProdutos.cs' ($form.Remove($a, $b-$a))
                } else { Copiar 'Views/FormProdutos.cs' }
            }
        }
        if ($n -ge 15) {
            Copiar 'Models/ItemCarrinho.cs'; Copiar 'Services/Carrinho.cs'
            $venda = Fonte 'Services/VendaService.cs'
            if ($n -eq 15) { $venda = $venda.Replace('IVendaRepository vendas, IProdutoRepository produtos','IProdutoRepository produtos'); $venda = Membros $venda @('BuscarProdutoAsync') }
            elseif ($n -lt 18) { $venda = Membros $venda @('BuscarProdutoAsync','ConsultarAsync','FinalizarAsync') }
            Escrever 'Services/VendaService.cs' $venda
            if ($n -ge 16) {
                foreach ($modelo in @('PedidoItem','VendaConfirmada','ResumoVenda')) { Copiar "Models/$modelo.cs" }
                $metodosVenda = @('BuscarAsync','FinalizarAsync')
                if ($n -ge 18) { $metodosVenda += 'RelatorioAsync' }
                Contrato 'IVendaRepository' $metodosVenda
                Escrever 'Repositories/VendaRepository.cs' (Membros (Fonte 'Repositories/VendaRepository.cs') $metodosVenda)
            }
            if ($n -ge 17) { Copiar 'Views/FormCaixa.cs' }
            else {
                $form = Fonte 'Views/FormCaixa.cs'
                $a = $form.IndexOf('        edicao.Controls.Add(Ui.Rotulo("Valor recebido')
                $form = $form.Substring(0,$a)
                $form = $form.Replace('    private bool pendente;', '').Replace('    private decimal recebidoPendente;', '')
                if ($n -eq 15) { $form = $form.Replace('    private Guid operacao = Guid.NewGuid();','').Replace('operacao = Guid.NewGuid();','') }
                if ($n -eq 16) {
                    $form += @'
        Ui.Botao(p, "Teste controlado: recebido 100 / mesma chave", async (_, _) =>
            await Ui.ExecutarAsync(async () =>
            {
                var resultado = await vendas.FinalizarAsync(sessao, operacao, carrinho.Itens, 100m);
                MessageBox.Show($"Venda {resultado.Id}; total {resultado.Total:C}; operacao {operacao}");
            }));
'@
                }
                $form += "`n        Ui.Botao(p, `"Sair / trocar usuario`", (_, _) => Close());`n    }`n}`n"
                # Campo executando so e lido pelo pagamento definitivo na aula 17.
                $form = $form.Replace('    private bool executando;', '').Replace('executando = true;', '').Replace('executando = false;', '')
                Escrever 'Views/FormCaixa.cs' $form
            }
        }
        if ($n -ge 18) { Copiar 'Views/FormRelatorio.cs' }
        if ($n -ge 10) {
            $paramAdmin = 'Sessao sessao'
            $botoes = ''
            if ($n -ge 11) { $paramAdmin += ', ProdutoService produtos' }
            if ($n -eq 11) { $botoes += 'Ui.Botao(p, "Verificar preco zero", async (_, _) => await Ui.ExecutarAsync(() => produtos.SalvarAsync(sessao, 0, "000123", "Arroz", 0)));' }
            if ($n -ge 12) { $botoes += 'Ui.Botao(p, "Produtos", (_, _) => { using var f = new FormProdutos(sessao, produtos); f.ShowDialog(this); });' }
            if ($n -ge 18) { $paramAdmin += ', VendaService vendas'; $botoes += 'Ui.Botao(p, "Relatorio", (_, _) => { using var f = new FormRelatorio(sessao, vendas); f.ShowDialog(this); });' }
            if ($n -ge 19) { $paramAdmin += ', AutenticacaoService usuarios'; $botoes += 'Ui.Botao(p, "Usuarios", (_, _) => { using var f = new FormUsuarios(sessao, usuarios); f.ShowDialog(this); });' }
            Escrever 'Views/FormAdministrador.cs' @"
using SupermercadoApp.Models;
using SupermercadoApp.Services;
namespace SupermercadoApp.Views;
public sealed class FormAdministrador : Form
{
    public FormAdministrador($paramAdmin)
    {
        Permissoes.ExigirAdministrador(sessao);
        var p = Ui.Preparar(this, "Administracao", 480, 350);
        p.Controls.Add(Ui.Rotulo(sessao.Username));
        $botoes
        Ui.Botao(p, "Sair / trocar usuario", (_, _) => Close());
    }
}
"@
            if ($n -ge 19) { Copiar 'Views/FormAdministrador.cs' }
            if ($n -lt 15) {
                Escrever 'Views/FormCaixa.cs' @'
using SupermercadoApp.Models;
using SupermercadoApp.Services;
namespace SupermercadoApp.Views;
public sealed class FormCaixa : Form
{
    public FormCaixa(Sessao sessao)
    {
        Permissoes.ExigirCaixa(sessao);
        var p = Ui.Preparar(this, "Caixa", 480, 300);
        p.Controls.Add(Ui.Rotulo(sessao.Username));
        Ui.Botao(p, "Sair / trocar usuario", (_, _) => Close());
    }
}
'@
            }
        }
        $composicao = ''
        if ($n -eq 11) { $composicao += 'var produtoService = new ProdutoService(new ProdutoEmMemoriaRepository());' }
        if ($n -ge 12) { $composicao += 'var produtos = new ProdutoRepository(banco); var produtoService = new ProdutoService(produtos);' }
        if ($n -eq 15) { $composicao += 'var vendaService = new VendaService(produtos);' }
        if ($n -ge 16) { $composicao += 'var vendaService = new VendaService(new VendaRepository(banco), produtos);' }
        $abrir = 'MessageBox.Show($"Bem-vindo, {sessao.Username}");'
        if ($n -ge 10) {
            $adminArgs = 'sessao'; $caixaArgs = 'sessao'
            if ($n -ge 11) { $adminArgs += ', produtoService' }
            if ($n -ge 15) { $caixaArgs += ', vendaService' }
            if ($n -ge 18) { $adminArgs += ', vendaService' }
            if ($n -ge 19) { $adminArgs = 'sessao, produtoService, autenticacao, vendaService' }
            $abrir = @"
if (args.Contains("--preparar-caixa"))
{
    Permissoes.ExigirAdministrador(sessao);
    using var cadastro = new FormUsuarios(sessao, autenticacao); cadastro.ShowDialog(); return;
}
using Form principal = sessao.Perfil == Perfil.Administrador
    ? new FormAdministrador($adminArgs) : new FormCaixa($caixaArgs);
principal.ShowDialog();
"@
        }
        Escrever 'Program.cs' @"
using System.Globalization;
using SupermercadoApp.Infrastructure;
using SupermercadoApp.Models;
using SupermercadoApp.Repositories;
using SupermercadoApp.Services;
using SupermercadoApp.Views;
internal static class Program
{
    [STAThread] private static void Main(string[] args)
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
        ApplicationConfiguration.Initialize();
        try
        {
            using var banco = Banco.Criar();
            var autenticacao = new AutenticacaoService(new UsuarioRepository(banco));
            $composicao
            if (args.Contains("--preparar-admin"))
            { using var preparo = new FormLogin(autenticacao, true); preparo.ShowDialog(); return; }
            while (true)
            {
                using var login = new FormLogin(autenticacao);
                if (login.ShowDialog() != DialogResult.OK || login.SessaoAutenticada is not { } sessao) break;
                $abrir
            }
        }
        catch (Exception erro)
        { MessageBox.Show(erro is InvalidOperationException or UnauthorizedAccessException ? erro.Message : "Falha ao iniciar. Confira configuracao."); }
    }
}
"@
    }
    if ($n -ge 7) { Escrever 'database/001-usuarios.sql' ([IO.File]::ReadAllText((Join-Path $repo 'database/001-usuarios.sql'))) }
    if ($n -ge 11) { Escrever 'database/002-produtos.sql' ([IO.File]::ReadAllText((Join-Path $repo 'database/002-produtos.sql'))) }
    if ($n -ge 16) { Escrever 'database/003-vendas.sql' ([IO.File]::ReadAllText((Join-Path $repo 'database/003-vendas.sql'))) }
    if ($n -eq 20) {
        $teste = [IO.File]::ReadAllText((Join-Path $repo 'tests/SupermercadoApp.Verificacoes/SupermercadoApp.Verificacoes.csproj')).Replace('../../src/SupermercadoApp/SupermercadoApp.csproj','../../SupermercadoApp.csproj')
        # Evita incluir os arquivos do console de testes no projeto principal.
        $projeto = $projeto.Replace('</Project>', '<ItemGroup><Compile Remove="tests/**/*.cs" /></ItemGroup></Project>')
        Escrever 'SupermercadoApp.csproj' $projeto
        Escrever 'tests/Verificacoes/Verificacoes.csproj' $teste
        Escrever 'tests/Verificacoes/Program.cs' ([IO.File]::ReadAllText((Join-Path $repo 'tests/SupermercadoApp.Verificacoes/Program.cs')))
    }
    $inventario = Get-ChildItem -LiteralPath $script:etapa -Recurse -File | ForEach-Object { $_.FullName.Substring($script:etapa.Length + 1).Replace('\','/') } | Sort-Object
    $assinaturas = foreach ($fonte in Get-ChildItem -LiteralPath $script:etapa -Recurse -File -Filter '*.cs') {
        $relativo = $fonte.FullName.Substring($script:etapa.Length + 1)
        foreach ($assinatura in [regex]::Matches([IO.File]::ReadAllText($fonte.FullName), '(?m)^\s*public (?:static )?(?:async )?[^\r\n]+\(')) {
            $relativo + ': ' + $assinatura.Value.Trim()
        }
    }
    Escrever 'METODOS.txt' ($assinaturas -join "`n")
    $lista = ($inventario | ForEach-Object { '- ' + $_ }) -join "`n"
    $preparo = if ($n -lt 8) { 'Sem banco para executar o aplicativo nesta etapa.' } else { 'Configure SUPERMERCADO_DB localmente e execute os scripts disponiveis na ordem, uma vez. Nao publique credenciais.' }
    $ferramenta = if ($n -ge 10 -and $n -lt 19) { 'FormUsuarios esta antecipado apenas para --preparar-caixa, ferramenta do professor com autenticacao administrativa. Nao aparece no menu normal.' } else { '' }
    Escrever 'README.md' @"
# Checkpoint $num

Resultado da aula $num. Para comecar a aula seguinte, copie esta pasta para sua area de trabalho. A versao 00 e o console inicial da aula 01. Nao modifique os checkpoints de outros estudantes.

$preparo

$ferramenta

Compilar: dotnet build SupermercadoApp.csproj -c Release

Executar: dotnet run --project SupermercadoApp.csproj

Consoles 00-04: acrescente -- --verificar para verificacao sem entrada interativa.

Os fontes deste checkpoint usam apenas os recursos introduzidos ate esta etapa, exceto a ferramenta docente identificada acima. A versao 09 oferece login em codigo para recuperacao; o adaptador Designer esta no capitulo.

METODOS.txt lista assinaturas publicas para comparar com a etapa anterior. O mapa de arquivos/metodos por aula tambem esta em src/README.md da referencia.

## Inventario

$lista
"@
}
Write-Output "21 checkpoints gerados em $saida"
