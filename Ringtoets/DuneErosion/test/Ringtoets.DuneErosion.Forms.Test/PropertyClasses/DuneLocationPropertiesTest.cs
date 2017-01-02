using System;
using System.ComponentModel;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Utils;
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Forms.PropertyClasses;

namespace Ringtoets.DuneErosion.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DuneLocationPropertiesTest
    {
        private const int requiredIdPropertyIndex = 0;
        private const int requiredNamePropertyIndex = 1;
        private const int requiredCoastalAreaIdPropertyIndex = 2;
        private const int requiredOffSetPropertyIndex = 3;
        private const int requiredLocationPropertyIndex = 4;
        private const int requiredWaterLevelPropertyIndex = 5;
        private const int requiredWaveHeightPropertyIndex = 6;
        private const int requiredWavePeriodPropertyIndex = 7;
        private const int requiredTargetProbabilityPropertyIndex = 8;
        private const int requiredTargetReliabilityPropertyIndex = 9;
        private const int requiredCalculatedProbabilityPropertyIndex = 10;
        private const int requiredCalculatedReliabilityPropertyIndex = 11;
        private const int requiredConvergencePropertyIndex = 12;

        [Test]
        public void GetProperties_ValidData_ReturnsExpectedValues()
        {
            // Setup
            var duneLocation = new TestDuneLocation();

            // Call
            var properties = new DuneLocationProperties
            {
                Data = duneLocation
            };

            // Assert
            Assert.AreEqual(duneLocation.Id, properties.Id);
            Assert.AreEqual(duneLocation.Name, properties.Name);
            Assert.AreEqual(duneLocation.CoastalAreaId, properties.CoastalAreaId);
            Assert.AreEqual(duneLocation.Offset, properties.Offset);
            Assert.AreEqual(duneLocation.Location, properties.Location);

            Assert.IsNaN(properties.WaterLevel);
            Assert.IsNaN(properties.WaveHeight);
            Assert.IsNaN(properties.WavePeriod);

            Assert.IsNaN(properties.TargetProbability);
            Assert.IsTrue(TypeUtils.HasTypeConverter<DuneLocationProperties,
                              NoProbabilityValueDoubleConverter>(p => p.TargetProbability));
            Assert.IsNaN(properties.TargetReliability);
            Assert.IsTrue(TypeUtils.HasTypeConverter<DuneLocationProperties,
                              NoValueRoundedDoubleConverter>(p => p.TargetReliability));

            Assert.IsNaN(properties.CalculatedProbability);
            Assert.IsTrue(TypeUtils.HasTypeConverter<DuneLocationProperties,
                              NoProbabilityValueDoubleConverter>(p => p.CalculatedProbability));
            Assert.IsNaN(properties.CalculatedReliability);
            Assert.IsTrue(TypeUtils.HasTypeConverter<DuneLocationProperties,
                              NoValueRoundedDoubleConverter>(p => p.CalculatedReliability));

            string convergenceValue = new EnumDisplayWrapper<CalculationConvergence>(CalculationConvergence.NotCalculated).DisplayName;
            Assert.AreEqual(convergenceValue, properties.Convergence);
        }

        [Test]
        [TestCase(CalculationConvergence.CalculatedNotConverged)]
        [TestCase(CalculationConvergence.CalculatedConverged)]
        [TestCase(CalculationConvergence.NotCalculated)]
        public void GetProperties_ValidDesignWaterLevel_ReturnsExpectedValues(CalculationConvergence convergence)
        {
            // Setup
            const long id = 1234L;
            const double x = 567.0;
            const double y = 890.0;
            const string name = "<some name>";
            const int coastalAreaId = 1337;

            var random = new Random();
            double offset = random.NextDouble();
            double orientation = random.NextDouble();
            double waterLevel = random.NextDouble();
            double waveHeight = random.NextDouble();
            double wavePeriod = random.NextDouble();
            double targetProbability = random.NextDouble();
            double targetReliability = random.NextDouble();
            double calculatedProbability = random.NextDouble();
            double calculatedReliability = random.NextDouble();

            var output = new DuneLocationOutput(waterLevel, waveHeight, wavePeriod, targetProbability, targetReliability,
                                                calculatedProbability, calculatedReliability, convergence);
            var location = new DuneLocation(id, name, new Point2D(x, y), coastalAreaId, offset, orientation, random.NextDouble())
            {
                Output = output
            };

            // Call
            var properties = new DuneLocationProperties
            {
                Data = location
            };

            // Assert
            Assert.AreEqual(id, properties.Id);
            Assert.AreEqual(name, properties.Name);
            Assert.AreEqual(coastalAreaId, properties.CoastalAreaId);
            Assert.AreEqual(1, location.Offset.NumberOfDecimalPlaces);
            Assert.AreEqual(location.Offset, properties.Offset);
            var expectedLocation = new Point2D(x, y);
            Assert.AreEqual(expectedLocation, properties.Location);

            Assert.AreEqual(2, properties.WaterLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(waterLevel, properties.WaterLevel, properties.WaterLevel.GetAccuracy());
            Assert.AreEqual(2, properties.WaveHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(waveHeight, properties.WaveHeight, properties.WaveHeight.GetAccuracy());
            Assert.AreEqual(2, properties.WavePeriod.NumberOfDecimalPlaces);
            Assert.AreEqual(wavePeriod, properties.WavePeriod, properties.WavePeriod.GetAccuracy());

            Assert.AreEqual(targetProbability, properties.TargetProbability);
            Assert.AreEqual(5, properties.TargetReliability.NumberOfDecimalPlaces);
            Assert.AreEqual(targetReliability, properties.TargetReliability, properties.TargetReliability.GetAccuracy());

            Assert.AreEqual(calculatedProbability, properties.CalculatedProbability, properties.CalculatedProbability);
            Assert.AreEqual(5, properties.CalculatedReliability.NumberOfDecimalPlaces);
            Assert.AreEqual(calculatedReliability, properties.CalculatedReliability, properties.CalculatedReliability.GetAccuracy());

            string convergenceValue = new EnumDisplayWrapper<CalculationConvergence>(convergence).DisplayName;
            Assert.AreEqual(convergenceValue, properties.Convergence);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var duneLocation = new TestDuneLocation();

            // Call
            var properties = new DuneLocationProperties
            {
                Data = duneLocation
            };

            // Assert
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            var propertyBag = new DynamicPropertyBag(properties);

            PropertyDescriptorCollection dynamicProperties = propertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            Assert.AreEqual(13, dynamicProperties.Count);
            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            PropertyDescriptor idProperty = dynamicProperties[requiredIdPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(idProperty,
                                                                            "Algemeen",
                                                                            "ID",
                                                                            "ID van de hydraulische duinlocatie in de database.",
                                                                            true);

            PropertyDescriptor nameProperty = dynamicProperties[requiredNamePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            "Algemeen",
                                                                            "Naam",
                                                                            "Naam van de hydraulische duinlocatie.",
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
                                                                            "Coördinaten van de hydraulische duinlocatie.",
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
                                                                            "Hs [m]",
                                                                            "Berekende rekenwaarde voor de significante golfhoogte voor het uitvoeren van een sterkteberekening voor het toetsspoor duinen.",
                                                                            true);

            PropertyDescriptor wavePeriodProperty = dynamicProperties[requiredWavePeriodPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(wavePeriodProperty,
                                                                            "Resultaat",
                                                                            "Tp [s]",
                                                                            "Berekende rekenwaarde voor de piekperiode van de golven voor het uitvoeren van een sterkteberekening voor het toetsspoor duinen.",
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
                                                                            "Is convergentie bereikt in de duinlocatie berekening?",
                                                                            true);
        }
    }
}