using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Controls.Swf;

namespace DelftTools.Shell.Gui.Swf
{
    public class NodePresenterHelper
    {
        /// <summary>
        /// Queries all pluginguis for context menu items for item
        /// </summary>
        /// <param name="gui"></param>
        /// <param name="sender"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IMenuItem GetContextMenuFromPluginGuis(IGui gui, object sender, object item)
        {
            var emptyMenu = new ContextMenuStrip();

            IMenuItem menuItem = new MenuItemContextMenuStripAdapter(emptyMenu);

            foreach (var plugin in gui.Plugins)
            {
                if (plugin != null)
                {
                    IMenuItem mi = plugin.GetContextMenu(sender, item);
                    if (mi != null)
                    {
                        menuItem.Add(mi);
                    }
                }
            }

            return menuItem;
        }

        public static void TrimSeparatorsGetContextMenu(IMenuItem menuItem)
        {
            if (menuItem is MenuItemContextMenuStripAdapter)
            {
                TrimSeparatorsGetContextMenu(
                    ((MenuItemContextMenuStripAdapter) menuItem).ContextMenuStrip
                    );
            }
        }

        public static void TrimSeparatorsGetContextMenu(ContextMenuStrip contextMenu)
        {
            //top
            for (int i = 0; i < contextMenu.Items.Count; i++)
            {
                if (contextMenu.Items[i] is ToolStripSeparator)
                {
                    contextMenu.Items[i].Available = false;
                }
                else
                {
                    if (contextMenu.Items[i].Available)
                    {
                        break;
                    }
                }
            }
            //bottom
            for (int i = contextMenu.Items.Count - 1; i >= 0; i--)
            {
                if (contextMenu.Items[i] is ToolStripSeparator)
                {
                    contextMenu.Items[i].Available = false;
                }
                else
                {
                    if (contextMenu.Items[i].Available)
                    {
                        break;
                    }
                }
            }
        }

        public static void RemoveDoubleSeparatorsGetContextMenu(IMenuItem menuItem)
        {
            if (menuItem is MenuItemContextMenuStripAdapter)
            {
                RemoveDoubleSeparatorsGetContextMenu(
                    ((MenuItemContextMenuStripAdapter) menuItem).ContextMenuStrip
                    );
            }
        }

        public static void RemoveDoubleSeparatorsGetContextMenu(ContextMenuStrip contextMenu)
        {
            for (int i = 1; i < contextMenu.Items.Count; i++)
            {
                if (contextMenu.Items[i] is ToolStripSeparator)
                {
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (contextMenu.Items[j] is ToolStripSeparator)
                        {
                            contextMenu.Items[j].Available = false;
                            break;
                        }

                        if (contextMenu.Items[j].Available)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}