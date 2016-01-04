using System.Collections.Generic;
using Core.Common.Controls.Commands;
using Core.Common.Gui.Forms;

namespace Core.Plugins.OxyPlot
{
    /// <summary>
    /// Interaction logic for ribbon.xaml
    /// </summary>
    public partial class Ribbon : IRibbonCommandHandler
    {
        private readonly ICommand openChartViewCommand;

        public Ribbon()
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

        public object GetRibbonControl()
        {
            return RibbonControl;
        }

        public void ValidateItems()
        {
        }

        public bool IsContextualTabVisible(string tabGroupName, string tabName)
        {
            return false;
        }

        private void ButtonOpenChartView_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            openChartViewCommand.Execute();
        }
    }
}
