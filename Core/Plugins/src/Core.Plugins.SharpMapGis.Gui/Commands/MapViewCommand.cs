using Core.Common.Controls;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.UI.Tools;
using Core.Plugins.SharpMapGis.Gui.Forms;

namespace Core.Plugins.SharpMapGis.Gui.Commands
{
    public abstract class MapViewCommand : Command
    {
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
            set
            {
                base.Checked = value;
            }
        }

        protected MapView MapView
        {
            get
            {
                return SharpMapGisGuiPlugin.GetFocusedMapView();
            }
        }

        protected IMap Map
        {
            get
            {
                return MapView != null ? MapView.Map : null;
            }
        }

        protected virtual IMapTool CurrentTool
        {
            get
            {
                return null;
            }
        }

        protected override void OnExecute(params object[] arguments) {}
    }
}