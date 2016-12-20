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

using System;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.TestUtil.Test
{
    [TestFixture]
    public class TestWaveConditionsOutputTest
    {
        [Test]
        public void Constructor_Always_ReturnsWithCalculationConvergenceNotCalculated()
        {
            // Call
            WaveConditionsOutput output = new TestWaveConditionsOutput();

            // Assert
            Assert.AreEqual(1.1, output.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.AreEqual(2.2, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(3.3, output.WavePeakPeriod, output.WavePeakPeriod.GetAccuracy());
            Assert.AreEqual(4.4, output.WaveAngle, output.WaveAngle.GetAccuracy());
            Assert.AreEqual(5.5, output.WaveDirection, output.WaveDirection.GetAccuracy());

            Assert.AreEqual(0.1, output.TargetProbability);
            Assert.AreEqual(1.282, output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(0.4, output.CalculatedProbability);
            Assert.AreEqual(0.253, output.CalculatedReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, output.CalculationConvergence);
        }
    }
}