using System;

namespace DelftTools.Shell.Core.Dao
{
    public interface IDataAccessListener : ICloneable, IProjectRepositoryListener
    {
        void OnPreLoad(object entity, object[] loadedState, string[] propertyNames);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="state"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        void OnPostLoad(object entity, object[] state, string[] propertyNames);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="state"></param>
        /// <param name="propertyNames"></param>
        /// <returns>true if the user modified the state in any way</returns>
        bool OnPreUpdate(object entity, object[] state, string[] propertyNames);

        /// <param name="entity"></param>
        /// <param name="state"></param>
        /// <param name="propertyNames"></param>
        /// <returns>true if the object should be vetoed and not inserted in the db</returns>
        bool OnPreInsert(object entity, object[] state, string[] propertyNames);

        /// <summary>
        /// Fired after entity was updated.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="state"></param>
        /// <param name="propertyNames"></param>
        void OnPostUpdate(object entity, object[] state, string[] propertyNames);

        /// <summary>
        /// Fired after entity was inserted.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="state"></param>
        /// <param name="propertyNames"></param>
        void OnPostInsert(object entity, object[] state, string[] propertyNames);

        /// <summary>
        /// Fired before entity is deleted
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="deletedState"></param>
        /// <param name="propertyNames"></param>
        /// <returns>true if you want to veto the delete</returns>
        bool OnPreDelete(object entity, object[] deletedState, string[] propertyNames);

        /// <summary>
        /// Fired after entity is deleted
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="deletedState"></param>
        /// <param name="propertyNames"></param>
        void OnPostDelete(object entity, object[] deletedState, string[] propertyNames);
    }
}