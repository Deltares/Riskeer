using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Controls;
using Core.Common.Controls.Swf.Table;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Utils;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.Data;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Map;
using Core.GIS.SharpMap.Rendering.Thematics;
using Core.GIS.SharpMap.Styles;
using Core.GIS.SharpMap.UI.Forms;
using Core.GIS.SharpMap.UI.Tools;
using Core.GIS.SharpMap.UI.Tools.Decorations;
using Core.Plugins.SharpMapGis.Gui.Forms;
using Core.Plugins.SharpMapGis.Gui.Forms.GridProperties;
using Core.Plugins.SharpMapGis.Gui.Forms.MapLegendView;
using Core.Plugins.SharpMapGis.Gui.NodePresenters;
using Core.Plugins.SharpMapGis.Gui.Properties;
using PropertyInfo = Core.Common.Gui.PropertyInfo;

namespace Core.Plugins.SharpMapGis.Gui
{
    public class SharpMapGisGuiPlugin : GuiPlugin
    {
        private static IGui gui;
        private static IGisGuiService gisGuiService;

        private MapLegendView mapLegendView;
        private IRibbonCommandHandler ribbonCommandHandler;

        public SharpMapGisGuiPlugin()
        {
            Instance = this;
            gisGuiService = new GisGuiService(this);
        }

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                if (ribbonCommandHandler == null)
                {
                    var ribbon = new Ribbon();
                    ribbonCommandHandler = ribbon;
                }
                return ribbonCommandHandler;
            }
        }

        public override IGui Gui
        {
            get
            {
                return gui;
            }
            set
            {
                gui = value;
            }
        }

        public static SharpMapGisGuiPlugin Instance { get; private set; }

        public IGisGuiService GisGuiService
        {
            get
            {
                return gisGuiService;
            }
            set
            {
                gisGuiService = value;
            }
        }

        public MapLegendView MapLegendView
        {
            get
            {
                return mapLegendView;
            }
        }

        public void InitializeMapLegend()
        {
            if ((mapLegendView == null) || (mapLegendView.IsDisposed))
            {
                mapLegendView = new MapLegendView(gui)
                {
                    OnOpenLayerAttributeTable = layer =>
                    {
                        var mapView = GetFocusedMapView();
                        if (mapView == null)
                        {
                            return;
                        }

                        var layerData = mapView.GetDataForLayer != null
                                            ? mapView.GetDataForLayer(layer)
                                            : null;

                        if (!Gui.DocumentViewsResolver.OpenViewForData(layerData, typeof(ILayerEditorView)) && layer is VectorLayer)
                        {
                            mapView.OpenLayerAttributeTable(layer, o => gui.CommandHandler.OpenView(o));
                        }
                    },
                    Text = Properties.Resources.SharpMapGisPluginGui_InitializeMapLegend_Map_Contents
                };
            }

            // TODO: subscribe to mapLegendView.TreeView.SelectionChanged! and update Gui.Selection if necessary

            if (gui.ToolWindowViews != null)
            {
                gui.ToolWindowViews.Add(mapLegendView, ViewLocation.Left | ViewLocation.Bottom);
                gui.ToolWindowViews.ActiveView = mapLegendView;
            }
        }

        public override void Activate()
        {
            InitializeMapLegend();

            if (Gui != null && Gui.DocumentViews != null && Gui.DocumentViews.ActiveView != null)
            {
                DocumentViewsActiveViewChanged(Gui, null);
            }

            gui.SelectionChanged += GuiSelectionChanged;
            gui.DocumentViews.ActiveViewChanged += DocumentViewsActiveViewChanged;
            gui.ProjectClosing += ApplicationProjectClosing;
        }

        public override void Dispose()
        {
            if (gui != null)
            {
                gui.SelectionChanged -= GuiSelectionChanged;
                var documentViews = gui.DocumentViews;
                if (documentViews != null)
                {
                    documentViews.ActiveViewChanged -= DocumentViewsActiveViewChanged;
                }
                gui.ProjectClosing -= ApplicationProjectClosing;
            }

            gui = null;
            Instance = null;
            mapLegendView = null;
            gisGuiService = null;
        }

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<Map, MapProperties>
            {
                AfterCreate = properties => properties.Owner = gui.MainWindow 
            };
            yield return new PropertyInfo<LegendTool, LegendToolProperties>();
            yield return new PropertyInfo<LayoutComponentTool, LayoutComponentToolProperties>();
            yield return new PropertyInfo<ILayer, LayerProperties>();
            yield return new PropertyInfo<VectorLayer, VectorLayerPointProperties>
            {
                AdditionalDataCheck = o => o.Style.GeometryType == typeof(IPoint)
            };
            yield return new PropertyInfo<VectorLayer, VectorLayerPolygonProperties>
            {
                AdditionalDataCheck = o => o.Style.GeometryType == typeof(IPolygon)
                                           || o.Style.GeometryType == typeof(IMultiPolygon)
            };
            yield return new PropertyInfo<VectorLayer, VectorLayerLineProperties>
            {
                AdditionalDataCheck = o => o.Style.GeometryType == typeof(ILineString)
                                           || o.Style.GeometryType == typeof(IMultiLineString)
            };
            yield return new PropertyInfo<VectorLayer, VectorLayerProperties>();
            yield return new PropertyInfo<VectorStyle, PointStyleProperties>
            {
                AdditionalDataCheck = o => o.GeometryType == typeof(IPoint)
            };
            yield return new PropertyInfo<VectorStyle, PolygonStyleProperties>
            {
                AdditionalDataCheck = o => o.GeometryType == typeof(IPolygon)
                                           || o.GeometryType == typeof(IMultiPolygon)
            };
            yield return new PropertyInfo<VectorStyle, LineStyleProperties>
            {
                AdditionalDataCheck = o => o.GeometryType == typeof(ILineString)
                                           || o.GeometryType == typeof(IMultiLineString)
            };
            yield return new PropertyInfo<VectorStyle, VectorStyleProperties>();
            yield return new PropertyInfo<ThemeItem, PointStyleProperties>
            {
                AdditionalDataCheck = o =>
                {
                    var themeItemVectorStyle = o.Style as VectorStyle;

                    return themeItemVectorStyle != null
                           && themeItemVectorStyle.GeometryType == typeof(IPoint);
                },
                GetObjectPropertiesData = o => o.Style
            };
            yield return new PropertyInfo<ThemeItem, PolygonStyleProperties>
            {
                AdditionalDataCheck = o =>
                {
                    var themeItemVectorStyle = o.Style as VectorStyle;

                    return themeItemVectorStyle != null
                           && (themeItemVectorStyle.GeometryType == typeof(IPolygon)
                               || themeItemVectorStyle.GeometryType == typeof(IMultiPolygon));
                },
                GetObjectPropertiesData = o => o.Style
            };
            yield return new PropertyInfo<ThemeItem, LineStyleProperties>
            {
                AdditionalDataCheck = o =>
                {
                    var themeItemVectorStyle = o.Style as VectorStyle;

                    return themeItemVectorStyle != null
                           && (themeItemVectorStyle.GeometryType == typeof(ILineString)
                               || themeItemVectorStyle.GeometryType == typeof(IMultiLineString));
                },
                GetObjectPropertiesData = o => o.Style
            };
            yield return new PropertyInfo<ThemeItem, ThemeItemProperties>();
            yield return new PropertyInfo<GroupLayer, GroupLayerProperties>();
            yield return new PropertyInfo<FeatureDataRow, FeatureDataRowProperties>();
        }

        public override IEnumerable<ViewInfo> GetViewInfoObjects()
        {
            yield return new ViewInfo<Map, MapView>
            {
                Image = Resources.Map,
                Description = "Map",
                AfterCreate = (view, map) => view.Owner = gui.MainWindow
            };
            yield return new ViewInfo<DataTable, TableView>
            {
                Description = "Table"
            };
        }

        public override IEnumerable<ITreeNodePresenter> GetProjectTreeViewNodePresenters()
        {
            yield return new MapProjectTreeViewNodePresenter();
        }

        internal static MapView GetFocusedMapView()
        {
            return gui.DocumentViews.ActiveView as MapView;
        }

        private void ApplicationProjectClosing(Project project)
        {
            if (gui == null)
            {
                return;
            }

            if (mapLegendView != null)
            {
                mapLegendView.Map = null;
            }
        }

        private static void GuiSelectionChanged(object sender, SelectedItemChangedEventArgs e)
        {
            // Send selection to all relevant views
            var selection = gui.Selection;
            if ((selection is IFeature) || (selection == null))
            {
                // Try to select the feature on the map corresponding to the selected row in the data table
                foreach (var view in gui.DocumentViews.AllViews.OfType<MapView>().Where(view => view.IsAllowSyncWithGuiSelection))
                {
                    view.MapControl.SelectedFeaturesChanged -= MapControlSelectedFeaturesChanged;
                    view.MapControl.SelectTool.Select((IFeature) selection);
                    view.MapControl.SelectedFeaturesChanged += MapControlSelectedFeaturesChanged;
                }
            }
            else if (selection is IEnumerable && ((IEnumerable) selection).OfType<IFeature>().Any())
            {
                // If the selection was made from within the map control, also select it in the corresponding attribute table
                var selectedFeatures = ((IEnumerable) selection).OfType<IFeature>();
                if (!selectedFeatures.Any())
                {
                    return;
                }
                foreach (var view in gui.DocumentViews.AllViews.OfType<MapView>().Where(view => view.IsAllowSyncWithGuiSelection))
                {
                    view.MapControl.SelectedFeaturesChanged -= MapControlSelectedFeaturesChanged;
                    view.MapControl.SelectTool.Select(selectedFeatures);
                    view.MapControl.SelectedFeaturesChanged += MapControlSelectedFeaturesChanged;
                }
            }
        }

        private void DocumentViewsActiveViewChanged(object sender, EventArgs e)
        {
            UpdateMapLegendView();

            var mapView = GetFocusedMapView();
            if (mapView != null)
            {
                mapView.MapControl.MouseDoubleClick -= mapView_MouseDoubleClick;
                mapView.MapControl.MouseDoubleClick += mapView_MouseDoubleClick;

                mapView.MapControl.ToolActivated -= MapControlOnToolActivated;
                mapView.MapControl.ToolActivated += MapControlOnToolActivated;

                var openViewTool = mapView.MapControl.GetToolByType<OpenViewMapTool>();
                if (openViewTool != null && openViewTool.OpenView == null)
                {
                    openViewTool.OpenView = feature => gui.DocumentViewsResolver.OpenViewForData(feature);
                    openViewTool.CanOpenView = feature => gui.DocumentViewsResolver.CanOpenViewFor(feature);
                }

                if (mapView.IsAllowSyncWithGuiSelection)
                {
                    mapView.MapControl.SelectedFeaturesChanged -= MapControlSelectedFeaturesChanged;
                    mapView.MapControl.SelectedFeaturesChanged += MapControlSelectedFeaturesChanged;
                }

                mapView.Map.MapRendered -= MapMapRendered;
                mapView.Map.MapRendered += MapMapRendered;
            }
        }

        private static void MapControlOnToolActivated(object sender, EventArgs<IMapTool> eventArgs)
        {
            RefreshRibbonItems();
        }

        private void UpdateMapLegendView()
        {
            if (mapLegendView == null)
            {
                return;
            }

            var mapView = GetFocusedMapView();
            if (mapView != null)
            {
                if (!Equals(mapLegendView.Data, mapView.Data))
                {
                    // only update when data is changed, 
                    mapLegendView.Data = mapView.Data;
                }
            }
            else
            {
                mapLegendView.Data = null;
            }
        }

        private static void RefreshRibbonItems()
        {
            gui.MainWindow.ValidateItems();
        }

        private static void MapMapRendered(Graphics g)
        {
            RefreshRibbonItems();
        }

        // TODO: WTF?!? Use mapControl.SelectTool.SelectTool.SelectionChanged += ...
        // Reason why not to: AddTool and EditTool also have their own selection mechanism
        private static void MapControlSelectedFeaturesChanged(object sender, EventArgs e)
        {
            gui.SelectionChanged -= GuiSelectionChanged;

            var mapControl = (MapControl) sender;

            IList<IFeature> selectedFeatures = mapControl.SelectedFeatures.ToList();
            if (selectedFeatures.Count != 0)
            {
                gui.Selection = selectedFeatures.Count == 1 ? (object) selectedFeatures[0] : selectedFeatures;
            }
            else
            {
                var selectedLayoutComponentTool = mapControl.Tools.OfType<LayoutComponentTool>().FirstOrDefault(t => t.Selected);
                gui.Selection = selectedLayoutComponentTool;
            }

            gui.SelectionChanged += GuiSelectionChanged;
        }

        private static void mapView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            IEnumerable<IFeature> selectedFeatures = ((MapControl) sender).SelectedFeatures;
            if (selectedFeatures == null || !selectedFeatures.Any())
            {
                return;
            }

            var data = selectedFeatures.First();

            gui.CommandHandler.OpenView(data);
        }
    }
}