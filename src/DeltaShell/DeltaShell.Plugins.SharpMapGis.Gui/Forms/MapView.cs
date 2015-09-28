﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Controls.Swf;
using DelftTools.Shell.Gui;
using DelftTools.Utils;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Collections.Generic;
using DeltaShell.Plugins.SharpMapGis.Gui.Properties;
using DeltaShell.Plugins.SharpMapGis.Tools;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using SharpMap;
using SharpMap.Api.Layers;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Layers;
using SharpMap.UI.Forms;
using SharpMap.UI.Tools;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms
{
    public partial class MapView : UserControl, ICanvasEditor, ICompositeView, ISearchableView
    {
        private MapViewTabControl tabControl;

        private bool canAddPoint = true;
        private bool canDeleteItem = true;
        private bool canMoveItem = true;
        private bool canMoveItemLinear = true;
        private bool canSelectItem = true;
        private bool settingSelection = false;
        private StackTrace constructorStackTrace;

        public MapView()
        {
            InitializeComponent();
            IsAllowSyncWithGuiSelection = true;

            tabControl = new MapViewTabControl { Size = new Size(300, 250), Dock = DockStyle.Bottom };
            Controls.Add(tabControl);

            mapControl.SelectedFeaturesChanged += (s,e) => SyncMapViewEditorSelection();
            collapsibleSplitter1.ControlToHide = tabControl;
            tabControl.ViewCollectionChanged += OnTabControlOnViewCollectionChanged;

            // hide for now
            IsTabControlVisible = false;

            // add some tools here, to avoid references to DeltaShell projects in SharpMap
            mapControl.Tools.Add(new ExportMapToImageMapTool());
            Map = new Map(mapControl.ClientSize) { Zoom = 100 };

            if (Assembly.GetEntryAssembly() == null) // HACK: detecting nasty dispose exceptions when assembly empty - we run from non-exe (test)
            {
                constructorStackTrace = new StackTrace();
            }
        }

        public bool IsTabControlVisible
        {
            get { return collapsibleSplitter1.Visible; }
            set
            {
                if (IsTabControlVisible == value)
                {
                    return;
                }

                collapsibleSplitter1.ToggleState();
                collapsibleSplitter1.Visible = value;
                tabControl.SendToBack();
            }
        }

        /// <summary>
        /// If true, selection in the MapView is synched with Gui.Selection
        /// </summary>
        public bool IsAllowSyncWithGuiSelection { get; set; }

        public Map Map
        {
            get { return mapControl.Map; }
            set
            {
                if(mapControl.Map == value)
                {
                    return;
                }

                UnsubscribeEvents();
                if (mapControl != null)
                {
                    mapControl.Map = value;
                }
                SubscribeEvents();
            }
        }

        public MapControl MapControl
        {
            get { return mapControl; }
        }

        public ViewInfo ViewInfo { get; set; }

        public bool CanSelectItem
        {
            get { return canSelectItem; }
            set { canSelectItem = value; }
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
            get { return canMoveItem; }
            set { canMoveItem = value; }
        }

        public bool IsMoveItemActive
        {
            get { return MapControl.MoveTool.IsActive; }
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
            get { return canMoveItemLinear; }
            set { canMoveItemLinear = value; }
        }

        public bool IsMoveItemLinearActive
        {
            get { return MapControl.LinearMoveTool.IsActive; }
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
            get { return canDeleteItem; }
            set { canDeleteItem = value; }
        }

        public bool IsDeleteItemActive
        {
            get { return true; }
            set { }
        }

        public bool CanAddPoint
        {
            get { return canAddPoint; }
            set { canAddPoint = value; }
        }

        public bool IsAddPointActive
        {
            get
            {
                var tool = (CurvePointTool)MapControl.GetToolByName("CurvePoint");
                return tool.IsActive && tool.Mode == CurvePointTool.EditMode.Add;
            }
            set
            {
                if (value)
                {
                    var tool = (CurvePointTool)MapControl.GetToolByName("CurvePoint");
                    tool.Mode = CurvePointTool.EditMode.Add;
                    MapControl.ActivateTool(tool);
                }
            }
        }

        public bool IsRemovePointActive
        {
            get
            {
                var tool = (CurvePointTool)MapControl.GetToolByName("CurvePoint");
                return tool.IsActive && tool.Mode == CurvePointTool.EditMode.Remove;
            }
            set
            {
                if (value)
                {
                    var tool = (CurvePointTool)MapControl.GetToolByName("CurvePoint");
                    tool.Mode = CurvePointTool.EditMode.Remove;
                    MapControl.ActivateTool(tool);
                }
            }
        }

        public bool CanRemovePoint { get { return true; } }

        /// <summary>
        /// Expects a Map
        /// </summary>
        public object Data
        {
            get { return mapControl.Map; }
            set { Map = (Map) value; }
        }

        public Image Image 
        {
            get { return Resources.Map; }
            set { }
        }

        public MapViewTabControl TabControl { get { return tabControl; } }

        public CollapsibleSplitter Splitter
        {
            get { return collapsibleSplitter1; }
            set { collapsibleSplitter1 = value; }
        }

        public IEventedList<IView> ChildViews
        {
            get { return tabControl.ChildViews; }
        }

        public bool HandlesChildViews
        {
            get { return true; }
        }

        public Func<ILayer, object> GetDataForLayer { get; set; }

        public Func<object, ILayer> GetLayerForData { get; set; }

        public void EnsureVisible(object item)
        {
            // recursive to tabs
            foreach (var view in TabControl.ChildViews)
            {
                try
                {
                    view.EnsureVisible(item);
                }
                catch (Exception) { /* gulp */ }
            }

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
            ILayer featureLayer = mapControl.Map.GetLayerByFeature(feature);
            if (featureLayer == null)
            {
                return;
            }
            EnsureFeatureVisible(feature, layer);
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

        public IView OpenLayerAttributeTable(ILayer layer, Action<object> openViewMethod = null)
        {
            if (!(layer is VectorLayer) || !layer.ShowAttributeTable)
            {
                return null;
            }

            var view = tabControl.ChildViews.OfType<VectorLayerAttributeTableView>().FirstOrDefault(v => v.Data == layer);
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
                    DeleteSelectedFeatures = () => mapControl.DeleteTool.DeleteSelection()
                };

            TabControl.AddView(view);
            
            return view;
        }

        public void ActivateChildView(IView childView)
        {
            TabControl.ActiveView = childView;
            if (IsTabControlVisible && collapsibleSplitter1.IsCollapsed)
                collapsibleSplitter1.ToggleState(); //re-show tabcontrol now
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
                    mapControl.Dispose();
                    tabControl.Dispose();
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
                Console.WriteLine("Constructor stack trace: " + constructorStackTrace);
            }
        }

        private void SubscribeEvents()
        {
            if (Map == null) return;

            Map.CollectionChanged += mapCollectionChangedEventHandler;

            DataBindings.Add("Text", Map, "Name", false, DataSourceUpdateMode.OnPropertyChanged);

            Text = Map.Name;
        }

        private void UnsubscribeEvents()
        {
            if (Map == null) return;

            Map.CollectionChanged -= mapCollectionChangedEventHandler;

            DataBindings.Clear();
        }

        private void mapCollectionChangedEventHandler(object sender, NotifyCollectionChangingEventArgs e)
        {
            var layer = e.Item as ILayer;

            if (e.Action == NotifyCollectionChangeAction.Remove)
            {
                RemoveTabFor(layer);
            }
        }

        [InvokeRequired]
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

            var view = tabControl.ChildViews.FirstOrDefault(v => Equals(v.Data, dataForLayer));
            if (view == null)
            {
                return;
            }

            tabControl.RemoveView(view);
        }

        private void MapControlMouseMove(object sender, MouseEventArgs e)
        {
            var worldLocation = mapControl.Map.ImageToWorld(e.Location);
            if (SharpMapGisGuiPlugin.Instance != null && SharpMapGisGuiPlugin.Instance.Gui != null && worldLocation != null)
            {
                var coordinateSystem = mapControl.Map.CoordinateSystem != null 
                    ? "(" + mapControl.Map.CoordinateSystem.Name + ")"
                    : "";
                var message = string.Format("Current map coordinates {0} : {1}, {2}", coordinateSystem, worldLocation.X, worldLocation.Y);
                // HACK:....
                if (SharpMapGisGuiPlugin.Instance.Gui.MainWindow != null)
                    SharpMapGisGuiPlugin.Instance.Gui.MainWindow.StatusBarMessage = message;
            }
        }

        private void MapControlMouseLeave(object sender, EventArgs e)
        {
            // HACK:....
            // HACK, TODO: move to GuiPlugin or some service class
            if (SharpMapGisGuiPlugin.Instance != null && SharpMapGisGuiPlugin.Instance.Gui != null)
            {
                if (SharpMapGisGuiPlugin.Instance.Gui.MainWindow != null)
                    SharpMapGisGuiPlugin.Instance.Gui.MainWindow.StatusBarMessage = "";
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
            var features = mapControl.SelectedFeatures;
            if (features == null || settingSelection) return;

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
            if (mapViewEditor == null || settingSelection) return;

            settingSelection = true;
            mapControl.SelectTool.Select(mapViewEditor.SelectedFeatures);
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

                if (IsTabControlVisible && !tabControl.ChildViews.Any())
                {
                    IsTabControlVisible = false;
                }
            }
        }

        public IEnumerable<System.Tuple<string, object>> SearchItemsByText(string text, bool caseSensitive, Func<bool> isSearchCancelled, Action<int> setProgressPercentage)
        {
            var visibleLayers = Map.GetAllVisibleLayers(false).Where(l => l.DataSource != null && l.DataSource.Features != null).ToList();
            if (visibleLayers.Count == 0) yield break;

            var percentageStep = 100.0 / visibleLayers.Count;
            var currentPercentage = 0.0;

            foreach (var layer in visibleLayers)
            {
                if (isSearchCancelled())
                {
                    yield break;
                }

                var matchingItems = layer.DataSource.Features.OfType<INameable>().
                                          Where(n => n.Name != null &&
                                                     (caseSensitive
                                                          ? n.Name.Contains(text)
                                                          : n.Name.ToLower().Contains(text.ToLower())));

                foreach (var item in matchingItems)
                {
                    yield return new System.Tuple<string, object>(string.Format("{0} ({1})", item.Name, layer.Name), item);
                }

                currentPercentage += percentageStep;
                setProgressPercentage((int) currentPercentage);
            }
        }
    }
}