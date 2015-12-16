using Core.GIS.SharpMap.UI.Tools;
using Core.GIS.SharpMap.UI.Tools.Zooming;

namespace Core.Plugins.SharpMapGis.Gui.Commands
{
    public class MapZoomInUsingRectangleCommand : MapViewCommand
    {
        protected override IMapTool CurrentTool
        {
            get
            {
                return MapView != null ? MapView.MapControl.GetToolByType<ZoomUsingRectangleTool>() : null;
            }
        }

        public override void Execute(params object[] arguments)
        {
            var activeView = SharpMapGisGuiPlugin.GetFocusedMapView();
            activeView.MapControl.ActivateTool(CurrentTool);
        }
    }
}