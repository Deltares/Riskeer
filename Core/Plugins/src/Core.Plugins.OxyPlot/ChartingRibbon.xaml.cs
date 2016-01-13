using System.Collections.Generic;
using System.Windows;
using Core.Common.Controls.Commands;
using Core.Common.Gui.Forms;
using Fluent;

namespace Core.Plugins.OxyPlot
{
    /// <summary>
    /// This class represents the ribbon interaction which has to do with charting.
    /// </summary>
    public partial class ChartingRibbon : IRibbonCommandHandler
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChartingRibbon"/>.
        /// </summary>
        public ChartingRibbon()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the command used when the open chart button is clicked.
        /// </summary>
        public ICommand OpenChartViewCommand { private get; set; }

        /// <summary>
        /// Sets the command used when the toggle legend view button is clicked.
        /// </summary>
        public ICommand ToggleLegendViewCommand { private get; set; }

        public IEnumerable<ICommand> Commands
        {
            get
            {
                if (OpenChartViewCommand != null)
                {
                    yield return OpenChartViewCommand;
                }
                if (ToggleLegendViewCommand != null)
                {
                    yield return ToggleLegendViewCommand;
                }
            }
        }

        /// <summary>
        /// Shows the charting contextual tab.
        /// </summary>
        public void ShowChartingTab()
        {
            ChartingContextualGroup.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hides the charting contextual tab.
        /// </summary>
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
            ToggleLegendViewButton.IsChecked = ToggleLegendViewCommand.Checked;
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