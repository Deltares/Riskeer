using System.Resources;

namespace Core.Common.Base
{
    public interface IPlugin
    {
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
    }
}