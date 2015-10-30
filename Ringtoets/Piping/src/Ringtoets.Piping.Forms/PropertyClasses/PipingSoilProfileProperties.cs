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
    [ResourcesDisplayName(typeof(Resources), "PipingSoilProfileProperties_DisplayName")]
    public class PipingSoilProfileProperties : ObjectProperties<PipingSoilProfile>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingSoilProfile_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSoilProfile_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingSoilProfile_Tops_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSoilProfile_Tops_Description")]
        public double[] TopLevels
        {
            get
            {
                return data.Layers.Select(l => l.Top).ToArray();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingSoilProfile_Bottom_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSoilProfile_Bottom_Description")]
        public double Bottom
        {
            get
            {
                return data.Bottom;
            }
        }
    }
}