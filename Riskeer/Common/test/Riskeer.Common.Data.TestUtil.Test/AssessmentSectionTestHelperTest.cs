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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class AssessmentSectionTestHelperTest
    {
        [Test]
        public void CreateAssessmentSectionStub_WithoutFailureMechanismAndFilePath_ReturnsExpectedAssessmentSectionStub()
        {
            // Setup
            var mocks = new MockRepository();

            // Call
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            mocks.ReplayAll();

            // Assert
            Assert.IsNotNull(assessmentSection);

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            Assert.IsNotNull(hydraulicBoundaryDatabase);
            Assert.IsNull(hydraulicBoundaryDatabase.FilePath);
            Assert.IsNull(hydraulicBoundaryDatabase.Version);
            CollectionAssert.IsEmpty(hydraulicBoundaryDatabase.Locations);
            Assert.IsFalse(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.CanUsePreprocessor);
            Assert.IsFalse(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessor);
            Assert.IsNull(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.PreprocessorDirectory);
            Assert.IsNotNull(assessmentSection.ReferenceLine);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateAssessmentSectionStub_WithFailureMechanismOnly_ReturnsExpectedAssessmentSectionStub()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();
            var mocks = new MockRepository();

            // Call
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            // Assert
            Assert.IsNotNull(assessmentSection);
            Assert.AreEqual("21", assessmentSection.Id);
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;

            Assert.IsNotNull(hydraulicBoundaryDatabase);
            Assert.IsNull(hydraulicBoundaryDatabase.FilePath);
            Assert.IsNull(hydraulicBoundaryDatabase.Version);
            Assert.AreEqual(1, hydraulicBoundaryDatabase.Locations.Count);
            HydraulicBoundaryLocation hydraulicBoundaryLocation = hydraulicBoundaryDatabase.Locations.First();
            Assert.AreEqual(1300001, hydraulicBoundaryLocation.Id);
            Assert.IsEmpty(hydraulicBoundaryLocation.Name);
            Assert.AreEqual(new Point2D(0, 0), hydraulicBoundaryLocation.Location);
            Assert.IsFalse(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.CanUsePreprocessor);
            Assert.IsFalse(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessor);
            Assert.IsNull(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.PreprocessorDirectory);
            Assert.IsNotNull(assessmentSection.ReferenceLine);

            HydraulicLocationConfigurationSettings settings = hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings;
            Assert.IsNull(settings.FilePath);
            Assert.IsNull(settings.ScenarioName);
            Assert.AreEqual(0, settings.Year);
            Assert.IsNull(settings.Scope);
            Assert.IsNull(settings.SeaLevel);
            Assert.IsNull(settings.RiverDischarge);
            Assert.IsNull(settings.LakeLevel);
            Assert.IsNull(settings.WindDirection);
            Assert.IsNull(settings.WindSpeed);
            Assert.IsNull(settings.Comment);

            CollectionAssert.AreEqual(new[]
            {
                failureMechanism
            }, assessmentSection.GetFailureMechanisms());

            FailureMechanismContribution contribution = assessmentSection.FailureMechanismContribution;
            Assert.AreEqual(1.0 / 10, contribution.LowerLimitNorm);
            Assert.AreEqual(1.0 / 30000, contribution.SignalingNorm);
            Assert.AreEqual(NormType.LowerLimit, contribution.NormativeNorm);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateAssessmentSectionStub_WithFailureMechanismAndFilePath_ReturnsExpectedAssessmentSectionStub()
        {
            // Setup
            const string path = "C://temp";
            var failureMechanism = new TestFailureMechanism();
            var mocks = new MockRepository();

            // Call
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks, path);
            mocks.ReplayAll();

            // Assert
            Assert.IsNotNull(assessmentSection);
            Assert.AreEqual("21", assessmentSection.Id);

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            Assert.IsNotNull(hydraulicBoundaryDatabase);
            Assert.AreEqual(path, hydraulicBoundaryDatabase.FilePath);
            Assert.IsNull(hydraulicBoundaryDatabase.Version);
            Assert.AreEqual(1, hydraulicBoundaryDatabase.Locations.Count);
            HydraulicBoundaryLocation hydraulicBoundaryLocation = hydraulicBoundaryDatabase.Locations.First();
            Assert.AreEqual(1300001, hydraulicBoundaryLocation.Id);
            Assert.IsEmpty(hydraulicBoundaryLocation.Name);
            Assert.AreEqual(new Point2D(0, 0), hydraulicBoundaryLocation.Location);
            Assert.IsFalse(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.CanUsePreprocessor);
            Assert.IsFalse(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessor);
            Assert.IsNull(hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.PreprocessorDirectory);
            Assert.IsNotNull(assessmentSection.ReferenceLine);

            HydraulicLocationConfigurationSettings settings = hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings;
            Assert.AreEqual("C:\\hlcd.sqlite", settings.FilePath);
            Assert.AreEqual("ScenarioName", settings.ScenarioName);
            Assert.AreEqual(1337, settings.Year);
            Assert.AreEqual("Scope", settings.Scope);
            Assert.AreEqual("SeaLevel", settings.SeaLevel);
            Assert.AreEqual("RiverDischarge", settings.RiverDischarge);
            Assert.AreEqual("LakeLevel", settings.LakeLevel);
            Assert.AreEqual("WindDirection", settings.WindDirection);
            Assert.AreEqual("WindSpeed", settings.WindSpeed);
            Assert.AreEqual("Comment", settings.Comment);

            CollectionAssert.AreEqual(new[]
            {
                failureMechanism
            }, assessmentSection.GetFailureMechanisms());

            FailureMechanismContribution contribution = assessmentSection.FailureMechanismContribution;
            Assert.AreEqual(1.0 / 10, contribution.LowerLimitNorm);
            Assert.AreEqual(1.0 / 30000, contribution.SignalingNorm);
            Assert.AreEqual(NormType.LowerLimit, contribution.NormativeNorm);

            mocks.VerifyAll();
        }

        [Test]
        public void GetTestNormativeAssessmentLevel_Always_ReturnsTheSameRandomValue()
        {
            // Call & Assert
            Assert.AreEqual(AssessmentSectionTestHelper.GetTestAssessmentLevel(), AssessmentSectionTestHelper.GetTestAssessmentLevel());
        }

        [Test]
        public void GetHydraulicBoundaryLocationCalculationConfigurationPerAssessmentSectionCategoryType_Always_ReturnsExpectedTestCaseDataCollection()
        {
            // Call
            TestCaseData[] testCaseDataCollection = AssessmentSectionTestHelper.GetHydraulicBoundaryLocationCalculationConfigurationPerAssessmentSectionCategoryType().ToArray();

            // Assert
            AssertTestCaseData(testCaseDataCollection,
                               "FactorizedSignalingNorm",
                               AssessmentSectionCategoryType.FactorizedSignalingNorm,
                               a => a.WaterLevelCalculationsForFactorizedSignalingNorm);
            AssertTestCaseData(testCaseDataCollection,
                               "SignalingNorm",
                               AssessmentSectionCategoryType.SignalingNorm,
                               a => a.WaterLevelCalculationsForSignalingNorm);
            AssertTestCaseData(testCaseDataCollection,
                               "LowerLimitNorm",
                               AssessmentSectionCategoryType.LowerLimitNorm,
                               a => a.WaterLevelCalculationsForLowerLimitNorm);
            AssertTestCaseData(testCaseDataCollection,
                               "FactorizedLowerLimitNorm",
                               AssessmentSectionCategoryType.FactorizedLowerLimitNorm,
                               a => a.WaterLevelCalculationsForFactorizedLowerLimitNorm);
        }

        private static void AssertTestCaseData(IEnumerable<TestCaseData> testCaseDataCollection,
                                               string expectedTestName,
                                               AssessmentSectionCategoryType categoryType,
                                               Func<IAssessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            TestCaseData testCaseData = testCaseDataCollection.Single(td => categoryType.Equals(td.Arguments.ElementAt(2)));

            Assert.AreEqual(expectedTestName, testCaseData.TestName);

            var assessmentSection = (IAssessmentSection) testCaseData.Arguments.ElementAt(0);
            var hydraulicBoundaryLocation = (HydraulicBoundaryLocation) testCaseData.Arguments.ElementAt(1);
            var hydraulicBoundaryLocationCalculation = (HydraulicBoundaryLocationCalculation) testCaseData.Arguments.ElementAt(3);

            HydraulicBoundaryLocationCalculation expectedHydraulicBoundaryLocationCalculation = getCalculationsFunc(assessmentSection).First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation));
            Assert.AreSame(expectedHydraulicBoundaryLocationCalculation, hydraulicBoundaryLocationCalculation);
        }
    }
}