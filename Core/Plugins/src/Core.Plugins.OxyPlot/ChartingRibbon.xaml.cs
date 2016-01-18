using System.Collections.Generic;
using System.Windows;
using Core.Common.Base;
using Core.Common.Controls.Commands;
using Core.Common.Gui.Forms;
using Core.Components.Charting;
using Fluent;

namespace Core.Plugins.OxyPlot
{
    /// <summary>
    /// This class represents the ribbon interaction which has to do with charting.
    /// </summary>
    public partial class ChartingRibbon : IRibbonCommandHandler, IObserver
    {
        private IChart chart;

        /// <summary>
        /// Creates a new instance of <see cref="ChartingRibbon"/>.
        /// </summary>
        public ChartingRibbon()
        {
            InitializeComponent();
        }

        public IChart Chart
        {
            private get
            {
                return chart;
            }
            set
            {
                SetChart(value);

                if (chart != null)
                {
                    ShowChartingTab();
                }
                else
                {
                    HideChartingTab();
                }
            }
        }

        private void SetChart(IChart value)
        {
            if (chart != null)
            {
                chart.Detach(this);
            }
            chart = value;
            if (chart != null)
            {
                chart.Attach(this);
            }
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
        private void ShowChartingTab()
        {
            ChartingContextualGroup.Visibility = Visibility.Visible;
            ValidateItems();
        }

        /// <summary>
        /// Hides the charting contextual tab.
        /// </summary>
        private void HideChartingTab()
        {
            ChartingContextualGroup.Visibility = Visibility.Collapsed;
        }

        public Ribbon GetRibbonControl()
        {
            return RibbonControl;
        }

        public void ValidateItems()
        {
            ToggleLegendViewButton.IsChecked = ToggleLegendViewCommand != null && ToggleLegendViewCommand.Checked;
            TogglePanningButton.IsChecked = Chart != null && Chart.IsPanning;
            ToggleRectangleZoomingButton.IsChecked = Chart != null && Chart.IsRectangleZooming;
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

        private void ButtonTogglePanning_Click(object sender, RoutedEventArgs e)
        {
            Chart.TogglePanning();
            Chart.NotifyObservers();
        }

        private void ButtonToggleRectangleZooming_Click(object sender, RoutedEventArgs e)
        {
            Chart.ToggleRectangleZooming();
            Chart.NotifyObservers();
        }

        private void ButtonZoomToAll_Click(object sender, RoutedEventArgs e)
        {
            Chart.ZoomToAll();
        }

        public void UpdateObserver()
        {
            ValidateItems();
        }
    }
}