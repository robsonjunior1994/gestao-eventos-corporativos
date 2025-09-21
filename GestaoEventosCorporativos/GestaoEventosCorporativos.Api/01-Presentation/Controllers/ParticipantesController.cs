using GestaoEventosCorporativos.Api._01_Presentation.DTOs.Requests;
using GestaoEventosCorporativos.Api._01_Presentation.DTOs.Responses;
using GestaoEventosCorporativos.Api._01_Presentation.Helpers;
using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEventosCorporativos.Api._01_Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ParticipantesController : ControllerBase
    {
        private readonly IParticipanteService _participanteService;

        public ParticipantesController(IParticipanteService participanteService)
        {
            _participanteService = participanteService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ParticipanteRequest request)
        {
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.Failure("Dados inválidos para o participante.",
                    StatusCodes.Status400BadRequest.ToString(), ModelState);

                return StatusCode(StatusCodes.Status400BadRequest, response);
            }

            var participante = new Participante
            {
                NomeCompleto = request.NomeCompleto,
                CPF = request.CPF,
                Telefone = request.Telefone,
                Tipo = request.Tipo
            };

            var result = await _participanteService.AddAsync(participante);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString(), request);
                return StatusCode(statusCode, response);
            }

            response.Success("Participante criado com sucesso.",
                StatusCodes.Status201Created.ToString(), new ParticipanteResponse
                {
                    Id = result.Data.Id,
                    NomeCompleto = result.Data.NomeCompleto,
                    CPF = result.Data.CPF,
                    Telefone = result.Data.Telefone,
                    Tipo = result.Data.Tipo.ToString()
                });

            return StatusCode(StatusCodes.Status201Created, response);
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var response = new ResponseDTO();
            var result = await _participanteService.GetAllAsync(pageNumber, pageSize);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString());
                return StatusCode(statusCode, response);
            }

            var participantesResponse = result.Data.Items.Select(p => new ParticipanteResponse
            {
                Id = p.Id,
                NomeCompleto = p.NomeCompleto,
                CPF = p.CPF,
                Telefone = p.Telefone,
                Tipo = p.Tipo.ToString()
            });

            var pagedResponse = new PagedResult<ParticipanteResponse>
            {
                Items = participantesResponse,
                TotalCount = result.Data.TotalCount,
                PageNumber = result.Data.PageNumber,
                PageSize = result.Data.PageSize
            };

            response.Success("Participantes recuperados com sucesso.",
                StatusCodes.Status200OK.ToString(), pagedResponse);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = new ResponseDTO();
            var result = await _participanteService.GetByIdAsync(id);

            if (!result.IsSuccess || result.Data == null)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode ?? ErrorCode.NOT_FOUND);
                response.Failure(result.ErrorMessage ?? "Participante não encontrado.", statusCode.ToString());
                return StatusCode(statusCode, response);
            }

            var participante = new ParticipanteResponse
            {
                Id = result.Data.Id,
                NomeCompleto = result.Data.NomeCompleto,
                CPF = result.Data.CPF,
                Telefone = result.Data.Telefone,
                Tipo = result.Data.Tipo.ToString()
            };

            response.Success("Participante recuperado com sucesso.",
                StatusCodes.Status200OK.ToString(), participante);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ParticipanteRequest request)
        {
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.Failure("Dados inválidos para o participante.",
                    StatusCodes.Status400BadRequest.ToString(), ModelState);

                return StatusCode(StatusCodes.Status400BadRequest, response);
            }

            var participante = new Participante
            {
                Id = id,
                NomeCompleto = request.NomeCompleto,
                CPF = request.CPF,
                Telefone = request.Telefone,
                Tipo = request.Tipo
            };

            var result = await _participanteService.UpdateAsync(participante);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString(), request);
                return StatusCode(statusCode, response);
            }

            response.Success("Participante atualizado com sucesso.",
                StatusCodes.Status200OK.ToString(), new ParticipanteResponse
                {
                    Id = result.Data.Id,
                    NomeCompleto = result.Data.NomeCompleto,
                    CPF = result.Data.CPF,
                    Telefone = result.Data.Telefone,
                    Tipo = result.Data.Tipo.ToString()
                });

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = new ResponseDTO();
            var result = await _participanteService.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString());
                return StatusCode(statusCode, response);
            }

            response.Success("Participante excluído com sucesso.",
                StatusCodes.Status200OK.ToString(), true);

            return Ok(response);
        }
    }
}
