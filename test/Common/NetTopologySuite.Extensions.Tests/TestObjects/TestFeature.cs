using System;
using System.ComponentModel;
using DelftTools.Utils.Data;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;

namespace NetTopologySuite.Extensions.Tests.TestObjects
{
    //Just a subclass
    class TestFeatureSubClass : TestFeature
    {

    }

    class TestFeature : Unique<long>, IFeature
    {
        [FeatureAttribute(Order = 2)]
        public string Name { get; set; }

        [DisplayName("Kees")]
        [FeatureAttribute(Order = 1, ExportName = "Piet")]
        public string Other { get; set; }
        
        public object Clone()
        {
            throw new NotImplementedException();
        }

        public IGeometry Geometry { get; set; }

        public IFeatureAttributeCollection Attributes { get; set; }
    }
}
