using System.Drawing;
using System.Reflection;
using Core.Common.Base;

namespace Core.Plugins.CommonTools
{
    /// <summary>
    /// Common tool plugins
    /// </summary>
    public class CommonToolsApplicationPlugin : ApplicationPlugin
    {
        /// <summary>
        /// Gets the name of the plugin
        /// Derived from <see cref="ApplicationPlugin.Name">ApplicationPlugin.Name</see>
        /// </summary>
        public override string Name
        {
            get
            {
                return "CommonToolsPlugin";
            }
        }

        /// <summary>
        /// Gets the display name from the <see cref="Properties.Resources.CommonToolsApplicationPlugin_DisplayName_Delta_Shell_Common_Tools_Plugin">resource</see>
        /// Derived from <see cref="ApplicationPlugin.DisplayName">ApplicationPlugin.DisplayName</see>
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return Properties.Resources.CommonToolsApplicationPlugin_DisplayName_Delta_Shell_Common_Tools_Plugin;
            }
        }

        /// <summary>
        /// Gets the description from the <see cref="Properties.Resources.CommonToolsApplicationPlugin_Description">resource</see>
        /// Derived from <see cref="ApplicationPlugin.Description">ApplicationPlugin.Description</see>
        /// </summary>
        public override string Description
        {
            get
            {
                return Properties.Resources.CommonToolsApplicationPlugin_Description;
            }
        }

        /// <summary>
        /// Gets the version from the assembly.
        /// Derived from <see cref="ApplicationPlugin.Version">ApplicationPlugin.Version</see>
        /// </summary>
        public override string Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        /// <summary>
        /// Image for displaying in gui. Default format 32x32 bitmap. 
        /// Derived from <see cref="ApplicationPlugin.Image">ApplicationPlugin.Image</see>
        /// </summary>
        public override Image Image
        {
            get
            {
                return new Bitmap(32, 32);
            }
        }
    }
}