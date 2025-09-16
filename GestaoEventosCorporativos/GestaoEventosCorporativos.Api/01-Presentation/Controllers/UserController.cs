using GestaoEventosCorporativos.Api._01_Presentation.DTOs.Requests;
using GestaoEventosCorporativos.Api._01_Presentation.DTOs.Responses;
using GestaoEventosCorporativos.Api._01_Presentation.Helpers;
using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Services;
using GestaoEventosCorporativos.Api._02_Core.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEventosCorporativos.Api._01_Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public UsersController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserRequest request)
        {
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.Failure("Dados inválidos para usuário.",
                    StatusCodes.Status400BadRequest.ToString(), ModelState);

                return StatusCode(StatusCodes.Status400BadRequest, response);
            }

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password
            };

            var result = await _userService.AddAsync(user);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString(), request);
                return StatusCode(statusCode, response);
            }

            var userResponse = new UserResponse
            {
                Id = result.Data.Id,
                Name = result.Data.Name,
                Email = result.Data.Email
            };

            response.Success("Usuário criado com sucesso.",
                StatusCodes.Status201Created.ToString(), userResponse);

            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.Failure("Dados inválidos para login.",
                    StatusCodes.Status400BadRequest.ToString(), ModelState);

                return StatusCode(StatusCodes.Status400BadRequest, response);
            }

            var result = await _userService.LoginAsync(request.Email, request.Password);

            if (!result.IsSuccess)
            {
                int statusCode = MapError.MapErrorToStatusCode(result.ErrorCode);
                response.Failure(result.ErrorMessage, statusCode.ToString(), request);
                return StatusCode(statusCode, response);
            }

            var loginResponse = new LoginResponse(result.Data);

            response.Success("Login realizado com sucesso.",
                StatusCodes.Status200OK.ToString(), loginResponse);

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetProfile()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userId = _jwtService.GetUserIdFromToken(token);
            var userEmail = _jwtService.GetUserEmailFromToken(token);
            var userName = _jwtService.GetUserNameFromToken(token);

            var profile = new
            {
                UserId = userId,
                Email = userEmail,
                Nome = userName
            };

            return Ok(profile);
        }
    }
}
