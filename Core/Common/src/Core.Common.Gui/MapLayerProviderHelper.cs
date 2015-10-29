using System;
using System.Collections.Generic;
using System.Linq;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Api.Layers;

namespace Core.Common.Gui
{
    public static class MapLayerProviderHelper
    {
        public static ILayer CreateLayersRecursive(object layerObject, object parentObject,
                                                   IList<IMapLayerProvider> mapLayerProviders, IDictionary<ILayer, object> ld = null)
        {
            var mapLayerProvider =
                mapLayerProviders.Where(p => p != null)
                                 .FirstOrDefault(p => p.CanCreateLayerFor(layerObject, parentObject));

            if (mapLayerProvider == null)
            {
                return null;
            }

            var layer = mapLayerProvider.CreateLayer(layerObject, parentObject);

            var groupLayer = layer as IGroupLayer;
            if (groupLayer != null)
            {
                ModifyGroupLayer(groupLayer,
                                 () =>
                                 {
                                     var childObjects = mapLayerProvider.ChildLayerObjects(layerObject);
                                     var childLayers = childObjects.Select(
                                         o =>
                                         CreateLayersRecursive(o, layerObject,
                                                               ReorderProviders(mapLayerProvider, mapLayerProviders), ld))
                                                                   .Where(childLayer => childLayer != null);

                                     groupLayer.Layers.AddRange(childLayers);
                                 });
            }

            if (layer != null && ld != null)
            {
                ld[layer] = layerObject;
            }

            return layer;
        }

        public static void RefreshLayersRecursive(ILayer layer, IDictionary<ILayer, object> layerObjectDictionary, IList<IMapLayerProvider> mapLayerProviders, object parentObject)
        {
            var groupLayer = layer as IGroupLayer;
            if (groupLayer == null)
            {
                return;
            }

            var groupLayerObject = layerObjectDictionary.ContainsKey(groupLayer) ? layerObjectDictionary[groupLayer] : null;
            if (groupLayerObject == null)
            {
                return;
            }

            var mapLayerProvider = mapLayerProviders.Where(p => p != null).First(p => p.CanCreateLayerFor(groupLayerObject, parentObject));
            var currentdataObjects = mapLayerProvider.ChildLayerObjects(groupLayerObject).Where(item => item != null).ToList();
            mapLayerProviders = ReorderProviders(mapLayerProvider, mapLayerProviders);
            var layerDataObjects = groupLayer.Layers.Where(layerObjectDictionary.ContainsKey)
                                             .Select(l => layerObjectDictionary[l])
                                             .ToList();

            if (layerDataObjects.Count != currentdataObjects.Count || layerDataObjects.Union(currentdataObjects).Count() != layerDataObjects.Count)
            {
                // remember old objects and layers
                var objectLayerDictionary = layerDataObjects.Intersect(currentdataObjects)
                                                            .ToDictionary(o => o, o => layerObjectDictionary.FirstOrDefault(kv => kv.Value == o).Key);

                ModifyGroupLayer(groupLayer, () =>
                {
                    // remove layers no longer up to date with current data
                    var layersToRemove = groupLayer.Layers.Where(l => layerObjectDictionary.ContainsKey(l) &&
                                                                      !currentdataObjects.Contains(
                                                                          layerObjectDictionary[l])).ToList();
                    layersToRemove.ForEach(l =>
                    {
                        groupLayer.Layers.Remove(l);

                        // Call dispose on all layers that implement it
                        l.DisposeLayersRecursive();

                        // clear all administration for the removed layer
                        ClearLayerDictionaryRecursive(l, layerObjectDictionary);
                    });

                    var index = 0;
                    foreach (var dataObject in currentdataObjects)
                    {
                        if (objectLayerDictionary.ContainsKey(dataObject))
                        {
                            // nothing to do on this level, but we do need to check the sublayers recursively
                            var existingLayer = objectLayerDictionary[dataObject];
                            RefreshLayersRecursive(existingLayer, layerObjectDictionary, mapLayerProviders, groupLayerObject);
                        }
                        else
                        {
                            // no layer found for this data, create new
                            var layerForData = CreateLayersRecursive(dataObject, groupLayerObject, mapLayerProviders,
                                                                     layerObjectDictionary);
                            if (layerForData == null)
                            {
                                continue; // no index increment
                            }
                            groupLayer.Layers.Insert(index, layerForData);
                            SetMapRecursive(layerForData, groupLayer.Map);
                        }
                        index++;
                    }
                });

                layerObjectDictionary[groupLayer] = groupLayerObject;
            }
            else
            {
                foreach (var subLayer in groupLayer.Layers)
                {
                    RefreshLayersRecursive(subLayer, layerObjectDictionary, mapLayerProviders, groupLayerObject);
                }
            }
        }

        /// <summary>
        /// Reorder the providers such that the 'first chance' provider is returned first (get's the first chance). This way a provider returning a 
        /// 'child object', is queried first for any layers it may have for that object, before other providers are queried.
        /// </summary>
        /// <param name="firstChanceProvider"></param>
        /// <param name="allProviders"></param>
        /// <returns></returns>
        private static IList<IMapLayerProvider> ReorderProviders(IMapLayerProvider firstChanceProvider, IEnumerable<IMapLayerProvider> allProviders)
        {
            return new[]
            {
                firstChanceProvider
            }.Concat(allProviders.Except(new[]
            {
                firstChanceProvider
            })).ToList();
        }

        private static void SetMapRecursive(ILayer layer, IMap map)
        {
            layer.Map = map;

            var groupLayer = layer as IGroupLayer;
            if (groupLayer == null)
            {
                return;
            }

            foreach (var subLayer in groupLayer.Layers)
            {
                SetMapRecursive(subLayer, map);
            }
        }

        private static void ModifyGroupLayer(IGroupLayer groupLayer, Action groupLayerAction)
        {
            var hasReadOnlyLayersCollection = groupLayer.LayersReadOnly;
            bool visible = groupLayer.Visible;

            try
            {
                // suppress that the layer will be rendered while modifying.
                groupLayer.Visible = false;
                groupLayer.LayersReadOnly = false;
                groupLayerAction();
            }
            finally
            {
                // return the values that where on the group layer.
                groupLayer.Visible = visible;
                groupLayer.LayersReadOnly = hasReadOnlyLayersCollection;
            }
        }

        private static void ClearLayerDictionaryRecursive(ILayer layer, IDictionary<ILayer, object> layerObjectDictionary)
        {
            if (layerObjectDictionary.ContainsKey(layer))
            {
                layerObjectDictionary.Remove(layer);
            }

            var groupLayer = layer as IGroupLayer;
            if (groupLayer == null)
            {
                return;
            }

            foreach (var subLayer in groupLayer.Layers)
            {
                ClearLayerDictionaryRecursive(subLayer, layerObjectDictionary);
            }
        }
    }
}