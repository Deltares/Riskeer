using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Core.GIS.GeoAPI.Extensions.Feature;

namespace Core.GIS.NetTopologySuite.Extensions.Features
{
    [Serializable]
    public class DictionaryFeatureAttributeCollection : Dictionary<string, object>, IFeatureAttributeCollection
    {
        public DictionaryFeatureAttributeCollection() {}

        protected DictionaryFeatureAttributeCollection(SerializationInfo info, StreamingContext context) : base(info, context) {}

        public object Clone()
        {
            var copy = new DictionaryFeatureAttributeCollection();
            foreach (var attribute in this)
            {
                copy[attribute.Key] = attribute.Value;
            }
            return copy;
        }
    }
}