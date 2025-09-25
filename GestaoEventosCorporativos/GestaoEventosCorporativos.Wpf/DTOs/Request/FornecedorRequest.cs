using System.ComponentModel.DataAnnotations;

namespace GestaoEventosCorporativos.Wpf.DTOs.Request
{
    public class FornecedorRequest
    {
        [Required(ErrorMessage = "Nome do serviço é obrigatório.")]
        [MaxLength(150, ErrorMessage = "Nome do serviço deve ter no máximo 150 caracteres.")]
        public string NomeServico { get; set; }

        [Required(ErrorMessage = "CNPJ é obrigatório.")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "CNPJ deve conter exatamente 14 caracteres.")]
        public string Cnpj { get; set; }

        [Range(typeof(decimal), "1", "1000000000", ErrorMessage = "O valor base deve estar entre 0 e 1 bilhão.")]
        public decimal ValorBase { get; set; }
    }
}
