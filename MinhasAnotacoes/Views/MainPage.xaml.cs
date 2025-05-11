using MinhasAnotacoes.Models;
using MinhasAnotacoes.Services;

namespace MinhasAnotacoes.Views;

public partial class MainPage : ContentPage
{
    // Serviço para gerenciar as notas
    private readonly NotaService _notaService = App.BancoDeDados;

    // Variável para controlar se o item foi arrastado
    private bool _swiped = false;

    public MainPage()
    {
        InitializeComponent();
    }

    // Método chamado quando a página aparece
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarNotas();
    }

    // Método para carregar as notas
    private async Task CarregarNotas()
    {
        var notas = await _notaService.GetNotasAsync();
        NotasCollection.ItemsSource = notas;
    }

    // Método chamado quando o botão de adicionar nota é clicado
    private async void NovaNota_Clicked(object sender, EventArgs e)
    {
        // Navega para a página de nova nota
        await Navigation.PushAsync(new NovaNotaPage());
    }

    // Método chamado quando um item da lista é selecionado
    private async void NotasCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Verifica se o item foi arrastado
        if (_swiped)
        {
            // Se o item foi arrastado, não faz nada
            return;
        }

        if (e.CurrentSelection.FirstOrDefault() is Nota notaSelecionada)
        {
            // Navega para a página de edição da nota selecionada
            await Navigation.PushAsync(new NovaNotaPage(notaSelecionada));

            // Limpa a seleção após navegar
            NotasCollection.SelectedItem = null;
        }
    }

    // Método chamado quando um item da lista é arrastado para a esquerda
    private void SwipeItem_Share_Invoked(object sender, EventArgs e)
    {
        // Pega o item que foi arrastado
        var item = (sender as SwipeItem)?.BindingContext as Nota;
        if (item != null)
        {
            // Cria uma mensagem com o título e texto da nota
            var mensagem = $"{item.Titulo}\n\n{item.Texto}";

            // Cria um objeto de compartilhamento com a mensagem
            Share.RequestAsync(new ShareTextRequest
            {
                Text = mensagem,
                Title = "Compartilhar Nota"
            });
        }
    }

    // Método chamado quando o item é arrastado para a direita
    private async void SwipeItem_Delete_Invoked(object sender, EventArgs e)
    {
        // Pega o item que foi arrastado
        var item = (sender as SwipeItem)?.BindingContext as Nota;
        if (item != null)
        {
            // Mostra um alerta de confirmação para excluir a nota
            var resultado = await DisplayAlert("Excluir Nota", "Você tem certeza que deseja excluir esta nota?", "Sim", "Não");

            // Se o usuário confirmar, deleta a nota
            if (resultado)
            {
                await _notaService.DeletarNotaAsync(item);
                await CarregarNotas();
            }
        }
    }

    // Método chamado quando o item é arrastado
    private void SwipeView_SwipeStarted(object sender, SwipeStartedEventArgs e)
    {
        // Define a variável _swiped como true quando o item é arrastado
        _swiped = true;
    }

    // Método chamado quando o item para de ser arrastado
    private void SwipeView_SwipeEnded(object sender, SwipeEndedEventArgs e)
    {
        // Define a variável _swiped como false quando o item para de ser arrastado
        _swiped = false;

        // Remove a seleção do item
        NotasCollection.SelectedItem = null;
    }
}