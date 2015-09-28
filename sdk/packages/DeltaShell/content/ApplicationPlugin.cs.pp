using DelftTools.Shell.Core;
using Mono.Addins;
 
namespace $rootnamespace$ 
{
    [Extension(typeof(IPlugin))]
    public class ApplicationPlugin : DelftTools.Shell.Core.ApplicationPlugin
    {
        public override string Name
        {
            get { return "<Type name of your plugin here>"; }
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