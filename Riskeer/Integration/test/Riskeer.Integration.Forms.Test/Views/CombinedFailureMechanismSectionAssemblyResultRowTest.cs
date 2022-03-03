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
        public void Constructor_WithCombinedFailureMechanismAssemblyResult_ExpectedValues()
        {
            // Setup
            CombinedFailureMechanismSectionAssemblyResult result = GetCombinedFailureMechanismSectionAssemblyResult();

            // Call
            var row = new CombinedFailureMechanismSectionAssemblyResultRow(result);

            // Assert
            Assert.IsInstanceOf<IHasColumnStateDefinitions>(row);

            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(16, columnStateDefinitions.Count);
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

            Assert.AreEqual(result.SectionNumber, row.SectionNumber);
            Assert.AreEqual(2, row.SectionStart.NumberOfDecimalPlaces);
            Assert.AreEqual(result.SectionStart, row.SectionStart, row.SectionStart.GetAccuracy());
            Assert.AreEqual(2, row.SectionStart.NumberOfDecimalPlaces);
            Assert.AreEqual(result.SectionEnd, row.SectionEnd, row.SectionEnd.GetAccuracy());
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(result.TotalResult), row.TotalResult);
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(result.Piping), row.Piping);
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(result.GrassCoverErosionInwards), row.GrassCoverErosionInwards);
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(result.MacroStabilityInwards), row.MacroStabilityInwards);
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(result.Microstability), row.Microstability);
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(result.StabilityStoneCover), row.StabilityStoneCover);
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(result.WaveImpactAsphaltCover), row.WaveImpactAsphaltCover);
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(result.WaterPressureAsphaltCover), row.WaterPressureAsphaltCover);
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(result.GrassCoverErosionOutwards), row.GrassCoverErosionOutwards);
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(result.GrassCoverSlipOffOutwards), row.GrassCoverSlipOffOutwards);
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(result.GrassCoverSlipOffInwards), row.GrassCoverSlipOffInwards);
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(result.HeightStructures), row.HeightStructures);
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(result.ClosingStructures), row.ClosingStructures);
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(result.PipingStructure), row.PipingStructure);
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(result.StabilityPointStructures), row.StabilityPointStructures);
            Assert.AreEqual(EnumDisplayNameHelper.GetDisplayName(result.DuneErosion), row.DuneErosion);
        }

        [Test]
        [TestCaseSource(typeof(AssemblyGroupColorTestHelper), nameof(AssemblyGroupColorTestHelper.FailureMechanismSectionAssemblyGroupColorCases))]
        public void Constructor_WithCombinedFailureMechanismAssemblyResult_ExpectedColumnStates(
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
                    DuneErosion = assemblyGroup
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
        }

        private static CombinedFailureMechanismSectionAssemblyResult GetCombinedFailureMechanismSectionAssemblyResult()
        {
            var random = new Random(21);
            return new CombinedFailureMechanismSectionAssemblyResult(
                random.Next(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                new CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties
                {
                    Piping = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                    GrassCoverErosionInwards = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                    MacroStabilityInwards = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                    Microstability = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                    StabilityStoneCover = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                    WaveImpactAsphaltCover = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                    WaterPressureAsphaltCover = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                    GrassCoverErosionOutwards = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                    GrassCoverSlipOffOutwards = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                    GrassCoverSlipOffInwards = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                    HeightStructures = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                    ClosingStructures = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                    PipingStructure = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                    StabilityPointStructures = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                    DuneErosion = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>()
                });
        }
    }
}