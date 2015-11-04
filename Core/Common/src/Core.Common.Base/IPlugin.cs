using System.Resources;

namespace Core.Common.Base
{
    public interface IPlugin
    {
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