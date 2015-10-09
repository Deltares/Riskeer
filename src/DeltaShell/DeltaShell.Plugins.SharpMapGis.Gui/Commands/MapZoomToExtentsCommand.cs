using DeltaShell.Plugins.SharpMapGis.Gui.Forms;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Commands
{
    public class MapZoomToExtentsCommand : MapViewCommand
    {
        public override bool Checked
        {
            get
            {
                return false;
            }
            set {}
        }

        /// <summary>
        /// if arguments is null the active mapview will zoom to it's default extents.
        /// Otherwise the first arguments will be assumed to be a mapview and the active view will zoom to that map extents
        /// </summary>
        /// <param name="arguments"></param>
        protected override void OnExecute(params object[] arguments)
        {
            MapView targetView = null;
            if (arguments.Length == 1) //target extent
            {
                targetView = arguments[0] as MapView;
            }

            MapView activeView = SharpMapGisGuiPlugin.GetFocusedMapView();

            if (targetView != null)
            {
                activeView.Map.ZoomToFit(targetView.Map.Envelope);
            }
            else
            {
                activeView.Map.ZoomToExtents();
            }
            activeView.MapControl.Refresh();
        }
    }
}