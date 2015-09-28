using SharpMap.UI.Tools;
using SharpMap.UI.Tools.Zooming;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Commands
{
    public class MapFixedZoomOutCommand : MapViewCommand
    {
        protected override void OnExecute(params object[] arguments)
        {
            var activeView = SharpMapGisGuiPlugin.GetFocusedMapView();
            
            IMapTool tool = activeView.MapControl.GetToolByType<FixedZoomOutTool>();
            tool.Execute();

            base.OnExecute(arguments);
        }

        public override bool Checked
        {
            get { return false; }
            set { }
        }
    }
}