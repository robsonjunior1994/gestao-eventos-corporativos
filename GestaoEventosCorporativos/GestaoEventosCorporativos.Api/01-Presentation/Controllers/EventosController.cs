using GestaoEventosCorporativos.Api._01_Presentation.DTOs.Requests;
using GestaoEventosCorporativos.Api._01_Presentation.DTOs.Responses;
using GestaoEventosCorporativos.Api._01_Presentation.Helpers;
using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Services;
using GestaoEventosCorporativos.Api._02_Core.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEventosCorporativos.Api._01_Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventosController : ControllerBase
    {
        private readonly IEventoService _eventoService;

        public EventosController(IEventoService eventoService)
        {
            _eventoService = eventoService;
        }

        // GET: api/eventos?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var response = new ResponseDTO();

            var result = await _eventoService.GetAllAsync(pageNumber, pageSize);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString());
                return StatusCode(statusCode, response);
            }

            var eventosResponse = result.Data.Items.Select(e => new EventoResponse
            {
                Id = e.Id,
                Nome = e.Nome,
                DataInicio = e.DataInicio,
                DataFim = e.DataFim,
                Local = e.Local,
                Endereco = e.Endereco,
                Observacoes = e.Observacoes,
                LotacaoMaxima = e.LotacaoMaxima,
                OrcamentoMaximo = e.OrcamentoMaximo,
                ValorTotalFornecedores = e.ValorTotalFornecedores,
                SaldoOrcamento = e.SaldoOrcamento,
                TipoEventoDescricao = e.TipoEvento?.Descricao,

                Participantes = e.Participantes?
                    .Where(pe => pe.Participante != null)
                    .Select(pe => pe.Participante!.NomeCompleto + ", CPF:" + pe.Participante!.CPF)
                    .ToList() ?? new List<string>(),

                Fornecedores = e.Fornecedores?
                    .Where(ef => ef.Fornecedor != null)
                    .Select(ef => ef.Fornecedor!.NomeServico + ", CNPJ:" + ef.Fornecedor!.CNPJ)
                    .ToList() ?? new List<string>()
            });

            var pagedResponse = new PagedResult<EventoResponse>
            {
                Items = eventosResponse,
                TotalCount = result.Data.TotalCount,
                PageNumber = result.Data.PageNumber,
                PageSize = result.Data.PageSize
            };

            response.Success("Events retrieved successfully.",
                StatusCodes.Status200OK.ToString(), pagedResponse);

            return Ok(response);
        }


        // GET: api/eventos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = new ResponseDTO();

            Result<Evento> result = await _eventoService.GetByIdAsync(id);

            if (!result.IsSuccess || result.Data == null)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode ?? ErrorCode.NOT_FOUND);
                response.Failure(result.ErrorMessage ?? "Event not found.", statusCode.ToString());
                return StatusCode(statusCode, response);
            }

            var eventoResponse = new EventoResponse
            {
                Id = result.Data.Id,
                Nome = result.Data.Nome,
                DataInicio = result.Data.DataInicio,
                DataFim = result.Data.DataFim,
                Local = result.Data.Local,
                Endereco = result.Data.Endereco,
                Observacoes = result.Data.Observacoes,
                LotacaoMaxima = result.Data.LotacaoMaxima,
                OrcamentoMaximo = result.Data.OrcamentoMaximo,
                ValorTotalFornecedores = result.Data.ValorTotalFornecedores,
                SaldoOrcamento = result.Data.SaldoOrcamento,
                TipoEventoDescricao = result.Data.TipoEvento?.Descricao,

                Participantes = result.Data.Participantes?
                    .Where(pe => pe.Participante != null)
                    .Select(pe => pe.Participante!.NomeCompleto + ", CPF:" + pe.Participante!.CPF)
                    .ToList() ?? new List<string>(),

                Fornecedores = result.Data.Fornecedores?
                    .Where(ef => ef.Fornecedor != null)
                    .Select(ef => ef.Fornecedor!.NomeServico + ", CNPJ:" + ef.Fornecedor!.CNPJ) // ou NomeFantasia se for seu campo
                    .ToList() ?? new List<string>()
            };

            response.Success("Event retrieved successfully.",
                StatusCodes.Status200OK.ToString(), eventoResponse);

            return Ok(response);
        }

        // POST: api/eventos
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EventoRequest request)
        {
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.Failure("Invalid event data.",
                    StatusCodes.Status400BadRequest.ToString(), ModelState);

                return StatusCode(StatusCodes.Status400BadRequest, response);
            }

            var evento = new Evento(
                nome: request.Nome,
                dataInicio: request.DataInicio,
                dataFim: request.DataFim,
                local: request.Local,
                endereco: request.Endereco,
                observacoes: request.Observacoes,
                lotacaoMaxima: request.LotacaoMaxima,
                orcamentoMaximo: request.OrcamentoMaximo,
                tipoEventoId: request.TipoEventoId
            );

            Result<Evento> result = await _eventoService.AddAsync(evento);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString(), request);
                return StatusCode(statusCode, response);
            }

            response.Success("Evento criado com sucesso.",
                StatusCodes.Status201Created.ToString(),
                new EventoResponse
                {
                    Id = result.Data.Id,
                    Nome = result.Data.Nome,
                    DataInicio = result.Data.DataInicio,
                    DataFim = result.Data.DataFim,
                    Local = result.Data.Local,
                    Endereco = result.Data.Endereco,
                    Observacoes = result.Data.Observacoes,
                    LotacaoMaxima = result.Data.LotacaoMaxima,
                    OrcamentoMaximo = result.Data.OrcamentoMaximo,
                    ValorTotalFornecedores = result.Data.ValorTotalFornecedores,
                    SaldoOrcamento = result.Data.SaldoOrcamento,
                    TipoEventoDescricao = result.Data.TipoEvento?.Descricao
                });

            return StatusCode(StatusCodes.Status201Created, response);
        }

        // PUT: api/eventos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EventoRequest request)
        {
            var response = new ResponseDTO();

            Result<Evento> result = await _eventoService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode ?? ErrorCode.NOT_FOUND);

                response.Failure(result.ErrorMessage ?? "Event not found.", statusCode.ToString());
                return StatusCode(statusCode, response);
            }

            var evento = result.Data;
            evento.Update(
                nome: request.Nome,
                dataInicio: request.DataInicio,
                dataFim: request.DataFim,
                local: request.Local,
                endereco: request.Endereco,
                observacoes: request.Observacoes,
                lotacaoMaxima: request.LotacaoMaxima,
                orcamentoMaximo: request.OrcamentoMaximo,
                tipoEventoId: request.TipoEventoId
            );

            Result<Evento> updateResult = await _eventoService.UpdateAsync(evento);

            if (!updateResult.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(updateResult.ErrorCode);
                response.Failure(updateResult.ErrorMessage, statusCode.ToString(), request);
                return StatusCode(statusCode, response);
            }

            response.Success("Evento atualizado com sucesso.",
                StatusCodes.Status201Created.ToString(),
                new EventoResponse
                {
                    Id = result.Data.Id,
                    Nome = result.Data.Nome,
                    DataInicio = result.Data.DataInicio,
                    DataFim = result.Data.DataFim,
                    Local = result.Data.Local,
                    Endereco = result.Data.Endereco,
                    Observacoes = result.Data.Observacoes,
                    LotacaoMaxima = result.Data.LotacaoMaxima,
                    OrcamentoMaximo = result.Data.OrcamentoMaximo,
                    ValorTotalFornecedores = result.Data.ValorTotalFornecedores,
                    SaldoOrcamento = result.Data.SaldoOrcamento,
                    TipoEventoDescricao = result.Data.TipoEvento?.Descricao
                });

            return Ok(response);
        }

        // DELETE: api/eventos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = new ResponseDTO();

            Result<bool> result = await _eventoService.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString());
                return StatusCode(statusCode, response);
            }

            response.Success("Event deleted successfully.",
                StatusCodes.Status200OK.ToString(), true);

            return Ok(response);
        }

        [HttpPost("{eventoId}/participantes")]
        public async Task<IActionResult> AddParticipante(int eventoId, [FromBody] EventoParticipanteRequest participanteEventoRequest)
        {
            var response = new ResponseDTO();

            if (string.IsNullOrWhiteSpace(participanteEventoRequest.CPF))
            {
                response.Failure("O CPF é obrigatório.",
                    StatusCodes.Status400BadRequest.ToString(), participanteEventoRequest);

                return StatusCode(StatusCodes.Status400BadRequest, response);
            }

            var result = await _eventoService.AddParticipanteByCpfAsync(eventoId, participanteEventoRequest.CPF);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString(), participanteEventoRequest);
                return StatusCode(statusCode, response);
            }

            response.Success("Participante adicionado ao evento com sucesso.",
                StatusCodes.Status201Created.ToString(), new ParticipanteResponse
                {
                    Id = result.Data.Id,
                    NomeCompleto = result.Data.NomeCompleto,
                    CPF = result.Data.CPF,
                    Tipo = result.Data.Tipo.ToString()
                });

            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPost("{eventoId}/fornecedores")]
        public async Task<IActionResult> AddFornecedor(int eventoId, [FromBody] EventoFornecedorRequest request)
        {
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.Failure("Dados inválidos para fornecedor.",
                    StatusCodes.Status400BadRequest.ToString(), ModelState);

                return StatusCode(StatusCodes.Status400BadRequest, response);
            }

            var result = await _eventoService.AddFornecedorByCnpjAsync(eventoId, request.CNPJ);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString(), request);
                return StatusCode(statusCode, response);
            }

            var fornecedorDto = new FornecedorResponse
            {
                Id = result.Data.Id,
                NomeServico = result.Data.NomeServico,
                CNPJ = result.Data.CNPJ,
                ValorBase = result.Data.ValorBase
            };

            response.Success("Fornecedor adicionado ao evento com sucesso.",
                StatusCodes.Status201Created.ToString(), fornecedorDto);

            return StatusCode(StatusCodes.Status201Created, response);
        }

    }
}
