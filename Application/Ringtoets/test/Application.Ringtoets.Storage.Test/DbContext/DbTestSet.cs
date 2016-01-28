using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Rhino.Mocks;

namespace Application.Ringtoets.Storage.Test.DbContext
{
    public static class DbTestSet
    {
        public static IDbSet<T> GetDbTestSet<T>(IList<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var dbSet = MockRepository.GenerateMock<IDbSet<T>, IQueryable>();

            dbSet.Stub(m => m.Provider).Return(queryable.Provider);
            dbSet.Stub(m => m.Expression).Return(queryable.Expression);
            dbSet.Stub(m => m.ElementType).Return(queryable.ElementType);
            dbSet.Stub(m => m.GetEnumerator()).Return(queryable.GetEnumerator());
            return dbSet;
        }
    }
}