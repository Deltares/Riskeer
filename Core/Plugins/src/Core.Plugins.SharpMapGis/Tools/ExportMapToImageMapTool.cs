using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.SharpMap.UI.Tools;
using Core.Plugins.SharpMapGis.Properties;

namespace Core.Plugins.SharpMapGis.Tools
{
    public class ExportMapToImageMapTool : MapTool
    {
        public override bool AlwaysActive
        {
            get
            {
                return true;
            }
        }

        public IWin32Window Owner { get; set; }

        public override IEnumerable<MapToolContextMenuItem> GetContextMenuItems(ICoordinate worldPosition)
        {
            var name = Resources.ExportMapToImageMapTool_OnBeforeContextMenu_Export_map_as_image;
            var exportAsImageMenuItem = new ToolStripMenuItem(name, Resources.exportIcon, ExportMapEventHandler);
            var zoomToExtentsMenuItem = new ToolStripMenuItem("Zoom to extents", Resources.MapZoomToExtentsImage, (s, e) => Map.ZoomToExtents());

            return new[]
            {
                new MapToolContextMenuItem
                {
                    Priority = 4,
                    MenuItem = zoomToExtentsMenuItem
                },
                new MapToolContextMenuItem
                {
                    Priority = 4,
                    MenuItem = exportAsImageMenuItem
                }
            };
        }

        public override void Execute()
        {
            ExportImageHelper.ExportWithDialog(Owner, MapControl.Image);
        }

        private void ExportMapEventHandler(object sender, EventArgs e)
        {
            Execute();
        }
    }
}