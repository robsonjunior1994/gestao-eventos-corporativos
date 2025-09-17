using GestaoEventosCorporativos.Wpf;
using GestaoEventosCorporativos.Wpf.Services;
using System.Windows;

namespace GestaoEventosCorporativos.Views
{
    public partial class LoginWindow : Window
    {
        private readonly UserService _userService;

        public LoginWindow()
        {
            InitializeComponent();
            _userService = new UserService();
        }

        private async void Entrar_Click(object sender, RoutedEventArgs e)
        {
            //var email = txtEmail.Text;
            //var senha = txtPassword.Password;

            //var result = await _userService.LoginAsync(email, senha);

            //if (result != null && result.IsSuccess)
            //{
            //    MessageBox.Show("Login realizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

            //    // aqui você pode abrir a tela principal do sistema
            //    var mainWindow = new MainWindow();
            //    mainWindow.Show();

            //    this.Close();
            //}
            //else
            //{
            //    MessageBox.Show(result?.Message ?? "Erro ao fazer login.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }
    }
}
