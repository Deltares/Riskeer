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
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.IO.Configurations;
using Ringtoets.Revetment.IO.Configurations.Helpers;

namespace Ringtoets.Revetment.IO.Test.Configurations.Helpers
{
    [TestFixture]
    public class WaveConditionsInputExtensionsTest
    {
        [Test]
        public void ToConfiguration_WithoutInput_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((WaveConditionsInput) null).ToConfiguration("name");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void ToConfiguration_WithoutName_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new WaveConditionsInput().ToConfiguration(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("value", exception.ParamName);
        }

        [Test]
        public void ToConfiguration_InputValuesNotSet_ReturnsExpectedConfiguration()
        {
            // Setup
            const string name = "name";

            // Call
            WaveConditionsCalculationConfiguration configuration = new WaveConditionsInput().ToConfiguration(name);

            // Assert
            Assert.AreEqual(name, configuration.Name);
            Assert.IsNaN(configuration.LowerBoundaryRevetment);
            Assert.IsNaN(configuration.UpperBoundaryRevetment);
            Assert.IsNaN(configuration.LowerBoundaryWaterLevels);
            Assert.IsNaN(configuration.UpperBoundaryWaterLevels);
            Assert.IsNaN(configuration.Orientation);
            Assert.AreEqual(ConfigurationWaveConditionsInputStepSize.Half, configuration.StepSize);
            Assert.IsNull(configuration.HydraulicBoundaryLocationName);
            Assert.IsNull(configuration.ForeshoreProfileId);
            Assert.IsNull(configuration.WaveReduction);
        }

        [Test]
        public void ToConfiguration_InputValuesSet_ReturnsExpectedConfiguration()
        {
            // Setup
            const string name = "other name";
            const string locationName = "name";
            const string foreshoreProfileName = "foreshore";

            const double breakWaterHeight = 9.22;
            const double lowerBoundaryRevetment = 1.2;
            const double upperBoundaryRevetment = 3.55;
            const double lowerBoundaryWaterLevels = 3.1;
            const double upperBoundaryWaterLevels = 8.66;
            const int orientation = 122;

            var input = new WaveConditionsInput
            {
                ForeshoreProfile = new TestForeshoreProfile(foreshoreProfileName),
                HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(locationName),
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment,
                UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment,
                LowerBoundaryWaterLevels = (RoundedDouble) lowerBoundaryWaterLevels,
                UpperBoundaryWaterLevels = (RoundedDouble) upperBoundaryWaterLevels,
                StepSize = WaveConditionsInputStepSize.One,
                Orientation = (RoundedDouble) orientation,
                BreakWater =
                {
                    Type = BreakWaterType.Wall,
                    Height = (RoundedDouble) breakWaterHeight
                },
                UseBreakWater = true,
                UseForeshore = true
            };

            // Call
            WaveConditionsCalculationConfiguration configuration = input.ToConfiguration(name);

            // Assert
            Assert.AreEqual(name, configuration.Name);
            Assert.AreEqual(lowerBoundaryRevetment, configuration.LowerBoundaryRevetment);
            Assert.AreEqual(upperBoundaryRevetment, configuration.UpperBoundaryRevetment);
            Assert.AreEqual(lowerBoundaryWaterLevels, configuration.LowerBoundaryWaterLevels);
            Assert.AreEqual(upperBoundaryWaterLevels, configuration.UpperBoundaryWaterLevels);
            Assert.AreEqual(orientation, configuration.Orientation);
            Assert.AreEqual(ConfigurationWaveConditionsInputStepSize.One, configuration.StepSize);
            Assert.AreEqual(locationName, configuration.HydraulicBoundaryLocationName);
            Assert.AreEqual(foreshoreProfileName, configuration.ForeshoreProfileId);
            Assert.AreEqual(breakWaterHeight, configuration.WaveReduction.BreakWaterHeight);
            Assert.AreEqual(ConfigurationBreakWaterType.Wall, configuration.WaveReduction.BreakWaterType);
            Assert.IsTrue(configuration.WaveReduction.UseForeshoreProfile);
            Assert.IsTrue(configuration.WaveReduction.UseBreakWater);
        }
    }
}