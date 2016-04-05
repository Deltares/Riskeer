﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.Overtopping
{
    [TestFixture]
    public class OvertoppingVariableCriticalOvertoppingTest
    {
        [Test]
        [TestCase(1.1, double.NaN, 1.1, 0.0006)]
        [TestCase(double.NaN, 1.1, 0.004, 1.1)]
        [TestCase(double.NaN, double.NaN, 0.004, 0.0006)]
        [TestCase(1.1, 2.2, 1.1, 2.2)]
        public void Constructor_EditableValues_ReturnsExpectedValues(double mean, double standardDeviation, double expectedMean, double expectedStandardDeviation)
        {
            // Call
            OvertoppingVariableCriticalOvertopping criticalOvertopping = new OvertoppingVariableCriticalOvertopping(mean, standardDeviation);

            // Assert
            Assert.AreEqual(expectedMean, criticalOvertopping.Mean);
            Assert.AreEqual(expectedStandardDeviation, criticalOvertopping.Variability);
        }
    }
}