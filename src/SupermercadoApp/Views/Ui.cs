using Npgsql;

namespace SupermercadoApp.Views;

internal static class Ui
{
    public static FlowLayoutPanel Preparar(Form form, string titulo, int largura = 960, int altura = 650)
    {
        form.Text = titulo;
        form.Size = new Size(largura, altura);
        form.StartPosition = FormStartPosition.CenterScreen;
        var painel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true,
            FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(16) };
        form.Controls.Add(painel);
        return painel;
    }
    public static Label Rotulo(string texto) => new() { Text = texto, AutoSize = true, Margin = new Padding(3, 8, 3, 3) };
    public static TextBox Texto(FlowLayoutPanel p, string rotulo, bool senha = false)
    {
        p.Controls.Add(Rotulo(rotulo));
        var campo = new TextBox { Width = 260, UseSystemPasswordChar = senha };
        p.Controls.Add(campo);
        return campo;
    }
    public static Button Botao(FlowLayoutPanel p, string texto, EventHandler clique)
    {
        var botao = new Button { Text = texto, AutoSize = true };
        botao.Click += clique;
        p.Controls.Add(botao);
        return botao;
    }
    public static DataGridView Grade(FlowLayoutPanel p)
    {
        var grade = new DataGridView { Width = 860, Height = 240, ReadOnly = true,
            AllowUserToAddRows = false, AllowUserToDeleteRows = false, MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        p.Controls.Add(grade);
        return grade;
    }
    public static async Task<bool> ExecutarAsync(Func<Task> acao)
    {
        try { await acao(); return true; }
        catch (PostgresException e) when (e.SqlState == PostgresErrorCodes.UniqueViolation)
        { MessageBox.Show("Código ou usuário já cadastrado. Use outro valor."); }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException or UnauthorizedAccessException)
        { MessageBox.Show(e.Message, "Verifique os dados"); }
        catch (Exception e)
        {
            // Nunca registre senha, SQL com valores ou string de conexão.
            System.Diagnostics.Trace.WriteLine($"Falha: {e.GetType().Name}");
            MessageBox.Show("Não foi possível concluir. Verifique a rede e tente novamente.", "Falha de comunicação");
        }
        return false;
    }
}
