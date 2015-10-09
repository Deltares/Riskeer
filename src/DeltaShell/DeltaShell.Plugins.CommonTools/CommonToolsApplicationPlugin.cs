using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using DelftTools.Shell.Core;
using DelftTools.Shell.Core.Dao;
using DelftTools.Shell.Gui;
using DelftTools.Utils;
using Mono.Addins;

namespace DeltaShell.Plugins.CommonTools
{
    [Extension(typeof(IPlugin))]
    public class CommonToolsApplicationPlugin : ApplicationPlugin, IDataAccessListenersProvider
    {
        public override string Name
        {
            get
            {
                return "CommonToolsPlugin";
            }
        }

        public override string DisplayName
        {
            get
            {
                return Properties.Resources.CommonToolsApplicationPlugin_DisplayName_Delta_Shell_Common_Tools_Plugin;
            }
        }

        public override string Description
        {
            get
            {
                return Properties.Resources.CommonToolsApplicationPlugin_Description;
            }
        }

        public override string Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public override Image Image
        {
            get
            {
                return new Bitmap(32, 32);
            }
        }

        public override IEnumerable<Assembly> GetPersistentAssemblies()
        {
            yield return typeof(Url).Assembly;
            yield return typeof(GuiContextManager).Assembly;
        }

        public IEnumerable<IDataAccessListener> CreateDataAccessListeners()
        {
            yield return new CommonToolsDataAccessListener();
        }
    }
}