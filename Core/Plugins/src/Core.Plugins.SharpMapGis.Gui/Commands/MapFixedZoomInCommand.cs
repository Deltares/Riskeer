using Core.GIS.SharpMap.UI.Tools;
using Core.GIS.SharpMap.UI.Tools.Zooming;
using Core.Plugins.SharpMapGis.Gui.Forms;

namespace Core.Plugins.SharpMapGis.Gui.Commands
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