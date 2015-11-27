using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls;

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
        /// Sets the action to be performed when importing <see cref="PipingSoilProfile"/> instances
        /// to <see cref="PipingFailureMechanism.SoilProfiles"/>.
        /// </summary>
        public Action ImportSoilProfilesAction { private get; set; }

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, IEnumerable<PipingSoilProfile> pipingSoilProfiles)
        {
            node.Text = Resources.PipingSoilProfilesCollection_DisplayName;
            node.Image = Resources.FolderIcon;

            node.ForegroundColor = GetTextColor(pipingSoilProfiles);
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
            if (ImportSoilProfilesAction != null)
            {
                return CreateContextMenu();
            }
            return null;
        }

        private static Color GetTextColor(object nodeData)
        {
            var pipingSoilProfiles = (IEnumerable<PipingSoilProfile>) nodeData;
            return Color.FromKnownColor(pipingSoilProfiles.Any() ? KnownColor.ControlText : KnownColor.GrayText);
        }

        private ContextMenuStrip CreateContextMenu()
        {
            var strip = new ContextMenuStrip();
            if (ImportSoilProfilesAction != null)
            {
                strip.AddMenuItem(
                    Resources.Import_SoilProfiles,
                    Resources.Import_SoilProfiles_Tooltip,
                    Resources.ImportIcon,
                    ImportSoilProfilesOnClick);
            }
            return strip;
        }

        private void ImportSoilProfilesOnClick(object sender, EventArgs e)
        {
            ImportSoilProfilesAction();
        }
    }
}