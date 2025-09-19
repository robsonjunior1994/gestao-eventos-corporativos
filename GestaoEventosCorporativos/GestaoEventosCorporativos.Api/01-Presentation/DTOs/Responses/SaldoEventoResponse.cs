namespace GestaoEventosCorporativos.Api._01_Presentation.DTOs.Responses
{
    public class SaldoEventoResponse
    {
        public int EventoId { get; set; }
        public string Nome { get; set; }
        public decimal OrcamentoMaximo { get; set; }
        public decimal ValorTotalFornecedores { get; set; }
        public decimal SaldoOrcamento { get; set; }
    }
}
