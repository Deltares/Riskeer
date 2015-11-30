using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui;
using Ringtoets.Common.Forms.Extensions;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// Tree node presenter representing the collection of <see cref="PipingSoilProfile"/> available for piping
    /// calculations.
    /// </summary>
    public class PipingSoilProfileCollectionNodePresenter : RingtoetsNodePresenterBase<IEnumerable<PipingSoilProfile>>
    {
        /// <summary>
        /// Sets the <see cref="IContextMenuBuilderProvider"/> to be used for creating the <see cref="ContextMenuStrip"/>.
        /// </summary>
        public IContextMenuBuilderProvider ContextMenuBuilderProvider { private get; set; }

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, IEnumerable<PipingSoilProfile> pipingSoilProfiles)
        {
            node.Text = Resources.PipingSoilProfilesCollection_DisplayName;
            node.Image = Resources.FolderIcon;

            node.ForegroundColor = Color.FromKnownColor(pipingSoilProfiles.Any() ? KnownColor.ControlText : KnownColor.GrayText);
        }

        protected override IEnumerable GetChildNodeObjects(IEnumerable<PipingSoilProfile> pipingSoilProfiles)
        {
            foreach (PipingSoilProfile profile in pipingSoilProfiles)
            {
                yield return profile;
            }
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode sender, IEnumerable<PipingSoilProfile> nodeData)
        {
            if (ContextMenuBuilderProvider == null)
            {
                return null;
            }
            return ContextMenuBuilderProvider
                .Get(sender)
                .AddExpandAllItem()
                .AddCollapseAllItem()
                .AddSeparator()
                .AddImportItem()
                .AddExportItem()
                .Build();
        }
    }
}