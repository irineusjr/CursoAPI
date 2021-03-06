﻿using ApiCatalogo.Context;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<T> GetById(Expression<Func<T, bool>> predicate)
        {
            return await _contexto.Set<T>().SingleOrDefaultAsync(predicate);
        }

        public void Add(T entity)
        {
            _contexto.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _contexto.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            _contexto.Entry(entity).State = EntityState.Modified;
            _contexto.Set<T>().Update(entity);
        }
    }
}
