using System.Collections;
using System.ComponentModel;
using System.Drawing;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.Swf;
using Core.Plugins.ProjectExplorer.Properties;

namespace Core.Plugins.ProjectExplorer.NodePresenters
{
    public class ProjectNodePresenter : TreeViewNodePresenterBaseForPluginGui<Project>
    {
        public ProjectNodePresenter(GuiPlugin guiPlugin) : base(guiPlugin)
        {
            GuiPlugin = guiPlugin;
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