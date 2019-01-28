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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Plugin.FileImporter;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class PipingStochasticSoilModelReplaceDataStrategyTest
    {
        private const string sourceFilePath = "path";

        [Test]
        public void Constructor_WithoutFailureMechanism_CreatesNewInstance()
        {
            // Call
            TestDelegate test = () => new PipingStochasticSoilModelReplaceDataStrategy(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_WitFailureMechanism_CreatesNewInstance()
        {
            // Call
            var strategy = new PipingStochasticSoilModelReplaceDataStrategy(new PipingFailureMechanism());

            // Assert
            Assert.IsInstanceOf<IStochasticSoilModelUpdateModelStrategy<PipingStochasticSoilModel>>(strategy);
            Assert.IsInstanceOf<ReplaceDataStrategyBase<PipingStochasticSoilModel, PipingFailureMechanism>>(strategy);
        }

        [Test]
        public void UpdateModelWithImportedData_ReadStochasticSoilModelsNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new PipingStochasticSoilModelReplaceDataStrategy(new PipingFailureMechanism());

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
            var strategy = new PipingStochasticSoilModelReplaceDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(new List<PipingStochasticSoilModel>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateModelWithImportedData_WithoutCurrentModelAndModelsImported_NewModelsAdded()
        {
            // Setup
            PipingStochasticSoilModel[] importedStochasticSoilModels =
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("A"),
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("B")
            };
            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.StochasticSoilModels.AddRange(importedStochasticSoilModels, sourceFilePath);
            var strategy = new PipingStochasticSoilModelReplaceDataStrategy(pipingFailureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(importedStochasticSoilModels,
                                                                                            "path");

            // Assert
            CollectionAssert.AreEqual(importedStochasticSoilModels, pipingFailureMechanism.StochasticSoilModels);
            CollectionAssert.AreEqual(new[]
            {
                pipingFailureMechanism.StochasticSoilModels
            }, affectedObjects);
        }

        [Test]
        public void UpdateModelWithImportedData_WithCurrentModelsAndImportedDataEmpty_ModelsRemoved()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("A"),
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("B")
            }, sourceFilePath);

            var strategy = new PipingStochasticSoilModelReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new List<PipingStochasticSoilModel>(), sourceFilePath);

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
            PipingStochasticSoilModel existingModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("existing");

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);
            var strategy = new PipingStochasticSoilModelReplaceDataStrategy(pipingFailureMechanism);
            PipingStochasticSoilModel readModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("read");

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new[]
            {
                readModel
            }, sourceFilePath);

            // Assert
            Assert.AreSame(readModel, pipingFailureMechanism.StochasticSoilModels[0]);
            CollectionAssert.AreEqual(new[]
            {
                pipingFailureMechanism.StochasticSoilModels
            }, affectedObjects);
        }

        [Test]
        public void UpdateModelWithImportedData_CalculationWithOutputAssignedRemovedSoilModelAndProfile_CalculationUpdatedAndCalculationAndInputReturned()
        {
            // Setup
            PipingStochasticSoilModel existingModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel();
            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    StochasticSoilModel = existingModel,
                    StochasticSoilProfile = existingModel.StochasticSoilProfiles.First()
                },
                Output = new PipingOutput(new PipingOutput.ConstructionProperties())
            };

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var strategy = new PipingStochasticSoilModelReplaceDataStrategy(failureMechanism);

            var targetCollection = new PipingStochasticSoilModelCollection();
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new List<PipingStochasticSoilModel>(), sourceFilePath).ToArray();

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
            PipingStochasticSoilModel[] importedStochasticSoilModels =
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("B"),
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("B")
            };
            var strategy = new PipingStochasticSoilModelReplaceDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(importedStochasticSoilModels, "path");

            // Assert
            var exception = Assert.Throws<UpdateDataException>(test);
            Assert.AreEqual("Stochastische ondergrondmodellen moeten een unieke naam hebben. " +
                            "Gevonden dubbele elementen: B.", exception.Message);
        }
    }
}