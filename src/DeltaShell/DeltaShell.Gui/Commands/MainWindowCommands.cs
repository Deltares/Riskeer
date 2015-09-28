using System.Windows.Input;
using DeltaShell.Gui.Properties;

namespace DeltaShell.Gui.Commands
{
    public static class MainWindowCommands
    {
        public static readonly RoutedUICommand CloseViewTabCommand = new RoutedUICommand(Resources.MainWindowCommands_CloseViewTabCommand_Text_Close_Current_view__Ctrl___W_, Resources.MainWindowCommands_CloseViewTabCommand_Name_Close_Current_View, typeof(MainWindowCommands));
        public static readonly RoutedUICommand ShowObjectInTreeViewCommand = new RoutedUICommand(Resources.MainWindowCommands_ShowObjectInTreeViewCommand_Text_Find_In_Project_Treeview__Shift___Alt___L_, Resources.MainWindowCommands_ShowObjectInTreeViewCommand_Name_Find_In_Project_Treeview, typeof(MainWindowCommands));
        public static readonly RoutedUICommand ShowMessageWindowCommand = new RoutedUICommand(Resources.MainWindowCommands_ShowMessageWindowCommand_Text_Show_Messages__Shift___Alt____O_, Resources.MainWindowCommands_ShowMessageWindowCommand_Name_Show_Messages, typeof(MainWindowCommands));
        public static readonly RoutedUICommand ShowMapLegendViewCommand = new RoutedUICommand(Resources.MainWindowCommands_ShowMapLegendViewCommand_Text_Show_Map_Contents__Shift___Alt____M_, Resources.MainWindowCommands_ShowMapLegendViewCommand_Name_Show_Map_Contents, typeof(MainWindowCommands));
        public static readonly RoutedUICommand ShowPropertiesViewCommand = new RoutedUICommand(Resources.MainWindowCommands_ShowPropertiesViewCommand_Text_Show_Properties__Shift___Alt____P_, Resources.MainWindowCommands_ShowPropertiesViewCommand_Name_Show_Properties, typeof(MainWindowCommands));
        public static readonly RoutedUICommand ShowChartContentsViewCommand = new RoutedUICommand(Resources.MainWindowCommands_ShowChartContentsViewCommand_Text_Show_Chart_Contents__Shift___Alt____C_, Resources.MainWindowCommands_ShowChartContentsViewCommand_Name_Show_Chart_Contents, typeof(MainWindowCommands));
    }
}