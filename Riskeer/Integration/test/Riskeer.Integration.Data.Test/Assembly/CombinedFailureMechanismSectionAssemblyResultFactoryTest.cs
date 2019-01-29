// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.Data.Assembly;

namespace Riskeer.Integration.Data.Test.Assembly
{
    public class CombinedFailureMechanismSectionAssemblyResultFactoryTest
    {
        [Test]
        public void Create_OutputNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => CombinedFailureMechanismSectionAssemblyResultFactory.Create(null, new Dictionary<IFailureMechanism, int>(),
                                                                                                  new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void Create_FailureMechanismsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => CombinedFailureMechanismSectionAssemblyResultFactory.Create(Enumerable.Empty<CombinedFailureMechanismSectionAssembly>(),
                                                                                                  null, new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanisms", exception.ParamName);
        }

        [Test]
        public void Create_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => CombinedFailureMechanismSectionAssemblyResultFactory.Create(Enumerable.Empty<CombinedFailureMechanismSectionAssembly>(),
                                                                                                  new Dictionary<IFailureMechanism, int>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Create_WithAllFailureMechanisms_ReturnsCombinedFailureMechanismSectionAssemblyResults()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            Dictionary<IFailureMechanism, int> failureMechanisms = assessmentSection.GetFailureMechanisms()
                                                                                    .Where(fm => fm.IsRelevant)
                                                                                    .Select((fm, i) => new
                                                                                    {
                                                                                        FailureMechanism = fm,
                                                                                        Index = i
                                                                                    })
                                                                                    .ToDictionary(x => x.FailureMechanism, x => x.Index);

            var section1 = new CombinedAssemblyFailureMechanismSection(0, 5, random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var section2 = new CombinedAssemblyFailureMechanismSection(5, 11, random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var output = new[]
            {
                new CombinedFailureMechanismSectionAssembly(section1, GetFailureMechanismsOutput(failureMechanisms.Keys, random)),
                new CombinedFailureMechanismSectionAssembly(section2, GetFailureMechanismsOutput(failureMechanisms.Keys, random))
            };

            // Call
            CombinedFailureMechanismSectionAssemblyResult[] results = CombinedFailureMechanismSectionAssemblyResultFactory.Create(output, failureMechanisms, assessmentSection).ToArray();

            // Assert
            Assert.AreEqual(output.Length, results.Length);
            for (var i = 0; i < output.Length; i++)
            {
                Assert.AreEqual(i + 1, results[i].SectionNumber);
                Assert.AreEqual(output[i].Section.SectionStart, results[i].SectionStart);
                Assert.AreEqual(output[i].Section.SectionEnd, results[i].SectionEnd);
                Assert.AreEqual(output[i].Section.CategoryGroup, results[i].TotalResult);

                Assert.AreEqual(output[i].FailureMechanismResults.ElementAt(failureMechanisms[assessmentSection.Piping]), results[i].Piping);
                Assert.AreEqual(output[i].FailureMechanismResults.ElementAt(failureMechanisms[assessmentSection.GrassCoverErosionInwards]), results[i].GrassCoverErosionInwards);
                Assert.AreEqual(output[i].FailureMechanismResults.ElementAt(failureMechanisms[assessmentSection.MacroStabilityInwards]), results[i].MacroStabilityInwards);
                Assert.AreEqual(output[i].FailureMechanismResults.ElementAt(failureMechanisms[assessmentSection.MacroStabilityOutwards]), results[i].MacroStabilityOutwards);
                Assert.AreEqual(output[i].FailureMechanismResults.ElementAt(failureMechanisms[assessmentSection.Microstability]), results[i].Microstability);
                Assert.AreEqual(output[i].FailureMechanismResults.ElementAt(failureMechanisms[assessmentSection.StabilityStoneCover]), results[i].StabilityStoneCover);
                Assert.AreEqual(output[i].FailureMechanismResults.ElementAt(failureMechanisms[assessmentSection.WaveImpactAsphaltCover]), results[i].WaveImpactAsphaltCover);
                Assert.AreEqual(output[i].FailureMechanismResults.ElementAt(failureMechanisms[assessmentSection.WaterPressureAsphaltCover]), results[i].WaterPressureAsphaltCover);
                Assert.AreEqual(output[i].FailureMechanismResults.ElementAt(failureMechanisms[assessmentSection.GrassCoverErosionOutwards]), results[i].GrassCoverErosionOutwards);
                Assert.AreEqual(output[i].FailureMechanismResults.ElementAt(failureMechanisms[assessmentSection.GrassCoverSlipOffOutwards]), results[i].GrassCoverSlipOffOutwards);
                Assert.AreEqual(output[i].FailureMechanismResults.ElementAt(failureMechanisms[assessmentSection.GrassCoverSlipOffInwards]), results[i].GrassCoverSlipOffInwards);
                Assert.AreEqual(output[i].FailureMechanismResults.ElementAt(failureMechanisms[assessmentSection.HeightStructures]), results[i].HeightStructures);
                Assert.AreEqual(output[i].FailureMechanismResults.ElementAt(failureMechanisms[assessmentSection.ClosingStructures]), results[i].ClosingStructures);
                Assert.AreEqual(output[i].FailureMechanismResults.ElementAt(failureMechanisms[assessmentSection.PipingStructure]), results[i].PipingStructure);
                Assert.AreEqual(output[i].FailureMechanismResults.ElementAt(failureMechanisms[assessmentSection.StabilityPointStructures]), results[i].StabilityPointStructures);
                Assert.AreEqual(output[i].FailureMechanismResults.ElementAt(failureMechanisms[assessmentSection.StrengthStabilityLengthwiseConstruction]), results[i].StrengthStabilityLengthwiseConstruction);
                Assert.AreEqual(output[i].FailureMechanismResults.ElementAt(failureMechanisms[assessmentSection.DuneErosion]), results[i].DuneErosion);
                Assert.AreEqual(output[i].FailureMechanismResults.ElementAt(failureMechanisms[assessmentSection.TechnicalInnovation]), results[i].TechnicalInnovation);
            }
        }

        [Test]
        public void Create_WithEmptyFailureMechanisms_ReturnsCombinedFailureMechanismSectionAssemblyResults()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            Dictionary<IFailureMechanism, int> failureMechanisms = assessmentSection.GetFailureMechanisms()
                                                                                    .Where(fm => fm.IsRelevant)
                                                                                    .Select((fm, i) => new
                                                                                    {
                                                                                        FailureMechanism = fm,
                                                                                        Index = i
                                                                                    })
                                                                                    .ToDictionary(x => x.FailureMechanism, x => x.Index);

            var section1 = new CombinedAssemblyFailureMechanismSection(0, 5, random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var section2 = new CombinedAssemblyFailureMechanismSection(5, 11, random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var output = new[]
            {
                new CombinedFailureMechanismSectionAssembly(section1, GetFailureMechanismsOutput(failureMechanisms.Keys, random)),
                new CombinedFailureMechanismSectionAssembly(section2, GetFailureMechanismsOutput(failureMechanisms.Keys, random))
            };

            // Call
            CombinedFailureMechanismSectionAssemblyResult[] results = CombinedFailureMechanismSectionAssemblyResultFactory.Create(output,
                                                                                                                                  new Dictionary<IFailureMechanism, int>(),
                                                                                                                                  assessmentSection).ToArray();

            // Assert
            Assert.AreEqual(output.Length, results.Length);
            for (var i = 0; i < output.Length; i++)
            {
                Assert.AreEqual(output[i].Section.SectionStart, results[i].SectionStart);
                Assert.AreEqual(output[i].Section.SectionEnd, results[i].SectionEnd);
                Assert.AreEqual(output[i].Section.CategoryGroup, results[i].TotalResult);

                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, results[i].Piping);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, results[i].GrassCoverErosionInwards);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, results[i].MacroStabilityInwards);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, results[i].MacroStabilityOutwards);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, results[i].Microstability);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, results[i].StabilityStoneCover);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, results[i].WaveImpactAsphaltCover);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, results[i].WaterPressureAsphaltCover);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, results[i].GrassCoverErosionOutwards);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, results[i].GrassCoverSlipOffOutwards);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, results[i].GrassCoverSlipOffInwards);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, results[i].HeightStructures);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, results[i].ClosingStructures);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, results[i].PipingStructure);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, results[i].StabilityPointStructures);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, results[i].StrengthStabilityLengthwiseConstruction);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, results[i].DuneErosion);
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, results[i].TechnicalInnovation);
            }
        }

        private static IEnumerable<FailureMechanismSectionAssemblyCategoryGroup> GetFailureMechanismsOutput(IEnumerable<IFailureMechanism> failureMechanisms,
                                                                                                            Random random)
        {
            return failureMechanisms.Select(fm => random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()).ToArray();
        }
    }
}