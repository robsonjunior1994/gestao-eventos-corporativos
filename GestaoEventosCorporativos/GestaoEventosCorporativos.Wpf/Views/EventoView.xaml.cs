using GestaoEventosCorporativos.Wpf.DTOs.Reponse;
using GestaoEventosCorporativos.Wpf.DTOs.Request;
using GestaoEventosCorporativos.Wpf.Services;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

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
            var dataInicio = dtpDataInicio.Value ?? DateTime.MinValue;
            var dataFim = dtpDataFim.Value ?? DateTime.MinValue;

            if (dataInicio < DateTime.Now)
            {
                MessageBox.Show("A data de início deve ser maior ou igual à data atual.",
                    "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dataFim <= DateTime.Now)
            {
                MessageBox.Show("A data de fim deve ser maior que a data atual.",
                    "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dataFim <= dataInicio)
            {
                MessageBox.Show("A data de fim deve ser maior que a data de início.",
                    "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtLotacaoMaxima.Text, out int lotacaoMaxima))
            {
                MessageBox.Show("Digite um número válido para Lotação Máxima.",
                    "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (lotacaoMaxima <= 0 || lotacaoMaxima > 100_000)
            {
                MessageBox.Show("A Lotação Máxima deve ser maior que 0 e no máximo 100.000.",
                    "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtOrcamentoMaximo.Text, out decimal orcamentoMaximo))
            {
                MessageBox.Show("Digite um valor numérico válido para Orçamento Máximo.",
                    "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (orcamentoMaximo < 1 || orcamentoMaximo > 1_000_000_000)
            {
                MessageBox.Show("O Orçamento Máximo deve ser maior que 0 e no máximo 1 bilhão.",
                    "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                TipoEventoId = cmbTipoEvento.SelectedValue != null
                    ? (int)cmbTipoEvento.SelectedValue
                    : 0
            };

            var context = new ValidationContext(request, null, null);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(request, context, results, true))
            {
                string mensagens = string.Join("\n", results.Select(r => r.ErrorMessage));
                MessageBox.Show(mensagens, "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 🔹 Continua fluxo normal
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
                        dtpDataInicio.Value = evento.DataInicio;
                        dtpDataFim.Value = evento.DataFim;
                        txtLocal.Text = evento.Local;
                        txtEndereco.Text = evento.Endereco;
                        txtObservacoes.Text = evento.Observacoes;
                        txtLotacaoMaxima.Text = evento.LotacaoMaxima.ToString();
                        txtOrcamentoMaximo.Text = evento.OrcamentoMaximo.ToString();

                        btnSalvar.Content = "Atualizar Evento";
                        btnSalvar.Background = new SolidColorBrush(Colors.Aqua);


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
            dtpDataInicio.Value = null;
            dtpDataFim.Value = null;
            txtLocal.Clear();
            txtCep.Clear();
            txtEndereco.Clear();
            txtObservacoes.Clear();
            txtLotacaoMaxima.Clear();
            txtOrcamentoMaximo.Clear();
            cmbTipoEvento.SelectedIndex = -1;

            btnSalvar.Content = "Cadastrar";
            btnSalvar.Background = new SolidColorBrush(Colors.Green);
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

        private void txtValorBase_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Tenta concatenar o valor atual com o novo caractere e ver se é número
            e.Handled = !decimal.TryParse(((TextBox)sender).Text + e.Text, out _);
        }

    }
}
