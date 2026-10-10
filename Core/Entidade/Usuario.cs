using Core.DTO;
using Core.Entidade.Enums;
using Core.ValueObjects;

namespace Core.Entidade
{
    public class Usuario : EntidadeBase
    {
        public string Nome { get; set; }
        public Email Email { get; set; }
        public string Senha { get; set; }
        public int NivelAcessoId { get; set; }
        public bool Ativo { get; set; }
        
        public NivelAcesso NivelAcesso { get; set; }

        public static Usuario AdicionarUsuario(UsuarioAdicionarRequisicao usuario, NivelAcessoEnum nivelAcesso, string? senha)
        {
            return new Usuario
            {
                Nome = usuario.Nome,
                Email = new Email(usuario.Email),
                Senha = senha,
                NivelAcessoId = (int)nivelAcesso,
                Ativo = true,
                DataCriacao = DateTime.UtcNow
            };
        }

        public virtual void Atualizar(Usuario usuarioAntigo, UsuarioAtualizarRequisicao usuarioNovo, string? senha)
        {
            Nome = string.IsNullOrEmpty(usuarioNovo.Nome) ? usuarioAntigo.Nome : usuarioNovo.Nome;
            Email = string.IsNullOrEmpty(usuarioNovo.Email) ? usuarioAntigo.Email : new Email(usuarioNovo.Email);
            Senha = string.IsNullOrEmpty(senha) ? usuarioAntigo.Senha : senha;
            DataAtualizacao = DateTime.UtcNow;
        }

        public void Ativar()
        {
            Ativo = true;
        }

        public void Desativar()
        {
            Ativo = false;
        }
    }
}
