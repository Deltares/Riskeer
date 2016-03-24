using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using Rhino.Mocks;

namespace Application.Ringtoets.Storage.Test.DbContext
{
    public static class DbTestSet
    {
        public static IDbSet<T> GetDbTestSet<T>(MockRepository mockRepository, ObservableCollection<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var dbSet = mockRepository.Stub<IDbSet<T>>();

            dbSet.Stub(m => m.Provider).Return(queryable.Provider);
            dbSet.Stub(m => m.Expression).Return(queryable.Expression);
            dbSet.Stub(m => m.ElementType).Return(queryable.ElementType);
            dbSet.Stub(m => m.GetEnumerator()).Return(queryable.GetEnumerator());
            dbSet.Stub(m => m.Local).Return(data);
            return dbSet;
        }
    }
}