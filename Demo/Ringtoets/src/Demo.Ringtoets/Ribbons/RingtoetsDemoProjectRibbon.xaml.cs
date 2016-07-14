using System.Collections.Generic;
using System.Windows;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Demo.Ringtoets.Commands;
using Fluent;

namespace Demo.Ringtoets.Ribbons
{
    /// <summary>
    /// Interaction logic for RingtoetsDemoProjectRibbon.xaml
    /// </summary>
    public partial class RingtoetsDemoProjectRibbon : IRibbonCommandHandler
    {
        private readonly ICommand addNewAssessmentSection, openMapViewCommand, openChartViewCommand;

        public RingtoetsDemoProjectRibbon(IProjectOwner projectOwner, IViewController viewController)
        {
            InitializeComponent();

            addNewAssessmentSection = new AddNewDemoAssessmentSectionCommand(projectOwner);
            openChartViewCommand = new OpenChartViewCommand(viewController);
            openMapViewCommand = new OpenMapViewCommand(viewController);
        }

        public IEnumerable<ICommand> Commands
        {
            get
            {
                yield return addNewAssessmentSection;
                yield return openChartViewCommand;
                yield return openMapViewCommand;
            }
        }

        public Ribbon GetRibbonControl()
        {
            return RingtoetsDemoProjectRibbonControl;
        }

        public void ValidateItems() {}

        public bool IsContextualTabVisible(string tabGroupName, string tabName)
        {
            return false;
        }

        private void AddNewDemoAssessmentSectionButton_Click(object sender, RoutedEventArgs e)
        {
            addNewAssessmentSection.Execute();
        }

        private void ButtonOpenChartView_Click(object sender, RoutedEventArgs e)
        {
            openChartViewCommand.Execute();
        }

        private void ButtonOpenMapView_Click(object sender, RoutedEventArgs e)
        {
            openMapViewCommand.Execute();
        }
    }
}