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
using Migration.Console.TestUtil;
using NUnit.Framework;
using SystemConsole = System.Console;

namespace Migration.Console.Test
{
    [TestFixture]
    public class ConsoleHelperTest
    {
        [TearDown]
        public void TearDown()
        {
            SystemConsole.ResetColor();
        }

        [Test]
        public void WriteErrorLine_StringNull_ThrowsArgumentNullException()
        {
            // Setup
            ConsoleColor originalColor = SystemConsole.ForegroundColor;
            const string args = "an argument";

            // Call
            TestDelegate call = () => ConsoleHelper.WriteErrorLine(null, args);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("format", paramName);
            Assert.AreEqual(originalColor, SystemConsole.ForegroundColor);
        }

        [Test]
        public void WriteErrorLine_ArgsNull_ThrowsArgumentNullException()
        {
            // Setup
            ConsoleColor originalColor = SystemConsole.ForegroundColor;
            const string writeLine = "this is an error line with {0}";

            // Call
            TestDelegate call = () => ConsoleHelper.WriteErrorLine(writeLine, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("args", paramName);
            Assert.AreEqual(originalColor, SystemConsole.ForegroundColor);
        }

        [Test]
        public void WriteErrorLine_StringAndParamArgs_WritesExpectedLine()
        {
            // Setup
            ConsoleColor originalColor = SystemConsole.ForegroundColor;
            const string writeLine = "this is an error line with {0}";
            const string args = "an argument";
            string consoleText;

            // Call
            using (var consoleOutput = new ConsoleOutput())
            {
                ConsoleHelper.WriteErrorLine(writeLine, args);
                consoleText = consoleOutput.GetConsoleOutput();
            }

            // Assert
            Assert.AreEqual(string.Format(writeLine, args) + Environment.NewLine, consoleText);
            Assert.AreEqual(originalColor, SystemConsole.ForegroundColor);
        }

        [Test]
        public void WriteErrorLine_String_WritesExpectedLine()
        {
            // Setup
            ConsoleColor originalColor = SystemConsole.ForegroundColor;
            const string writeLine = "this is an error line";
            string consoleText;

            // Call
            using (var consoleOutput = new ConsoleOutput())
            {
                ConsoleHelper.WriteErrorLine(writeLine);
                consoleText = consoleOutput.GetConsoleOutput();
            }

            // Assert
            Assert.AreEqual(writeLine + Environment.NewLine, consoleText);
            Assert.AreEqual(originalColor, SystemConsole.ForegroundColor);
        }

        [Test]
        public void WriteErrorLine_InvalidString_ThrowsFormatException()
        {
            // Setup
            const string invalidFormat = "{d}";

            // Call
            TestDelegate call = () => ConsoleHelper.WriteErrorLine(invalidFormat, "ABC");

            // Assert
            Assert.Throws<FormatException>(call);
        }

        [Test]
        public void WriteCommandDescriptionLine_StringNull_ThrowsArgumentNullException()
        {
            // Setup
            const string args = "an argument";

            // Call
            TestDelegate call = () => ConsoleHelper.WriteCommandDescriptionLine(null, args);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("format", paramName);
        }

        [Test]
        public void WriteCommandDescriptionLine_ArgsNull_ThrowsArgumentNullException()
        {
            // Setup
            const string writeLine = "this is an error line with {0}";

            // Call
            TestDelegate call = () => ConsoleHelper.WriteCommandDescriptionLine(writeLine, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("args", paramName);
        }

        [Test]
        public void WriteCommandDescriptionLine_StringAndParamArgs_WritesExpectedLine()
        {
            // Setup
            const string writeLine = "this is an error line with {0}";
            const string args = "an argument";
            string consoleText;

            // Call
            using (var consoleOutput = new ConsoleOutput())
            {
                ConsoleHelper.WriteCommandDescriptionLine(writeLine, args);
                consoleText = consoleOutput.GetConsoleOutput();
            }

            // Assert
            string expectedText = string.Concat("          ", string.Format(writeLine, args),
                                                Environment.NewLine, Environment.NewLine);
            Assert.AreEqual(expectedText, consoleText);
        }

        [Test]
        public void WriteCommandDescriptionLine_String_WritesExpectedLine()
        {
            // Setup
            const string writeLine = "this is an error line";
            string consoleText;

            // Call
            using (var consoleOutput = new ConsoleOutput())
            {
                ConsoleHelper.WriteCommandDescriptionLine(writeLine);
                consoleText = consoleOutput.GetConsoleOutput();
            }

            // Assert
            string expectedText = string.Concat("          ", writeLine, Environment.NewLine, Environment.NewLine);
            Assert.AreEqual(expectedText, consoleText);
        }

        [Test]
        public void WriteCommandDescriptionLine_InvalidString_ThrowsFormatException()
        {
            // Setup
            const string invalidFormat = "{d}";

            // Call
            TestDelegate call = () => ConsoleHelper.WriteCommandDescriptionLine(invalidFormat, "ABC");

            // Assert
            Assert.Throws<FormatException>(call);
        }
    }
}