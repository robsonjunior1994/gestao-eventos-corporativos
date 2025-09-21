using GestaoEventosCorporativos.Wpf.DTOs.Request;
using GestaoEventosCorporativos.Wpf.Services;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace GestaoEventosCorporativos.Wpf.Views
{
    public partial class RegisterView : UserControl
    {
        private readonly MainWindow _main;
        private readonly UserService _userService;

        public RegisterView(MainWindow main)
        {
            InitializeComponent();
            _main = main;
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

            try
            {
                var result = await _userService.RegisterUserAsync(request);

                if (result != null && result.IsSuccess)
                {
                    
                    MessageBox.Show(
                        $"{result.Message}\n\nUsuário: {result.Data.Name}\nEmail: {result.Data.Email}",
                        "Sucesso",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    _main.Navigate(new LoginView(_main));
                }
                else
                {
                    string errorMessage = $"StatusCode: {result?.StatusCode}\n{result?.Message ?? "Erro ao cadastrar usuário."}";

                    if (result?.Errors != null)
                    {
                        
                        string jsonErrors = JsonSerializer.Serialize(
                            result.Errors,
                            new JsonSerializerOptions { WriteIndented = true });

                        errorMessage += $"\n\nDetalhes:\n{jsonErrors}";
                    }

                    MessageBox.Show(errorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro inesperado: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            _main.Navigate(new LoginView(_main));
        }

    }
}
