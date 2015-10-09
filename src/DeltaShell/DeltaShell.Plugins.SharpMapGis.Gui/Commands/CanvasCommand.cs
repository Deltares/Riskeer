using DelftTools.Controls;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Commands
{
    public abstract class CanvasCommand : Command
    {
        protected ICanvasEditor CanvasEditor
        {
            get
            {
                return SharpMapGisGuiPlugin.GetFocusedMapView();
            }
        }

        protected override void OnExecute(params object[] arguments) {}
    }
}