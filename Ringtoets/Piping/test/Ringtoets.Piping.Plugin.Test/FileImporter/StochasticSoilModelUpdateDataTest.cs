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
using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Plugin.FileImporter;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class StochasticSoilModelUpdateDataTest
    {
        private const string sourceFilePath = "path";

        [Test]
        public void Constructor_WithoutCalculations_CreatesNewInstance()
        {
            // Call
            TestDelegate test = () => new StochasticSoilModelUpdateData(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_WithCalculations_CreatesNewInstance()
        {
            // Call
            var strategy = new StochasticSoilModelUpdateData(new PipingFailureMechanism());

            // Assert
            Assert.IsInstanceOf<IStochasticSoilModelUpdateStrategy>(strategy);
        }

        [Test]
        public void UpdateModelWithImportedData_ReadStochasticSoilModelsNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new StochasticSoilModelUpdateData(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(null, string.Empty, new StochasticSoilModelCollection());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("readStochasticSoilModels", paramName);
        }

        [Test]
        public void UpdateModelWithImportedData_SourceFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new StochasticSoilModelUpdateData(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(new List<StochasticSoilModel>(), null, new StochasticSoilModelCollection());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("filePath", paramName);
        }

        [Test]
        public void UpdateModelWithImportedData_TargetCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new StochasticSoilModelUpdateData(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(new List<StochasticSoilModel>(), string.Empty, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("targetCollection", paramName);
        }

        [Test]
        public void UpdateModelWithImportedData_WithCurrentModelsAndImportedMultipleModelsWithSameName_ThrowsInvalidOperationException()
        {
            // Setup
            var nonUniqueName = "non-unique name";

            var targetCollection = new StochasticSoilModelCollection();
            targetCollection.AddRange(new[]
                                      {
                                          new TestStochasticSoilModel(nonUniqueName)
                                      }, sourceFilePath);

            var strategy = new StochasticSoilModelUpdateData(new PipingFailureMechanism());
            var importedStochasticSoilModels = new[]
            {
                new TestStochasticSoilModel(nonUniqueName),
                new TestStochasticSoilModel(nonUniqueName)
            };

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(importedStochasticSoilModels, sourceFilePath, targetCollection);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
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
            var strategy = new StochasticSoilModelUpdateData(new PipingFailureMechanism());
            var targetCollection = new StochasticSoilModelCollection();

            // Call
            strategy.UpdateModelWithImportedData(importedStochasticSoilModels, "path", targetCollection);

            // Assert
            CollectionAssert.AreEquivalent(importedStochasticSoilModels, targetCollection);
        }

        [Test]
        public void UpdateModelWithImportedData_WithCurrentModelsAndImportedDataEmpty_ModelsRemoved()
        {
            // Setup
            var targetCollection = new StochasticSoilModelCollection();
            targetCollection.AddRange(new[]
                                      {
                                          new TestStochasticSoilModel(),
                                          new TestStochasticSoilModel()
                                      }, sourceFilePath);

            var strategy = new StochasticSoilModelUpdateData(new PipingFailureMechanism());

            // Call
            strategy.UpdateModelWithImportedData(new List<StochasticSoilModel>(), sourceFilePath, targetCollection);

            // Assert
            CollectionAssert.IsEmpty(targetCollection);
        }

        [Test]
        public void UpdateModelWithImportedData_WithCurrentModelAndImportedModelWithOtherName_ModelReplaced()
        {
            // Setup
            var existingModel = new TestStochasticSoilModel("existing");

            var targetCollection = new StochasticSoilModelCollection();
            targetCollection.AddRange(new[]
                                      {
                                          existingModel,
                                      }, sourceFilePath);

            var strategy = new StochasticSoilModelUpdateData(new PipingFailureMechanism());
            var readModel = new TestStochasticSoilModel("read");

            // Call
            strategy.UpdateModelWithImportedData(new[] { readModel }, sourceFilePath, targetCollection);

            // Assert
            Assert.AreSame(readModel, targetCollection.First());
        }

        [Test]
        public void UpdateModelWithImportedData_WithCurrentModelAndImportedModelWithSameName_ModelUpdated()
        {
            // Setup
            var modelsName = "same model";
            var existingModel = new TestStochasticSoilModel(modelsName);

            var targetCollection = new StochasticSoilModelCollection();
            targetCollection.AddRange(new[]
                                      {
                                          existingModel,
                                      }, sourceFilePath);

            var strategy = new StochasticSoilModelUpdateData(new PipingFailureMechanism());
            var readModel = new TestStochasticSoilModel(modelsName);

            // Call
            strategy.UpdateModelWithImportedData(new[] { readModel }, sourceFilePath, targetCollection);

            // Assert
            Assert.AreEqual(1, targetCollection.Count);
            Assert.AreSame(existingModel, targetCollection.First());
        }

        [Test]
        public void UpdateModelWithImportedData_UpdateCurrentModelWithImportedModelWithOtherProfiles_ProfilesAdded()
        {
            // Setup
            var modelsName = "same model";
            var existingModel = new TestStochasticSoilModel(modelsName);

            var targetCollection = new StochasticSoilModelCollection();
            targetCollection.AddRange(new[]
                                      {
                                          existingModel,
                                      }, sourceFilePath);

            var strategy = new StochasticSoilModelUpdateData(new PipingFailureMechanism());
            var readModel = CreateSimpleModel(modelsName, "new profile A", "new profile B");

            // Call
            strategy.UpdateModelWithImportedData(new[] { readModel }, sourceFilePath, targetCollection);

            // Assert
            Assert.AreSame(existingModel, targetCollection.First());
            Assert.AreEqual(2, targetCollection.First().StochasticSoilProfiles.Count);
        }

        [Test]
        public void UpdateModelWithImportedData_ProfilesAssignedToCalculationsWithOneImportedModelProfileRemoved_OneProfileRemovedOtherUpdatedCalculationsUpdatedAccordingly()
        {
            // Setup
            var modelsName = "same model";
            var existingModel = new TestStochasticSoilModel(modelsName);

            var targetCollection = new StochasticSoilModelCollection();
            targetCollection.AddRange(new[]
                                      {
                                          existingModel,
                                      }, sourceFilePath);

            var firstExistingProfile = existingModel.StochasticSoilProfiles.First();
            var readModel = CreateSimpleModel(modelsName, firstExistingProfile.SoilProfile.Name);

            var calculationWithUpdatedProfile = new PipingCalculationScenario(new GeneralPipingInput());
            calculationWithUpdatedProfile.InputParameters.StochasticSoilModel = existingModel;
            calculationWithUpdatedProfile.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles[0];
            calculationWithUpdatedProfile.Output = new PipingOutput(new PipingOutput.ConstructionProperties());

            var calculationWithDeletedProfile = new PipingCalculationScenario(new GeneralPipingInput());
            calculationWithDeletedProfile.InputParameters.StochasticSoilModel = existingModel;
            calculationWithDeletedProfile.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles[1];
            calculationWithDeletedProfile.Output = new PipingOutput(new PipingOutput.ConstructionProperties());

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationWithDeletedProfile);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithUpdatedProfile);

            var strategy = new StochasticSoilModelUpdateData(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(
                new[] { readModel }, 
                sourceFilePath, 
                targetCollection).ToArray();

            // Assert
            var firstSoilModel = targetCollection.First();
            Assert.AreSame(existingModel, firstSoilModel);
            Assert.AreEqual(1, firstSoilModel.StochasticSoilProfiles.Count);
            Assert.AreSame(firstExistingProfile, firstSoilModel.StochasticSoilProfiles.First());

            Assert.IsFalse(calculationWithUpdatedProfile.HasOutput);
            CollectionAssert.Contains(affectedObjects, calculationWithUpdatedProfile);
            CollectionAssert.Contains(affectedObjects, calculationWithUpdatedProfile.InputParameters);

            Assert.IsFalse(calculationWithDeletedProfile.HasOutput);
            Assert.IsNull(calculationWithDeletedProfile.InputParameters.StochasticSoilProfile);
            CollectionAssert.Contains(affectedObjects, calculationWithUpdatedProfile);
            CollectionAssert.Contains(affectedObjects, calculationWithUpdatedProfile.InputParameters);

        }

        [Test]
        public void UpdateModelWithImportedData_CalculationWithOutputAssignedRemovedSoilModelAndProfile_CalculationUpdatedAndCalculationAndInputReturned()
        {
            // Setup
            var existingModel = new TestStochasticSoilModel();
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            calculation.InputParameters.StochasticSoilModel = existingModel;
            calculation.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles.First();
            calculation.Output = new PipingOutput(new PipingOutput.ConstructionProperties());

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var strategy = new StochasticSoilModelUpdateData(failureMechanism);

            var targetCollection = new StochasticSoilModelCollection();
            targetCollection.AddRange(new[]
                                      {
                                          existingModel,
                                      }, sourceFilePath);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(
                new List<StochasticSoilModel>(),
                sourceFilePath,
                targetCollection).ToArray();

            // Assert
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.InputParameters.StochasticSoilModel);
            Assert.IsNull(calculation.InputParameters.StochasticSoilProfile);
            CollectionAssert.Contains(affectedObjects, calculation);
            CollectionAssert.Contains(affectedObjects, calculation.InputParameters);
        }

        /// <summary>
        /// Creates a simple model with names for the model and profiles in the model set as specified.
        /// </summary>
        /// <param name="modelName">Name of the created model.</param>
        /// <param name="profileNames">List of names for the profiles to be added to the model.</param>
        /// <returns>A new <see cref="StochasticSoilModel"/>.</returns>
        private StochasticSoilModel CreateSimpleModel(string modelName, params string[] profileNames)
        {
            var model = new StochasticSoilModel(-1, modelName, "segment name");
            foreach (var profileName in profileNames)
            {
                model.StochasticSoilProfiles.Add(
                    new StochasticSoilProfile(1.0 / profileNames.Length, SoilProfileType.SoilProfile1D, -1)
                    {
                        SoilProfile = new TestPipingSoilProfile(profileName)
                    });
            }
            return model;
        }
    }
}