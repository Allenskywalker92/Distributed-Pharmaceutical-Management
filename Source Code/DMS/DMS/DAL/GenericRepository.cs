using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace DMS.DAL
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        internal DMPEntities Context;
        internal DbSet<TEntity> dbSet;

        public GenericRepository(DMPEntities context)
        {
           this.Context = context;
            this.dbSet = context.Set<TEntity>();
        }

        //public TEntity Add(TEntity tEntity)
        //{
        //    return dbSet.Add(tEntity);
        //}
        //public TEntity Remove(TEntity tEntity)
        //{
        //    return dbSet.Remove(tEntity);
        //}
        //public TEntity Update(TEntity tEntity)
        //{
        //    var updated = dbSet.Attach(tEntity);
        //    Context.DbContext.Entry(tEntity).State=EntityState.Modified;
        //    return updated;
        //}
        //public IQueryable<TEntity> GetAll(TEntity tEntity)
        //{
        //    return dbSet;
        //}
        //public TEntity Get(object key)
        //{
        //    return dbSet.Find(key);
        //}
        public void SaveChanges()
        {
            Context.SaveChanges();
        }
        public virtual IEnumerable<TEntity> GetAll()
        {
            IQueryable<TEntity> query = dbSet;
            return query.ToList();
        }
        public virtual IEnumerable<TEntity> Get(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }
        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual bool Insert(TEntity entity)
        {
            try
            {
                dbSet.Add(entity);
                return true;
            }
            catch (Exception e)
            {
                var message = e.Message;
                return false;
            }
            
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }

    }
}