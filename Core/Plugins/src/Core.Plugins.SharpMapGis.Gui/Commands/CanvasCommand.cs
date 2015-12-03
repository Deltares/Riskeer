using Core.Common.Controls;
using Core.Plugins.SharpMapGis.Gui.Forms;

namespace Core.Plugins.SharpMapGis.Gui.Commands
{
    public abstract class CanvasCommand : Command
    {
        protected MapView CanvasEditor
        {
            get
            {
                return SharpMapGisGuiPlugin.GetFocusedMapView();
            }
        }

        protected override void OnExecute(params object[] arguments) {}
    }
}