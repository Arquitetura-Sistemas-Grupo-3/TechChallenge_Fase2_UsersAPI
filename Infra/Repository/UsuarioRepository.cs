using Core.DTO;
using Core.Entidade;
using Core.Repository;
using Core.ValueObjects;
using Microsoft.EntityFrameworkCore;


namespace Infra.Repository
{
    public class UsuarioRepository : EFRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(ApplicationDbContext context) 
            : base(context)
        {
        }

        public async Task<Usuario?> ValidaEmailSenha(string email)
        {
            if (!Email.TentarCriar(email, out var emailVo)) return null;

            return await _dbSet.FirstOrDefaultAsync(u => u.Email == emailVo && u.Ativo);
        }

        public async Task<Usuario?> ValidaEmail(string email)
        {
            if (!Email.TentarCriar(email, out var emailVo)) return null;

            return await _dbSet.FirstOrDefaultAsync(u => u.Email == emailVo);
        }

        public async Task<List<UsuarioListarResposta>> ListarUsuario()
        {
            return await _dbSet
                .Include(u => u.NivelAcesso)
                .Where(u => u.Ativo)
                .Select(u => new UsuarioListarResposta { Id = u.Id, Nome = u.Nome, Email = u.Email.Endereco, DataCriacao = u.DataCriacao, NivelAcesso = u.NivelAcesso.Nome })
                .ToListAsync();

        }

        public async Task<UsuarioBuscarPorIdResposta?> BuscarUsuarioPorId(int id)
        {
            return await _dbSet
                .Where(u => u.Ativo)
                .Select(u => new UsuarioBuscarPorIdResposta { Id = u.Id, Nome = u.Nome, Email = u.Email.Endereco, DataCriacao = u.DataCriacao, NivelAcesso = u.NivelAcesso.Nome })
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<UsuarioBuscarAutenticadoResposta?> BuscarUsuarioPorEmail(string email)
        {
            if (!Email.TentarCriar(email, out var emailVo)) return null;

            return await _dbSet
                .Where(u => u.Email == emailVo)
                .Select(u => new UsuarioBuscarAutenticadoResposta { Id = u.Id, Nome = u.Nome, Email = u.Email.Endereco, DataCriacao = u.DataCriacao, NivelAcesso = u.NivelAcesso.Nome })
                .FirstOrDefaultAsync();
        }
    }
}
