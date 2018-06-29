﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Storage.Core.Properties;

namespace Ringtoets.Storage.Core.Test
{
    [TestFixture]
    [Explicit("Creates a new Ringtoets.rtd file in the root of the Application.Ringtoets.Storage")]
    public class RingtoetsDatabaseCreatorTest
    {
        /// <summary>
        /// Creates a new Ringtoets.rtd file in the root of the <see cref="Application.Ringtoets.Storage"/>, 
        /// which is used to auto-generate the database code.
        /// </summary>
        [Test]
        public void RingtoetsDatabaseCreator_Explicit_CreatesRingtoetsProjectDatabaseFile()
        {
            // Setup
            string storageFile = GetPathToStorageFile();
            if (File.Exists(storageFile))
            {
                TestDelegate precondition = () => File.Delete(storageFile);
                Assert.DoesNotThrow(precondition, "Precondition failed: file could not be deleted: '{0}'", storageFile);
            }

            // Call
            SqLiteDatabaseHelper.CreateDatabaseFile(storageFile, Resources.DatabaseStructure);

            // Assert
            Assert.IsTrue(File.Exists(storageFile));
        }

        private static string GetPathToStorageFile()
        {
            return Path.Combine(Path.GetDirectoryName(TestHelper.SolutionRoot), "Application", "Ringtoets", "src", "Application.Ringtoets.Storage", "Ringtoets.rtd");
        }
    }
}