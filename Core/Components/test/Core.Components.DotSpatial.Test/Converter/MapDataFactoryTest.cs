using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Components.DotSpatial.Converter;
using Core.Components.DotSpatial.Data;
using Core.Components.DotSpatial.TestUtil;
using DotSpatial.Data;
using DotSpatial.Topology;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Converter
{
    [TestFixture]
    public class MapDataFactoryTest
    {
        [Test]
        public void Create_MapPointData_ReturnFeatureSetWithPointType()
        {
            // Setup
            var factory = new MapDataFactory();
            var testData = CreateTestData();

            // Call
            IList<FeatureSet> featureSets = factory.Create(new MapPointData(testData));

            // Assert
            Assert.IsInstanceOf<IList<FeatureSet>>(featureSets);
            var featureSet = featureSets[0];
            Assert.AreEqual(testData.Count, featureSet.Features.Count);
            Assert.IsInstanceOf<FeatureSet>(featureSet);
            Assert.AreEqual(FeatureType.Point, featureSet.FeatureType);
            CollectionAssert.AreNotEqual(testData, featureSet.Features[0].Coordinates);
        }

        [Test]
        public void Create_MapLineData_ReturnFeatureSetWithLineType()
        {
            // Setup
            var factory = new MapDataFactory();
            var testData = CreateTestData();

            // Call
            IList<FeatureSet> featureSets = factory.Create(new MapLineData(testData));

            // Assert
            Assert.IsInstanceOf<IList<FeatureSet>> (featureSets);
            var featureSet = featureSets[0];
            Assert.AreEqual(1, featureSet.Features.Count);
            Assert.IsInstanceOf<FeatureSet>(featureSet);
            Assert.IsInstanceOf<LineString>(featureSet.Features[0].BasicGeometry);
            Assert.AreEqual(FeatureType.Line, featureSet.FeatureType);
        }

        [Test]
        public void Create_MapPolygonData_ReturnFeatureSetWithPolygonType()
        {
            // Setup
            var factory = new MapDataFactory();
            var testData = CreateTestData();

            // Call
            IList<FeatureSet> featureSets = factory.Create(new MapPolygonData(testData));

            // Assert
            Assert.IsInstanceOf<IList<FeatureSet>> (featureSets);
            var featureSet = featureSets[0];
            Assert.AreEqual(1, featureSet.Features.Count);
            Assert.IsInstanceOf<FeatureSet>(featureSet);
            Assert.IsInstanceOf<Polygon>(featureSet.Features[0].BasicGeometry);
            Assert.AreEqual(FeatureType.Polygon, featureSet.FeatureType);
        }

        [Test]
        public void Create_OtherData_ThrownsNotSupportedException()
        {
            // Setup
            var factory = new MapDataFactory();
            var testData = new TestMapData();

            // Call
            TestDelegate test = () => factory.Create(testData);

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }

        private static Collection<Tuple<double, double>>  CreateTestData()
        {
            return new Collection<Tuple<double, double>>
            {
                new Tuple<double, double>(1.2, 3.4),
                new Tuple<double, double>(3.2, 3.4),
                new Tuple<double, double>(0.2, 2.4)
            };
        }
    }
}
