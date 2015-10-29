using System;
using System.Collections.Generic;

namespace Core.GIS.GeoApi.Extensions.Feature
{
    public interface IFeatureAttributeCollection : IDictionary<string, object>, ICloneable {}
}