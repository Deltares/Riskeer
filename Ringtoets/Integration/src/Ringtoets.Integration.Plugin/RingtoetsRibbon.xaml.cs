using System.Collections.Generic;

using Core.Common.Controls;
using Core.Common.Controls.Commands;
using Core.Common.Gui.Forms;
using Fluent;

namespace Ringtoets.Integration.Plugin
{
    /// <summary>
    /// Interaction logic for RingtoetsRibbon.xaml
    /// </summary>
    public partial class RingtoetsRibbon : IRibbonCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RingtoetsRibbon"/> class.
        /// </summary>
        public RingtoetsRibbon()
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

        public void ValidateItems()
        {
            // Do nothing
        }

        public bool IsContextualTabVisible(string tabGroupName, string tabName)
        {
            return false;
        }
    }
}