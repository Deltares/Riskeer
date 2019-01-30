// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Service.MessageProviders;

namespace Riskeer.StabilityPointStructures.Service.Test
{
    [TestFixture]
    public class StabilityPointStructuresCalculationMessageProviderTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var messageProvider = new StabilityPointStructuresCalculationMessageProvider();

            // Assert
            Assert.IsInstanceOf<IStructuresCalculationMessageProvider>(messageProvider);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetCalculationFailedMessage_VariousParameters_ReturnsExpectedValue(string name)
        {
            // Setup
            var provider = new StabilityPointStructuresCalculationMessageProvider();

            // Call
            string activityDescription = provider.GetCalculationFailedMessage(name);

            // Assert
            string expectedName = $"De berekening voor kunstwerk puntconstructies '{name}' is mislukt. Er is geen foutrapport beschikbaar.";
            Assert.AreEqual(expectedName, activityDescription);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetCalculationFailedWithErrorReportMessage_VariousParameters_ReturnsExpectedValue(string name)
        {
            // Setup
            var provider = new StabilityPointStructuresCalculationMessageProvider();
            const string failureMessage = "It failed!";

            // Call
            string message = provider.GetCalculationFailedWithErrorReportMessage(name, failureMessage);

            // Assert
            string expectedMessage = $"De berekening voor kunstwerk puntconstructies '{name}' is mislukt. " +
                                     $"Bekijk het foutrapport door op details te klikken.{Environment.NewLine}{failureMessage}";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetCalculationPerformedMessage_VariousParameters_ReturnsExpectedValue(string directory)
        {
            // Setup
            var provider = new StabilityPointStructuresCalculationMessageProvider();

            // Call
            string message = provider.GetCalculationPerformedMessage(directory);

            // Assert
            string expectedMessage = $"Puntconstructies berekening is uitgevoerd op de tijdelijke locatie '{directory}'. " +
                                     "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.";
            Assert.AreEqual(expectedMessage, message);
        }
    }
}