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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Integration.Data;
using Riskeer.Integration.Plugin.Merge;

namespace Riskeer.Integration.Plugin.Test.Merge
{
    [TestFixture]
    public class AssessmentSectionProviderTest : NUnitFormTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Plugin, nameof(AssessmentSectionProvider));

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            // Call
            var provider = new AssessmentSectionProvider(viewParent);

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionProvider>(provider);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ViewParentNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssessmentSectionProvider(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("viewParent", exception.ParamName);
        }

        [Test]
        public void GetAssessmentSections_FilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var provider = new AssessmentSectionProvider(viewParent);

            // Call
            TestDelegate call = () => provider.GetAssessmentSections(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("filePath", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAssessmentSections_AssessmentSectionsFromActivityNull_ThrowsAssessmentSectionProviderException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var provider = new AssessmentSectionProvider(viewParent);

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            // Call
            TestDelegate call = () => provider.GetAssessmentSections("filePath");

            // Assert
            Assert.Throws<AssessmentSectionProviderException>(call);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAssessmentSections_ValidFilePath_ReturnsAssessmentSections()
        {
            // Setup
            var mocks = new MockRepository();
            var viewParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var provider = new AssessmentSectionProvider(viewParent);
            string filePath = Path.Combine(testDataPath, "project.risk");

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            // Call
            IEnumerable<AssessmentSection> assessmentSections = provider.GetAssessmentSections(filePath);

            // Assert
            Assert.AreEqual(1, assessmentSections.Count());
            mocks.VerifyAll();
        }
    }
}