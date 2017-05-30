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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class StochasticSoilModelCollectionTest :
        CustomObservableUniqueItemCollectionWithSourcePathTestFixtureBase<
            ObservableUniqueItemCollectionWithSourcePath<StochasticSoilModel>, StochasticSoilModel>
    {
        protected override ObservableUniqueItemCollectionWithSourcePath<StochasticSoilModel> CreateCollection()
        {
            return new StochasticSoilModelCollection();
        }

        protected override IEnumerable<StochasticSoilModel> UniqueElements()
        {
            yield return new StochasticSoilModel(5, "Model A", "segmentA");
            yield return new StochasticSoilModel(6, "Model B", "segmentA");
        }

        protected override IEnumerable<StochasticSoilModel> SingleNonUniqueElements()
        {
            const string someName = "Soil model";
            yield return new StochasticSoilModel(5, someName, "segmentA");
            yield return new StochasticSoilModel(6, someName, "segmentB");
        }

        protected override IEnumerable<StochasticSoilModel> MultipleNonUniqueElements()
        {
            const string someName = "Soil model";
            const string someOtherName = "Other soil model";
            yield return new StochasticSoilModel(5, someName, "segmentA");
            yield return new StochasticSoilModel(6, someName, "segmentB");
            yield return new StochasticSoilModel(7, someOtherName, "segmentC");
            yield return new StochasticSoilModel(8, someOtherName, "segmentD");
            yield return new StochasticSoilModel(9, someOtherName, "segmentE");
        }

        protected override void AssertSingleNonUniqueElements(ArgumentException exception, IEnumerable<StochasticSoilModel> itemsToAdd)
        {
            string someName = itemsToAdd.First().Name;
            Assert.AreEqual("Stochastische ondergrondmodellen moeten een unieke naam hebben. " +
                            $"Gevonden dubbele elementen: {someName}.", exception.Message);
        }

        protected override void AssertMultipleNonUniqueElements(ArgumentException exception, IEnumerable<StochasticSoilModel> itemsToAdd)
        {
            string someName = itemsToAdd.First().Name;
            string someOtherName = itemsToAdd.First(i => i.Name != someName).Name;
            Assert.AreEqual("Stochastische ondergrondmodellen moeten een unieke naam hebben. " +
                            $"Gevonden dubbele elementen: {someName}, {someOtherName}.", exception.Message);
        }
    }
}