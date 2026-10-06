using Core.Entidade;
using MassTransit;
using UsersAPI.Interface;
using static UsersAPI.Services.UsuarioService;

namespace UsersAPI.Services
{
    public class MensageriaService : IMensageriaService
    {
        private readonly IPublishEndpoint _publish;

        public MensageriaService(IPublishEndpoint publish)
        {
            _publish = publish;
        }

        public async Task<UsuarioCriado> PublicarMensagemFila(Usuario usuarioInput, CancellationToken ct = default)
        {
            var evento = new UsuarioCriado(Guid.NewGuid(), usuarioInput.Nome, usuarioInput.Email, DateTime.UtcNow);
            await _publish.Publish(evento, ct);
            return evento;
        }
    }
}
