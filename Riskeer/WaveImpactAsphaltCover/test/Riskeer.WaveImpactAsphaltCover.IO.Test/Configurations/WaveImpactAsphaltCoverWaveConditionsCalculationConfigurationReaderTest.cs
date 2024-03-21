﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.IO.Configurations;
using Riskeer.Revetment.IO.Configurations;
using Riskeer.WaveImpactAsphaltCover.IO.Configurations;

namespace Riskeer.WaveImpactAsphaltCover.IO.Test.Configurations
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationReaderTest
    {
        private readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.WaveImpactAsphaltCover.IO,
                                                                               nameof(WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationReader));

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationEmptyCalculation.xml");

            // Call
            var reader = new WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationReader(filePath);

            // Assert
            Assert.IsInstanceOf<WaveConditionsCalculationConfigurationReader<WaveConditionsCalculationConfiguration>>(reader);
        }

        [Test]
        public void Read_ValidConfigurationWithFullCalculation_ReturnExpectedReadWaveConditionsCalculation()
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, "validConfigurationFullCalculation.xml");
            var reader = new WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readItems = reader.Read().ToArray();

            // Assert
            var configuration = (WaveConditionsCalculationConfiguration) readItems.Single();

            AssertConfiguration(configuration);
            Assert.AreEqual(0.1, configuration.TargetProbability);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void Read_ValidPreviousVersionConfigurationWithFullCalculation_ReturnExpectedReadCalculation(int versionNumber)
        {
            // Setup
            string filePath = Path.Combine(testDirectoryPath, $"version{versionNumber}ValidConfigurationFullCalculation.xml");
            var reader = new WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationReader(filePath);

            // Call
            IEnumerable<IConfigurationItem> readConfigurationItems = reader.Read().ToArray();

            // Assert
            var configuration = (WaveConditionsCalculationConfiguration) readConfigurationItems.Single();

            AssertMigratedConfiguration(configuration);
            Assert.IsNull(configuration.TargetProbability);
        }

        private static void AssertConfiguration(WaveConditionsCalculationConfiguration configuration)
        {
            Assert.IsNotNull(configuration);
            Assert.AreEqual("Locatie", configuration.HydraulicBoundaryLocationName);
            Assert.AreEqual(1.1, configuration.UpperBoundaryRevetment);
            Assert.AreEqual(2.2, configuration.LowerBoundaryRevetment);
            Assert.AreEqual(3.3, configuration.UpperBoundaryWaterLevels);
            Assert.AreEqual(4.4, configuration.LowerBoundaryWaterLevels);
            Assert.AreEqual(0.55, configuration.StepSize);
            Assert.AreEqual("Voorlandprofiel", configuration.ForeshoreProfileId);
            Assert.AreEqual(6.6, configuration.Orientation);
            Assert.IsTrue(configuration.WaveReduction.UseBreakWater);
            Assert.AreEqual(ConfigurationBreakWaterType.Caisson, configuration.WaveReduction.BreakWaterType);
            Assert.AreEqual(7.7, configuration.WaveReduction.BreakWaterHeight);
            Assert.IsFalse(configuration.WaveReduction.UseForeshoreProfile);
        }
        
        private static void AssertMigratedConfiguration(WaveConditionsCalculationConfiguration configuration)
        {
            Assert.IsNotNull(configuration);
            Assert.AreEqual("Locatie", configuration.HydraulicBoundaryLocationName);
            Assert.AreEqual(1.1, configuration.UpperBoundaryRevetment);
            Assert.AreEqual(2.2, configuration.LowerBoundaryRevetment);
            Assert.AreEqual(3.3, configuration.UpperBoundaryWaterLevels);
            Assert.AreEqual(4.4, configuration.LowerBoundaryWaterLevels);
            Assert.AreEqual(0.5, configuration.StepSize);
            Assert.AreEqual("Voorlandprofiel", configuration.ForeshoreProfileId);
            Assert.AreEqual(5.5, configuration.Orientation);
            Assert.IsTrue(configuration.WaveReduction.UseBreakWater);
            Assert.AreEqual(ConfigurationBreakWaterType.Caisson, configuration.WaveReduction.BreakWaterType);
            Assert.AreEqual(6.6, configuration.WaveReduction.BreakWaterHeight);
            Assert.IsFalse(configuration.WaveReduction.UseForeshoreProfile);
        }
    }
}