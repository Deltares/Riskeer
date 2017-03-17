﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.IO.FileImporters.MessageProviders;

namespace Ringtoets.Common.IO.Test.FileImporters.MessageProviders
{
    [TestFixture]
    public class UpdateMessageProviderTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var messageProvider = new UpdateMessageProvider();

            // Assert
            Assert.IsInstanceOf<IImporterMessageProvider>(messageProvider);
        }

        [Test]
        public void GetAddDataToModelProgressText_Always_ReturnsExpectedMessage()
        {
            // Setup
            var messageProvider = new UpdateMessageProvider();

            // Call
            string message = messageProvider.GetAddDataToModelProgressText();

            // Assert
            const string expectedMessage = "Bijwerken data in het toetsspoor.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void GetCancelledLogMessageText_DescriptionNull_ThrowsArgumentNullException()
        {
            // Setup
            var messageProvider = new UpdateMessageProvider();

            // Call
            TestDelegate call = () => messageProvider.GetCancelledLogMessageText(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("typeDescriptor", paramName);
        }

        [Test]
        public void GetCancelledLogMessageText_WithDescription_ReturnsExpectedMessage()
        {
            // Setup
            const string typeDescriptor = "Items";
            var messageProvider = new UpdateMessageProvider();

            // Call
            string message = messageProvider.GetCancelledLogMessageText(typeDescriptor);

            // Assert
            const string expectedMessage = "Items bijwerken afgebroken. Geen data ingelezen.";
            Assert.AreEqual(expectedMessage, message);
        }
    }
}