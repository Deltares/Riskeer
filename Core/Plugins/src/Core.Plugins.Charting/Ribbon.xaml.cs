using System.Collections.Generic;
using System.Windows;
using Core.Common.Forms.Commands;
using Core.Common.Gui.Forms;
using Core.Plugins.Charting.Commands;

namespace Core.Plugins.Charting
{
    /// <summary>
    /// Interaction logic for Ribbon.xaml
    /// </summary>
    public partial class Ribbon : IRibbonCommandHandler
    {
        private readonly ToggleChartLegendViewCommand toggleChartLegendViewCommand;
        private readonly ExportChartAsImageCommand exportAsImageCommand;
        private readonly DecreaseFontSizeCommand decreaseFontSizeCommand;
        private readonly IncreaseFontSizeCommand increasFontSizeCommand;
        private readonly RulerCommand rulerCommand;

        public Ribbon()
        {
            InitializeComponent();

            toggleChartLegendViewCommand = new ToggleChartLegendViewCommand();
            exportAsImageCommand = new ExportChartAsImageCommand();
            increasFontSizeCommand = new IncreaseFontSizeCommand();
            decreaseFontSizeCommand = new DecreaseFontSizeCommand();
            rulerCommand = new RulerCommand();
            chartTab.Group = chartingContextualGroup;
        }

        public IEnumerable<ICommand> Commands
        {
            get
            {
                yield return toggleChartLegendViewCommand;
                yield return exportAsImageCommand;
                yield return increasFontSizeCommand;
                yield return decreaseFontSizeCommand;
                yield return rulerCommand;
            }
        }

        public void ValidateItems()
        {
            ButtonExportAsImage.IsEnabled = exportAsImageCommand.Enabled;
            ButtonIncreaseFontSize.IsEnabled = increasFontSizeCommand.Enabled;
            ButtonDecreaseFontSize.IsEnabled = decreaseFontSizeCommand.Enabled;
            ButtonChartLegendToolWindow.IsChecked = toggleChartLegendViewCommand.Checked;
            RulerToggleButton.IsEnabled = rulerCommand.Enabled;
            RulerToggleButton.IsChecked = rulerCommand.Checked;
        }

        public bool IsContextualTabVisible(string tabGroupName, string tabName)
        {
            return tabGroupName == chartingContextualGroup.Name && tabName == chartTab.Name && exportAsImageCommand.Enabled;
        }

        public Fluent.Ribbon GetRibbonControl()
        {
            return RibbonControl;
        }

        private void ButtonChartLegendToolWindow_Click(object sender, RoutedEventArgs e)
        {
            toggleChartLegendViewCommand.Execute();
            ValidateItems();
        }

        private void ButtonDecreaseFontSize_Click(object sender, RoutedEventArgs e)
        {
            decreaseFontSizeCommand.Execute();
        }

        private void ButtonIncreaseFontSize_Click(object sender, RoutedEventArgs e)
        {
            increasFontSizeCommand.Execute();
        }

        private void ButtonExportAsImage_Click(object sender, RoutedEventArgs e)
        {
            exportAsImageCommand.Execute();
        }

        private void RulerButtonChecked(object sender, RoutedEventArgs e)
        {
            if (rulerCommand.Enabled)
            {
                rulerCommand.Execute(RulerToggleButton.IsChecked);
            }
        }
    }
}