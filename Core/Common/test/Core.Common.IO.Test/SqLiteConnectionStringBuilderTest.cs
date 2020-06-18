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
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.IO.Test
{
    [TestFixture]
    public class SqLiteConnectionStringBuilderTest
    {
        private const string pathToSqLiteFile = @"C:\validPath.sqlite";

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void BuildSqLiteConnectionString_InvalidPath_ThrowsArgumentNullException(string invalidPath)
        {
            // Call
            void Call() => SqLiteConnectionStringBuilder.BuildSqLiteConnectionString(invalidPath, true);

            // Assert
            Assert.Throws<ArgumentNullException>(Call);
        }

        [Test]
        public void BuildSqLiteConnectionString_ValidPathToSqLiteFile_ValidConnectionString()
        {
            // Call
            bool readOnly = new Random(643).NextBoolean();
            string connectionString = SqLiteConnectionStringBuilder.BuildSqLiteConnectionString(pathToSqLiteFile, readOnly);

            // Assert
            Assert.That(!string.IsNullOrEmpty(connectionString));
            StringAssert.Contains(pathToSqLiteFile, connectionString);
            StringAssert.DoesNotContain("metadata=", connectionString);
            StringAssert.DoesNotContain("System.Data.SQLite.EF6", connectionString);
            StringAssert.Contains("failifmissing=True", connectionString);
            StringAssert.Contains($"data source={pathToSqLiteFile}", connectionString);
            StringAssert.Contains($"read only={readOnly}", connectionString);
            StringAssert.Contains("foreign keys=True", connectionString);
            StringAssert.Contains("version=3", connectionString);
            StringAssert.Contains("pooling=False", connectionString);
            StringAssert.Contains("journal mode=Off", connectionString);
            StringAssert.Contains("synchronous=Full", connectionString);
        }

        [Test]
        public void BuildSqLiteConnectionString_UncPathToSqLiteFile_ValidConnectionString()
        {
            // Setup
            bool readOnly = new Random(645).NextBoolean();
            const string uncPathToSqlFile = @"\\server\share\file.sqlite";

            // Call
            string connectionString = SqLiteConnectionStringBuilder.BuildSqLiteConnectionString(uncPathToSqlFile, readOnly);

            // Assert
            Assert.That(!string.IsNullOrEmpty(connectionString));
            StringAssert.DoesNotContain("metadata=", connectionString);
            StringAssert.DoesNotContain("System.Data.SQLite.EF6", connectionString);
            StringAssert.Contains("failifmissing=True", connectionString);
            StringAssert.Contains($@"data source={uncPathToSqlFile}", connectionString);
            StringAssert.Contains($"read only={readOnly}", connectionString);
            StringAssert.Contains("foreign keys=True", connectionString);
            StringAssert.Contains("version=3", connectionString);
            StringAssert.Contains("pooling=False", connectionString);
            StringAssert.Contains("journal mode=Off", connectionString);
            StringAssert.Contains("synchronous=Full", connectionString);
        }
    }
}