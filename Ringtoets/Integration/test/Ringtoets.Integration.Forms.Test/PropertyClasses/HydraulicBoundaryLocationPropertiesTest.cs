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
using Core.Common.Util;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryLocationPropertiesTest
    {
        [Test]
        public void Constructor_HydraulicBoundaryLocationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestHydraulicBoundaryLocationProperties(null, new HydraulicBoundaryLocationCalculation());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocation", paramName);
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationCalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestHydraulicBoundaryLocationProperties(new TestHydraulicBoundaryLocation(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocationCalculation", paramName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestHydraulicBoundaryLocationProperties(new TestHydraulicBoundaryLocation(),
                                                                                  new HydraulicBoundaryLocationCalculation(),
                                                                                  null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("propertyIndexes", paramName);
        }

        [Test]
        public void Constructor_ValidData_ReturnsExpectedValues()
        {
            // Setup
            const long id = 1234L;
            const double x = 567.0;
            const double y = 890.0;
            const string name = "<some name>";

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y);

            // Call
            HydraulicBoundaryLocationProperties hydraulicBoundaryLocationProperties = new TestHydraulicBoundaryLocationProperties(hydraulicBoundaryLocation);

            // Assert
            Assert.AreEqual(id, hydraulicBoundaryLocationProperties.Id);
            Assert.AreEqual(name, hydraulicBoundaryLocationProperties.Name);
            var coordinates = new Point2D(x, y);
            Assert.AreEqual(coordinates, hydraulicBoundaryLocationProperties.Location);
        }

        [Test]
        public void Constructor_ValidDataWithGeneralResult_ReturnsExpectedValues()
        {
            // Setup
            const long id = 1;
            const double x = 567.0;
            const double y = 890.0;
            const string name = "name";
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y);

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
            var locationProperties = new TestHydraulicBoundaryLocationProperties(hydraulicBoundaryLocation)
            {
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
            SubMechanismIllustrationPoint subMechanismIllustrationPoint = topLevelIllustrationPoint.SubMechanismIllustrationPoint;
            Assert.AreEqual(subMechanismIllustrationPoint.Name, topLevelProperties.Name);
            Assert.AreEqual(topLevelIllustrationPoint.WindDirection.Name, topLevelProperties.WindDirection);
            Assert.AreEqual(topLevelIllustrationPoint.ClosingSituation, topLevelProperties.ClosingSituation);
            Assert.AreEqual(subMechanismIllustrationPoint.Beta, topLevelProperties.CalculatedReliability);
            double expectedProbability = StatisticsConverter.ReliabilityToProbability(
                subMechanismIllustrationPoint.Beta);
            Assert.AreEqual(expectedProbability, topLevelProperties.CalculatedProbability);
            CollectionAssert.AreEqual(subMechanismIllustrationPoint.Stochasts, topLevelProperties.AlphaValues);
            CollectionAssert.AreEqual(subMechanismIllustrationPoint.Stochasts, topLevelProperties.Durations);
            Assert.AreSame(subMechanismIllustrationPoint, topLevelProperties.SubMechanismIllustrationPointValues.Data);
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

            // Call
            HydraulicBoundaryLocationProperties hydraulicBoundaryLocationProperties = new TestHydraulicBoundaryLocationProperties(hydraulicBoundaryLocation);

            // Assert
            string expectedString = $"{name} {new Point2D(x, y)}";
            Assert.AreEqual(expectedString, hydraulicBoundaryLocationProperties.ToString());
        }

        [Test]
        public void Constructor_WithGeneralIllustrationPointsResult_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "", 0.0, 0.0);

            // Call
            var hydraulicBoundaryLocationProperties = new TestHydraulicBoundaryLocationProperties(hydraulicBoundaryLocation)
            {
                GeneralResult = new TestGeneralResultSubMechanismIllustrationPoint()
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
                                                                            "Invloedscoëfficiënten [-]",
                                                                            "Berekende invloedscoëfficiënten voor alle beschouwde stochasten.",
                                                                            true);

            TestHelper.AssertTypeConverter<HydraulicBoundaryLocationProperties, KeyValueExpandableArrayConverter>(nameof(HydraulicBoundaryLocationProperties.Durations));
            PropertyDescriptor durationsProperty = dynamicProperties[5];
            Assert.NotNull(durationsProperty.Attributes[typeof(KeyValueElementAttribute)]);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(durationsProperty,
                                                                            illustrationPointsCategory,
                                                                            "Tijdsduren [uur]",
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

            // Call
            var hydraulicBoundaryLocationProperties = new TestHydraulicBoundaryLocationProperties(
                hydraulicBoundaryLocation,
                new HydraulicBoundaryLocationProperties.ConstructionProperties
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
                GeneralResult = new TestGeneralResultSubMechanismIllustrationPoint()
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

            // Call
            var hydraulicBoundaryLocationProperties = new TestHydraulicBoundaryLocationProperties(hydraulicBoundaryLocation);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(hydraulicBoundaryLocationProperties);
            Assert.AreEqual(3, dynamicProperties.Count);

            Assert.AreEqual(nameof(HydraulicBoundaryLocationProperties.Id), dynamicProperties[0].Name);
            Assert.AreEqual(nameof(HydraulicBoundaryLocationProperties.Name), dynamicProperties[1].Name);
            Assert.AreEqual(nameof(HydraulicBoundaryLocationProperties.Location), dynamicProperties[2].Name);
        }

        private class TestHydraulicBoundaryLocationProperties : HydraulicBoundaryLocationProperties
        {
            public GeneralResult<TopLevelSubMechanismIllustrationPoint> GeneralResult;

            public TestHydraulicBoundaryLocationProperties(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                           HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation)
                : base(hydraulicBoundaryLocation,
                       hydraulicBoundaryLocationCalculation,
                       new ConstructionProperties()) {}

            public TestHydraulicBoundaryLocationProperties(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                           HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation,
                                                           ConstructionProperties propertyIndexes)
                : base(hydraulicBoundaryLocation,
                       hydraulicBoundaryLocationCalculation,
                       propertyIndexes) {}

            protected override GeneralResult<TopLevelSubMechanismIllustrationPoint> GetGeneralResult()
            {
                return GeneralResult;
            }
        }
    }
}