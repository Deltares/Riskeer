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
        public void GetCalculationName_ValidNames_ExpectedValue(string name)
        {
            // Setup
            var provider = new GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider();

            // Call
            var calculationName = provider.GetCalculationName(name);

            // Assert
            var expectedName = string.Format("Golfhoogte bij doorsnede-eis voor locatie '{0}'", name);
            Assert.AreEqual(expectedName, calculationName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetActivityName_ValidNames_ExpectedValue(string name)
        {
            // Setup
            var provider = new GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider();

            // Call
            var activityName = provider.GetActivityName(name);

            // Assert
            var expectedName = string.Format("Golfhoogte bij doorsnede-eis berekenen voor locatie '{0}'", name);
            Assert.AreEqual(expectedName, activityName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("value")]
        public void GetCalculationFailedMessage_ValidNames_ExpectedValue(string name)
        {
            // Setup
            var provider = new GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider();

            // Call
            var message = provider.GetCalculationFailedMessage(name);

            // Assert
            var expectedMessage = string.Format("Er is een fout opgetreden tijdens de Golfhoogte bij " +
                                                "doorsnede-eis berekening '{0}': inspecteer het logbestand.", name);
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
            var message = provider.GetCalculatedNotConvergedMessage(name);

            // Assert
            var expectedMessage = string.Format("Golfhoogte bij doorsnede-eis berekening voor locatie '{0}' is niet geconvergeerd.", name);
            Assert.AreEqual(expectedMessage, message);
        }
    }
}