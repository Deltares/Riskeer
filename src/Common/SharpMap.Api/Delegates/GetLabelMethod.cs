using GeoAPI.Extensions.Feature;

namespace SharpMap.Api.Delegates
{
    /// <summary>
    /// Delegate method for creating advanced label texts
    /// </summary>
    /// <param name="fdr"></param>
    /// <returns></returns>
    public delegate string GetLabelMethod(IFeature fdr);
}