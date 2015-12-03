using System.Windows.Forms;

namespace Core.Common.Controls.Swf.Table.Editors
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