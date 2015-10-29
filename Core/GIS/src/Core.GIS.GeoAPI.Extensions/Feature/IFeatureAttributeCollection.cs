using System;
using System.Collections.Generic;

namespace Core.GIS.GeoAPI.Extensions.Feature
{
    public interface IFeatureAttributeCollection : IDictionary<string, object>, ICloneable {}
}