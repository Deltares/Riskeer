using SharpMap.UI.Tools;
using SharpMap.UI.Tools.Zooming;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Commands
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

        protected override void OnExecute(params object[] arguments)
        {
            var activeView = SharpMapGisGuiPlugin.GetFocusedMapView();
            activeView.MapControl.ActivateTool(CurrentTool);
            base.OnExecute(arguments);
        }
    }
}