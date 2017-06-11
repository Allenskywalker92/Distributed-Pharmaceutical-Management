using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DMS.DAL
{
    public class Context<T>:IContext<T> where T : class 
    {
        public Context()
        {
            DbContext = new DbContext(ConfigurationManager.ConnectionStrings["DMPEntities"].ConnectionString);
            DbSet = DbContext.Set<T>();
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }

        public DbContext DbContext { get; private set; }
        public IDbSet<T> DbSet { get; private set; }
    }
}