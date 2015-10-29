using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using Core.Common.Gui;
using Core.Common.Utils;
using Core.Gis.GeoApi.CoordinateSystems;
using Core.GIS.SharpMap.CoordinateSystems.Transformations;
using Core.GIS.SharpMap.Map;
using Core.GIS.SharpMap.UI.Forms;
using Core.Plugins.SharpMapGis.Gui.Properties;
using log4net;
using MessageBox = Core.Common.Controls.Swf.MessageBox;

namespace Core.Plugins.SharpMapGis.Gui.Forms.GridProperties
{
    [ResourcesDisplayName(typeof(Resources), "MapProperties_Map_DisplayName")]
    public class MapProperties : ObjectProperties<Map>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MapProperties));

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "Common_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "MapProperties_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
            set
            {
                data.Name = value;
            }
        }

        [Category("Coordinates")]
        [DisplayName("Coordinate system")]
        [Description("Coordinate system (geographic or projected) used for drawing.")]
        [TypeConverter(typeof(CoordinateSystemStringTypeConverter))]
        [Editor(typeof(CoordinateSystemTypeEditor), typeof(UITypeEditor))]
        public ICoordinateSystem CoordinateSystem
        {
            get
            {
                return data.CoordinateSystem;
            }
            set
            {
                try
                {
                    data.CoordinateSystem = value;
                    data.ZoomToExtents();
                }
                catch (CoordinateTransformException e)
                {
                    MessageBox.Show("Cannot convert map to coordinate system: " + e.Message,
                                    "Coordinate transformation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        [Category("Coordinates")]
        [DisplayName("Show grid")]
        [Description("Shows or hides latitude / longitude grid.")]
        public bool ShowGrid
        {
            get
            {
                return data.ShowGrid;
            }
            set
            {
                if (data.CoordinateSystem == null)
                {
                    Log.DebugFormat("Showing latitude / longitude grid is supported only when map coordinate system is non-empty");

                    return;
                }

                data.ShowGrid = value;
            }
        }
    }
}