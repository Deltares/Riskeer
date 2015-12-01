using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Plugins.ProjectExplorer.Properties;

namespace Core.Plugins.ProjectExplorer.NodePresenters
{
    public class ProjectNodePresenter : TreeViewNodePresenterBase<Project>
    {
        /// <summary>
        /// Sets the <see cref="IContextMenuBuilderProvider"/> to be used for creating the <see cref="ContextMenuStrip"/>.
        /// </summary>
        public IContextMenuBuilderProvider ContextMenuBuilderProvider { private get; set; }

        /// <summary>
        /// Sets the <see cref="IGuiCommandHandler"/> to be used for binding items to actions in the <see cref="ContextMenuStrip"/>.
        /// </summary>
        public IGuiCommandHandler CommandHandler { private get; set; }

        public override IEnumerable GetChildNodeObjects(Project project)
        {
            return project.Items;
        }

        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, Project project)
        {
            node.Text = project.Name;
            Image image = Resources.Project;
            node.Image = image;

            node.Tag = project;
        }

        public override ContextMenuStrip GetContextMenu(ITreeNode sender, object nodeData)
        {
            if (ContextMenuBuilderProvider == null)
            {
                return null;
            }
            var addItem = new StrictContextMenuItem(
                Resources.AddItem,
                null,
                Resources.plus,
                (s, e) => CommandHandler.AddNewItem(nodeData));

            return ContextMenuBuilderProvider
                .Get(sender)
                .AddCustomItem(addItem)
                .AddSeparator()
                .AddExpandAllItem()
                .AddCollapseAllItem()
                .AddSeparator()
                .AddExportItem()
                .AddImportItem()
                .AddSeparator()
                .AddPropertiesItem()
                .Build();
        }

        public override DragOperations CanDrag(Project nodeData)
        {
            return DragOperations.None;
        }

        public override DragOperations CanDrop(object item, ITreeNode sourceNode, ITreeNode targetNode, DragOperations validOperations)
        {
            return GetDefaultDropOperation(TreeView, item, sourceNode, targetNode, validOperations);
        }

        public override void OnDragDrop(object item, object sourceParentNodeData, Project target, DragOperations operation, int position)
        {
            if ((operation & DragOperations.Move) != 0)
            {
                // Remove the item from the parent project
                var parentProject = sourceParentNodeData as Project;
                if (parentProject != null)
                {
                    parentProject.Items.Remove(item);
                }

                // Insert the item into the project
                target.Items.Insert(position, item);
            }
        }

        protected override void OnPropertyChanged(Project item, ITreeNode node, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                node.Text = item.Name;
            }
        }
    }
}