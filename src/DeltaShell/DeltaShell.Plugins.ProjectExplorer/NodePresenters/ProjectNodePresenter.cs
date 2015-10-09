using System.Collections;
using System.ComponentModel;
using System.Drawing;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DelftTools.Shell.Gui.Swf;
using DeltaShell.Plugins.ProjectExplorer.Properties;

namespace DeltaShell.Plugins.ProjectExplorer.NodePresenters
{
    public class ProjectNodePresenter : TreeViewNodePresenterBaseForPluginGui<Project>
    {
        public ProjectNodePresenter(GuiPlugin guiPlugin) : base(guiPlugin)
        {
            GuiPlugin = guiPlugin;
        }

        public override bool CanRenameNode(ITreeNode node)
        {
            return false;
        }

        public override void OnNodeRenamed(Project project, string newName)
        {
            if (project.Name != newName)
            {
                project.Name = newName;
            }
        }

        public override IEnumerable GetChildNodeObjects(Project project, ITreeNode node)
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
            //small HACK, force update of name if path is changed, because name references path.
            if (e.PropertyName == "Name" || e.PropertyName == "Path")
            {
                node.Text = item.Name;
            }
        }
    }
}