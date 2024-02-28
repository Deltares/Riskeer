// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
// 
// This file is part of DiKErnel.
// 
// DiKErnel is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with this
// program. If not, see <http://www.gnu.org/licenses/>.
// 
// All names, logos, and references to "Deltares" are registered trademarks of Stichting
// Deltares and remain full property of Stichting Deltares at all times. All rights reserved.

using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Data.Assembly;

namespace Riskeer.Integration.Data.Test.Assembly
{
    [TestFixture]
    public class AssessmentSectionAssemblyHelperTest
    {
        [Test]
        public void AllCorrelatedFailureMechanismsInAssembly_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AssessmentSectionAssemblyHelper.AllCorrelatedFailureMechanismsInAssembly(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(true, true, true)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void AllCorrelatedFailureMechanismsInAssembly_VariousCorrelatedFailureMechanismsInAssemblyScenarios_ReturnsExpectedResult(
            bool grassCoverErosionInwardsInAssembly, bool heightStructuresInAssembly, bool expectedResult)
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>())
            {
                GrassCoverErosionInwards =
                {
                    InAssembly = grassCoverErosionInwardsInAssembly
                },
                HeightStructures =
                {
                    InAssembly = heightStructuresInAssembly
                }
            };

            // Call
            bool result = AssessmentSectionAssemblyHelper.AllCorrelatedFailureMechanismsInAssembly(assessmentSection);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}