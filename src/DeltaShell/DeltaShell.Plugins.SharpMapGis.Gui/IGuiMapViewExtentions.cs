using System.Collections.Generic;
using System.Linq;
using DelftTools.Controls;
using DelftTools.Shell.Gui;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms;

namespace DeltaShell.Plugins.SharpMapGis.Gui
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

            var compositeView = view as ICompositeView;
            if (compositeView != null)
            {
                foreach (var childView in compositeView.ChildViews)
                {
                    foreach (var compositeMapView in GetMapViews(childView))
                    {
                        yield return compositeMapView;
                    }
                }
            }
        }
    }
}