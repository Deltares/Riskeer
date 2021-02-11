﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

namespace Riskeer.Common.Service.Test.MessageProviders
{
    [TestFixture]
    public class WaveHeightCalculationMessageProviderTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_CategoryBoundaryNameInvalid_ThrowsArgumentException(string categoryBoundaryName)
        {
            // Call
            TestDelegate call = () => new WaveHeightCalculationMessageProvider(categoryBoundaryName);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("'categoryBoundaryName' must have a value.", exception.Message);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var provider = new WaveHeightCalculationMessageProvider("A");

            // Assert
            Assert.IsInstanceOf<ICalculationMessageProvider>(provider);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetActivityDescription_VariousParameters_ReturnsExpectedValue(string name)
        {
            // Setup
            const string categoryBoundaryName = "A";
            var provider = new WaveHeightCalculationMessageProvider(categoryBoundaryName);

            // Call
            string description = provider.GetActivityDescription(name);

            // Assert
            string expectedDescription = $"Golfhoogte berekenen voor locatie '{name}' (Categoriegrens {categoryBoundaryName})";
            Assert.AreEqual(expectedDescription, description);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetCalculationFailedMessage_VariousParameters_ReturnsExpectedValue(string name)
        {
            // Setup
            const string categoryBoundaryName = "A";
            var provider = new WaveHeightCalculationMessageProvider(categoryBoundaryName);

            // Call
            string message = provider.GetCalculationFailedMessage(name);

            // Assert
            string expectedMessage = $"Er is een fout opgetreden tijdens de golfhoogte berekening voor locatie '{name}' (Categoriegrens {categoryBoundaryName}). " +
                                     "Er is geen foutrapport beschikbaar.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetCalculationFailedWithErrorReportMessage_ValidNames_ExpectedValues(string name)
        {
            // Setup
            const string categoryBoundaryName = "A";
            var provider = new WaveHeightCalculationMessageProvider(categoryBoundaryName);
            const string failureMessage = "It failed";

            // Call
            string message = provider.GetCalculationFailedWithErrorReportMessage(name, failureMessage);

            // Assert
            string expectedMessage = $"Er is een fout opgetreden tijdens de golfhoogte berekening voor locatie '{name}' (Categoriegrens {categoryBoundaryName}). " +
                                     $"Bekijk het foutrapport door op details te klikken.{Environment.NewLine}{failureMessage}";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetCalculationFailedWithErrorReportMessage_ValidFailureMessages_ExpectedValues(string failureMessage)
        {
            // Setup
            const string categoryBoundaryName = "A";
            var provider = new WaveHeightCalculationMessageProvider(categoryBoundaryName);
            const string name = "calculation name";

            // Call
            string message = provider.GetCalculationFailedWithErrorReportMessage(name, failureMessage);

            // Assert
            string expectedMessage = $"Er is een fout opgetreden tijdens de golfhoogte berekening voor locatie '{name}' (Categoriegrens {categoryBoundaryName}). " +
                                     $"Bekijk het foutrapport door op details te klikken.{Environment.NewLine}{failureMessage}";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetCalculatedNotConvergedMessage_VariousParameters_ReturnsExpectedValue(string name)
        {
            // Setup
            const string categoryBoundaryName = "A";
            var provider = new WaveHeightCalculationMessageProvider(categoryBoundaryName);

            // Call
            string message = provider.GetCalculatedNotConvergedMessage(name);

            // Assert
            string expectedMessage = $"Golfhoogte berekening voor locatie '{name}' (Categoriegrens {categoryBoundaryName}) is niet geconvergeerd.";
            Assert.AreEqual(expectedMessage, message);
        }
    }
}