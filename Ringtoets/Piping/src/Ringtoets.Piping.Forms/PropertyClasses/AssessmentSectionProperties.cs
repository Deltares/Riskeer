using Core.Common.Gui;
using Core.Common.Utils;
using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="AssessmentSection"/> for properties panel.
    /// </summary>
    [ResourcesDisplayName(typeof(Resources), "AssessmentSectionProperties_DisplayName")]
    public class AssessmentSectionProperties : ObjectProperties<AssessmentSection>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "AssessmentSection_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "AssessmentSection_Name_Description")]
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