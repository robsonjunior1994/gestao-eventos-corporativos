namespace GestaoEventosCorporativos.Api._01_Presentation.DTOs.Responses
{
    public class EventoAgendaResponse
    {
        public int EventoId { get; set; }
        public string NomeEvento { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Local { get; set; }
    }
}
