using System;
using System.ComponentModel;
using Core.GIS.GeoApi.Extensions.Feature;
using Core.GIS.GeoApi.Geometries;

namespace Core.GIS.NetTopologySuite.Extensions.Tests.TestObjects
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