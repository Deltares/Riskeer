// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.GrassCoverErosionOutwards.Service.MessageProviders;

namespace Ringtoets.GrassCoverErosionOutwards.Service.Test.MessageProviders
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveHeightCalculationMessageProviderTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup & Call
            var provider = new GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider();

            // Assert
            Assert.IsInstanceOf<ICalculationMessageProvider>(provider);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetActivityDescription_ValidNames_ExpectedValue(string name)
        {
            // Setup
            var provider = new GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider();

            // Call
            string activityDescription = provider.GetActivityDescription(name);

            // Assert
            string expectedName = $"Golfhoogte bij doorsnede-eis berekenen voor locatie '{name}'";
            Assert.AreEqual(expectedName, activityDescription);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetCalculationFailedMessage_ValidNames_ExpectedValue(string name)
        {
            // Setup
            var provider = new GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider();
            const string failureMessage = "It failed!";

            // Call
            string message = provider.GetCalculationFailedMessage(name, failureMessage);

            // Assert
            string expectedMessage = $"Er is een fout opgetreden tijdens de Golfhoogte bij doorsnede-eis berekening '{name}'. " +
                                     $"Bekijk het foutrapport door op details te klikken.{Environment.NewLine}{failureMessage}";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetCalculatedNotConvergedMessage_ValidNames_ExpectedValue(string name)
        {
            // Setup
            var provider = new GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider();

            // Call
            string message = provider.GetCalculatedNotConvergedMessage(name);

            // Assert
            string expectedMessage = $"Golfhoogte bij doorsnede-eis berekening voor locatie '{name}' is niet geconvergeerd.";
            Assert.AreEqual(expectedMessage, message);
        }
    }
}