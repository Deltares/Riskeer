using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Controls.Swf.Table;
using DelftTools.Shell.Core;
using DelftTools.Shell.Core.Workflow;
using DelftTools.Shell.Gui;
using DelftTools.Shell.Gui.Forms;
using DelftTools.Utils;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms.MapLegendView;
using DeltaShell.Plugins.SharpMapGis.Gui.NodePresenters;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;
using Mono.Addins;
using SharpMap;
using SharpMap.Api.Layers;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.UI.Forms;
using SharpMap.UI.Tools;
using SharpMap.UI.Tools.Decorations;
using PropertyInfo = DelftTools.Shell.Gui.PropertyInfo;

namespace DeltaShell.Plugins.SharpMapGis.Gui
{
    [Extension(typeof(IPlugin))]
    public class SharpMapGisGuiPlugin : GuiPlugin
    {
        private bool isActive;
        private static IGui gui;

        private static SharpMapGisGuiPlugin instance;
        private static MapLegendView mapLegendView;
        private static IGisGuiService gisGuiService;

        private IRibbonCommandHandler ribbonCommandHandler;


        public SharpMapGisGuiPlugin()
        {
            instance = this;
            gisGuiService = new GisGuiService(this);
        }

        public override string Name
        {
            get { return "GIS (UI)"; }
        }

        public override string DisplayName
        {
            get { return "SharpMap GIS Plugin (UI)"; }
        }

        public override string Description
        {
            get { return SharpMapGis.Properties.Resources.SharpMapGisApplicationPlugin_Description; }
        }

        public override string Version
        {
            get { return GetType().Assembly.GetName().Version.ToString(); }
        }

        public override bool IsActive
        {
            get { return isActive; }
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
            get { return gui; }
            set { gui = value; }
        }

        public static SharpMapGisGuiPlugin Instance
        {
            get { return instance; }
        }

        public IGisGuiService GisGuiService
        {
            get { return gisGuiService; }
            set { gisGuiService = value; }
        }

        public MapLegendView MapLegendView
        {
            get { return mapLegendView; }
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
            gui.DocumentViews.ChildViewChanged += DocumentViewsActiveViewChanged;
            gui.Application.ProjectClosing += ApplicationProjectClosing;

            if (gui.Application.ActivityRunner != null)
            {
                gui.Application.ActivityRunner.ActivityCompleted += ActivityRunnerOnActivityCompleted;
            }

            isActive = true;
        }

        public override void Dispose()
        {
            if (gui != null)
            {
                gui.SelectionChanged -= GuiSelectionChanged;
                var documentViews = gui.DocumentViews;
                if (documentViews != null)
                {
                    documentViews.ChildViewChanged -= DocumentViewsActiveViewChanged;
                    documentViews.ActiveViewChanged -= DocumentViewsActiveViewChanged;
                }

                var application = gui.Application;
                if (gui.Application != null)
                {
                    application.ProjectClosing -= ApplicationProjectClosing;

                    if (application.ActivityRunner != null)
                    {
                        application.ActivityRunner.ActivityCompleted -= ActivityRunnerOnActivityCompleted;
                    }
                }
            }

            gui = null;
            instance = null;
            mapLegendView = null;
            gisGuiService = null;
        }

        public override void OnViewRemoved(IView view)
        {
            UpdateMapLegendView();

            var mapView = GetFocusedMapView(view);
            if (mapView == null) return;

            mapView.MapControl.SelectedFeaturesChanged -= MapControlSelectedFeaturesChanged;
            mapView.MapControl.MouseDoubleClick -= mapView_MouseDoubleClick;
            mapView.Map.MapRendered -= MapMapRendered;
        }

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            return SharpMapGisPropertyInfoProvider.GetPropertyInfos();
        }

        public override IEnumerable<ViewInfo> GetViewInfoObjects()
        {
            yield return new ViewInfo<Map, MapView> {Description = "Map"};
            yield return new ViewInfo<VectorLayer, VectorLayerAttributeTableView>
                {
                    Description = "Attribute table",
                    CompositeViewType = typeof(ProjectItemMapView),
                    GetViewName = (v,o) => o. Name + " Attributes"
                };
            yield return new ViewInfo<DataTable, TableView> { Description = "Table" };
            yield return new ViewInfo<IProjectItem, IProjectItem, ProjectItemMapView>
                {
                    Description = "Map",
                    AdditionalDataCheck = o => CanLayerProvidersCreateLayerFor(o),
                    GetViewData = o => o,
                    GetViewName = (v, o) => o.Name,
                    OnActivateView = (v, o) =>
                        {
                            var layerData = o;
                            var layer = v.MapView.GetLayerForData(layerData);
                            if (layer != null)
                            {
                                v.MapView.EnsureVisible(layer);

                                if (MapLegendView != null)
                                {
                                    MapLegendView.EnsureVisible(layer);
                                }
                           }
                        },
                    AfterCreate = (v, o) =>
                        {
                            var mapLayerProviders = Gui.Plugins.Select(p => p.MapLayerProvider).Where(p => p != null).ToList();

                            v.CreateLayerForData = (lo, ld) => MapLayerProviderHelper.CreateLayersRecursive(lo, null, mapLayerProviders, ld);
                            v.RefreshLayersAction = (l, ld, po) => MapLayerProviderHelper.RefreshLayersRecursive(l, ld, mapLayerProviders, po);
                        },
                    CloseForData = (v, o) => v.Data.Equals(o) // only close the view if it is the root object 
                };
        }
        
        public override IMenuItem GetContextMenu(object sender, object data)
        {
            //custom treenodes for maplegend view
            if (sender is TreeNode)
            {
                var treeNode = (TreeNode)sender;
                if (treeNode.TreeView.Parent == mapLegendView)
                {
                    return mapLegendView != null ? mapLegendView.GetContextMenu(data) : null;
                }

            }

            return null;
        }

        public override IEnumerable<ITreeNodePresenter> GetProjectTreeViewNodePresenters()
        {
            yield return new MapProjectTreeViewNodePresenter {GuiPlugin = this};
        }

        public override bool CanDrop(object source, object target)
        {
            if (source is ILayer && target is Map)
            {
                return true;
            }
            
            if (source is IList && target is Map && ((IEnumerable)source).Cast<object>().FirstOrDefault() is IGeometry)
            {
                return true;
            }

            return false;
        }

        public override void OnDragDrop(object source, object target)
        {
            var map = target as Map;

            if(map == null)
            {
                return;
            }

            if (source is ILayer)
            {
                map.Layers.Add((ILayer) source);
            }

            if (source is IList)
            {
                var items = (IList)source;
                
                if(items.Count > 0 && items[0] is IGeometry)
                {
                    var provider = new FeatureCollection();
                    foreach (var item in items)
                    {
                        var geometry = (IGeometry) item;
                        provider.Features.Add(geometry);
                    }
                    ILayer layer = new VectorLayer { DataSource = provider };
                    map.Layers.Add(layer);
                }

                if (items.Count > 0 && items[0] is IFeature)
                {
                    var provider = new FeatureCollection {Features = items};
                    ILayer layer = new VectorLayer { DataSource = provider };
                    map.Layers.Add(layer);
                }
            }
        }

        public override IEnumerable<Assembly> GetPersistentAssemblies()
        {
            yield return GetType().Assembly;
        }

        /// <summary>
        /// Creates a <see cref="VectorLayerAttributeTableView"/> <see cref="ViewInfo"/> object for 
        /// a IEnumerable of TFeature that is present part of a TFeatureContainer
        /// </summary>
        /// <example>
        /// Model - boundaries 
        /// <code>
        /// var viewInfo = CreateAttributeTableViewInfo<![CDATA[<BoundaryType, ModelType>]]>(m => m.Boundaries, () => Gui)
        /// </code>
        /// </example>
        /// <typeparam name="TFeature">Type of the sub features</typeparam>
        /// <typeparam name="TFeatureContainer">Type of the feature container</typeparam>
        /// <param name="getCollection">Function for getting the IEnumerable of <typeparam name="TFeature"/> from <typeparam name="TFeatureContainer"/> </param>
        /// <param name="getGui">Function for getting an <see cref="IGui"/></param>
        public static ViewInfo<IEnumerable<TFeature>, ILayer, VectorLayerAttributeTableView> CreateAttributeTableViewInfo<TFeature, TFeatureContainer>(Func<TFeatureContainer, IEnumerable<TFeature>> getCollection, Func<IGui> getGui)
        {
            return new ViewInfo<IEnumerable<TFeature>, ILayer, VectorLayerAttributeTableView>
                {
                    Description = "Attribute Table",
                    GetViewName = (v, o) => o.Name,
                    AdditionalDataCheck = o =>
                        {
                            var container = getGui().Application.Project.Items.OfType<TFeatureContainer>().FirstOrDefault(fc => Equals(o, getCollection(fc)));
                            return container != null;
                        },
                    GetViewData = o =>
                        {
                            var centralMap = getGui().DocumentViews.OfType<ProjectItemMapView>()
                                .FirstOrDefault(v => v.MapView.GetLayerForData(o) != null);
                            return centralMap == null ? null : centralMap.MapView.GetLayerForData(o);
                        },
                    CompositeViewType = typeof (ProjectItemMapView),
                    GetCompositeViewData = o => getGui().Application.Project.Items.OfType<IProjectItem>().FirstOrDefault(fc =>
                        {
                            if (fc is TFeatureContainer)
                            {
                                return Equals(o, getCollection((TFeatureContainer)fc));
                            }

                            return false;
                        }),
                    AfterCreate = (v, o) =>
                        {
                            var centralMap = getGui().DocumentViews.OfType<ProjectItemMapView>()
                                .FirstOrDefault(vi => vi.MapView.GetLayerForData(o) != null);
                            if (centralMap == null) return;

                            v.DeleteSelectedFeatures = () => centralMap.MapView.MapControl.DeleteTool.DeleteSelection();
                            v.OpenViewMethod = ob => getGui().CommandHandler.OpenView(ob);
                            v.ZoomToFeature = feature => centralMap.MapView.EnsureVisible(feature);
                            v.CanAddDeleteAttributes = false;
                        }
                };
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
                                if (mapView == null) return;

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

        private void ApplicationProjectClosing(Project project)
        {
            if(gui == null)
            {
                return;
            }

            if (mapLegendView != null)
            {
                mapLegendView.Map = null;
            }
        }

        static void GuiSelectionChanged(object sender, SelectedItemChangedEventArgs e)
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
            else if (selection is IEnumerable && ((IEnumerable)selection).OfType<IFeature>().Any())
            {
                // If the selection was made from within the map control, also select it in the corresponding attribute table
                var selectedFeatures = ((IEnumerable)selection).OfType<IFeature>();
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

        internal static MapView GetFocusedMapView(IView view = null)
        {
            return gui.GetFocusedMapView(view);
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

        private void ActivityRunnerOnActivityCompleted(object sender, ActivityEventArgs e)
        {
            if (mapLegendView == null)
            {
                return;
            }

            // Force a refresh of the map legend view (tree view)
            mapLegendView.Data = null;

            UpdateMapLegendView();
        }

        private static void UpdateMapLegendView()
        {
            if(mapLegendView == null)
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

        static void MapMapRendered(Graphics g)
        {
            RefreshRibbonItems();
        }

        // TODO: WTF?!? Use mapControl.SelectTool.SelectTool.SelectionChanged += ...
        // Reason why not to: AddTool and EditTool also have their own selection mechanism
        static void MapControlSelectedFeaturesChanged(object sender, EventArgs e)
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

        static void mapView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            IEnumerable<IFeature> selectedFeatures = ((MapControl)sender).SelectedFeatures;
            if (selectedFeatures == null || !selectedFeatures.Any())
            {
                return;
            }

            var data = selectedFeatures.First();
            
            gui.CommandHandler.OpenView(data);
        }

        private bool CanLayerProvidersCreateLayerFor(object data)
        {
            if (Gui == null || Gui.Plugins == null)
            {
                return false;
            }

            return Gui.Plugins.Select(p => p.MapLayerProvider)
                      .Where(p => p != null)
                      .Any(p => p.CanCreateLayerFor(data, Gui.SelectedProjectItem));
        }
    }
}