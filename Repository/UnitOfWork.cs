using ApiCatalogo.Context;
using System.Threading.Tasks;

namespace ApiCatalogo.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ProdutoRepository _produtoRepo;
        private CategoriaRepository _categoriaRepo;
        public CatalogoDBContext _context;

        public UnitOfWork(CatalogoDBContext contexto)
        {
            _context = contexto;
        }
        public IProdutoRepository ProdutoRepository
        {
            get
            {
                return _produtoRepo ??= new ProdutoRepository(_context);
            }
        }
        public ICategoriaRepository CategoriaRepository
        {
            get
            {
                return _categoriaRepo ??= new CategoriaRepository(_context);
            }
        }
        public async Task Commit()
        {
            await _context.SaveChangesAsync();

        }
        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
