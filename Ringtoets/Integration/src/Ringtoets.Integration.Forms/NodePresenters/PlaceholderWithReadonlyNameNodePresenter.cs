using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls.Swf.TreeViewControls;
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
        /// <summary>
        /// Creates a new instance of <see cref="PlaceholderWithReadonlyNameNodePresenter"/>, which uses the 
        /// <paramref name="contextMenuBuilderProvider"/> to create and bind its <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> 
        /// to use for  building a <see cref="ContextMenuStrip"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="contextMenuBuilderProvider"/> was provided.</exception>
        public PlaceholderWithReadonlyNameNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) { }

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, PlaceholderWithReadonlyName nodeData)
        {
            node.Text = nodeData.Name;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.GrayText);
            node.Image = GetIconForPlaceholder(nodeData);
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode node, PlaceholderWithReadonlyName nodeData)
        {
            IContextMenuBuilder menuBuilder = contextMenuBuilderProvider.Get(node);

            if (nodeData is InputPlaceholder || nodeData is OutputPlaceholder)
            {
                menuBuilder.AddOpenItem();
            }
            
            if (nodeData is OutputPlaceholder)
            {
                var clearItem = new StrictContextMenuItem(
                    RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase,
                    RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase_ToolTip,
                    RingtoetsCommonFormsResources.ClearIcon,
                    null)
                {
                    Enabled = false
                };

                menuBuilder.AddCustomItem(clearItem);
            }

            if (nodeData is InputPlaceholder || nodeData is OutputPlaceholder)
            {
                menuBuilder.AddSeparator();
            }
            return menuBuilder.AddImportItem()
                              .AddExportItem()
                              .AddSeparator()
                              .AddPropertiesItem()
                              .Build();
        }

        private static Bitmap GetIconForPlaceholder(PlaceholderWithReadonlyName nodeData)
        {
            if (nodeData is InputPlaceholder || nodeData is OutputPlaceholder)
            {
                return Resources.GenericInputOutputIcon;
            }
            return Resources.PlaceholderIcon;
        }
    }
}