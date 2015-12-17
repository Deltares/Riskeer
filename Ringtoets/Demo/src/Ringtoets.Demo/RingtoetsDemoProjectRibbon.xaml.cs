using System.Collections.Generic;
using System.Windows;

using Core.Common.Controls;
using Core.Common.Controls.Commands;
using Core.Common.Gui.Forms;

using Ringtoets.Demo.Commands;

using DemoResources = Ringtoets.Demo.Properties.Resources;

namespace Ringtoets.Demo
{
    /// <summary>
    /// Interaction logic for RingtoetsDemoProjectRibbon.xaml
    /// </summary>
    public partial class RingtoetsDemoProjectRibbon : IRibbonCommandHandler
    {
        private readonly ICommand addNewDikeAssessmentSection, addNewDuneAssessmentSection;

        public RingtoetsDemoProjectRibbon()
        {
            InitializeComponent();

            addNewDikeAssessmentSection = new AddNewDemoDikeAssessmentSectionCommand();
            addNewDuneAssessmentSection = new AddNewDemoDuneAssessmentSectionCommand();
        }

        public IEnumerable<ICommand> Commands
        {
            get
            {
                yield return addNewDikeAssessmentSection;
                yield return addNewDuneAssessmentSection;
            }
        }

        public object GetRibbonControl()
        {
            return RingtoetsDemoProjectRibbonControl;
        }

        public void ValidateItems()
        {
            
        }

        public bool IsContextualTabVisible(string tabGroupName, string tabName)
        {
            return false;
        }

        private void AddNewDemoDikeAssessmentSectionButton_Click(object sender, RoutedEventArgs e)
        {
            addNewDikeAssessmentSection.Execute();
        }

        private void AddNewDemoDuneAssessmentSectionButton_Click(object sender, RoutedEventArgs e)
        {
            addNewDuneAssessmentSection.Execute();
        }
    }
}
