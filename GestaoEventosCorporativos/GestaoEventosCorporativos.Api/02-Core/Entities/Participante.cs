using GestaoEventosCorporativos.Api._02_Core.Enums;

namespace GestaoEventosCorporativos.Api._02_Core.Entities
{
    public class Participante
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; }
        public string CPF { get; set; }
        public string Telefone { get; set; }
        public TipoParticipante Tipo { get; set; }

        public ICollection<ParticipanteEvento> Eventos { get; set; } = new List<ParticipanteEvento>();
    }
}
