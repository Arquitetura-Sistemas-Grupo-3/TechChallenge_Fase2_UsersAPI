using Core.DTO;
using Core.Entidade;

namespace Core.Repository
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        public Task<Usuario?> ValidaEmailSenha(string email);
        public Task<List<UsuarioListarResposta>> ListarUsuario();
        public Task<UsuarioBuscarPorIdResposta?> BuscarUsuarioPorId(int id);
        public Task<Usuario?> ValidaEmail(string email);
        public Task<UsuarioBuscarAutenticadoResposta?> BuscarUsuarioPorEmail(string email);
    }
}
