﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Plugin.FileImporter;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class StochasticSoilModelReplaceDataStrategyTest
    {
        private const string sourceFilePath = "path";

        [Test]
        public void Constructor_WithoutCalculations_CreatesNewInstance()
        {
            // Call
            TestDelegate test = () => new StochasticSoilModelReplaceDataStrategy(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_WithCalculations_CreatesNewInstance()
        {
            // Call
            var strategy = new StochasticSoilModelReplaceDataStrategy(new PipingFailureMechanism());

            // Assert
            Assert.IsInstanceOf<IStochasticSoilModelUpdateModelStrategy>(strategy);
        }

        [Test]
        public void UpdateModelWithImportedData_ReadStochasticSoilModelsNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new StochasticSoilModelReplaceDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(new ObservableCollectionWithSourcePath<StochasticSoilModel>(), null, string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("readStochasticSoilModels", paramName);
        }

        [Test]
        public void UpdateModelWithImportedData_SourceFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new StochasticSoilModelReplaceDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(new ObservableCollectionWithSourcePath<StochasticSoilModel>(), new List<StochasticSoilModel>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateModelWithImportedData_TargetCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new StochasticSoilModelReplaceDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(null, new List<StochasticSoilModel>(), string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("targetCollection", paramName);
        }

        [Test]
        public void UpdateModelWithImportedData_WithoutCurrentModelAndModelsImported_NewModelsAdded()
        {
            // Setup
            var importedStochasticSoilModels = new[]
            {
                new TestStochasticSoilModel("A"),
                new TestStochasticSoilModel("B")
            };
            var strategy = new StochasticSoilModelReplaceDataStrategy(new PipingFailureMechanism());
            var targetCollection = new ObservableCollectionWithSourcePath<StochasticSoilModel>();

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(targetCollection, importedStochasticSoilModels, "path");

            // Assert
            CollectionAssert.AreEqual(importedStochasticSoilModels, targetCollection);
            CollectionAssert.AreEqual(new[]
            {
                targetCollection
            }, affectedObjects);
        }

        [Test]
        public void UpdateModelWithImportedData_WithCurrentModelsAndImportedDataEmpty_ModelsRemoved()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                new TestStochasticSoilModel(),
                new TestStochasticSoilModel()
            }, sourceFilePath);

            var strategy = new StochasticSoilModelReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(failureMechanism.StochasticSoilModels, new List<StochasticSoilModel>(), sourceFilePath);

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
            var existingModel = new TestStochasticSoilModel("existing");

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
            {
                existingModel,
            }, sourceFilePath);
            var strategy = new StochasticSoilModelReplaceDataStrategy(pipingFailureMechanism);
            var readModel = new TestStochasticSoilModel("read");

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(
                pipingFailureMechanism.StochasticSoilModels,
                new[]
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
            var existingModel = new TestStochasticSoilModel();
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            calculation.InputParameters.StochasticSoilModel = existingModel;
            calculation.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles[0];
            calculation.Output = new PipingOutput(new PipingOutput.ConstructionProperties());

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var strategy = new StochasticSoilModelReplaceDataStrategy(failureMechanism);

            var targetCollection = new ObservableCollectionWithSourcePath<StochasticSoilModel>();
            targetCollection.AddRange(new[]
            {
                existingModel,
            }, sourceFilePath);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(targetCollection, new List<StochasticSoilModel>(), sourceFilePath).ToArray();

            // Assert
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.InputParameters.StochasticSoilModel);
            Assert.IsNull(calculation.InputParameters.StochasticSoilProfile);
            CollectionAssert.Contains(affectedObjects, calculation);
            CollectionAssert.Contains(affectedObjects, calculation.InputParameters);
        }
    }
}