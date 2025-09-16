namespace GestaoEventosCorporativos.Api._01_Presentation.DTOs.Responses
{
    public class ParticipanteResponse
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; }
        public string CPF { get; set; }
        public string Telefone { get; set; }
        public string Tipo { get; set; }
    }
}