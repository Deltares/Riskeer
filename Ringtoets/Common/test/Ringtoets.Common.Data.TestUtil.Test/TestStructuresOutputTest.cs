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
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class TestStructuresOutputTest
    {
        [Test]
        public void Constructor_SetExpectedValues()
        {
            // Call
            var output = new TestStructuresOutput();

            // Assert
            Assert.IsInstanceOf<StructuresOutput>(output);

            ProbabilityAssessmentOutput probabilityAssessmentOutput =
                output.ProbabilityAssessmentOutput;
            Assert.AreEqual(0, probabilityAssessmentOutput.FactorOfSafety.Value);
            Assert.AreEqual(0, probabilityAssessmentOutput.RequiredProbability);
            Assert.AreEqual(0, probabilityAssessmentOutput.Probability);
            Assert.AreEqual(0, probabilityAssessmentOutput.RequiredReliability.Value);
            Assert.AreEqual(0, probabilityAssessmentOutput.Reliability.Value);
            Assert.IsNull(output.GeneralResult);
            Assert.IsFalse(output.HasGeneralResult);
        }
    }
}