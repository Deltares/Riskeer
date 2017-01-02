using System;
using Core.Common.Base.Geometry;
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
    public class DuneLocationContextPropertiesTest
    {
        private const int requiredIdPropertyIndex = 0;
        private const int requiredNamePropertyIndex = 1;
        private const int requiredCoastalAreaIdPropertyIndex = 2;
        private const int requiredLocationPropertyIndex = 3;
        private const int requiredWaterLevelPropertyIndex = 4;
        private const int requiredWaveHeightPropertyIndex = 5;
        private const int requiredWavePeakPeriodPropertyIndex = 6;
        private const int requiredSpectralWavePeriodPropertyIndex = 7;
        private const int requiredTargetProbabilityPropertyIndex = 8;
        private const int requiredTargetReliabilityPropertyIndex = 9;
        private const int requiredCalculatedProbabilityPropertyIndex = 10;
        private const int requiredCalculatedReliabilityPropertyIndex = 11;

        [Test]
        public void GetProperties_ValidData_ReturnsExpectedValues()
        {
            // Setup
            var duneLocation = new TestDuneLocation();

            // Call
            var properties = new DuneLocationContextProperties
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
            Assert.IsTrue(TypeUtils.HasTypeConverter<DuneLocationContextProperties,
                              NoProbabilityValueDoubleConverter>(p => p.TargetProbability));
            Assert.IsNaN(properties.TargetReliability);
            Assert.IsTrue(TypeUtils.HasTypeConverter<DuneLocationContextProperties,
                              NoValueRoundedDoubleConverter>(p => p.TargetReliability));

            Assert.IsNaN(properties.CalculatedProbability);
            Assert.IsTrue(TypeUtils.HasTypeConverter<DuneLocationContextProperties,
                              NoProbabilityValueDoubleConverter>(p => p.CalculatedProbability));
            Assert.IsNaN(properties.CalculatedReliability);
            Assert.IsTrue(TypeUtils.HasTypeConverter<DuneLocationContextProperties,
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
            var location = new DuneLocation(id, name, new Point2D(x, y), coastalAreaId, offset, orientation, random.NextDouble());
            location.Output = output;

            // Call
            var properties = new DuneLocationContextProperties
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
    }
}