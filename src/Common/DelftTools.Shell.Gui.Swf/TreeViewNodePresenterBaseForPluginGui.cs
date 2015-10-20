using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Controls.Swf.TreeViewControls;

namespace DelftTools.Shell.Gui.Swf
{
    /// <summary>
    /// Generic abstract class to save repetition in implementation of nodepresenters.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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