using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using DelftTools.Shell.Gui;
using DelftTools.Utils;
using DeltaShell.Plugins.SharpMapGis.Gui.Properties;
using GeoAPI.CoordinateSystems;
using log4net;
using SharpMap;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.UI.Forms;
using MessageBox = DelftTools.Controls.Swf.MessageBox;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms.GridProperties
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