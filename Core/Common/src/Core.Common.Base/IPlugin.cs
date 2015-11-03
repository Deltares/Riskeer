using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Resources;
using Mono.Addins;

namespace Core.Common.Base
{
    [TypeExtensionPoint]
    public interface IPlugin
    {
        /// <summary>
        ///  Gets the name of the plugin.
        ///  <value>The name.</value></summary>
        string Name { get; }

        /// <summary>
        ///  Gets the name of the plugin as displayed in the Gui.
        ///  <value>The name.</value></summary>
        string DisplayName { get; }

        /// <summary>
        ///  Gets the description.
        ///  <value>The description.</value></summary>
        string Description { get; }

        /// <summary>
        ///  Gets the version of the plugin.
        ///  <value>The version.</value></summary>
        string Version { get; }

        /// <summary>
        ///  Gets a value indicating whether the plugin is active.
        ///  <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value></summary>
        bool IsActive { get; }

        /// <summary>
        ///  Image for displaying in gui. Default format 32x32 bitmap or scalable. 
        ///  </summary>
        Image Image { get; }

        /// <summary>
        ///  ResourceManger of plugin. Default Properties.Resources.
        ///  </summary>
        ResourceManager Resources { get; set; }

        /// <summary>
        ///  Activates the plugin.
        ///  </summary>
        void Activate();

        /// <summary>
        ///  Deactivates the plugin.
        ///  </summary>
        void Deactivate();

        // TODO: check if we can remove it
        IEnumerable<Assembly> GetPersistentAssemblies();
    }
}