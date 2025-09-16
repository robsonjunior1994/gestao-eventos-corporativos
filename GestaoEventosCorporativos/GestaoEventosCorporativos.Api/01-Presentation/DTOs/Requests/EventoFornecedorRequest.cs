using System.ComponentModel.DataAnnotations;

namespace GestaoEventosCorporativos.Api._01_Presentation.DTOs.Requests
{
    public class EventoFornecedorRequest
    {
        [Required, StringLength(18)]
        public string CNPJ { get; set; } = string.Empty;
    }
}
