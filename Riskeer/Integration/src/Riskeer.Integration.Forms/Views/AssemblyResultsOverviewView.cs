using System.Windows.Forms;
using Core.Common.Controls.Views;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.Controls;

namespace Riskeer.Integration.Forms.Views
{
    public partial class AssemblyResultsOverviewView : UserControl, IView
    {
        public AssemblyResultsOverviewView(AssessmentSection assessmentSection)
        {
            AssessmentSection = assessmentSection;
            InitializeComponent();

            wpfElementHost.Child = new AssemblyOverviewControl(assessmentSection);
        }

        public AssessmentSection AssessmentSection { get; }
        
        public object Data { get; set; }
    }
}