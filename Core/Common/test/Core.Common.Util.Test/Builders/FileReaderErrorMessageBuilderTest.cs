// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using Core.Common.Util.Builders;
using NUnit.Framework;

namespace Core.Common.Util.Test.Builders
{
    [TestFixture]
    public class FileReaderErrorMessageBuilderTest
    {
        [Test]
        public void Build_BasedOnPathAndMessage_ReturnBuiltErrorMessage()
        {
            // Setup
            const string filePath = "<file path>";
            const string errorMessage = "test test 1,2,3";

            // Call
            string message = new FileReaderErrorMessageBuilder(filePath).Build(errorMessage);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': {errorMessage}";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Build_BasedOnPathAndMessageStartingWithUpperCase_ReturnWithInnerErrorMessageStartingWithLowercase()
        {
            // Setup
            const string filePath = "<file path>";

            // Call
            string message = new FileReaderErrorMessageBuilder(filePath).Build("Test TEst 1,2,3");

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand \'{filePath}\': test TEst 1,2,3";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Build_BasedOnPathAndMessageWithLocation_ReturnBuiltErrorMessage()
        {
            // Setup
            const string filePath = "<file path>";
            const string errorMessage = "test test 1,2,3";
            const string location = "<location description>";

            // Call
            string message = new FileReaderErrorMessageBuilder(filePath).WithLocation(location).Build(errorMessage);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}' {location}: {errorMessage}";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Build_BasedOnPathAndMessageWithSubject_ReturnBuiltErrorMessage()
        {
            // Setup
            const string filePath = "<file path>";
            const string errorMessage = "test test 1,2,3";
            const string subject = "<subject description>";

            // Call
            string message = new FileReaderErrorMessageBuilder(filePath).WithSubject(subject).Build(errorMessage);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}' ({subject}): {errorMessage}";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Build_BasedOnPathAndMessageWithSubjectAndLocation_ReturnBuiltErrorMessage()
        {
            // Setup
            const string filePath = "<file path>";
            const string errorMessage = "test test 1,2,3";
            const string subject = "<subject description>";
            const string location = "<location description>";

            // Call
            string message = new FileReaderErrorMessageBuilder(filePath).WithSubject(subject).WithLocation(location).Build(errorMessage);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}' {location} ({subject}): {errorMessage}";
            Assert.AreEqual(expectedMessage, message);
        }
    }
}