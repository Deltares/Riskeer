using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
