using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using Core.Common.Gui;
using Core.Common.Utils;
using Core.GIS.GeoAPI.CoordinateSystems;
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

        [ResourcesCategory(typeof(Resources), "Categories_Coordinates")]
        [ResourcesDisplayName(typeof(Resources), "MapProperties_CoordinateSystem_DisplayName")]
        [ResourcesDescription(typeof(Resources), "MapProperties_CoordinateSystem_Description")]
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
                    var message = string.Format(Resources.MapProperties_CoordinateSystem_Cannot_convert_map_to_coordinate_system___0_, e.Message);
                    MessageBox.Show(message,
                                    Resources.MapProperties_CoordinateSystem_Coordinate_transformation_error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Coordinates")]
        [ResourcesDisplayName(typeof(Resources), "MapProperties_ShowGrid_DisplayName")]
        [ResourcesDescription(typeof(Resources), "MapProperties_ShowGrid_Description")]
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
                    Log.DebugFormat(Resources.MapProperties_ShowGrid_Showing_latitude___longitude_grid_is_supported_only_when_map_coordinate_system_is_non_empty);

                    return;
                }

                data.ShowGrid = value;
            }
        }
    }
}