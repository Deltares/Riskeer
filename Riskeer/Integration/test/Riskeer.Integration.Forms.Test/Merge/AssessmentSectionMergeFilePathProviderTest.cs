// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Gui.Helpers;
using NSubstitute;
using NUnit.Framework;
using Riskeer.Integration.Forms.Merge;

namespace Riskeer.Integration.Forms.Test.Merge
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
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            // Call
            var provider = new AssessmentSectionMergeFilePathProvider(inquiryHelper);

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionMergeFilePathProvider>(provider);
        }

        [Test]
        [TestCase(null)]
        [TestCase("FilePath/Something")]
        public void GetFilePath_Always_ReturnFilePathFromInquiryHelper(string expectedFilePath)
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            inquiryHelper.GetSourceFileLocation("Riskeerproject (*.risk)|*.risk").Returns(expectedFilePath);
            var provider = new AssessmentSectionMergeFilePathProvider(inquiryHelper);

            // Call
            string filePath = provider.GetFilePath();

            // Assert
            Assert.AreEqual(expectedFilePath, filePath);
        }
    }
}