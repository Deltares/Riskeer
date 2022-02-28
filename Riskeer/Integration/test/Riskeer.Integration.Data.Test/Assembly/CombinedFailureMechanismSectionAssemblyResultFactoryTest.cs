// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
            void Call() => CombinedFailureMechanismSectionAssemblyResultFactory.Create(
                null, new Dictionary<IFailureMechanism, int>(),
                new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void Create_FailureMechanismsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => CombinedFailureMechanismSectionAssemblyResultFactory.Create(
                Enumerable.Empty<CombinedFailureMechanismSectionAssembly>(), null,
                new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanisms", exception.ParamName);
        }

        [Test]
        public void Create_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => CombinedFailureMechanismSectionAssemblyResultFactory.Create(
                Enumerable.Empty<CombinedFailureMechanismSectionAssembly>(),
                new Dictionary<IFailureMechanism, int>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Create_WithAllFailureMechanisms_ReturnsCombinedFailureMechanismSectionAssemblyResults()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            Dictionary<IFailureMechanism, int> failureMechanisms = assessmentSection.GetFailureMechanisms()
                                                                                    .Where(fm => fm.InAssembly)
                                                                                    .Select((fm, i) => new
                                                                                    {
                                                                                        FailureMechanism = fm,
                                                                                        Index = i
                                                                                    })
                                                                                    .ToDictionary(x => x.FailureMechanism, x => x.Index);

            var section1 = new CombinedAssemblyFailureMechanismSection(0, 5, random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());
            var section2 = new CombinedAssemblyFailureMechanismSection(5, 11, random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());
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
                Assert.AreEqual(output[i].Section.AssemblyGroup, results[i].TotalResult);

                Assert.AreEqual(output[i].FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.Piping]), results[i].Piping);
                Assert.AreEqual(output[i].FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.GrassCoverErosionInwards]), results[i].GrassCoverErosionInwards);
                Assert.AreEqual(output[i].FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.MacroStabilityInwards]), results[i].MacroStabilityInwards);
                Assert.AreEqual(output[i].FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.Microstability]), results[i].Microstability);
                Assert.AreEqual(output[i].FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.StabilityStoneCover]), results[i].StabilityStoneCover);
                Assert.AreEqual(output[i].FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.WaveImpactAsphaltCover]), results[i].WaveImpactAsphaltCover);
                Assert.AreEqual(output[i].FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.WaterPressureAsphaltCover]), results[i].WaterPressureAsphaltCover);
                Assert.AreEqual(output[i].FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.GrassCoverErosionOutwards]), results[i].GrassCoverErosionOutwards);
                Assert.AreEqual(output[i].FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.GrassCoverSlipOffOutwards]), results[i].GrassCoverSlipOffOutwards);
                Assert.AreEqual(output[i].FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.GrassCoverSlipOffInwards]), results[i].GrassCoverSlipOffInwards);
                Assert.AreEqual(output[i].FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.HeightStructures]), results[i].HeightStructures);
                Assert.AreEqual(output[i].FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.ClosingStructures]), results[i].ClosingStructures);
                Assert.AreEqual(output[i].FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.PipingStructure]), results[i].PipingStructure);
                Assert.AreEqual(output[i].FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.StabilityPointStructures]), results[i].StabilityPointStructures);
                Assert.AreEqual(output[i].FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.DuneErosion]), results[i].DuneErosion);
            }
        }

        [Test]
        public void Create_WithEmptyFailureMechanisms_ReturnsCombinedFailureMechanismSectionAssemblyResults()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            Dictionary<IFailureMechanism, int> failureMechanisms = assessmentSection.GetFailureMechanisms()
                                                                                    .Where(fm => fm.InAssembly)
                                                                                    .Select((fm, i) => new
                                                                                    {
                                                                                        FailureMechanism = fm,
                                                                                        Index = i
                                                                                    })
                                                                                    .ToDictionary(x => x.FailureMechanism, x => x.Index);

            var section1 = new CombinedAssemblyFailureMechanismSection(0, 5, random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());
            var section2 = new CombinedAssemblyFailureMechanismSection(5, 11, random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());
            var output = new[]
            {
                new CombinedFailureMechanismSectionAssembly(section1, GetFailureMechanismsOutput(failureMechanisms.Keys, random)),
                new CombinedFailureMechanismSectionAssembly(section2, GetFailureMechanismsOutput(failureMechanisms.Keys, random))
            };

            // Call
            CombinedFailureMechanismSectionAssemblyResult[] results = CombinedFailureMechanismSectionAssemblyResultFactory.Create(
                output, new Dictionary<IFailureMechanism, int>(), assessmentSection).ToArray();

            // Assert
            Assert.AreEqual(output.Length, results.Length);
            for (var i = 0; i < output.Length; i++)
            {
                Assert.AreEqual(output[i].Section.SectionStart, results[i].SectionStart);
                Assert.AreEqual(output[i].Section.SectionEnd, results[i].SectionEnd);
                Assert.AreEqual(output[i].Section.AssemblyGroup, results[i].TotalResult);

                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, results[i].Piping);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, results[i].GrassCoverErosionInwards);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, results[i].MacroStabilityInwards);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, results[i].Microstability);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, results[i].StabilityStoneCover);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, results[i].WaveImpactAsphaltCover);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, results[i].WaterPressureAsphaltCover);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, results[i].GrassCoverErosionOutwards);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, results[i].GrassCoverSlipOffOutwards);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, results[i].GrassCoverSlipOffInwards);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, results[i].HeightStructures);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, results[i].ClosingStructures);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, results[i].PipingStructure);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, results[i].StabilityPointStructures);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, results[i].DuneErosion);
            }
        }

        private static IEnumerable<FailureMechanismSectionAssemblyGroup> GetFailureMechanismsOutput(
            IEnumerable<IFailureMechanism> failureMechanisms, Random random)
        {
            return failureMechanisms.Select(fm => random.NextEnumValue<FailureMechanismSectionAssemblyGroup>()).ToArray();
        }
    }
}