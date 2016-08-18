﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Application.Ringtoets.Storage.Test
{
    [TestFixture]
    public class SqLiteConnectionStringBuilderTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void BuildSqLiteEntityConnectionString_InvalidPathToSqLiteFile_ThrowsArgumentNullException(string invalidPathToSqLiteFile)
        {
            // Call
            TestDelegate test = () => SqLiteConnectionStringBuilder.BuildSqLiteEntityConnectionString(invalidPathToSqLiteFile);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void BuildSqLiteEntityConnectionString_ValidPathToSqLiteFile_ValidConnectionString()
        {
            // Call
            var connectionString = SqLiteConnectionStringBuilder.BuildSqLiteEntityConnectionString(pathToSqLiteFile);

            // Assert
            Assert.IsNotNullOrEmpty(connectionString);
            StringAssert.Contains(string.Format("metadata=res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl;", "DbContext.RingtoetsEntities"), connectionString);
            StringAssert.Contains("provider=System.Data.SQLite.EF6", connectionString);
            StringAssert.Contains("failifmissing=True", connectionString);
            StringAssert.Contains(string.Format("data source={0}", pathToSqLiteFile), connectionString);
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
            var connectionString = SqLiteConnectionStringBuilder.BuildSqLiteEntityConnectionString(uncPathToSqlFile);

            // Assert
            Assert.IsNotNullOrEmpty(connectionString);
            StringAssert.Contains(string.Format("metadata=res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl;", "DbContext.RingtoetsEntities"), connectionString);
            StringAssert.Contains("provider=System.Data.SQLite.EF6", connectionString);
            StringAssert.Contains("failifmissing=True", connectionString);
            StringAssert.Contains(string.Format(@"data source=\\{0}", uncPathToSqlFile), connectionString);
            StringAssert.Contains("read only=False", connectionString);
            StringAssert.Contains("foreign keys=True", connectionString);
            StringAssert.Contains("version=3", connectionString);
            StringAssert.Contains("pooling=False", connectionString);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void BuildSqLiteConnectionString_InvalidPath_ThrowsArgumentNullException(string invalidPath)
        {
            // Call
            TestDelegate test = () => SqLiteConnectionStringBuilder.BuildSqLiteConnectionString(invalidPath);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void BuildSqLiteConnectionString_ValidPathToSqLiteFile_ValidConnectionString()
        {
            // Call
            var connectionString = SqLiteConnectionStringBuilder.BuildSqLiteConnectionString(pathToSqLiteFile);

            // Assert
            Assert.IsNotNullOrEmpty(connectionString);
            StringAssert.Contains(pathToSqLiteFile, connectionString);
            StringAssert.DoesNotContain("metadata=", connectionString);
            StringAssert.DoesNotContain("System.Data.SQLite.EF6", connectionString);
            StringAssert.Contains("failifmissing=True", connectionString);
            StringAssert.Contains(string.Format("data source={0}", pathToSqLiteFile), connectionString);
            StringAssert.Contains("read only=False", connectionString);
            StringAssert.Contains("foreign keys=True", connectionString);
            StringAssert.Contains("version=3", connectionString);
            StringAssert.Contains("pooling=False", connectionString);
        }

        [Test]
        public void BuildSqLiteConnectionString_UncPathToSqLiteFile_ValidConnectionString()
        {
            // Setup
            const string uncPathToSqlFile = @"\\server\share\file.sqlite";

            // Call
            var connectionString = SqLiteConnectionStringBuilder.BuildSqLiteConnectionString(uncPathToSqlFile);

            // Assert
            Assert.IsNotNullOrEmpty(connectionString);
            StringAssert.DoesNotContain("metadata=", connectionString);
            StringAssert.DoesNotContain("System.Data.SQLite.EF6", connectionString);
            StringAssert.Contains("failifmissing=True", connectionString);
            StringAssert.Contains(string.Format(@"data source={0}", uncPathToSqlFile), connectionString);
            StringAssert.Contains("read only=False", connectionString);
            StringAssert.Contains("foreign keys=True", connectionString);
            StringAssert.Contains("version=3", connectionString);
            StringAssert.Contains("pooling=False", connectionString);
        }

        private const string pathToSqLiteFile = @"C:\SqLiteFile.sqlite";
    }
}