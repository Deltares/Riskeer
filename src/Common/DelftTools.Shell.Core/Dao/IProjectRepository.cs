using System;
using DelftTools.Utils.Data;

namespace DelftTools.Shell.Core.Dao
{
    public interface IProjectRepository : IObjectRepository<Project, long>, IDisposable
    {
        /// <summary>
        /// Path to the current repository.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Returns true if repository is currently opened.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Opens repository using provided path.
        /// </summary>
        /// <param name="path">Path to project repository, usually file with .dsproj extension.</param>
        Project Open(string path);

        /// <summary>
        /// Creates a new repository and establishes and opens it.
        /// </summary>
        /// <param name="path"></param>
        void Create(string path);

        /// <summary>
        /// Closes repository file.
        /// </summary>
        void Close();

        void SaveAs(Project project, string targetPath);

        /// <summary>
        /// Gets project object from the repository. Project repository always contains one Project.
        /// </summary>
        /// <returns></returns>
        Project GetProject();
    }
}