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

using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Storage.Core.Properties;
using Riskeer.Storage.Core.TestUtil;

namespace Riskeer.Storage.Core.Test
{
    [TestFixture]
    [Explicit("Creates a new Riskeer.rtd file in the root of the Riskeer.Storage.Core")]
    public class RiskeerDatabaseCreatorTest
    {
        /// <summary>
        /// Creates a new Riskeer.rtd file in the root of the <c>Riskeer.Storage.Core</c>, 
        /// which is used to auto-generate the database code.
        /// </summary>
        [Test]
        public void RingtoetsDatabaseCreator_Explicit_CreatesRiskeerProjectDatabaseFile()
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
            return Path.Combine(Path.GetDirectoryName(TestHelper.SolutionRoot), "Ringtoets", "Storage", "src", "Riskeer.Storage.Core", "Riskeer.rtd");
        }
    }
}