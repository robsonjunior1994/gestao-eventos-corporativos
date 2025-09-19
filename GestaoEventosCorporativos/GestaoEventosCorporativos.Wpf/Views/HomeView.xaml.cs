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
            _main.Navigate(new TipoEventoView());
        }

        private void BtnFornecedor_Click(object sender, RoutedEventArgs e)
        {
            _main.Navigate(new FornecedorView());
        }

    }
}
