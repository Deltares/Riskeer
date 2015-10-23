using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Utils.Collections;
using Wti.Data;
using Wti.Forms.Extensions;
using Wti.Forms.Properties;

namespace Wti.Forms.NodePresenters
{
    /// <summary>
    /// Tree node presenter representing the collection of <see cref="PipingSoilProfile"/> available for piping
    /// calculations.
    /// </summary>
    public class PipingSoilProfileCollectionNodePresenter : ITreeNodePresenter
    {
        /// <summary>
        /// Sets the action to be performed when importing <see cref="PipingSoilProfile"/> instances
        /// to <see cref="PipingFailureMechanism.SoilProfiles"/>.
        /// </summary>
        public Action ImportSoilProfilesAction { private get; set; }

        public ITreeView TreeView { get; set; }

        public Type NodeTagType
        {
            get
            {
                return typeof(IEnumerable<PipingSoilProfile>);
            }
        }

        public void UpdateNode(ITreeNode parentNode, ITreeNode node, object nodeData)
        {
            node.Text = Resources.PipingSoilProfilesCollectionName;
            node.ForegroundColor = Color.FromKnownColor(KnownColor.GrayText);
            node.Image = Resources.FolderIcon;
        }

        public IEnumerable GetChildNodeObjects(object parentNodeData, ITreeNode node)
        {
            var pipingSoilProfiles = (IEnumerable<PipingSoilProfile>) parentNodeData;

            foreach (PipingSoilProfile profile in pipingSoilProfiles)
            {
                yield return profile;
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
            if (ImportSoilProfilesAction != null)
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
            var strip = new ContextMenuStrip();
            if (ImportSoilProfilesAction != null)
            {
                strip.AddMenuItem(
                    Resources.ImportSoilProfiles,
                    Resources.ImportSoilProfilesDescription,
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