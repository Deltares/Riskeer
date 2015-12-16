using System.Linq;
using Core.Plugins.SharpMapGis.Tools;

namespace Core.Plugins.SharpMapGis.Gui.Commands
{
    public class ExportMapAsImageCommand : MapViewCommand
    {
        public override void Execute(params object[] arguments)
        {
            var exportMapTool = MapView.MapControl.Tools.First(tool => tool is ExportMapToImageMapTool);
            exportMapTool.Execute();
        }
    }
}