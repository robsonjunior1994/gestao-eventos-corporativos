namespace GestaoEventosCorporativos.Wpf.DTOs.Reponse
{
    public class TipoEventoListResponse
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
