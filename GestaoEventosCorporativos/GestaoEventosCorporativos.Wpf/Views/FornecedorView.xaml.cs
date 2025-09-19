using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GestaoEventosCorporativos.Wpf.DTOs.Request;
using GestaoEventosCorporativos.Wpf.Services;

namespace GestaoEventosCorporativos.Wpf.Views
{
    public partial class FornecedorView : UserControl
    {
        private readonly FornecedorService _fornecedorService;
        private int _paginaAtual = 1;
        private int _totalPaginas = 1;
        private const int _pageSize = 5;

        public FornecedorView()
        {
            InitializeComponent();
            _fornecedorService = new FornecedorService();

            _ = CarregarLista(_paginaAtual, _pageSize);
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

            var result = await _fornecedorService.CadastrarFornecedorAsync(request);

            if (result != null && result.IsSuccess)
            {
                MessageBox.Show($"{result.Message}\nID: {result.Data.Id}\nServiço: {result.Data.NomeServico}",
                    "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                txtNomeServico.Clear();
                txtCnpj.Clear();
                txtValorBase.Clear();

                await CarregarLista(_paginaAtual, _pageSize);
            }
            else
            {
                string errorMessage = $"StatusCode: {result?.StatusCode}\n{result?.Message ?? "Erro ao cadastrar"}";
                MessageBox.Show(errorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private async void Editar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is DTOs.Reponse.FornecedorListResponse fornecedor)
            {
                // Abre uma janela simples de input para editar os valores
                var novoNomeServico = Microsoft.VisualBasic.Interaction.InputBox(
                    "Editar Nome do Serviço:",
                    "Editar Fornecedor",
                    fornecedor.NomeServico);

                var novoCnpj = Microsoft.VisualBasic.Interaction.InputBox(
                    "Editar CNPJ:",
                    "Editar Fornecedor",
                    fornecedor.Cnpj);

                var novoValorStr = Microsoft.VisualBasic.Interaction.InputBox(
                    "Editar Valor Base:",
                    "Editar Fornecedor",
                    fornecedor.ValorBase.ToString());

                if (decimal.TryParse(novoValorStr, out decimal novoValor))
                {
                    var request = new DTOs.Request.FornecedorRequest
                    {
                        NomeServico = novoNomeServico,
                        Cnpj = novoCnpj,
                        ValorBase = novoValor
                    };

                    var result = await _fornecedorService.EditarFornecedorAsync(fornecedor.Id, request);

                    if (result != null && result.IsSuccess)
                    {
                        MessageBox.Show(result.Message, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        await CarregarLista(_paginaAtual, _pageSize);
                    }
                    else
                    {
                        string errorMessage = $"StatusCode: {result?.StatusCode}\n{result?.Message ?? "Erro ao editar."}";
                        MessageBox.Show(errorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Valor Base inválido.");
                }
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
    }
}
