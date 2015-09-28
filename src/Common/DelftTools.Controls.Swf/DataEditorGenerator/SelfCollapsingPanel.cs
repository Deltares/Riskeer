using System;
using System.Windows.Forms;
using DelftTools.Utils;

namespace DelftTools.Controls.Swf.DataEditorGenerator
{
    public class SelfCollapsingPanel : Panel
    {
        private bool visibleWithoutParent = true;
        
        /// <summary>
        /// Gets Visible when this panel would not have been added to a panel.
        /// It ignores the state of the parent control so it will be more likely that this value
        /// will be true.
        /// </summary>
        public bool VisibleWithoutParent
        {
            get { return visibleWithoutParent; }
        }

        public event EventHandler<EventArgs<bool>> VisibleWithoutParentChanged;

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(value);

            bool oldValue = visibleWithoutParent;

            visibleWithoutParent = value;

            if (visibleWithoutParent != oldValue)
            {
                if (VisibleWithoutParentChanged != null)
                {
                    VisibleWithoutParentChanged(this, new EventArgs<bool>(visibleWithoutParent));
                }
            }
        }
    }
}
