using GestaoEventosCorporativos.Wpf.DTOs.Reponse;

public class AgendaParticipanteResponse
{
    public string NomeParticipante { get; set; }
    public string Cpf { get; set; }
    public List<EventoAgendaResponse> Eventos { get; set; }
}