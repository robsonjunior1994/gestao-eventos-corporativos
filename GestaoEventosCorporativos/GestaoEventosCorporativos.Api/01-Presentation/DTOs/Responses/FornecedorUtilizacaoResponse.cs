namespace GestaoEventosCorporativos.Api._01_Presentation.DTOs.Responses
{
    public class FornecedorUtilizacaoResponse
    {
        public string NomeServico { get; set; }
        public string CNPJ { get; set; }
        public int QuantidadeEventos { get; set; }
        public decimal ValorTotalContratado { get; set; }
    }
}
