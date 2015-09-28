namespace DelftTools.Shell.Core.Dao
{
    public abstract class DataAccessListenerBase : IDataAccessListener
    {
        public virtual IProjectRepository ProjectRepository { get; set; }
        public abstract object Clone();

        #region Other

        public virtual void OnPreLoad(object entity, object[] loadedState, string[] propertyNames)
        {
        }

        public virtual void OnPostLoad(object entity, object[] state, string[] propertyNames)
        {
        }

        public virtual bool OnPreUpdate(object entity, object[] state, string[] propertyNames)
        {
            return false;
        }

        public virtual bool OnPreInsert(object entity, object[] state, string[] propertyNames)
        {
            return false;
        }

        public virtual void OnPostUpdate(object entity, object[] state, string[] propertyNames)
        {
        }

        public virtual void OnPostInsert(object entity, object[] state, string[] propertyNames)
        {
        }

        public virtual bool OnPreDelete(object entity, object[] deletedState, string[] propertyNames)
        {
            return false;
        }

        public virtual void OnPostDelete(object entity, object[] deletedState, string[] propertyNames)
        {
        }

        #endregion
    }
}