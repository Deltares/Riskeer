﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Drawing;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Integration.Data.Assembly;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class CombinedFailureMechanismSectionAssemblyResultRowTest
    {
        private const int totalResultIndex = 2;
        private const int pipingIndex = 3;
        private const int grassCoverErosionInwardsIndex = 4;
        private const int macroStabilityInwardsIndex = 5;
        private const int macroStabilityOutwardsIndex = 6;
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
        private const int strengthStabilityLengthwiseIndex = 18;
        private const int duneErosionIndex = 19;
        private const int technicalInnovationIndex = 20;

        [Test]
        public void Constructor_CombinedFailureMechanismAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new CombinedFailureMechanismSectionAssemblyResultRow(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
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
            Assert.AreEqual(19, columnStateDefinitions.Count);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, totalResultIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, pipingIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, grassCoverErosionInwardsIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, macroStabilityInwardsIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, macroStabilityOutwardsIndex);
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
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, strengthStabilityLengthwiseIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, duneErosionIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, technicalInnovationIndex);

            Assert.AreEqual(3, row.SectionStart.NumberOfDecimalPlaces);
            Assert.AreEqual(result.SectionStart, row.SectionStart, row.SectionStart.GetAccuracy());
            Assert.AreEqual(3, row.SectionStart.NumberOfDecimalPlaces);
            Assert.AreEqual(result.SectionEnd, row.SectionEnd, row.SectionEnd.GetAccuracy());
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(result.TotalResult), row.TotalResult);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(result.Piping), row.Piping);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(result.GrassCoverErosionInwards), row.GrassCoverErosionInwards);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(result.MacroStabilityInwards), row.MacroStabilityInwards);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(result.MacroStabilityOutwards), row.MacroStabilityOutwards);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(result.Microstability), row.Microstability);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(result.StabilityStoneCover), row.StabilityStoneCover);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(result.WaveImpactAsphaltCover), row.WaveImpactAsphaltCover);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(result.WaterPressureAsphaltCover), row.WaterPressureAsphaltCover);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(result.GrassCoverErosionOutwards), row.GrassCoverErosionOutwards);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(result.GrassCoverSlipOffOutwards), row.GrassCoverSlipOffOutwards);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(result.GrassCoverSlipOffInwards), row.GrassCoverSlipOffInwards);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(result.HeightStructures), row.HeightStructures);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(result.ClosingStructures), row.ClosingStructures);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(result.PipingStructure), row.PipingStructure);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(result.StabilityPointStructures), row.StabilityPointStructures);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(result.StrengthStabilityLengthwise), row.StrengthStabilityLengthwise);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(result.DuneErosion), row.DuneErosion);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroupHelper.GetCategoryGroupDisplayname(result.TechnicalInnovation), row.TechnicalInnovation);
        }

        [Test]
        [TestCaseSource(typeof(AssemblyCategoryColorTestHelper), nameof(AssemblyCategoryColorTestHelper.FailureMechanismSectionAssemblyCategoryGroupColorCases))]
        public void Constructor_WithCombinedFailureMechanismAssemblyResult_ExpectedStateDefinitionPropertiesSet(
            FailureMechanismSectionAssemblyCategoryGroup categoryGroup,
            Color expectedBackgroundColor)
        {
            // Setup
            var random = new Random(21);

            var result = new CombinedFailureMechanismSectionAssemblyResult(
                random.NextDouble(),
                random.NextDouble(),
                categoryGroup,
                new CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties
                {
                    Piping = categoryGroup,
                    GrassCoverErosionInwards = categoryGroup,
                    MacroStabilityInwards = categoryGroup,
                    MacroStabilityOutwards = categoryGroup,
                    Microstability = categoryGroup,
                    StabilityStoneCover = categoryGroup,
                    WaveImpactAsphaltCover = categoryGroup,
                    WaterPressureAsphaltCover = categoryGroup,
                    GrassCoverErosionOutwards = categoryGroup,
                    GrassCoverSlipOffOutwards = categoryGroup,
                    GrassCoverSlipOffInwards = categoryGroup,
                    HeightStructures = categoryGroup,
                    ClosingStructures = categoryGroup,
                    PipingStructure = categoryGroup,
                    StabilityPointStructures = categoryGroup,
                    StrengthStabilityLengthwise = categoryGroup,
                    DuneErosion = categoryGroup,
                    TechnicalInnovation = categoryGroup
                });

            // Call
            var row = new CombinedFailureMechanismSectionAssemblyResultRow(result);

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[totalResultIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[pipingIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[grassCoverErosionInwardsIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[macroStabilityInwardsIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[macroStabilityOutwardsIndex], expectedBackgroundColor);
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
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[strengthStabilityLengthwiseIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[duneErosionIndex], expectedBackgroundColor);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnWithColorState(columnStateDefinitions[technicalInnovationIndex], expectedBackgroundColor);
        }

        private static CombinedFailureMechanismSectionAssemblyResult GetCombinedFailureMechanismSectionAssemblyResult()
        {
            var random = new Random(21);
            return new CombinedFailureMechanismSectionAssemblyResult(
                random.NextDouble(),
                random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                new CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties
                {
                    Piping = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    GrassCoverErosionInwards = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    MacroStabilityInwards = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    MacroStabilityOutwards = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    Microstability = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    StabilityStoneCover = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    WaveImpactAsphaltCover = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    WaterPressureAsphaltCover = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    GrassCoverErosionOutwards = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    GrassCoverSlipOffOutwards = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    GrassCoverSlipOffInwards = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    HeightStructures = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    ClosingStructures = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    PipingStructure = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    StabilityPointStructures = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    StrengthStabilityLengthwise = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    DuneErosion = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    TechnicalInnovation = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()
                });
        }
    }
}