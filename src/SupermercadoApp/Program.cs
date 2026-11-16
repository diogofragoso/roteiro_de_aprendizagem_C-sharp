using System.Globalization;
using SupermercadoApp.Infrastructure;
using SupermercadoApp.Models;
using SupermercadoApp.Repositories;
using SupermercadoApp.Services;
using SupermercadoApp.Views;

namespace SupermercadoApp;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("pt-BR");
        ApplicationConfiguration.Initialize();
        try
        {
            using var banco = Banco.Criar();
            var usuarios = new UsuarioRepository(banco);
            var produtos = new ProdutoRepository(banco);
            var autenticacao = new AutenticacaoService(usuarios);
            var produtoService = new ProdutoService(produtos);
            var vendaService = new VendaService(new VendaRepository(banco), produtos);
            if (args.Contains("--preparar-admin"))
            {
                using var preparacao = new FormLogin(autenticacao, prepararAdmin: true);
                preparacao.ShowDialog();
                return;
            }
            // O login é descartado após cada sessão; não há usuário global estático.
            while (true)
            {
                using var login = new FormLogin(autenticacao);
                if (login.ShowDialog() != DialogResult.OK || login.SessaoAutenticada is not { } sessao) break;
                if (args.Contains("--preparar-caixa"))
                {
                    Permissoes.ExigirAdministrador(sessao);
                    using var cadastro = new FormUsuarios(sessao, autenticacao);
                    cadastro.ShowDialog();
                    return;
                }
                using Form principal = sessao.Perfil == Perfil.Administrador
                    ? new FormAdministrador(sessao, produtoService, autenticacao, vendaService)
                    : new FormCaixa(sessao, vendaService);
                principal.ShowDialog();
            }
        }
        catch (Exception e)
        {
            System.Diagnostics.Trace.WriteLine($"Falha ao iniciar: {e.GetType().Name}");
            MessageBox.Show(e is InvalidOperationException ? e.Message : "Falha ao iniciar. Verifique a configuração do banco.");
        }
    }
}
