
namespace Infra.Exceptions
{
    public class ExcepetionUsuarioNaoEncontrado : ExceptionBase
    {
        public ExcepetionUsuarioNaoEncontrado(string message) : base(message)
        {
        }
        public override int StatusCode { get; set; } = 404;

    }
}
