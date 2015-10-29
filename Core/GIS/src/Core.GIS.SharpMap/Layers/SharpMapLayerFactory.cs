using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Api.Layers;

namespace Core.GIS.SharpMap.Layers
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