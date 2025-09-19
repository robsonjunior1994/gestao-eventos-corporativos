using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GestaoEventosCorporativos.Wpf.DTOs.Reponse;
using GestaoEventosCorporativos.Wpf.Services;

namespace GestaoEventosCorporativos.Wpf.Views
{
    public partial class EventoAddFornecedoresView : UserControl
    {
        private readonly MainWindow _main;
        private readonly EventoService _eventoService;
        private readonly FornecedorService _fornecedorService;

        private readonly EventoResponse _evento;
        private int _paginaAtual = 1;
        private int _totalPaginas = 1;
        private const int _pageSize = 10;

        public EventoAddFornecedoresView(MainWindow main, EventoResponse evento)
        {
            InitializeComponent();
            _main = main;
            _evento = evento;

            _eventoService = new EventoService();
            _fornecedorService = new FornecedorService();

            PreencherCabecalho();
            _ = CarregarFornecedoresDisponiveis(_paginaAtual, _pageSize);
            _ = CarregarFornecedoresDoEvento(_evento.Id);
        }

        private void PreencherCabecalho()
        {
            lblTitulo.Text = $"Adicionar Fornecedores — {_evento.Nome}";
            lblPeriodo.Text = $"Período: {_evento.DataInicio:dd/MM/yyyy HH:mm} até {_evento.DataFim:dd/MM/yyyy HH:mm}";
            lblTipo.Text = $"Tipo: {_evento.TipoEventoDescricao}";
            lblLocal.Text = $"Local: {_evento.Local} — {_evento.Endereco}";
            lblOrcamento.Text = $"Orçamento Máximo: {_evento.OrcamentoMaximo:C} | Saldo: {_evento.SaldoOrcamento:C}";
        }

        private async Task CarregarFornecedoresDisponiveis(int pageNumber, int pageSize)
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
                MessageBox.Show(result?.Message ?? "Erro ao carregar fornecedores.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task CarregarFornecedoresDoEvento(int eventoId)
        {
            var evento = await _eventoService.ObterEventoPorIdAsync(eventoId);
            if (evento != null && evento.IsSuccess)
            {
                var lista = new List<FornecedorResumoView>();

                foreach (var f in evento.Data.Fornecedores)
                {
                    var linha = f.Replace(";", "").Trim();
                    var partes = linha.Split(',');

                    string servico = "";
                    string cnpj = "";

                    foreach (var parte in partes)
                    {
                        if (parte.Contains("SERVIÇO:"))
                            servico = parte.Replace("SERVIÇO:", "").Trim();
                        else if (parte.Contains("CNPJ:"))
                            cnpj = parte.Replace("CNPJ:", "").Trim();
                    }

                    lista.Add(new FornecedorResumoView
                    {
                        NomeServico = servico,
                        Cnpj = cnpj
                    });
                }

                dgFornecedoresEvento.ItemsSource = lista;

                // Atualiza saldo/orçamento
                lblOrcamento.Text = $"Orçamento Máximo: {evento.Data.OrcamentoMaximo:C} | Saldo: {evento.Data.SaldoOrcamento:C}";
            }
            else
            {
                MessageBox.Show(evento?.Message ?? "Erro ao carregar fornecedores do evento.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Adicionar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string cnpj && !string.IsNullOrWhiteSpace(cnpj))
            {
                var confirm = MessageBox.Show($"Adicionar o fornecedor (CNPJ: {cnpj}) ao evento \"{_evento.Nome}\"?",
                    "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (confirm == MessageBoxResult.Yes)
                {
                    var resp = await _eventoService.AdicionarFornecedorAsync(_evento.Id, cnpj);

                    if (resp != null && resp.IsSuccess)
                    {
                        MessageBox.Show(resp.Message, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        await CarregarFornecedoresDoEvento(_evento.Id); // 🔄 Atualiza lista e saldo
                    }
                    else
                    {
                        MessageBox.Show(resp?.Message ?? "Erro ao adicionar fornecedor.",
                            "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private async void Anterior_Click(object sender, RoutedEventArgs e)
        {
            if (_paginaAtual > 1)
                await CarregarFornecedoresDisponiveis(_paginaAtual - 1, _pageSize);
        }

        private async void Proxima_Click(object sender, RoutedEventArgs e)
        {
            if (_paginaAtual < _totalPaginas)
                await CarregarFornecedoresDisponiveis(_paginaAtual + 1, _pageSize);
        }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            _main.Navigate(new EventoView(_main));
        }
    }
}
