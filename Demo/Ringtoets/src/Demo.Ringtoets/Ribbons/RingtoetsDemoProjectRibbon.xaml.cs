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
        private readonly ICommand addNewDikeAssessmentSection, openMapViewCommand, openChartViewCommand;

        public RingtoetsDemoProjectRibbon(IProjectOwner projectOwner, IDocumentViewController documentViewController)
        {
            InitializeComponent();

            addNewDikeAssessmentSection = new AddNewDemoDikeAssessmentSectionCommand(projectOwner);
            openChartViewCommand = new OpenChartViewCommand(documentViewController);
            openMapViewCommand = new OpenMapViewCommand(documentViewController);
        }

        public IEnumerable<ICommand> Commands
        {
            get
            {
                yield return addNewDikeAssessmentSection;
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

        private void AddNewDemoDikeAssessmentSectionButton_Click(object sender, RoutedEventArgs e)
        {
            addNewDikeAssessmentSection.Execute();
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