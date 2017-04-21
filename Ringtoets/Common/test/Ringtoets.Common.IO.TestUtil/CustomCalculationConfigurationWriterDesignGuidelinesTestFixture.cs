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
using System.Linq;
using System.Security.AccessControl;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.IO.Configurations.Export;

namespace Ringtoets.Common.IO.TestUtil
{
    [TestFixture]
    public abstract class CustomCalculationConfigurationWriterDesignGuidelinesTestFixture<TWriter, TCalculation>
        where TCalculation : class, ICalculation
        where TWriter : CalculationConfigurationWriter<TCalculation>, new()
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var writer = new TWriter();

            // Assert
            AssertDefaultConstructedInstance(writer);
        }

        [Test]
        public void Write_ConfigurationNull_ThrowArgumentNullException()
        {
            // Setup
            var writer = new TWriter();

            // Call
            TestDelegate test = () => writer.Write(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            AssertNullConfiguration(exception);
        }

        [Test]
        public void Write_FilePathNull_ThrowArgumentNullException()
        {
            // Setup
            var writer = new TWriter();

            // Call
            TestDelegate test = () => writer.Write(Enumerable.Empty<ICalculationBase>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            AssertNullFilePath(exception);
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Write_FilePathInvalid_ThrowCriticalFileWriteException(string filePath)
        {
            // Setup
            var writer = new TWriter();

            // Call
            TestDelegate call = () => writer.Write(Enumerable.Empty<ICalculationBase>(), filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(call);
            AssertInvalidFilePath(exception, filePath);
        }

        [Test]
        public void Write_FilePathTooLong_ThrowCriticalFileWriteException()
        {
            // Setup
            var filePath = new string('a', 249);
            var writer = new TWriter();

            // Call
            TestDelegate call = () => writer.Write(Enumerable.Empty<ICalculationBase>(), filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(call);
            AssertTooLongPath(exception, filePath);
        }

        [Test]
        public void Write_InvalidDirectoryRights_ThrowCriticalFileWriteException()
        {
            // Setup
            var writer = new TWriter();
            string directoryPath = TestHelper.GetScratchPadPath(nameof(Write_InvalidDirectoryRights_ThrowCriticalFileWriteException));
            using (var disposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(Write_InvalidDirectoryRights_ThrowCriticalFileWriteException)))
            {
                string filePath = Path.Combine(directoryPath, "test.xml");
                disposeHelper.LockDirectory(FileSystemRights.Write);

                // Call
                TestDelegate call = () => writer.Write(Enumerable.Empty<ICalculationBase>(), filePath);

                // Assert
                var exception = Assert.Throws<CriticalFileWriteException>(call);
                AssertInvalidDirectoryRights(exception, filePath);
            }
        }

        [Test]
        public void Write_FileInUse_ThrowCriticalFileWriteException()
        {
            // Setup
            string path = TestHelper.GetScratchPadPath(nameof(Write_FileInUse_ThrowCriticalFileWriteException));
            var writer = new TWriter();

            using (var fileDisposeHelper = new FileDisposeHelper(path))
            {
                fileDisposeHelper.LockFiles();

                // Call
                TestDelegate call = () => writer.Write(Enumerable.Empty<ICalculationBase>(), path);

                // Assert
                var exception = Assert.Throws<CriticalFileWriteException>(call);
                AssertFileInUse(exception, path);
            }
        }

        protected virtual void AssertDefaultConstructedInstance(TWriter writer)
        {
            Assert.IsInstanceOf<CalculationConfigurationWriter<TCalculation>>(writer);
        }

        protected virtual void AssertNullConfiguration(ArgumentNullException exception)
        {
            Assert.IsNotNull(exception);
            Assert.AreEqual("configuration", exception.ParamName);
        }

        protected virtual void AssertNullFilePath(ArgumentNullException exception)
        {
            Assert.IsNotNull(exception);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        protected virtual void AssertInvalidFilePath(CriticalFileWriteException exception, string filePath)
        {
            Assert.IsNotNull(exception);
            Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
        }

        protected virtual void AssertTooLongPath(CriticalFileWriteException exception, string filePath)
        {
            Assert.IsNotNull(exception);
            Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
            Assert.IsInstanceOf<PathTooLongException>(exception.InnerException);
        }

        protected virtual void AssertInvalidDirectoryRights(CriticalFileWriteException exception, string filePath)
        {
            Assert.IsNotNull(exception);
            Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
            Assert.IsInstanceOf<UnauthorizedAccessException>(exception.InnerException);
        }

        protected virtual void AssertFileInUse(CriticalFileWriteException exception, string filePath)
        {
            Assert.IsNotNull(exception);
            Assert.AreEqual($"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.", exception.Message);
            Assert.IsInstanceOf<IOException>(exception.InnerException);
        }
    }
}