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
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.Data.Assembly;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class CombinedFailureMechanismSectionAssemblyResultRowTest
    {
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
        public void Class_Always_ExpectedTypeConverters()
        {
            // Assert
            TestHelper.AssertTypeConverter<CombinedFailureMechanismSectionAssemblyResultRow, EnumTypeConverter>(
                nameof(CombinedFailureMechanismSectionAssemblyResultRow.TotalResult));

            TestHelper.AssertTypeConverter<CombinedFailureMechanismSectionAssemblyResultRow, EnumTypeConverter>(
                nameof(CombinedFailureMechanismSectionAssemblyResultRow.Piping));

            TestHelper.AssertTypeConverter<CombinedFailureMechanismSectionAssemblyResultRow, EnumTypeConverter>(
                nameof(CombinedFailureMechanismSectionAssemblyResultRow.GrassCoverErosionInwards));

            TestHelper.AssertTypeConverter<CombinedFailureMechanismSectionAssemblyResultRow, EnumTypeConverter>(
                nameof(CombinedFailureMechanismSectionAssemblyResultRow.MacroStabilityInwards));

            TestHelper.AssertTypeConverter<CombinedFailureMechanismSectionAssemblyResultRow, EnumTypeConverter>(
                nameof(CombinedFailureMechanismSectionAssemblyResultRow.MacroStabilityOutwards));

            TestHelper.AssertTypeConverter<CombinedFailureMechanismSectionAssemblyResultRow, EnumTypeConverter>(
                nameof(CombinedFailureMechanismSectionAssemblyResultRow.Microstability));

            TestHelper.AssertTypeConverter<CombinedFailureMechanismSectionAssemblyResultRow, EnumTypeConverter>(
                nameof(CombinedFailureMechanismSectionAssemblyResultRow.StabilityStoneCover));

            TestHelper.AssertTypeConverter<CombinedFailureMechanismSectionAssemblyResultRow, EnumTypeConverter>(
                nameof(CombinedFailureMechanismSectionAssemblyResultRow.WaveImpactAsphaltCover));

            TestHelper.AssertTypeConverter<CombinedFailureMechanismSectionAssemblyResultRow, EnumTypeConverter>(
                nameof(CombinedFailureMechanismSectionAssemblyResultRow.WaterPressureAsphaltCover));

            TestHelper.AssertTypeConverter<CombinedFailureMechanismSectionAssemblyResultRow, EnumTypeConverter>(
                nameof(CombinedFailureMechanismSectionAssemblyResultRow.GrassCoverErosionOutwards));

            TestHelper.AssertTypeConverter<CombinedFailureMechanismSectionAssemblyResultRow, EnumTypeConverter>(
                nameof(CombinedFailureMechanismSectionAssemblyResultRow.GrassCoverSlipOffOutwards));

            TestHelper.AssertTypeConverter<CombinedFailureMechanismSectionAssemblyResultRow, EnumTypeConverter>(
                nameof(CombinedFailureMechanismSectionAssemblyResultRow.GrassCoverSlipOffInwards));

            TestHelper.AssertTypeConverter<CombinedFailureMechanismSectionAssemblyResultRow, EnumTypeConverter>(
                nameof(CombinedFailureMechanismSectionAssemblyResultRow.HeightStructures));

            TestHelper.AssertTypeConverter<CombinedFailureMechanismSectionAssemblyResultRow, EnumTypeConverter>(
                nameof(CombinedFailureMechanismSectionAssemblyResultRow.ClosingStructures));

            TestHelper.AssertTypeConverter<CombinedFailureMechanismSectionAssemblyResultRow, EnumTypeConverter>(
                nameof(CombinedFailureMechanismSectionAssemblyResultRow.PipingStructures));

            TestHelper.AssertTypeConverter<CombinedFailureMechanismSectionAssemblyResultRow, EnumTypeConverter>(
                nameof(CombinedFailureMechanismSectionAssemblyResultRow.StabilityPointStructures));

            TestHelper.AssertTypeConverter<CombinedFailureMechanismSectionAssemblyResultRow, EnumTypeConverter>(
                nameof(CombinedFailureMechanismSectionAssemblyResultRow.StrengthStabilityLengthwise));

            TestHelper.AssertTypeConverter<CombinedFailureMechanismSectionAssemblyResultRow, EnumTypeConverter>(
                nameof(CombinedFailureMechanismSectionAssemblyResultRow.DuneErosion));

            TestHelper.AssertTypeConverter<CombinedFailureMechanismSectionAssemblyResultRow, EnumTypeConverter>(
                nameof(CombinedFailureMechanismSectionAssemblyResultRow.TechnicalInnovation));
        }

        [Test]
        public void Constructor_WithCombinedFailureMechanismAssemblyResult_ExpectedValues()
        {
            // Setup
            CombinedFailureMechanismSectionAssemblyResult result = GetCombinedFailureMechanismSectionAssemblyResult();

            // Call
            var row = new CombinedFailureMechanismSectionAssemblyResultRow(result);

            // Assert
            Assert.AreEqual(result.SectionStart, row.SectionStart);
            Assert.AreEqual(result.SectionEnd, row.SectionEnd);
            Assert.AreEqual(result.TotalResult, row.TotalResult);
            Assert.AreEqual(result.Piping, row.Piping);
            Assert.AreEqual(result.GrassCoverErosionInwards, row.GrassCoverErosionInwards);
            Assert.AreEqual(result.MacroStabilityInwards, row.MacroStabilityInwards);
            Assert.AreEqual(result.MacroStabilityOutwards, row.MacroStabilityOutwards);
            Assert.AreEqual(result.Microstability, row.Microstability);
            Assert.AreEqual(result.StabilityStoneCover, row.StabilityStoneCover);
            Assert.AreEqual(result.WaveImpactAsphaltCover, row.WaveImpactAsphaltCover);
            Assert.AreEqual(result.WaterPressureAsphaltCover, row.WaterPressureAsphaltCover);
            Assert.AreEqual(result.GrassCoverErosionOutwards, row.GrassCoverErosionOutwards);
            Assert.AreEqual(result.GrassCoverSlipOffOutwards, row.GrassCoverSlipOffOutwards);
            Assert.AreEqual(result.GrassCoverSlipOffInwards, row.GrassCoverSlipOffInwards);
            Assert.AreEqual(result.HeightStructures, row.HeightStructures);
            Assert.AreEqual(result.ClosingStructures, row.ClosingStructures);
            Assert.AreEqual(result.PipingStructures, row.PipingStructures);
            Assert.AreEqual(result.StabilityPointStructures, row.StabilityPointStructures);
            Assert.AreEqual(result.StrengthStabilityLengthwise, row.StrengthStabilityLengthwise);
            Assert.AreEqual(result.DuneErosion, row.DuneErosion);
            Assert.AreEqual(result.TechnicalInnovation, row.TechnicalInnovation);
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
                    PipingStructures = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    StabilityPointStructures = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    StrengthStabilityLengthwise = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    DuneErosion = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    TechnicalInnovation = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()
                });
        }
    }
}