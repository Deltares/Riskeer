using Core.GIS.GeoAPI.Extensions.Feature;

namespace Core.GIS.SharpMap.Api.Delegates
{
    /// <summary>
    /// Delegate method for creating advanced label texts
    /// </summary>
    /// <param name="fdr"></param>
    /// <returns></returns>
    public delegate string GetLabelMethod(IFeature fdr);
}