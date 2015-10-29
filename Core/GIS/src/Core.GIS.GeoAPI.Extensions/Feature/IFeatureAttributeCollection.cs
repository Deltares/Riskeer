using System;
using System.Collections.Generic;

namespace Core.Gis.GeoApi.Extensions.Feature
{
    public interface IFeatureAttributeCollection : IDictionary<string, object>, ICloneable {}
}