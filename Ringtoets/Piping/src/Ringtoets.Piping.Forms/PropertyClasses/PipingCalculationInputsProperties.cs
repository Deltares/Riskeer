using Core.Common.Gui;
using Core.Common.Utils;

using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    [ResourcesDisplayName(typeof(Resources), "PipingCalculationInputsProperties_DisplayName")]
    public class PipingCalculationInputsProperties : ObjectProperties<PipingCalculationInputs>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingCalculationData_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingCalculationData_Name_Description")]
        public string Name
        {
            get
            {
                return data.PipingData.Name;
            }
            set
            {
                data.PipingData.Name = value;
                data.PipingData.NotifyObservers();
            }
        }
    }
}