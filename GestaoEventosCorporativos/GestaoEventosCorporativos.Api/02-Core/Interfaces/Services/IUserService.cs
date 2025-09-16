using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Shared;

namespace GestaoEventosCorporativos.Api._02_Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<Result<User>> AddAsync(User user);
        Task<Result<string>> LoginAsync(string email, string senha);
    }
}
