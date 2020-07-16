using ApiCatalogo.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.Context
{
    public class CatalogoDBContext : DbContext
    {
        public CatalogoDBContext(DbContextOptions<CatalogoDBContext> options) : base(options)
        {
        }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }
    }
}
