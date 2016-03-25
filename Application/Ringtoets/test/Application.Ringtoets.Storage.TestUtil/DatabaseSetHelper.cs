using System.Data.Entity;
using Application.Ringtoets.Storage.DbContext;
using Rhino.Mocks;

namespace Application.Ringtoets.Storage.TestUtil
{
    public static class DatabaseSetHelper
    {
        public static void AddSetExpectancy<T>(MockRepository mocks, IRingtoetsEntities entities) where T : class
        {
            var set = mocks.Stub<DbSet<T>>();
            entities.Stub(c => c.Set<T>()).Return(set);
        } 
    }
}