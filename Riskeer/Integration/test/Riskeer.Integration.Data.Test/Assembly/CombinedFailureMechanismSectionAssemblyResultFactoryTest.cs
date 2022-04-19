// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
                new CombinedFailureMechanismSectionAssemblyResultWrapper(
                    Enumerable.Empty<CombinedFailureMechanismSectionAssembly>(),
                    AssemblyMethod.BOI3A1, AssemblyMethod.BOI3B1, AssemblyMethod.BOI3C1),
                null, new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanisms", exception.ParamName);
        }

        [Test]
        public void Create_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => CombinedFailureMechanismSectionAssemblyResultFactory.Create(
                new CombinedFailureMechanismSectionAssemblyResultWrapper(
                    Enumerable.Empty<CombinedFailureMechanismSectionAssembly>(),
                    AssemblyMethod.BOI3A1, AssemblyMethod.BOI3B1, AssemblyMethod.BOI3C1),
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
            assessmentSection.SpecificFailureMechanisms.Add(new SpecificFailureMechanism());
            Dictionary<IFailureMechanism, int> failureMechanisms = assessmentSection.GetFailureMechanisms()
                                                                                    .Concat(assessmentSection.SpecificFailureMechanisms)
                                                                                    .Where(fm => fm.InAssembly)
                                                                                    .Select((fm, i) => new
                                                                                    {
                                                                                        FailureMechanism = fm,
                                                                                        Index = i
                                                                                    })
                                                                                    .ToDictionary(x => x.FailureMechanism, x => x.Index);

            var section1 = new CombinedAssemblyFailureMechanismSection(0, 5, random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());
            var section2 = new CombinedAssemblyFailureMechanismSection(5, 11, random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());
            var output = new CombinedFailureMechanismSectionAssemblyResultWrapper(
                new[]
                {
                    new CombinedFailureMechanismSectionAssembly(section1, GetFailureMechanismsOutput(failureMechanisms.Keys, random)),
                    new CombinedFailureMechanismSectionAssembly(section2, GetFailureMechanismsOutput(failureMechanisms.Keys, random))
                }, AssemblyMethod.BOI3A1, AssemblyMethod.BOI3B1, AssemblyMethod.BOI3C1);

            // Call
            CombinedFailureMechanismSectionAssemblyResult[] results = CombinedFailureMechanismSectionAssemblyResultFactory.Create(output, failureMechanisms, assessmentSection).ToArray();

            // Assert
            Assert.AreEqual(output.AssemblyResults.Count(), results.Length);
            for (var i = 0; i < output.AssemblyResults.Count(); i++)
            {
                CombinedFailureMechanismSectionAssembly assemblyResult = output.AssemblyResults.ElementAt(i);
                Assert.AreEqual(assemblyResult.Section.SectionStart, results[i].SectionStart);
                Assert.AreEqual(assemblyResult.Section.SectionEnd, results[i].SectionEnd);
                Assert.AreEqual(assemblyResult.Section.FailureMechanismSectionAssemblyGroup, results[i].TotalResult);
                Assert.AreEqual(assemblyResult.FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.Piping]), results[i].Piping);
                Assert.AreEqual(assemblyResult.FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.GrassCoverErosionInwards]), results[i].GrassCoverErosionInwards);
                Assert.AreEqual(assemblyResult.FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.MacroStabilityInwards]), results[i].MacroStabilityInwards);
                Assert.AreEqual(assemblyResult.FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.Microstability]), results[i].Microstability);
                Assert.AreEqual(assemblyResult.FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.StabilityStoneCover]), results[i].StabilityStoneCover);
                Assert.AreEqual(assemblyResult.FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.WaveImpactAsphaltCover]), results[i].WaveImpactAsphaltCover);
                Assert.AreEqual(assemblyResult.FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.WaterPressureAsphaltCover]), results[i].WaterPressureAsphaltCover);
                Assert.AreEqual(assemblyResult.FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.GrassCoverErosionOutwards]), results[i].GrassCoverErosionOutwards);
                Assert.AreEqual(assemblyResult.FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.GrassCoverSlipOffOutwards]), results[i].GrassCoverSlipOffOutwards);
                Assert.AreEqual(assemblyResult.FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.GrassCoverSlipOffInwards]), results[i].GrassCoverSlipOffInwards);
                Assert.AreEqual(assemblyResult.FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.HeightStructures]), results[i].HeightStructures);
                Assert.AreEqual(assemblyResult.FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.ClosingStructures]), results[i].ClosingStructures);
                Assert.AreEqual(assemblyResult.FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.PipingStructure]), results[i].PipingStructure);
                Assert.AreEqual(assemblyResult.FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.StabilityPointStructures]), results[i].StabilityPointStructures);
                Assert.AreEqual(assemblyResult.FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[assessmentSection.DuneErosion]), results[i].DuneErosion);
                Assert.AreEqual(assessmentSection.SpecificFailureMechanisms.Count, results[i].SpecificFailureMechanisms.Length);
                foreach (SpecificFailureMechanism specificFailureMechanism in assessmentSection.SpecificFailureMechanisms)
                {
                    Assert.AreEqual(assemblyResult.FailureMechanismSectionAssemblyGroupResults.ElementAt(failureMechanisms[specificFailureMechanism]), results[i].SpecificFailureMechanisms.Single());
                }

                Assert.AreEqual(output.CommonSectionAssemblyMethod, results[i].CommonSectionAssemblyMethod);
                Assert.AreEqual(output.FailureMechanismResultsAssemblyMethod, results[i].FailureMechanismResultsAssemblyMethod);
                Assert.AreEqual(output.CombinedSectionResultAssemblyMethod, results[i].CombinedSectionResultAssemblyMethod);
            }
        }

        [Test]
        public void Create_WithEmptyFailureMechanisms_ReturnsCombinedFailureMechanismSectionAssemblyResults()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            assessmentSection.SpecificFailureMechanisms.Add(new SpecificFailureMechanism());
            Dictionary<IFailureMechanism, int> failureMechanisms = assessmentSection.GetFailureMechanisms()
                                                                                    .Concat(assessmentSection.SpecificFailureMechanisms)
                                                                                    .Where(fm => fm.InAssembly)
                                                                                    .Select((fm, i) => new
                                                                                    {
                                                                                        FailureMechanism = fm,
                                                                                        Index = i
                                                                                    })
                                                                                    .ToDictionary(x => x.FailureMechanism, x => x.Index);
            var section1 = new CombinedAssemblyFailureMechanismSection(0, 5, random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());
            var section2 = new CombinedAssemblyFailureMechanismSection(5, 11, random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());
            var output = new CombinedFailureMechanismSectionAssemblyResultWrapper(
                new[]
                {
                    new CombinedFailureMechanismSectionAssembly(section1, GetFailureMechanismsOutput(failureMechanisms.Keys, random)),
                    new CombinedFailureMechanismSectionAssembly(section2, GetFailureMechanismsOutput(failureMechanisms.Keys, random))
                }, AssemblyMethod.BOI3A1, AssemblyMethod.BOI3B1, AssemblyMethod.BOI3C1);

            // Call
            CombinedFailureMechanismSectionAssemblyResult[] results = CombinedFailureMechanismSectionAssemblyResultFactory.Create(
                output, new Dictionary<IFailureMechanism, int>(), assessmentSection).ToArray();

            // Assert
            Assert.AreEqual(output.AssemblyResults.Count(), results.Length);
            for (var i = 0; i < output.AssemblyResults.Count(); i++)
            {
                CombinedFailureMechanismSectionAssembly assemblyResults = output.AssemblyResults.ElementAt(i);
                Assert.AreEqual(assemblyResults.Section.SectionStart, results[i].SectionStart);
                Assert.AreEqual(assemblyResults.Section.SectionEnd, results[i].SectionEnd);
                Assert.AreEqual(assemblyResults.Section.FailureMechanismSectionAssemblyGroup, results[i].TotalResult);

                Assert.IsNull(results[i].Piping);
                Assert.IsNull(results[i].GrassCoverErosionInwards);
                Assert.IsNull(results[i].MacroStabilityInwards);
                Assert.IsNull(results[i].Microstability);
                Assert.IsNull(results[i].StabilityStoneCover);
                Assert.IsNull(results[i].WaveImpactAsphaltCover);
                Assert.IsNull(results[i].WaterPressureAsphaltCover);
                Assert.IsNull(results[i].GrassCoverErosionOutwards);
                Assert.IsNull(results[i].GrassCoverSlipOffOutwards);
                Assert.IsNull(results[i].GrassCoverSlipOffInwards);
                Assert.IsNull(results[i].HeightStructures);
                Assert.IsNull(results[i].ClosingStructures);
                Assert.IsNull(results[i].PipingStructure);
                Assert.IsNull(results[i].StabilityPointStructures);
                Assert.IsNull(results[i].DuneErosion);
                Assert.AreEqual(assessmentSection.SpecificFailureMechanisms.Count, results[i].SpecificFailureMechanisms.Length);
                foreach (FailureMechanismSectionAssemblyGroup? failureMechanismSectionAssemblyGroup in results[i].SpecificFailureMechanisms)
                {
                    Assert.IsNull(failureMechanismSectionAssemblyGroup);
                }
            }
        }

        private static IEnumerable<FailureMechanismSectionAssemblyGroup> GetFailureMechanismsOutput(
            IEnumerable<IFailureMechanism> failureMechanisms, Random random)
        {
            return failureMechanisms.Select(fm => random.NextEnumValue<FailureMechanismSectionAssemblyGroup>()).ToArray();
        }
    }
}