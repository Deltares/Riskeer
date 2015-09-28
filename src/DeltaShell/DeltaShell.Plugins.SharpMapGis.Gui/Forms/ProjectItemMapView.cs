using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DelftTools.Utils;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Collections.Generic;
using DelftTools.Utils.Editing;
using GisSharpBlog.NetTopologySuite.Geometries;
using SharpMap.Api.Layers;
using SharpMap.Layers;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms
{
    public partial class ProjectItemMapView : UserControl, ICompositeView, ISearchableView
    {
        private MapView mapView;
        private IProjectItem mapData;
        private Func<object, Dictionary<ILayer, object>, ILayer> createLayerForData;
        private readonly Dictionary<ILayer, object> layerDataDictionary = new Dictionary<ILayer, object>();
        private ILayer mapDataLayer;
        public Action<ILayer, Dictionary<ILayer, object>, object> RefreshLayersAction;
        private readonly IEventedList<IView> childViews = new EventedList<IView>();
        private bool viewChangeInitiatedHere;

        public ProjectItemMapView()
        {
            InitializeComponent();
            AddMapView();

            childViews.Add(mapView);
            childViews.CollectionChanged += (sender, args) =>
            {
                if (viewChangeInitiatedHere)
                    return; // prevent recursion

                var layerEditor = args.Item as ILayerEditorView;
                if (layerEditor == null) return;

                viewChangeInitiatedHere = true;
                try
                {
                    switch (args.Action)
                    {
                        case NotifyCollectionChangeAction.Add:
                        {
                            var layer = mapView.GetLayerForData(layerEditor.Data);
                            if (layer != null)
                            {
                                layerEditor.Layer = layer;
                            }
                            mapView.TabControl.AddView(layerEditor);
                        }
                            break;
                        case NotifyCollectionChangeAction.Remove:
                            layerEditor.Layer = null;

                            mapView.TabControl.RemoveView(layerEditor);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                finally
                {
                    viewChangeInitiatedHere = false;
                }
            };
        }
        
        public MapView MapView { get { return mapView; } }

        public Func<object, Dictionary<ILayer, object>, ILayer> CreateLayerForData
        {
            get { return createLayerForData; }
            set
            {
                createLayerForData = value;
                AddMapDataLayer();
            }
        }

        public object Data
        {
            get { return mapData; }
            set
            {
                UnSubscribe(); 

                mapData = (IProjectItem) value;

                AddMapDataLayer();

                Subscribe();
            }
        }

        public Image Image
        {
            get { return Properties.Resources.Map; }
            set
            {
                
            }
        }

        public IEventedList<IView> ChildViews
        {
            get { return childViews; }
        }

        public bool HandlesChildViews { get { return false; } }
        
        public void ActivateChildView(IView childView)
        {
            mapView.ActivateChildView(childView);
        }

        public void EnsureVisible(object item)
        {
            if (mapView != null)
                mapView.EnsureVisible(item);
        }

        public ViewInfo ViewInfo { get; set; }

        private void UnSubscribe()
        {
            var notifyCollectionChange = mapData as INotifyCollectionChange;
            if (notifyCollectionChange != null)
            {
                notifyCollectionChange.CollectionChanged -= MapDataCollectionChanged;
            }

            var notifyPropertyChange = mapData as INotifyPropertyChange;
            if (notifyPropertyChange != null)
            {
                notifyPropertyChange.PropertyChanged -= MapDataPropertyChanged;
            }
        }

        private void Subscribe()
        {
            var notifyCollectionChange = mapData as INotifyCollectionChange;
            if (notifyCollectionChange != null)
            {
                notifyCollectionChange.CollectionChanged += MapDataCollectionChanged;
            }

            var notifyPropertyChange = mapData as INotifyPropertyChange;
            if (notifyPropertyChange != null)
            {
                notifyPropertyChange.PropertyChanged += MapDataPropertyChanged;
            }
        }

        private int IsEditingCount = 0;
        
        void MapDataPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(Equals(sender, mapData)) || e.PropertyName != "IsEditing")
            {
                 return;
            }
            
            var editableObject = sender as IEditableObject;
            if (editableObject == null) return;

            if (editableObject.IsEditing)
                IsEditingCount++;
            else if (IsEditingCount > 0)
                IsEditingCount--;

            if(IsEditingCount != 0) return;

            RefreshLayers();
        }

        private void MapDataCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            //only refresh when IsEditing is false, set by the MapDataPropertyChanged, checked by the count (IsEditingCount) being 0
            if (IsEditingCount != 0) return;

            RefreshLayers();
        }

        public void RefreshLayers()
        {
            if (RefreshLayersAction != null)
            {
                RefreshLayersAction(mapDataLayer, layerDataDictionary, null);
            }
        }

        private void AddMapView()
        {
            mapView = new MapView
                          {
                              Dock = DockStyle.Fill,
                              Map = {Name = "Map"},
                              IsTabControlVisible = false,
                              GetDataForLayer = layer => layerDataDictionary != null && layerDataDictionary.ContainsKey(layer)
                                                             ? layerDataDictionary[layer]
                                                             : null,
                              GetLayerForData = o => layerDataDictionary != null && layerDataDictionary.ContainsValue(o)
                                                         ? layerDataDictionary.FirstOrDefault(x => x.Value == o).Key
                                                         : null
                          };

            mapView.TabControl.ViewCollectionChanged += MapTabCollectionChanged;

            Controls.Add(mapView);
        }

        private void MapTabCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (viewChangeInitiatedHere)
                return;

            // apparently a tab was added / remove through another path (Open Layer Editor in map legend, 
            // for example); update our child views
            viewChangeInitiatedHere = true;
            try
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        childViews.Add((IView) e.NewItems[0]);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        childViews.Remove((IView) e.OldItems[0]);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            finally
            {
                viewChangeInitiatedHere = false;
            }
        }

        private void AddMapDataLayer()
        {
            mapDataLayer = null;

            layerDataDictionary.Clear();

            if (mapData == null || createLayerForData == null)
            {
                return;
            }

            mapView.Map.Layers.Clear();

            mapDataLayer = createLayerForData(mapData, layerDataDictionary);

            if (mapDataLayer != null)
            {
                mapView.Map.Layers.Add(mapDataLayer);
                mapDataLayer.CanBeRemovedByUser = false;
            }

            if (mapDataLayer != null && (mapDataLayer.Envelope == null || mapDataLayer.Envelope.Area <= 1))
            {
                mapView.Map.ZoomToFit(new Envelope(new Coordinate(0, 0), new Coordinate(2000, 2000)));
            }
            else
            {
                mapView.Map.ZoomToExtents();
            }
        }

        private static IList<GeneratedMapLayerInfo> CreateMapLayerInfoList(ILayer layer)
        {
            return CreateLayerByPathLookup(layer, "")
                    .Select(pathLayerKvp => new GeneratedMapLayerInfo(pathLayerKvp.Value) {ParentPath = pathLayerKvp.Key})
                    .ToList();
        }

        private static IEnumerable<KeyValuePair<string, ILayer>> CreateLayerByPathLookup(ILayer layer, string path)
        {
            yield return new KeyValuePair<string, ILayer>(path, layer);

            var groupLayer = layer as GroupLayer;
            if (groupLayer == null) yield break;

            var parentPath = string.Format("{0}\\{1}", path, groupLayer.Name);
            foreach (var kvp in groupLayer.Layers.SelectMany(subLayer => CreateLayerByPathLookup(subLayer, parentPath)))
            {
                yield return kvp;
            }
        }

        private static void RestoreGeneratedMapLayerData(ILayer layer, IEnumerable<GeneratedMapLayerInfo> generatedMapLayerInfos)
        {
            var layerByPathLookup = CreateLayerByPathLookup(layer, "").ToDictionary(kvp => string.Format("{0}\\{1}", kvp.Key, kvp.Value.Name), kvp => kvp.Value);
            var generatedMapLayerInfoByPath = generatedMapLayerInfos.Select(i => new KeyValuePair<string, GeneratedMapLayerInfo>(i.ParentPath, i));

            foreach (var pathGeneratedMapLayerInfoKvp in generatedMapLayerInfoByPath)
            {
                var key = string.Format("{0}\\{1}", pathGeneratedMapLayerInfoKvp.Key, pathGeneratedMapLayerInfoKvp.Value.Name);
                
                ILayer generatedLayer;
                layerByPathLookup.TryGetValue(key, out generatedLayer);
                if (generatedLayer == null) return;

                pathGeneratedMapLayerInfoKvp.Value.SetToLayer(generatedLayer);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (mapView != null)
            {
                if (disposing)
                {
                    mapView.TabControl.BeforeDispose();
                }
                mapView.TabControl.ViewCollectionChanged -= MapTabCollectionChanged;
                mapView.GetLayerForData = null;
                mapView.GetDataForLayer = null;

                if (mapView.Map != null)
                {
                    var map = mapView.Map;
                    mapView.Map = null; // prevent further rendering
                    // project item map view owns the map, so we should dispose the layers here:
                    foreach (var layer in map.Layers)
                        layer.DisposeLayersRecursive();
                }

                if (disposing)
                    mapView.Dispose();
                mapView = null;
            }

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public IEnumerable<System.Tuple<string, object>> SearchItemsByText(string text, bool caseSensitive, Func<bool> isSearchCancelled,
            Action<int> setProgressPercentage)
        {
            return mapView.SearchItemsByText(text, caseSensitive, isSearchCancelled, setProgressPercentage);
        }
    }
}
