using ApiCatalogo.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ApiCatalogo.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected CatalogoDBContext _contexto;
        public Repository(CatalogoDBContext contexto)
        {
            _contexto = contexto;
        }

        public IQueryable<T> Get()
        {
            return _contexto.Set<T>().AsNoTracking();
        }

        public void Add(T entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public T GetById(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
