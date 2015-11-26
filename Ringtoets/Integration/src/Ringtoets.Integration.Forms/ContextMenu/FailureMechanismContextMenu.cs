using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui;
using Ringtoets.Common.Forms.Extensions;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Piping.Data;

namespace Ringtoets.Integration.Forms.ContextMenu
{
    public class FailureMechanismContextMenu
    {
        public static ContextMenuStrip CreateContextMenu(IGuiCommandHandler commandHandler, ITreeView treeView, FailureMechanismPlaceholder item, IContextMenuProvider provider)
        {
            ContextMenuStrip contextMenu = provider.Get(item);

            contextMenu.InsertMenuItem(0,
                Resources.Calculate_all,
                Resources.Calculate_all_ToolTip,
                Resources.CalculateAllIcon, null).Enabled = false;
            contextMenu.InsertMenuItem(1,
                Resources.Clear_all_output,
                Resources.Clear_all_output_ToolTip,
                Resources.ClearIcon, null).Enabled = false;
            contextMenu.InsertSeperator(2);

            contextMenu.InsertMenuItem(3,
                Resources.FailureMechanism_Expand_all,
                Resources.FailureMechanism_Expand_all_ToolTip,
                Resources.ExpandAllIcon, (s, e) => ExpandAllItemClicked(treeView));
            contextMenu.InsertMenuItem(4,
                Resources.FailureMechanism_Collapse_all,
                Resources.FailureMechanism_Collapse_all_ToolTip,
                Resources.CollapseAllIcon, (s,e) => CollapseAllItemClicked(treeView));
            contextMenu.InsertSeperator(5);

            return contextMenu;
        }

        public static void ShowProperties(IGuiCommandHandler commandHandler)
        {
            if (commandHandler != null)
            {
                commandHandler.ShowProperties();
            }
        }

        public static void CollapseAllItemClicked(ITreeView treeView)
        {
            treeView.CollapseAll(treeView.SelectedNode);
        }

        public static void ExpandAllItemClicked(ITreeView treeView)
        {
            treeView.ExpandAll(treeView.SelectedNode);
        }
    }
}