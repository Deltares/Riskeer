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
using NUnit.Framework;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Revetment.IO.Configurations;

namespace Ringtoets.Revetment.IO.Test.Configurations
{
    [TestFixture]
    public class WaveConditionsCalculationConfigurationTest
    {
        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new WaveConditionsCalculationConfiguration(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithoutValues_PropertiesAreDefault()
        {
            // Setup
            var name = "some name";

            // Call
            var readCalculation = new WaveConditionsCalculationConfiguration(name);

            // Assert
            Assert.IsInstanceOf<IConfigurationItem>(readCalculation);
            Assert.AreEqual(name, readCalculation.Name);
            Assert.IsNull(readCalculation.HydraulicBoundaryLocation);
            Assert.IsNull(readCalculation.UpperBoundaryRevetment);
            Assert.IsNull(readCalculation.LowerBoundaryRevetment);
            Assert.IsNull(readCalculation.UpperBoundaryWaterLevels);
            Assert.IsNull(readCalculation.LowerBoundaryWaterLevels);
            Assert.IsNull(readCalculation.StepSize);
            Assert.IsNull(readCalculation.ForeshoreProfile);
            Assert.IsNull(readCalculation.Orientation);
            Assert.IsNull(readCalculation.UseBreakWater);
            Assert.IsNull(readCalculation.BreakWaterType);
            Assert.IsNull(readCalculation.BreakWaterHeight);
            Assert.IsNull(readCalculation.UseForeshore);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithValuesSet_PropertiesAsExpected()
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
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                UpperBoundaryRevetment = upperBoundaryRevetment,
                LowerBoundaryRevetment = lowerBoundaryRevetment,
                UpperBoundaryWaterLevels = upperBoundaryWaterLevels,
                LowerBoundaryWaterLevels = lowerBoundaryWaterLevels,
                StepSize = stepSize,
                ForeshoreProfile = foreshoreProfileName,
                Orientation = orientation,
                UseBreakWater = useBreakWater,
                BreakWaterType = breakWaterType,
                BreakWaterHeight = breakWaterHeight,
                UseForeshore = useForeshore
            };

            // Assert
            Assert.AreEqual(calculationName, readWaveConditionsCalculation.Name);
            Assert.AreEqual(hydraulicBoundaryLocation, readWaveConditionsCalculation.HydraulicBoundaryLocation);
            Assert.AreEqual(upperBoundaryRevetment, readWaveConditionsCalculation.UpperBoundaryRevetment);
            Assert.AreEqual(lowerBoundaryRevetment, readWaveConditionsCalculation.LowerBoundaryRevetment);
            Assert.AreEqual(upperBoundaryWaterLevels, readWaveConditionsCalculation.UpperBoundaryWaterLevels);
            Assert.AreEqual(lowerBoundaryWaterLevels, readWaveConditionsCalculation.LowerBoundaryWaterLevels);
            Assert.AreEqual(stepSize, readWaveConditionsCalculation.StepSize);
            Assert.AreEqual(foreshoreProfileName, readWaveConditionsCalculation.ForeshoreProfile);
            Assert.AreEqual(orientation, readWaveConditionsCalculation.Orientation);
            Assert.AreEqual(useBreakWater, readWaveConditionsCalculation.UseBreakWater);
            Assert.AreEqual(breakWaterType, readWaveConditionsCalculation.BreakWaterType);
            Assert.AreEqual(breakWaterHeight, readWaveConditionsCalculation.BreakWaterHeight);
            Assert.AreEqual(useForeshore, readWaveConditionsCalculation.UseForeshore);
        }
    }
}