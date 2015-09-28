using System.Collections.Generic;
using System.Windows;
using DelftTools.Controls;
using DelftTools.Shell.Gui.Forms;
using DeltaShell.Plugins.DemoGuiPlugin.Commands;

namespace DeltaShell.Plugins.DemoGuiPlugin
{
    /// <summary>
    /// Interaction logic for Ribbon.xaml.
    /// </summary>
    public partial class Ribbon : IRibbonCommandHandler
    {
        private readonly ICommand wtiProjectCommand;

        public Ribbon()
        {
            InitializeComponent();

            wtiProjectCommand = new WTIProjectCommand();
        }

        public IEnumerable<ICommand> Commands
        {
            get
            {
                yield return wtiProjectCommand;
            }
        }

        public void ValidateItems()
        {
            ButtonWTIProject.IsEnabled = wtiProjectCommand.Enabled;
        }

        public bool IsContextualTabVisible(string tabGroupName, string tabName)
        {
            return false;
        }

        public object GetRibbonControl()
        {
            return RibbonControl;
        }

        private void ButtonWTIProject_Click(object sender, RoutedEventArgs e)
        {
            wtiProjectCommand.Execute();
        }
    }
}
