// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.IO;
using System.Windows.Forms;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.IO.Exceptions;

namespace Ringtoets.Integration.Forms.Test
{
    [TestFixture]
    public class AssessmentSectionFromFileCommandHandlerTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "ReferenceLineMetaImporter");

        [Test]
        public void CreateAssessmentSectionFromFile_InvalidDirectory_ThrowsCriticalFileReadException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog = mockRepository.StrictMock<IWin32Window>();
            mockRepository.ReplayAll();
            AssessmentSectionFromFileCommandHandler assessmentSectionFromFile = new AssessmentSectionFromFileCommandHandler(parentDialog);

            string pathToNonExistingFolder = Path.Combine(testDataPath, "I do not exist");

            // Call
            TestDelegate call = () => assessmentSectionFromFile.CreateAssessmentSectionFromFile(pathToNonExistingFolder);

            // Assert
            Assert.Throws<CriticalFileReadException>(call);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CreateAssessmentSectionFromFile_validDirectoryWithEmptyShapeFile_ThrowsCriticalFileValidationException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var parentDialog = mockRepository.StrictMock<IWin32Window>();
            mockRepository.ReplayAll();
            AssessmentSectionFromFileCommandHandler assessmentSectionFromFile = new AssessmentSectionFromFileCommandHandler(parentDialog);

            string pathValidFolder = Path.Combine(testDataPath, "EmptyShapeFile");

            // Call
            TestDelegate call = () => assessmentSectionFromFile.CreateAssessmentSectionFromFile(pathValidFolder);

            // Assert
            CriticalFileValidationException exception = Assert.Throws<CriticalFileValidationException>(call);
            Assert.AreEqual("Er kunnen geen trajecten gelezen worden uit het shape bestand.", exception.Message);
            mockRepository.VerifyAll();
        }
    }
}