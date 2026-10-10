using Contracts.Events;
using Core.Entidade;

namespace UsersAPI.Interface
{
    public interface IMensageriaService
    {
        Task EnviarMensagemFila(Usuario usuario);
    }
}
