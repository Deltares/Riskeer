using System;
using System.Windows.Forms;
using DelftTools.Controls.Swf;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms.MapLegendView;
using log4net;
using SharpMap;
using SharpMap.Layers;
using SharpMap.Web.Wms;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Commands
{
    public class MapAddWmsLayerCommand : MapViewCommand
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MapLegendView));

        protected override void OnExecute(params object[] arguments)
        {
            var openUrlDialog = new OpenUrlDialog();
            openUrlDialog.Url = "http://www2.demis.nl/wms/wms.asp?wms=WorldMap&REQUEST=GetCapabilities";

            if (openUrlDialog.ShowDialog() == DialogResult.OK)
            {
                string url = openUrlDialog.Url;
                
                AddLayerFromExternalSource(url);
            }
        }

        public void AddLayerFromExternalSource(string url)
        {
            if (MapView == null) return;
            var map = MapView.Data as Map;
            if (map == null) return;

            try
            {
                WmsLayer layer = new WmsLayer(url, url);
                layer.TimeOut = 100000;
                layer.SpatialReferenceSystem = "EPSG:4326";
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
    }
}