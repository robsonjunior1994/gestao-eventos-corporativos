using System.ComponentModel.DataAnnotations;

namespace GestaoEventosCorporativos.Api._01_Presentation.DTOs.Requests
{
    public class EventoParticipanteRequest
    {

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [MinLength(11, ErrorMessage = "O CPF deve conter 11 dígitos numéricos.")]
        [MaxLength(11, ErrorMessage = "O CPF deve conter 11 dígitos numéricos.")]
        public string CPF { get; set; }
    }
}
