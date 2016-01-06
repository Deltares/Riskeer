using System.Collections.Generic;
using Core.Common.Controls;
using Core.Common.Controls.Commands;
using Core.Common.Gui.Forms;
using Fluent;

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

        public IEnumerable<ICommand> Commands
        {
            get
            {
                yield break;
            }
        }

        public Ribbon GetRibbonControl()
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