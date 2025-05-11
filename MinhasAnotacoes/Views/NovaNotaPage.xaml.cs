using MinhasAnotacoes.Models;
using MinhasAnotacoes.Services;
using System.Windows.Input;

namespace MinhasAnotacoes.Views;

public partial class NovaNotaPage : ContentPage
{
    // Servi�o para gerenciar as notas
    private readonly NotaService _notaService = App.BancoDeDados;

    // Vari�vel para armazenar a nota atual
    private Nota? _nota;

    // Strings que guardam o estado atual da nota quando for editada
    private string _tituloOriginal;
    private string _textoOriginal;

    public ICommand BackCommand { get; set; }

    public NovaNotaPage(Nota nota = null)
    {
        InitializeComponent();

        BackCommand = new Command(ExecuteBackCommand);
        BindingContext = this;

        _nota = nota;

        if (_nota != null)
        {
            TituloEntry.Text = _nota.Titulo;
            TextoEditor.Text = _nota.Texto;

            ExcluirButton.IsVisible = true;
            ShareButton.IsVisible = true;

            // Define o t�tulo da p�gina como "Editar Nota"
            Title = "Editar Nota";
        }

        // Armazena o estado original da nota
        _tituloOriginal = TituloEntry.Text;
        _textoOriginal = TextoEditor.Text;
    }

    private async void ExecuteBackCommand()
    {
        if (TituloEntry.Text != _tituloOriginal || TextoEditor.Text != _textoOriginal)
        {
            bool sair = await DisplayAlert("Sair sem salvar?", "Voc� fez altera��es. Deseja sair sem salvar?", "Sim", "N�o");

            if (sair)
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                return;
            }
        }

        // Navega paa tr�s se _title n�o estiver vazio
        await Shell.Current.GoToAsync("..");
    }

    protected override bool OnBackButtonPressed()
    {
        ExecuteBackCommand();
        return true; // Impede o comportamento padr�o do bot�o Voltar
    }

    private async void Salvar_Clicked(object sender, EventArgs e)
    {
        // Verifica se o t�tulo e o texto est�o vazios
        if (string.IsNullOrWhiteSpace(TituloEntry.Text) && string.IsNullOrWhiteSpace(TextoEditor.Text))
        {
            // Exibe um alerta se ambos os campos estiverem vazios
            await DisplayAlert("Aviso", "Voc� precisa escrever algo!", "OK");
            return;
        }

        // Se a nota for nula, cria uma nova nota
        _nota ??= new Nota();

        // Atualiza os campos da nota
        _nota.Titulo = TituloEntry.Text;
        _nota.Texto = TextoEditor.Text;

        // Salva a nota ou adiciona uma nota nova
        await _notaService.SalvarNotaAsync(_nota);
        await Navigation.PopAsync();
    }

    private async void Excluir_Clicked(object sender, EventArgs e)
    {
        // Verifica se a nota n�o � nula
        if (_nota != null)
        {
            // Confirma se o usu�rio deseja excluir a nota
            var resultado = await DisplayAlert("Excluir Nota", "Tem certeza que deseja excluir esta nota?", "Sim", "N�o");
            if (resultado)
            {
                await _notaService.DeletarNotaAsync(_nota);
                await Navigation.PopAsync();
            }
        }
    }

    private async void Compartilhar_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TituloEntry.Text) && string.IsNullOrWhiteSpace(TextoEditor.Text))
        {
            // Exibe um alerta se ambos os campos estiverem vazios
            await DisplayAlert("Aviso", "Voc� precisa escrever algo antes de compartilhar!", "OK");
            return;
        }

        var mensagem = $"{TituloEntry.Text}\n\n{TextoEditor.Text}";

        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Text = mensagem,
            Title = "Compartilhar anota��o"
        });
    }
}