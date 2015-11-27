using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
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
        private readonly IContextMenuBuilderProvider contextMenuBuilderProvider;

        public PlaceholderWithReadonlyNameNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider)
        {
            this.contextMenuBuilderProvider = contextMenuBuilderProvider;
        }

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
            ContextMenuBuilder menuBuilder = contextMenuBuilderProvider.Get(sender);

            if (nodeData is InputPlaceholder || nodeData is OutputPlaceholder)
            {
                var clearItem = new StrictContextMenuItem(
                    RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase,
                    RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase_ToolTip,
                    RingtoetsCommonFormsResources.ClearIcon,
                    null)
                {
                    Enabled = false
                };

                menuBuilder.AddOpenItem()
                           .AddCustomItem(clearItem)
                           .AddSeparator();
            }

            return menuBuilder.AddImportItem()
                              .AddExportItem()
                              .AddSeparator()
                              .AddPropertiesItem()
                              .Build();
        }
    }
}