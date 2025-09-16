using GestaoEventosCorporativos.Api._01_Presentation.DTOs.Requests;
using GestaoEventosCorporativos.Api._01_Presentation.DTOs.Responses;
using GestaoEventosCorporativos.Api._01_Presentation.Helpers;
using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Services;
using GestaoEventosCorporativos.Api._02_Core.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEventosCorporativos.Api._01_Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoEventosController : ControllerBase
    {
        private readonly ITipoEventoService _tipoEventoService;

        public TipoEventosController(ITipoEventoService tipoEventoService)
        {
            _tipoEventoService = tipoEventoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = new ResponseDTO();
            var result = await _tipoEventoService.GetAllAsync();

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString());
                return StatusCode(statusCode, response);
            }

            var tipos = result.Data.Select(t => new TipoEventoResponse
            {
                Id = t.Id,
                Descricao = t.Descricao
            });

            response.Success("Tipos de evento recuperados com sucesso.",
                StatusCodes.Status200OK.ToString(), tipos);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = new ResponseDTO();
            var result = await _tipoEventoService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString());
                return StatusCode(statusCode, response);
            }

            var tipo = new TipoEventoResponse
            {
                Id = result.Data.Id,
                Descricao = result.Data.Descricao
            };

            response.Success("Tipo de evento recuperado com sucesso.",
                StatusCodes.Status200OK.ToString(), tipo);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TipoEventoRequest request)
        {
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.Failure("Dados inválidos para o tipo de evento.",
                    StatusCodes.Status400BadRequest.ToString(), ModelState);

                return StatusCode(StatusCodes.Status400BadRequest, response);
            }

            var tipoEvento = new TipoEvento { Descricao = request.Descricao };
            var result = await _tipoEventoService.AddAsync(tipoEvento);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString(), request);
                return StatusCode(statusCode, response);
            }

            response.Success("Tipo de evento criado com sucesso.",
                StatusCodes.Status201Created.ToString(), new TipoEventoResponse
                {
                    Id = result.Data.Id,
                    Descricao = result.Data.Descricao
                });

            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TipoEventoRequest request)
        {
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.Failure("Dados inválidos para o tipo de evento.",
                    StatusCodes.Status400BadRequest.ToString(), ModelState);

                return StatusCode(StatusCodes.Status400BadRequest, response);
            }

            var tipoEvento = new TipoEvento
            {
                Id = id,
                Descricao = request.Descricao
            };

            var result = await _tipoEventoService.UpdateAsync(tipoEvento);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString(), request);
                return StatusCode(statusCode, response);
            }

            response.Success("Tipo de evento atualizado com sucesso.",
                StatusCodes.Status200OK.ToString(), new TipoEventoResponse
                {
                    Id = result.Data.Id,
                    Descricao = result.Data.Descricao
                });

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = new ResponseDTO();
            var result = await _tipoEventoService.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString());
                return StatusCode(statusCode, response);
            }

            response.Success("Tipo de evento excluído com sucesso.",
                StatusCodes.Status200OK.ToString(), true);

            return Ok(response);
        }
    }
}
