using System.ComponentModel.DataAnnotations;

namespace GestaoEventosCorporativos.Api._01_Presentation.DTOs.Requests
{
    public class EventoFornecedorRequest
    {
        [Required(ErrorMessage = "O CNPJ é obrigatório.")]
        [MinLength(14, ErrorMessage = "O CNPJ deve conter 14 dígitos numéricos.")]
        [MaxLength(14, ErrorMessage = "O CNPJ deve conter 14 dígitos numéricos.")]
        public string CNPJ { get; set; } = string.Empty;
    }
}
