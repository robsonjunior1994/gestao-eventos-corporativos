namespace GestaoEventosCorporativos.Api._01_Presentation.DTOs.Requests
{
    public class EventoRequest
    {
        public string Nome { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Local { get; set; }
        public string Endereco { get; set; }
        public string Observacoes { get; set; }
        public int LotacaoMaxima { get; set; }
        public decimal OrcamentoMaximo { get; set; }
        public int TipoEventoId { get; set; }
    }
}
