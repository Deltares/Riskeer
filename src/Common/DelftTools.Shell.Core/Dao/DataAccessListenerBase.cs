namespace DelftTools.Shell.Core.Dao
{
    public abstract class DataAccessListenerBase : IDataAccessListener
    {
        public virtual IProjectRepository ProjectRepository { get; set; }

        public abstract object Clone();
    }
}