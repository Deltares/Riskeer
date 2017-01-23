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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Plugin.FileImporter;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class StochasticSoilModelUpdateDataTest
    {
        [Test]
        public void DefaultConstructor_CreatesNewInstance()
        {
            // Call
            var strategy = new StochasticSoilModelUpdateData();

            // Assert
            Assert.IsInstanceOf<IStochasticSoilModelUpdateStrategy>(strategy);
        }

        [Test]
        public void UpdateModelWithImportedData_MultipleCurrentModelsWithSameNameAndImportedDataContainsModelWithSameName_ThrowsInvalidOperationException()
        {
            // Setup
            var nonUniqueName = "non-unique name";
            var sourceFilePath = "path";

            var targetCollection = new StochasticSoilModelCollection();
            targetCollection.AddRange(new[]
                                      {
                                          new TestStochasticSoilModel(nonUniqueName),
                                          new TestStochasticSoilModel(nonUniqueName)
                                      }, sourceFilePath);

            var strategy = new StochasticSoilModelUpdateData();
            var importedStochasticSoilModels = new[]
            {
                new TestStochasticSoilModel(nonUniqueName)
            };

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(importedStochasticSoilModels, sourceFilePath, targetCollection, NotifyProgress);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void UpdateModelWithImportedData_CurrentModelsEmptyModelsImported_NewModelsAdded()
        {
            // Setup
            var importedStochasticSoilModels = new[]
            {
                new TestStochasticSoilModel(),
                new TestStochasticSoilModel()
            };
            var strategy = new StochasticSoilModelUpdateData();
            var targetCollection = new StochasticSoilModelCollection();

            // Call
            strategy.UpdateModelWithImportedData(importedStochasticSoilModels, "path", targetCollection, NotifyProgress);

            // Assert
            CollectionAssert.AreEquivalent(importedStochasticSoilModels, targetCollection);
        }

        [Test]
        public void UpdateModelWithImportedData_CurrentModelsContainsModelsImportedDataEmpty_ModelsRemoved()
        {
            // Setup
            var sourceFilePath = "path";
            var targetCollection = new StochasticSoilModelCollection();
            targetCollection.AddRange(new[]
                                      {
                                          new TestStochasticSoilModel(),
                                          new TestStochasticSoilModel()
                                      }, sourceFilePath);

            var strategy = new StochasticSoilModelUpdateData();

            // Call
            strategy.UpdateModelWithImportedData(new List<StochasticSoilModel>(), sourceFilePath, targetCollection, NotifyProgress);

            // Assert
            CollectionAssert.IsEmpty(targetCollection);
        }

        [Test]
        public void UpdateModelWithImportedData_CurrentModelsContainsModelsImportedDataContainsModelWithOtherName_ModelReplaced()
        {
            // Setup
            var sourceFilePath = "path";
            var readModel = new TestStochasticSoilModel("read");
            var existingModel = new TestStochasticSoilModel("existing");

            var targetCollection = new StochasticSoilModelCollection();
            targetCollection.AddRange(new[]
                                      {
                                          existingModel,
                                      }, sourceFilePath);

            var strategy = new StochasticSoilModelUpdateData();

            // Call
            strategy.UpdateModelWithImportedData(new[] { readModel }, sourceFilePath, targetCollection, NotifyProgress);

            // Assert
            Assert.AreSame(readModel, targetCollection.First());
        }

        [Test]
        public void UpdateModelWithImportedData_CurrentModelsContainsModelsImportedDataContainsModelWithSameName_ModelUpdated()
        {
            // Setup
            var sourceFilePath = "path";
            var modelsName = "same model";
            var readModel = new TestStochasticSoilModel(modelsName);
            var existingModel = new TestStochasticSoilModel(modelsName);

            var targetCollection = new StochasticSoilModelCollection();
            targetCollection.AddRange(new[]
                                      {
                                          existingModel,
                                      }, sourceFilePath);

            var strategy = new StochasticSoilModelUpdateData();

            // Call
            strategy.UpdateModelWithImportedData(new[] { readModel }, sourceFilePath, targetCollection, NotifyProgress);

            // Assert
            Assert.AreSame(existingModel, targetCollection.First());
        }

        private void NotifyProgress(string stepName, int stepNr, int totalNr) {}
    }
}