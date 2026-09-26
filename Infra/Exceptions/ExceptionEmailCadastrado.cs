namespace Infra.Exceptions
{
    public class ExceptionEmailCadastrado : ExceptionBase
    {
        public ExceptionEmailCadastrado(string message) : base(message)
        {
        }

        public override int StatusCode { get; set; } = 400;
    }
}

