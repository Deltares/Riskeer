using System;
using System.Data;

namespace DelftTools.Shell.Core.Dao
{
    public interface ILegacyLoader
    {
        void OnAfterInitialize(object entity, IDbConnection dbConnection);
        void OnAfterProjectMigrated(Project project);
        Func<object, object> DeproxifyFunc { get; set; }
    }

    public abstract class LegacyLoader : ILegacyLoader
    {
        public virtual void OnAfterInitialize(object entity, IDbConnection dbConnection)
        {
        }

        public virtual void OnAfterProjectMigrated(Project project)
        {
        }

        public Func<object, object> DeproxifyFunc { get; set; }

        protected T Deproxify<T>(T potentialProxy)
        {
            return (T) DeproxifyFunc(potentialProxy);
        }
    }
}