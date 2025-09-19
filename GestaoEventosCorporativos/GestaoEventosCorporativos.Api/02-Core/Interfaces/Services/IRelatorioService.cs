using GestaoEventosCorporativos.Api._01_Presentation.DTOs.Responses;
using GestaoEventosCorporativos.Api._02_Core.Shared;

public interface IRelatorioService
{
    Task<Result<AgendaParticipanteResponse>> GetAgendaParticipanteAsync(string cpf);
    Task<Result<IEnumerable<FornecedorUtilizacaoResponse>>> GetFornecedoresMaisUtilizadosAsync(int pageNumber = 1, int pageSize = 1000);
    Task<Result<IEnumerable<TipoParticipanteFrequenciaResponse>>> GetTiposParticipantesMaisFrequentesAsync(int pageNumber = 1, int pageSize = 1000);
    Task<Result<IEnumerable<SaldoEventoResponse>>> GetSaldoOrcamentoEventosAsync(int pageNumber = 1, int pageSize = 1000);

}
