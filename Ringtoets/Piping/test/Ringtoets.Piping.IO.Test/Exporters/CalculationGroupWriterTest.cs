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

using System;
using System.IO;
using System.Security.AccessControl;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Piping.IO.Exporters;

namespace Ringtoets.Piping.IO.Test.Exporters
{
    [TestFixture]
    public class CalculationGroupWriterTest
    {
        [Test]
        public void WriteCalculationGroups_CalculationGroupNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => CalculationGroupWriter.WriteCalculationGroups(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("rootCalculationGroup", exception.ParamName);
        }

        [Test]
        public void WriteCalculationGroups_FilePathNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => CalculationGroupWriter.WriteCalculationGroups(new CalculationGroup(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("c:\\>")]
        public void WriteCalculationGroups_FilePathInvalid_ThrowCriticalFileWriteException(string filePath)
        {
            // Call
            TestDelegate call = () => CalculationGroupWriter.WriteCalculationGroups(new CalculationGroup(), filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(call);
            Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);
        }

        [Test]
        public void WriteCalculationGroups_FilePathTooLong_ThrowCriticalFileWriteException()
        {
            // Setup
            var filePath = new string('a', 249);

            // Call
            TestDelegate call = () => CalculationGroupWriter.WriteCalculationGroups(new CalculationGroup(), filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(call);
            Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
            Assert.IsInstanceOf<PathTooLongException>(exception.InnerException);
        }

        [Test]
        public void WriteCalculationGroups_InvalidDirectoryRights_ThrowCriticalFileWriteException()
        {
            // Setup
            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO,
                                                              "WriteCalculationGroups_InvalidDirectoryRights_ThrowCriticalFileWriteException");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.xml");

            // Call
            TestDelegate call = () => CalculationGroupWriter.WriteCalculationGroups(new CalculationGroup(), filePath);

            try
            {
                using (new DirectoryPermissionsRevoker(directoryPath, FileSystemRights.Write))
                {
                    // Assert
                    var exception = Assert.Throws<CriticalFileWriteException>(call);
                    Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
                    Assert.IsInstanceOf<UnauthorizedAccessException>(exception.InnerException);
                }
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }
    }
}