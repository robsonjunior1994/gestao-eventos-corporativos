namespace GestaoEventosCorporativos.Wpf.DTOs.Reponse
{
    public class EventoResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Local { get; set; }
        public string Endereco { get; set; }
        public string Observacoes { get; set; }
        public int LotacaoMaxima { get; set; }
        public decimal OrcamentoMaximo { get; set; }
        public decimal ValorTotalFornecedores { get; set; }
        public decimal SaldoOrcamento { get; set; }
        public string TipoEventoDescricao { get; set; }
        public List<string> Participantes { get; set; }
        public List<string> Fornecedores { get; set; }

        // Helpers para exibição
        public string ParticipantesResumo => Participantes != null && Participantes.Any()
            ? string.Join(", ", Participantes)
            : "Nenhum";

        public string FornecedoresResumo => Fornecedores != null && Fornecedores.Any()
            ? string.Join(", ", Fornecedores)
            : "Nenhum";
    }
}
