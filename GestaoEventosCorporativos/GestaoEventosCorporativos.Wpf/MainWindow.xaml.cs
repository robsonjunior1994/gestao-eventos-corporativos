using System.Windows;
using System.Windows.Controls;

namespace GestaoEventosCorporativos.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Abre com a tela de login
            MainContent.Content = new Views.LoginView(this);
        }

        // Método para trocar de view
        public void Navigate(UserControl nextPage)
        {
            MainContent.Content = nextPage;
        }
    }
}
