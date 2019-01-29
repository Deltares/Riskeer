﻿using System;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Riskeer.HydraRing.Calculation.Data.Input;

namespace Riskeer.Common.Service.Test
{
    [TestFixture]
    public class HydraRingCalculationSettingsFactoryTest
    {
        [Test]
        public void CreateSettings_HydraulicBoundaryCalculationSettingsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HydraRingCalculationSettingsFactory.CreateSettings(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryCalculationSettings", exception.ParamName);
        }

        [Test]
        public void CreateSettings_WithHydraulicBoundaryCalculationSettings_ReturnsExpectedSettings()
        {
            // Setup
            var hydraulicBoundaryCalculationSettings = new HydraulicBoundaryCalculationSettings("HydraulicBoundaryDataBaseFilePath",
                                                                                                "hlcdFilePath",
                                                                                                "preprocessorDirectory");

            // Call
            HydraRingCalculationSettings hydraRingCalculationSettings = HydraRingCalculationSettingsFactory.CreateSettings(hydraulicBoundaryCalculationSettings);

            // Assert
            Assert.AreEqual(hydraulicBoundaryCalculationSettings.HlcdFilePath, hydraRingCalculationSettings.HlcdFilePath);
            Assert.AreEqual(hydraulicBoundaryCalculationSettings.PreprocessorDirectory, hydraRingCalculationSettings.PreprocessorDirectory);
        }
    }
}