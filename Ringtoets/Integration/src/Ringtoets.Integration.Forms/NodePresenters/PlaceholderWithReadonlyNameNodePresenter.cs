using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Common.Placeholder;
using Ringtoets.Integration.Forms.ContextMenu;
using Ringtoets.Integration.Forms.Properties;

using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.NodePresenters
{
    /// <summary>
    /// Node presenter class for <see cref="PlaceholderWithReadonlyName"/>
    /// </summary>
    public class PlaceholderWithReadonlyNameNodePresenter : RingtoetsNodePresenterBase<PlaceholderWithReadonlyName>
    {
        private IGuiCommandHandler guiHandler;
        private IContextMenuProvider contextMenuProvider;

        public PlaceholderWithReadonlyNameNodePresenter(IContextMenuProvider contextMenuProvider, IGuiCommandHandler guiHandler = null)
        {
            this.guiHandler = guiHandler;
            this.contextMenuProvider = contextMenuProvider;
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
            return PlaceholderWithReadonlyNameContextMenu.CreateContextMenu(nodeData, guiHandler, contextMenuProvider);
        }
    }
}