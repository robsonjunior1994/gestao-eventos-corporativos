using GestaoEventosCorporativos.Api._01_Presentation.DTOs.Requests;
using GestaoEventosCorporativos.Api._01_Presentation.DTOs.Responses;
using GestaoEventosCorporativos.Api._01_Presentation.Helpers;
using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEventosCorporativos.Api._01_Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FornecedoresController : ControllerBase
    {
        private readonly IFornecedorService _fornecedorService;

        public FornecedoresController(IFornecedorService fornecedorService)
        {
            _fornecedorService = fornecedorService;
        }

        // GET: api/fornecedores?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var response = new ResponseDTO();
            var result = await _fornecedorService.GetAllAsync(pageNumber, pageSize);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString());
                return StatusCode(statusCode, response);
            }

            var fornecedoresResponse = result.Data.Items.Select(f => new FornecedorResponse
            {
                Id = f.Id,
                NomeServico = f.NomeServico,
                CNPJ = f.CNPJ,
                ValorBase = f.ValorBase
            });

            var pagedResponse = new PagedResult<FornecedorResponse>
            {
                Items = fornecedoresResponse,
                TotalCount = result.Data.TotalCount,
                PageNumber = result.Data.PageNumber,
                PageSize = result.Data.PageSize
            };

            response.Success("Fornecedores recuperados com sucesso.",
                StatusCodes.Status200OK.ToString(), pagedResponse);

            return Ok(response);
        }


        // GET: api/fornecedores/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = new ResponseDTO();
            var result = await _fornecedorService.GetByIdAsync(id);

            if (!result.IsSuccess || result.Data == null)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode ?? ErrorCode.NOT_FOUND);
                response.Failure(result.ErrorMessage ?? "Fornecedor não encontrado.", statusCode.ToString());
                return StatusCode(statusCode, response);
            }

            var dto = new FornecedorResponse
            {
                Id = result.Data.Id,
                NomeServico = result.Data.NomeServico,
                CNPJ = result.Data.CNPJ,
                ValorBase = result.Data.ValorBase
            };

            response.Success("Fornecedor recuperado com sucesso.",
                StatusCodes.Status200OK.ToString(), dto);

            return Ok(response);
        }

        // POST: api/fornecedores
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FornecedorRequest request)
        {
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.Failure("Dados inválidos para fornecedor.",
                    StatusCodes.Status400BadRequest.ToString(), ModelState);
                return StatusCode(StatusCodes.Status400BadRequest, response);
            }

            var fornecedor = new Fornecedor
            {
                NomeServico = request.NomeServico,
                CNPJ = request.CNPJ,
                ValorBase = request.ValorBase
            };

            var result = await _fornecedorService.AddAsync(fornecedor);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString(), request);
                return StatusCode(statusCode, response);
            }

            var dto = new FornecedorResponse
            {
                Id = result.Data.Id,
                NomeServico = result.Data.NomeServico,
                CNPJ = result.Data.CNPJ,
                ValorBase = result.Data.ValorBase
            };

            response.Success("Fornecedor criado com sucesso.",
                StatusCodes.Status201Created.ToString(), dto);

            return StatusCode(StatusCodes.Status201Created, response);
        }

        // PUT: api/fornecedores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FornecedorRequest request)
        {
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.Failure("Dados inválidos para fornecedor.",
                    StatusCodes.Status400BadRequest.ToString(), ModelState);
                return StatusCode(StatusCodes.Status400BadRequest, response);
            }

            var fornecedor = new Fornecedor
            {
                Id = id,
                NomeServico = request.NomeServico,
                CNPJ = request.CNPJ,
                ValorBase = request.ValorBase
            };

            var result = await _fornecedorService.UpdateAsync(fornecedor);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString(), request);
                return StatusCode(statusCode, response);
            }

            var dto = new FornecedorResponse
            {
                Id = result.Data.Id,
                NomeServico = result.Data.NomeServico,
                CNPJ = result.Data.CNPJ,
                ValorBase = result.Data.ValorBase
            };

            response.Success("Fornecedor atualizado com sucesso.",
                StatusCodes.Status200OK.ToString(), dto);

            return Ok(response);
        }

        // DELETE: api/fornecedores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = new ResponseDTO();

            var result = await _fornecedorService.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString());
                return StatusCode(statusCode, response);
            }

            response.Success("Fornecedor excluído com sucesso.",
                StatusCodes.Status200OK.ToString(), true);

            return Ok(response);
        }
    }
}
