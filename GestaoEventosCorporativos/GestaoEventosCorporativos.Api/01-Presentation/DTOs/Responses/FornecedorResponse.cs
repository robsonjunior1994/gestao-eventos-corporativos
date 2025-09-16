namespace GestaoEventosCorporativos.Api._01_Presentation.DTOs.Responses
{
    public class FornecedorResponse
    {
        public int Id { get; set; }
        public string NomeServico { get; set; } = string.Empty;
        public string CNPJ { get; set; } = string.Empty;
        public decimal ValorBase { get; set; }
    }
}
