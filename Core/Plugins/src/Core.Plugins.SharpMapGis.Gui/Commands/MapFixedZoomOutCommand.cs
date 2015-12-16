using Core.GIS.SharpMap.UI.Tools;
using Core.GIS.SharpMap.UI.Tools.Zooming;

namespace Core.Plugins.SharpMapGis.Gui.Commands
{
    public class MapFixedZoomOutCommand : MapViewCommand
    {
        public override bool Checked
        {
            get
            {
                return false;
            }
        }

        public override void Execute(params object[] arguments)
        {
            var activeView = SharpMapGisGuiPlugin.GetFocusedMapView();

            IMapTool tool = activeView.MapControl.GetToolByType<FixedZoomOutTool>();
            tool.Execute();
        }
    }
}