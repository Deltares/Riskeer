﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
            void Call() => new CombinedFailureMechanismSectionAssemblyResult(random.NextDouble(), random.NextDouble(),
                                                                             random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                                                                             random.NextEnumValue<AssemblyMethod>(), random.NextEnumValue<AssemblyMethod>(),
                                                                             random.NextEnumValue<AssemblyMethod>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("properties", exception.ParamName);
        }

        [Test]
        public void Constructor_WithEmptyConstructionProperties_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double sectionStart = random.NextDouble();
            double sectionEnd = random.NextDouble();
            var totalResult = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var commonSectionAssemblyMethod = random.NextEnumValue<AssemblyMethod>();
            var failureMechanismResultsAssemblyMethod = random.NextEnumValue<AssemblyMethod>();
            var combinedSectionResultAssemblyMethod = random.NextEnumValue<AssemblyMethod>();

            // Call
            var result = new CombinedFailureMechanismSectionAssemblyResult(sectionStart, sectionEnd, totalResult, commonSectionAssemblyMethod,
                                                                           failureMechanismResultsAssemblyMethod, combinedSectionResultAssemblyMethod,
                                                                           new CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties());

            // Assert
            Assert.AreEqual(sectionStart, result.SectionStart);
            Assert.AreEqual(sectionEnd, result.SectionEnd);
            Assert.AreEqual(totalResult, result.TotalResult);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, result.Piping);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, result.GrassCoverErosionInwards);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, result.MacroStabilityInwards);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, result.Microstability);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, result.StabilityStoneCover);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, result.WaveImpactAsphaltCover);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, result.WaterPressureAsphaltCover);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, result.GrassCoverErosionOutwards);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, result.GrassCoverSlipOffOutwards);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, result.GrassCoverSlipOffInwards);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, result.HeightStructures);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, result.ClosingStructures);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, result.PipingStructure);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, result.StabilityPointStructures);
            Assert.AreEqual(FailureMechanismSectionAssemblyGroup.Gr, result.DuneErosion);
            Assert.IsEmpty(result.SpecificFailureMechanisms);

            Assert.AreEqual(result.CommonSectionAssemblyMethod, commonSectionAssemblyMethod);
            Assert.AreEqual(result.FailureMechanismResultsAssemblyMethod, failureMechanismResultsAssemblyMethod);
            Assert.AreEqual(result.CombinedSectionResultAssemblyMethod, combinedSectionResultAssemblyMethod);
        }

        [Test]
        public void Constructor_WithConstructionPropertiesRepresentingAllFailureMechanismsInAssembly_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double sectionStart = random.NextDouble();
            double sectionEnd = random.NextDouble();
            var totalResult = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var pipingResult = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var grassCoverErosionInwardsResult = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var macroStabilityInwardsResult = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var microstabilityResult = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var stabilityStoneCoverResult = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var waveImpactAsphaltCoverResult = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var waterPressureAsphaltCoverResult = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var grassCoverErosionOutwardsResult = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var grassCoverSlipOffOutwardsResult = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var grassCoverSlipOffInwardsResult = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var heightStructuresResult = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var closingStructuresResult = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var pipingStructureResult = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var stabilityPointStructuresResult = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var duneErosionResult = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var specificFailureMechanismResults = new FailureMechanismSectionAssemblyGroup?[]
            {
                random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                random.NextEnumValue<FailureMechanismSectionAssemblyGroup>()
            };
            var commonSectionAssemblyMethod = random.NextEnumValue<AssemblyMethod>();
            var failureMechanismResultsAssemblyMethod = random.NextEnumValue<AssemblyMethod>();
            var combinedSectionResultAssemblyMethod = random.NextEnumValue<AssemblyMethod>();

            // Call
            var result = new CombinedFailureMechanismSectionAssemblyResult(sectionStart, sectionEnd, totalResult, commonSectionAssemblyMethod,
                                                                           failureMechanismResultsAssemblyMethod, combinedSectionResultAssemblyMethod,
                                                                           new CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties
                                                                           {
                                                                               Piping = pipingResult,
                                                                               GrassCoverErosionInwards = grassCoverErosionInwardsResult,
                                                                               MacroStabilityInwards = macroStabilityInwardsResult,
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
                                                                               DuneErosion = duneErosionResult,
                                                                               SpecificFailureMechanisms = specificFailureMechanismResults
                                                                           });

            // Assert
            Assert.AreEqual(sectionStart, result.SectionStart);
            Assert.AreEqual(sectionEnd, result.SectionEnd);
            Assert.AreEqual(totalResult, result.TotalResult);
            Assert.AreEqual(pipingResult, result.Piping);
            Assert.AreEqual(grassCoverErosionInwardsResult, result.GrassCoverErosionInwards);
            Assert.AreEqual(macroStabilityInwardsResult, result.MacroStabilityInwards);
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
            Assert.AreEqual(duneErosionResult, result.DuneErosion);
            CollectionAssert.AreEqual(specificFailureMechanismResults, result.SpecificFailureMechanisms);
            Assert.AreEqual(result.CommonSectionAssemblyMethod, commonSectionAssemblyMethod);
            Assert.AreEqual(result.FailureMechanismResultsAssemblyMethod, failureMechanismResultsAssemblyMethod);
            Assert.AreEqual(result.CombinedSectionResultAssemblyMethod, combinedSectionResultAssemblyMethod);
        }

        [Test]
        public void Constructor_WithConstructionPropertiesRepresentingAllFailureMechanismsNotInAssembly_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double sectionStart = random.NextDouble();
            double sectionEnd = random.NextDouble();
            var totalResult = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var specificFailureMechanismResults = new FailureMechanismSectionAssemblyGroup?[]
            {
                null,
                null,
                null
            };
            var commonSectionAssemblyMethod = random.NextEnumValue<AssemblyMethod>();
            var failureMechanismResultsAssemblyMethod = random.NextEnumValue<AssemblyMethod>();
            var combinedSectionResultAssemblyMethod = random.NextEnumValue<AssemblyMethod>();

            // Call
            var result = new CombinedFailureMechanismSectionAssemblyResult(sectionStart, sectionEnd, totalResult, commonSectionAssemblyMethod,
                                                                           failureMechanismResultsAssemblyMethod, combinedSectionResultAssemblyMethod,
                                                                           new CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties
                                                                           {
                                                                               Piping = null,
                                                                               GrassCoverErosionInwards = null,
                                                                               MacroStabilityInwards = null,
                                                                               Microstability = null,
                                                                               StabilityStoneCover = null,
                                                                               WaveImpactAsphaltCover = null,
                                                                               WaterPressureAsphaltCover = null,
                                                                               GrassCoverErosionOutwards = null,
                                                                               GrassCoverSlipOffOutwards = null,
                                                                               GrassCoverSlipOffInwards = null,
                                                                               HeightStructures = null,
                                                                               ClosingStructures = null,
                                                                               PipingStructure = null,
                                                                               StabilityPointStructures = null,
                                                                               DuneErosion = null,
                                                                               SpecificFailureMechanisms = specificFailureMechanismResults
                                                                           });

            // Assert
            Assert.AreEqual(sectionStart, result.SectionStart);
            Assert.AreEqual(sectionEnd, result.SectionEnd);
            Assert.AreEqual(totalResult, result.TotalResult);
            Assert.IsNull(result.Piping);
            Assert.IsNull(result.GrassCoverErosionInwards);
            Assert.IsNull(result.MacroStabilityInwards);
            Assert.IsNull(result.Microstability);
            Assert.IsNull(result.StabilityStoneCover);
            Assert.IsNull(result.WaveImpactAsphaltCover);
            Assert.IsNull(result.WaterPressureAsphaltCover);
            Assert.IsNull(result.GrassCoverErosionOutwards);
            Assert.IsNull(result.GrassCoverSlipOffOutwards);
            Assert.IsNull(result.GrassCoverSlipOffInwards);
            Assert.IsNull(result.HeightStructures);
            Assert.IsNull(result.ClosingStructures);
            Assert.IsNull(result.PipingStructure);
            Assert.IsNull(result.StabilityPointStructures);
            Assert.IsNull(result.DuneErosion);
            Assert.AreEqual(specificFailureMechanismResults.Length, result.SpecificFailureMechanisms.Length);
            foreach (FailureMechanismSectionAssemblyGroup? specificFailureMechanismResult in result.SpecificFailureMechanisms)
            {
                Assert.IsNull(specificFailureMechanismResult);
            }

            Assert.AreEqual(result.CommonSectionAssemblyMethod, commonSectionAssemblyMethod);
            Assert.AreEqual(result.FailureMechanismResultsAssemblyMethod, failureMechanismResultsAssemblyMethod);
            Assert.AreEqual(result.CombinedSectionResultAssemblyMethod, combinedSectionResultAssemblyMethod);
        }
    }
}