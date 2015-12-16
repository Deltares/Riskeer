using System.Collections.Generic;
using System.Linq;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Plugins.SharpMapGis.Gui.Forms;

namespace Core.Plugins.SharpMapGis.Gui
{
    public static class GuiMapViewExtentions
    {
        public static MapView GetFocusedMapView(this IGui gui, IView view = null)
        {
            if (gui == null || gui.DocumentViews == null)
            {
                return null;
            }

            var viewToSearch = view ?? gui.DocumentViews.ActiveView;
            return GetMapViews(viewToSearch).FirstOrDefault();
        }

        private static IEnumerable<MapView> GetMapViews(IView view)
        {
            var mapView = view as MapView;
            if (mapView != null)
            {
                yield return mapView;
            }
        }
    }
}