namespace GestaoEventosCorporativos.Api._02_Core.Entities
{
    public class EventoFornecedor
    {
        public int EventoId { get; set; }
        public Evento Evento { get; set; }

        public int FornecedorId { get; set; }
        public Fornecedor Fornecedor { get; set; }

        public decimal ValorContratado { get; set; }
    }
}
