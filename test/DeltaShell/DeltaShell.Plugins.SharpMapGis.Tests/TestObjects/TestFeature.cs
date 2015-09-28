using System;
using System.ComponentModel;
using DelftTools.Utils.Data;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;

namespace DeltaShell.Plugins.SharpMapGis.Tests.TestObjects
{

    public class TestFeature : Unique<long>, IFeature
    {
        [DisplayName("DisplayName")]
        [FeatureAttribute]
        public string Name { get; set; }

        [FeatureAttribute]
        public int Value{ get; set; }
        
        public object Clone()
        {
            throw new NotImplementedException();
        }

        public IGeometry Geometry
        {
            get; set;
        }

        public IFeatureAttributeCollection Attributes
        {
            get; set; 
        }
    }
}