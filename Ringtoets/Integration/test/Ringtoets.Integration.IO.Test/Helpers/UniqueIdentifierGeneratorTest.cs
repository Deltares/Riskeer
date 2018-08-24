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

using NUnit.Framework;
using Ringtoets.Integration.IO.Helpers;

namespace Ringtoets.Integration.IO.Test.Helpers
{
    [TestFixture]
    public class UniqueIdentifierGeneratorTest
    {
        [Test]
        public void GetNewId_Always_ReturnsExpectedValue()
        {
            // Setup
            var generator = new UniqueIdentifierGenerator();

            // Call
            string id = generator.GetNewId();

            // Assert
            Assert.AreEqual("0", id);
        }

        [Test]
        public void GivenGeneratedId_WhenGetNewIdCalled_ThenNewIdGenerated()
        {
            // Given
            var generator = new UniqueIdentifierGenerator();
            string currentId = generator.GetNewId();

            // Precondition
            Assert.AreEqual("0", currentId);

            // When
            string[] generatedIds =
            {
                generator.GetNewId(),
                generator.GetNewId()
            };

            // Then
            CollectionAssert.AreEqual(new[]
            {
                "1",
                "2"
            }, generatedIds);
        }
    }
}