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
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Plugin.FileImporter;

namespace Ringtoets.MacroStabilityInwards.Plugin.Test.FileImporter
{
    [TestFixture]
    public class MacroStabilityInwardsStochasticSoilModelReplaceDataStrategyTest
    {
        private const string sourceFilePath = "path";

        [Test]
        public void Constructor_WithoutFailureMechanism_CreatesNewInstance()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsStochasticSoilModelReplaceDataStrategy(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_WithFailureMechanism_CreatesNewInstance()
        {
            // Call
            var strategy = new MacroStabilityInwardsStochasticSoilModelReplaceDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Assert
            Assert.IsInstanceOf<IStochasticSoilModelUpdateModelStrategy<MacroStabilityInwardsStochasticSoilModel>>(strategy);
            Assert.IsInstanceOf<ReplaceDataStrategyBase<MacroStabilityInwardsStochasticSoilModel, MacroStabilityInwardsFailureMechanism>>(strategy);
        }

        [Test]
        public void UpdateModelWithImportedData_ReadStochasticSoilModelsNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new MacroStabilityInwardsStochasticSoilModelReplaceDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(null, string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("importedDataCollection", paramName);
        }

        [Test]
        public void UpdateModelWithImportedData_SourceFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new MacroStabilityInwardsStochasticSoilModelReplaceDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(new List<MacroStabilityInwardsStochasticSoilModel>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateModelWithImportedData_WithoutCurrentModelAndModelsImported_NewModelsAdded()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilModel[] importedStochasticSoilModels =
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("A"),
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("B")
            };
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.StochasticSoilModels.AddRange(importedStochasticSoilModels, sourceFilePath);
            var strategy = new MacroStabilityInwardsStochasticSoilModelReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(importedStochasticSoilModels,
                                                                                            "path");

            // Assert
            CollectionAssert.AreEqual(importedStochasticSoilModels, failureMechanism.StochasticSoilModels);
            CollectionAssert.AreEqual(new[]
            {
                failureMechanism.StochasticSoilModels
            }, affectedObjects);
        }

        [Test]
        public void UpdateModelWithImportedData_WithCurrentModelsAndImportedDataEmpty_ModelsRemoved()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("A"),
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("B")
            }, sourceFilePath);

            var strategy = new MacroStabilityInwardsStochasticSoilModelReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new List<MacroStabilityInwardsStochasticSoilModel>(), sourceFilePath);

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
            CollectionAssert.AreEqual(new[]
            {
                failureMechanism.StochasticSoilModels
            }, affectedObjects);
        }

        [Test]
        public void UpdateModelWithImportedData_WithCurrentModelAndImportedModel_ModelReplaced()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilModel existingModel = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("existing");

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);
            var strategy = new MacroStabilityInwardsStochasticSoilModelReplaceDataStrategy(failureMechanism);
            MacroStabilityInwardsStochasticSoilModel readModel = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("read");

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new[]
            {
                readModel
            }, sourceFilePath);

            // Assert
            Assert.AreSame(readModel, failureMechanism.StochasticSoilModels[0]);
            CollectionAssert.AreEqual(new[]
            {
                failureMechanism.StochasticSoilModels
            }, affectedObjects);
        }

        [Test]
        public void UpdateModelWithImportedData_CalculationWithOutputAssignedRemovedSoilModelAndProfile_CalculationUpdatedAndCalculationAndInputReturned()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilModel existingModel = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel();
            var calculation = new MacroStabilityInwardsCalculationScenario();
            calculation.InputParameters.StochasticSoilModel = existingModel;
            calculation.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles.ElementAt(0);
            calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateOutput();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var strategy = new MacroStabilityInwardsStochasticSoilModelReplaceDataStrategy(failureMechanism);

            var targetCollection = new MacroStabilityInwardsStochasticSoilModelCollection();
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new List<MacroStabilityInwardsStochasticSoilModel>(), sourceFilePath).ToArray();

            // Assert
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.InputParameters.StochasticSoilModel);
            Assert.IsNull(calculation.InputParameters.StochasticSoilProfile);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                calculation,
                calculation.InputParameters,
                failureMechanism.StochasticSoilModels
            }, affectedObjects);
        }

        [Test]
        public void UpdateModelWithImportedData_ImportedModelsContainDuplicateNames_ThrowsUpdateDataException()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilModel[] importedStochasticSoilModels =
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("B"),
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("B")
            };
            var strategy = new MacroStabilityInwardsStochasticSoilModelReplaceDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(importedStochasticSoilModels, "path");

            // Assert
            var exception = Assert.Throws<UpdateDataException>(test);
            Assert.AreEqual("Stochastische ondergrondmodellen moeten een unieke naam hebben. " +
                            "Gevonden dubbele elementen: B.", exception.Message);
        }
    }
}