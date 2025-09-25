using System.ComponentModel.DataAnnotations;

namespace GestaoEventosCorporativos.Wpf.DTOs.Request
{
    public class UserRequest
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        [MaxLength(50, ErrorMessage = "A senha deve ter no máximo 50 caracteres.")]
        public string Password { get; set; }
    }
}
