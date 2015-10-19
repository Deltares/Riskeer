namespace DelftTools.Shell.Core.Dao
{
    public interface IProjectRepositoryFactory
    {
        IProjectRepository CreateNew();

        void AddPlugin(IPlugin plugin);

        void AddDataAccessListener(IDataAccessListener dataAccessListener);
    }
}