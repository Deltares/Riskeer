namespace DelftTools.Shell.Core.Dao
{
    public interface IProjectRepositoryFactory
    {
        bool SpeedUpSessionCreationUsingParallelThread { get; set; }

        bool SpeedUpConfigurationCreationUsingCaching { get; set; }

        string ConfigurationCacheDirectory { get; set; }
        IProjectRepository CreateNew();

        void AddPlugin(IPlugin plugin);

        void AddDataAccessListener(IDataAccessListener dataAccessListener);
    }
}