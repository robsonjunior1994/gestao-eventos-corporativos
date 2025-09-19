using System.Windows;
using System.Windows.Controls;

namespace GestaoEventosCorporativos.Wpf.Views
{
    public partial class HomeView : UserControl
    {
        private readonly MainWindow _main;

        public HomeView(MainWindow main)
        {
            InitializeComponent();
            _main = main;
        }

        private void BtnTipoEvento_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Agora o TipoEventoView pega o token direto do AppSession
            _main.Navigate(new TipoEventoView(_main));
        }

        private void BtnFornecedor_Click(object sender, RoutedEventArgs e)
        {
            _main.Navigate(new FornecedorView(_main));
        }

        private void BtnParticipante_Click(object sender, RoutedEventArgs e)
        {
            _main.Navigate(new ParticipanteView(_main));
        }

        private void BtnEvento_Click(object sender, RoutedEventArgs e)
        {
            _main.Navigate(new EventoView(_main));
        }

    }
}
