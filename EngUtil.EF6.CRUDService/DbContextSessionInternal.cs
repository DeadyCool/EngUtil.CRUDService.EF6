// --------------------------------------------------------------------------------
// <copyright filename="DbContextSessionInternal.cs" date="20-06-2016">(c) 2016 All Rights Reserved</copyright>
// <author>Oliver Engels</author>
// --------------------------------------------------------------------------------

using System.Data.Entity;

namespace EngUtil.EF6.CRUDService
{
    public class DbContextSessionInternal<T> : IDbContextService 
        where T : DbContext
    {
        protected T DbContext;

        public DbContextSessionInternal(T context)
        {
            DbContext = context;
        }

        public DbContext CreateContext()
        {
            return DbContext;            
        }
    }
}
