using System.ComponentModel.DataAnnotations;

namespace GestaoEventosCorporativos.Wpf.DTOs.Request
{
    public class ParticipanteRequest
    {
        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        [MaxLength(150, ErrorMessage = "O nome completo deve ter no máximo 150 caracteres.")]
        public string NomeCompleto { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter exatamente 11 dígitos.")]
        public string Cpf { get; set; }

        [MaxLength(15, ErrorMessage = "O telefone deve ter no máximo 15 caracteres.")]
        [Required(ErrorMessage = "O Telefone é obrigatório.")]
        public string Telefone { get; set; }

        [Range(0, 2, ErrorMessage = "Selecione um tipo válido (Cliente, Fornecedor ou Colaborador).")]
        [Required(ErrorMessage = "O Tipo é obrigatório.")]

        public int Tipo { get; set; }
    }
}
