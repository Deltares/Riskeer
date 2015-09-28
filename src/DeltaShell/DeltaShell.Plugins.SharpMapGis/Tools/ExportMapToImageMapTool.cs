using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DelftTools.Controls.Swf;
using DeltaShell.Plugins.SharpMapGis.Properties;
using GeoAPI.Geometries;
using SharpMap.UI.Tools;

namespace DeltaShell.Plugins.SharpMapGis.Tools
{
    public class ExportMapToImageMapTool : MapTool
    {
        public override bool AlwaysActive
        {
            get { return true; }
        }

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

        private void ExportMapEventHandler(object sender, EventArgs e)
        {
            Execute();
        }

        public override void Execute()
        {
            ExportImageHelper.ExportWithDialog(MapControl.Image);
        }
    }
}