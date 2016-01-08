using System.Drawing;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.Map;
using Core.Plugins.SharpMapGis.Gui.Properties;

namespace Core.Plugins.SharpMapGis.Gui.NodePresenters
{
    public class MapProjectTreeViewNodePresenter : TreeViewNodePresenterBase<Map>
    {
        private static readonly Bitmap MapIcon = Resources.Map;

        public override void UpdateNode(TreeNode parentNode, TreeNode node, Map map)
        {
            node.Text = map.Name;
            node.Image = MapIcon;
        }

        public override bool CanRenameNode(TreeNode node)
        {
            return true;
        }

        public override DragOperations CanDrag(Map nodeData)
        {
            return DragOperations.Move;
        }

        public override bool CanInsert(object item, TreeNode sourceNode, TreeNode targetNode)
        {
            return (null == TreeView.TreeViewNodeSorter);
        }

        public override DragOperations CanDrop(object item, TreeNode sourceNode, TreeNode targetNode, DragOperations validOperations)
        {
            if (item is ILayer)
            {
                return GetDefaultDropOperation(validOperations);
            }

            return DragOperations.None;
        }

        public override void OnDragDrop(object item, object itemParent, Map target, DragOperations operation, int position)
        {
            if (target == null)
            {
                return;
            }

            if (item is ILayer)
            {
                var layer = (ILayer) item;

                target.Layers.Remove(layer); // only changing position of layer within map
                target.Layers.Insert(position, layer);
                target.NotifyObservers();
            }
        }

        public override void OnNodeRenamed(Map map, string newName)
        {
            if (map.Name != newName)
            {
                map.Name = newName;
            }
        }

        protected override bool CanRemove(Map nodeData)
        {
            return true;
        }

        protected override bool RemoveNodeData(object parentNodeData, Map nodeData)
        {
            var project = parentNodeData as Project;
            if (project != null)
            {
                project.Items.Remove(nodeData);
                project.NotifyObservers();

                return true;
            }

            return false;
        }
    }
}