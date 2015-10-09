using System.Collections.Generic;
using System.Windows;
using DelftTools.Controls;
using DelftTools.Shell.Gui.Forms;
using DeltaShell.Plugins.ProjectExplorer.Commands;

namespace DeltaShell.Plugins.ProjectExplorer
{
    /// <summary>
    /// Interaction logic for Ribbon.xaml
    /// </summary>
    public partial class Ribbon : IRibbonCommandHandler
    {
        private readonly ICommand showProjectExplorerCommand;

        public Ribbon()
        {
            InitializeComponent();

            showProjectExplorerCommand = new ShowProjectExplorerCommand();
        }

        public IEnumerable<ICommand> Commands
        {
            get
            {
                yield return showProjectExplorerCommand;
            }
        }

        public void ValidateItems()
        {
            ButtonShowProjectExplorerToolWindow.IsChecked = showProjectExplorerCommand.Checked;
        }

        public bool IsContextualTabVisible(string tabGroupName, string tabName)
        {
            return false;
        }

        public object GetRibbonControl()
        {
            return RibbonControl;
        }

        private void ButtonShowProjectExplorerToolWindowClick(object sender, RoutedEventArgs e)
        {
            showProjectExplorerCommand.Execute();
            ValidateItems();
        }
    }
}