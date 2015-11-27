using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.Swf;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Map;
using Core.Plugins.SharpMapGis.Gui.Properties;

namespace Core.Plugins.SharpMapGis.Gui.Forms.MapLegendView
{
    public class MapTreeViewNodePresenter : TreeViewNodePresenterBaseForPluginGui<Map>
    {
        private static readonly Bitmap MapIcon = Resources.Map;

        public MapTreeViewNodePresenter(GuiPlugin guiPlugin) : base(guiPlugin) {}

        public override bool CanRenameNode(ITreeNode node)
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

        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, Map map)
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

        public override bool CanInsert(object item, ITreeNode sourceNode, ITreeNode targetNode)
        {
            return true;
        }

        public override DragOperations CanDrop(object item, ITreeNode sourceNode, ITreeNode targetNode, DragOperations validOperations)
        {
            if (item is Layer)
            {
                return GetDefaultDropOperation(TreeView, item, sourceNode, targetNode, validOperations);
            }

            return DragOperations.None;
        }

        public override void OnDragDrop(object item, object sourceParentNodeData, Map target, DragOperations operation, int position)
        {
            if (item is ILayer)
            {
                var sourceLayerGroup = sourceParentNodeData as GroupLayer;
                var layer = (ILayer) item;

                var removed = sourceLayerGroup != null ? sourceLayerGroup.Layers.Remove(layer) : target.Layers.Remove(layer);

                if (removed)
                {
                    target.Layers.Insert(position, layer);
                    target.NotifyObservers();
                }
            }
        }

        protected override void OnPropertyChanged(Map map, ITreeNode node, PropertyChangedEventArgs e)
        {
            if (node == null)
            {
                return;
            }

            if (e.PropertyName == "Name")
            {
                SetNodeText(node, map);
            }
        }

        private static void SetNodeText(ITreeNode node, Map map)
        {
            node.Text = map.CoordinateSystem != null ? string.Format("{0} ({1})", map.Name, map.CoordinateSystem.Name) : map.Name;
        }
    }
}