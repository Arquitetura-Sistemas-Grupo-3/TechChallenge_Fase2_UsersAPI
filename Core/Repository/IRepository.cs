using Core.Entidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repository
{
    public interface IRepository<T> where T : EntidadeBase
    {
        Task<IList<T>> ObterTodos();
        Task<T> ObterPorId(int id);
        public void Cadastrar(T entidade);
        public void Alterar(T entidade);
        Task Deletar(int id);
    }
}
