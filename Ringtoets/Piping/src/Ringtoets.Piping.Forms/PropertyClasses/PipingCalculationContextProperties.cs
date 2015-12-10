using Core.Common.Gui;
using Core.Common.Utils.Attributes;

using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    [ResourcesDisplayName(typeof(Resources), "PipingCalculationContextProperties_DisplayName")]
    public class PipingCalculationContextProperties : ObjectProperties<PipingCalculationContext>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingCalculation_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingCalculation_Name_Description")]
        public string Name
        {
            get
            {
                return data.WrappedData.Name;
            }
            set
            {
                data.WrappedData.Name = value;
                data.WrappedData.NotifyObservers();
            }
        }
    }
}