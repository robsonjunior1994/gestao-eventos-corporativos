namespace GestaoEventosCorporativos.Api._02_Core.Entities
{
    public class TipoEvento
    {
        public int Id { get; set; }
        public string Descricao { get; set; }

        public ICollection<Evento> Eventos { get; set; } = new List<Evento>();
    }
}
