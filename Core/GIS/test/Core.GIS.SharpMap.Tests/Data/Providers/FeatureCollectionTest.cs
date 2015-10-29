using System;
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Utils;
using Core.Common.Utils.Collections.Generic;
using Core.Gis.GeoApi.Extensions.Feature;
using Core.Gis.GeoApi.Geometries;
using Core.GIS.NetTopologySuite.Extensions.Features;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.Data.Providers;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Tests.Data.Providers
{
    [TestFixture]
    public class FeatureCollectionTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void AddingInvalidTypeGivesArgumentException()
        {
            new FeatureCollection(new List<IFeature>(), typeof(string));
        }

        [Test]
        public void ChangingFeatureGeometryShouldTriggerFeaturesChangedEvent()
        {
            var feature = new EventedFeature
            {
                Geometry = new Point(0, 0)
            };
            var list = new EventedList<IFeature>
            {
                feature
            };
            var featureCollection = new FeatureCollection(list, typeof(Feature));

            var count = 0;
            featureCollection.FeaturesChanged += (s, e) => count++;

            feature.Geometry = new Point(1, 1);

            Assert.AreEqual(1, count, "FeaturesChanged event should be generated.");
        }

        private class EventedFeature : Feature, INotifyPropertyChange
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public event PropertyChangingEventHandler PropertyChanging;
            private IGeometry geometry;

            public override IGeometry Geometry
            {
                get
                {
                    return geometry;
                }
                set
                {
                    if (PropertyChanging != null)
                    {
                        PropertyChanging(this, new PropertyChangingEventArgs("Geometry"));
                    }

                    geometry = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Geometry"));
                    }
                }
            }

            public bool HasParent { get; set; }
        }
    }
}