using System.ComponentModel.DataAnnotations;
using GestaoEventosCorporativos.Api._02_Core.Enums;

namespace GestaoEventosCorporativos.Api._01_Presentation.DTOs.Requests
{
    public class ParticipanteRequest
    {
        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        [MaxLength(200, ErrorMessage = "O nome completo não pode ultrapassar 200 caracteres.")]
        public string NomeCompleto { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [MinLength(11, ErrorMessage = "O CPF deve conter 11 dígitos numéricos.")]
        [MaxLength(11, ErrorMessage = "O CPF deve conter 11 dígitos numéricos.")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [Phone(ErrorMessage = "Número de telefone inválido.")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "O tipo do participante é obrigatório.")]
        public TipoParticipante Tipo { get; set; }
    }
}
