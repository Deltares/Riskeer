using Core.GIS.SharpMap.UI.Tools;

namespace Core.Plugins.SharpMapGis.Gui.Commands
{
    public class MapMeasureCommand : MapViewCommand
    {
        protected override IMapTool CurrentTool
        {
            get
            {
                if (MapView == null)
                {
                    return null;
                }
                return MapView.MapControl.GetToolByType<MeasureTool>();
            }
        }

        public override void Execute(object[] arguments)
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
        }
    }
}