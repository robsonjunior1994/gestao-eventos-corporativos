using System.ComponentModel.DataAnnotations;

namespace GestaoEventosCorporativos.Api._01_Presentation.DTOs.Requests
{
    public class EventoRequest
    {
        [Required(ErrorMessage = "O nome do evento é obrigatório.")]
        [StringLength(200, ErrorMessage = "O nome do evento deve ter no máximo 200 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A data de início é obrigatória.")]
        public DateTime DataInicio { get; set; }

        [Required(ErrorMessage = "A data de fim é obrigatória.")]
        public DateTime DataFim { get; set; }

        [Required(ErrorMessage = "O local do evento é obrigatório.")]
        [StringLength(200, ErrorMessage = "O local deve ter no máximo 200 caracteres.")]
        public string Local { get; set; }

        [Required(ErrorMessage = "O endereço do evento é obrigatório.")]
        [StringLength(300, ErrorMessage = "O endereço deve ter no máximo 300 caracteres.")]
        public string Endereco { get; set; }

        [StringLength(500, ErrorMessage = "As observações devem ter no máximo 500 caracteres.")]
        public string Observacoes { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A lotação máxima deve ser maior que zero.")]
        public int LotacaoMaxima { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "O orçamento máximo deve ser maior que zero.")]
        public decimal OrcamentoMaximo { get; set; }

        [Required(ErrorMessage = "O tipo de evento é obrigatório.")]
        public int TipoEventoId { get; set; }
    }
}
