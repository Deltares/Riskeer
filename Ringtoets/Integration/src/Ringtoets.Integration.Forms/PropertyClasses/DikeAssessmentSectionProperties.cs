using Core.Common.Gui;
using Core.Common.Utils;

using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Properties;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="DikeAssessmentSection"/> for properties panel.
    /// </summary>
    [ResourcesDisplayName(typeof(Resources), "DikeAssessmentSection_DisplayName")]
    public class DikeAssessmentSectionProperties : ObjectProperties<DikeAssessmentSection>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "DikeAssessmentSection_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DikeAssessmentSection_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
            set
            {
                data.Name = value;
                data.NotifyObservers();
            }
        }
    }
}