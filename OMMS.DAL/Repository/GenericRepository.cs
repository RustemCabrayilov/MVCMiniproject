using Microsoft.EntityFrameworkCore;
using OMMS.DAL.Data;
using OMMS.DAL.Entities;
using OMMS.DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMMS.DAL.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity, new()
	{
		private readonly OMMSDbContext _dbContext;
		private readonly DbSet<T> _dbSet;

		public GenericRepository(OMMSDbContext dbContext)
		{
			_dbContext = dbContext;
			_dbSet = dbContext.Set<T>();
		}

		public async Task<T> Create(T entity)
		{
			await _dbSet.AddAsync(entity);
			return entity;
		}

		public async Task<T> Get(int id,params string[] includes)
		{
		/*	var entity = await _dbSet.FindAsync(id);
			return entity;*/
            var query = _dbSet.Where(x => x.Id == id).AsQueryable();

            if (includes is not null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

		public async Task<IQueryable<T>> GetAll()
		{
			return  _dbSet;
		}

		public T Remove(int id)
		{
			var entity =  _dbSet.Find(id);
			_dbSet.Remove(entity);
			return entity;
		}

		public T Update(T entity)
		{
			
			_dbSet.Update(entity);
			return entity;
		}
		public async Task SaveAsync()
		{
			await _dbContext.SaveChangesAsync();
		}
		public void Save()
		{
			 _dbContext.SaveChanges();
		}
	}
}
