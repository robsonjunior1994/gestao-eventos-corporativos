namespace GestaoEventosCorporativos.Api._02_Core.Entities
{
    public class ParticipanteEvento
    {
        public int ParticipanteId { get; set; }
        public Participante Participante { get; set; }

        public int EventoId { get; set; }
        public Evento Evento { get; set; }

        public DateTime DataInscricao { get; set; } = DateTime.Now;
    }
}
