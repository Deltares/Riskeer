using System.ComponentModel;
using DelftTools.Shell.Gui;
using DeltaShell.Plugins.DemoApplicationPlugin.DataObjects.FailureMechanism;

namespace DeltaShell.Plugins.DemoGuiPlugin.PropertClasses.FailureMechanism
{
    [DisplayName("Assessment")]
    public class AssessmentProperties : ObjectProperties<IAssessment>
    {
        [Category("General")]
        [DisplayName("Name")]
        [Description("The name of the assessment")]
        public string Name
        {
            get { return data.Name; }
            set { data.Name = value; }
        }
    }
}