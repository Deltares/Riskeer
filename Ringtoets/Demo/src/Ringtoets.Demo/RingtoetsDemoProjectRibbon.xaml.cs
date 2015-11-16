using System.Collections.Generic;

using Core.Common.Controls;
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
        private readonly ICommand addNewDemoProject;

        public RingtoetsDemoProjectRibbon()
        {
            InitializeComponent();

            addNewDemoProject = new AddNewDemoAssessmentSectionCommand();
        }

        public IEnumerable<ICommand> Commands
        {
            get
            {
                yield return addNewDemoProject;
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

        private void AddNewRingtoetsDemoProjectButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            addNewDemoProject.Execute();
        }
    }
}
