using DeltaShell.Plugins.SharpMapGis.Gui.Forms;
using SharpMap.UI.Tools;
using SharpMap.UI.Tools.Zooming;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Commands
{
    public class MapFixedZoomInCommand : MapViewCommand
    {
        public override bool Checked
        {
            get
            {
                return false;
            }
            set {}
        }

        protected override void OnExecute(params object[] arguments)
        {
            MapView activeView = SharpMapGisGuiPlugin.GetFocusedMapView();

            IMapTool tool = activeView.MapControl.GetToolByType<FixedZoomInTool>();
            tool.Execute();

            base.OnExecute(arguments);
        }
    }
}