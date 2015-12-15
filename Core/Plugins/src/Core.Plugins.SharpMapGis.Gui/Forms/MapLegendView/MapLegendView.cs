using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Controls.Swf;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui;
using Core.GIS.GeoAPI.CoordinateSystems;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.Extensions.Layers;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Map;
using Core.GIS.SharpMap.Web.Wms;
using Core.Plugins.SharpMapGis.Gui.Commands;
using Core.Plugins.SharpMapGis.Gui.Properties;
using log4net;

namespace Core.Plugins.SharpMapGis.Gui.Forms.MapLegendView
{
    public partial class MapLegendView : UserControl, IView
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MapLegendView));
        private readonly IGui gui;
        private readonly IGisGuiService gisService;

        public Action<ILayer> OnOpenLayerAttributeTable = layer => { };

        private Map map;
        private bool disableGuiSelectionSync;

        public MapLegendView(IGui gui)
        {
            InitializeComponent();

            var guiPlugin = gui.Plugins.OfType<SharpMapGisGuiPlugin>().FirstOrDefault(); // this is bad, refactor it to depend only on simple interfaces
            gisService = guiPlugin.GisGuiService;

            this.gui = gui;

            var mapTreeViewNodePresenter = new MapTreeViewNodePresenter(guiPlugin);
            var mapLayerTreeViewNodePresenter = new MapLayerTreeViewNodePresenter(guiPlugin);
            var mapLayerLegendStyleTreeViewNodePresenter = new VectorStyleTreeViewNodePresenter();
            var themeItemTreeViewNodePresenter = new ThemeItemTreeViewNodePresenter(guiPlugin);

            TreeView.RegisterNodePresenter(mapTreeViewNodePresenter);
            TreeView.RegisterNodePresenter(mapLayerTreeViewNodePresenter);
            TreeView.RegisterNodePresenter(mapLayerLegendStyleTreeViewNodePresenter);
            TreeView.RegisterNodePresenter(themeItemTreeViewNodePresenter);

            TreeView.SelectedNodeChanged += TreeViewSelectedNodeChanged;
            TreeView.NodeMouseClick += TreeViewNodeMouseClick;
            TreeView.DoubleClick += TreeViewDoubleClick;
            UpdateButtonsVisibility();

            Image = Resources.Map;
        }

        public Map Map
        {
            private get
            {
                return map;
            }
            set
            {
                if (value == map)
                {
                    return;
                }

                map = value;

                disableGuiSelectionSync = true;

                TreeView.Data = map;

                disableGuiSelectionSync = false;

                if (map != null)
                {
                    UpdateButtonsVisibility();
                }
            }
        }

        public object Data
        {
            get
            {
                return Map;
            }
            set
            {
                SetMap(value);
            }
        }

        public Image Image { get; set; }

        public ViewInfo ViewInfo { get; set; }

        public ContextMenuStrip GetContextMenu(object nodeTag)
        {
            if (nodeTag is Layer)
            {
                return contextMenuLayer;
            }
            if (nodeTag is Client.WmsServerLayer)
            {
                return contextMenuWmsLayer;
            }
            if (nodeTag is Map)
            {
                return contextMenuMap;
            }

            return null;
        }

        public void OpenFiles(IEnumerable<string> files)
        {
            if (map == null)
            {
                return;
            }
            foreach (var file in files)
            {
                AddLayerFromFile(file);
            }
        }

        public static void HideAllLayersButThisOne(ILayer layer, Map map)
        {
            //find the grouplayer in which the layer resides :
            var groupLayer = map.GetGroupLayerContainingLayer(layer);
            if (groupLayer != null)
            {
                foreach (var l in groupLayer.Layers.Where(l => l != layer))
                {
                    l.Visible = false;
                }
            }
            //no grouplayer found so the layer must be on top lavel
            else
            {
                if (!map.Layers.Contains(layer))
                {
                    throw new InvalidOperationException(string.Format("Layer {0} not part of map {1}", layer.Name, map.Name));
                }
                foreach (var l in map.Layers.Where(l => l != layer))
                {
                    l.Visible = false;
                }
            }
            layer.Visible = true;
        }

        public void EnsureVisible(object item)
        {
            var layer = item as ILayer;
            if (layer == null)
            {
                return;
            }

            var node = TreeView.GetNodeByTag(layer, false);
            if (node == null)
            {
                return;
            }

            node.EnsureVisible();

            disableGuiSelectionSync = true;
            TreeView.SelectedNode = node;
            disableGuiSelectionSync = false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.Insert))
            {
                ButtonAddLayerClick(this, null);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void TreeViewDoubleClick(object sender, EventArgs e)
        {
            var layer = TreeView.SelectedNode.Tag as ILayer;
            if (layer == null)
            {
                return;
            }

            OnOpenLayerAttributeTable(layer);
        }

        private void TreeViewSelectedNodeChanged(object sender, EventArgs e)
        {
            if (disableGuiSelectionSync)
            {
                return;
            }

            var selectedNode = TreeView.SelectedNode;
            gui.Selection = selectedNode != null ? selectedNode.Tag : null;
            UpdateButtonsVisibility();
        }

        private void UpdateButtonsVisibility()
        {
            var node = TreeView.SelectedNode != null
                           ? TreeView.GetNodeByTag(TreeView.SelectedNode.Tag)
                           : null;

            var layer = TreeView.SelectedNode != null ? TreeView.SelectedNode.Tag as ILayer : null;

            buttonAddLayer.Enabled = map != null;
            buttonAddWmsLayer.Enabled = map != null;
            buttonRemoveLayer.Enabled = TreeView.SelectedNode != null
                                        && layer != null
                                        && !(node != null && node.Parent.Tag is GroupLayer && ((GroupLayer) node.Parent.Tag).LayersReadOnly);

            contextMenuLayerOpenAttributeTable.Enabled = layer != null && layer.ShowAttributeTable;
            // TODO: Only enable export layer button when node is group layer, but also contains vector layer at some depth (group layer within group layer that contains a vector layer for example)
        }

        private void SetMap(object value)
        {
            Map = (Map) value;
            UpdateButtonsVisibility();
        }

        private void ButtonAddLayerClick(object sender, EventArgs e)
        {
            AddLayer();
        }

        private void AddLayer()
        {
            var addLayerCommand = new MapAddLayerCommand();
            addLayerCommand.Execute();
            UpdateButtonsVisibility();
        }

        private void AddLayerFromFile(string path)
        {
            Log.DebugFormat("Adding file to current map as a layer: {0}", path);

            var addLayerCommand = new MapAddLayerCommand();
            addLayerCommand.AddLayerFromFile(path, map);
        }

        private void ButtonAddWmsLayerClick(object sender, EventArgs e)
        {
            var openUrlDialog = new OpenUrlDialog(gui.MainWindow)
            {
                Urls = new[]
                {
                    "http://openstreetmap.org",
                    "Bing Maps - Aerial",
                    "Bing Maps - Hybrid",
                    "Bing Maps - Roads",
                    "http://geoservices.rijkswaterstaat.nl/actueel_hoogtebestand_nl?",
                    "http://geoservices.rijkswaterstaat.nl/noordzee_bathymetry_in_lat_raster?",
                    "http://geoservices.rijkswaterstaat.nl/primaire_waterkeringen_rws?",
                    "http://geoservices.rijkswaterstaat.nl/hoogte_platen?",
                    "http://geoservices.rijkswaterstaat.nl/kusthoogte?",
                    "http://geoservices.rijkswaterstaat.nl/hoogte_krib?",
                    "http://geoservices.rijkswaterstaat.nl/ijsselmeergebied_diepte?",
                    "http://geoservices.rijkswaterstaat.nl/zd_bodemhoogte_zeeland_rd?",
                    // see aso http://geoservices.rijkswaterstaat.nl/services-index.html
                },
                Url = "http://openstreetmap.org"
            };

            if (openUrlDialog.ShowDialog() == DialogResult.OK)
            {
                AddLayerFromExternalSource(openUrlDialog.Url);
            }
        }

        private void AddLayerFromExternalSource(string url)
        {
            try
            {
                var csFactory = Map.CoordinateSystemFactory;

                if (url.Contains("openstreetmap"))
                {
                    var layer = new OpenStreetMapLayer
                    {
                        Name = "Open Street Map"
                    };
                    AddWmsLayerToMapAndSyncCoordinateSystem(layer, csFactory.CreateFromEPSG(3857)); // webmercator
                }
                else if (url.Contains("Bing Maps - Aerial"))
                {
                    var layer = new BingLayer
                    {
                        Name = "Bing Maps - Aerial", MapType = "Aerial"
                    };
                    AddWmsLayerToMapAndSyncCoordinateSystem(layer, csFactory.CreateFromEPSG(3857)); // webmercator
                }
                else if (url.Contains("Bing Maps - Hybrid"))
                {
                    var layer = new BingLayer
                    {
                        Name = "Bing Maps - Hybrid", MapType = "Hybrid"
                    };
                    AddWmsLayerToMapAndSyncCoordinateSystem(layer, csFactory.CreateFromEPSG(3857)); // webmercator
                }
                else if (url.Contains("Bing Maps - Roads"))
                {
                    var layer = new BingLayer
                    {
                        Name = "Bing Maps - Roads", MapType = "Roads"
                    };
                    AddWmsLayerToMapAndSyncCoordinateSystem(layer, csFactory.CreateFromEPSG(3857)); // webmercator
                }
                else
                {
                    var layer = WmscLayer.CreateWmsLayersFromUrl(url);
                    AddWmsLayerToMapAndSyncCoordinateSystem(layer, csFactory.CreateFromEPSG(3857));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
        }

        // HACK: single responsibility!!!
        private void AddWmsLayerToMapAndSyncCoordinateSystem(ILayer layer, ICoordinateSystem layerCoordinateSystem)
        {
            if (layerCoordinateSystem != null)
            {
                if (map.CoordinateSystem == null) // auto-set
                {
                    map.CoordinateSystem = layerCoordinateSystem;
                    map.ZoomToExtents(); // reset zoom (before adding wms layer)
                }
                else if (map.CoordinateSystem.AuthorityCode != layerCoordinateSystem.AuthorityCode)
                {
                    var message = string.Format(
                        "The layer being added has a different coordinate system ({0}) than the current map coordinate system ({1}).{2}{2}Would you like to adjust the map coordinate system?",
                        layerCoordinateSystem.Name, map.CoordinateSystem.Name, Environment.NewLine);

                    var res = MessageBox.Show(message, "Adjust map coordinate system?", MessageBoxButtons.YesNoCancel);
                    if (res == DialogResult.Yes)
                    {
                        map.CoordinateSystem = layerCoordinateSystem;
                        map.ZoomToExtents(); // reset zoom (before adding wms layer)
                    }
                    else if (res == DialogResult.Cancel)
                    {
                        if (layer is IDisposable)
                        {
                            ((IDisposable) layer).Dispose();
                        }
                        return; // don't add anything
                    }
                }
            }

            map.Layers.Add(layer);
            map.NotifyObservers();
        }

        private void RemoveLayer(ILayer layer)
        {
            if (layer == null || !layer.CanBeRemovedByUser || !map.Layers.Remove(layer))
            {
                return;
            }

            UpdateButtonsVisibility();

            if (layer is IDisposable)
            {
                var disposableLayer = layer as IDisposable;
                disposableLayer.Dispose();
            }
            map.NotifyObservers();
        }

        private void ButtonRemoveLayerClick(object sender, EventArgs e)
        {
            RemoveLayer(TreeView.SelectedNode.Tag as ILayer);
        }

        private void ContextMenuLayerDeleteClick(object sender, EventArgs e)
        {
            RemoveLayer(TreeView.SelectedNode.Tag as ILayer);
        }

        private void ContextMenuLayerRenameClick(object sender, EventArgs e)
        {
            TreeView.StartLabelEdit();
        }

        private void ContextMenuOpenLayerAttributeTableClick(object sender, EventArgs e)
        {
            var layer = TreeView.SelectedNode.Tag as ILayer;
            if (layer == null)
            {
                return;
            }

            OnOpenLayerAttributeTable(layer);
        }

        // TODO: inject it from GuiPlugin (as event/delegate) and remove dependency from IGui
        private void TreeViewNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            gui.Selection = ((ITreeNode) e.Node).Tag;
        }

        private void ZoomToLayerToolStripMenuItemClick(object sender, EventArgs e)
        {
            var tag = TreeView.SelectedNode.Tag;
            if (tag is Client.WmsServerLayer)
            {
                var layer = (Client.WmsServerLayer) tag;
                var layerEnvelope = layer.LatLonBoundingBox;
                gisService.ZoomCurrentMapToEnvelope(layerEnvelope);
            }
        }

        private void AddLayerToolStripMenuItemClick(object sender, EventArgs e)
        {
            AddLayer();
        }

        private void ZoomToLayerToolStripMenuItem1Click(object sender, EventArgs e)
        {
            new ZoomLayerCommand
            {
                Gui = gui
            }.Execute(TreeView.SelectedNode.Tag);
        }

        /// <summary>
        /// Hides all other layer the same level (top level or current layer group).
        /// </summary>
        private void HideAllButThisOneToolStripMenuItemClick(object sender, EventArgs e)
        {
            var layer = TreeView.SelectedNode.Tag as ILayer;
            HideAllLayersButThisOne(layer, Map);
        }

        private void ZoomToExtentsToolStripMenuItemClick(object sender, EventArgs e)
        {
            var cmd = new MapZoomToExtentsCommand();
            cmd.Execute();
        }

        private void ShowLabelsToolStripMenuItemCheckStateChanged(object sender, EventArgs e)
        {
            // set enabled state for the labelLayer of the selected vector layer
            var showLabels = ((ToolStripMenuItem) sender).CheckState == CheckState.Checked;

            var selectedLayer = TreeView.SelectedNode.Tag as ILayer;

            if (selectedLayer != null)
            {
                selectedLayer.ShowLabels = showLabels;
            }
        }

        private void ContextMenuLayerOpening(object sender, CancelEventArgs e)
        {
            if (TreeView.SelectedNode.Tag is Map)
            {
                return;
            }

            var selectedLayer = TreeView.SelectedNode.Tag as ILayer;

            // Setting the showLabelsToolStripMenuItem state properties (enabled & checked)
            if (selectedLayer != null)
            {
                var node = TreeView.GetNodeByTag(TreeView.SelectedNode.Tag);

                showLabelsToolStripMenuItem.Enabled = selectedLayer.LabelLayer.LabelColumn != null;
                showLabelsToolStripMenuItem.Checked = selectedLayer.ShowLabels;
                showInLegendToolStripMenuItem.Checked = selectedLayer.ShowInLegend;
                showInLegendToolStripMenuItem.Enabled = true;
                contextMenuLayerDelete.Enabled = !(node != null && node.Parent.Tag is GroupLayer && ((GroupLayer) node.Parent.Tag).LayersReadOnly);
                contextMenuLayerRename.Enabled = !selectedLayer.NameIsReadOnly;
            }
            else
            {
                showLabelsToolStripMenuItem.Enabled = false;
                showInLegendToolStripMenuItem.Enabled = false;
            }
        }

        private void ShowInLegendToolStripMenuItemClick(object sender, EventArgs e)
        {
            // set ShowInLegend property of selected layer
            var selectedLayer = (ILayer) TreeView.SelectedNode.Tag;
            selectedLayer.ShowInLegend = showInLegendToolStripMenuItem.Checked;
        }

        private void AddLayergroupToolStripMenuItemClick(object sender, EventArgs e)
        {
            map.Layers.Add(new GroupLayer("New group"));
            map.NotifyObservers();
        }

        private void ContextMenuMapVisibleChanged(object sender, EventArgs e)
        {
            if (((Control) sender).Visible)
            {
                SetZoomToMapSelection(zoomToMapExtentsToolStripMenuItem);
            }
        }

        private void ContextMenuLayerVisibleChanged(object sender, EventArgs e)
        {
            if (((Control) sender).Visible)
            {
                SetZoomToMapSelection(zoomToMapExtentsToolStripMenuItem1);
            }
        }

        private static void SetZoomToMapSelection(ToolStripMenuItem zoomToMapItem)
        {
            //check if there are mapviews available to zoom to

            foreach (ToolStripMenuItem menuItem in zoomToMapItem.DropDownItems)
            {
                menuItem.Click -= ZoomToMapMenuItemClick;
            }
            zoomToMapItem.DropDownItems.Clear();

            int no = 0;
            var lstDropDownItems = new List<ToolStripMenuItem>();
            zoomToMapItem.Enabled = false;

            var documentViews = SharpMapGisGuiPlugin.Instance.Gui.DocumentViews;

            //Get all 'inactive' mapviews..we should be able to zoom to them.
            foreach (var view in documentViews.OfType<MapView>().Where(v => v != documentViews.ActiveView))
            {
                var toolStripMenuItem = new ToolStripMenuItem
                {
                    Name = "ZoomToMapMenuItem" + no,
                    Text = view.Text,
                    Tag = view
                };

                toolStripMenuItem.Click += ZoomToMapMenuItemClick;
                lstDropDownItems.Add(toolStripMenuItem);
                no++;
            }

            if (lstDropDownItems.Count > 0)
            {
                zoomToMapItem.Enabled = true;
                zoomToMapItem.DropDownItems.AddRange(lstDropDownItems.ToArray());
            }
        }

        private static void ZoomToMapMenuItemClick(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem) sender;
            var zoomToExtentsCommand = new MapZoomToExtentsCommand();
            zoomToExtentsCommand.Execute(menuItem.Tag);
        }

        private void BringToFrontToolStripMenuItemClick(object sender, EventArgs e)
        {
            map.BringToFront(gui.Selection as ILayer);
        }

        private void SendToBackToolStripMenuItemClick(object sender, EventArgs e)
        {
            map.SendToBack(gui.Selection as ILayer);
        }

        private void SendBackwardToolStripMenuItemClick(object sender, EventArgs e)
        {
            map.SendBackward(gui.Selection as ILayer);
        }

        private void BringForwardToolStripMenuItemClick(object sender, EventArgs e)
        {
            map.BringForward(gui.Selection as ILayer);
        }

        private void ChangeCoordinateSystemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cmd = new MapChangeCoordinateSystemCommand();
            cmd.Execute();
        }
    }
}