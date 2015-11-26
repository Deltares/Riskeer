using System.Windows.Forms;
using Core.Common.Gui;
using Ringtoets.Common.Forms.Extensions;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Placeholder;

namespace Ringtoets.Integration.Forms.ContextMenu
{
    public class PlaceholderWithReadonlyNameContextMenu
    {
        public static ContextMenuStrip CreateContextMenu(PlaceholderWithReadonlyName nodeData, IGuiCommandHandler guiHandler, IContextMenuProvider provider)
        {
            ContextMenuStrip contextMenu = provider.Get(nodeData);

            if (nodeData is InputPlaceholder || nodeData is OutputPlaceholder)
            {
                contextMenu.InsertMenuItem(0,
                    Resources.FailureMechanism_InputsOutputs_Open,
                    Resources.FailureMechanism_InputsOutputs_Open_ToolTip,
                    Resources.OpenIcon, null).Enabled = false;
                contextMenu.InsertSeperator(1);

                contextMenu.InsertMenuItem(2,
                    Resources.FailureMechanism_InputsOutputs_Erase,
                    Resources.FailureMechanism_InputsOutputs_Erase_ToolTip,
                    Resources.ClearIcon, null).Enabled = false;
                contextMenu.InsertSeperator(3);

                contextMenu.AddSeperator();
                contextMenu.AddMenuItem(
                    Resources.FailureMechanism_Properties,
                    Resources.FailureMechanism_Properties_ToolTip,
                    Resources.PropertiesIcon, (s, e) => ShowProperties(guiHandler));
            }
            return contextMenu;
        }

        private static void ShowProperties(IGuiCommandHandler guiHandler)
        {
            if (guiHandler != null)
            {
                guiHandler.ShowProperties();
            }
        }
    }
}