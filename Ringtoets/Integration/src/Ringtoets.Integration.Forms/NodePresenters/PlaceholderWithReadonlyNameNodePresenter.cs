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
                var openItem = new ToolStripMenuItem
                {
                    Text = RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Open,
                    ToolTipText = RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Open_ToolTip,
                    Image = RingtoetsCommonFormsResources.OpenIcon,
                    Enabled = false
                };
                var clearItem = new ToolStripMenuItem
                {
                    Text = RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase,
                    ToolTipText = RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase_ToolTip,
                    Image = RingtoetsCommonFormsResources.ClearIcon,
                    Enabled = false
                };

                menuBuilder.AddCustomItem(openItem)
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