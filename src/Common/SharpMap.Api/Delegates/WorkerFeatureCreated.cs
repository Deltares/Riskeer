using GeoAPI.Extensions.Feature;

namespace SharpMap.Api.Delegates
{
    public delegate void WorkerFeatureCreated(IFeature sourceFeature, IFeature workFeature);
}