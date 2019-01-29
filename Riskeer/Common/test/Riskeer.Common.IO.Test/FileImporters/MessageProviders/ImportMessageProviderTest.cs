// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.Common.IO.FileImporters.MessageProviders;

namespace Riskeer.Common.IO.Test.FileImporters.MessageProviders
{
    [TestFixture]
    public class ImportMessageProviderTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var messageProvider = new ImportMessageProvider();

            // Assert
            Assert.IsInstanceOf<IImporterMessageProvider>(messageProvider);
        }

        [Test]
        public void GetAddDataToModelProgressText_Always_ReturnsExpectedMessage()
        {
            // Setup
            var messageProvider = new ImportMessageProvider();

            // Call
            string message = messageProvider.GetAddDataToModelProgressText();

            // Assert
            const string expectedMessage = "Geïmporteerde data toevoegen aan het toetsspoor.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void GetCancelledLogMessageText_DescriptorNull_ThrowsArgumentNullException()
        {
            // Setup
            var messageProvider = new ImportMessageProvider();

            // Call
            TestDelegate call = () => messageProvider.GetCancelledLogMessageText(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("typeDescriptor", paramName);
        }

        [Test]
        public void GetCancelledLogMessageText_WithDescriptor_ReturnsExpectedMessage()
        {
            // Setup
            const string typeDescriptor = "Items";
            var messageProvider = new ImportMessageProvider();

            // Call
            string message = messageProvider.GetCancelledLogMessageText(typeDescriptor);

            // Assert
            const string expectedMessage = "Items importeren afgebroken. Geen gegevens gewijzigd.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void GetUpdateDataFailedLogMessageText_DescriptorNull_ThrowsArgumentNullException()
        {
            // Setup
            var messageProvider = new ImportMessageProvider();

            // Call
            TestDelegate call = () => messageProvider.GetUpdateDataFailedLogMessageText(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("typeDescriptor", paramName);
        }

        [Test]
        public void GetUpdateDataFailedLogMessageText_WithDescriptor_ReturnsExpectedMessage()
        {
            // Setup
            const string typeDescriptor = "Items";
            var messageProvider = new ImportMessageProvider();

            // Call
            string message = messageProvider.GetUpdateDataFailedLogMessageText(typeDescriptor);

            // Assert
            const string expectedMessage = "Het importeren van de items is mislukt: {0}. Geen gegevens gewijzigd.";
            Assert.AreEqual(expectedMessage, message);
        }
    }
}