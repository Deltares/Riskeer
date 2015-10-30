using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Utils.Collections;
using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.Extensions;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// Tree node presenter representing the collection of <see cref="RingtoetsPipingSurfaceLine"/> available for piping
    /// calculations.
    /// </summary>
    public class PipingSurfaceLineCollectionNodePresenter : ITreeNodePresenter
    {
        /// <summary>
        /// Injects the action to be performed when importing <see cref="RingtoetsPipingSurfaceLine"/>
        /// instances to <see cref="PipingFailureMechanism.SurfaceLines"/>.
        /// </summary>
        public Action ImportSurfaceLinesAction { private get; set; }

        public ITreeView TreeView { get; set; }

        public Type NodeTagType
        {
            get
            {
                return typeof(IEnumerable<RingtoetsPipingSurfaceLine>);
            }
        }

        public void UpdateNode(ITreeNode parentNode, ITreeNode node, object nodeData)
        {
            var data = (IEnumerable<RingtoetsPipingSurfaceLine>)nodeData;
            node.Text = Resources.PipingSurfaceLinesCollection_DisplayName;
            node.ForegroundColor = data.Any() ? Color.FromKnownColor(KnownColor.ControlText) : Color.FromKnownColor(KnownColor.GrayText);
            node.Image = Resources.FolderIcon;
        }

        public IEnumerable GetChildNodeObjects(object parentNodeData, ITreeNode node)
        {
            var surfaceLines = (IEnumerable<RingtoetsPipingSurfaceLine>) parentNodeData;
            foreach (var pipingSurfaceLine in surfaceLines)
            {
                yield return pipingSurfaceLine;
            }
        }

        public bool CanRenameNode(ITreeNode node)
        {
            return false;
        }

        public bool CanRenameNodeTo(ITreeNode node, string newName)
        {
            return false;
        }

        public void OnNodeRenamed(object nodeData, string newName)
        {
            throw new InvalidOperationException(string.Format("Cannot rename tree node of type {0}.", GetType().Name));
        }

        public void OnNodeChecked(ITreeNode node) {}

        public DragOperations CanDrag(object nodeData)
        {
            return DragOperations.None;
        }

        public DragOperations CanDrop(object item, ITreeNode sourceNode, ITreeNode targetNode, DragOperations validOperations)
        {
            return DragOperations.None;
        }

        public bool CanInsert(object item, ITreeNode sourceNode, ITreeNode targetNode)
        {
            return false;
        }

        public void OnDragDrop(object item, object sourceParentNodeData, object targetParentNodeData, DragOperations operation, int position) {}

        public void OnNodeSelected(object nodeData) {}

        public ContextMenuStrip GetContextMenu(ITreeNode sender, object nodeData)
        {
            if (ImportSurfaceLinesAction != null)
            {
                return CreateContextMenu();
            }
            return null;
        }

        public void OnPropertyChanged(object sender, ITreeNode node, PropertyChangedEventArgs e) {}

        public void OnCollectionChanged(object sender, NotifyCollectionChangingEventArgs e) {}

        public bool CanRemove(object parentNodeData, object nodeData)
        {
            return false;
        }

        public bool RemoveNodeData(object parentNodeData, object nodeData)
        {
            throw new InvalidOperationException(String.Format("Cannot delete node of type {0}.", GetType().Name));
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