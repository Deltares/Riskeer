using System.Linq;
using System.Windows.Forms;
using DelftTools.Utils;

namespace DelftTools.Controls.Swf.DataEditorGenerator
{
    public class SelfCollapsingGroupbox : GroupBox
    {
        private Control container;

        public void SetChildContainer(Control containerWithChildren)
        {
            container = containerWithChildren;
        }

        public void SubscribeChild(SelfCollapsingPanel child)
        {
            child.VisibleWithoutParentChanged += Child_VisibleChanged;

            UpdateVisibility();
        }

        private void Child_VisibleChanged(object sender, EventArgs<bool> eventArgs)
        {
            if (eventArgs.Value)
            {
                Visible = true;
            }
            else
            {
                UpdateVisibility();
            }
        }

        private void UpdateVisibility()
        {
            // one has become false, so check them all
            Visible = container.Controls.OfType<SelfCollapsingPanel>().Any(child => child.VisibleWithoutParent);
        }
    }
}
