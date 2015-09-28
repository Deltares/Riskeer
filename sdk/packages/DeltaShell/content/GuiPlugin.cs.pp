using DelftTools.Shell.Core;
using Mono.Addins;
 
namespace $rootnamespace$ 
{
    [Extension(typeof(IPlugin))]
    public class GuiPlugin : DelftTools.Shell.Gui.GuiPlugin
    {
        public override string Name
        {
            get { return "<Type name of your gui plugin here>"; }
        }
 
        public override string Description
        {
            get { return "<Type short description of plugin>"; }
        }
 
        public override string Version
        {
            get { return "1.0"; }
        }
    }
}