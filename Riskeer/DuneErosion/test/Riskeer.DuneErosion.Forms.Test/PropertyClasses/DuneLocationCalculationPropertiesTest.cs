// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Globalization;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.DuneErosion.Forms.PropertyClasses;

namespace Riskeer.DuneErosion.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DuneLocationCalculationPropertiesTest
    {
        private const int requiredIdPropertyIndex = 0;
        private const int requiredNamePropertyIndex = 1;
        private const int requiredCoastalAreaIdPropertyIndex = 2;
        private const int requiredOffSetPropertyIndex = 3;
        private const int requiredLocationPropertyIndex = 4;

        private const int requiredWaterLevelPropertyIndex = 5;
        private const int requiredWaveHeightPropertyIndex = 6;
        private const int requiredWavePeriodPropertyIndex = 7;
        private const int requiredD50PropertyIndex = 8;
        private const int requiredTargetProbabilityPropertyIndex = 9;
        private const int requiredTargetReliabilityPropertyIndex = 10;
        private const int requiredCalculatedProbabilityPropertyIndex = 11;
        private const int requiredCalculatedReliabilityPropertyIndex = 12;
        private const int requiredConvergencePropertyIndex = 13;

        [Test]
        public void Constructor_DuneLocationCalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DuneLocationCalculationProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void GetProperties_ValidData_ReturnsExpectedValues()
        {
            // Setup
            var duneLocation = new TestDuneLocation();
            var duneLocationCalculation = new DuneLocationCalculation(duneLocation);

            // Call
            var properties = new DuneLocationCalculationProperties(duneLocationCalculation);

            // Assert
            Assert.AreEqual(duneLocation.Id, properties.Id);
            Assert.AreEqual(duneLocation.Name, properties.Name);
            Assert.AreEqual(duneLocation.CoastalAreaId, properties.CoastalAreaId);
            Assert.AreEqual(duneLocation.Offset.ToString("0.#", CultureInfo.InvariantCulture), properties.Offset);
            Assert.AreEqual(duneLocation.Location, properties.Location);

            Assert.IsNaN(properties.WaterLevel);
            Assert.IsNaN(properties.WaveHeight);
            Assert.IsNaN(properties.WavePeriod);

            Assert.IsNaN(properties.TargetProbability);
            TestHelper.AssertTypeConverter<DuneLocationCalculationProperties, NoProbabilityValueDoubleConverter>(
                nameof(DuneLocationCalculationProperties.TargetProbability));
            Assert.IsNaN(properties.TargetReliability);
            TestHelper.AssertTypeConverter<DuneLocationCalculationProperties, NoValueRoundedDoubleConverter>(
                nameof(DuneLocationCalculationProperties.TargetReliability));

            Assert.IsNaN(properties.CalculatedProbability);
            TestHelper.AssertTypeConverter<DuneLocationCalculationProperties, NoProbabilityValueDoubleConverter>(
                nameof(DuneLocationCalculationProperties.CalculatedProbability));
            Assert.IsNaN(properties.CalculatedReliability);
            TestHelper.AssertTypeConverter<DuneLocationCalculationProperties, NoValueRoundedDoubleConverter>(
                nameof(DuneLocationCalculationProperties.CalculatedReliability));

            string convergenceValue = new EnumDisplayWrapper<CalculationConvergence>(CalculationConvergence.NotCalculated).DisplayName;
            Assert.AreEqual(convergenceValue, properties.Convergence);
        }

        [Test]
        public void GetProperties_ValidDesignWaterLevel_ReturnsExpectedValues()
        {
            // Setup
            const long id = 1234L;
            const double x = 567.0;
            const double y = 890.0;
            const string name = "<some name>";

            var random = new Random();
            int coastalAreaId = random.Next();
            var convergence = random.NextEnumValue<CalculationConvergence>();
            double offset = random.NextDouble();
            double orientation = random.NextDouble();
            double d50 = random.NextDouble();
            double waterLevel = random.NextDouble();
            double waveHeight = random.NextDouble();
            double wavePeriod = random.NextDouble();
            double targetProbability = random.NextDouble();
            double targetReliability = random.NextDouble();
            double calculatedProbability = random.NextDouble();
            double calculatedReliability = random.NextDouble();

            var output = new DuneLocationCalculationOutput(
                convergence,
                new DuneLocationCalculationOutput.ConstructionProperties
                {
                    WaterLevel = waterLevel,
                    WaveHeight = waveHeight,
                    WavePeriod = wavePeriod,
                    TargetProbability = targetProbability,
                    TargetReliability = targetReliability,
                    CalculatedProbability = calculatedProbability,
                    CalculatedReliability = calculatedReliability
                });
            var duneLocation = new DuneLocation(new HydraulicBoundaryLocation(id, "", 0, 0),
                                                name,
                                                new Point2D(x, y),
                                                new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = coastalAreaId,
                                                    Offset = offset,
                                                    Orientation = orientation,
                                                    D50 = d50
                                                });
            var duneLocationCalculation = new DuneLocationCalculation(duneLocation)
            {
                Output = output
            };

            // Call
            var properties = new DuneLocationCalculationProperties(duneLocationCalculation);

            // Assert
            Assert.AreEqual(id, properties.Id);
            Assert.AreEqual(name, properties.Name);
            Assert.AreEqual(coastalAreaId, properties.CoastalAreaId);
            Assert.AreEqual(duneLocation.Offset.ToString("0.#", CultureInfo.InvariantCulture), properties.Offset);
            var expectedLocation = new Point2D(x, y);
            Assert.AreEqual(expectedLocation, properties.Location);

            Assert.AreEqual(waterLevel, properties.WaterLevel, properties.WaterLevel.GetAccuracy());
            Assert.AreEqual(waveHeight, properties.WaveHeight, properties.WaveHeight.GetAccuracy());
            Assert.AreEqual(wavePeriod, properties.WavePeriod, properties.WavePeriod.GetAccuracy());
            Assert.AreEqual(d50, properties.D50, properties.D50.GetAccuracy());

            Assert.AreEqual(targetProbability, properties.TargetProbability);
            Assert.AreEqual(targetReliability, properties.TargetReliability, properties.TargetReliability.GetAccuracy());
            Assert.AreEqual(calculatedProbability, properties.CalculatedProbability, properties.CalculatedProbability);
            Assert.AreEqual(calculatedReliability, properties.CalculatedReliability, properties.CalculatedReliability.GetAccuracy());

            string convergenceValue = new EnumDisplayWrapper<CalculationConvergence>(convergence).DisplayName;
            Assert.AreEqual(convergenceValue, properties.Convergence);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var duneLocationCalculation = new DuneLocationCalculation(new TestDuneLocation());

            // Call
            var properties = new DuneLocationCalculationProperties(duneLocationCalculation);

            // Assert
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(14, dynamicProperties.Count);
            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            PropertyDescriptor idProperty = dynamicProperties[requiredIdPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(idProperty,
                                                                            "Algemeen",
                                                                            "ID",
                                                                            "ID van de hydraulische belastingenlocatie in de database.",
                                                                            true);

            PropertyDescriptor nameProperty = dynamicProperties[requiredNamePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            "Algemeen",
                                                                            "Naam",
                                                                            "Naam van de hydraulische belastingenlocatie.",
                                                                            true);

            PropertyDescriptor coastalAreaIdProperty = dynamicProperties[requiredCoastalAreaIdPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(coastalAreaIdProperty,
                                                                            "Algemeen",
                                                                            "Kustvaknummer",
                                                                            "Nummer van het kustvak waar de locatie onderdeel van uitmaakt.",
                                                                            true);

            PropertyDescriptor offsetProperty = dynamicProperties[requiredOffSetPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(offsetProperty,
                                                                            "Algemeen",
                                                                            "Metrering [dam]",
                                                                            "Metrering van de locatie binnen het kustvak waar het onderdeel van uitmaakt.",
                                                                            true);

            PropertyDescriptor locationProperty = dynamicProperties[requiredLocationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(locationProperty,
                                                                            "Algemeen",
                                                                            "Coördinaten [m]",
                                                                            "Coördinaten van de hydraulische belastingenlocatie.",
                                                                            true);

            PropertyDescriptor waterLevelProperty = dynamicProperties[requiredWaterLevelPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waterLevelProperty,
                                                                            "Resultaat",
                                                                            "Rekenwaarde waterstand [m+NAP]",
                                                                            "Berekende rekenwaarde voor de waterstand voor het uitvoeren van een sterkteberekening voor het toetsspoor duinen.",
                                                                            true);

            PropertyDescriptor waveHeightProperty = dynamicProperties[requiredWaveHeightPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waveHeightProperty,
                                                                            "Resultaat",
                                                                            "Rekenwaarde Hs [m]",
                                                                            "Berekende rekenwaarde voor de significante golfhoogte voor het uitvoeren van een sterkteberekening voor het toetsspoor duinen.",
                                                                            true);

            PropertyDescriptor wavePeriodProperty = dynamicProperties[requiredWavePeriodPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(wavePeriodProperty,
                                                                            "Resultaat",
                                                                            "Rekenwaarde Tp [s]",
                                                                            "Berekende rekenwaarde voor de piekperiode van de golven voor het uitvoeren van een sterkteberekening voor het toetsspoor duinen.",
                                                                            true);

            PropertyDescriptor d50Property = dynamicProperties[requiredD50PropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(d50Property,
                                                                            "Resultaat",
                                                                            "Rekenwaarde d50 [m]",
                                                                            "Rekenwaarde voor de d50 voor het uitvoeren van een sterkteberekening voor het toetsspoor duinen.",
                                                                            true);

            PropertyDescriptor targetProbabilityProperty = dynamicProperties[requiredTargetProbabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(targetProbabilityProperty,
                                                                            "Resultaat",
                                                                            "Doelkans [1/jaar]",
                                                                            "De ingevoerde kans waarvoor het resultaat moet worden berekend.",
                                                                            true);

            PropertyDescriptor targetReliabilityProperty = dynamicProperties[requiredTargetReliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(targetReliabilityProperty,
                                                                            "Resultaat",
                                                                            "Betrouwbaarheidsindex doelkans [-]",
                                                                            "Betrouwbaarheidsindex van de ingevoerde kans waarvoor het resultaat moet worden berekend.",
                                                                            true);

            PropertyDescriptor calculatedProbabilityProperty = dynamicProperties[requiredCalculatedProbabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(calculatedProbabilityProperty,
                                                                            "Resultaat",
                                                                            "Berekende kans [1/jaar]",
                                                                            "De berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor calculatedReliabilityProperty = dynamicProperties[requiredCalculatedReliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(calculatedReliabilityProperty,
                                                                            "Resultaat",
                                                                            "Betrouwbaarheidsindex berekende kans [-]",
                                                                            "Betrouwbaarheidsindex van de berekende kans van voorkomen van het berekende resultaat.",
                                                                            true);

            PropertyDescriptor convergenceProperty = dynamicProperties[requiredConvergencePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(convergenceProperty,
                                                                            "Resultaat",
                                                                            "Convergentie",
                                                                            "Is convergentie bereikt in de berekening van de hydraulische belastingen voor de duinlocatie?",
                                                                            true);
        }

        [Test]
        [TestCase(3.0, "3")]
        [TestCase(3.1, "3.1")]
        public void Offset_Always_FormatToString(double offset, string expectedPropertyValue)
        {
            var duneLocation = new DuneLocation(new TestHydraulicBoundaryLocation(),
                                                "test",
                                                new Point2D(0, 0),
                                                new DuneLocation.ConstructionProperties
                                                {
                                                    Offset = offset
                                                });
            var duneLocationCalculation = new DuneLocationCalculation(duneLocation);

            // Call
            var properties = new DuneLocationCalculationProperties(duneLocationCalculation);

            // Assert
            Assert.AreEqual(expectedPropertyValue, properties.Offset);
        }

        [Test]
        public void ToString_Always_ExpectedValue()
        {
            // Setup
            var duneLocation = new DuneLocation(new TestHydraulicBoundaryLocation(),
                                                "Name",
                                                new Point2D(0.0, 1.1),
                                                new DuneLocation.ConstructionProperties());
            var duneLocationCalculation = new DuneLocationCalculation(duneLocation);
            var properties = new DuneLocationCalculationProperties(duneLocationCalculation);

            // Call
            string result = properties.ToString();

            // Assert
            Assert.AreEqual($"{duneLocation.Name} {duneLocation.Location}", result);
        }
    }
}