using System.Collections.Generic;
using DelftTools.Shell.Gui.Forms;

using ICommand = DelftTools.Controls.ICommand;

namespace Wti.Plugin
{
    /// <summary>
    /// Interaction logic for WtiRibbon.xaml
    /// </summary>
    public partial class WtiRibbon : IRibbonCommandHandler
    {
        public WtiRibbon()
        {
            InitializeComponent();
        }

        public IEnumerable<ICommand> Commands
        {
            get
            {
                yield break;
            }
        }

        public object GetRibbonControl()
        {
            return WtiRibbonControl;
        }

        public void ValidateItems()
        {
            
        }

        public bool IsContextualTabVisible(string tabGroupName, string tabName)
        {
            return false;
        }
    }
}
