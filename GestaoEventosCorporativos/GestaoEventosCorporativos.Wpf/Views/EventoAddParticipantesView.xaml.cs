using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GestaoEventosCorporativos.Wpf.DTOs.Reponse;
using GestaoEventosCorporativos.Wpf.Services;

namespace GestaoEventosCorporativos.Wpf.Views
{
    public partial class EventoAddParticipantesView : UserControl
    {
        private readonly MainWindow _main;
        private readonly EventoService _eventoService;
        private readonly ParticipanteService _participanteService;

        private readonly EventoResponse _evento;
        private int _paginaAtual = 1;
        private int _totalPaginas = 1;
        private const int _pageSize = 10;

        public EventoAddParticipantesView(MainWindow main, EventoResponse evento)
        {
            InitializeComponent();
            _main = main;
            _evento = evento;

            _eventoService = new EventoService();
            _participanteService = new ParticipanteService();

            Loaded += EventoAddParticipantesView_Loaded;
        }

        private async void EventoAddParticipantesView_Loaded(object sender, RoutedEventArgs e)
        {
            PreencherCabecalho();

            await CarregarParticipantesDoEvento(_evento.Id);

            await CarregarParticipantesDisponiveis(_paginaAtual, _pageSize);
        }

        private void PreencherCabecalho()
        {
            lblTitulo.Text = $"Adicionar Participantes — {_evento.Nome}";
            lblPeriodo.Text = $"Período: {_evento.DataInicio:dd/MM/yyyy HH:mm} até {_evento.DataFim:dd/MM/yyyy HH:mm}";
            lblTipo.Text = $"Tipo: {_evento.TipoEventoDescricao}";
            lblLocal.Text = $"Local: {_evento.Local} — {_evento.Endereco}";
            lblOrcamento.Text = $"Orçamento Máximo: {_evento.OrcamentoMaximo:C} | Saldo: {_evento.SaldoOrcamento:C}";
            lblLotacao.Text = $"Número atual de participantes: {_evento.LotacaoMaxima} | Número atual de participantes: {_evento.Participantes.Count}";
        }

        private async Task CarregarParticipantesDoEvento(int eventoId)
        {
            var evento = await _eventoService.ObterEventoPorIdAsync(eventoId);
            if (evento != null && evento.IsSuccess)
            {
                var lista = new List<ParticipanteResumoView>();

                foreach (var p in evento.Data.Participantes)
                {
                    
                    var linha = p.Replace(";", "").Trim();

                    
                    var partes = linha.Split(',');

                    string nome = "";
                    string cpf = "";

                    foreach (var parte in partes)
                    {
                        if (parte.Contains("NOME:"))
                        {
                            nome = parte.Replace("NOME:", "").Trim();
                        }
                        else if (parte.Contains("CPF:"))
                        {
                            cpf = parte.Replace("CPF:", "").Trim();
                        }
                    }

                    lista.Add(new ParticipanteResumoView
                    {
                        NomeCompleto = nome,
                        Cpf = cpf
                    });
                }

                dgParticipantesEvento.ItemsSource = lista;

                
                lblLotacao.Text =
                    $"Lotação máxima: {evento.Data.LotacaoMaxima} | " +
                    $"Número atual de participantes: {lista.Count}";
            }
            else
            {
                MessageBox.Show(evento?.Message ?? "Erro ao carregar participantes do evento.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task CarregarParticipantesDisponiveis(int pageNumber, int pageSize)
        {
            var result = await _participanteService.ListarParticipantesAsync(pageNumber, pageSize);

            if (result != null && result.IsSuccess)
            {
                dgParticipantes.ItemsSource = result.Data.Items;
                _paginaAtual = result.Data.PageNumber;
                _totalPaginas = result.Data.TotalPages;
                txtPagina.Text = $"Página {_paginaAtual} de {_totalPaginas}";
            }
            else
            {
                MessageBox.Show(result?.Message ?? "Erro ao carregar participantes disponíveis.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Adicionar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string cpf && !string.IsNullOrWhiteSpace(cpf))
            {
                var confirm = MessageBox.Show(
                    $"Adicionar o participante (CPF: {cpf}) ao evento \"{_evento.Nome}\"?",
                    "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (confirm == MessageBoxResult.Yes)
                {
                    var resp = await _eventoService.AdicionarParticipanteAsync(_evento.Id, cpf);

                    if (resp != null && resp.IsSuccess)
                    {
                        MessageBox.Show(resp.Message, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                        
                        await CarregarParticipantesDoEvento(_evento.Id);
                        await CarregarParticipantesDisponiveis(_paginaAtual, _pageSize);
                    }
                    else
                    {
                        MessageBox.Show(resp?.Message ?? "Erro ao adicionar participante.",
                            "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private async void Anterior_Click(object sender, RoutedEventArgs e)
        {
            if (_paginaAtual > 1)
                await CarregarParticipantesDisponiveis(_paginaAtual - 1, _pageSize);
        }

        private async void Proxima_Click(object sender, RoutedEventArgs e)
        {
            if (_paginaAtual < _totalPaginas)
                await CarregarParticipantesDisponiveis(_paginaAtual + 1, _pageSize);
        }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            _main.Navigate(new EventoView(_main));
        }

        private async void Remover_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string cpf && !string.IsNullOrWhiteSpace(cpf))
            {
                var confirm = MessageBox.Show(
                    $"Remover o participante (CPF: {cpf}) do evento \"{_evento.Nome}\"?",
                    "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes)
                {
                    var resp = await _eventoService.RemoverParticipanteAsync(_evento.Id, cpf);

                    if (resp != null && resp.IsSuccess && resp.Data)
                    {
                        MessageBox.Show(resp.Message, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                        await CarregarParticipantesDoEvento(_evento.Id);
                        await CarregarParticipantesDisponiveis(_paginaAtual, _pageSize);
                    }
                    else
                    {
                        MessageBox.Show(resp?.Message ?? "Erro ao remover participante.",
                            "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
            }
        }



    }
}
