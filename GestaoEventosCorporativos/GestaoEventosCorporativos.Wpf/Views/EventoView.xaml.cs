using GestaoEventosCorporativos.Wpf.DTOs.Reponse;
using GestaoEventosCorporativos.Wpf.DTOs.Request;
using GestaoEventosCorporativos.Wpf.Services;
using System;
using System.DirectoryServices.ActiveDirectory;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GestaoEventosCorporativos.Wpf.Views
{
    public partial class EventoView : UserControl
    {
        private readonly EventoService _eventoService;
        private readonly TipoEventoService _tipoEventoService;
        private int _paginaAtual = 1;
        private int _totalPaginas = 1;
        private int? _eventoEmEdicaoId = null;
        private readonly MainWindow _main;

        public EventoView(MainWindow main)
        {
            InitializeComponent();
            _eventoService = new EventoService();
            _tipoEventoService = new TipoEventoService();
            _main = main;


            _ = CarregarTiposEvento();
            _ = CarregarEventos();
        }

        private async Task CarregarTiposEvento()
        {
            var result = await _tipoEventoService.ListarTipoEventosAsync(1, 50);
            if (result != null && result.IsSuccess)
                cmbTipoEvento.ItemsSource = result.Data.Items;
        }

        private async Task CarregarEventos()
        {
            var result = await _eventoService.ListarEventosAsync(_paginaAtual, 10);

            if (result != null && result.IsSuccess)
            {
                dgEventos.ItemsSource = result.Data.Items;
                _totalPaginas = result.Data.TotalPages;
                txtPagina.Text = $"Página {_paginaAtual} de {_totalPaginas}";
            }
            else
            {
                MessageBox.Show(result?.Message ?? "Erro ao carregar eventos.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Cadastrar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbTipoEvento.SelectedValue == null)
            {
                MessageBox.Show("Selecione um Tipo de Evento.");
                return;
            }

            if (!DateTime.TryParse(dpDataInicio.Text, out DateTime dataInicio) ||
                !DateTime.TryParse(dpDataFim.Text, out DateTime dataFim))
            {
                MessageBox.Show("Datas inválidas.");
                return;
            }

            if (!int.TryParse(txtLotacaoMaxima.Text, out int lotacaoMaxima) ||
                !decimal.TryParse(txtOrcamentoMaximo.Text, out decimal orcamentoMaximo))
            {
                MessageBox.Show("Lotação ou Orçamento inválidos.");
                return;
            }

            var request = new EventoRequest
            {
                Nome = txtNome.Text,
                DataInicio = dataInicio,
                DataFim = dataFim,
                Local = txtLocal.Text,
                Endereco = txtEndereco.Text,
                Observacoes = txtObservacoes.Text,
                LotacaoMaxima = lotacaoMaxima,
                OrcamentoMaximo = orcamentoMaximo,
                TipoEventoId = (int)cmbTipoEvento.SelectedValue
            };

            ApiResponse<EventoResponse> result;

            if (_eventoEmEdicaoId.HasValue)
            {
                result = await _eventoService.AtualizarEventoAsync(_eventoEmEdicaoId.Value, request);
            }
            else
            {
                result = await _eventoService.CadastrarEventoAsync(request);
            }

            if (result != null && result.IsSuccess)
            {
                MessageBox.Show(result.Message, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                _eventoEmEdicaoId = null;
                LimparFormulario();
                await CarregarEventos();
            }
            else
            {
                MessageBox.Show(result?.Message ?? "Erro ao salvar evento.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Anterior_Click(object sender, RoutedEventArgs e)
        {
            if (_paginaAtual > 1)
            {
                _paginaAtual--;
                await CarregarEventos();
            }
        }

        private async void Proxima_Click(object sender, RoutedEventArgs e)
        {
            if (_paginaAtual < _totalPaginas)
            {
                _paginaAtual++;
                await CarregarEventos();
            }
        }

        private async void BuscarCep_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCep.Text))
            {
                MessageBox.Show("Digite um CEP.");
                return;
            }

            var cepService = new CepService();
            var endereco = await cepService.BuscarCepAsync(txtCep.Text);

            if (!string.IsNullOrEmpty(endereco))
                txtEndereco.Text = endereco;
            else
                MessageBox.Show("CEP não encontrado.");
        }

        private async void Excluir_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag.ToString(), out int id))
            {
                var confirm = MessageBox.Show("Deseja realmente excluir este evento?",
                                              "Confirmação",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Question);

                if (confirm == MessageBoxResult.Yes)
                {
                    var result = await _eventoService.DeletarEventoAsync(id);

                    if (result != null && result.IsSuccess)
                    {
                        MessageBox.Show("Evento excluído com sucesso!", "Sucesso",
                                        MessageBoxButton.OK, MessageBoxImage.Information);
                        await CarregarEventos(); 
                    }
                    else
                    {
                        MessageBox.Show(result?.Message ?? "Erro ao excluir evento.",
                                        "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private async void Editar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag.ToString(), out int id))
            {
                var eventoResult = await _eventoService.ObterEventoPorIdAsync(id);

                if (eventoResult != null && eventoResult.IsSuccess)
                {
                    var evento = eventoResult.Data;

                    var tiposResult = await _tipoEventoService.ListarTipoEventosAsync(1, 50);
                    if (tiposResult != null && tiposResult.IsSuccess)
                    {
                        cmbTipoEvento.ItemsSource = tiposResult.Data.Items;

                        txtNome.Text = evento.Nome;
                        dpDataInicio.SelectedDate = evento.DataInicio;
                        dpDataFim.SelectedDate = evento.DataFim;
                        txtLocal.Text = evento.Local;
                        txtEndereco.Text = evento.Endereco;
                        txtObservacoes.Text = evento.Observacoes;
                        txtLotacaoMaxima.Text = evento.LotacaoMaxima.ToString();
                        txtOrcamentoMaximo.Text = evento.OrcamentoMaximo.ToString();

                        var tipoSelecionado = tiposResult.Data.Items
                            .FirstOrDefault(t => t.Descricao == evento.TipoEventoDescricao);

                        if (tipoSelecionado != null)
                            cmbTipoEvento.SelectedValue = tipoSelecionado.Id;

                        _eventoEmEdicaoId = evento.Id;
                    }
                }
                else
                {
                    MessageBox.Show("Erro ao carregar evento para edição.");
                }
            }
        }

        private void LimparFormulario()
        {
            txtNome.Clear();
            dpDataInicio.SelectedDate = null;
            dpDataFim.SelectedDate = null;
            txtLocal.Clear();
            txtCep.Clear();
            txtEndereco.Clear();
            txtObservacoes.Clear();
            txtLotacaoMaxima.Clear();
            txtOrcamentoMaximo.Clear();
            cmbTipoEvento.SelectedIndex = -1;
        }
        private void AdicionarParticipantes_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is EventoResponse evento)
            {
                _main.Navigate(new EventoAddParticipantesView(_main, evento));
            }
        }
        private void AdicionarFornecedores_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is EventoResponse evento)
            {
                _main.Navigate(new EventoAddFornecedoresView(_main, evento));
            }
        }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            _main.Navigate(new HomeView(_main));
        }
    }
}
