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
    public class CombinedAssemblyFailureMechanismSectionFactoryTest
    {
        [Test]
        public void CreateInput_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => CombinedAssemblyFailureMechanismSectionFactory.CreateInput(null, Enumerable.Empty<IFailureMechanism>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateInput_FailureMechanismsNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = new AssessmentSection(new Random(21).NextEnumValue<AssessmentSectionComposition>());

            // Call
            TestDelegate call = () => CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanisms", exception.ParamName);
        }

        [Test]
        public void CreateInput_WithAllFailureMechanisms_ReturnsInputCollection()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmenSectionWithAllFailureMechanismSectionsAndResults(
                new Random(21).NextEnumValue<AssessmentSectionComposition>());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                IEnumerable<CombinedAssemblyFailureMechanismSection>[] inputs = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(
                    assessmentSection, assessmentSection.GetFailureMechanisms()).ToArray();

                // Assert
                Assert.AreEqual(18, inputs.Length);
                AssertSections(assessmentSection.Piping.SectionResults, inputs[0]);
                AssertSections(assessmentSection.GrassCoverErosionInwards.SectionResults, inputs[1]);
                AssertSections(assessmentSection.MacroStabilityInwards.SectionResults, inputs[2]);
                AssertSections(assessmentSection.MacroStabilityOutwards.SectionResults, inputs[3]);
                AssertSections(assessmentSection.Microstability.SectionResults, inputs[4]);
                AssertSections(assessmentSection.StabilityStoneCover.SectionResults, inputs[5]);
                AssertSections(assessmentSection.WaveImpactAsphaltCover.SectionResults, inputs[6]);
                AssertSections(assessmentSection.WaterPressureAsphaltCover.SectionResults, inputs[7]);
                AssertSections(assessmentSection.GrassCoverErosionOutwards.SectionResults, inputs[8]);
                AssertSections(assessmentSection.GrassCoverSlipOffOutwards.SectionResults, inputs[9]);
                AssertSections(assessmentSection.GrassCoverSlipOffInwards.SectionResults, inputs[10]);
                AssertSections(assessmentSection.HeightStructures.SectionResults, inputs[11]);
                AssertSections(assessmentSection.ClosingStructures.SectionResults, inputs[12]);
                AssertSections(assessmentSection.PipingStructure.SectionResults, inputs[13]);
                AssertSections(assessmentSection.StabilityPointStructures.SectionResults, inputs[14]);
                AssertSections(assessmentSection.StrengthStabilityLengthwiseConstruction.SectionResults, inputs[15]);
                AssertSections(assessmentSection.DuneErosion.SectionResults, inputs[16]);
                AssertSections(assessmentSection.TechnicalInnovation.SectionResults, inputs[17]);
            }
        }

        private static void AssertSections<T>(IEnumerable<T> originalSectionResults, IEnumerable<CombinedAssemblyFailureMechanismSection> inputSections)
            where T : FailureMechanismSectionResult
        {
            Assert.AreEqual(originalSectionResults.Count(), inputSections.Count());

            double expectedSectionStart = 0;

            for (var i = 0; i < originalSectionResults.Count(); i++)
            {
                double expectedSectionEnd = expectedSectionStart + originalSectionResults.ElementAt(i).Section.Length;

                Assert.AreEqual(expectedSectionStart, inputSections.ElementAt(i).SectionStart);
                Assert.AreEqual(expectedSectionEnd, inputSections.ElementAt(i).SectionEnd);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIv, inputSections.ElementAt(i).CategoryGroup);

                expectedSectionStart = expectedSectionEnd;
            }
        }
    }
}