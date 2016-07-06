// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.HydraRing.Calculation.Data.Output;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Output
{
    [TestFixture]
    public class WaveHeightCalculationOutputTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var value = random.NextDouble();
            var isDominant = Convert.ToBoolean(random.Next(0, 2));

            // Call
            var targetProbabilityCalculationOutput = new WaveHeightCalculationOutput(value, isDominant);

            // Assert
            Assert.AreEqual(value, targetProbabilityCalculationOutput.WaveHeight);
            Assert.AreEqual(isDominant, targetProbabilityCalculationOutput.IsOvertoppingDominant);
        }
    }
}