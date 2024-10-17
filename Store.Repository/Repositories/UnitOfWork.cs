using Store.Date.Context;
using Store.Date.Entities;
using Store.Repository.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repository.Repositories
{
    public class UnitOfWork : IUnitOfwork
    {
        private readonly StoreDbContext context;
        private Hashtable repositories;

        public UnitOfWork(StoreDbContext context)
        {
            this.context = context;
        }

        public StoreDbContext Context { get; }

        public async Task<int> CompleteAsync()
            =>await context.SaveChangesAsync();

        public IGenericRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            if (repositories is null) repositories = new Hashtable();

            var entityKey = typeof(TEntity).Name; // "Product"
            if (!repositories.ContainsKey(entityKey))
            {
                var repositoryType = typeof(GenericRepository<,>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity), typeof(TKey)), context);
                repositories.Add(entityKey, repositoryInstance);
            }
            return (IGenericRepository<TEntity, TKey>)repositories[entityKey];
        }
    }
}
