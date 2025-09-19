using GestaoEventosCorporativos.Wpf.DTOs.Request;
using GestaoEventosCorporativos.Wpf.Services;
using System.DirectoryServices.ActiveDirectory;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GestaoEventosCorporativos.Wpf.Views
{
    public partial class FornecedorView : UserControl
    {
        private readonly FornecedorService _fornecedorService;
        private int _paginaAtual = 1;
        private int _totalPaginas = 1;
        private const int _pageSize = 5;
        private readonly MainWindow _main;
        private int? _fornecedorEmEdicaoId = null;

        public FornecedorView(MainWindow main)
        {
            InitializeComponent();
            _fornecedorService = new FornecedorService();

            _ = CarregarLista(_paginaAtual, _pageSize);

            _main = main;
        }

        private async void Cadastrar_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(txtValorBase.Text, out decimal valorBase))
            {
                MessageBox.Show("Valor Base inválido.");
                return;
            }

            var request = new FornecedorRequest
            {
                NomeServico = txtNomeServico.Text,
                Cnpj = txtCnpj.Text,
                ValorBase = valorBase
            };

            if (_fornecedorEmEdicaoId == null)
            {
                // 🔹 Cadastro
                var result = await _fornecedorService.CadastrarFornecedorAsync(request);

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
                // 🔹 Atualização
                var result = await _fornecedorService.EditarFornecedorAsync(_fornecedorEmEdicaoId.Value, request);

                if (result != null && result.IsSuccess)
                {
                    MessageBox.Show(result.Message, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(result?.Message ?? "Erro ao atualizar", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                _fornecedorEmEdicaoId = null;
                btnCadastrar.Content = "Cadastrar"; // volta texto original
            }

            // 🔹 Recarrega lista e limpa form
            await CarregarLista(_paginaAtual, _pageSize);
            LimparFormulario();
        }

        private async Task CarregarLista(int pageNumber, int pageSize)
        {
            var result = await _fornecedorService.ListarFornecedoresAsync(pageNumber, pageSize);

            if (result != null && result.IsSuccess)
            {
                dgFornecedores.ItemsSource = result.Data.Items;
                _paginaAtual = result.Data.PageNumber;
                _totalPaginas = result.Data.TotalPages;
                txtPagina.Text = $"Página {_paginaAtual} de {_totalPaginas}";
            }
            else
            {
                string errorMessage = $"StatusCode: {result?.StatusCode}\n{result?.Message ?? "Erro ao carregar lista"}";
                MessageBox.Show(errorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is DTOs.Reponse.FornecedorResponse fornecedor)
            {
                _fornecedorEmEdicaoId = fornecedor.Id;

                txtNomeServico.Text = fornecedor.NomeServico;
                txtCnpj.Text = fornecedor.Cnpj;
                txtValorBase.Text = fornecedor.ValorBase.ToString();

                btnCadastrar.Content = "Atualizar"; // muda o texto do botão
            }
        }

        private async void Excluir_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var confirm = MessageBox.Show($"Deseja realmente excluir o fornecedor ID {id}?",
                    "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes)
                {
                    var result = await _fornecedorService.DeletarFornecedorAsync(id);

                    if (result != null && result.IsSuccess)
                    {
                        MessageBox.Show(result.Message, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        await CarregarLista(_paginaAtual, _pageSize); // recarrega lista
                    }
                    else
                    {
                        string errorMessage = $"StatusCode: {result?.StatusCode}\n{result?.Message ?? "Erro ao excluir"}";
                        MessageBox.Show(errorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
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

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            _main.Navigate(new HomeView(_main));
        }

        private void LimparFormulario()
        {
            txtNomeServico.Clear();
            txtCnpj.Clear();
            txtValorBase.Clear();
        }


    }
}
