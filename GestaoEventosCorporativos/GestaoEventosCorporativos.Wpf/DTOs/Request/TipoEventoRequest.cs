using System.ComponentModel.DataAnnotations;

namespace GestaoEventosCorporativos.Wpf.DTOs.Request
{
    public class TipoEventoRequest
    {
        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [MaxLength(100, ErrorMessage = "A descrição deve ter no máximo 100 caracteres.")]
        public string Descricao { get; set; }
    }
}
