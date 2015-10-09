using SharpMap.Api;
using SharpMap.Api.Layers;

namespace SharpMap.Layers
{
    public static class SharpMapLayerFactory
    {
        public static ILayer CreateLayer(IFeatureProvider featureProvider)
        {
            return new VectorLayer
            {
                ReadOnly = true,
                DataSource = featureProvider
            };
        }
    }
}