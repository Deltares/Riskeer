using Core.Common.Gui;
using Core.Common.Gui.Attributes;
using Core.Common.Utils.Attributes;

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
        [DynamicReadOnly]
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
        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadonlyValidator(string propertyName)
        {
            return !data.WrappedData.IsNameEditable;
        }
    }
}