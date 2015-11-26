using System.Windows.Forms;

namespace Core.Common.Gui
{
    public interface IContextMenuProvider
    {
        ContextMenuStrip Get(object obj);
    }
}