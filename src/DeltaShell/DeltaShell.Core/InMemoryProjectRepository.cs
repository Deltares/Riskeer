using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DelftTools.Shell.Core;
using DelftTools.Shell.Core.Dao;
using DeltaShell.Core.Properties;

namespace DeltaShell.Core
{
    /// <summary>
    /// Default implementation of the IProjectRepository, used until project is saved first time.
    /// </summary>
    public class InMemoryProjectRepository : IProjectRepository
    {
        private Project project = new Project();
        private List<Type> typesToIncludeInMigration = new List<Type>();

        public string Path
        {
            get { return string.Empty; }
        }

        public string PreviousPath
        {
            get { return string.Empty; }
        }

        public bool IsOpen
        {
            get { return true; }
        }

        public List<Type> TypesToIncludeInMigration
        {
            get { return typesToIncludeInMigration; }
            set { typesToIncludeInMigration = value; }
        }

        public Project Open(string path)
        {
            throw new NotSupportedException(Resources.InMemoryProjectRepository_Open_Can_t_read_project_from_path__project_can_be_only_created_in_memory_using_this_project_repository);
        }

        public void Create(string path)
        {
            project = new Project();
        }

        public void Close()
        {
            project = null;
        }

        public void SaveAs(Project project, string targetPath)
        {
        }

        public Project GetProject()
        {
            return project;
        }

        public IEnumerable<T> GetAllEntities<T>()
        {
            yield break;
        }

        public void SaveOrUpdateEntity<T>(T obj)
        {
        }

        public void PreLoad<T>(params Expression<Func<T, object>>[] collectionToPreload) where T : class
        {
            throw new NotImplementedException();
        }

        public IList<Project> GetAll()
        {
            return new List<Project> { project };
        }

        public Project GetById(long id)
        {
            return project;
        }

        public void SaveOrUpdate(Project project)
        {
        }

        public void Dispose()
        {
            project = null;
        }
    }
}