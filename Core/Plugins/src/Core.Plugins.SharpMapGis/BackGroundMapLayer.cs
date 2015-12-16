using System.Collections.Generic;
using System.Linq;
using Core.Common.Utils.Collections;
using Core.GIS.SharpMap.Api.Collections;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Map;
using Core.Plugins.SharpMapGis.Properties;
using log4net;

namespace Core.Plugins.SharpMapGis
{
    /// <summary>
    /// Layer group based on a map. Contains the layers of the source map.
    /// </summary>
    public class BackGroundMapLayer : GroupLayer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BackGroundMapLayer));

        private Map lastUpdatedMap; // TODO: what is this for??

        private Map backgroundMap;
        private bool backgroundMapChanged;

        public BackGroundMapLayer()
        {
            Selectable = false;
        }

        public BackGroundMapLayer(Map map) : this()
        {
            BackgroundMap = map;
        }

        public override EventedList<ILayer> Layers
        {
            get
            {
                if (backgroundMapChanged)
                {
                    backgroundMapChanged = false;
                    UpdateLayers();
                }

                return base.Layers;
            }

            set
            {
                base.Layers = value;
            }
        }

        public override bool ReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Map used a basis for this grouplayer
        /// </summary>
        public Map BackgroundMap
        {
            get
            {
                return backgroundMap;
            }
            set
            {
                UnsubscribeFromBackgroundMap();
                backgroundMap = value;
                SubscribeToBackgroundMap();

                // mark background map as dirty to make sure that layers will be updated (using lazy way)
                backgroundMapChanged = true;
                RenderRequired = true;
            }
        }

        public void UnsubscribeFromBackgroundMap()
        {
            if (backgroundMap == null)
            {
                return;
            }
            ((INotifyCollectionChanged) backgroundMap).CollectionChanged -= BackgroundMapCollectionChanged;
        }

        public void SubscribeToBackgroundMap()
        {
            if (backgroundMap == null)
            {
                return;
            }

            ((INotifyCollectionChanged) backgroundMap).CollectionChanged += BackgroundMapCollectionChanged;
        }

        public void UpdateLayers()
        {
            if (BackgroundMap == null)
            {
                Layers.Clear();
                return;
            }

            Name = string.Format(Resources.BackGroundMapLayer_UpdateLayers_Background_BackgroundMapName, BackgroundMap.Name);

            // check if we need to update layers
            bool updateLayers = false;

            if (Layers.Count != backgroundMap.Layers.Count)
            {
                updateLayers = true;
            }
            else
            {
                for (var i = 0; i < Layers.Count; i++)
                {
                    if (Layers[i].DataSource != backgroundMap.Layers[i].DataSource
                        || Layers[i].Visible != backgroundMap.Layers[i].Visible)
                    {
                        updateLayers = true;
                        break;
                    }
                }
            }

            if (!updateLayers)
            {
                return;
            }

            UnsubscribeFromBackgroundMap();

            bool mapChanged = lastUpdatedMap != BackgroundMap;
            bool layersChanged = Layers.Count != BackgroundMap.Layers.Count;
            bool storeEnabled = !mapChanged && !layersChanged;
            //storeEnabled = false;
            var wasEnabled = new List<bool>();
            if (storeEnabled)
            {
                wasEnabled = Layers.Select(l => l.Visible).ToList();
            }

            Layers.Clear();
            foreach (var layer in BackgroundMap.Layers)
            {
                var clone = (ILayer) layer.Clone();
                clone.Selectable = false;
                Layers.Add(clone);
            }

            if (storeEnabled)
            {
                for (int i = 0; i < Layers.Count; i++)
                {
                    Layers[i].Visible = wasEnabled[i];
                }
            }

            //set the map of the group layer to check if it was changed after
            lastUpdatedMap = BackgroundMap;
            //set renderrequired to true because we want the treeview and legendview to update...

            SubscribeToBackgroundMap();
        }

        private void BackgroundMapCollectionChanged(object sender, NotifyCollectionChangeEventArgs e)
        {
            backgroundMapChanged = true;
            RenderRequired = true;
        }
    }
}