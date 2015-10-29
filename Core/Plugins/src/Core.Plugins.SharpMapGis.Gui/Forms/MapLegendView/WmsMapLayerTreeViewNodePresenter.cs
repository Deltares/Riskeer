using System.Collections.Generic;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.Swf;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Web.Wms;

namespace Core.Plugins.SharpMapGis.Gui.Forms.MapLegendView
{
    public class WmsMapLayerTreeViewNodePresenter : TreeViewNodePresenterBaseForPluginGui<Client.WmsServerLayer>
    {
        private readonly IGisGuiService gisGuiService;

        public WmsMapLayerTreeViewNodePresenter(GuiPlugin guiPlugin, IGisGuiService gisGuiService) : base(guiPlugin)
        {
            this.gisGuiService = gisGuiService;
        }

        // small hack
        public static WmsLayer CurrentWmsLayer { get; set; }

        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, Client.WmsServerLayer layer)
        {
            node.Text = layer.Name;
            node.Tag = layer;
            node.Checked = CurrentWmsLayer.LayerList.Contains(layer.Name);
            node.ShowCheckBox = true;
        }

        public override void OnNodeChecked(ITreeNode node)
        {
            Client.WmsServerLayer layer = (Client.WmsServerLayer) node.Tag;
            if (node.Checked)
            {
                List<string> oldLayers = new List<string>(CurrentWmsLayer.LayerList);

                // TODO: add support for layer order when dragging will work
                CurrentWmsLayer.LayerList.Clear();
                foreach (Client.WmsServerLayer childLayer in CurrentWmsLayer.RootLayer.ChildLayers)
                {
                    if (childLayer.Name.Equals(layer.Name) || oldLayers.Contains(childLayer.Name))
                    {
                        CurrentWmsLayer.LayerList.Add(childLayer.Name);
                    }
                }
            }
            else
            {
                CurrentWmsLayer.LayerList.Remove(layer.Name);
            }

            CurrentWmsLayer.RenderRequired = true;
            gisGuiService.RefreshMapView(CurrentWmsLayer.Map);
        }
    }
}