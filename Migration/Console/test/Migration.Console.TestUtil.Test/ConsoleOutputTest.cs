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
using System.IO;
using NUnit.Framework;
using SystemConsole = System.Console;

namespace Migration.Console.TestUtil.Test
{
    [TestFixture]
    public class ConsoleOutputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            TextWriter originalWriter = SystemConsole.Out;
            TextWriter consoleOutputwriter;

            // Call
            using (new ConsoleOutput())
            {
                consoleOutputwriter = SystemConsole.Out;
            }

            // Assert
            Assert.AreNotSame(consoleOutputwriter, SystemConsole.Out);
            Assert.AreSame(originalWriter, SystemConsole.Out);
        }

        [Test]
        public void GetConsoleOutput_TextSentToConsole_ReturnsCapturedText()
        {
            // Setup
            const string lineOne = "first line.";
            const string lineTwo = "second line.";
            SystemConsole.WriteLine(@"Before");
            string capturedText;

            // Call
            using (var consoleOutput = new ConsoleOutput())
            {
                SystemConsole.WriteLine(lineOne);
                SystemConsole.WriteLine(lineTwo);

                capturedText = consoleOutput.GetConsoleOutput();
            }

            // Assert
            SystemConsole.WriteLine(@"After");
            Assert.AreEqual(string.Concat(
                                lineOne, Environment.NewLine,
                                lineTwo, Environment.NewLine
                            ), capturedText);
        }

        [Test]
        public void Dispose_AlreadyDisposed_DoesNotThrowException()
        {
            // Call
            TestDelegate call = () =>
            {
                using (var consoleOutput = new ConsoleOutput())
                {
                    consoleOutput.Dispose();
                }
            };

            // Assert
            Assert.DoesNotThrow(call);
        }
    }
}