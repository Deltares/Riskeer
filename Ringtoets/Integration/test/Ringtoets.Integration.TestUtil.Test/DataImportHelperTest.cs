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
using System.IO;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data;

namespace Ringtoets.Integration.TestUtil.Test
{
    [TestFixture]
    public class DataImportHelperTest
    {
        private AssessmentSection assessmentSection;

        [SetUp]
        public void SetUp()
        {
            assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
        }

        [Test]
        public void ImportReferenceLine_ValidAssessmentSection_AddsReferenceLineGeometry()
        {
            // Call
            DataImportHelper.ImportReferenceLine(assessmentSection);

            // Assert
            Assert.AreEqual(2380, assessmentSection.ReferenceLine.Points.Count());
        }

        [Test]
        public void ImportFailureMechanismSections_WithReferenceLine_AddsSectionsToSuppliedFailureMechanismOnly()
        {
            // Setup
            DataImportHelper.ImportReferenceLine(assessmentSection);
            const int failureMechanismCount = 18;
            int chosenFailureMechanismIndex = new Random(21).Next(0, failureMechanismCount);
            IFailureMechanism failureMechanism = assessmentSection.GetFailureMechanisms().ElementAt(chosenFailureMechanismIndex);

            // Call
            DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);

            // Assert
            int[] expectedSectionCounts = Enumerable.Repeat(0, failureMechanismCount).ToArray();
            expectedSectionCounts[chosenFailureMechanismIndex] = 283;
            CollectionAssert.AreEqual(expectedSectionCounts, assessmentSection.GetFailureMechanisms().Select(fm => fm.Sections.Count()));
        }

        [Test]
        public void ImportFailureMechanismSections_MultipleFailureMechanismsWithReferenceLine_AddsSectionsToSuppliedFailureMechanismOnly()
        {
            // Setup
            DataImportHelper.ImportReferenceLine(assessmentSection);
            const int failureMechanismCount = 18;
            var random = new Random(21);
            IEnumerable<int> chosenNumbers = new[]
            {
                random.Next(failureMechanismCount),
                random.Next(failureMechanismCount),
                random.Next(failureMechanismCount)
            }.Distinct();
            IEnumerable<IFailureMechanism> sectionFailureMechanisms = assessmentSection.GetFailureMechanisms().ToArray();
            IEnumerable<IFailureMechanism> failureMechanisms = sectionFailureMechanisms.Where((fm, i) => chosenNumbers.Contains(i));

            // Call
            DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanisms);

            // Assert
            int[] expectedSectionCounts = Enumerable.Repeat(0, failureMechanismCount).ToArray();
            foreach (int chosenNumber in chosenNumbers)
            {
                expectedSectionCounts[chosenNumber] = 283;
            }

            CollectionAssert.AreEqual(expectedSectionCounts, sectionFailureMechanisms.Select(fm => fm.Sections.Count()));
        }

        [Test]
        public void ImportHydraulicBoundaryDatabase_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => DataImportHelper.ImportHydraulicBoundaryDatabase(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void ImportHydraulicBoundaryDatabase_ValidAssessmentSection_AddsHydraulicBoundaryLocations()
        {
            // Call
            DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection);

            // Assert
            Assert.AreEqual(18, assessmentSection.HydraulicBoundaryDatabase.Locations.Count);
        }

        [Test]
        public void ImportHydraulicBoundaryDatabaseWithFilePath_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => DataImportHelper.ImportHydraulicBoundaryDatabase(null, "");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void ImportHydraulicBoundaryDatabaseWithFilePath_ValidData_AddsHydraulicBoundaryLocations()
        {
            // Setup
            string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.TestUtil, nameof(DataImportHelper));

            // Call
            DataImportHelper.ImportHydraulicBoundaryDatabase(assessmentSection, Path.Combine(testDataPath, "complete.sqlite"));

            // Assert
            Assert.AreEqual(18, assessmentSection.HydraulicBoundaryDatabase.Locations.Count);
        }

        [Test]
        public void ImportPipingSurfaceLines_WithReferenceLine_AddsFourSurfaceLines()
        {
            // Setup
            DataImportHelper.ImportReferenceLine(assessmentSection);

            // Call
            DataImportHelper.ImportPipingSurfaceLines(assessmentSection);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "PK001_0001",
                "PK001_0002",
                "PK001_0003",
                "PK001_0004"
            }, assessmentSection.Piping.SurfaceLines.Select(sm => sm.Name));
        }

        [Test]
        public void ImportPipingStochasticSoilModels_Always_AddsFourSoilModelsWithProfiles()
        {
            // Call
            DataImportHelper.ImportPipingStochasticSoilModels(assessmentSection);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "PK001_0001_Piping",
                "PK001_0002_Piping",
                "PK001_0003_Piping",
                "PK001_0004_Piping"
            }, assessmentSection.Piping.StochasticSoilModels.Select(sm => sm.Name));
            CollectionAssert.AreEqual(new[]
            {
                1,
                1,
                1,
                1
            }, assessmentSection.Piping.StochasticSoilModels.SelectMany(sm => sm.StochasticSoilProfiles.Select(sp => sp.Probability)));
            CollectionAssert.AreEqual(new[]
            {
                "W1-6_0_1D1",
                "W1-6_4_1D1",
                "W1-7_0_1D1",
                "W1-8_6_1D1"
            }, assessmentSection.Piping.StochasticSoilModels.SelectMany(sm => sm.StochasticSoilProfiles.Select(sp => sp.SoilProfile.Name)));
        }

        [Test]
        public void ImportMacroStabilityInwardsSurfaceLines_WithReferenceLine_AddsFourSurfaceLines()
        {
            // Setup
            DataImportHelper.ImportReferenceLine(assessmentSection);

            // Call
            DataImportHelper.ImportMacroStabilityInwardsSurfaceLines(assessmentSection);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "PK001_0001",
                "PK001_0002",
                "PK001_0003",
                "PK001_0004"
            }, assessmentSection.MacroStabilityInwards.SurfaceLines.Select(sm => sm.Name));
        }

        [Test]
        public void ImportMacroStabilityInwardsStochasticSoilModels_Always_AddsFourSoilModelsWithProfiles()
        {
            // Call
            DataImportHelper.ImportMacroStabilityInwardsStochasticSoilModels(assessmentSection);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "PK001_0001_Stability",
                "PK001_0002_Stability",
                "PK001_0003_Stability",
                "PK001_0004_Stability"
            }, assessmentSection.MacroStabilityInwards.StochasticSoilModels.Select(sm => sm.Name));
            CollectionAssert.AreEqual(new[]
            {
                1,
                1,
                1,
                1
            }, assessmentSection.MacroStabilityInwards.StochasticSoilModels.SelectMany(sm => sm.StochasticSoilProfiles.Select(sp => sp.Probability)));
            CollectionAssert.AreEqual(new[]
            {
                "W1-6_0_1D1",
                "W1-6_4_1D1",
                "W1-7_0_1D1",
                "W1-8_6_1D1"
            }, assessmentSection.MacroStabilityInwards.StochasticSoilModels.SelectMany(sm => sm.StochasticSoilProfiles.Select(sp => sp.SoilProfile.Name)));
        }
    }
}