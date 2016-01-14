using System;
using System.Collections;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Map;
using TreeNode = Core.Common.Controls.TreeView.TreeNode;

namespace Core.Plugins.SharpMapGis.Gui.Forms.MapLegendView
{
    public class MapLayerTreeViewNodePresenter : TreeViewNodePresenterBase<ILayer>
    {
        private readonly MapLegendView contextMenuProvider;

        public MapLayerTreeViewNodePresenter(MapLegendView legend)
        {
            contextMenuProvider = legend;
        }

        public override bool CanRenameNode(TreeNode node)
        {
            var layer = (ILayer) node.Tag;

            return !layer.NameIsReadOnly;
        }

        public override void OnNodeRenamed(ILayer layer, string newName)
        {
            if (layer.Name != newName)
            {
                layer.Name = newName;
            }
        }

        public override void UpdateNode(TreeNode parentNode, TreeNode node, ILayer layer)
        {
            node.Tag = layer;
            node.Text = layer.Name;
            node.Checked = layer.Visible;
            node.ShowCheckBox = true;
        }

        public override IEnumerable GetChildNodeObjects(ILayer mapLayer)
        {
            var legend = new MapLayerLegend(mapLayer);

            if (legend.Theme != null)
            {
                foreach (var themeItem in legend.Theme.ThemeItems)
                {
                    yield return themeItem;
                }
            }
            else
            {
                var groupLayer = mapLayer as GroupLayer;
                if (groupLayer != null)
                {
                    foreach (var layer in groupLayer.Layers.Where(l => l.ShowInTreeView))
                    {
                        yield return layer;
                    }
                }
                else
                {
                    foreach (var style in legend.Styles)
                    {
                        yield return style;
                    }
                }
            }
        }

        public override void OnNodeChecked(TreeNode node)
        {
            var layer = (ILayer) node.Tag;

            if (layer.Visible != node.Checked)
            {
                layer.Visible = node.Checked;
            }
        }

        public override DragOperations CanDrag(ILayer nodeData)
        {
            var parentNode = TreeView.GetNodeByTag(nodeData).Parent;
            if (parentNode != null && parentNode.Tag is GroupLayer && ((GroupLayer) parentNode.Tag).LayersReadOnly)
            {
                return DragOperations.None; // No dragging of items in 'readonly' grouplayers
            }

            return DragOperations.Move;
        }

        public override bool CanInsert(object item, TreeNode sourceNode, TreeNode targetNode)
        {
            return (null == TreeView.TreeViewNodeSorter);
        }

        public override DragOperations CanDrop(object item, TreeNode sourceNode, TreeNode targetNode, DragOperations validOperations)
        {
            var groupLayer = targetNode.Tag as IGroupLayer;
            if (groupLayer != null && item is Layer)
            {
                return groupLayer.LayersReadOnly
                           ? DragOperations.None // No dropping into 'readonly' grouplayers
                           : DragOperations.Move;
            }

            return DragOperations.None; // No dropping into non grouplayers
        }

        public override void OnDragDrop(object item, object itemParent, ILayer target, DragOperations operation, int position)
        {
            if (TreeView == null)
            {
                throw new NullReferenceException("TreeView not assigned to foldernodepresenter");
            }

            var sourceMap = itemParent as Map;
            var sourceLayerGroup = itemParent as GroupLayer;
            var targetLayerGroup = (GroupLayer) target;

            if ((operation & DragOperations.Move) != 0)
            {
                if (sourceLayerGroup != null)
                {
                    sourceLayerGroup.Layers.Remove((ILayer) item);
                }
                else if (sourceMap != null)
                {
                    sourceMap.Layers.Remove((ILayer) item);
                    sourceMap.NotifyObservers();
                }
                else
                {
                    throw new NotSupportedException("Can not drag layer from the source node: " + item);
                }

                targetLayerGroup.Layers.Insert(position, (ILayer) item);
            }
        }

        public override ContextMenuStrip GetContextMenu(TreeNode node, object nodeData)
        {
            return contextMenuProvider.GetContextMenu(nodeData);
        }

        protected override bool CanRemove(ILayer nodeData)
        {
            var parentNode = TreeView.GetNodeByTag(nodeData).Parent;
            if (parentNode != null && parentNode.Tag is GroupLayer && ((GroupLayer) parentNode.Tag).LayersReadOnly)
            {
                return false; // No removing of items in 'readonly' grouplayers
            }

            if (!nodeData.CanBeRemovedByUser)
            {
                return false;
            }

            return true;
        }

        protected override bool RemoveNodeData(object parentNodeData, ILayer layer)
        {
            if (!layer.Map.Layers.Remove(layer))
            {
                return false;
            }

            if (layer is IDisposable)
            {
                var disposableLayer = layer as IDisposable;

                disposableLayer.Dispose();
            }

            return true;
        }
    }
}