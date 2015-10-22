using System.Linq;

using DelftTools.Shell.Gui;
using DelftTools.Utils;

using Wti.Data;
using Wti.Forms.Properties;
using TypeConverter = System.ComponentModel.TypeConverterAttribute;

namespace Wti.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="RingtoetsPipingSurfaceLine"/> for properties panel.
    /// </summary>
    [ResourcesDisplayName(typeof(Resources), "SurfaceLinePropertiesDisplayName")]
    public class PipingSurfaceLineProperties : ObjectProperties<RingtoetsPipingSurfaceLine>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingSurfaceLineNameDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSurfaceLineNameDescription")]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingSurfaceLinePointsDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSurfaceLinePointsDescription")]
        public Point3D[] Points
        {
            get
            {
                return data.Points.ToArray();
            }
        }
    }
}