using Core.Common.Gui;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.UI.Tools;
using Core.Plugins.SharpMapGis.Gui.Forms;

namespace Core.Plugins.SharpMapGis.Gui.Commands
{
    public abstract class MapViewCommand : IGuiCommand
    {
        public virtual bool Enabled
        {
            get
            {
                return MapView != null;
            }
        }

        public virtual bool Checked
        {
            get
            {
                return CurrentTool != null && CurrentTool.IsActive;
            }
        }

        public IGui Gui { get; set; }

        public abstract void Execute(params object[] arguments);

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
    }
}