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

using System.IO;
using Core.Common.Util.Builders;
using NUnit.Framework;

namespace Core.Common.Util.Test.Builders
{
    [TestFixture]
    public class DirectoryWriterErrorMessageBuilderTest
    {
        [Test]
        public void Build_WithMessage_ReturnsDirectoryWriterErrorMessage()
        {
            // Setup
            string folderPath = Directory.GetCurrentDirectory();
            const string customMessage = "<Some custom message>";

            // Call
            string message = new DirectoryWriterErrorMessageBuilder(folderPath).Build(customMessage);

            // Assert
            string expectedMessage = $"Fout bij het schrijven naar bestandsmap '{folderPath}': {customMessage}";
            Assert.AreEqual(expectedMessage, message);
        }
    }
}