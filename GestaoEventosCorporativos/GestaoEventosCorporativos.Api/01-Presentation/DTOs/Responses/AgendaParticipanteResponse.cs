namespace GestaoEventosCorporativos.Api._01_Presentation.DTOs.Responses
{
    public class AgendaParticipanteResponse
    {
        public string NomeParticipante { get; set; }
        public string CPF { get; set; }
        public List<EventoAgendaResponse> Eventos { get; set; } = new();
    }
}
