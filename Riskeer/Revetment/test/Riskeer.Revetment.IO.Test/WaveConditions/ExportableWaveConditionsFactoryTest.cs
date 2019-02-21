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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Data.TestUtil;
using Riskeer.Revetment.IO.WaveConditions;

namespace Riskeer.Revetment.IO.Test.WaveConditions
{
    [TestFixture]
    public class ExportableWaveConditionsFactoryTest
    {
        private static readonly TestWaveConditionsOutput waveConditionsOutput = new TestWaveConditionsOutput();

        private readonly WaveConditionsOutput[] waveConditionsOutputCollection =
        {
            waveConditionsOutput
        };

        [Test]
        public void CreateExportableWaveConditionsCollectionWithFailureMechanismCategoryWaveConditionsInput_NameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection(null,
                                                                                                               new FailureMechanismCategoryWaveConditionsInput(),
                                                                                                               waveConditionsOutputCollection, CoverType.GrassWaveRunUp);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollectionWithFailureMechanismCategoryWaveConditionsInput_WaveConditionsInputNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName",
                                                                                                               (FailureMechanismCategoryWaveConditionsInput) null,
                                                                                                               waveConditionsOutputCollection,
                                                                                                               CoverType.GrassWaveRunUp);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveConditionsInput", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollectionWithFailureMechanismCategoryWaveConditionsInput_OutputNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName",
                                                                                                               new FailureMechanismCategoryWaveConditionsInput(),
                                                                                                               null,
                                                                                                               CoverType.GrassWaveRunUp);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollectionWithFailureMechanismCategoryWaveConditionsInput_CoverTypeNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName",
                                                                                                               new FailureMechanismCategoryWaveConditionsInput(),
                                                                                                               waveConditionsOutputCollection,
                                                                                                               null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("coverType", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(GetCoverTypes))]
        public void CreateExportableWaveConditionsCollectionWithFailureMechanismCategoryWaveConditionsInput_ValidDataWithCoverType_ReturnsValidCollection(CoverType coverType)
        {
            // Setup
            var waveConditionsInput = new FailureMechanismCategoryWaveConditionsInput
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "hblName", 1.0, 8.0),
                ForeshoreProfile = new TestForeshoreProfile(),
                UseForeshore = true,
                CategoryType = FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm
            };

            // Call
            ExportableWaveConditions[] exportableWaveConditionsCollection =
                ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("ewcName",
                                                                                         waveConditionsInput,
                                                                                         waveConditionsOutputCollection,
                                                                                         coverType).ToArray();

            // Assert
            Assert.AreEqual(1, exportableWaveConditionsCollection.Length);
            ExportableWaveConditions exportableWaveConditions = exportableWaveConditionsCollection[0];
            Assert.AreEqual("ewcName", exportableWaveConditions.CalculationName);
            Assert.AreEqual("hblName", exportableWaveConditions.LocationName);
            Assert.AreEqual(1.0, exportableWaveConditions.LocationXCoordinate);
            Assert.AreEqual(8.0, exportableWaveConditions.LocationYCoordinate);
            Assert.AreEqual("id", exportableWaveConditions.ForeshoreId);
            Assert.AreEqual(false, exportableWaveConditions.UseBreakWater);
            Assert.AreEqual(true, exportableWaveConditions.UseForeshore);
            Assert.AreEqual(coverType, exportableWaveConditions.CoverType);
            Assert.AreEqual("Iv", exportableWaveConditions.CategoryBoundaryName);
            Assert.AreEqual(2, exportableWaveConditions.WaterLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(2, exportableWaveConditions.WaveHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(2, exportableWaveConditions.WavePeriod.NumberOfDecimalPlaces);
            Assert.AreEqual(2, exportableWaveConditions.WaveAngle.NumberOfDecimalPlaces);
            Assert.AreEqual(waveConditionsOutput.WaterLevel, exportableWaveConditions.WaterLevel);
            Assert.AreEqual(waveConditionsOutput.WaveHeight, exportableWaveConditions.WaveHeight);
            Assert.AreEqual(waveConditionsOutput.WavePeakPeriod, exportableWaveConditions.WavePeriod);
            Assert.AreEqual(waveConditionsOutput.WaveAngle, exportableWaveConditions.WaveAngle);
            Assert.AreEqual(waveConditionsOutput.WaveDirection, exportableWaveConditions.WaveDirection);
        }

        [Test]
        public void CreateExportableWaveConditionsCollectionWithAssessmentSectionCategoryWaveConditionsInput_NameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection(null,
                                                                                                               new AssessmentSectionCategoryWaveConditionsInput(),
                                                                                                               waveConditionsOutputCollection,
                                                                                                               CoverType.Asphalt);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollectionWithAssessmentSectionCategoryWaveConditionsInput_WaveConditionsInputNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName",
                                                                                                               (AssessmentSectionCategoryWaveConditionsInput) null,
                                                                                                               waveConditionsOutputCollection,
                                                                                                               CoverType.Asphalt);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveConditionsInput", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollectionWithAssessmentSectionCategoryWaveConditionsInput_OutputNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName",
                                                                                                               new AssessmentSectionCategoryWaveConditionsInput(),
                                                                                                               null,
                                                                                                               CoverType.Asphalt);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollectionWithAssessmentSectionCategoryWaveConditionsInput_CoverTypeNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName",
                                                                                                               new AssessmentSectionCategoryWaveConditionsInput(),
                                                                                                               waveConditionsOutputCollection,
                                                                                                               null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("coverType", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(GetCoverTypes))]
        public void CreateExportableWaveConditionsCollectionWithAssessmentSectionCategoryWaveConditionsInput_ValidDataWithCoverType_ReturnsValidCollection_ValidDataWithCoverType_ReturnsValidCollection(CoverType coverType)
        {
            // Setup
            var waveConditionsInput = new AssessmentSectionCategoryWaveConditionsInput
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "hblName", 1.0, 8.0),
                ForeshoreProfile = new TestForeshoreProfile(),
                UseForeshore = true,
                CategoryType = AssessmentSectionCategoryType.LowerLimitNorm
            };

            // Call
            ExportableWaveConditions[] exportableWaveConditionsCollection =
                ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("ewcName",
                                                                                         waveConditionsInput,
                                                                                         waveConditionsOutputCollection,
                                                                                         coverType).ToArray();

            // Assert
            Assert.AreEqual(1, exportableWaveConditionsCollection.Length);
            ExportableWaveConditions exportableWaveConditions = exportableWaveConditionsCollection[0];
            Assert.AreEqual("ewcName", exportableWaveConditions.CalculationName);
            Assert.AreEqual("hblName", exportableWaveConditions.LocationName);
            Assert.AreEqual(1.0, exportableWaveConditions.LocationXCoordinate);
            Assert.AreEqual(8.0, exportableWaveConditions.LocationYCoordinate);
            Assert.AreEqual("id", exportableWaveConditions.ForeshoreId);
            Assert.AreEqual(false, exportableWaveConditions.UseBreakWater);
            Assert.AreEqual(true, exportableWaveConditions.UseForeshore);
            Assert.AreEqual(coverType, exportableWaveConditions.CoverType);
            Assert.AreEqual("B", exportableWaveConditions.CategoryBoundaryName);
            Assert.AreEqual(2, exportableWaveConditions.WaterLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(2, exportableWaveConditions.WaveHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(2, exportableWaveConditions.WavePeriod.NumberOfDecimalPlaces);
            Assert.AreEqual(2, exportableWaveConditions.WaveAngle.NumberOfDecimalPlaces);
            Assert.AreEqual(waveConditionsOutput.WaterLevel, exportableWaveConditions.WaterLevel);
            Assert.AreEqual(waveConditionsOutput.WaveHeight, exportableWaveConditions.WaveHeight);
            Assert.AreEqual(waveConditionsOutput.WavePeakPeriod, exportableWaveConditions.WavePeriod);
            Assert.AreEqual(waveConditionsOutput.WaveAngle, exportableWaveConditions.WaveAngle);
            Assert.AreEqual(waveConditionsOutput.WaveDirection, exportableWaveConditions.WaveDirection);
        }

        private static IEnumerable<TestCaseData> GetCoverTypes()
        {
            yield return new TestCaseData(CoverType.Asphalt);
            yield return new TestCaseData(CoverType.GrassWaveRunUp);
            yield return new TestCaseData(CoverType.GrassWaveImpact);
            yield return new TestCaseData(CoverType.StoneCoverBlocks);
            yield return new TestCaseData(CoverType.StoneCoverColumns);
        }
    }
}