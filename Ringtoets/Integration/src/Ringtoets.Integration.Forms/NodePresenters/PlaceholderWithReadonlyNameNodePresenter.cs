using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;
using Ringtoets.Common.Forms.Extensions;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Common.Placeholder;
using Ringtoets.Integration.Forms.Properties;

using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.NodePresenters
{
    /// <summary>
    /// Node presenter class for <see cref="PlaceholderWithReadonlyName"/>
    /// </summary>
    public class PlaceholderWithReadonlyNameNodePresenter : RingtoetsNodePresenterBase<PlaceholderWithReadonlyName>
    {
        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PlaceholderWithReadonlyName nodeData)
        {
            node.Text = nodeData.Name;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.GrayText);
            node.Image = GetIconForPlaceholder(nodeData);
        }

        private static Bitmap GetIconForPlaceholder(PlaceholderWithReadonlyName nodeData)
        {
            if (nodeData is InputPlaceholder || nodeData is OutputPlaceholder)
            {
                return Resources.GenericInputOutputIcon;
            }
            return Resources.PlaceholderIcon;
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode sender, PlaceholderWithReadonlyName nodeData)
        {
            var contextMenu = new ContextMenuStrip();

            if (nodeData is InputPlaceholder || nodeData is OutputPlaceholder)
            {
                contextMenu.AddMenuItem(
                    RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Open,
                    RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Open_ToolTip,
                    RingtoetsCommonFormsResources.OpenIcon, null);
                contextMenu.AddSeperator();

                contextMenu.AddMenuItem(
                    RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase,
                    RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase_ToolTip,
                    RingtoetsCommonFormsResources.ClearIcon, null);
                contextMenu.AddSeperator();
            }

            if (nodeData is InputPlaceholder)
            {
                contextMenu.AddMenuItem(
                    RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Import,
                    RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Import_ToolTip,
                    RingtoetsCommonFormsResources.ImportIcon, null);
            }

            if (nodeData is InputPlaceholder || nodeData is OutputPlaceholder)
            {
                contextMenu.AddMenuItem(
                    RingtoetsCommonFormsResources.FailureMechanism_Export,
                    RingtoetsCommonFormsResources.FailureMechanism_Export_ToolTip,
                    RingtoetsCommonFormsResources.ExportIcon, null);
                contextMenu.AddSeperator();

                contextMenu.AddMenuItem(
                    RingtoetsCommonFormsResources.FailureMechanism_Properties,
                    RingtoetsCommonFormsResources.FailureMechanism_Properties_ToolTip,
                    RingtoetsCommonFormsResources.PropertiesIcon, null);
            }
            return contextMenu;
        }
    }
}