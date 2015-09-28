using System.Collections.Generic;
using DelftTools.Controls;
using DelftTools.Shell.Gui;
using DelftTools.Shell.Gui.Swf;
using SharpMap.Layers;
using SharpMap.Web.Wms;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms.MapLegendView
{
    public class WmsMapLayerTreeViewNodePresenter : TreeViewNodePresenterBaseForPluginGui<Client.WmsServerLayer>
    {
        private IGisGuiService gisGuiService;
        private static WmsLayer currentWmsLayer;

        public WmsMapLayerTreeViewNodePresenter(GuiPlugin guiPlugin, IGisGuiService gisGuiService) : base(guiPlugin)
        {
            this.gisGuiService = gisGuiService;
        }


        // small hack
        public static WmsLayer CurrentWmsLayer
        {
            get { return currentWmsLayer; }
            set { currentWmsLayer = value; }
        }

        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, Client.WmsServerLayer layer)
        {
            node.Text = layer.Name;
            node.Tag = layer;
            node.Checked = currentWmsLayer.LayerList.Contains(layer.Name);
            node.ShowCheckBox = true;
        }
 
        public override void OnNodeChecked(ITreeNode node)
        {
            Client.WmsServerLayer layer = (Client.WmsServerLayer)node.Tag;
            if(node.Checked)
            {
                List<string> oldLayers = new List<string>(currentWmsLayer.LayerList);

                // TODO: add support for layer order when dragging will work
                currentWmsLayer.LayerList.Clear();
                foreach (Client.WmsServerLayer childLayer in currentWmsLayer.RootLayer.ChildLayers)
                {
                    if(childLayer.Name.Equals(layer.Name) || oldLayers.Contains(childLayer.Name))
                    {
                        currentWmsLayer.LayerList.Add(childLayer.Name);
                    }
                }
            }
            else
            {
                currentWmsLayer.LayerList.Remove(layer.Name);
            }

            currentWmsLayer.RenderRequired = true;
            gisGuiService.RefreshMapView(currentWmsLayer.Map);
        }
    }
}