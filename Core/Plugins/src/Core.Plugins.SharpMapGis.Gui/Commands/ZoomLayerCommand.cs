using System.Linq;
using Core.Common.Gui;
using Core.Common.Utils.Collections;
using Core.Gis.GeoApi.Geometries;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.Api.Layers;
using Core.Plugins.SharpMapGis.Gui.Forms;

namespace Core.Plugins.SharpMapGis.Gui.Commands
{
    public class ZoomLayerCommand : MapViewCommand, IGuiCommand
    {
        public IGui Gui { get; set; }

        protected override void OnExecute(params object[] arguments)
        {
            // parameter 0 must be a layer
            ILayer layer = (arguments[0] is ILayer) ? arguments[0] as ILayer : null;
            if (layer == null)
            {
                return;
            }

            var manager = Gui.DocumentViews;

            // find the correct mapview
            var mapView = manager.GetActiveViews<MapView>().FirstOrDefault(v => Equals(v.Data, layer.Map));

            if (mapView == null)
            {
                return;
            }

            var envelope = new Envelope();
            GIS.SharpMap.Map.Map.GetLayers(new[]
            {
                layer
            }, false, false).ForEach(l => envelope.ExpandToInclude((IEnvelope) l.Envelope));

            if (envelope.IsNull)
            {
                return;
            }

            mapView.Map.ZoomToFit(envelope, true);
            mapView.MapControl.Refresh();
        }
    }
}