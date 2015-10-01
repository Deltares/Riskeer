using DelftTools.Shell.Gui;
using DelftTools.Utils;
using Wti.Data;
using Wti.Forms.Properties;

namespace Wti.Forms.PropertyClasses
{
    [ResourcesDisplayName(typeof(Resources), "PipingDataPropertiesDisplayName")]
    public class PipingDataProperties : ObjectProperties<PipingData>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataAssessmentLevelDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataAssessmentLevelDescription")]
        public double AssessmentLevel
        {
            get
            {
                return data.AssessmentLevel;
            }
            set
            {
                data.AssessmentLevel = value;
                data.NotifyObservers();
            }
        }
    }
}