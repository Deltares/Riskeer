using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.BaseDelftTools;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Utils;
using Core.Common.Utils.Collections;
using Core.Common.Utils.Collections.Generic;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.Layers;
using Core.Plugins.SharpMapGis.Gui.Properties;
using IEditableObject = Core.Common.Utils.Editing.IEditableObject;

namespace Core.Plugins.SharpMapGis.Gui.Forms
{
    public partial class ProjectItemMapView : UserControl, ICompositeView, ISearchableView
    {
        private readonly Dictionary<ILayer, object> layerDataDictionary = new Dictionary<ILayer, object>();
        private readonly IEventedList<IView> childViews = new EventedList<IView>();
        public Action<ILayer, Dictionary<ILayer, object>, object> RefreshLayersAction;
        private IProjectItem mapData;
        private Func<object, Dictionary<ILayer, object>, ILayer> createLayerForData;
        private ILayer mapDataLayer;
        private bool viewChangeInitiatedHere;

        private int IsEditingCount = 0;

        public ProjectItemMapView()
        {
            InitializeComponent();
            AddMapView();

            childViews.Add(MapView);
            childViews.CollectionChanged += (sender, args) =>
            {
                if (viewChangeInitiatedHere)
                {
                    return; // prevent recursion
                }

                var layerEditor = args.Item as ILayerEditorView;
                if (layerEditor == null)
                {
                    return;
                }

                viewChangeInitiatedHere = true;
                try
                {
                    switch (args.Action)
                    {
                        case NotifyCollectionChangeAction.Add:
                        {
                            var layer = MapView.GetLayerForData(layerEditor.Data);
                            if (layer != null)
                            {
                                layerEditor.Layer = layer;
                            }
                            MapView.TabControl.AddView(layerEditor);
                        }
                            break;
                        case NotifyCollectionChangeAction.Remove:
                            layerEditor.Layer = null;

                            MapView.TabControl.RemoveView(layerEditor);
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

        public MapView MapView { get; private set; }

        public Func<object, Dictionary<ILayer, object>, ILayer> CreateLayerForData
        {
            get
            {
                return createLayerForData;
            }
            set
            {
                createLayerForData = value;
                AddMapDataLayer();
            }
        }

        public object Data
        {
            get
            {
                return mapData;
            }
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
            get
            {
                return Resources.Map;
            }
            set {}
        }

        public IEventedList<IView> ChildViews
        {
            get
            {
                return childViews;
            }
        }

        public bool HandlesChildViews
        {
            get
            {
                return false;
            }
        }

        public ViewInfo ViewInfo { get; set; }

        public void RefreshLayers()
        {
            if (RefreshLayersAction != null)
            {
                RefreshLayersAction(mapDataLayer, layerDataDictionary, null);
            }
        }

        public void ActivateChildView(IView childView)
        {
            MapView.ActivateChildView(childView);
        }

        public void EnsureVisible(object item)
        {
            if (MapView != null)
            {
                MapView.EnsureVisible(item);
            }
        }

        public IEnumerable<System.Tuple<string, object>> SearchItemsByText(string text, bool caseSensitive, Func<bool> isSearchCancelled,
                                                                           Action<int> setProgressPercentage)
        {
            return MapView.SearchItemsByText(text, caseSensitive, isSearchCancelled, setProgressPercentage);
        }

        protected override void Dispose(bool disposing)
        {
            if (MapView != null)
            {
                if (disposing)
                {
                    MapView.TabControl.BeforeDispose();
                }
                MapView.TabControl.ViewCollectionChanged -= MapTabCollectionChanged;
                MapView.GetLayerForData = null;
                MapView.GetDataForLayer = null;

                if (MapView.Map != null)
                {
                    var map = MapView.Map;
                    MapView.Map = null; // prevent further rendering
                    // project item map view owns the map, so we should dispose the layers here:
                    foreach (var layer in map.Layers)
                    {
                        layer.DisposeLayersRecursive();
                    }
                }

                if (disposing)
                {
                    MapView.Dispose();
                }
                MapView = null;
            }

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

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

        private void MapDataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(Equals(sender, mapData)) || e.PropertyName != "IsEditing")
            {
                return;
            }

            var editableObject = sender as IEditableObject;
            if (editableObject == null)
            {
                return;
            }

            if (editableObject.IsEditing)
            {
                IsEditingCount++;
            }
            else if (IsEditingCount > 0)
            {
                IsEditingCount--;
            }

            if (IsEditingCount != 0)
            {
                return;
            }

            RefreshLayers();
        }

        private void MapDataCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            //only refresh when IsEditing is false, set by the MapDataPropertyChanged, checked by the count (IsEditingCount) being 0
            if (IsEditingCount != 0)
            {
                return;
            }

            RefreshLayers();
        }

        private void AddMapView()
        {
            MapView = new MapView
            {
                Dock = DockStyle.Fill,
                Map =
                {
                    Name = "Map"
                },
                IsTabControlVisible = false,
                GetDataForLayer = layer => layerDataDictionary != null && layerDataDictionary.ContainsKey(layer)
                                               ? layerDataDictionary[layer]
                                               : null,
                GetLayerForData = o => layerDataDictionary != null && layerDataDictionary.ContainsValue(o)
                                           ? layerDataDictionary.FirstOrDefault(x => x.Value == o).Key
                                           : null
            };

            MapView.TabControl.ViewCollectionChanged += MapTabCollectionChanged;

            Controls.Add(MapView);
        }

        private void MapTabCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (viewChangeInitiatedHere)
            {
                return;
            }

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

            MapView.Map.Layers.Clear();

            mapDataLayer = createLayerForData(mapData, layerDataDictionary);

            if (mapDataLayer != null)
            {
                MapView.Map.Layers.Add(mapDataLayer);
                mapDataLayer.CanBeRemovedByUser = false;
            }

            if (mapDataLayer != null && (mapDataLayer.Envelope == null || mapDataLayer.Envelope.Area <= 1))
            {
                MapView.Map.ZoomToFit(new Envelope(new Coordinate(0, 0), new Coordinate(2000, 2000)));
            }
            else
            {
                MapView.Map.ZoomToExtents();
            }
        }

        private static IEnumerable<KeyValuePair<string, ILayer>> CreateLayerByPathLookup(ILayer layer, string path)
        {
            yield return new KeyValuePair<string, ILayer>(path, layer);

            var groupLayer = layer as GroupLayer;
            if (groupLayer == null)
            {
                yield break;
            }

            var parentPath = string.Format("{0}\\{1}", path, groupLayer.Name);
            foreach (var kvp in groupLayer.Layers.SelectMany(subLayer => CreateLayerByPathLookup(subLayer, parentPath)))
            {
                yield return kvp;
            }
        }
    }
}