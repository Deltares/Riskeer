using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Controls.Swf;
using DelftTools.Utils.Collections;
using Wti.Data;
using Wti.Forms.Extensions;
using Wti.Forms.Properties;

namespace Wti.Forms.NodePresenters
{
    /// <summary>
    /// Tree node presenter representing the collection of <see cref="PipingSurfaceLine"/> available for piping
    /// calculations.
    /// </summary>
    public class PipingSurfaceLineCollectionNodePresenter : ITreeNodePresenter
    {
        /// <summary>
        /// Injects the action to be performed when importing <see cref="PipingSurfaceLine"/>
        /// instances to <see cref="PipingFailureMechanism.SurfaceLines"/>.
        /// </summary>
        public Action ImportSurfaceLinesAction { private get; set; }

        public ITreeView TreeView { get; set; }

        public Type NodeTagType
        {
            get
            {
                return typeof(IEnumerable<PipingSurfaceLine>);
            }
        }

        public void UpdateNode(ITreeNode parentNode, ITreeNode node, object nodeData)
        {
            var data = (IEnumerable<PipingSurfaceLine>)nodeData;
            node.Text = Resources.PipingSurfaceLinesCollectionName;
            node.ForegroundColor = data.Any() ? Color.FromKnownColor(KnownColor.ControlText) : Color.FromKnownColor(KnownColor.GrayText);
            node.Image = Resources.FolderIcon;
        }

        public IEnumerable GetChildNodeObjects(object parentNodeData, ITreeNode node)
        {
            var surfaceLines = (IEnumerable<PipingSurfaceLine>) parentNodeData;
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

        public IMenuItem GetContextMenu(ITreeNode sender, object nodeData)
        {
            if (ImportSurfaceLinesAction != null)
            {
                return new MenuItemContextMenuStripAdapter(CreateContextMenu());
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
                rootMenu.AddMenuItem(Resources.ImportSurfaceLines, Resources.ImportSurfaceLinesDescription,
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