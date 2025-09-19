using GestaoEventosCorporativos.Api._01_Presentation.DTOs.Responses;
using GestaoEventosCorporativos.Api._01_Presentation.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEventosCorporativos.Api._01_Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RelatoriosController : ControllerBase
    {
        private readonly IRelatorioService _relatorioService;

        public RelatoriosController(IRelatorioService relatorioService)
        {
            _relatorioService = relatorioService;
        }

        [HttpGet("agenda-participante/{cpf}")]
        public async Task<IActionResult> GetAgendaParticipante(string cpf)
        {
            var response = new ResponseDTO();

            var result = await _relatorioService.GetAgendaParticipanteAsync(cpf);
            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString());
                return StatusCode(statusCode, response);
            }

            response.Success("Agenda do participante gerada com sucesso.",
                StatusCodes.Status200OK.ToString(), result.Data);

            return Ok(response);
        }

        [HttpGet("fornecedores-mais-utilizados")]
        public async Task<IActionResult> GetFornecedoresMaisUtilizados([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var response = new ResponseDTO();

            var result = await _relatorioService.GetFornecedoresMaisUtilizadosAsync(pageNumber, pageSize);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString());
                return StatusCode(statusCode, response);
            }

            response.Success("Relatório de fornecedores mais utilizados gerado com sucesso.",
                StatusCodes.Status200OK.ToString(), result.Data);

            return Ok(response);
        }

        [HttpGet("tipos-participantes-frequentes")]
        public async Task<IActionResult> GetTiposParticipantesMaisFrequentes([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var response = new ResponseDTO();

            var result = await _relatorioService.GetTiposParticipantesMaisFrequentesAsync(pageNumber, pageSize);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString());
                return StatusCode(statusCode, response);
            }

            response.Success("Relatório de tipos de participantes gerado com sucesso.",
                StatusCodes.Status200OK.ToString(), result.Data);

            return Ok(response);
        }

        [HttpGet("eventos-saldo-orcamento")]
        public async Task<IActionResult> GetSaldoOrcamentoEventos([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var response = new ResponseDTO();

            var result = await _relatorioService.GetSaldoOrcamentoEventosAsync(pageNumber, pageSize);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString());
                return StatusCode(statusCode, response);
            }

            response.Success("Relatório de saldos de orçamento dos eventos gerado com sucesso.",
                StatusCodes.Status200OK.ToString(), result.Data);

            return Ok(response);
        }



    }
}
