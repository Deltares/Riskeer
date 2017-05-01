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

using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PresentationObjects;
using Ringtoets.Revetment.TestUtil;

namespace Ringtoets.Revetment.Forms.TestUtil.Test
{
    [TestFixture]
    public class TestWaveConditionsInputContextTest
    {
        [Test]
        public void Constructor_WithInput_ExpectedValues()
        {
            // Setup
            var waveConditionsInput = new WaveConditionsInput();

            // Call
            var context = new TestWaveConditionsInputContext(waveConditionsInput);

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputContext>(context);
            Assert.AreSame(waveConditionsInput, context.WrappedData);
            Assert.IsInstanceOf<TestWaveConditionsCalculation>(context.Calculation);
            CollectionAssert.IsEmpty(context.ForeshoreProfiles);
            CollectionAssert.IsEmpty(context.HydraulicBoundaryLocations);
        }

        [Test]
        public void Constructor_WithInputAndForeshoreProfilesAndLocations_ExpectedValues()
        {
            // Setup
            var waveConditionsInput = new WaveConditionsInput();
            var profiles = new[]
            {
                new TestForeshoreProfile("test1"),
                new TestForeshoreProfile("test2")
            };
            var locations = new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            // Call
            var context = new TestWaveConditionsInputContext(waveConditionsInput, profiles, locations);

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputContext>(context);
            Assert.AreSame(waveConditionsInput, context.WrappedData);
            Assert.IsInstanceOf<TestWaveConditionsCalculation>(context.Calculation);
            CollectionAssert.AreEqual(profiles, context.ForeshoreProfiles);
            CollectionAssert.AreEqual(locations, context.HydraulicBoundaryLocations);
        }

        [Test]
        public void Constructor_WithAllParameters_ExpectedValues()
        {
            // Setup
            var waveConditionsInput = new WaveConditionsInput();
            var calculation = new TestWaveConditionsCalculation();
            var profiles = new[]
            {
                new TestForeshoreProfile("test1"),
                new TestForeshoreProfile("test2")
            };
            var locations = new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            // Call
            var context = new TestWaveConditionsInputContext(waveConditionsInput, calculation, profiles, locations);

            // Assert
            Assert.IsInstanceOf<WaveConditionsInputContext>(context);
            Assert.AreSame(waveConditionsInput, context.WrappedData);
            Assert.AreEqual(calculation, context.Calculation);
            CollectionAssert.AreEqual(profiles, context.ForeshoreProfiles);
            CollectionAssert.AreEqual(locations, context.HydraulicBoundaryLocations);
        }
    }
}