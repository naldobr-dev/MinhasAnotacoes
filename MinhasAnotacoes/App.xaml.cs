using MinhasAnotacoes.Services;

namespace MinhasAnotacoes
{
    public partial class App : Application
    {
        // Cria uma instância do serviço de notas
        public static NotaService BancoDeDados { get; private set; }

        public App()
        {
            InitializeComponent();

            // Inicializa o serviço de notas
            BancoDeDados = new NotaService();

            MainPage = new AppShell();
        }
    }
}
