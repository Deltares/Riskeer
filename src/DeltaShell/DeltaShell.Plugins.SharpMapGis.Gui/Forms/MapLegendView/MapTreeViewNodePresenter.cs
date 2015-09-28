using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DelftTools.Controls;
using DelftTools.Shell.Gui;
using DelftTools.Shell.Gui.Swf;
using SharpMap;
using SharpMap.Api.Layers;
using SharpMap.Layers;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms.MapLegendView
{
    public class MapTreeViewNodePresenter : TreeViewNodePresenterBaseForPluginGui<Map>
    {
        private static readonly Bitmap MapIcon = Properties.Resources.Map;

        public MapTreeViewNodePresenter(GuiPlugin guiPlugin) : base(guiPlugin)
        {
        }

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
                    newName = newName.Substring(0, indexOfCoordinateSystem);
            }

            if (map.Name != newName)
                map.Name = newName;
        }

        private static void SetNodeText(ITreeNode node, Map map)
        {
            if (map.CoordinateSystem != null)
                node.Text = string.Format("{0} ({1})", map.Name, map.CoordinateSystem.Name);
            else
                node.Text = map.Name;
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

        protected override void OnPropertyChanged(Map map, ITreeNode node, PropertyChangedEventArgs e)
        {
            if (node == null) return;

            if (e.PropertyName == "Name")
            {
                SetNodeText(node, map);
            }
        }

        public override IEnumerable GetChildNodeObjects(Map map, ITreeNode node)
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

                bool removed;
                if (sourceLayerGroup != null)
                {
                    removed = sourceLayerGroup.Layers.Remove(layer);
                }
                else
                {
                    removed = target.Layers.Remove(layer); // only changing position of layer within map
                }

                if (removed)
                {
                    target.Layers.Insert(position, layer);
                }
            }
        }
    }
}