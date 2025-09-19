using GestaoEventosCorporativos.Wpf.Services;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GestaoEventosCorporativos.Wpf.Views
{
    public partial class HomeView : UserControl
    {
        private readonly MainWindow _main;
        private readonly RelatorioService _relatorioService;

        public HomeView(MainWindow main)
        {
            InitializeComponent();
            _main = main;
            _relatorioService = new RelatorioService();

            Loaded += HomeView_Loaded;
        }

        private async void HomeView_Loaded(object sender, RoutedEventArgs e)
        {
            // Carrega saldo/orçamento
            await CarregarRelatorioSaldo();

            // Carrega tipos de participantes
            await CarregarRelatorioTiposParticipantes();

            // Carrega relatório de fornecedores mais utilizados
            await CarregarRelatorioFornecedores();

        }

        private async void BtnBuscarAgenda_Click(object sender, RoutedEventArgs e)
        {
            var cpf = txtCpfAgenda.Text?.Trim();
            if (string.IsNullOrWhiteSpace(cpf))
            {
                MessageBox.Show("Digite um CPF válido.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = await _relatorioService.ObterAgendaParticipanteAsync(cpf);

            if (result != null && result.IsSuccess)
            {
                dgAgendaParticipante.ItemsSource = result.Data.Eventos;
                MessageBox.Show($"Agenda carregada para {result.Data.NomeParticipante} ({result.Data.Cpf})",
                    "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(result?.Message ?? "Erro ao carregar agenda.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task CarregarRelatorioFornecedores()
        {
            var result = await _relatorioService.ListarFornecedoresMaisUtilizadosAsync();

            if (result != null && result.IsSuccess)
            {
                dgRelatorioFornecedores.ItemsSource = result.Data;
            }
            else
            {
                MessageBox.Show(result?.Message ?? "Erro ao carregar relatório de fornecedores.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task CarregarRelatorioSaldo()
        {
            var result = await _relatorioService.ListarSaldoOrcamentoEventosAsync();

            if (result != null && result.IsSuccess)
            {
                dgRelatorioSaldo.ItemsSource = result.Data;
            }
            else
            {
                MessageBox.Show(result?.Message ?? "Erro ao carregar relatório de saldo/orçamento.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task CarregarRelatorioTiposParticipantes()
        {
            var result = await _relatorioService.ListarTiposParticipantesFrequentesAsync();
            if (result != null && result.IsSuccess)
            {
                dgRelatorioTiposParticipantes.ItemsSource = result.Data;
            }
            else
            {
                MessageBox.Show(result?.Message ?? "Erro ao carregar relatório de tipos de participantes.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnTipoEvento_Click(object sender, RoutedEventArgs e)
        {
            _main.Navigate(new TipoEventoView(_main));
        }

        private void BtnFornecedor_Click(object sender, RoutedEventArgs e)
        {
            _main.Navigate(new FornecedorView(_main));
        }

        private void BtnParticipante_Click(object sender, RoutedEventArgs e)
        {
            _main.Navigate(new ParticipanteView(_main));
        }

        private void BtnEvento_Click(object sender, RoutedEventArgs e)
        {
            _main.Navigate(new EventoView(_main));
        }
    }
}
