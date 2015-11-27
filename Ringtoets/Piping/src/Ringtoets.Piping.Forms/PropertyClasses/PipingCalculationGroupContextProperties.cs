using Core.Common.Gui;
using Core.Common.Utils;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// Object properties class for <see cref="PipingCalculationGroup"/>
    /// </summary>
    [ResourcesDisplayName(typeof(Resources), "PipingCalculationGroupContextProperties_DisplayName")]
    public class PipingCalculationGroupContextProperties : ObjectProperties<PipingCalculationGroupContext>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingCalculationGroup_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingCalculationGroup_Name_Description")]
        public string Name
        {
            get
            {
                return data.WrappedData.Name;
            }
            set
            {
                data.WrappedData.Name = value;
                data.NotifyObservers();
            }
        }
    }
}