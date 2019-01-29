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
using Core.Common.Base;
using NUnit.Framework;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.DikeProfiles
{
    [TestFixture]
    public class DikeProfileCollectionTest :
        CustomObservableUniqueItemCollectionWithSourcePathTestFixtureBase<
            ObservableUniqueItemCollectionWithSourcePath<DikeProfile>, DikeProfile>
    {
        protected override ObservableUniqueItemCollectionWithSourcePath<DikeProfile> CreateCollection()
        {
            return new DikeProfileCollection();
        }

        protected override IEnumerable<DikeProfile> UniqueElements()
        {
            yield return DikeProfileTestFactory.CreateDikeProfile(string.Empty, "Dike ID A");
            yield return DikeProfileTestFactory.CreateDikeProfile(string.Empty, "Dike ID B");
        }

        protected override IEnumerable<DikeProfile> SingleNonUniqueElements()
        {
            const string someId = "Dike profile";
            yield return DikeProfileTestFactory.CreateDikeProfile("Standard Dike Profile Name", someId);
            yield return DikeProfileTestFactory.CreateDikeProfile("Other Dike Profile Name", someId);
        }

        protected override void AssertSingleNonUniqueElements(ArgumentException exception, IEnumerable<DikeProfile> itemsToAdd)
        {
            string someId = itemsToAdd.First().Id;
            Assert.AreEqual($"Dijkprofielen moeten een unieke id hebben. Gevonden dubbele elementen: {someId}.", exception.Message);
        }

        protected override IEnumerable<DikeProfile> MultipleNonUniqueElements()
        {
            const string someId = "Dike profile";
            const string someotherId = "Other dike profile";

            yield return DikeProfileTestFactory.CreateDikeProfile("Dike profile Name 1", someId);
            yield return DikeProfileTestFactory.CreateDikeProfile("Dike profile Name 2", someId);
            yield return DikeProfileTestFactory.CreateDikeProfile("Dike profile Name 3", someotherId);
            yield return DikeProfileTestFactory.CreateDikeProfile("Dike profile Name 4", someotherId);
        }

        protected override void AssertMultipleNonUniqueElements(ArgumentException exception, IEnumerable<DikeProfile> itemsToAdd)
        {
            string someId = itemsToAdd.First().Id;
            string someotherId = itemsToAdd.First(i => i.Id != someId).Id;
            Assert.AreEqual($"Dijkprofielen moeten een unieke id hebben. Gevonden dubbele elementen: {someId}, {someotherId}.", exception.Message);
        }
    }
}