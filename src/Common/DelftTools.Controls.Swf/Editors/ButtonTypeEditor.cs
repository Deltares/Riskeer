using System;
using System.Drawing;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf.Editors
{
    public class ButtonTypeEditor : UserControl, ITypeEditor
    {
        /// <summary>
        /// Caption for this button. Not visible when <see cref="Image "/>  property is set.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Tooltip for this button. 
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Image for this button. If this property is set, it will override the <see cref="Caption"/> of this button.
        /// </summary>
        public Image Image { get; set; }

        public object EditableValue { get; set; }

        public bool HideOnReadOnly { get; set; }

        public bool CanAcceptEditValue()
        {
            return false;
        }

        public bool CanPopup()
        {
            return false;
        }

        public Action ButtonClickAction { get; set; }

        protected override void Dispose(bool disposing)
        {
            EditableValue = null;
            ButtonClickAction = null;

            base.Dispose(disposing);
        }
    }
}