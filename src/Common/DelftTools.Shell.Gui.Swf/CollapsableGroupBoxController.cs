using System;
using System.Linq;
using System.Windows.Forms;

namespace DelftTools.Shell.Gui.Swf
{
    public class CollapsableGroupBoxController
    {
        public void Initialize(Control control)
        {
            CollapsableGroupBox topMost = null;

            foreach (var groupBox in control.Controls.OfType<CollapsableGroupBox>())
            {
                if (groupBox.Dock != DockStyle.Top)
                {
                    throw new ArgumentException(String.Format("Groupbox {0} is not docked to top", groupBox.Name));
                }

                groupBox.MaximumSize = groupBox.Size;

                if (groupBox.Top < ((topMost != null) ? topMost.Top : control.Height))
                {
                    topMost = groupBox;
                }
                groupBox.Collapse();
            }

            if (topMost != null)
            {
                topMost.Expand();
            }
        }
    }
}