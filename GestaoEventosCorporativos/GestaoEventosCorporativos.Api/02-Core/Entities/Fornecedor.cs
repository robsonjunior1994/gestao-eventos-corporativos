namespace GestaoEventosCorporativos.Api._02_Core.Entities
{
    public class Fornecedor
    {
        public int Id { get; set; }
        public string NomeServico { get; set; }
        public string CNPJ { get; set; }
        public decimal ValorBase { get; set; }

        public ICollection<EventoFornecedor> Eventos { get; set; } = new List<EventoFornecedor>();
    }
}
