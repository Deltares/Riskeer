using System.Collections;
using DelftTools.Controls;
using DelftTools.Shell.Gui;
using DelftTools.Shell.Gui.Swf;
using SharpMap.Api.Layers;
using SharpMap.Layers;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms.MapLegendView
{
    public class WmsMapLayerGroupTreeViewNodePresenter : TreeViewNodePresenterBaseForPluginGui<WmsLayer>
    {
        private readonly IGisGuiService gisGuiService;

        public WmsMapLayerGroupTreeViewNodePresenter(GuiPlugin guiPlugin, IGisGuiService gisGuiService) : base(guiPlugin)
        {
            this.gisGuiService = gisGuiService;
        }

        public override bool CanRenameNode(ITreeNode node)
        {
            ILayer layer = (ILayer) node.Tag;
            return !layer.NameIsReadOnly;
        }

        public override void OnNodeRenamed(WmsLayer layer, string newName)
        {
            if (layer.Name != newName)
            {
                layer.Name = newName;
            }
        }

        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, WmsLayer layer)
        {
            node.Text = layer.RootLayer.Name;
            node.Checked = layer.Visible;
            node.ShowCheckBox = true;
        }

        public override IEnumerable GetChildNodeObjects(WmsLayer layer, ITreeNode node)
        {
            WmsMapLayerTreeViewNodePresenter.CurrentWmsLayer = layer;
            return new ArrayList(layer.RootLayer.ChildLayers).ToArray();
        }

        public override void OnNodeChecked(ITreeNode node)
        {
            ILayer layer = (ILayer) node.Tag;
            layer.Visible = node.Checked;
            gisGuiService.RefreshMapView(layer.Map);
        }

        public override DragOperations CanDrag(WmsLayer nodeData)
        {
            return DragOperations.Move;
        }

        protected override bool CanRemove(WmsLayer nodeData)
        {
            return true;
        }
    }
}