using Core.Common.Base.Data;

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface for exposing commands/methods related to saving/loading a <see cref="Project"/>.
    /// </summary>
    public interface IStorageCommands
    {
        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <remarks>
        /// The creation action might be cancelled (due to user interaction).
        /// </remarks>
        void CreateNewProject();

        /// <summary>
        /// Saves the project to a new location.
        /// </summary>
        /// <returns>Returns if the project was successfully saved.</returns>
        bool SaveProjectAs();

        /// <summary>
        /// Saves the project to the currently selected location.
        /// </summary>
        /// <returns>Returns if the project was successfully saved.</returns>
        bool SaveProject();

        /// <summary>
        /// Opens an existing project.
        /// </summary>
        /// <remarks>
        /// The opening action might be cancelled (due to user interaction).
        /// </remarks>
        /// <returns>Whether or not an existing project was correctly opened.</returns>
        bool OpenExistingProject();

        /// <summary>
        /// Opens an existing project from file.
        /// </summary>
        /// <param name="filePath">The path to the existing project file.</param>
        /// <remarks>
        /// The opening action might be cancelled (due to user interaction).
        /// </remarks>
        /// <returns>Whether or not an existing project was correctly opened.</returns>
        bool OpenExistingProject(string filePath);

        /// <summary>
        /// Closes the current project.
        /// </summary>
        void CloseProject();
    }
}