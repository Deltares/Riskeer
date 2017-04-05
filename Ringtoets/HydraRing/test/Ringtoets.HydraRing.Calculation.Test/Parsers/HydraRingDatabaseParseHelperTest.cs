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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.Parsers;

namespace Ringtoets.HydraRing.Calculation.Test.Parsers
{
    [TestFixture]
    public class HydraRingDatabaseParseHelperTest
    {
        private const string emptyWorkingDirectory = "EmptyWorkingDirectory";
        private const string emptyDatabase = "EmptyDatabase";
        private const string validFile = "ValidFile";

        private const string query = "SELECT * FROM IterateToGivenBetaConvergence " +
                                     "WHERE OuterIterationId = (SELECT MAX(OuterIterationId) FROM IterateToGivenBetaConvergence);";

        private static readonly string testDirectory = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation,
                                                                                  Path.Combine("Readers", nameof(HydraRingDatabaseParseHelper)));

        [Test]
        public void Parse_WorkingDirectoryNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => HydraRingDatabaseParseHelper.Parse(null, "", 0, "", reader => { });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("workingDirectory", exception.ParamName);
        }

        [Test]
        public void Parse_QueryNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => HydraRingDatabaseParseHelper.Parse("", null, 0, "", reader => { });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("query", exception.ParamName);
        }

        [Test]
        public void Parse_ExceptionMessageNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => HydraRingDatabaseParseHelper.Parse("", "", 0, null, reader => { });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("exceptionMessage", exception.ParamName);
        }

        [Test]
        public void Parse_ReadResultActionNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => HydraRingDatabaseParseHelper.Parse("", "", 0, "", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("readResultAction", exception.ParamName);
        }

        [Test]
        public void Parse_ReaderThrowsSQLiteException_ThrowHydraRingFileParserException()
        {
            // Setup
            string directory = Path.Combine(testDirectory, emptyWorkingDirectory);

            // Call
            TestDelegate test = () => HydraRingDatabaseParseHelper.Parse(directory, query, 0, "", reader => { });

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.AreEqual("Er kon geen resultaat gelezen worden uit de Hydra-Ring uitvoerdatabase.", exception.Message);
        }

        [Test]
        public void Parse_ReaderThrowsHydraRingDatabaseReaderException_ThrowHydraRingFileParserExceptionWithCustomMessage()
        {
            // Setup
            const string customMessage = "Exception message";
            string directory = Path.Combine(testDirectory, emptyDatabase);

            // Call
            TestDelegate test = () => HydraRingDatabaseParseHelper.Parse(directory, query, 0, customMessage, reader => { });

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.AreEqual(customMessage, exception.Message);
        }

        [Test]
        public void Parse_ReadResultActionThrowsInvalidCastException_ThrowHydraRingFileParserExceptionWithCustomMessage()
        {
            // Setup
            const string customMessage = "Exception message";
            string directory = Path.Combine(testDirectory, validFile);

            // Call
            TestDelegate test = () => HydraRingDatabaseParseHelper.Parse(directory, query, 0, customMessage, reader => { throw new InvalidCastException(); });

            // Assert
            var exception = Assert.Throws<HydraRingFileParserException>(test);
            Assert.AreEqual(customMessage, exception.Message);
        }

        [Test]
        public void Parse_ValidData_ReadResultActionPerformed()
        {
            // Setup
            string directory = Path.Combine(testDirectory, validFile);
            var performed = false;

            // Call
            HydraRingDatabaseParseHelper.Parse(directory, query, 0, "", reader => { performed = true; });

            // Assert
            Assert.IsTrue(performed);
        }
    }
}