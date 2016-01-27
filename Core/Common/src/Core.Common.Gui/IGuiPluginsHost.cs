using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface describing the object that hosts all the leaded graphical user interface
    /// plugins of the application.
    /// </summary>
    public interface IGuiPluginsHost
    {
        /// <summary>
        /// List of plugins.
        /// </summary>
        IList<GuiPlugin> Plugins { get; }

        /// <summary>
        /// Returns GuiPlugin for a given type.
        /// TODO: a bit too implicit method, to be removed.
        /// </summary>
        /// <param name="type">Any type loaded from plugin.</param>
        /// <returns>Plugin gui associated with a given type</returns>
        GuiPlugin GetPluginGuiForType(Type type);

        /// <summary>
        /// Queries the plugins to get all data with view definitions recursively given a
        /// piece of hierarchical data.
        /// </summary>
        /// <param name="rootDataObject">The root data object.</param>
        /// <returns>An enumeration of all (child)data that have view definitions declared.</returns>
        IEnumerable GetAllDataWithViewDefinitionsRecursively(object rootDataObject);
    }
}