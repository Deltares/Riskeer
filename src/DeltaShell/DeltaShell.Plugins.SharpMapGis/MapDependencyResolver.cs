using System.Collections.Generic;
using System.Linq;
using SharpMap;
using SharpMap.Api.Layers;

namespace DeltaShell.Plugins.SharpMapGis
{
    /// <summary>
    /// Currently only support for chilc data item IRegularGridCoverage
    /// </summary>
    public class MapDependencyResolver
    {
        private static IEnumerable<ILayer> GetMapLayersForData(Map map, object value)
        {
            foreach (var layer in map.GetAllLayers(true).ToArray())
            {
                if ((layer is BackGroundMapLayer) && ((layer as BackGroundMapLayer).BackgroundMap == value))
                {
                    yield return layer;
                }
            }
        }

        public static void RemoveItemsFromMap(Map map, object child)
        {
            var layers = GetMapLayersForData(map, child);

            foreach (var layer in layers)
            {
                if (map.Layers.Contains(layer))
                {
                    map.Layers.Remove(layer);
                }
                else
                {
                    var groupLayer = map.GetGroupLayerContainingLayer(layer);
                    if (groupLayer != null && !groupLayer.LayersReadOnly)
                    {
                        groupLayer.Layers.Remove(layer);
                    }
                }
            }
        }
    }
}