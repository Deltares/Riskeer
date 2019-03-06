﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;

namespace Riskeer.GrassCoverErosionInwards.Data.TestUtil.Test
{
    [TestFixture]
    public class TestOvertoppingOutputTest
    {
        [Test]
        public void Constructor_WithReliability_ExpectedValues()
        {
            // Setup
            const double reliability = 0.3;

            // Call
            var output = new TestOvertoppingOutput(reliability);

            // Assert
            Assert.IsInstanceOf<OvertoppingOutput>(output);

            Assert.AreEqual(1.0, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.IsTrue(output.IsOvertoppingDominant);
            Assert.AreEqual(reliability, output.Reliability);

            Assert.IsNull(output.GeneralResult);
        }

        [Test]
        public void Constructor_WithGeneralResult_ExpectedValues()
        {
            // Setup
            var generalResult = new TestGeneralResultFaultTreeIllustrationPoint();

            // Call
            var output = new TestOvertoppingOutput(generalResult);

            // Assert
            Assert.IsInstanceOf<OvertoppingOutput>(output);

            Assert.AreEqual(1.0, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.IsTrue(output.IsOvertoppingDominant);
            Assert.AreEqual(1.0, output.Reliability);

            Assert.AreSame(generalResult, output.GeneralResult);
        }
    }
}