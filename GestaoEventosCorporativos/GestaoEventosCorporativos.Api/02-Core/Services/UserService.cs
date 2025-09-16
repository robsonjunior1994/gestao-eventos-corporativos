using GestaoEventosCorporativos.Api._01_Presentation.Helpers;
using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Services;
using GestaoEventosCorporativos.Api._02_Core.Shared;

namespace GestaoEventosCorporativos.Api._02_Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IEncryptionPasswordService _encryptionPasswordService;

        public UserService(
            IUserRepository userRepository,
            IJwtService jwtService,
            IEncryptionPasswordService encryptionPasswordService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _encryptionPasswordService = encryptionPasswordService;
        }

        public async Task<Result<User>> AddAsync(User user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.Name))
                    return Result<User>.Failure("O nome é obrigatório.", ErrorCode.VALIDATION_ERROR);

                if (string.IsNullOrWhiteSpace(user.Email))
                    return Result<User>.Failure("O e-mail é obrigatório.", ErrorCode.VALIDATION_ERROR);

                if (string.IsNullOrWhiteSpace(user.Password))
                    return Result<User>.Failure("A senha é obrigatória.", ErrorCode.VALIDATION_ERROR);

                // Verifica duplicidade
                var existingUser = await _userRepository.GetByEmailAsync(user.Email);
                if (existingUser != null)
                    return Result<User>.Failure("Já existe um usuário com este e-mail.", ErrorCode.RESOURCE_ALREADY_EXISTS);

                // Criptografar senha antes de salvar
                user.Password = _encryptionPasswordService.EncryptPassword(user.Password);

                await _userRepository.AddAsync(user);
                return Result<User>.Success(user);
            }
            catch (Exception)
            {
                return Result<User>.Failure("Erro ao criar usuário.", ErrorCode.DATABASE_ERROR);
            }
        }

        public async Task<Result<string>> LoginAsync(string email, string senha)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                    return Result<string>.Failure("Usuário não encontrado.", ErrorCode.NOT_FOUND);

                // Validar senha com hash + salt
                var isPasswordValid = _encryptionPasswordService.ValidatePassword(senha, user.Password);
                if (!isPasswordValid)
                    return Result<string>.Failure("Senha inválida.", ErrorCode.VALIDATION_ERROR);

                var token = _jwtService.GenerateToken(user);
                return Result<string>.Success(token.Data);
            }
            catch (Exception)
            {
                return Result<string>.Failure("Erro ao realizar login.", ErrorCode.DATABASE_ERROR);
            }
        }

    }
}
