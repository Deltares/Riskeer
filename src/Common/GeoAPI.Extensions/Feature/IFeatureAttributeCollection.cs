using System;
using System.Collections.Generic;

namespace GeoAPI.Extensions.Feature
{
    public interface IFeatureAttributeCollection : IDictionary<string, object>, ICloneable {}
}