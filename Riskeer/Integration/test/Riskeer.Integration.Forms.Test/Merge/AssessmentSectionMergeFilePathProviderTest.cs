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
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Integration.Forms.Merge;

namespace Ringtoets.Integration.Forms.Test.Merge
{
    [TestFixture]
    public class AssessmentSectionMergeFilePathProviderTest
    {
        [Test]
        public void Constructor_InquiryHelperNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssessmentSectionMergeFilePathProvider(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("inquiryHelper", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            var provider = new AssessmentSectionMergeFilePathProvider(inquiryHelper);

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionMergeFilePathProvider>(provider);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(null)]
        [TestCase("FilePath/Something")]
        public void GetFilePath_Always_ReturnFilePathFromInquiryHelper(string expectedFilePath)
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(ih => ih.GetSourceFileLocation("Ringtoetsproject (*.rtd)|*.rtd")).Return(expectedFilePath);
            mocks.ReplayAll();

            var provider = new AssessmentSectionMergeFilePathProvider(inquiryHelper);

            // Call
            string filePath = provider.GetFilePath();

            // Assert
            Assert.AreEqual(expectedFilePath, filePath);
            mocks.VerifyAll();
        }
    }
}