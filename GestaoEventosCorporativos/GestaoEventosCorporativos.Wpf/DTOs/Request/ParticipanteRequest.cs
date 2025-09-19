namespace GestaoEventosCorporativos.Wpf.DTOs.Request
{
    public class ParticipanteRequest
    {
        public string NomeCompleto { get; set; }
        public string Cpf { get; set; }
        public string Telefone { get; set; }
        public int Tipo { get; set; } // 0 = VIP, 1 = Normal, etc.
    }
}
