using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;

namespace Core.Common.Gui.Swf
{
    /// <summary>
    /// Generic abstract class to save repetition in implementation of nodepresenters.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    // TODO: Remove this class and use TreeViewNodePresenterBase<T> instead or directly derive ITreeNodePresenter
    public abstract class TreeViewNodePresenterBaseForPluginGui<T> : TreeViewNodePresenterBase<T>
    {
        public TreeViewNodePresenterBaseForPluginGui() {}

        public TreeViewNodePresenterBaseForPluginGui(GuiPlugin guiPlugin)
        {
            GuiPlugin = guiPlugin;
        }

        public GuiPlugin GuiPlugin { get; set; }

        public override ContextMenuStrip GetContextMenu(ITreeNode sender, object nodeData)
        {
            if (GuiPlugin == null)
            {
                return null;
            }
            var x = GuiPlugin.GetContextMenu(sender, nodeData);
            return x;
        }

        protected IGui Gui
        {
            get
            {
                return GuiPlugin.Gui;
            }
        }
    }
}