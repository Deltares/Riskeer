// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionOutwardsHydraulicBoundaryLocationContextPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new TestGrassCoverErosionOutwardsLocationProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<GrassCoverErosionOutwardsHydraulicBoundaryLocationContext>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_ValidArguments_DoesNotThrowException()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2.0, 3.0);
            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            };

            var context = new TestGrassCoverErosionOutwardsLocationContext(locations, hydraulicBoundaryLocation);

            // Call
            TestDelegate test = () => new TestGrassCoverErosionOutwardsLocationProperties
            {
                Data = context
            };

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void GetProperties_ValidDataWithoutGeneralResult_ReturnsExpectedValues()
        {
            // Setup
            const long id = 1;
            const double x = 567.0;
            const double y = 890.0;
            const string name = "name";
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y);
            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            };
            var context = new TestGrassCoverErosionOutwardsLocationContext(locations, hydraulicBoundaryLocation);

            // Call
            var locationProperties = new TestGrassCoverErosionOutwardsLocationProperties
            {
                Data = context
            };

            // Assert
            Assert.AreEqual(id, locationProperties.Id);
            Assert.AreEqual(name, locationProperties.Name);
            var coordinates = new Point2D(x, y);
            Assert.AreEqual(coordinates, locationProperties.Location);
        }

        [Test]
        public void GetProperties_ValidDataWithGeneralResult_ReturnsExpectedValues()
        {
            // Setup
            const long id = 1;
            const double x = 567.0;
            const double y = 890.0;
            const string name = "name";
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y);
            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            };
            var context = new TestGrassCoverErosionOutwardsLocationContext(locations, hydraulicBoundaryLocation);

            var random = new Random(21);
            var topLevelIllustrationPoint =
                new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection("Wind Direction Name"),
                                                          "closing situation",
                                                          new TestSubMechanismIllustrationPoint());

            var stochast = new Stochast("stochastName",
                                        random.NextDouble(),
                                        random.NextDouble());
            var generalResult = new GeneralResult<TopLevelSubMechanismIllustrationPoint>(
                WindDirectionTestFactory.CreateTestWindDirection("Governing wind Direction name"), new[]
                {
                    stochast
                }, new[]
                {
                    topLevelIllustrationPoint
                });

            // Call
            var locationProperties = new TestGrassCoverErosionOutwardsLocationProperties
            {
                Data = context,
                GeneralResult = generalResult
            };

            // Assert
            Assert.AreEqual(id, locationProperties.Id);
            Assert.AreEqual(name, locationProperties.Name);
            var coordinates = new Point2D(x, y);
            Assert.AreEqual(coordinates, locationProperties.Location);

            Assert.AreEqual(generalResult.GoverningWindDirection.Name, locationProperties.GoverningWindDirection);
            CollectionAssert.AreEqual(generalResult.Stochasts, locationProperties.AlphaValues);
            CollectionAssert.AreEqual(generalResult.Stochasts, locationProperties.Durations);

            TopLevelSubMechanismIllustrationPointProperties topLevelProperties =
                locationProperties.IllustrationPoints.Single();
            Assert.AreEqual(topLevelIllustrationPoint.SubMechanismIllustrationPoint.Name, topLevelProperties.Name);
            Assert.AreEqual(topLevelIllustrationPoint.WindDirection.Name, topLevelProperties.WindDirection);
            Assert.AreEqual(topLevelIllustrationPoint.ClosingSituation, topLevelProperties.ClosingSituation);
            Assert.AreEqual(topLevelIllustrationPoint.SubMechanismIllustrationPoint.Beta, topLevelProperties.CalculatedReliability);
            double expectedProbability = StatisticsConverter.ReliabilityToProbability(
                topLevelIllustrationPoint.SubMechanismIllustrationPoint.Beta);
            Assert.AreEqual(expectedProbability, topLevelProperties.CalculatedProbability);
            CollectionAssert.IsEmpty(topLevelProperties.AlphaValues);
            CollectionAssert.IsEmpty(topLevelProperties.Durations);
            CollectionAssert.IsEmpty(topLevelProperties.IllustrationPointResults);
        }

        [Test]
        [TestCase("")]
        [TestCase("some name")]
        public void ToString_WithName_ReturnsName(string name)
        {
            // Setup
            const double x = 567.0;
            const double y = 890.0;
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, name, x, y);
            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            };
            var context = new TestGrassCoverErosionOutwardsLocationContext(locations, hydraulicBoundaryLocation);

            // Call
            var locationProperties = new TestGrassCoverErosionOutwardsLocationProperties
            {
                Data = context
            };

            // Assert
            string expectedString = $"{name} {new Point2D(x, y)}";
            Assert.AreEqual(expectedString, locationProperties.ToString());
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            const long id = 1;
            const double x = 567.0;
            const double y = 890.0;
            const string name = "name";
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y);
            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            };
            var context = new TestGrassCoverErosionOutwardsLocationContext(locations, hydraulicBoundaryLocation);

            // Call
            var locationProperties = new TestGrassCoverErosionOutwardsLocationProperties
            {
                Data = context,
                GeneralResult = new TestGeneralResultSubMechanismIllustrationPoint()
            };

            // Assert
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(locationProperties, true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(locationProperties);
            Assert.AreEqual(7, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string illustrationPointsCategory = "Illustratiepunten";
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[0],
                                                                            generalCategory,
                                                                            "ID",
                                                                            "ID van de hydraulische randvoorwaardenlocatie in de database.",
                                                                            true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[1],
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "Naam van de hydraulische randvoorwaardenlocatie.",
                                                                            true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[2],
                                                                            generalCategory,
                                                                            "Coördinaten [m]",
                                                                            "Coördinaten van de hydraulische randvoorwaardenlocatie.",
                                                                            true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[3],
                                                                            illustrationPointsCategory,
                                                                            "Maatgevende windrichting",
                                                                            "De windrichting waarvoor de berekende betrouwbaarheidsindex het laagst is.",
                                                                            true);

            TestHelper.AssertTypeConverter<GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties,
                KeyValueExpandableArrayConverter>(nameof(GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties.AlphaValues));
            PropertyDescriptor alphaValuesProperty = dynamicProperties[4];
            Assert.NotNull(alphaValuesProperty.Attributes[typeof(KeyValueElementAttribute)]);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(alphaValuesProperty,
                                                                            illustrationPointsCategory,
                                                                            "Alfa's [-]",
                                                                            "Berekende invloedscoëfficiënten voor alle beschouwde stochasten.",
                                                                            true);

            TestHelper.AssertTypeConverter<GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties,
                KeyValueExpandableArrayConverter>(nameof(GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties.Durations));
            PropertyDescriptor durationsProperty = dynamicProperties[5];
            Assert.NotNull(durationsProperty.Attributes[typeof(KeyValueElementAttribute)]);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(durationsProperty,
                                                                            illustrationPointsCategory,
                                                                            "Tijdsduren [min]",
                                                                            "Tijdsduren waarop de stochasten betrekking hebben.",
                                                                            true);

            TestHelper.AssertTypeConverter<GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties,
                ExpandableArrayConverter>(nameof(GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties.IllustrationPoints));
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dynamicProperties[6],
                                                                            illustrationPointsCategory,
                                                                            "Illustratiepunten",
                                                                            "De lijst van illustratiepunten voor de berekening.",
                                                                            true);
        }

        [Test]
        public void Constructor_WithGeneralIllustrationPointsResultAndDifferentPropertyOrder_PropertiesAreInExpectedOrder()
        {
            // Setup
            const long id = 1;
            const double x = 567.0;
            const double y = 890.0;
            const string name = "name";
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y);
            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            };
            var context = new TestGrassCoverErosionOutwardsLocationContext(locations, hydraulicBoundaryLocation);

            // Call
            var hydraulicBoundaryLocationProperties = new TestGrassCoverErosionOutwardsLocationProperties(
                new GrassCoverErosionOutwardsHydraulicBoundaryLocationContextProperties.ConstructionProperties
                {
                    IllustrationPointsIndex = 1,
                    IdIndex = 2,
                    NameIndex = 3,
                    LocationIndex = 4,
                    GoverningWindDirectionIndex = 5,
                    StochastsIndex = 6,
                    DurationsIndex = 7
                })
            {
                GeneralResult = new TestGeneralResultSubMechanismIllustrationPoint(),
                Data = context
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(hydraulicBoundaryLocationProperties);
            Assert.AreEqual(7, dynamicProperties.Count);

            Assert.AreEqual(nameof(GrassCoverErosionOutwardsHydraulicBoundaryLocationContextProperties.IllustrationPoints), dynamicProperties[0].Name);
            Assert.AreEqual(nameof(GrassCoverErosionOutwardsHydraulicBoundaryLocationContextProperties.Id), dynamicProperties[1].Name);
            Assert.AreEqual(nameof(GrassCoverErosionOutwardsHydraulicBoundaryLocationContextProperties.Name), dynamicProperties[2].Name);
            Assert.AreEqual(nameof(GrassCoverErosionOutwardsHydraulicBoundaryLocationContextProperties.Location), dynamicProperties[3].Name);
            Assert.AreEqual(nameof(GrassCoverErosionOutwardsHydraulicBoundaryLocationContextProperties.GoverningWindDirection), dynamicProperties[4].Name);
            Assert.AreEqual(nameof(GrassCoverErosionOutwardsHydraulicBoundaryLocationContextProperties.AlphaValues), dynamicProperties[5].Name);
            Assert.AreEqual(nameof(GrassCoverErosionOutwardsHydraulicBoundaryLocationContextProperties.Durations), dynamicProperties[6].Name);
        }

        [Test]
        public void Constructor_WithoutGeneralIllustrationPointsResult_PropertiesAreInExpectedOrder()
        {
            // Setup
            const long id = 1;
            const double x = 567.0;
            const double y = 890.0;
            const string name = "name";
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y);
            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            };
            var context = new TestGrassCoverErosionOutwardsLocationContext(locations, hydraulicBoundaryLocation);

            // Call
            var hydraulicBoundaryLocationProperties = new TestGrassCoverErosionOutwardsLocationProperties
            {
                Data = context
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(hydraulicBoundaryLocationProperties);
            Assert.AreEqual(3, dynamicProperties.Count);

            Assert.AreEqual(nameof(GrassCoverErosionOutwardsHydraulicBoundaryLocationContextProperties.Id), dynamicProperties[0].Name);
            Assert.AreEqual(nameof(GrassCoverErosionOutwardsHydraulicBoundaryLocationContextProperties.Name), dynamicProperties[1].Name);
            Assert.AreEqual(nameof(GrassCoverErosionOutwardsHydraulicBoundaryLocationContextProperties.Location), dynamicProperties[2].Name);
        }

        private static void AssertStochast(Stochast stochast, Stochast actualStochast)
        {
            Assert.AreEqual(stochast.Name, actualStochast.Name);
            Assert.AreEqual(stochast.Alpha, actualStochast.Alpha);
            Assert.AreEqual(stochast.Duration, actualStochast.Duration);
        }

        private class TestGrassCoverErosionOutwardsLocationProperties : GrassCoverErosionOutwardsHydraulicBoundaryLocationContextProperties
        {
            public GeneralResult<TopLevelSubMechanismIllustrationPoint> GeneralResult;
            public TestGrassCoverErosionOutwardsLocationProperties() : base(new ConstructionProperties()) {}

            public TestGrassCoverErosionOutwardsLocationProperties(ConstructionProperties propertyIndexes) : base(propertyIndexes) {}

            protected override GeneralResult<TopLevelSubMechanismIllustrationPoint> GetGeneralResultSubMechanismIllustrationPoints()
            {
                return GeneralResult;
            }
        }

        private class TestGrassCoverErosionOutwardsLocationContext : GrassCoverErosionOutwardsHydraulicBoundaryLocationContext
        {
            public TestGrassCoverErosionOutwardsLocationContext(
                ObservableList<HydraulicBoundaryLocation> wrappedData,
                HydraulicBoundaryLocation hydraulicBoundaryLocation)
                : base(wrappedData, hydraulicBoundaryLocation) {}
        }
    }
}