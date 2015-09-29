using System.Collections.Generic;
using System.Linq;

namespace SharpMap.Api.Layers
{
    public static class LayerExtensions
    {
        public static IEnumerable<ILayer> GetLayersRecursive(this ILayer layer)
        {
            yield return layer;

            var groupLayer = layer as IGroupLayer;
            if (groupLayer == null) yield break;

            foreach (var subLayer in groupLayer.Layers.SelectMany(GetLayersRecursive))
            {
                yield return subLayer;
            }
        }

        public static IEnumerable<T> GetLayersRecursive<T>(this ILayer layer)
        {
            if (layer is T)
            {
                yield return (T) layer;
            }

            var groupLayer = layer as IGroupLayer;
            if (groupLayer == null) yield break;

            foreach (var typedLayer in groupLayer.Layers.SelectMany(GetLayersRecursive<T>))
            {
                yield return typedLayer;
            }
        }

        public static void DisposeLayersRecursive(this ILayer layer, bool disposeDataSource = true)
        {
            var disposableLayers = GetLayersRecursive<ILayer>(layer).ToList();

            foreach (var disposableLayer in disposableLayers)
            {
                disposableLayer.Dispose(disposeDataSource);
            }
        }
    }
}