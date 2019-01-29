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
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.Configurations.Export;

namespace Riskeer.Common.IO.TestUtil
{
    [TestFixture]
    public abstract class CustomCalculationConfigurationWriterDesignGuidelinesTestFixture<TWriter, TConfiguration>
        where TConfiguration : class, IConfigurationItem
        where TWriter : CalculationConfigurationWriter<TConfiguration>
    {
        [Test]
        public void Write_CalculationOfTypeOtherThanGiven_ThrowsCriticalFileWriteExceptionWithInnerArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<IConfigurationItem>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetScratchPadPath("test.xml");

            try
            {
                TWriter writer = CreateWriterInstance(filePath);

                // Call
                TestDelegate test = () => writer.Write(new[]
                {
                    calculation
                });

                // Assert
                var exception = Assert.Throws<CriticalFileWriteException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual($"Cannot write calculation of type '{calculation.GetType()}' using this writer.", innerException.Message);
            }
            finally
            {
                File.Delete(filePath);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutFilePath_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => CreateWriterInstance(null);

            // Assert
            AssertNullFilePath(Assert.Throws<ArgumentException>(call));
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Constructor_WithoutValidFilePath_ThrowsArgumentException(string filePath)
        {
            // Call
            TestDelegate call = () => CreateWriterInstance(filePath);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void Write_FilePathTooLong_ThrowCriticalFileWriteException()
        {
            // Setup
            var filePath = new string('a', 249);
            TWriter writerInstance = CreateWriterInstance(filePath);

            // Call
            TestDelegate call = () => writerInstance.Write(Enumerable.Empty<IConfigurationItem>());

            // Assert
            var exception = Assert.Throws<CriticalFileWriteException>(call);
            AssertTooLongPath(exception, filePath);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            TWriter writer = CreateWriterInstance("//validpath");

            // Assert
            AssertDefaultConstructedInstance(writer);
        }

        [Test]
        public void Write_ConfigurationNull_ThrowArgumentNullException()
        {
            // Setup
            TWriter writer = CreateWriterInstance("//validpath");

            // Call
            TestDelegate test = () => writer.Write(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            AssertNullConfiguration(exception);
        }

        [Test]
        public void Write_InvalidDirectoryRights_ThrowCriticalFileWriteException()
        {
            // Setup
            string directoryPath = TestHelper.GetScratchPadPath(nameof(Write_InvalidDirectoryRights_ThrowCriticalFileWriteException));
            using (var disposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(Write_InvalidDirectoryRights_ThrowCriticalFileWriteException)))
            {
                string filePath = Path.Combine(directoryPath, "test.xml");
                disposeHelper.LockDirectory(FileSystemRights.Write);
                TWriter writer = CreateWriterInstance(filePath);

                // Call
                TestDelegate call = () => writer.Write(Enumerable.Empty<IConfigurationItem>());

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

            using (var fileDisposeHelper = new FileDisposeHelper(path))
            {
                fileDisposeHelper.LockFiles();
                TWriter writer = CreateWriterInstance(path);

                // Call
                TestDelegate call = () => writer.Write(Enumerable.Empty<IConfigurationItem>());

                // Assert
                var exception = Assert.Throws<CriticalFileWriteException>(call);
                AssertFileInUse(exception, path);
            }
        }

        protected abstract TWriter CreateWriterInstance(string filePath);

        protected virtual void AssertDefaultConstructedInstance(TWriter writer)
        {
            Assert.IsInstanceOf<CalculationConfigurationWriter<TConfiguration>>(writer);
        }

        protected virtual void AssertNullConfiguration(ArgumentNullException exception)
        {
            Assert.IsNotNull(exception);
            Assert.AreEqual("configurations", exception.ParamName);
        }

        protected virtual void AssertNullFilePath(ArgumentException exception)
        {
            Assert.IsNotNull(exception);
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