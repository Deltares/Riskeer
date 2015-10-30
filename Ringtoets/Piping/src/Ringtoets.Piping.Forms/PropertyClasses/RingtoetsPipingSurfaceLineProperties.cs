using System.Linq;
using Core.Common.Gui;
using Core.Common.Utils;
using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.Properties;
using TypeConverter = System.ComponentModel.TypeConverterAttribute;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="RingtoetsPipingSurfaceLine"/> for properties panel.
    /// </summary>
    [ResourcesDisplayName(typeof(Resources), "RingtoetsPipingSurfaceLine_DisplayName")]
    public class RingtoetsPipingSurfaceLineProperties : ObjectProperties<RingtoetsPipingSurfaceLine>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "RingtoetsPipingSurfaceLine_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "RingtoetsPipingSurfaceLine_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "RingtoetsPipingSurfaceLine_Points_DisplayName")]
        [ResourcesDescription(typeof(Resources), "RingtoetsPipingSurfaceLine_Points_Description")]
        public Point3D[] Points
        {
            get
            {
                return data.Points.ToArray();
            }
        }
    }
}