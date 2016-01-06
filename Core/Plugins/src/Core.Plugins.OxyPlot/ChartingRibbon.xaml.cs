using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using Core.Common.Forms.Commands;
using Core.Common.Gui.Forms;
using Fluent;

namespace Core.Plugins.OxyPlot
{
    /// <summary>
    /// Interaction logic for ribbon.xaml
    /// </summary>
    public partial class ChartingRibbon : IRibbonCommandHandler
    {
        private readonly ICommand openChartViewCommand;

        public ChartingRibbon()
        {
            InitializeComponent();
            openChartViewCommand = new OpenChartViewCommand();
        }

        public IEnumerable<ICommand> Commands
        {
            get
            {
                yield return openChartViewCommand;
            }
        }

        public Ribbon GetRibbonControl()
        {
            return RibbonControl;
        }

        public void ValidateItems()
        {
        }

        public bool IsContextualTabVisible(string tabGroupName, string tabName)
        {
            return tabGroupName == ChartingContextualGroup.Name;
        }

        private void ButtonOpenChartView_Click(object sender, RoutedEventArgs e)
        {
            openChartViewCommand.Execute();
        }

        public void ShowChartingTab()
        {
            ChartingContextualGroup.Visibility = Visibility.Visible;
        }

        public void HideChartingTab()
        {
            ChartingContextualGroup.Visibility = Visibility.Collapsed;
        }
    }
}
