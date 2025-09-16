using System.ComponentModel.DataAnnotations;

namespace GestaoEventosCorporativos.Api._01_Presentation.DTOs.Requests
{
    public class FornecedorRequest
    {
        [Required, StringLength(150)]
        public string NomeServico { get; set; } = string.Empty;

        [Required, StringLength(18)] 
        public string CNPJ { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "O valor base deve ser maior que zero.")]
        public decimal ValorBase { get; set; }
    }
}
