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
using Core.Common.Base.Geometry;
using Core.Common.Gui.Converters;
using Core.Common.TestUtil;
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryLocationPropertiesTest
    {
        [Test]
        public void Constructor_DefaultArgumentValues_DoesNotThrowException()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "", 0.0, 0.0);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);
            var context = new TestHydraulicBoundaryLocationContext(hydraulicBoundaryDatabase, hydraulicBoundaryLocation);

            // Call
            TestDelegate test = () => new TestHydraulicBoundaryLocationProperties
            {
                Data = context
            };

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void GetProperties_ValidData_ReturnsExpectedValues()
        {
            // Setup
            const long id = 1234L;
            const double x = 567.0;
            const double y = 890.0;
            const string name = "<some name>";

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);
            var context = new TestHydraulicBoundaryLocationContext(hydraulicBoundaryDatabase, hydraulicBoundaryLocation);

            // Call
            HydraulicBoundaryLocationProperties hydraulicBoundaryLocationProperties = new TestHydraulicBoundaryLocationProperties
            {
                Data = context
            };

            // Assert
            Assert.AreEqual(id, hydraulicBoundaryLocationProperties.Id);
            Assert.AreEqual(name, hydraulicBoundaryLocationProperties.Name);
            var coordinates = new Point2D(x, y);
            Assert.AreEqual(coordinates, hydraulicBoundaryLocationProperties.Location);
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

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);
            var context = new TestHydraulicBoundaryLocationContext(hydraulicBoundaryDatabase, hydraulicBoundaryLocation);

            var random = new Random(21);
            var topLevelIllustrationPoint =
                new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection("Wind direction name"),
                                                          "closing situation",
                                                          new TestSubMechanismIllustrationPoint());

            var stochast = new Stochast("stochastName",
                                        random.NextDouble(),
                                        random.NextDouble());
            var generalResult = new GeneralResult<TopLevelSubMechanismIllustrationPoint>(
                WindDirectionTestFactory.CreateTestWindDirection("Governing Wind Direction Name"), new[]
                {
                    stochast
                }, new[]
                {
                    topLevelIllustrationPoint
                });

            // Call
            var locationProperties = new TestHydraulicBoundaryLocationProperties
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
            const long id = 1234L;
            const double x = 567.0;
            const double y = 890.0;
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);
            var context = new TestHydraulicBoundaryLocationContext(hydraulicBoundaryDatabase, hydraulicBoundaryLocation);

            // Call
            HydraulicBoundaryLocationProperties hydraulicBoundaryLocationProperties = new TestHydraulicBoundaryLocationProperties
            {
                Data = context
            };

            // Assert
            string expectedString = $"{name} {new Point2D(x, y)}";
            Assert.AreEqual(expectedString, hydraulicBoundaryLocationProperties.ToString());
        }

        [Test]
        public void Constructor_WithGeneralIllustrationPointsResult_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "", 0.0, 0.0);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);
            var context = new TestHydraulicBoundaryLocationContext(hydraulicBoundaryDatabase, hydraulicBoundaryLocation);

            // Call
            var hydraulicBoundaryLocationProperties = new TestHydraulicBoundaryLocationProperties
            {
                GeneralResult = new TestGeneralResultSubMechanismIllustrationPoint(),
                Data = context
            };

            // Assert
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(hydraulicBoundaryLocationProperties, true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            const string generalCategory = "Algemeen";
            const string illustrationPointsCategory = "Illustratiepunten";
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(hydraulicBoundaryLocationProperties);
            Assert.AreEqual(7, dynamicProperties.Count);
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

            TestHelper.AssertTypeConverter<HydraulicBoundaryLocationProperties, KeyValueExpandableArrayConverter>(nameof(HydraulicBoundaryLocationProperties.AlphaValues));
            PropertyDescriptor alphaValuesProperty = dynamicProperties[4];
            Assert.NotNull(alphaValuesProperty.Attributes[typeof(KeyValueElementAttribute)]);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(alphaValuesProperty,
                                                                            illustrationPointsCategory,
                                                                            "Alfa's [-]",
                                                                            "Berekende invloedscoëfficiënten voor alle beschouwde stochasten.",
                                                                            true);

            TestHelper.AssertTypeConverter<HydraulicBoundaryLocationProperties, KeyValueExpandableArrayConverter>(nameof(HydraulicBoundaryLocationProperties.Durations));
            PropertyDescriptor durationsProperty = dynamicProperties[5];
            Assert.NotNull(durationsProperty.Attributes[typeof(KeyValueElementAttribute)]);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(durationsProperty,
                                                                            illustrationPointsCategory,
                                                                            "Tijdsduren [min]",
                                                                            "Tijdsduren waarop de stochasten betrekking hebben.",
                                                                            true);

            TestHelper.AssertTypeConverter<HydraulicBoundaryLocationProperties, ExpandableArrayConverter>(nameof(HydraulicBoundaryLocationProperties.IllustrationPoints));
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
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "", 0.0, 0.0);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);
            var context = new TestHydraulicBoundaryLocationContext(hydraulicBoundaryDatabase, hydraulicBoundaryLocation);

            // Call
            var hydraulicBoundaryLocationProperties = new TestHydraulicBoundaryLocationProperties(new HydraulicBoundaryLocationProperties.ConstructionProperties
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

            Assert.AreEqual(nameof(HydraulicBoundaryLocationProperties.IllustrationPoints), dynamicProperties[0].Name);
            Assert.AreEqual(nameof(HydraulicBoundaryLocationProperties.Id), dynamicProperties[1].Name);
            Assert.AreEqual(nameof(HydraulicBoundaryLocationProperties.Name), dynamicProperties[2].Name);
            Assert.AreEqual(nameof(HydraulicBoundaryLocationProperties.Location), dynamicProperties[3].Name);
            Assert.AreEqual(nameof(HydraulicBoundaryLocationProperties.GoverningWindDirection), dynamicProperties[4].Name);
            Assert.AreEqual(nameof(HydraulicBoundaryLocationProperties.AlphaValues), dynamicProperties[5].Name);
            Assert.AreEqual(nameof(HydraulicBoundaryLocationProperties.Durations), dynamicProperties[6].Name);
        }

        [Test]
        public void Constructor_WithoutGeneralIllustrationPointsResult_PropertiesAreInExpectedOrder()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "", 0.0, 0.0);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);
            var context = new TestHydraulicBoundaryLocationContext(hydraulicBoundaryDatabase, hydraulicBoundaryLocation);

            // Call
            var hydraulicBoundaryLocationProperties = new TestHydraulicBoundaryLocationProperties
            {
                Data = context
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(hydraulicBoundaryLocationProperties);
            Assert.AreEqual(3, dynamicProperties.Count);

            Assert.AreEqual(nameof(HydraulicBoundaryLocationProperties.Id), dynamicProperties[0].Name);
            Assert.AreEqual(nameof(HydraulicBoundaryLocationProperties.Name), dynamicProperties[1].Name);
            Assert.AreEqual(nameof(HydraulicBoundaryLocationProperties.Location), dynamicProperties[2].Name);
        }

        private static void AssertStochast(Stochast stochast, Stochast actualStochast)
        {
            Assert.AreEqual(stochast.Name, actualStochast.Name);
            Assert.AreEqual(stochast.Alpha, actualStochast.Alpha);
            Assert.AreEqual(stochast.Duration, actualStochast.Duration);
        }

        private class TestHydraulicBoundaryLocationProperties : HydraulicBoundaryLocationProperties
        {
            public GeneralResult<TopLevelSubMechanismIllustrationPoint> GeneralResult;
            public TestHydraulicBoundaryLocationProperties() : base(new ConstructionProperties()) {}

            public TestHydraulicBoundaryLocationProperties(ConstructionProperties propertyIndexes) : base(propertyIndexes) {}

            protected override GeneralResult<TopLevelSubMechanismIllustrationPoint> GetGeneralResultSubMechanismIllustrationPoints()
            {
                return GeneralResult;
            }
        }

        private class TestHydraulicBoundaryLocationContext : HydraulicBoundaryLocationContext
        {
            public TestHydraulicBoundaryLocationContext(HydraulicBoundaryDatabase wrappedData, HydraulicBoundaryLocation hydraulicBoundaryLocation)
                : base(wrappedData, hydraulicBoundaryLocation) {}
        }
    }
}