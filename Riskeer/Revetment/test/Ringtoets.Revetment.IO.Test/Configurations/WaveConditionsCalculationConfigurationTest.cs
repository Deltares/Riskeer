// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Revetment.IO.Configurations;

namespace Ringtoets.Revetment.IO.Test.Configurations
{
    [TestFixture]
    public class WaveConditionsCalculationConfigurationTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new WaveConditionsCalculationConfiguration(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_WithName_ExpectedValues()
        {
            // Setup
            const string name = "some name";

            // Call
            var readCalculation = new WaveConditionsCalculationConfiguration(name);

            // Assert
            Assert.IsInstanceOf<IConfigurationItem>(readCalculation);
            Assert.AreEqual(name, readCalculation.Name);
            Assert.IsNull(readCalculation.HydraulicBoundaryLocationName);
            Assert.IsNull(readCalculation.UpperBoundaryRevetment);
            Assert.IsNull(readCalculation.LowerBoundaryRevetment);
            Assert.IsNull(readCalculation.UpperBoundaryWaterLevels);
            Assert.IsNull(readCalculation.LowerBoundaryWaterLevels);
            Assert.IsNull(readCalculation.StepSize);
            Assert.IsNull(readCalculation.ForeshoreProfileId);
            Assert.IsNull(readCalculation.Orientation);
            Assert.IsNull(readCalculation.WaveReduction);
        }

        [Test]
        public void SimpleProperties_SetNewValues_NewValuesSet()
        {
            // Setup
            const string calculationName = "Name of the calculation";
            const string hydraulicBoundaryLocation = "Name of the hydraulic boundary location";
            const double upperBoundaryRevetment = 1.1;
            const double lowerBoundaryRevetment = 2.2;
            const double upperBoundaryWaterLevels = 3.3;
            const double lowerBoundaryWaterLevels = 4.4;
            const ConfigurationWaveConditionsInputStepSize stepSize = ConfigurationWaveConditionsInputStepSize.Half;
            const string foreshoreProfileName = "Name of the foreshore profile";
            const double orientation = 6.6;
            const bool useBreakWater = true;
            const ConfigurationBreakWaterType breakWaterType = ConfigurationBreakWaterType.Caisson;
            const double breakWaterHeight = 7.7;
            const bool useForeshore = false;

            // Call
            var readWaveConditionsCalculation = new WaveConditionsCalculationConfiguration(calculationName)
            {
                HydraulicBoundaryLocationName = hydraulicBoundaryLocation,
                UpperBoundaryRevetment = upperBoundaryRevetment,
                LowerBoundaryRevetment = lowerBoundaryRevetment,
                UpperBoundaryWaterLevels = upperBoundaryWaterLevels,
                LowerBoundaryWaterLevels = lowerBoundaryWaterLevels,
                StepSize = stepSize,
                ForeshoreProfileId = foreshoreProfileName,
                Orientation = orientation,
                WaveReduction = new WaveReductionConfiguration
                {
                    UseBreakWater = useBreakWater,
                    BreakWaterType = breakWaterType,
                    BreakWaterHeight = breakWaterHeight,
                    UseForeshoreProfile = useForeshore
                }
            };

            // Assert
            Assert.AreEqual(calculationName, readWaveConditionsCalculation.Name);
            Assert.AreEqual(hydraulicBoundaryLocation, readWaveConditionsCalculation.HydraulicBoundaryLocationName);
            Assert.AreEqual(upperBoundaryRevetment, readWaveConditionsCalculation.UpperBoundaryRevetment);
            Assert.AreEqual(lowerBoundaryRevetment, readWaveConditionsCalculation.LowerBoundaryRevetment);
            Assert.AreEqual(upperBoundaryWaterLevels, readWaveConditionsCalculation.UpperBoundaryWaterLevels);
            Assert.AreEqual(lowerBoundaryWaterLevels, readWaveConditionsCalculation.LowerBoundaryWaterLevels);
            Assert.AreEqual(stepSize, readWaveConditionsCalculation.StepSize);
            Assert.AreEqual(foreshoreProfileName, readWaveConditionsCalculation.ForeshoreProfileId);
            Assert.AreEqual(orientation, readWaveConditionsCalculation.Orientation);
            Assert.AreEqual(useBreakWater, readWaveConditionsCalculation.WaveReduction.UseBreakWater);
            Assert.AreEqual(breakWaterType, readWaveConditionsCalculation.WaveReduction.BreakWaterType);
            Assert.AreEqual(breakWaterHeight, readWaveConditionsCalculation.WaveReduction.BreakWaterHeight);
            Assert.AreEqual(useForeshore, readWaveConditionsCalculation.WaveReduction.UseForeshoreProfile);
        }

        [Test]
        public void Name_Null_ThrowsArgumentNullException()
        {
            // Setup
            var calculationConfiguration = new WaveConditionsCalculationConfiguration("valid name");

            // Call
            TestDelegate test = () => calculationConfiguration.Name = null;

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }
    }
}