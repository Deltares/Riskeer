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
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;

namespace Riskeer.Piping.Data.Test.SoilProfile
{
    [TestFixture]
    public class PipingStochasticSoilModelCollectionTest :
        CustomObservableUniqueItemCollectionWithSourcePathTestFixtureBase<
            ObservableUniqueItemCollectionWithSourcePath<PipingStochasticSoilModel>, PipingStochasticSoilModel>
    {
        protected override ObservableUniqueItemCollectionWithSourcePath<PipingStochasticSoilModel> CreateCollection()
        {
            return new PipingStochasticSoilModelCollection();
        }

        protected override IEnumerable<PipingStochasticSoilModel> UniqueElements()
        {
            yield return PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("Model A");
            yield return PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("Model B");
        }

        protected override IEnumerable<PipingStochasticSoilModel> SingleNonUniqueElements()
        {
            const string someName = "Soil model";
            yield return PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(someName);
            yield return PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(someName);
        }

        protected override IEnumerable<PipingStochasticSoilModel> MultipleNonUniqueElements()
        {
            const string someName = "Soil model";
            const string someOtherName = "Other soil model";
            yield return PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(someName);
            yield return PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(someName);
            yield return PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(someOtherName);
            yield return PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(someOtherName);
            yield return PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(someOtherName);
        }

        protected override void AssertSingleNonUniqueElements(ArgumentException exception, IEnumerable<PipingStochasticSoilModel> itemsToAdd)
        {
            string someName = itemsToAdd.First().Name;
            Assert.AreEqual("Stochastische ondergrondmodellen moeten een unieke naam hebben. " +
                            $"Gevonden dubbele elementen: {someName}.", exception.Message);
        }

        protected override void AssertMultipleNonUniqueElements(ArgumentException exception, IEnumerable<PipingStochasticSoilModel> itemsToAdd)
        {
            string someName = itemsToAdd.First().Name;
            string someOtherName = itemsToAdd.First(i => i.Name != someName).Name;
            Assert.AreEqual("Stochastische ondergrondmodellen moeten een unieke naam hebben. " +
                            $"Gevonden dubbele elementen: {someName}, {someOtherName}.", exception.Message);
        }
    }
}