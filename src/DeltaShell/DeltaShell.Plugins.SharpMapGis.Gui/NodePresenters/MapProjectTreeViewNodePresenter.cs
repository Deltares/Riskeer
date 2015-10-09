using System.Drawing;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui.Swf;
using DeltaShell.Plugins.SharpMapGis.Gui.Properties;
using SharpMap;
using SharpMap.Api.Layers;

namespace DeltaShell.Plugins.SharpMapGis.Gui.NodePresenters
{
    public class MapProjectTreeViewNodePresenter : TreeViewNodePresenterBaseForPluginGui<Map>
    {
        private static readonly Bitmap MapIcon = Resources.Map;

        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, Map map)
        {
            node.Text = map.Name;
            node.Image = MapIcon;
        }

        public override bool CanRenameNode(ITreeNode node)
        {
            return true;
        }

        public override DragOperations CanDrag(Map nodeData)
        {
            return DragOperations.Move;
        }

        public override DragOperations CanDrop(object item, ITreeNode sourceNode, ITreeNode targetNode, DragOperations validOperations)
        {
            if (item is ILayer)
            {
                return GetDefaultDropOperation(TreeView, item, sourceNode, targetNode, validOperations);
            }

            return DragOperations.None;
        }

        public override void OnDragDrop(object item, object sourceParentNodeData, Map target, DragOperations operation, int position)
        {
            if (target == null)
            {
                return;
            }

            if (item is ILayer)
            {
                var sourceMap = sourceParentNodeData as Map;
                IGroupLayer sourceGroupLayer = null;
                if (sourceMap == null)
                {
                    sourceGroupLayer = sourceParentNodeData as IGroupLayer;
                }

                var layer = (ILayer) item;

                target.Layers.Remove(layer); // only changing position of layer within map
                target.Layers.Insert(position, layer);
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