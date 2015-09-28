using System.Windows.Forms;

namespace DelftTools.Controls.Swf.Editors
{
    public class MultiLineTextEdior : UserControl, ITypeEditor
    {
        public object EditableValue { get; set; }

        public bool CanAcceptEditValue()
        {
            return false;
        }

        public bool CanPopup()
        {
            return true;
        }
    }
}