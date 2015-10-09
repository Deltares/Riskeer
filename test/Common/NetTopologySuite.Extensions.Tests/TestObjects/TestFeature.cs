using System;
using System.ComponentModel;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;

namespace NetTopologySuite.Extensions.Tests.TestObjects
{
    //Just a subclass
    internal class TestFeatureSubClass : TestFeature {}

    internal class TestFeature : IFeature
    {
        [FeatureAttribute(Order = 2)]
        public string Name { get; set; }

        [DisplayName("Kees")]
        [FeatureAttribute(Order = 1, ExportName = "Piet")]
        public string Other { get; set; }

        public IGeometry Geometry { get; set; }

        public IFeatureAttributeCollection Attributes { get; set; }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}