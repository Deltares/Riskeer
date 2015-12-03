using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;

namespace Core.Common.Gui.Forms
{
    public interface IProjectExplorer : IView
    {
        ITreeView TreeView { get; }

        void ScrollTo(object o);
    }
}