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
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Readers;

namespace Ringtoets.HydraRing.Calculation.Test.Readers
{
    [TestFixture]
    public class HydraRingDatabaseReaderTest
    {
        private const string validDatabase = "ValidDatabase";
        private const string invalidDatabase = "InvalidDatabase";
        private const string emptyWorkingDirectory = "EmptyWorkingDirectory";
        private const string emptyDatabase = "EmptyDatabase";

        private const string query = "SELECT * FROM IterateToGivenBetaConvergence " +
                                     "WHERE OuterIterationId = (SELECT MAX(OuterIterationId) FROM IterateToGivenBetaConvergence);";

        private static readonly string testDirectory = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation,
                                                                                  Path.Combine("Readers", nameof(HydraRingDatabaseReader)));

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            string directory = Path.Combine(testDirectory, validDatabase);

            // Call
            using (var reader = new HydraRingDatabaseReader(directory, "", 1))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(reader);
            }
        }

        [Test]
        public void Constructor_WorkingDirectoryNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new HydraRingDatabaseReader(null, "", 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("workingDirectory", exception.ParamName);
        }

        [Test]
        public void Constructor_QueryNull_ThrowArgumentNullException()
        {
            // Setup
            string directory = Path.Combine(testDirectory, validDatabase);

            // Call
            TestDelegate test = () => new HydraRingDatabaseReader(directory, null, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("query", exception.ParamName);
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Constructor_WithInvalidWorkingDirectoryPath_ThrowsArgumentException(string invalidPath)
        {
            // Call
            TestDelegate test = () => new HydraRingDatabaseReader(invalidPath, "", 0);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        [TestCase(emptyWorkingDirectory)]
        [TestCase(invalidDatabase)]
        public void Constructor_EmptyWorkingDirectoryOrInvalidDatabase_ThrowSQLiteException(string path)
        {
            // Setup
            string directory = Path.Combine(testDirectory, path);

            // Call
            TestDelegate test = () =>
            {
                using (new HydraRingDatabaseReader(directory, query, 0)) {}
            };

            // Assert
            Assert.Throws<SQLiteException>(test);
        }

        [Test]
        public void ReadLine_EmptyDatabase_ReturnsNull()
        {
            // Setup
            string directory = Path.Combine(testDirectory, emptyDatabase);

            using (var reader = new HydraRingDatabaseReader(directory, query, 1))
            {
                // Call
                Dictionary<string, object> result = reader.ReadLine();

                // Assert
                Assert.IsNull(result);
            }
        }

        [Test]
        public void NextResult_EmptyDatabase_ReturnsFalse()
        {
            // Setup
            string directory = Path.Combine(testDirectory, emptyDatabase);

            using (var reader = new HydraRingDatabaseReader(directory, query, 1))
            {
                // Call
                bool couldGetNextResult = reader.NextResult();

                // Assert
                Assert.IsFalse(couldGetNextResult);
            }
        }

        [Test]
        public void NextResult_SingleResult_ReturnsFalse()
        {
            // Setup
            string directory = Path.Combine(testDirectory, validDatabase);

            using (var reader = new HydraRingDatabaseReader(directory, query, 1))
            {
                // Call
                bool couldGetNextResult = reader.NextResult();

                // Assert
                Assert.IsFalse(couldGetNextResult);
            }
        }

        [Test]
        public void NextResult_MultipleResult_ReturnsTrue()
        {
            // Setup
            string directory = Path.Combine(testDirectory, validDatabase);

            using (var reader = new HydraRingDatabaseReader(directory, query + query, 1))
            {
                // Call
                bool couldGetNextResult = reader.NextResult();

                // Assert
                Assert.IsTrue(couldGetNextResult);
            }
        }

        [Test]
        public void NextResult_MultipleResultProceededToSecondResult_ReturnsFalse()
        {
            // Setup
            string directory = Path.Combine(testDirectory, validDatabase);

            using (var reader = new HydraRingDatabaseReader(directory, query + query, 1))
            {
                reader.NextResult();

                // Call
                bool couldGetNextResult = reader.NextResult();

                // Assert
                Assert.IsFalse(couldGetNextResult);
            }
        }
    }
}