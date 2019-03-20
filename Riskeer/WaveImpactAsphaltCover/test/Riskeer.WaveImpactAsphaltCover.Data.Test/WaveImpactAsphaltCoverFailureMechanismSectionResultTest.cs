// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Primitives;

namespace Riskeer.WaveImpactAsphaltCover.Data.Test
{
    [TestFixture]
    public class WaveImpactAsphaltCoverFailureMechanismSectionResultTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var result = new WaveImpactAsphaltCoverFailureMechanismSectionResult(section);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResult>(result);
            Assert.AreSame(section, result.Section);
            Assert.AreEqual(SimpleAssessmentResultType.None, result.SimpleAssessmentResult);
            Assert.AreEqual(DetailedAssessmentResultType.None, result.DetailedAssessmentResultForFactorizedSignalingNorm);
            Assert.AreEqual(DetailedAssessmentResultType.None, result.DetailedAssessmentResultForSignalingNorm);
            Assert.AreEqual(DetailedAssessmentResultType.None, result.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm);
            Assert.AreEqual(DetailedAssessmentResultType.None, result.DetailedAssessmentResultForLowerLimitNorm);
            Assert.AreEqual(DetailedAssessmentResultType.None, result.DetailedAssessmentResultForFactorizedLowerLimitNorm);
            Assert.AreEqual(TailorMadeAssessmentCategoryGroupResultType.None, result.TailorMadeAssessmentResult);
            Assert.IsFalse(result.UseManualAssembly);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, result.ManualAssemblyCategoryGroup);
        }
    }
}