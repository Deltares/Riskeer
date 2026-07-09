// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Util;
using Core.Gui.Helpers;
using NSubstitute;
using NUnit.Framework;

namespace Core.Gui.Test.Helpers
{
    [TestFixture]
    public class ExportHelperTest
    {
        [Test]
        public void GetFilePath_InquiryHelperNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportHelper.GetFilePath(null, new FileFilterGenerator());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("inquiryHelper", exception.ParamName);
        }

        [Test]
        public void GetFilePath_FileFilterGeneratorNull_ThrowsArgumentNullException()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();

            // Call
            void Call() => ExportHelper.GetFilePath(inquiryHelper, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("fileFilterGenerator", exception.ParamName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("C:/test")]
        public void GetFilePath_WithoutSuggestedFileName_MakesExpectedCallToGetTargetFileLocationAndReturnsSelectedFilePath(string expectedFilePath)
        {
            // Setup
            var fileFilterGenerator = new FileFilterGenerator("testExtension", "testDescription");

            var inquiryHelper = Substitute.For<IInquiryHelper>();
            inquiryHelper.GetTargetFileLocation(fileFilterGenerator.Filter, null).Returns(expectedFilePath);

            // Call
            string filePath = ExportHelper.GetFilePath(inquiryHelper, fileFilterGenerator);

            // Assert
            Assert.AreEqual(expectedFilePath, filePath);
            inquiryHelper.Received().GetTargetFileLocation(fileFilterGenerator.Filter, null);
        }

        [Test]
        [Combinatorial]
        public void GetFilePath_WithSuggestedFileName_MakesExpectedCallToGetTargetFileLocationAndReturnsSelectedFilePath(
            [Values(null, "C:/test")] string expectedFilePath, [Values(null, "random.txt")] string suggestedFileName)
        {
            // Setup
            var fileFilterGenerator = new FileFilterGenerator("testExtension", "testDescription");

            var inquiryHelper = Substitute.For<IInquiryHelper>();
            inquiryHelper.GetTargetFileLocation(fileFilterGenerator.Filter, suggestedFileName).Returns(expectedFilePath);

            // Call
            string filePath = ExportHelper.GetFilePath(inquiryHelper, fileFilterGenerator, suggestedFileName);

            // Assert
            Assert.AreEqual(expectedFilePath, filePath);
            inquiryHelper.Received().GetTargetFileLocation(fileFilterGenerator.Filter, suggestedFileName);
        }

        [Test]
        public void GetFolderPath_InquiryHelperNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportHelper.GetFolderPath(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("inquiryHelper", exception.ParamName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("C:/test")]
        public void GetFolderPath_Always_ReturnsSelectedFolderPath(string expectedFilePath)
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            inquiryHelper.GetTargetFolderLocation().Returns(expectedFilePath);

            // Call
            string filePath = ExportHelper.GetFolderPath(inquiryHelper);

            // Assert
            Assert.AreEqual(expectedFilePath, filePath);
            inquiryHelper.Received().GetTargetFolderLocation();
        }
    }
}