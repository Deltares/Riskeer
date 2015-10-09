using DelftTools.Controls;
using DelftTools.Controls.Swf.TreeViewControls;
using log4net;

namespace DelftTools.Shell.Gui.Swf
{
    /// <summary>
    /// Generic abstract class to save repetition in implementation of nodepresenters.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TreeViewNodePresenterBaseForPluginGui<T> : TreeViewNodePresenterBase<T>
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(TreeViewNodePresenterBaseForPluginGui<T>));

        public TreeViewNodePresenterBaseForPluginGui() {}

        public TreeViewNodePresenterBaseForPluginGui(GuiPlugin guiPlugin)
        {
            GuiPlugin = guiPlugin;
        }

        public GuiPlugin GuiPlugin { get; set; }

        public override IMenuItem GetContextMenu(ITreeNode sender, object nodeData)
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