using MinhasAnotacoes.Models;
using MinhasAnotacoes.Services;
using System.Windows.Input;

namespace MinhasAnotacoes.Views;

public partial class NovaNotaPage : ContentPage
{
    // Serviço para gerenciar as notas
    private readonly NotaService _notaService = App.BancoDeDados;

    // Variável para armazenar a nota atual
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

            // Define o título da página como "Editar Nota"
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
            bool sair = await DisplayAlert("Sair sem salvar?", "Você fez alterações. Deseja sair sem salvar?", "Sim", "Não");

            if (sair)
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                return;
            }
        }

        // Navega paa trás se _title não estiver vazio
        await Shell.Current.GoToAsync("..");
    }

    protected override bool OnBackButtonPressed()
    {
        ExecuteBackCommand();
        return true; // Impede o comportamento padrão do botão Voltar
    }

    private async void Salvar_Clicked(object sender, EventArgs e)
    {
        // Verifica se o título e o texto estão vazios
        if (string.IsNullOrWhiteSpace(TituloEntry.Text) && string.IsNullOrWhiteSpace(TextoEditor.Text))
        {
            // Exibe um alerta se ambos os campos estiverem vazios
            await DisplayAlert("Aviso", "Você precisa escrever algo!", "OK");
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
        // Verifica se a nota não é nula
        if (_nota != null)
        {
            // Confirma se o usuário deseja excluir a nota
            var resultado = await DisplayAlert("Excluir Nota", "Tem certeza que deseja excluir esta nota?", "Sim", "Não");
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
            await DisplayAlert("Aviso", "Você precisa escrever algo antes de compartilhar!", "OK");
            return;
        }

        var mensagem = $"{TituloEntry.Text}\n\n{TextoEditor.Text}";

        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Text = mensagem,
            Title = "Compartilhar anotação"
        });
    }
}