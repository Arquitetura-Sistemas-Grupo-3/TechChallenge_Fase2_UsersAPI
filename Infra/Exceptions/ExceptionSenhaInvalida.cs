namespace Infra.Exceptions
{
    public class ExceptionSenhaInvalida : ExceptionBase
    {
        public ExceptionSenhaInvalida(string message) : base(message)
        {
        }
        public override int StatusCode { get; set; } = 400;

    }
}
