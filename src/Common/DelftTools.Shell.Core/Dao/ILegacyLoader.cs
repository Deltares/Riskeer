using System;
using System.Data;

namespace DelftTools.Shell.Core.Dao
{
    public interface ILegacyLoader
    {
        Func<object, object> DeproxifyFunc { get; set; }
        void OnAfterInitialize(object entity, IDbConnection dbConnection);
        void OnAfterProjectMigrated(Project project);
    }

    public abstract class LegacyLoader : ILegacyLoader
    {
        public Func<object, object> DeproxifyFunc { get; set; }

        public virtual void OnAfterInitialize(object entity, IDbConnection dbConnection) {}

        public virtual void OnAfterProjectMigrated(Project project) {}

        protected T Deproxify<T>(T potentialProxy)
        {
            return (T) DeproxifyFunc(potentialProxy);
        }
    }
}