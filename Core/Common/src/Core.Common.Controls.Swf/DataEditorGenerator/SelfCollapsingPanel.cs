using System;
using System.Windows.Forms;
using Core.Common.Utils;

namespace Core.Common.Controls.Swf.DataEditorGenerator
{
    public class SelfCollapsingPanel : Panel
    {
        public event EventHandler<EventArgs<bool>> VisibleWithoutParentChanged;
        private bool visibleWithoutParent = true;

        /// <summary>
        /// Gets Visible when this panel would not have been added to a panel.
        /// It ignores the state of the parent control so it will be more likely that this value
        /// will be true.
        /// </summary>
        public bool VisibleWithoutParent
        {
            get
            {
                return visibleWithoutParent;
            }
        }

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