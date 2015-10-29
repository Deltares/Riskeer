using System.Linq;
using Core.Common.Gui;
using Core.Common.Utils;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using TypeConverter = System.ComponentModel.TypeConverterAttribute;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="PipingSoilProfile"/> for properties panel.
    /// </summary>
    [ResourcesDisplayName(typeof(Resources), "PipingSoilProfilePropertiesDisplayName")]
    public class PipingSoilProfileProperties : ObjectProperties<PipingSoilProfile>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingSoilProfileNameDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSoilProfileNameDescription")]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingSoilProfileTopsDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSoilProfileTopsDescription")]
        public double[] TopLevels
        {
            get
            {
                return data.Layers.Select(l => l.Top).ToArray();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingSoilProfileBottomDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSoilProfileBottomDescription")]
        public double Bottom
        {
            get
            {
                return data.Bottom;
            }
        }
    }
}