using System.Collections.Generic;
using System.Windows;
using Core.Common.Controls.Commands;
using Core.Common.Gui.Forms;
using Fluent;

namespace Core.Plugins.OxyPlot
{
    /// <summary>
    /// Interaction logic for ribbon.xaml
    /// </summary>
    public partial class ChartingRibbon : IRibbonCommandHandler
    {
        public ChartingRibbon()
        {
            InitializeComponent();
        }

        public ICommand OpenChartViewCommand { private get; set; }
        public ICommand ToggleLegendViewCommand { private get; set; }

        public IEnumerable<ICommand> Commands
        {
            get
            {
                yield return OpenChartViewCommand;
                yield return ToggleLegendViewCommand;
            }
        }

        public void ShowChartingTab()
        {
            ChartingContextualGroup.Visibility = Visibility.Visible;
        }

        public void HideChartingTab()
        {
            ChartingContextualGroup.Visibility = Visibility.Collapsed;
        }

        public Ribbon GetRibbonControl()
        {
            return RibbonControl;
        }

        public void ValidateItems()
        {
            ToggleLegendButton.IsChecked = ToggleLegendViewCommand.Checked;
        }

        public bool IsContextualTabVisible(string tabGroupName, string tabName)
        {
            // TODO: Required only because this method is called each time ValidateItems is called in MainWindow
            // Once ValidateItems isn't responsible for showing/hiding contextual tabs, then this method can return false,
            // but more ideally be removed.
            return ChartingContextualGroup.Name == tabGroupName && ChartingContextualGroup.Visibility == Visibility.Visible;
        }

        private void ButtonOpenChartView_Click(object sender, RoutedEventArgs e)
        {
            OpenChartViewCommand.Execute();
        }

        private void ButtonToggleLegend_Click(object sender, RoutedEventArgs e)
        {
            ToggleLegendViewCommand.Execute();
        }
    }
}