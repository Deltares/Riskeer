using Core.Common.Gui;
using Core.Common.Utils;

using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    [ResourcesDisplayName(typeof(Resources), "PipingCalculationContextProperties_DisplayName")]
    public class PipingCalculationContextProperties : ObjectProperties<PipingCalculationContext>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingCalculationData_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingCalculationData_Name_Description")]
        public string Name
        {
            get
            {
                return data.WrappedPipingCalculation.Name;
            }
            set
            {
                data.WrappedPipingCalculation.Name = value;
                data.WrappedPipingCalculation.NotifyObservers();
            }
        }
    }
}