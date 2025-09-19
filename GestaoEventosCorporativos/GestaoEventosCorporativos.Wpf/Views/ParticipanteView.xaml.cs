using GestaoEventosCorporativos.Wpf.DTOs.Reponse;
using GestaoEventosCorporativos.Wpf.DTOs.Request;
using GestaoEventosCorporativos.Wpf.Services;
using System.Windows;
using System.Windows.Controls;

namespace GestaoEventosCorporativos.Wpf.Views
{
    public partial class ParticipanteView : UserControl
    {
        private readonly ParticipanteService _participanteService;
        private int _paginaAtual = 1;
        private int _pageSize = 10;
        private int _totalPages = 1;
        private readonly MainWindow _main;

        public ParticipanteView(MainWindow main)
        {
            InitializeComponent();
            _participanteService = new ParticipanteService();
            _ = CarregarLista(_paginaAtual, _pageSize);
            _main = main;
        }

        private async Task CarregarLista(int pageNumber, int pageSize)
        {
            var result = await _participanteService.ListarParticipantesAsync(pageNumber, pageSize);

            if (result != null && result.IsSuccess)
            {
                dgParticipantes.ItemsSource = result.Data.Items;
                _paginaAtual = result.Data.PageNumber;
                _totalPages = result.Data.TotalPages;
                txtPagina.Text = $"Página {_paginaAtual} de {_totalPages}";
            }
            else
            {
                MessageBox.Show(result?.Message ?? "Erro ao carregar lista de participantes.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Cadastrar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbTipo.SelectedItem is ComboBoxItem selectedItem && int.TryParse(selectedItem.Tag.ToString(), out int tipo))
            {
                var request = new ParticipanteRequest
                {
                    NomeCompleto = txtNomeCompleto.Text,
                    Cpf = txtCpf.Text,
                    Telefone = txtTelefone.Text,
                    Tipo = tipo
                };

                var result = await _participanteService.CadastrarParticipanteAsync(request);

                if (result != null && result.IsSuccess)
                {
                    MessageBox.Show(result.Message, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                    txtNomeCompleto.Clear();
                    txtCpf.Clear();
                    txtTelefone.Clear();
                    cmbTipo.SelectedIndex = -1;

                    await CarregarLista(_paginaAtual, _pageSize);
                }
                else
                {
                    string errorMessage = $"StatusCode: {result?.StatusCode}\n{result?.Message ?? "Erro ao cadastrar"}";
                    MessageBox.Show(errorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Selecione um tipo válido.");
            }
        }

        private async void Excluir_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var confirm = MessageBox.Show($"Deseja realmente excluir o participante ID {id}?",
                    "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes)
                {
                    var result = await _participanteService.DeletarParticipanteAsync(id);

                    if (result != null && result.IsSuccess)
                    {
                        MessageBox.Show(result.Message, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        await CarregarLista(_paginaAtual, _pageSize); // recarrega a lista
                    }
                    else
                    {
                        string errorMessage = $"StatusCode: {result?.StatusCode}\n{result?.Message ?? "Erro ao excluir"}";
                        MessageBox.Show(errorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private async void Editar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ParticipanteResponse participante)
            {
                // Caixa simples para editar os campos
                var novoNome = Microsoft.VisualBasic.Interaction.InputBox(
                    "Editar Nome Completo:", "Editar Participante", participante.NomeCompleto);

                var novoCpf = Microsoft.VisualBasic.Interaction.InputBox(
                    "Editar CPF:", "Editar Participante", participante.Cpf);

                var novoTelefone = Microsoft.VisualBasic.Interaction.InputBox(
                    "Editar Telefone:", "Editar Participante", participante.Telefone);

                // Seleção do tipo: 0 = VIP, 1 = Normal, 2 = Interno, 3 = Externo
                var tipoStr = Microsoft.VisualBasic.Interaction.InputBox(
                    "Editar Tipo (0=VIP, 1=Interno, 2=Externo):",
                    "Editar Participante", "1");

                if (!int.TryParse(tipoStr, out int novoTipo))
                {
                    MessageBox.Show("Tipo inválido. Use 0=VIP, 1=Interno, 2=Externo.");
                    return;
                }

                var request = new ParticipanteRequest
                {
                    NomeCompleto = string.IsNullOrWhiteSpace(novoNome) ? participante.NomeCompleto : novoNome,
                    Cpf = string.IsNullOrWhiteSpace(novoCpf) ? participante.Cpf : novoCpf,
                    Telefone = string.IsNullOrWhiteSpace(novoTelefone) ? participante.Telefone : novoTelefone,
                    Tipo = novoTipo
                };

                var result = await _participanteService.EditarParticipanteAsync(participante.Id, request);

                if (result != null && result.IsSuccess)
                {
                    MessageBox.Show(result.Message, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    await CarregarLista(_paginaAtual, _pageSize); // Recarrega a lista
                }
                else
                {
                    string errorMessage = $"StatusCode: {result?.StatusCode}\n{result?.Message ?? "Erro ao editar"}";
                    MessageBox.Show(errorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private async void Anterior_Click(object sender, RoutedEventArgs e)
        {
            if (_paginaAtual > 1)
                await CarregarLista(--_paginaAtual, _pageSize);
        }

        private async void Proxima_Click(object sender, RoutedEventArgs e)
        {
            if (_paginaAtual < _totalPages)
                await CarregarLista(++_paginaAtual, _pageSize);
        }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            _main.Navigate(new HomeView(_main));
        }

    }
}
