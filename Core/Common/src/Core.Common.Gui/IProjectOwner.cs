using System;

using Core.Common.Base.Data;

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface declaring members related to owning an <see cref="Project"/>.
    /// </summary>
    public interface IProjectOwner
    {
        /// <summary>
        /// Occurs when a new instance is available at <see cref="Project"/>.
        /// </summary>
        event Action<Project> ProjectOpened;

        /// <summary>
        /// Occurs when the instance available at <see cref="Project"/> is removed.
        /// </summary>
        event Action<Project> ProjectClosing;

        /// <summary>
        /// Gets or sets the project of the application.
        /// </summary>
        Project Project { get; set; }

        /// <summary>
        /// Gets or sets the project path of the application.
        /// </summary>
        string ProjectFilePath { get; set; } 
    }
}