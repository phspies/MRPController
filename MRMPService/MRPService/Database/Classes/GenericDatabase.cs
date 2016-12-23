using MRMPService.MRMPService.Log;
using MRMPService.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace MRMPService.LocalDatabase
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private DbContext context;
        private DbSet<TEntity> dbSet;


        public GenericRepository(DbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }
        public virtual List<TEntity> GetAll()
        {
            return dbSet.ToList();
        }
        public virtual List<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProperty);

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            return query.ToList();
        }

        public virtual TEntity GetById(object id)
        {
            return dbSet.Find(id);
        }

        public virtual bool Exists(Expression<Func<TEntity, bool>> filter = null)
        {
            return dbSet.Any(filter);
        }

        public virtual TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = dbSet;

            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            return query.FirstOrDefault(filter);
        }

        public virtual void Insert(TEntity entity)
        {
            try
            {
                dbSet.Add(entity);
                context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Logger.log(String.Format("Error adding record to database: {0} : {1} ({2})", validationError.PropertyName, validationError.ErrorMessage, JsonConvert.SerializeObject(entity)), Logger.Severity.Fatal);
                        throw new Exception("Error adding record to database");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error adding record to database: {0} : {1}", ex.GetBaseException().Message, JsonConvert.SerializeObject(entity)),Logger.Severity.Fatal);
                throw new Exception("Error adding record to database");
            }

        }

        public virtual void Update(TEntity entity)
        {
            try
            {
                dbSet.Attach(entity);
                context.Entry(entity).State = EntityState.Modified;
                context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Logger.log(String.Format("Error updating database: {0} : {1} ({2})", validationError.PropertyName, validationError.ErrorMessage, JsonConvert.SerializeObject(entity)), Logger.Severity.Fatal);
                        throw new Exception("Error updating database");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error updating database: {0} : {1}", ex.GetBaseException().Message, JsonConvert.SerializeObject(entity)), Logger.Severity.Fatal);
                throw new Exception("Error updating database");
            }
        }

        public virtual void Delete(object id)
        {
            try
            {
                TEntity entityToDelete = dbSet.Find(id);
                if (context.Entry(entityToDelete).State == EntityState.Detached)
                {
                    dbSet.Attach(entityToDelete);
                }
                dbSet.Remove(entityToDelete);
                context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Logger.log(String.Format("Error deleting record form databasee: {0} : {1} ({2})", validationError.PropertyName, validationError.ErrorMessage, JsonConvert.SerializeObject(id)), Logger.Severity.Fatal);
                        throw new Exception("Error deleting record form database");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log(String.Format("Error deleting record form database: {0} : {1}", ex.GetBaseException().Message, JsonConvert.SerializeObject(id)), Logger.Severity.Fatal);
                throw new Exception("Error deleting record from database");
            }

        }
    }
}
