using System.ComponentModel;
using DelftTools.Shell.Gui;
using DelftTools.Utils.Collections.Generic;
using DeltaShell.Plugins.DemoApplicationPlugin.DataObjects.FailureMechanism;

namespace DeltaShell.Plugins.DemoGuiPlugin.PropertClasses.FailureMechanism
{
    [DisplayName("Assessments")]
    public class AssessmentsProperties : ObjectProperties<IEventedList<IAssessment>>
    {
        [Category("General")]
        [DisplayName("Number of assessments")]
        [Description("The number of assessments")]
        public int NumberOfAssessments
        {
            get { return data.Count; }
        }
    }
}