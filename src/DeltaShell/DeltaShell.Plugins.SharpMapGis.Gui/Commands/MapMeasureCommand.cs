using SharpMap.UI.Tools;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Commands
{
    public class MapMeasureCommand : MapViewCommand
    {
        protected override void OnExecute(object[] arguments)
        {
            var tool = CurrentTool;

            if (tool != null)
            {
                if (tool.IsActive)
                {
                    tool.IsActive = false;
                }
                else
                {
                    MapView.MapControl.ActivateTool(tool);
                }

            }

            base.OnExecute(arguments);
        }

        protected override IMapTool CurrentTool
        {
            get
            {
                if (MapView == null) return null;
                return MapView.MapControl.GetToolByType<MeasureTool>(); 
            }
        }
    }
}