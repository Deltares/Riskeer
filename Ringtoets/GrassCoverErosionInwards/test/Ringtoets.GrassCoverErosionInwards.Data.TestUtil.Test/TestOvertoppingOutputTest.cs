﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.GrassCoverErosionInwards.Data.TestUtil.Test
{
    [TestFixture]
    public class TestOvertoppingOutputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const double reliability = 0.3;

            // Call
            var output = new TestOvertoppingOutput(reliability);

            // Assert
            Assert.IsInstanceOf<OvertoppingOutput>(output);

            Assert.AreEqual(1.0, output.WaveHeight.Value);
            Assert.IsTrue(output.IsOvertoppingDominant);
            Assert.AreEqual(reliability, output.Reliability);

            ProbabilityAssessmentOutput probabilityAssessmentOutput = output.ProbabilityAssessmentOutput;
            Assert.AreEqual(0, probabilityAssessmentOutput.FactorOfSafety, probabilityAssessmentOutput.FactorOfSafety.GetAccuracy());
            Assert.AreEqual(0, probabilityAssessmentOutput.Probability);
            Assert.AreEqual(0, probabilityAssessmentOutput.Reliability, probabilityAssessmentOutput.Reliability.GetAccuracy());
            Assert.AreEqual(0, probabilityAssessmentOutput.RequiredProbability);
            Assert.AreEqual(0, probabilityAssessmentOutput.RequiredReliability, probabilityAssessmentOutput.RequiredReliability.GetAccuracy());
            Assert.IsNull(output.GeneralResult);
        }
    }
}