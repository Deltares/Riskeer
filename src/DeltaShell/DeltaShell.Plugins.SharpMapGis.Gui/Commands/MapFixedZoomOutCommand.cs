using SharpMap.UI.Tools;
using SharpMap.UI.Tools.Zooming;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Commands
{
    public class MapFixedZoomOutCommand : MapViewCommand
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
            var activeView = SharpMapGisGuiPlugin.GetFocusedMapView();

            IMapTool tool = activeView.MapControl.GetToolByType<FixedZoomOutTool>();
            tool.Execute();

            base.OnExecute(arguments);
        }
    }
}