using Core.GIS.GeoAPI.Extensions.Feature;

namespace Core.GIS.SharpMap.Api.Delegates
{
    public delegate void WorkerFeatureCreated(IFeature sourceFeature, IFeature workFeature);
}