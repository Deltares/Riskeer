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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.DikeProfiles
{
    [TestFixture]
    public class ForeshoreProfileCollectionTest
        : CustomObservableUniqueItemCollectionWithSourcePathTestFixtureBase<ForeshoreProfileCollection, ForeshoreProfile>
    {
        protected override ForeshoreProfileCollection CreateCollection()
        {
            return new ForeshoreProfileCollection();
        }

        protected override IEnumerable<ForeshoreProfile> UniqueElements()
        {
            yield return new TestForeshoreProfile(string.Empty, "ForeshoreProfile ID A");
            yield return new TestForeshoreProfile(string.Empty, "ForeshoreProfile ID B");
        }

        protected override IEnumerable<ForeshoreProfile> SingleNonUniqueElements()
        {
            const string nonUniqueId = "ForeshoreProfile ID";

            yield return new TestForeshoreProfile(string.Empty, nonUniqueId);
            yield return new TestForeshoreProfile(string.Empty, nonUniqueId);
        }

        protected override IEnumerable<ForeshoreProfile> MultipleNonUniqueElements()
        {
            const string nonUniqueIdOne = "ForeshoreProfile ID";
            const string nonUniqueIdTwo = "Other ForeshoreProfile ID";

            yield return new TestForeshoreProfile(string.Empty, nonUniqueIdOne);
            yield return new TestForeshoreProfile(string.Empty, nonUniqueIdOne);
            yield return new TestForeshoreProfile(string.Empty, nonUniqueIdTwo);
            yield return new TestForeshoreProfile(string.Empty, nonUniqueIdTwo);
        }

        protected override void AssertSingleNonUniqueElements(ArgumentException exception, IEnumerable<ForeshoreProfile> itemsToAdd)
        {
            string someId = itemsToAdd.First().Id;
            Assert.AreEqual($"Voorlandprofielen moeten een unieke id hebben. Gevonden dubbele elementen: {someId}.", exception.Message);
        }

        protected override void AssertMultipleNonUniqueElements(ArgumentException exception, IEnumerable<ForeshoreProfile> itemsToAdd)
        {
            string someId = itemsToAdd.First().Id;
            string someotherId = itemsToAdd.First(i => i.Id != someId).Id;
            Assert.AreEqual($"Voorlandprofielen moeten een unieke id hebben. Gevonden dubbele elementen: {someId}, {someotherId}.", exception.Message);
        }
    }
}