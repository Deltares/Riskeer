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
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.DikeProfiles
{
    [TestFixture]
    public class DikeProfileCollectionTest
    {
        [Test]
        public void DefaultConstructor_ReturnsCollectionWithPath()
        {
            // Call
            var collection = new DikeProfileCollection();

            // Assert
            Assert.IsInstanceOf<ObservableUniqueItemCollectionWithSourcePath<DikeProfile>>(collection);
        }

        [Test]
        public void AddRange_DikeProfilesWithDifferentIds_AddsDikeProfiles()
        {
            // Setup
            var dikeProfilesToAdd = new[]
            {
                new TestDikeProfile(string.Empty, "Dike ID A"),
                new TestDikeProfile(string.Empty, "Dike ID B")
            };

            var collection = new DikeProfileCollection();
            const string expectedFilePath = "other/path";

            // Call
            collection.AddRange(dikeProfilesToAdd, expectedFilePath);

            // Assert
            Assert.AreEqual(expectedFilePath, collection.SourcePath);
            CollectionAssert.AreEqual(dikeProfilesToAdd, collection);
        }

        [Test]
        public void AddRange_WithDikeProfilesWithEqualIds_ThrowsArgumentException()
        {
            // Setup
            var collection = new DikeProfileCollection();
            const string someId = "Dike profile";
            const string name = "Standard Dike Profile Name";
            var modelsToAdd = new[]
            {
                new TestDikeProfile(name, someId),
                new TestDikeProfile(name, someId)
            };

            // Call
            TestDelegate call = () => collection.AddRange(modelsToAdd, "valid/file/path");

            // Assert
            string message = $"Dijkprofielen moeten een unieke id hebben. Gevonden dubbele elementen: {someId}.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message);
        }

        [Test]
        public void AddRange_WithMultipleDikeProfilesWithEqualIds_ThrowsArgumentException()
        {
            // Setup
            var collection = new DikeProfileCollection();
            const string someId = "Dike profile";
            const string someotherId = "Other dike profile";
            const string name = "Some Dike profile Name";
            var modelsToAdd = new[]
            {
                new TestDikeProfile(name, someId),
                new TestDikeProfile(name, someId),
                new TestDikeProfile(name, someotherId),
                new TestDikeProfile(name, someotherId)
            };

            // Call
            TestDelegate call = () => collection.AddRange(modelsToAdd, "valid/file/path");

            // Assert
            string message = $"Dijkprofielen moeten een unieke id hebben. Gevonden dubbele elementen: {someId}, {someotherId}.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message);
        }
    }
}