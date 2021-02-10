// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Gui.Helpers;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Helpers
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
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            void Call() => ExportHelper.GetFilePath(inquiryHelper, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("fileFilterGenerator", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(null)]
        [TestCase("C:/test")]
        public void GetFilePath_Always_ReturnsSelectedFilePath(string expectedFilePath)
        {
            // Setup
            var fileFilterGenerator = new FileFilterGenerator("testExtension", "testDescription");

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(ih => ih.GetTargetFileLocation(fileFilterGenerator.Filter, null))
                         .Return(expectedFilePath);
            mocks.ReplayAll();

            // Call
            string filePath = ExportHelper.GetFilePath(inquiryHelper, fileFilterGenerator);

            // Assert
            Assert.AreEqual(expectedFilePath, filePath);
            mocks.VerifyAll();
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
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(ih => ih.GetTargetFolderLocation())
                         .Return(expectedFilePath);
            mocks.ReplayAll();

            // Call
            string filePath = ExportHelper.GetFolderPath(inquiryHelper);

            // Assert
            Assert.AreEqual(expectedFilePath, filePath);
            mocks.VerifyAll();
        }
    }
}