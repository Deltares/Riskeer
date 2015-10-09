using SharpMap.UI.Tools.Zooming;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Commands
{
    public abstract class MapZoomHistoryCommand : MapViewCommand
    {
        public override bool Checked
        {
            get
            {
                return false;
            }
            set {}
        }

        public ZoomHistoryTool ZoomHistoryToolMapTool
        {
            get
            {
                return MapView.MapControl.GetToolByType<ZoomHistoryTool>();
            }
        }
    }
}