using DelftTools.Shell.Core;
using DelftTools.Shell.Core.Dao;

namespace DeltaShell.Core
{
    /// <summary>
    /// Searches for a IProjectRepository implementation in the currently loaded assemblies and returns a new instance of it.
    /// </summary>
    public class ProjectRepositoryFactory<T> : IProjectRepositoryFactory where T : IProjectRepository, new()
    {
        public string[] PluginNames { get; set; }

        public string FileFormatVersion { get; set; }

        public bool SpeedUpSessionCreationUsingParallelThread { get; set; }

        public bool SpeedUpConfigurationCreationUsingCaching { get; set; }

        public string ConfigurationCacheDirectory { get; set; }

        public IProjectRepository CreateNew()
        {
            var repository = new T();

            return repository;
        }

        public void AddPlugin(IPlugin plugin) {}

        public void AddDataAccessListener(IDataAccessListener dataAccessListener) {}
    }
}