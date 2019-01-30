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

using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.Data.Assembly;

namespace Riskeer.Integration.Data.Test.Assembly
{
    [TestFixture]
    public class CombinedFailureMechanismSectionAssemblyResultTest
    {
        [Test]
        public void Constructor_PropertiesNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => new CombinedFailureMechanismSectionAssemblyResult(random.Next(), random.NextDouble(), random.NextDouble(),
                                                                                        random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("properties", exception.ParamName);
        }

        [Test]
        public void Constructor_WithEmptyConstructionProperties_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            int sectionNumber = random.Next();
            double sectionStart = random.NextDouble();
            double sectionEnd = random.NextDouble();
            var totalResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            // Call
            var result = new CombinedFailureMechanismSectionAssemblyResult(sectionNumber, sectionStart, sectionEnd, totalResult,
                                                                           new CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties());

            // Assert
            Assert.AreEqual(sectionNumber, result.SectionNumber);
            Assert.AreEqual(sectionStart, result.SectionStart);
            Assert.AreEqual(sectionEnd, result.SectionEnd);
            Assert.AreEqual(totalResult, result.TotalResult);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, result.Piping);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, result.GrassCoverErosionInwards);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, result.MacroStabilityInwards);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, result.MacroStabilityOutwards);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, result.Microstability);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, result.StabilityStoneCover);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, result.WaveImpactAsphaltCover);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, result.WaterPressureAsphaltCover);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, result.GrassCoverErosionOutwards);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, result.GrassCoverSlipOffOutwards);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, result.GrassCoverSlipOffInwards);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, result.HeightStructures);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, result.ClosingStructures);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, result.PipingStructure);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, result.StabilityPointStructures);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, result.StrengthStabilityLengthwiseConstruction);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, result.DuneErosion);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.None, result.TechnicalInnovation);
        }

        [Test]
        public void Constructor_WithConstructionProperties_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            int sectionNumber = random.Next();
            double sectionStart = random.NextDouble();
            double sectionEnd = random.NextDouble();
            var totalResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var pipingResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var grassCoverErosionInwardsResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var macroStabilityInwardsResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var macroStabilityOutwardsResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var microstabilityResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var stabilityStoneCoverResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var waveImpactAsphaltCoverResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var waterPressureAsphaltCoverResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var grassCoverErosionOutwardsResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var grassCoverSlipOffOutwardsResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var grassCoverSlipOffInwardsResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var heightStructuresResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var closingStructuresResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var pipingStructureResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var stabilityPointStructuresResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var strengthStabilityLengthwiseResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var duneErosionResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var technicalInnovationResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            // Call
            var result = new CombinedFailureMechanismSectionAssemblyResult(sectionNumber, sectionStart, sectionEnd, totalResult,
                                                                           new CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties
                                                                           {
                                                                               Piping = pipingResult,
                                                                               GrassCoverErosionInwards = grassCoverErosionInwardsResult,
                                                                               MacroStabilityInwards = macroStabilityInwardsResult,
                                                                               MacroStabilityOutwards = macroStabilityOutwardsResult,
                                                                               Microstability = microstabilityResult,
                                                                               StabilityStoneCover = stabilityStoneCoverResult,
                                                                               WaveImpactAsphaltCover = waveImpactAsphaltCoverResult,
                                                                               WaterPressureAsphaltCover = waterPressureAsphaltCoverResult,
                                                                               GrassCoverErosionOutwards = grassCoverErosionOutwardsResult,
                                                                               GrassCoverSlipOffOutwards = grassCoverSlipOffOutwardsResult,
                                                                               GrassCoverSlipOffInwards = grassCoverSlipOffInwardsResult,
                                                                               HeightStructures = heightStructuresResult,
                                                                               ClosingStructures = closingStructuresResult,
                                                                               PipingStructure = pipingStructureResult,
                                                                               StabilityPointStructures = stabilityPointStructuresResult,
                                                                               StrengthStabilityLengthwiseConstruction = strengthStabilityLengthwiseResult,
                                                                               DuneErosion = duneErosionResult,
                                                                               TechnicalInnovation = technicalInnovationResult
                                                                           });

            // Assert
            Assert.AreEqual(sectionNumber, result.SectionNumber);
            Assert.AreEqual(sectionStart, result.SectionStart);
            Assert.AreEqual(sectionEnd, result.SectionEnd);
            Assert.AreEqual(totalResult, result.TotalResult);
            Assert.AreEqual(pipingResult, result.Piping);
            Assert.AreEqual(grassCoverErosionInwardsResult, result.GrassCoverErosionInwards);
            Assert.AreEqual(macroStabilityInwardsResult, result.MacroStabilityInwards);
            Assert.AreEqual(macroStabilityOutwardsResult, result.MacroStabilityOutwards);
            Assert.AreEqual(microstabilityResult, result.Microstability);
            Assert.AreEqual(stabilityStoneCoverResult, result.StabilityStoneCover);
            Assert.AreEqual(waveImpactAsphaltCoverResult, result.WaveImpactAsphaltCover);
            Assert.AreEqual(waterPressureAsphaltCoverResult, result.WaterPressureAsphaltCover);
            Assert.AreEqual(grassCoverErosionOutwardsResult, result.GrassCoverErosionOutwards);
            Assert.AreEqual(grassCoverSlipOffOutwardsResult, result.GrassCoverSlipOffOutwards);
            Assert.AreEqual(grassCoverSlipOffInwardsResult, result.GrassCoverSlipOffInwards);
            Assert.AreEqual(heightStructuresResult, result.HeightStructures);
            Assert.AreEqual(closingStructuresResult, result.ClosingStructures);
            Assert.AreEqual(pipingStructureResult, result.PipingStructure);
            Assert.AreEqual(stabilityPointStructuresResult, result.StabilityPointStructures);
            Assert.AreEqual(strengthStabilityLengthwiseResult, result.StrengthStabilityLengthwiseConstruction);
            Assert.AreEqual(duneErosionResult, result.DuneErosion);
            Assert.AreEqual(technicalInnovationResult, result.TechnicalInnovation);
        }
    }
}