namespace GestaoEventosCorporativos.Api._02_Core.Entities
{

    public class Evento
    {
        public Evento(string nome, DateTime dataInicio, DateTime dataFim,
                      string local, string endereco, string observacoes,
                      int lotacaoMaxima, decimal orcamentoMaximo, int tipoEventoId)
        {
            Nome = nome;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Local = local;
            Endereco = endereco;
            Observacoes = observacoes;
            LotacaoMaxima = lotacaoMaxima;
            OrcamentoMaximo = orcamentoMaximo;
            TipoEventoId = tipoEventoId;
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Local { get; set; }
        public string Endereco { get; set; }
        public string Observacoes { get; set; }
        public int LotacaoMaxima { get; set; }
        public decimal OrcamentoMaximo { get; set; }
        public int TipoEventoId { get; set; }
        public TipoEvento TipoEvento { get; set; }

        public ICollection<ParticipanteEvento> Participantes { get; set; } = new List<ParticipanteEvento>();
        public ICollection<EventoFornecedor> Fornecedores { get; set; } = new List<EventoFornecedor>();

        public decimal ValorTotalFornecedores
            => Fornecedores.Sum(f => f.ValorContratado);

        public decimal SaldoOrcamento
            => OrcamentoMaximo - ValorTotalFornecedores;

        public void Update(
            string nome,
            DateTime dataInicio,
            DateTime dataFim,
            string local,
            string endereco,
            string observacoes,
            int lotacaoMaxima,
            decimal orcamentoMaximo,
            int tipoEventoId)
        {
            Nome = nome;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Local = local;
            Endereco = endereco;
            Observacoes = observacoes;
            LotacaoMaxima = lotacaoMaxima;
            OrcamentoMaximo = orcamentoMaximo;
            TipoEventoId = tipoEventoId;
        }
    }
}
