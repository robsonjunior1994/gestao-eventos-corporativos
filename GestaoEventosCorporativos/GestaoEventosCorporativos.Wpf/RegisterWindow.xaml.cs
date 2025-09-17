using GestaoEventosCorporativos.Wpf.DTOs.Request;
using GestaoEventosCorporativos.Wpf.Services;
using System.Windows;

namespace GestaoEventosCorporativos.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly UserService _userService;

        public RegisterWindow()
        {
            InitializeComponent();
            _userService = new UserService();
        }

        private async void Cadastrar_Click(object sender, RoutedEventArgs e)
        {
            var request = new UserRequest
            {
                Name = txtName.Text,
                Email = txtEmail.Text,
                Password = txtPassword.Password
            };

            var result = await _userService.RegisterUserAsync(request);

            if (result != null && result.IsSuccess)
            {
                MessageBox.Show(result.Message, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                // Abre a tela de login
                var loginWindow = new LoginWindow();
                loginWindow.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show(result?.Message ?? "Erro desconhecido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
