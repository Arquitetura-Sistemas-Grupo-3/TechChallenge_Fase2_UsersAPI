namespace Core.DTO
{
    public class UsuarioBuscarPorIdResposta
    {
        public int Id { get; set; }
        public DateTime DataCriacao { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string NivelAcesso { get; set; }
    }
}
