using MinhasAnotacoes.Models;
using MinhasAnotacoes.Services;

namespace MinhasAnotacoes.Views;

public partial class MainPage : ContentPage
{
    // Servi�o para gerenciar as notas
    private readonly NotaService _notaService = App.BancoDeDados;

    // Vari�vel para controlar se o item foi arrastado
    private bool _swiped = false;

    public MainPage()
    {
        InitializeComponent();
    }

    // M�todo chamado quando a p�gina aparece
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarNotas();
    }

    // M�todo para carregar as notas
    private async Task CarregarNotas()
    {
        var notas = await _notaService.GetNotasAsync();
        NotasCollection.ItemsSource = notas;
    }

    // M�todo chamado quando o bot�o de adicionar nota � clicado
    private async void NovaNota_Clicked(object sender, EventArgs e)
    {
        // Navega para a p�gina de nova nota
        await Navigation.PushAsync(new NovaNotaPage());
    }

    // M�todo chamado quando um item da lista � selecionado
    private async void NotasCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Verifica se o item foi arrastado
        if (_swiped)
        {
            // Se o item foi arrastado, n�o faz nada
            return;
        }

        if (e.CurrentSelection.FirstOrDefault() is Nota notaSelecionada)
        {
            // Navega para a p�gina de edi��o da nota selecionada
            await Navigation.PushAsync(new NovaNotaPage(notaSelecionada));

            // Limpa a sele��o ap�s navegar
            NotasCollection.SelectedItem = null;
        }
    }

    // M�todo chamado quando um item da lista � arrastado para a esquerda
    private void SwipeItem_Share_Invoked(object sender, EventArgs e)
    {
        // Pega o item que foi arrastado
        var item = (sender as SwipeItem)?.BindingContext as Nota;
        if (item != null)
        {
            // Cria uma mensagem com o t�tulo e texto da nota
            var mensagem = $"{item.Titulo}\n\n{item.Texto}";

            // Cria um objeto de compartilhamento com a mensagem
            Share.RequestAsync(new ShareTextRequest
            {
                Text = mensagem,
                Title = "Compartilhar Nota"
            });
        }
    }

    // M�todo chamado quando o item � arrastado para a direita
    private async void SwipeItem_Delete_Invoked(object sender, EventArgs e)
    {
        // Pega o item que foi arrastado
        var item = (sender as SwipeItem)?.BindingContext as Nota;
        if (item != null)
        {
            // Mostra um alerta de confirma��o para excluir a nota
            var resultado = await DisplayAlert("Excluir Nota", "Voc� tem certeza que deseja excluir esta nota?", "Sim", "N�o");

            // Se o usu�rio confirmar, deleta a nota
            if (resultado)
            {
                await _notaService.DeletarNotaAsync(item);
                await CarregarNotas();
            }
        }
    }

    // M�todo chamado quando o item � arrastado
    private void SwipeView_SwipeStarted(object sender, SwipeStartedEventArgs e)
    {
        // Define a vari�vel _swiped como true quando o item � arrastado
        _swiped = true;
    }

    // M�todo chamado quando o item para de ser arrastado
    private void SwipeView_SwipeEnded(object sender, SwipeEndedEventArgs e)
    {
        // Define a vari�vel _swiped como false quando o item para de ser arrastado
        _swiped = false;

        // Remove a sele��o do item
        NotasCollection.SelectedItem = null;
    }
}