using System.Collections.Generic;
using System.Windows;
using Core.Common.Controls.Commands;
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
        private readonly ICommand addNewDikeAssessmentSection, addNewDuneAssessmentSection, openMapViewCommand;

        public RingtoetsDemoProjectRibbon()
        {
            InitializeComponent();

            addNewDikeAssessmentSection = new AddNewDemoDikeAssessmentSectionCommand();
            addNewDuneAssessmentSection = new AddNewDemoDuneAssessmentSectionCommand();
            openMapViewCommand = new OpenMapViewCommand();
        }

        public IEnumerable<ICommand> Commands
        {
            get
            {
                yield return addNewDikeAssessmentSection;
                yield return addNewDuneAssessmentSection;
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

        private void AddNewDemoDuneAssessmentSectionButton_Click(object sender, RoutedEventArgs e)
        {
            addNewDuneAssessmentSection.Execute();
        }

        private void ButtonOpenMapView_Click(object sender, RoutedEventArgs e)
        {
            openMapViewCommand.Execute();
        }
    }
}