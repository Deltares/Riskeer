using Core.GIS.SharpMap.UI.Tools;
using Core.GIS.SharpMap.UI.Tools.Zooming;
using Core.Plugins.SharpMapGis.Gui.Forms;

namespace Core.Plugins.SharpMapGis.Gui.Commands
{
    public class MapPanZoomCommand : MapViewCommand
    {
        protected override IMapTool CurrentTool
        {
            get
            {
                return null != MapView ? MapView.MapControl.GetToolByType<PanZoomTool>() : null;
            }
        }

        public override void Execute(params object[] arguments)
        {
            MapView activeView = SharpMapGisGuiPlugin.GetFocusedMapView();
            activeView.MapControl.ActivateTool(CurrentTool);
        }
    }
}