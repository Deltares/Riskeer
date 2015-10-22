using System.Linq;

using DelftTools.Shell.Gui;
using DelftTools.Utils;

using Wti.Data;
using Wti.Forms.Properties;

namespace Wti.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="PipingSurfaceLine"/> for properties panel.
    /// </summary>
    [ResourcesDisplayName(typeof(Resources), "SurfaceLinePropertiesDisplayName")]
    public class PipingSurfaceLineProperties : ObjectProperties<PipingSurfaceLine>
    {
        [ResourcesDisplayName(typeof(Resources), "PipingSurfaceLineNameDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSurfaceLineNameDescription")]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

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