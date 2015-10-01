using DelftTools.Controls;

namespace Wti.Controller
{
    public interface IContextMenuProvider
    {
        IMenuItem GetContextMenu(object source);
    }
}