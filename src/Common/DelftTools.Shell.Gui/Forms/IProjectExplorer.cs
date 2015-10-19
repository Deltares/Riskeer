using DelftTools.Controls;
using DelftTools.Shell.Core;

namespace DelftTools.Shell.Gui.Forms
{
    public interface IProjectExplorer : IView
    {
        ITreeView TreeView { get; }

        void ScrollTo(IProjectItem projectItem);
    }
}