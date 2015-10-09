using DelftTools.Shell.Core.Services;

namespace DelftTools.Shell.Core
{
    /// <summary>
    /// Imports another project into a project.
    /// </summary>
    public interface IProjectItemExporter : IFileExporter
    {
        /// <summary>
        /// Project service used to export project
        /// </summary>
        IProjectService ProjectService { get; set; }
    }
}