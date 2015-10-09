namespace DeltaShell.Plugins.SharpMapGis.Gui.Commands
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

        protected override void OnExecute(params object[] arguments)
        {
            ZoomHistoryToolMapTool.PreviousZoomState();
            MapView.MapControl.Refresh();

            base.OnExecute(arguments);
        }
    }
}