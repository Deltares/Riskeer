using DelftTools.Controls;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Commands
{
    public abstract class CanvasCommand : Command
    {
        protected override void OnExecute(params object[] arguments)
        {
        }

        protected ICanvasEditor CanvasEditor
        {
            get { return SharpMapGisGuiPlugin.GetFocusedMapView(); }
        }
    }
}