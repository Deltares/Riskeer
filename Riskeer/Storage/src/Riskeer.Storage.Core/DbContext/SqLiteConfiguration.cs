using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;

namespace Riskeer.Storage.Core.DbContext
{
    public class SqLiteConfiguration : DbConfiguration
    {
        public SqLiteConfiguration()
        {
            DbProviderServices providerServices = (DbProviderServices) System.Data.SQLite.EF6.SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices));

            SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);

            SetProviderServices("System.Data.SQLite", providerServices);

            SetProviderFactory("System.Data.SQLite.EF6", System.Data.SQLite.EF6.SQLiteProviderFactory.Instance);

            SetProviderServices("System.Data.SQLite.EF6", providerServices);
        }
    }
}