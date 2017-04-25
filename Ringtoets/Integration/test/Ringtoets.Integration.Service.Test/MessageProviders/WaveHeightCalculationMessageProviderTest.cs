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
using NUnit.Framework;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.Integration.Service.MessageProviders;

namespace Ringtoets.Integration.Service.Test.MessageProviders
{
    [TestFixture]
    public class WaveHeightCalculationMessageProviderTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup & Call
            var provider = new WaveHeightCalculationMessageProvider();

            // Assert
            Assert.IsInstanceOf<ICalculationMessageProvider>(provider);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetCalculationName_VariousParameters_ReturnsExpectedValue(string name)
        {
            // Setup
            var provider = new WaveHeightCalculationMessageProvider();

            // Call
            string calculationName = provider.GetCalculationName(name);

            // Assert
            string expectedName = string.Format("Golfhoogte berekenen voor locatie '{0}'", name);
            Assert.AreEqual(expectedName, calculationName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetActivityName_VariousParameters_ReturnsExpectedValue(string name)
        {
            // Setup
            var provider = new WaveHeightCalculationMessageProvider();

            // Call
            string activityName = provider.GetActivityName(name);

            // Assert
            string expectedName = string.Format("Golfhoogte berekenen voor locatie '{0}'", name);
            Assert.AreEqual(expectedName, activityName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetCalculationFailedUnexplainedMessage_VariousParameters_ReturnsExpectedValue(string name)
        {
            // Setup
            var provider = new WaveHeightCalculationMessageProvider();

            // Call
            string message = provider.GetCalculationFailedUnexplainedMessage(name);

            // Assert
            string expectedMessage = string.Format("Er is een fout opgetreden tijdens de golfhoogte berekening '{0}'. Er is geen foutrapport beschikbaar.", name);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetCalculationFailedMessage_VariousParameters_ReturnsExpectedValue(string name)
        {
            // Setup
            var provider = new WaveHeightCalculationMessageProvider();
            const string failureMessage = "It failed!";

            // Call
            string message = provider.GetCalculationFailedMessage(name, failureMessage);

            // Assert
            string expectedMessage = $"Er is een fout opgetreden tijdens de golfhoogte berekening '{name}'. Bekijk het foutrapport door op details te klikken.{Environment.NewLine}{failureMessage}";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetCalculatedNotConvergedMessage_VariousParameters_ReturnsExpectedValue(string name)
        {
            // Setup
            var provider = new WaveHeightCalculationMessageProvider();

            // Call
            string message = provider.GetCalculatedNotConvergedMessage(name);

            // Assert
            string expectedMessage = string.Format("Golfhoogte berekening voor locatie {0} is niet geconvergeerd.", name);
            Assert.AreEqual(expectedMessage, message);
        }
    }
}