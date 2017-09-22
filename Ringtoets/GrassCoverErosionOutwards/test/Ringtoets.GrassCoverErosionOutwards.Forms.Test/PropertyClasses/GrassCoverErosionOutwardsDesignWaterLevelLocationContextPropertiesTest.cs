﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Common.Utils;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionOutwardsDesignWaterLevelLocationContextPropertiesTest
    {
        private const int idPropertyIndex = 0;
        private const int namePropertyIndex = 1;
        private const int coordinatesPropertyIndex = 2;
        private const int designWaterLevelPropertyIndex = 3;
        private const int targetProbabilityPropertyIndex = 4;
        private const int targetReliabilityPropertyIndex = 5;
        private const int calculatedProbabilityPropertyIndex = 6;
        private const int calculatedReliabilityPropertyIndex = 7;
        private const int convergencePropertyIndex = 8;
        private const int shouldCalculateIllustrationPointsIndex = 9;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties();

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionOutwardsHydraulicBoundaryLocationContextProperties>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_ValidData_ReturnsExpectedValues()
        {
            // Setup
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            // Call
            var properties = new GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties
            {
                Data = new GrassCoverErosionOutwardsDesignWaterLevelLocationContext(new ObservableList<HydraulicBoundaryLocation>
                {
                    hydraulicBoundaryLocation
                }, hydraulicBoundaryLocation)
            };

            // Assert
            Assert.AreEqual(hydraulicBoundaryLocation.Id, properties.Id);
            Assert.AreEqual(hydraulicBoundaryLocation.Name, properties.Name);
            Assert.AreEqual(hydraulicBoundaryLocation.Location, properties.Location);
            Assert.IsNaN(properties.DesignWaterLevel);
            TestHelper.AssertTypeConverter<GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties, NoValueRoundedDoubleConverter>(
                nameof(GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties.DesignWaterLevel));
            Assert.AreEqual(double.NaN, properties.TargetProbability);
            TestHelper.AssertTypeConverter<GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties, NoProbabilityValueDoubleConverter>(
                nameof(GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties.TargetProbability));
            Assert.IsNaN(properties.TargetReliability);
            TestHelper.AssertTypeConverter<GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties, NoValueRoundedDoubleConverter>
                (nameof(GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties.TargetReliability));
            Assert.AreEqual(double.NaN, properties.CalculatedProbability);
            TestHelper.AssertTypeConverter<GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties, NoProbabilityValueDoubleConverter>(
                nameof(GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties.CalculatedProbability));
            Assert.IsNaN(properties.CalculatedReliability);
            TestHelper.AssertTypeConverter<GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties, NoValueRoundedDoubleConverter>(
                nameof(GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties.CalculatedReliability));
            Assert.IsEmpty(properties.Convergence);
            Assert.AreEqual(hydraulicBoundaryLocation.DesignWaterLevelCalculation.InputParameters.ShouldIllustrationPointsBeCalculated, properties.ShouldIllustrationPointsBeCalculated);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetProperties_FullyConfiguredLocation_ReturnsExpected(bool withIllustrationPoints)
        {
            // Setup
            var random = new Random();
            const long id = 1;
            const double x = 567.0;
            const double y = 890.0;
            const string name = "name";

            double targetProbability = random.NextDouble();
            double targetReliability = random.NextDouble();
            double calculatedProbability = random.NextDouble();
            double calculatedReliability = random.NextDouble();
            double designWaterLevel = random.NextDouble();
            var convergence = random.NextEnumValue<CalculationConvergence>();

            var illustrationPoints = new[]
            {
                new TopLevelSubMechanismIllustrationPoint(new WindDirection("WEST", 4), "sluit", new TestSubMechanismIllustrationPoint())
            };
            var stochasts = new[]
            {
                new Stochast("a", 2, 3)
            };
            const string governingWindDirection = "EAST";
            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult =
                withIllustrationPoints
                    ? new GeneralResult<TopLevelSubMechanismIllustrationPoint>(new WindDirection(governingWindDirection, 2),
                                                                               stochasts,
                                                                               illustrationPoints)
                    : null;

            var hydraulicBoundaryLocationOutput = new HydraulicBoundaryLocationOutput(designWaterLevel,
                                                                                      targetProbability,
                                                                                      targetReliability,
                                                                                      calculatedProbability,
                                                                                      calculatedReliability,
                                                                                      convergence,
                                                                                      generalResult);

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y)
            {
                DesignWaterLevelCalculation =
                {
                    Output = hydraulicBoundaryLocationOutput
                }
            };

            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            };
            var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationContext(locations, hydraulicBoundaryLocation);

            // Call
            var properties = new GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties
            {
                Data = context
            };

            // Assert
            Assert.AreEqual(id, properties.Id);
            Assert.AreEqual(name, properties.Name);
            var coordinates = new Point2D(x, y);
            Assert.AreEqual(coordinates, properties.Location);
            Assert.AreEqual(designWaterLevel, properties.DesignWaterLevel, properties.DesignWaterLevel.GetAccuracy());

            Assert.AreEqual(targetProbability, properties.TargetProbability);
            Assert.AreEqual(targetReliability, properties.TargetReliability, properties.TargetReliability.GetAccuracy());
            Assert.AreEqual(calculatedProbability, properties.CalculatedProbability);
            Assert.AreEqual(calculatedReliability, properties.CalculatedReliability, properties.CalculatedReliability.GetAccuracy());

            string convergenceValue = new EnumDisplayWrapper<CalculationConvergence>(convergence).DisplayName;
            Assert.AreEqual(convergenceValue, properties.Convergence);

            if (withIllustrationPoints)
            {
                GeneralResult<TopLevelSubMechanismIllustrationPoint> expectedGeneralResult = hydraulicBoundaryLocationOutput.GeneralResult;
                CollectionAssert.AreEqual(expectedGeneralResult.Stochasts, properties.AlphaValues);
                CollectionAssert.AreEqual(expectedGeneralResult.Stochasts, properties.Durations);
                CollectionAssert.AreEqual(expectedGeneralResult.TopLevelIllustrationPoints, properties.IllustrationPoints.Select(ip => ip.Data));
                Assert.AreEqual(expectedGeneralResult.GoverningWindDirection.Name, properties.GoverningWindDirection);
            }
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            // Call
            var properties = new GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties
            {
                Data = new GrassCoverErosionOutwardsDesignWaterLevelLocationContext(new ObservableList<HydraulicBoundaryLocation>
                {
                    hydraulicBoundaryLocation
                }, hydraulicBoundaryLocation)
            };

            // Assert
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(10, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string resultCategory = "Resultaat";

            PropertyDescriptor idProperty = dynamicProperties[idPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(idProperty,
                                                                            generalCategory,
                                                                            "ID",
                                                                            "ID van de hydraulische randvoorwaardenlocatie in de database.",
                                                                            true);

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "Naam van de hydraulische randvoorwaardenlocatie.",
                                                                            true);

            PropertyDescriptor coordinatesProperty = dynamicProperties[coordinatesPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(coordinatesProperty,
                                                                            generalCategory,
                                                                            "Coördinaten [m]",
                                                                            "Coördinaten van de hydraulische randvoorwaardenlocatie.",
                                                                            true);

            PropertyDescriptor designWaterLevelProperty = dynamicProperties[designWaterLevelPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(designWaterLevelProperty,
                                                                            resultCategory,
                                                                            "Waterstand bij doorsnede-eis [m+NAP]",
                                                                            "Berekende waterstand bij doorsnede-eis.",
                                                                            true);

            PropertyDescriptor targetProbabilityProperty = dynamicProperties[targetProbabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(targetProbabilityProperty,
                                                                            resultCategory,
                                                                            "Doelkans [1/jaar]",
                                                                            "De ingevoerde kans waarvoor het resultaat moet worden berekend.",
                                                                            true);

            PropertyDescriptor targetReliabilityProperty = dynamicProperties[targetReliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(targetReliabilityProperty,
                                                                            resultCategory,
                                                                            "Betrouwbaarheidsindex doelkans [-]",
                                                                            "Betrouwbaarheidsindex van de ingevoerde kans waarvoor het resultaat moet worden berekend.",
                                                                            true);

            PropertyDescriptor calculatedProbabilityProperty = dynamicProperties[calculatedProbabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(calculatedProbabilityProperty,
                                                                            resultCategory,
                                                                            "Berekende kans [1/jaar]",
                                                                            "De berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor calculatedReliabilityProperty = dynamicProperties[calculatedReliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(calculatedReliabilityProperty,
                                                                            resultCategory,
                                                                            "Betrouwbaarheidsindex berekende kans [-]",
                                                                            "Betrouwbaarheidsindex van de berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor convergenceProperty = dynamicProperties[convergencePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(convergenceProperty,
                                                                            resultCategory,
                                                                            "Convergentie",
                                                                            "Is convergentie bereikt in de berekening van de waterstand bij doorsnede-eis?",
                                                                            true);

            PropertyDescriptor calculateIllustrationPointsProperty = dynamicProperties[shouldCalculateIllustrationPointsIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(calculateIllustrationPointsProperty,
                                                                            "Illustratiepunten",
                                                                            "Illustratiepunten inlezen",
                                                                            "Neem de informatie over de illustratiepunten op in het berekeningsresultaat.");
        }

        [Test]
        public void ShouldIllustrationPointsBeCalculated_SetNewValue_NotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            HydraulicBoundaryLocation hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            };

            locations.Attach(observer);

            var properties = new GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties
            {
                Data = new GrassCoverErosionOutwardsDesignWaterLevelLocationContext(locations, hydraulicBoundaryLocation)
            };

            // Precondition
            Assert.IsFalse(hydraulicBoundaryLocation.DesignWaterLevelCalculation.InputParameters.ShouldIllustrationPointsBeCalculated);

            // Call
            properties.ShouldIllustrationPointsBeCalculated = true;

            // Assert
            Assert.IsTrue(hydraulicBoundaryLocation.DesignWaterLevelCalculation.InputParameters.ShouldIllustrationPointsBeCalculated);
            mocks.VerifyAll();
        }
    }
}