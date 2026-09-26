using Core.DTO;
using Core.Output;

namespace UsersAPI.Interface
{
    public interface IAuthService
    {
        Task<ServiceResponse<AutenticarResposta>> Autenticar(string email, string senha);
    }
}
