namespace DelftTools.Utils.IO
{
    /// <summary>
    /// Allows to postpone writing changes to the underlying access (file, web, etc.) until CommitChanges is called.
    /// Only one transaction at a time is allowed.
    /// </summary>
    public interface ITransactionalChangeAccess
    {
        void BeginChanges();

        void RollbackChanges();

        void CommitChanges();

        bool IsChanging { get; }
    }
}