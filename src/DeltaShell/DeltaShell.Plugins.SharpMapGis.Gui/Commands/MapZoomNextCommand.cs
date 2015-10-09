namespace DeltaShell.Plugins.SharpMapGis.Gui.Commands
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

        protected override void OnExecute(params object[] arguments)
        {
            ZoomHistoryToolMapTool.NextZoomState();
            MapView.MapControl.Refresh();

            base.OnExecute(arguments);
        }
    }
}