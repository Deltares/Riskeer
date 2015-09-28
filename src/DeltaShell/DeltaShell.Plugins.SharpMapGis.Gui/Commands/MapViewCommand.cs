using DelftTools.Controls;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms;
using SharpMap.Api;
using SharpMap.UI.Tools;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Commands
{
    public abstract class MapViewCommand : Command
    {
        protected override void OnExecute(params object[] arguments)
        {
        }

        protected MapView MapView
        {
            get { return SharpMapGisGuiPlugin.GetFocusedMapView(); }
        }

        protected IMap Map
        {
            get { return MapView != null ? MapView.Map : null; }
        }

        protected virtual IMapTool CurrentTool
        {
            get { return null; }
        }

        public override bool Enabled
        {
            get
            {
                return MapView != null;
            }
        }

        public override bool Checked
        {
            get
            {
                return CurrentTool != null && CurrentTool.IsActive;
            }
            set { base.Checked = value; }
        }
    }
}