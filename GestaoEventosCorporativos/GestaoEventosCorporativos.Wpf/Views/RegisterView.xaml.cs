using System.Windows;
using System.Windows.Controls;

namespace GestaoEventosCorporativos.Wpf.Views
{
    public partial class RegisterView : UserControl
    {
        private readonly MainWindow _main;

        public RegisterView(MainWindow main)
        {
            InitializeComponent();
            _main = main;
        }

        private void Cadastrar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Usuário cadastrado com sucesso!");

            // Depois do cadastro, volta para tela de login
            _main.Navigate(new LoginView(_main));
        }
    }
}
