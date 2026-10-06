using Core.Entidade;
using static UsersAPI.Services.UsuarioService;

namespace UsersAPI.Interface
{
    public interface IMensageriaService
    {
        Task<UsuarioCriado> PublicarMensagemFila(Usuario usuario, CancellationToken ct);
    }
}
