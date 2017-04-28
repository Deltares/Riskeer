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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.IO.Importers;

namespace Ringtoets.Piping.IO.TestUtil.Test
{
    [TestFixture]
    public class TestStochasticSoilModelUpdateModelStrategyTest
    {
        [Test]
        public void DefaultConstructor_CreatesNewInstance()
        {
            // Call
            var strategy = new TestStochasticSoilModelUpdateModelStrategy();

            // Assert
            Assert.IsInstanceOf<IStochasticSoilModelUpdateModelStrategy>(strategy);
        }

        [Test]
        public void UpdateModelWithImportedData_WithoutSettingUpdatedInstances_SetPropertiesAndReturnSetUpdatedInstances()
        {
            // Setup
            var strategy = new TestStochasticSoilModelUpdateModelStrategy();
            IEnumerable<StochasticSoilModel> readModels = new[]
            {
                new TestStochasticSoilModel("A"),
                new TestStochasticSoilModel("B"),
                new TestStochasticSoilModel("C")
            };
            var filePath = new string('x', new Random(21).Next(5, 23));

            // Call
            strategy.UpdateModelWithImportedData(null, readModels, filePath);

            // Assert
            Assert.IsTrue(strategy.Updated);
            CollectionAssert.AreEqual(readModels, strategy.ReadModels);
            Assert.AreEqual(filePath, strategy.FilePath);
            Assert.IsEmpty(strategy.UpdatedInstances);
        }

        [Test]
        public void UpdateModelWithImportedData_UpdatedInstancesSet_ReturnsSetUpdatedInstances()
        {
            // Setup
            var strategy = new TestStochasticSoilModelUpdateModelStrategy();
            IEnumerable<StochasticSoilModel> readModels = Enumerable.Empty<StochasticSoilModel>();
            var filePath = new string('x', new Random(21).Next(5, 23));

            IEnumerable<IObservable> updatedInstances = Enumerable.Empty<IObservable>();
            strategy.UpdatedInstances = updatedInstances;

            // Call
            IEnumerable<IObservable> updatedData = strategy.UpdateModelWithImportedData(null, readModels, filePath);

            // Assert
            Assert.AreSame(updatedInstances, updatedData);
        }
    }
}