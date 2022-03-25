﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Drawing;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.Forms.Views;

namespace Riskeer.Integration.Forms.Test.Views
{
    [TestFixture]
    public class CombinedFailureMechanismSectionAssemblyResultRowTest
    {
        private const int totalResultIndex = 3;
        private const int pipingIndex = 4;
        private const int grassCoverErosionInwardsIndex = 5;
        private const int macroStabilityInwardsIndex = 6;
        private const int microstabililityIndex = 7;
        private const int stabilityStoneCoverIndex = 8;
        private const int waveImpactAsphaltCoverIndex = 9;
        private const int waterPressureAsphaltCoverIndex = 10;
        private const int grassCoverErosionOutwardsIndex = 11;
        private const int grassCoverSlipOffOutwardsIndex = 12;
        private const int grassCoverSlipOffInwardsIndex = 13;
        private const int heightStructuresIndex = 14;
        private const int closingStructuresIndex = 15;
        private const int pipingStructureIndex = 16;
        private const int stabilityPointStructuresIndex = 17;
        private const int duneErosionIndex = 18;
        private const int specificFailurePathStartIndex = 19;

        [Test]
        public void Constructor_CombinedFailureMechanismAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new CombinedFailureMechanismSectionAssemblyResultRow(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("combinedFailureMechanismSectionAssemblyResult", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_WithCombinedFailureMechanismAssemblyResult_ExpectedValues(bool failureMechanismsInAssembly)
        {
            // Setup
            CombinedFailureMechanismSectionAssemblyResult result = GetCombinedFailureMechanismSectionAssemblyResult(failureMechanismsInAssembly);

            // Call
            var row = new CombinedFailureMechanismSectionAssemblyResultRow(result);

            // Assert
            Assert.IsInstanceOf<IHasColumnStateDefinitions>(row);

            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(20, columnStateDefinitions.Count);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, totalResultIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, pipingIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, grassCoverErosionInwardsIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, macroStabilityInwardsIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, microstabililityIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, stabilityStoneCoverIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, waveImpactAsphaltCoverIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, waterPressureAsphaltCoverIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, grassCoverErosionOutwardsIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, grassCoverSlipOffOutwardsIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, grassCoverSlipOffInwardsIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, heightStructuresIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, closingStructuresIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, pipingStructureIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, stabilityPointStructuresIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, duneErosionIndex);
            for (var i = 1; i < result.SpecificFailureMechanisms.Length; i++)
            {
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, duneErosionIndex + i);
            }

            Assert.AreEqual(result.SectionNumber, row.SectionNumber);
            Assert.AreEqual(2, row.SectionStart.NumberOfDecimalPlaces);
            Assert.AreEqual(result.SectionStart, row.SectionStart, row.SectionStart.GetAccuracy());
            Assert.AreEqual(2, row.SectionStart.NumberOfDecimalPlaces);
            Assert.AreEqual(result.SectionEnd, row.SectionEnd, row.SectionEnd.GetAccuracy());
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(result.TotalResult), row.TotalResult);
            Assert.AreEqual(GetExpectedDisplayNameForFailureMechanism(result.Piping), row.Piping);
            Assert.AreEqual(GetExpectedDisplayNameForFailureMechanism(result.GrassCoverErosionInwards), row.GrassCoverErosionInwards);
            Assert.AreEqual(GetExpectedDisplayNameForFailureMechanism(result.MacroStabilityInwards), row.MacroStabilityInwards);
            Assert.AreEqual(GetExpectedDisplayNameForFailureMechanism(result.Microstability), row.Microstability);
            Assert.AreEqual(GetExpectedDisplayNameForFailureMechanism(result.StabilityStoneCover), row.StabilityStoneCover);
            Assert.AreEqual(GetExpectedDisplayNameForFailureMechanism(result.WaveImpactAsphaltCover), row.WaveImpactAsphaltCover);
            Assert.AreEqual(GetExpectedDisplayNameForFailureMechanism(result.WaterPressureAsphaltCover), row.WaterPressureAsphaltCover);
            Assert.AreEqual(GetExpectedDisplayNameForFailureMechanism(result.GrassCoverErosionOutwards), row.GrassCoverErosionOutwards);
            Assert.AreEqual(GetExpectedDisplayNameForFailureMechanism(result.GrassCoverSlipOffOutwards), row.GrassCoverSlipOffOutwards);
            Assert.AreEqual(GetExpectedDisplayNameForFailureMechanism(result.GrassCoverSlipOffInwards), row.GrassCoverSlipOffInwards);
            Assert.AreEqual(GetExpectedDisplayNameForFailureMechanism(result.HeightStructures), row.HeightStructures);
            Assert.AreEqual(GetExpectedDisplayNameForFailureMechanism(result.ClosingStructures), row.ClosingStructures);
            Assert.AreEqual(GetExpectedDisplayNameForFailureMechanism(result.PipingStructure), row.PipingStructure);
            Assert.AreEqual(GetExpectedDisplayNameForFailureMechanism(result.StabilityPointStructures), row.StabilityPointStructures);
            Assert.AreEqual(GetExpectedDisplayNameForFailureMechanism(result.DuneErosion), row.DuneErosion);
            for (var i = 0; i < result.SpecificFailureMechanisms.Length; i++)
            {
                FailureMechanismSectionAssemblyGroup? specificFailureMechanism = result.SpecificFailureMechanisms[i];
                Assert.AreEqual(GetExpectedDisplayNameForFailureMechanism(specificFailureMechanism), row.SpecificFailurePaths[i]);
            }
        }

        [Test]
        [TestCaseSource(typeof(FailureMechanismSectionAssemblyGroupColorTestHelper), nameof(FailureMechanismSectionAssemblyGroupColorTestHelper.FailureMechanismSectionAssemblyGroupColorCases))]
        public void Constructor_WithCombinedFailureMechanismAssemblyResultContainingOnlyFailureMechanismsInAssembly_ExpectedColumnStates(
            FailureMechanismSectionAssemblyGroup assemblyGroup,
            Color expectedBackgroundColor)
        {
            // Setup
            var random = new Random(21);

            var result = new CombinedFailureMechanismSectionAssemblyResult(
                random.Next(),
                random.NextDouble(),
                random.NextDouble(),
                assemblyGroup,
                new CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties
                {
                    Piping = assemblyGroup,
                    GrassCoverErosionInwards = assemblyGroup,
                    MacroStabilityInwards = assemblyGroup,
                    Microstability = assemblyGroup,
                    StabilityStoneCover = assemblyGroup,
                    WaveImpactAsphaltCover = assemblyGroup,
                    WaterPressureAsphaltCover = assemblyGroup,
                    GrassCoverErosionOutwards = assemblyGroup,
                    GrassCoverSlipOffOutwards = assemblyGroup,
                    GrassCoverSlipOffInwards = assemblyGroup,
                    HeightStructures = assemblyGroup,
                    ClosingStructures = assemblyGroup,
                    PipingStructure = assemblyGroup,
                    StabilityPointStructures = assemblyGroup,
                    DuneErosion = assemblyGroup,
                    SpecificFailureMechanisms = new FailureMechanismSectionAssemblyGroup?[]
                    {
                        assemblyGroup,
                        assemblyGroup
                    }
                });

            // Call
            var row = new CombinedFailureMechanismSectionAssemblyResultRow(result);

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[totalResultIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[pipingIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[grassCoverErosionInwardsIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[macroStabilityInwardsIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[microstabililityIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[stabilityStoneCoverIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[waveImpactAsphaltCoverIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[waterPressureAsphaltCoverIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[grassCoverErosionOutwardsIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[grassCoverSlipOffOutwardsIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[grassCoverSlipOffInwardsIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[heightStructuresIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[closingStructuresIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[pipingStructureIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[stabilityPointStructuresIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[duneErosionIndex], expectedBackgroundColor);

            for (var i = 0; i < result.SpecificFailureMechanisms.Length; i++)
            {
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[specificFailurePathStartIndex + i], expectedBackgroundColor);
            }
        }

        [Test]
        public void Constructor_WithCombinedFailureMechanismAssemblyResultContainingOnlyFailureMechanismsNotInAssembly_ExpectedColumnStates()
        {
            // Setup
            var random = new Random(21);

            var result = new CombinedFailureMechanismSectionAssemblyResult(
                random.Next(),
                random.NextDouble(),
                random.NextDouble(),
                FailureMechanismSectionAssemblyGroup.Gr,
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
                    SpecificFailureMechanisms = new FailureMechanismSectionAssemblyGroup?[]
                    {
                        null,
                        null
                    }
                });

            // Call
            var row = new CombinedFailureMechanismSectionAssemblyResultRow(result);

            // Assert
            Color expectedBackgroundColor = Color.FromArgb(255, 255, 255);
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[totalResultIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[pipingIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[grassCoverErosionInwardsIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[macroStabilityInwardsIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[microstabililityIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[stabilityStoneCoverIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[waveImpactAsphaltCoverIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[waterPressureAsphaltCoverIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[grassCoverErosionOutwardsIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[grassCoverSlipOffOutwardsIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[grassCoverSlipOffInwardsIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[heightStructuresIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[closingStructuresIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[pipingStructureIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[stabilityPointStructuresIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[duneErosionIndex], expectedBackgroundColor);

            for (var i = 0; i < result.SpecificFailureMechanisms.Length; i++)
            {
                DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[specificFailurePathStartIndex + i], expectedBackgroundColor);
            }
        }

        private static CombinedFailureMechanismSectionAssemblyResult GetCombinedFailureMechanismSectionAssemblyResult(bool failureMechanismsInAssembly)
        {
            var random = new Random(21);
            return new CombinedFailureMechanismSectionAssemblyResult(
                random.Next(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                new CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties
                {
                    Piping = GetFailureMechanismSectionAssemblyGroup(failureMechanismsInAssembly, random),
                    GrassCoverErosionInwards = GetFailureMechanismSectionAssemblyGroup(failureMechanismsInAssembly, random),
                    MacroStabilityInwards = GetFailureMechanismSectionAssemblyGroup(failureMechanismsInAssembly, random),
                    Microstability = GetFailureMechanismSectionAssemblyGroup(failureMechanismsInAssembly, random),
                    StabilityStoneCover = GetFailureMechanismSectionAssemblyGroup(failureMechanismsInAssembly, random),
                    WaveImpactAsphaltCover = GetFailureMechanismSectionAssemblyGroup(failureMechanismsInAssembly, random),
                    WaterPressureAsphaltCover = GetFailureMechanismSectionAssemblyGroup(failureMechanismsInAssembly, random),
                    GrassCoverErosionOutwards = GetFailureMechanismSectionAssemblyGroup(failureMechanismsInAssembly, random),
                    GrassCoverSlipOffOutwards = GetFailureMechanismSectionAssemblyGroup(failureMechanismsInAssembly, random),
                    GrassCoverSlipOffInwards = GetFailureMechanismSectionAssemblyGroup(failureMechanismsInAssembly, random),
                    HeightStructures = GetFailureMechanismSectionAssemblyGroup(failureMechanismsInAssembly, random),
                    ClosingStructures = GetFailureMechanismSectionAssemblyGroup(failureMechanismsInAssembly, random),
                    PipingStructure = GetFailureMechanismSectionAssemblyGroup(failureMechanismsInAssembly, random),
                    StabilityPointStructures = GetFailureMechanismSectionAssemblyGroup(failureMechanismsInAssembly, random),
                    DuneErosion = GetFailureMechanismSectionAssemblyGroup(failureMechanismsInAssembly, random),
                    SpecificFailureMechanisms = new[]
                    {
                        GetFailureMechanismSectionAssemblyGroup(failureMechanismsInAssembly, random),
                        GetFailureMechanismSectionAssemblyGroup(failureMechanismsInAssembly, random),
                        GetFailureMechanismSectionAssemblyGroup(failureMechanismsInAssembly, random)
                    }
                });
        }

        private static FailureMechanismSectionAssemblyGroup? GetFailureMechanismSectionAssemblyGroup(bool failureMechanismInAssembly, Random random)
        {
            return failureMechanismInAssembly
                       ? random.NextEnumValue<FailureMechanismSectionAssemblyGroup>()
                       : (FailureMechanismSectionAssemblyGroup?) null;
        }

        private static string GetExpectedDisplayNameForFailureMechanism(FailureMechanismSectionAssemblyGroup? failureMechanismSectionAssemblyGroup)
        {
            return failureMechanismSectionAssemblyGroup.HasValue
                       ? EnumDisplayNameHelper.GetDisplayName(failureMechanismSectionAssemblyGroup.Value)
                       : "-";
        }
    }
}