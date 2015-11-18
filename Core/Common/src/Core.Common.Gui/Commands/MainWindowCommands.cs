using System.Windows.Input;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.Commands
{
    public static class MainWindowCommands
    {
        public static readonly RoutedUICommand CloseViewTabCommand = new RoutedUICommand();
        public static readonly RoutedUICommand ShowObjectInTreeViewCommand = new RoutedUICommand();
        public static readonly RoutedUICommand ShowMessageWindowCommand = new RoutedUICommand();
        public static readonly RoutedUICommand ShowMapLegendViewCommand = new RoutedUICommand();
        public static readonly RoutedUICommand ShowPropertiesViewCommand = new RoutedUICommand();
        public static readonly RoutedUICommand ShowChartContentsViewCommand = new RoutedUICommand();
    }
}