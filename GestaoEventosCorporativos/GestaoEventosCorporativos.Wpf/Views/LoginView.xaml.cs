using System.Windows;
using System.Windows.Controls;

namespace GestaoEventosCorporativos.Wpf.Views
{
    public partial class LoginView : UserControl
    {
        private readonly MainWindow _main;

        public LoginView(MainWindow main)
        {
            InitializeComponent();
            _main = main;
        }

        private void Cadastrar_Click(object sender, RoutedEventArgs e)
        {
            _main.Navigate(new RegisterView(_main));
        }

        private void Entrar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Login realizado com sucesso!");
        }
    }
}
