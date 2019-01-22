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
using NUnit.Framework;

namespace Ringtoets.Storage.Core.Test
{
    [TestFixture]
    public class SqLiteEntityConnectionStringBuilderTest
    {
        private const string pathToSqLiteFile = @"C:\SqLiteFile.sqlite";

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void BuildSqLiteEntityConnectionString_InvalidPathToSqLiteFile_ThrowsArgumentNullException(string invalidPathToSqLiteFile)
        {
            // Call
            TestDelegate test = () => SqLiteEntityConnectionStringBuilder.BuildSqLiteEntityConnectionString(
                invalidPathToSqLiteFile);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void BuildSqLiteEntityConnectionString_ValidPathToSqLiteFile_ValidConnectionString()
        {
            // Call
            string connectionString = SqLiteEntityConnectionStringBuilder.BuildSqLiteEntityConnectionString(
                pathToSqLiteFile);

            // Assert
            Assert.That(!string.IsNullOrEmpty(connectionString));
            StringAssert.Contains(string.Format("metadata=res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl;",
                                                "DbContext.RiskeerEntities"), connectionString);
            StringAssert.Contains("provider=System.Data.SQLite.EF6", connectionString);
            StringAssert.Contains("failifmissing=True", connectionString);
            StringAssert.Contains($"data source={pathToSqLiteFile}", connectionString);
            StringAssert.Contains("read only=False", connectionString);
            StringAssert.Contains("foreign keys=True", connectionString);
            StringAssert.Contains("version=3", connectionString);
            StringAssert.Contains("pooling=False", connectionString);
        }

        [Test]
        public void BuildSqLiteEntityConnectionString_ValidUncPathToSqLiteFile_ValidConnectionString()
        {
            // Setup
            const string uncPathToSqlFile = @"\\server\share\file.sqlite";

            // Call
            string connectionString = SqLiteEntityConnectionStringBuilder.BuildSqLiteEntityConnectionString(
                uncPathToSqlFile);

            // Assert
            Assert.That(!string.IsNullOrEmpty(connectionString));
            StringAssert.Contains(string.Format("metadata=res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl;",
                                                "DbContext.RiskeerEntities"), connectionString);
            StringAssert.Contains("provider=System.Data.SQLite.EF6", connectionString);
            StringAssert.Contains("failifmissing=True", connectionString);
            StringAssert.Contains($@"data source=\\{uncPathToSqlFile}", connectionString);
            StringAssert.Contains("read only=False", connectionString);
            StringAssert.Contains("foreign keys=True", connectionString);
            StringAssert.Contains("version=3", connectionString);
            StringAssert.Contains("pooling=False", connectionString);
        }
    }
}