using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Controls.Swf;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Utils.Collections;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.CoordinateSystems.Transformations;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Map;
using Core.GIS.SharpMap.UI.Forms;
using Core.GIS.SharpMap.UI.Tools;
using Core.Plugins.SharpMapGis.Tools;

namespace Core.Plugins.SharpMapGis.Gui.Forms
{
    public partial class MapView : UserControl, IView
    {
        private bool canAddPoint = true;
        private bool canDeleteItem = true;
        private bool canMoveItem = true;
        private bool canMoveItemLinear = true;
        private bool canSelectItem = true;
        private bool settingSelection = false;
        private readonly ExportMapToImageMapTool exportMapToImageMapTool;

        public MapView()
        {
            InitializeComponent();
            IsAllowSyncWithGuiSelection = true;

            TabControl = new MapViewTabControl
            {
                Size = new Size(300, 250), Dock = DockStyle.Bottom
            };
            Controls.Add(TabControl);

            MapControl.SelectedFeaturesChanged += (s, e) => SyncMapViewEditorSelection();
            Splitter.ControlToHide = TabControl;
            TabControl.ViewCollectionChanged += OnTabControlOnViewCollectionChanged;

            // hide for now
            IsTabControlVisible = false;

            // add some tools here, to avoid references to Ringtoets projects in SharpMap
            exportMapToImageMapTool = new ExportMapToImageMapTool();
            MapControl.Tools.Add(exportMapToImageMapTool);
            Map = new Map(MapControl.ClientSize)
            {
                Zoom = 100
            };
        }

        public IWin32Window Owner
        {
            get
            {
                return exportMapToImageMapTool.Owner;
            }
            set
            {
                exportMapToImageMapTool.Owner = value;
            }
        }

        public bool IsTabControlVisible
        {
            get
            {
                return Splitter.Visible;
            }
            set
            {
                if (IsTabControlVisible == value)
                {
                    return;
                }

                Splitter.ToggleState();
                Splitter.Visible = value;
                TabControl.SendToBack();
            }
        }

        /// <summary>
        /// If true, selection in the MapView is synched with Gui.Selection
        /// </summary>
        public bool IsAllowSyncWithGuiSelection { get; set; }

        public Map Map
        {
            get
            {
                return MapControl.Map;
            }
            set
            {
                if (MapControl.Map == value)
                {
                    return;
                }

                UnsubscribeEvents();
                if (MapControl != null)
                {
                    MapControl.Map = value;
                }
                SubscribeEvents();
            }
        }

        public MapControl MapControl { get; private set; }

        public MapViewTabControl TabControl { get; private set; }

        public CollapsibleSplitter Splitter { get; set; }

        public Func<ILayer, object> GetDataForLayer { get; set; }

        public Func<object, ILayer> GetLayerForData { get; set; }

        public bool CanSelectItem
        {
            get
            {
                return canSelectItem;
            }
            set
            {
                canSelectItem = value;
            }
        }

        public bool IsSelectItemActive
        {
            get
            {
                return (MapControl.SelectTool.IsActive &&
                        (MapControl.SelectTool.MultiSelectionMode == MultiSelectionMode.Rectangle));
            }
            set
            {
                if (value)
                {
                    MapControl.SelectTool.MultiSelectionMode = MultiSelectionMode.Rectangle;
                    MapControl.ActivateTool(MapControl.SelectTool);
                }
            }
        }

        public bool CanMoveItem
        {
            get
            {
                return canMoveItem;
            }
            set
            {
                canMoveItem = value;
            }
        }

        public bool IsMoveItemActive
        {
            get
            {
                return MapControl.MoveTool.IsActive;
            }
            set
            {
                // only support setting to true
                if (value)
                {
                    MapControl.ActivateTool(MapControl.MoveTool);
                }
            }
        }

        public bool CanMoveItemLinear
        {
            get
            {
                return canMoveItemLinear;
            }
            set
            {
                canMoveItemLinear = value;
            }
        }

        public bool IsMoveItemLinearActive
        {
            get
            {
                return MapControl.LinearMoveTool.IsActive;
            }
            set
            {
                if (value)
                {
                    MapControl.ActivateTool(MapControl.LinearMoveTool);
                }
            }
        }

        public bool CanDeleteItem
        {
            get
            {
                return canDeleteItem;
            }
            set
            {
                canDeleteItem = value;
            }
        }

        public bool IsDeleteItemActive
        {
            get
            {
                return true;
            }
            set {}
        }

        public bool CanAddPoint
        {
            get
            {
                return canAddPoint;
            }
            set
            {
                canAddPoint = value;
            }
        }

        public bool IsAddPointActive
        {
            get
            {
                var tool = (CurvePointTool) MapControl.GetToolByName("CurvePoint");
                return tool.IsActive && tool.Mode == CurvePointTool.EditMode.Add;
            }
            set
            {
                if (value)
                {
                    var tool = (CurvePointTool) MapControl.GetToolByName("CurvePoint");
                    tool.Mode = CurvePointTool.EditMode.Add;
                    MapControl.ActivateTool(tool);
                }
            }
        }

        public bool IsRemovePointActive
        {
            get
            {
                var tool = (CurvePointTool) MapControl.GetToolByName("CurvePoint");
                return tool.IsActive && tool.Mode == CurvePointTool.EditMode.Remove;
            }
            set
            {
                if (value)
                {
                    var tool = (CurvePointTool) MapControl.GetToolByName("CurvePoint");
                    tool.Mode = CurvePointTool.EditMode.Remove;
                    MapControl.ActivateTool(tool);
                }
            }
        }

        public bool CanRemovePoint
        {
            get
            {
                return true;
            }
        }

        public ViewInfo ViewInfo { get; set; }

        /// <summary>
        /// Expects a Map
        /// </summary>
        public object Data
        {
            get
            {
                return MapControl.Map;
            }
            set
            {
                Map = (Map) value;
            }
        }

        public IView OpenLayerAttributeTable(ILayer layer, Action<object> openViewMethod = null)
        {
            if (!(layer is VectorLayer) || !layer.ShowAttributeTable)
            {
                return null;
            }

            var view = TabControl.ChildViews.OfType<VectorLayerAttributeTableView>().FirstOrDefault(v => v.Data == layer);
            if (view != null)
            {
                TabControl.ActiveView = view;
                return null;
            }

            if (!IsTabControlVisible)
            {
                IsTabControlVisible = true;
            }

            view = new VectorLayerAttributeTableView
            {
                Data = layer,
                Layer = layer,
                Text = layer.Name,
                ZoomToFeature = feature => EnsureVisible(feature),
                OpenViewMethod = openViewMethod,
                DeleteSelectedFeatures = () => MapControl.DeleteTool.DeleteSelection()
            };

            TabControl.AddView(view);

            return view;
        }

        public void EnsureVisible(object item)
        {
            var layer = item as ILayer;
            if (layer != null)
            {
                layer.Visible = true;
                return;
            }

            // try show in map
            var feature = item as IFeature;
            if (feature == null)
            {
                return;
            }
            ILayer featureLayer = MapControl.Map.GetLayerByFeature(feature);
            if (featureLayer == null)
            {
                return;
            }

            EnsureFeatureVisible(feature, layer);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    MapControl.Dispose();
                    TabControl.Dispose();
                }

                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occured during dispose: " + e);
            }
        }

        private void EnsureFeatureVisible(IFeature feature, ILayer layer)
        {
            var envelope = feature.Geometry.EnvelopeInternal;

            if (feature.Geometry is IPoint)
            {
                var point = (IPoint) feature.Geometry;
                var envelopeExpansion = 100d; //crap!!!!!
                envelope = new Envelope(point.X - envelopeExpansion, point.X + envelopeExpansion,
                                        point.Y - envelopeExpansion, point.Y + envelopeExpansion);
            }
            else
            {
                envelope.ExpandBy(envelope.Width*0.05); // 10% bigger
            }

            if (layer != null && layer.CoordinateTransformation != null)
            {
                envelope = GeometryTransform.TransformBox(envelope, layer.CoordinateTransformation.MathTransform);
            }

            Map.ZoomToFit(envelope);
            MapControl.Refresh();
        }

        private void SubscribeEvents()
        {
            if (Map == null)
            {
                return;
            }

            Map.CollectionChanged += mapCollectionChangedEventHandler;

            DataBindings.Add("Text", Map, "Name", false, DataSourceUpdateMode.OnPropertyChanged);

            Text = Map.Name;
        }

        private void UnsubscribeEvents()
        {
            if (Map == null)
            {
                return;
            }

            Map.CollectionChanged -= mapCollectionChangedEventHandler;

            DataBindings.Clear();
        }

        private void mapCollectionChangedEventHandler(object sender, NotifyCollectionChangeEventArgs e)
        {
            var layer = e.Item as ILayer;

            if (e.Action == NotifyCollectionChangeAction.Remove)
            {
                RemoveTabFor(layer);
            }
        }

        private void RemoveTabFor(ILayer layer)
        {
            var groupLayer = layer as GroupLayer;
            if (groupLayer != null)
            {
                foreach (var subLayer in groupLayer.Layers)
                {
                    RemoveTabFor(subLayer);
                }
            }

            var dataForLayer = GetDataForLayer != null
                                   ? GetDataForLayer(layer) ?? layer
                                   : layer;

            var view = TabControl.ChildViews.FirstOrDefault(v => Equals(v.Data, dataForLayer));
            if (view == null)
            {
                return;
            }

            TabControl.RemoveView(view);
        }

        private void MapControlMouseMove(object sender, MouseEventArgs e)
        {
            var worldLocation = MapControl.Map.ImageToWorld(e.Location);
            if (SharpMapGisGuiPlugin.Instance != null && SharpMapGisGuiPlugin.Instance.Gui != null && worldLocation != null)
            {
                var coordinateSystem = MapControl.Map.CoordinateSystem != null
                                           ? "(" + MapControl.Map.CoordinateSystem.Name + ")"
                                           : "";
                var message = string.Format("Current map coordinates {0} : {1}, {2}", coordinateSystem, worldLocation.X, worldLocation.Y);
                // HACK:....
                if (SharpMapGisGuiPlugin.Instance.Gui.MainWindow != null)
                {
                    SharpMapGisGuiPlugin.Instance.Gui.MainWindow.StatusBarMessage = message;
                }
            }
        }

        private void MapControlMouseLeave(object sender, EventArgs e)
        {
            // HACK:....
            // HACK, TODO: move to GuiPlugin or some service class
            if (SharpMapGisGuiPlugin.Instance != null && SharpMapGisGuiPlugin.Instance.Gui != null)
            {
                if (SharpMapGisGuiPlugin.Instance.Gui.MainWindow != null)
                {
                    SharpMapGisGuiPlugin.Instance.Gui.MainWindow.StatusBarMessage = "";
                }
            }
        }

        /// <summary>
        /// Bubble MouseEnter to MapView (otherwise that event will never be fired)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapControlMouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter(e);
        }

        private void SyncMapViewEditorSelection()
        {
            var features = MapControl.SelectedFeatures;
            if (features == null || settingSelection)
            {
                return;
            }

            settingSelection = true;

            features = features.ToList();

            foreach (var mapViewEditor in TabControl.ChildViews.OfType<ILayerEditorView>())
            {
                mapViewEditor.SelectedFeatures = features;
            }
            settingSelection = false;
        }

        private void MapEditorSelectedFeaturesChanged(object sender, EventArgs e)
        {
            var mapViewEditor = sender as ILayerEditorView;
            if (mapViewEditor == null || settingSelection)
            {
                return;
            }

            settingSelection = true;
            MapControl.SelectTool.Select(mapViewEditor.SelectedFeatures);
            settingSelection = false;
        }

        private void OnTabControlOnViewCollectionChanged(object s, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var mapEditor = e.NewItems != null ? e.NewItems.OfType<ILayerEditorView>().FirstOrDefault() : null;

                if (mapEditor != null)
                {
                    mapEditor.SelectedFeaturesChanged += MapEditorSelectedFeaturesChanged;
                    mapEditor.Layer = GetLayerForData != null ? GetLayerForData(mapEditor.Data) : null;
                }
                if (!IsTabControlVisible)
                {
                    IsTabControlVisible = true;
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var mapEditor = e.OldItems != null ? e.OldItems.OfType<ILayerEditorView>().FirstOrDefault() : null;
                if (mapEditor != null)
                {
                    mapEditor.SelectedFeaturesChanged -= MapEditorSelectedFeaturesChanged;
                    mapEditor.Layer = null;
                    mapEditor.Data = null;
                    mapEditor.Dispose();
                }

                if (IsTabControlVisible && !TabControl.ChildViews.Any())
                {
                    IsTabControlVisible = false;
                }
            }
        }
    }
}