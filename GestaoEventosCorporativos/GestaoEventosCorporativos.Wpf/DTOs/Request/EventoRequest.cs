using System.ComponentModel.DataAnnotations;

namespace GestaoEventosCorporativos.Wpf.DTOs.Request
{
    public class EventoRequest
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Data de início obrigatória.")]
        public DateTime DataInicio { get; set; }

        [Required(ErrorMessage = "Data de fim obrigatória.")]
        public DateTime DataFim { get; set; }

        [Required(ErrorMessage = "O local é obrigatório.")]
        [StringLength(200, ErrorMessage = "O local deve ter no máximo 200 caracteres.")]
        public string Local { get; set; }

        [StringLength(500, ErrorMessage = "O endereço deve ter no máximo 500 caracteres.")]
        [Required(ErrorMessage = "O endereço é obrigatório.")]
        public string Endereco { get; set; }
        [StringLength(500, ErrorMessage = "O local deve ter no máximo 500 caracteres.")]

        public string Observacoes { get; set; }

        [Range(1, 100000, ErrorMessage = "A lotação deve ser entre 1 e 100.000 participantes.")]
        public int LotacaoMaxima { get; set; }

        [Range(typeof(decimal), "1", "1000000000", ErrorMessage = "TESTE O valor base deve estar entre 0 e 1 bilhão.")]
        public decimal OrcamentoMaximo { get; set; }

        [Required(ErrorMessage = "O tipo do evento é obrigatório.")]
        public int TipoEventoId { get; set; }
    }
}
