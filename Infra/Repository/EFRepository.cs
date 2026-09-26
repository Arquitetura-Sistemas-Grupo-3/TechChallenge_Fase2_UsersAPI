using Core.Entidade;
using Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repository
{
    public class EFRepository<T> : IRepository<T> where T : EntidadeBase
    {
        protected ApplicationDbContext _context;
        protected DbSet<T> _dbSet;

        public EFRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public void Alterar(T entidade)
        {
            _dbSet.Update(entidade);
            _context.SaveChangesAsync();
        }

        public void Cadastrar(T entidade)
        {
            _dbSet.Add(entidade);
            _context.SaveChangesAsync();
        }

        public async Task Deletar(int id)
        {
            _dbSet.Remove(await ObterPorId(id));
            await _context.SaveChangesAsync();
        }

        public async Task<T> ObterPorId(int id) => await _dbSet.FirstOrDefaultAsync(e => e.Id == id);

        public async Task<IList<T>> ObterTodos() => await _dbSet.ToListAsync();
    }
}
