using DelftTools.Shell.Core.Services;

namespace DelftTools.Shell.Core
{
    /// <summary>
    /// Imports another project into a project.
    /// 
    /// TODO: move it and the only 1 implementation into DeltaShell (default functionality)
    /// </summary>
    public interface IProjectImporter : IFileImporter
    {
        /// <summary>
        /// Project service used to open the project to import
        /// </summary>
        IProjectService ProjectService { get; set;}
    }
}