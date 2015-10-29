using Core.Common.Controls;

namespace Core.Plugins.SharpMapGis.Gui.Commands
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