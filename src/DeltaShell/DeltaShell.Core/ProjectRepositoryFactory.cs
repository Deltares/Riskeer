using DelftTools.Shell.Core;
using DelftTools.Shell.Core.Dao;

namespace DeltaShell.Core
{
    /// <summary>
    /// Searches for a IProjectRepository implementation in the currently loaded assemblies and returns a new instance of it.
    /// </summary>
    public class ProjectRepositoryFactory<T> : IProjectRepositoryFactory where T : IProjectRepository, new()
    {
        public IProjectRepository CreateNew()
        {
            var repository = new T();

            return repository;
        }

        public void AddPlugin(IPlugin plugin) {}

        public void AddDataAccessListener(IDataAccessListener dataAccessListener) {}
    }
}