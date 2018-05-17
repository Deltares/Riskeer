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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data.Assembly;
using Ringtoets.Integration.TestUtil;

namespace Ringtoets.Integration.Data.Test.Assembly
{
    [TestFixture]
    public class CombinedAssemblyFailureMechanismInputFactoryTest
    {
        [Test]
        public void CreateInput_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => CombinedAssemblyFailureMechanismInputFactory.CreateInput(null, Enumerable.Empty<IFailureMechanism>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateInput_failureMechanismsNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = new AssessmentSection(new Random(21).NextEnumValue<AssessmentSectionComposition>());

            // Call
            TestDelegate call = () => CombinedAssemblyFailureMechanismInputFactory.CreateInput(assessmentSection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanisms", exception.ParamName);
        }

        [Test]
        public void CreateInput_WithAllFailureMechanismsRelevant_ReturnsInputCollection()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmensectionWithAllFailureMechanismSectionsAndResults(
                new Random(21).NextEnumValue<AssessmentSectionComposition>());

            IFailureMechanism[] failureMechanisms = assessmentSection.GetFailureMechanisms().ToArray();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                IEnumerable<CombinedAssemblyFailureMechanismSection>[] inputs = CombinedAssemblyFailureMechanismInputFactory.CreateInput(
                    assessmentSection, failureMechanisms).ToArray();

                // Assert
                Assert.AreEqual(4, inputs.Length);
                AssertSections(assessmentSection.Piping.SectionResults.ToArray(), inputs[0].ToArray());
                AssertSections(assessmentSection.GrassCoverErosionInwards.SectionResults.ToArray(), inputs[1].ToArray());
                AssertSections(assessmentSection.MacroStabilityInwards.SectionResults.ToArray(), inputs[2].ToArray());
                AssertSections(assessmentSection.MacroStabilityOutwards.SectionResults.ToArray(), inputs[3].ToArray());
            }
        }

        private static void AssertSections<T>(T[] originalSectionResults, CombinedAssemblyFailureMechanismSection[] inputSections)
            where T : FailureMechanismSectionResult
        {
            Assert.AreEqual(originalSectionResults.Length, inputSections.Length);

            double expectedSectionStart = 0;

            for (var i = 0; i < originalSectionResults.Length; i++)
            {
                double expectedSectionEnd = expectedSectionStart + originalSectionResults[i].Section.Length;

                Assert.AreEqual(expectedSectionStart, inputSections[i].SectionStart);
                Assert.AreEqual(expectedSectionEnd, inputSections[i].SectionEnd);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIv, inputSections[i].CategoryGroup);

                expectedSectionStart = expectedSectionEnd;
            }
        }
    }
}