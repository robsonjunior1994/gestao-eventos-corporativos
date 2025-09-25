using GestaoEventosCorporativos.Wpf.DTOs.Request;
using GestaoEventosCorporativos.Wpf.Services;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;


namespace GestaoEventosCorporativos.Wpf.Views
{
    public partial class TipoEventoView : UserControl
    {
        private readonly TipoEventoService _tipoEventoService;
        private int _paginaAtual = 1;
        private int _totalPaginas = 1;
        private const int _pageSize = 5;
        private readonly MainWindow _main;
        private int? _tipoEventoEmEdicaoId = null;

        public TipoEventoView(MainWindow main)
        {
            InitializeComponent();
            _tipoEventoService = new TipoEventoService();

            _ = CarregarLista(_paginaAtual, _pageSize);
            _main = main;
        }

        private async void Cadastrar_Click(object sender, RoutedEventArgs e)
        {
            var request = new TipoEventoRequest { Descricao = txtDescricao.Text };

            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(request, context, results, true))
            {
                string erros = string.Join("\n", results.Select(r => r.ErrorMessage));
                MessageBox.Show(erros, "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_tipoEventoEmEdicaoId == null)
            {
                var result = await _tipoEventoService.CadastrarTipoEventoAsync(request);

                if (result != null && result.IsSuccess)
                {
                    MessageBox.Show(result.Message, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(result?.Message ?? "Erro ao cadastrar", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                var result = await _tipoEventoService.EditarTipoEventoAsync(_tipoEventoEmEdicaoId.Value, request);

                if (result != null && result.IsSuccess)
                {
                    MessageBox.Show(result.Message, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(result?.Message ?? "Erro ao atualizar", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                _tipoEventoEmEdicaoId = null;
                btnCadastrar.Content = "Cadastrar";
            }

            await CarregarLista(_paginaAtual, _pageSize);
            LimparFormulario();
        }

        private async Task CarregarLista(int pageNumber, int pageSize)
        {
            var result = await _tipoEventoService.ListarTipoEventosAsync(pageNumber, pageSize);

            if (result != null && result.IsSuccess)
            {
                dgTipoEventos.ItemsSource = result.Data.Items;
                _paginaAtual = result.Data.PageNumber;
                _totalPaginas = result.Data.TotalPages;

                txtPagina.Text = $"Página {_paginaAtual} de {_totalPaginas}";
            }
            else
            {
                string errorMessage = $"StatusCode: {result?.StatusCode}\n{result?.Message ?? "Erro ao carregar lista."}";
                MessageBox.Show(errorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Anterior_Click(object sender, RoutedEventArgs e)
        {
            if (_paginaAtual > 1)
            {
                await CarregarLista(_paginaAtual - 1, _pageSize);
            }
        }

        private async void Proxima_Click(object sender, RoutedEventArgs e)
        {
            if (_paginaAtual < _totalPaginas)
            {
                await CarregarLista(_paginaAtual + 1, _pageSize);
            }
        }

        private async void Excluir_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var confirm = MessageBox.Show($"Deseja realmente excluir o TipoEvento ID {id}?",
                    "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes)
                {
                    var result = await _tipoEventoService.DeletarTipoEventoAsync(id);

                    if (result != null && result.IsSuccess)
                    {
                        MessageBox.Show(result.Message, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        await CarregarLista(_paginaAtual, _pageSize);
                    }
                    else
                    {
                        string errorMessage = $"StatusCode: {result?.StatusCode}\n{result?.Message ?? "Erro ao excluir."}";
                        MessageBox.Show(errorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is DTOs.Reponse.TipoEventoResponse tipoEvento)
            {
                _tipoEventoEmEdicaoId = tipoEvento.Id;
                txtDescricao.Text = tipoEvento.Descricao;
                btnCadastrar.Content = "Atualizar";
                btnCadastrar.Background = new SolidColorBrush(Colors.Aqua);
            }
        }


        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            _main.Navigate(new HomeView(_main));
        }

        private void LimparFormulario()
        {
            txtDescricao.Clear();
            btnCadastrar.Content = "Cadastrar";
            btnCadastrar.Background = new SolidColorBrush(Colors.Green);
        }


    }
}
