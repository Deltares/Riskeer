using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.SharpMap.Api;
using Core.Plugins.SharpMapGis.Gui.Forms;

namespace Core.Plugins.SharpMapGis.Gui
{
    public class GisGuiService : IGisGuiService
    {
        private readonly GuiPlugin guiPlugin;

        public GisGuiService(GuiPlugin guiPlugin)
        {
            this.guiPlugin = guiPlugin;
        }

        /// <summary>
        /// TODO: this should happen automatically, probably we should add proxy around map which will also take care about INotifyPropertyChanged, hoever it should also work with proxy from NHibernate!
        /// </summary>
        /// <param name="map"></param>
        public void RefreshMapView(IMap map)
        {
            IList<IView> mapVews = guiPlugin.Gui.DocumentViewsResolver.GetViewsForData(map);
            foreach (IView view in mapVews)
            {
                var mapView = view as MapView;
                if (mapView != null)
                {
                    Cursor currentCursor = mapView.MapControl.Cursor;
                    mapView.MapControl.Cursor = Cursors.WaitCursor;
                    mapView.MapControl.Refresh();
                    mapView.MapControl.Cursor = currentCursor;
                }
            }
        }

        public void ZoomCurrentMapToEnvelope(IEnvelope envelope)
        {
            var mapView = guiPlugin.Gui.DocumentViews.ActiveView as MapView;
            if (mapView == null)
            {
                return;
            }

            mapView.Map.ZoomToFit(envelope);
            mapView.Refresh();
        }
    }
}