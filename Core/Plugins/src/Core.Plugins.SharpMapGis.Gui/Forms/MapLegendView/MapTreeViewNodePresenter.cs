using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Map;
using Core.Plugins.SharpMapGis.Gui.Properties;
using TreeNode = Core.Common.Controls.TreeView.TreeNode;

namespace Core.Plugins.SharpMapGis.Gui.Forms.MapLegendView
{
    public class MapTreeViewNodePresenter : TreeViewNodePresenterBase<Map>
    {
        private static readonly Bitmap MapIcon = Resources.Map;

        private readonly MapLegendView contextMenuProvider;

        public MapTreeViewNodePresenter(MapLegendView mapLegend)
        {
            contextMenuProvider = mapLegend;
        }

        public override bool CanRenameNode(TreeNode node)
        {
            return true;
        }

        public override void OnNodeRenamed(Map map, string newName)
        {
            if (map.CoordinateSystem != null)
            {
                var indexOfCoordinateSystem = newName.IndexOf(string.Format(" ({0})", map.CoordinateSystem.Name),
                                                              StringComparison.InvariantCulture);
                if (indexOfCoordinateSystem >= 0)
                {
                    newName = newName.Substring(0, indexOfCoordinateSystem);
                }
            }

            if (map.Name != newName)
            {
                map.Name = newName;
            }
        }

        public override void UpdateNode(TreeNode parentNode, TreeNode node, Map map)
        {
            SetNodeText(node, map);
            node.Image = MapIcon;

            if (!node.IsLoaded)
            {
                node.Expand();
                foreach (var subNode in node.Nodes)
                {
                    subNode.Expand();
                }
            }
        }

        public override IEnumerable GetChildNodeObjects(Map map)
        {
            return map.Layers.Where(l => l.ShowInTreeView);
        }

        public override bool CanInsert(object item, TreeNode sourceNode, TreeNode targetNode)
        {
            return true;
        }

        public override DragOperations CanDrop(object item, TreeNode sourceNode, TreeNode targetNode, DragOperations validOperations)
        {
            if (item is Layer)
            {
                return GetDefaultDropOperation(validOperations);
            }

            return DragOperations.None;
        }

        public override void OnDragDrop(object item, object itemParent, Map target, DragOperations operation, int position)
        {
            if (item is ILayer)
            {
                var sourceLayerGroup = itemParent as GroupLayer;
                var layer = (ILayer) item;

                var removed = sourceLayerGroup != null ? sourceLayerGroup.Layers.Remove(layer) : target.Layers.Remove(layer);

                if (removed)
                {
                    target.Layers.Insert(position, layer);
                    target.NotifyObservers();
                }
            }
        }

        public override ContextMenuStrip GetContextMenu(TreeNode node, object nodeData)
        {
            return contextMenuProvider.GetContextMenu(nodeData);
        }

        private static void SetNodeText(TreeNode node, Map map)
        {
            node.Text = map.CoordinateSystem != null ? string.Format("{0} ({1})", map.Name, map.CoordinateSystem.Name) : map.Name;
        }
    }
}