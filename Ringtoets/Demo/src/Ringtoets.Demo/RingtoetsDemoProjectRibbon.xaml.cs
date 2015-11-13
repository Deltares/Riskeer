using System.Collections.Generic;
using System.Linq;

using Core.Common.Controls;
using Core.Common.Gui.Forms;
using DemoResources = Ringtoets.Demo.Properties.Resources;

namespace Ringtoets.Demo
{
    /// <summary>
    /// Interaction logic for RingtoetsDemoProjectRibbon.xaml
    /// </summary>
    public partial class RingtoetsDemoProjectRibbon : IRibbonCommandHandler
    {
        public RingtoetsDemoProjectRibbon()
        {
            InitializeComponent();
        }

        public IEnumerable<ICommand> Commands
        {
            get
            {
                return Enumerable.Empty<ICommand>();
            }
        }

        public object GetRibbonControl()
        {
            return RingtoetsDemoProjectRibbonControl;
        }

        public void ValidateItems()
        {
            
        }

        public bool IsContextualTabVisible(string tabGroupName, string tabName)
        {
            return false;
        }

        private void AddNewRingtoetsDemoProjectButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
