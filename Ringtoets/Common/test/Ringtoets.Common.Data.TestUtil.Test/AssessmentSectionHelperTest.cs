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

using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class AssessmentSectionHelperTest
    {
        [Test]
        public void CreateAssessmentSectionStub_WithoutFailureMechanismAndFilePath_ReturnsExpectedAssessmentSectionStub()
        {
            // Setup
            var mocks = new MockRepository();

            // Call
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);
            mocks.ReplayAll();

            // Assert
            Assert.IsNotNull(assessmentSection);

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            Assert.IsNotNull(hydraulicBoundaryDatabase);
            Assert.IsNull(hydraulicBoundaryDatabase.FilePath);
            Assert.IsNull(hydraulicBoundaryDatabase.Version);
            CollectionAssert.IsEmpty(hydraulicBoundaryDatabase.Locations);
            Assert.IsFalse(hydraulicBoundaryDatabase.CanUsePreprocessor);
            Assert.IsFalse(hydraulicBoundaryDatabase.UsePreprocessor);
            Assert.IsNull(hydraulicBoundaryDatabase.PreprocessorDirectory);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateAssessmentSectionStub_WithFailureMechanismOnly_ReturnsExpectedAssessmentSectionStub()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();
            var mocks = new MockRepository();

            // Call
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
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
            Assert.IsFalse(hydraulicBoundaryDatabase.CanUsePreprocessor);
            Assert.IsFalse(hydraulicBoundaryDatabase.UsePreprocessor);
            Assert.IsNull(hydraulicBoundaryDatabase.PreprocessorDirectory);

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
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks, path);
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
            Assert.IsFalse(hydraulicBoundaryDatabase.CanUsePreprocessor);
            Assert.IsFalse(hydraulicBoundaryDatabase.UsePreprocessor);
            Assert.IsNull(hydraulicBoundaryDatabase.PreprocessorDirectory);

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
            Assert.AreEqual(AssessmentSectionHelper.GetTestNormativeAssessmentLevel(), AssessmentSectionHelper.GetTestNormativeAssessmentLevel());
        }
    }
}