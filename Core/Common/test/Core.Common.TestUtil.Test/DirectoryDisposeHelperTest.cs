// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.IO;
using NUnit.Framework;

namespace Core.Common.TestUtil.Test
{
    public class DirectoryDisposeHelperTest
    {
        [Test]
        public void Constructor_NotExistingDirectory_DoesNotThrowException()
        {
            // Setup
            string directoryPath = "doesNotExist";
            DirectoryDisposeHelper disposeHelper = null;

            // Precondition
            Assert.IsFalse(Directory.Exists(directoryPath), String.Format("Precondition failed: Directory '{0}' should not exist", directoryPath));

            // Call
            TestDelegate test = () => disposeHelper = new DirectoryDisposeHelper(directoryPath);

            // Assert
            Assert.DoesNotThrow(test);
            disposeHelper.Dispose();
            Assert.IsFalse(Directory.Exists(directoryPath));
        }

        [Test]
        public void Constructor_ExistingDirectory_DoesNotThrowException()
        {
            // Setup
            string directoryPath = "doesExist";
            DirectoryDisposeHelper disposeHelper = null;

            try
            {
                Directory.CreateDirectory(directoryPath);

                // Precondition
                Assert.IsTrue(Directory.Exists(directoryPath), String.Format("Precondition failed: Directory '{0}' should exist", directoryPath));

                // Call
                TestDelegate test = () => disposeHelper = new DirectoryDisposeHelper(directoryPath);

                // Assert
                Assert.DoesNotThrow(test);
                disposeHelper.Dispose();
            }
            catch (Exception exception)
            {
                Directory.Delete(directoryPath);
                Assert.Fail(exception.Message);
            }
            Assert.IsFalse(Directory.Exists(directoryPath));
        }

        [Test]
        public void Create_DirectoryDoesNotExist_CreatesDirectory()
        {
            // Setup
            string directoryPath = "willExist";

            try
            {
                // Precondition
                Assert.IsFalse(Directory.Exists(directoryPath));

                // Call
                using (new DirectoryDisposeHelper(directoryPath))
                {
                    // Assert
                    Assert.IsTrue(Directory.Exists(directoryPath));
                }
            }
            catch (Exception exception)
            {
                Directory.Delete(directoryPath);
                Assert.Fail(exception.Message);
            }
            Assert.IsFalse(Directory.Exists(directoryPath));
        }

        [Test]
        public void Create_InvalidPath_DoesNotThrowException()
        {
            // Setup
            var directoryPath = String.Empty;

            // Call
            TestDelegate test = () => new DirectoryDisposeHelper(directoryPath);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void Dispose_ExistingDirectory_DeletesDirectory()
        {
            // Setup
            string directoryPath = "doesExist";

            try
            {
                Directory.CreateDirectory(directoryPath);

                // Precondition
                Assert.IsTrue(Directory.Exists(directoryPath));

                // Call
                using (new DirectoryDisposeHelper(directoryPath)) { }
            }
            catch (Exception exception)
            {
                Directory.Delete(directoryPath);
                Assert.Fail(exception.Message);
            }

            // Assert
            Assert.IsFalse(Directory.Exists(directoryPath));
        }
    }
}