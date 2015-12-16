namespace Core.Plugins.SharpMapGis.Gui.Commands
{
    public class MapZoomNextCommand : MapZoomHistoryCommand
    {
        public override bool Enabled
        {
            get
            {
                if (MapView == null)
                {
                    return false;
                }

                return ZoomHistoryToolMapTool.RedoCount > 0;
            }
        }

        public override void Execute(params object[] arguments)
        {
            ZoomHistoryToolMapTool.NextZoomState();
            MapView.MapControl.Refresh();
        }
    }
}