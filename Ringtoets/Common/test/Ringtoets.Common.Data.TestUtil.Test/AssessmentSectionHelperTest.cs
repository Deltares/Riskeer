﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class AssessmentSectionHelperTest
    {
        [Test]
        public void CreateAssessmentSectionStub_WithoutFilePathSet_ReturnsStubbedAssessmentSectionWithHRDWithoutPath()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();
            var mocks = new MockRepository();

            // Call
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            // Assert
            Assert.IsNotNull(assessmentSectionStub);
            Assert.AreEqual("21", assessmentSectionStub.Id);
            var hydraulicBoundaryDatabase = assessmentSectionStub.HydraulicBoundaryDatabase;
            Assert.IsNotNull(hydraulicBoundaryDatabase);
            Assert.IsNull(hydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(1, hydraulicBoundaryDatabase.Locations.Count);
            var hydraulicBoundaryLocation = hydraulicBoundaryDatabase.Locations.First();
            Assert.AreEqual(1300001, hydraulicBoundaryLocation.Id);
            Assert.AreEqual(string.Empty, hydraulicBoundaryLocation.Name);
            Assert.AreEqual(new Point2D(0, 0), hydraulicBoundaryLocation.Location);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateAssessmentSectionStub_WithFilePathSet_ReturnsStubbedAssessmentSectionWithHRDWithPath()
        {
            // Setup
            var path = "C://temp";
            var failureMechanism = new TestFailureMechanism();
            var mocks = new MockRepository();

            // Call
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks, path);
            mocks.ReplayAll();

            // Assert
            Assert.IsNotNull(assessmentSectionStub);
            Assert.AreEqual("21", assessmentSectionStub.Id);
            var hydraulicBoundaryDatabase = assessmentSectionStub.HydraulicBoundaryDatabase;
            Assert.IsNotNull(hydraulicBoundaryDatabase);
            Assert.AreEqual(path, hydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(1, hydraulicBoundaryDatabase.Locations.Count);
            var hydraulicBoundaryLocation = hydraulicBoundaryDatabase.Locations.First();
            Assert.AreEqual(1300001, hydraulicBoundaryLocation.Id);
            Assert.AreEqual(string.Empty, hydraulicBoundaryLocation.Name);
            Assert.AreEqual(new Point2D(0, 0), hydraulicBoundaryLocation.Location);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateAssessmentSectionStubWithoutBoundaryDatabase_Always_ReturnsStubbedAssessmentSectionWithoutHRD()
        {
            var failureMechanism = new TestFailureMechanism();
            var mocks = new MockRepository();

            // Call
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(failureMechanism, mocks);
            mocks.ReplayAll();

            // Assert
            Assert.IsNotNull(assessmentSectionStub);
            Assert.AreEqual("21", assessmentSectionStub.Id);
            Assert.IsNull(assessmentSectionStub.HydraulicBoundaryDatabase);
            mocks.VerifyAll();
        }
    }
}