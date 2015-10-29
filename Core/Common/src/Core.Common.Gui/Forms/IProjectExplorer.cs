using Core.Common.BaseDelftTools;
using Core.Common.Controls;

namespace Core.Common.Gui.Forms
{
    public interface IProjectExplorer : IView
    {
        ITreeView TreeView { get; }

        void ScrollTo(IProjectItem projectItem);
    }
}