using GestaoEventosCorporativos.Wpf.DTOs.Request;
using GestaoEventosCorporativos.Wpf.Services;
using System.Windows;
using System.Windows.Controls;

namespace GestaoEventosCorporativos.Wpf.Views
{
    public partial class LoginView : UserControl
    {
        private readonly MainWindow _main;
        private readonly UserService _userService;

        public LoginView(MainWindow main)
        {
            InitializeComponent();
            _main = main;
            _userService = new UserService();


            //EXCLUIR DEPOIS
            txtEmail.Text = "robson1@example.com";
            txtPassword.Password = "123456789";
        }

        private void Cadastrar_Click(object sender, RoutedEventArgs e)
        {
            _main.Navigate(new RegisterView(_main));
        }

        private async void Entrar_Click(object sender, RoutedEventArgs e)
        {
            var request = new LoginRequest
            {
                Email = txtEmail.Text,
                Password = txtPassword.Password
            };

            var result = await _userService.LoginAsync(request);

            if (result != null && result.IsSuccess && result.Data != null)
            {
                AppSession.Token = result.Data.Token; // 🔑 guarda o token
                _main.Navigate(new HomeView(_main));
            }

            else
            {
                MessageBox.Show(result?.Message ?? "Erro ao realizar login.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
