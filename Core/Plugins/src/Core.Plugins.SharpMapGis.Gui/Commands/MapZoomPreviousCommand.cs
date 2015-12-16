namespace Core.Plugins.SharpMapGis.Gui.Commands
{
    public class MapZoomPreviousCommand : MapZoomHistoryCommand
    {
        public override bool Enabled
        {
            get
            {
                if (MapView == null)
                {
                    return false;
                }

                return ZoomHistoryToolMapTool.UndoCount > 0;
            }
        }

        public override void Execute(params object[] arguments)
        {
            ZoomHistoryToolMapTool.PreviousZoomState();
            MapView.MapControl.Refresh();
        }
    }
}