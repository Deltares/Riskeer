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
    /// Tree node presenter representing the collection of <see cref="RingtoetsPipingSurfaceLine"/> available for piping
    /// calculations.
    /// </summary>
    public class PipingSurfaceLineCollectionNodePresenter : RingtoetsNodePresenterBase<IEnumerable<RingtoetsPipingSurfaceLine>>
    {
        /// <summary>
        /// Injects the action to be performed when importing <see cref="RingtoetsPipingSurfaceLine"/>
        /// instances to <see cref="PipingFailureMechanism.SurfaceLines"/>.
        /// </summary>
        public Action ImportSurfaceLinesAction { private get; set; }

        protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, IEnumerable<RingtoetsPipingSurfaceLine> nodeData)
        {
            node.Text = Resources.PipingSurfaceLinesCollection_DisplayName;
            node.ForegroundColor = nodeData.Any() ? Color.FromKnownColor(KnownColor.ControlText) : Color.FromKnownColor(KnownColor.GrayText);
            node.Image = Resources.FolderIcon;
        }

        protected override IEnumerable GetChildNodeObjects(IEnumerable<RingtoetsPipingSurfaceLine> nodeData, ITreeNode node)
        {
            foreach (var pipingSurfaceLine in nodeData)
            {
                yield return pipingSurfaceLine;
            }
        }

        protected override ContextMenuStrip GetContextMenu(ITreeNode sender, IEnumerable<RingtoetsPipingSurfaceLine> nodeData)
        {
            if (ImportSurfaceLinesAction != null)
            {
                return CreateContextMenu();
            }
            return null;
        }

        private ContextMenuStrip CreateContextMenu()
        {
            var rootMenu = new ContextMenuStrip();

            if (ImportSurfaceLinesAction != null)
            {
                rootMenu.AddMenuItem(Resources.Import_SurfaceLines, Resources.Import_SurfaceLines_Tooltip,
                                     Resources.ImportIcon, ImportItemOnClick);
            }

            return rootMenu;
        }

        private void ImportItemOnClick(object sender, EventArgs eventArgs)
        {
            ImportSurfaceLinesAction();
        }
    }
}