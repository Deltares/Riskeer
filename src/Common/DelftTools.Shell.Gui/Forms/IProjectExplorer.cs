using DelftTools.Controls;
using DelftTools.Shell.Core;

namespace DelftTools.Shell.Gui.Forms
{
    public interface IProjectExplorer : IView
    {
        ITreeView TreeView { get; }

        IMenuItem GetContextMenu(ITreeNode sender, object o);

        void ScrollTo(IProjectItem projectItem);
    }
}