using System.Linq;
using DeltaShell.Plugins.SharpMapGis.Tools;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Commands
{
    public class ExportMapAsImageCommand : MapViewCommand
    {
        protected override void OnExecute(params object[] arguments)
        {
            var exportMapTool = MapView.MapControl.Tools.First(tool => tool is ExportMapToImageMapTool);
            exportMapTool.Execute();
        }
    }
}