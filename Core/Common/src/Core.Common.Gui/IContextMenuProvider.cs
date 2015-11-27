using Core.Common.Controls;
using Core.Common.Gui.ContextMenu;

namespace Core.Common.Gui
{
    public interface IContextMenuProvider
    {
        ContextMenuBuilder Get(ITreeNode obj);
    }
}