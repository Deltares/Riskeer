using System.Collections.Generic;
using Core.Common.Controls;
using Core.Common.Controls.Commands;
using Core.Common.Gui.Forms;

namespace Ringtoets.Piping.Plugin
{
    /// <summary>
    /// Interaction logic for RingtoetsRibbon.xaml
    /// </summary>
    public partial class PipingRibbon : IRibbonCommandHandler
    {
        public PipingRibbon()
        {
            InitializeComponent();
        }

        public IEnumerable<Command> Commands
        {
            get
            {
                yield break;
            }
        }

        public object GetRibbonControl()
        {
            return RingtoetsRibbonControl;
        }

        public void ValidateItems() {}

        public bool IsContextualTabVisible(string tabGroupName, string tabName)
        {
            return false;
        }
    }
}