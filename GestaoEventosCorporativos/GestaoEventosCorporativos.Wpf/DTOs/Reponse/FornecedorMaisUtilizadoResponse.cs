namespace GestaoEventosCorporativos.Wpf.DTOs.Relatorios
{
    public class FornecedorMaisUtilizadoResponse
    {
        public string NomeServico { get; set; }
        public string Cnpj { get; set; }
        public int QuantidadeEventos { get; set; }
        public decimal ValorTotalContratado { get; set; }
    }
}
