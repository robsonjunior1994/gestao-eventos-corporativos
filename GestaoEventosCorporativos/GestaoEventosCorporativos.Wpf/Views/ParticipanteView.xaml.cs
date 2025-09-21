using GestaoEventosCorporativos.Wpf.DTOs.Reponse;
using GestaoEventosCorporativos.Wpf.DTOs.Request;
using GestaoEventosCorporativos.Wpf.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GestaoEventosCorporativos.Wpf.Views
{
    public partial class ParticipanteView : UserControl
    {
        private readonly ParticipanteService _participanteService;
        private int _paginaAtual = 1;
        private int _pageSize = 10;
        private int _totalPages = 1;
        private readonly MainWindow _main;
        private int? _participanteEmEdicaoId = null;

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

                if (_participanteEmEdicaoId == null)
                {

                    var result = await _participanteService.CadastrarParticipanteAsync(request);

                    if (result != null && result.IsSuccess)
                    {
                        MessageBox.Show(result.Message, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        string errorMessage = $"StatusCode: {result?.StatusCode}\n{result?.Message ?? "Erro ao cadastrar"}";
                        MessageBox.Show(errorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    var result = await _participanteService.EditarParticipanteAsync(_participanteEmEdicaoId.Value, request);

                    if (result != null && result.IsSuccess)
                    {
                        MessageBox.Show(result.Message, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        string errorMessage = $"StatusCode: {result?.StatusCode}\n{result?.Message ?? "Erro ao atualizar"}";
                        MessageBox.Show(errorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    _participanteEmEdicaoId = null;
                    btnCadastrar.Content = "Cadastrar"; 
                }

                await CarregarLista(_paginaAtual, _pageSize);
                LimparFormulario();
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
                        await CarregarLista(_paginaAtual, _pageSize);
                    }
                    else
                    {
                        string errorMessage = $"StatusCode: {result?.StatusCode}\n{result?.Message ?? "Erro ao excluir"}";
                        MessageBox.Show(errorMessage, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ParticipanteResponse participante)
            {
                _participanteEmEdicaoId = participante.Id;
                txtNomeCompleto.Text = participante.NomeCompleto;
                txtCpf.Text = participante.Cpf;
                txtTelefone.Text = participante.Telefone;

                int tipo = participante.Tipo switch
                {
                    "VIP" => 0,
                    "Interno" => 1,
                    "Externo" => 2,
                    _ => 0
                };

                foreach (ComboBoxItem item in cmbTipo.Items)
                {
                    if (item.Tag != null && int.TryParse(item.Tag.ToString(), out int itemTipo) && itemTipo == tipo)
                    {
                        cmbTipo.SelectedItem = item;
                        break;
                    }
                }

                btnCadastrar.Content = "Atualizar";
                btnCadastrar.Background = new SolidColorBrush(Colors.Aqua);
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

        private void LimparFormulario()
        {
            txtNomeCompleto.Clear();
            txtCpf.Clear();
            txtTelefone.Clear();
            cmbTipo.SelectedIndex = -1;

            btnCadastrar.Content = "Cadastrar";
            btnCadastrar.Background = new SolidColorBrush(Colors.Green);
        }


    }
}
