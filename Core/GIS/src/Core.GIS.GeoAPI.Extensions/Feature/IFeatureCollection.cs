using System.Collections.Generic;

namespace GeoAPI.Extensions.Feature
{
    public interface IFeatureCollection<T>: IList<T> where T : IFeature
    {
    }
}
