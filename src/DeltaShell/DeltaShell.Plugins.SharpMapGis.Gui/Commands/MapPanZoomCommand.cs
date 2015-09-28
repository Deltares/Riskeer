using DeltaShell.Plugins.SharpMapGis.Gui.Forms;
using SharpMap.UI.Tools;
using SharpMap.UI.Tools.Zooming;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Commands
{
    public class MapPanZoomCommand : MapViewCommand
    {
        protected override void OnExecute(params object[] arguments)
        {
            MapView activeView = SharpMapGisGuiPlugin.GetFocusedMapView();
            activeView.MapControl.ActivateTool(CurrentTool);
            base.OnExecute(arguments);
        }
        protected override IMapTool CurrentTool
        {
            get 
            {
                return null != MapView ? MapView.MapControl.GetToolByType<PanZoomTool>() : null;
            }
        }
    }
}