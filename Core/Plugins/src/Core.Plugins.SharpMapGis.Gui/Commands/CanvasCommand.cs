using Core.Common.Controls.Commands;
using Core.Plugins.SharpMapGis.Gui.Forms;

namespace Core.Plugins.SharpMapGis.Gui.Commands
{
    public abstract class CanvasCommand : ICommand
    {
        protected MapView CanvasEditor
        {
            get
            {
                return SharpMapGisGuiPlugin.GetFocusedMapView();
            }
        }

        public abstract bool Enabled { get; }

        public abstract bool Checked { get; }

        public abstract void Execute(params object[] arguments);
    }
}