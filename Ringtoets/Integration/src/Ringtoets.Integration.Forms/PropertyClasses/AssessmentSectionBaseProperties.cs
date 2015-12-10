using Core.Common.Gui;
using Core.Common.Utils.Attributes;

using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Properties;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="DikeAssessmentSection"/> for properties panel.
    /// </summary>
    public class AssessmentSectionBaseProperties : ObjectProperties<AssessmentSectionBase>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "AssessmentSectionBase_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "AssessmentSectionBase_Name_Description")]
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