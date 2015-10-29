using System;
using System.Windows.Forms;
using Core.Common.Controls.Swf;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Map;
using Core.GIS.SharpMap.Web.Wms;
using Core.Plugins.SharpMapGis.Gui.Forms.MapLegendView;
using log4net;

namespace Core.Plugins.SharpMapGis.Gui.Commands
{
    public class MapAddWmsLayerCommand : MapViewCommand
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MapLegendView));

        public void AddLayerFromExternalSource(string url)
        {
            if (MapView == null)
            {
                return;
            }
            var map = MapView.Data as Map;
            if (map == null)
            {
                return;
            }

            try
            {
                WmsLayer layer = new WmsLayer(url, url)
                {
                    TimeOut = 100000,
                    SpatialReferenceSystem = "EPSG:4326"
                };
                foreach (Client.WmsServerLayer childLayer in layer.RootLayer.ChildLayers)
                {
                    // no visible by default
                    // layer.AddLayer(childLayer.Name);
                }

                layer.SetImageFormat(layer.OutputFormats[0]);
                map.Layers.Add(layer);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
        }

        protected override void OnExecute(params object[] arguments)
        {
            var openUrlDialog = new OpenUrlDialog
            {
                Url = "http://www2.demis.nl/wms/wms.asp?wms=WorldMap&REQUEST=GetCapabilities"
            };

            if (openUrlDialog.ShowDialog() == DialogResult.OK)
            {
                string url = openUrlDialog.Url;

                AddLayerFromExternalSource(url);
            }
        }
    }
}