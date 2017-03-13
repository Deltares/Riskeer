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
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Importers;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Plugin.FileImporter;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class StochasticSoilModelUpdateDataStrategyTest
    {
        private const string sourceFilePath = "path";

        [Test]
        public void Constructor_WithoutCalculations_CreatesNewInstance()
        {
            // Call
            TestDelegate test = () => new StochasticSoilModelUpdateDataStrategy(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_WithCalculations_CreatesNewInstance()
        {
            // Call
            var strategy = new StochasticSoilModelUpdateDataStrategy(new PipingFailureMechanism());

            // Assert
            Assert.IsInstanceOf<IStochasticSoilModelUpdateModelStrategy>(strategy);
            Assert.IsInstanceOf<UpdateDataStrategyBase<StochasticSoilModel, PipingFailureMechanism>>(strategy);
        }

        [Test]
        public void UpdateModelWithImportedData_ReadStochasticSoilModelsNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new StochasticSoilModelUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(new StochasticSoilModelCollection(), null, string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("importedDataCollection", paramName);
        }

        [Test]
        public void UpdateModelWithImportedData_SourceFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new StochasticSoilModelUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(new StochasticSoilModelCollection(), new List<StochasticSoilModel>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateModelWithImportedData_TargetCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new StochasticSoilModelUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(null, new List<StochasticSoilModel>(), string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("targetDataCollection", paramName);
        }

        [Test]
        public void UpdateModelWithImportedData_WithCurrentModelsAndImportedMultipleModelsWithSameName_ThrowsStochasticSoilModelUpdateException()
        {
            // Setup
            const string nonUniqueName = "non-unique name";

            var targetCollection = new StochasticSoilModelCollection();
            targetCollection.AddRange(new[]
            {
                new TestStochasticSoilModel(nonUniqueName)
            }, sourceFilePath);

            var strategy = new StochasticSoilModelUpdateDataStrategy(new PipingFailureMechanism());
            var importedStochasticSoilModels = new[]
            {
                new TestStochasticSoilModel(nonUniqueName),
                new TestStochasticSoilModel(nonUniqueName)
            };

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(targetCollection, importedStochasticSoilModels, sourceFilePath);

            // Assert
            var exception = Assert.Throws<StochasticSoilModelUpdateException>(test);
            Assert.AreEqual("Het bijwerken van de stochastische ondergrondmodellen is mislukt.", exception.Message);
            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
        }

        [Test]
        public void UpdateModelWithImportedData_WithoutCurrentModelsAndImportedMultipleModelsWithSameName_ThrowsStochasticSoilModelUpdateException()
        {
            // Setup
            const string nonUniqueName = "non-unique name";

            var targetCollection = new StochasticSoilModelCollection();

            var strategy = new StochasticSoilModelUpdateDataStrategy(new PipingFailureMechanism());
            var importedStochasticSoilModels = new[]
            {
                new TestStochasticSoilModel(nonUniqueName),
                new TestStochasticSoilModel(nonUniqueName)
            };

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(targetCollection, importedStochasticSoilModels, sourceFilePath);

            // Assert
            var exception = Assert.Throws<StochasticSoilModelUpdateException>(test);
            Assert.AreEqual("Stochastische ondergrondmodellen moeten een unieke naam hebben. Gevonden dubbele elementen: non-unique name.", exception.Message);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);

            CollectionAssert.IsEmpty(targetCollection);
        }

        [Test]
        public void UpdateModelWithImportedData_WithoutCurrentModelAndNoImportedModels_NoChangeNoNotification()
        {
            // Setup
            var importedStochasticSoilModels = Enumerable.Empty<TestStochasticSoilModel>();
            var strategy = new StochasticSoilModelUpdateDataStrategy(new PipingFailureMechanism());
            var targetCollection = new StochasticSoilModelCollection();

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(targetCollection, importedStochasticSoilModels, "path");

            // Assert
            Assert.IsEmpty(targetCollection);
            Assert.IsEmpty(affectedObjects);
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
            var strategy = new StochasticSoilModelUpdateDataStrategy(new PipingFailureMechanism());
            var targetCollection = new StochasticSoilModelCollection();

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
            StochasticSoilModelCollection stochasticSoilModelCollection = failureMechanism.StochasticSoilModels;
            stochasticSoilModelCollection.AddRange(new[]
            {
                new TestStochasticSoilModel("A"),
                new TestStochasticSoilModel("B")
            }, sourceFilePath);

            var strategy = new StochasticSoilModelUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(stochasticSoilModelCollection,
                                                                                            new List<StochasticSoilModel>(),
                                                                                            sourceFilePath);

            // Assert
            CollectionAssert.IsEmpty(stochasticSoilModelCollection);
            CollectionAssert.AreEquivalent(new[]
            {
                stochasticSoilModelCollection
            }, affectedObjects);
        }

        [Test]
        public void UpdateModelWithImportedData_WithCurrentModelAndImportedModelWithOtherName_ModelReplaced()
        {
            // Setup
            var existingModel = new TestStochasticSoilModel("existing");

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);
            var strategy = new StochasticSoilModelUpdateDataStrategy(pipingFailureMechanism);
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
        public void UpdateModelWithImportedData_WithCurrentModelAndImportedModelWithSameName_ModelUpdated()
        {
            // Setup
            var modelsName = "same model";
            var existingModel = new TestStochasticSoilModel(modelsName);

            var targetCollection = new StochasticSoilModelCollection();
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            var strategy = new StochasticSoilModelUpdateDataStrategy(new PipingFailureMechanism());
            var readModel = new TestStochasticSoilModel(modelsName);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(targetCollection, new[]
            {
                readModel
            }, sourceFilePath);

            // Assert
            Assert.AreEqual(1, targetCollection.Count);
            Assert.AreSame(existingModel, targetCollection[0]);
            CollectionAssert.AreEquivalent(new[]
            {
                existingModel
            }, affectedObjects);
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
                existingModel
            }, sourceFilePath);

            var strategy = new StochasticSoilModelUpdateDataStrategy(new PipingFailureMechanism());
            StochasticSoilModel readModel = CreateSimpleModel(modelsName, "new profile A", "new profile B");

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(targetCollection, new[]
            {
                readModel
            }, sourceFilePath);

            // Assert
            Assert.AreEqual(1, targetCollection.Count);
            Assert.AreSame(existingModel, targetCollection[0]);
            Assert.AreEqual(2, targetCollection[0].StochasticSoilProfiles.Count);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                existingModel
            }, affectedObjects);
        }

        [Test]
        public void UpdateModelWithImportedData_ProfilesAssignedToCalculationsWithOneImportedModelProfileRemoved_OneProfileRemovedCalculationUpdatedAccordingly()
        {
            // Setup
            var modelsName = "same model";
            var existingModel = new TestStochasticSoilModel(modelsName);

            var targetCollection = new StochasticSoilModelCollection();
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            StochasticSoilProfile firstExistingProfile = existingModel.StochasticSoilProfiles[0];
            StochasticSoilModel readModel = CreateSimpleModel(modelsName, firstExistingProfile.SoilProfile.Name);

            var calculationWithNotUpdatedProfile = new PipingCalculationScenario(new GeneralPipingInput());
            calculationWithNotUpdatedProfile.InputParameters.StochasticSoilModel = existingModel;
            calculationWithNotUpdatedProfile.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles[0];
            calculationWithNotUpdatedProfile.Output = new PipingOutput(new PipingOutput.ConstructionProperties());

            var calculationWithDeletedProfile = new PipingCalculationScenario(new GeneralPipingInput());
            calculationWithDeletedProfile.InputParameters.StochasticSoilModel = existingModel;
            calculationWithDeletedProfile.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles[1];
            calculationWithDeletedProfile.Output = new PipingOutput(new PipingOutput.ConstructionProperties());

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationWithDeletedProfile);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithNotUpdatedProfile);

            var strategy = new StochasticSoilModelUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(targetCollection, new[]
            {
                readModel
            }, sourceFilePath).ToArray();

            // Assert
            StochasticSoilModel firstSoilModel = targetCollection[0];
            Assert.AreSame(existingModel, firstSoilModel);
            Assert.AreEqual(1, firstSoilModel.StochasticSoilProfiles.Count);
            Assert.AreSame(firstExistingProfile, firstSoilModel.StochasticSoilProfiles[0]);

            Assert.IsTrue(calculationWithNotUpdatedProfile.HasOutput);
            CollectionAssert.DoesNotContain(affectedObjects, calculationWithNotUpdatedProfile);
            CollectionAssert.DoesNotContain(affectedObjects, calculationWithNotUpdatedProfile.InputParameters);

            Assert.IsFalse(calculationWithDeletedProfile.HasOutput);
            Assert.IsNull(calculationWithDeletedProfile.InputParameters.StochasticSoilProfile);
            CollectionAssert.Contains(affectedObjects, calculationWithDeletedProfile);
            CollectionAssert.Contains(affectedObjects, calculationWithDeletedProfile.InputParameters);
        }

        [Test]
        public void UpdateModelWithImportedData_ProfilesAssignedToCalculationsOneWithNoChangeInProfile_OnlyCalculationWithChangedProfileUpdated()
        {
            // Setup
            var modelsName = "same model";
            var existingModel = new TestStochasticSoilModel(modelsName);

            var targetCollection = new StochasticSoilModelCollection();
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            StochasticSoilModel readModel = new TestStochasticSoilModel(modelsName);
            StochasticSoilProfile changedProfile = CloneAndSlightlyModify(readModel.StochasticSoilProfiles.ElementAt(0));
            readModel.StochasticSoilProfiles[0] = changedProfile;

            var calculationWithUpdatedProfile = new PipingCalculationScenario(new GeneralPipingInput());
            calculationWithUpdatedProfile.InputParameters.StochasticSoilModel = existingModel;
            calculationWithUpdatedProfile.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles[0];
            calculationWithUpdatedProfile.Output = new PipingOutput(new PipingOutput.ConstructionProperties());

            var calculationWithNotUpdatedProfile = new PipingCalculationScenario(new GeneralPipingInput());
            calculationWithNotUpdatedProfile.InputParameters.StochasticSoilModel = existingModel;
            calculationWithNotUpdatedProfile.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles[1];
            calculationWithNotUpdatedProfile.Output = new PipingOutput(new PipingOutput.ConstructionProperties());

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationWithNotUpdatedProfile);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithUpdatedProfile);

            var strategy = new StochasticSoilModelUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(targetCollection, new[]
            {
                readModel
            }, sourceFilePath).ToArray();

            // Assert
            StochasticSoilModel firstSoilModel = targetCollection[0];
            Assert.AreSame(existingModel, firstSoilModel);
            Assert.AreEqual(2, firstSoilModel.StochasticSoilProfiles.Count);
            Assert.AreSame(existingModel.StochasticSoilProfiles[0], firstSoilModel.StochasticSoilProfiles[0]);
            Assert.AreSame(existingModel.StochasticSoilProfiles[1], firstSoilModel.StochasticSoilProfiles[1]);

            Assert.IsFalse(calculationWithUpdatedProfile.HasOutput);
            Assert.AreSame(existingModel.StochasticSoilProfiles[0], calculationWithUpdatedProfile.InputParameters.StochasticSoilProfile);

            Assert.IsTrue(calculationWithNotUpdatedProfile.HasOutput);
            Assert.AreSame(existingModel.StochasticSoilProfiles[1], calculationWithNotUpdatedProfile.InputParameters.StochasticSoilProfile);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                firstSoilModel,
                calculationWithUpdatedProfile,
                calculationWithUpdatedProfile.InputParameters,
                calculationWithUpdatedProfile.InputParameters.StochasticSoilProfile
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
            StochasticSoilModelCollection stochasticSoilModelCollection = failureMechanism.StochasticSoilModels;
            stochasticSoilModelCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            var strategy = new StochasticSoilModelUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(stochasticSoilModelCollection, new List<StochasticSoilModel>(), sourceFilePath).ToArray();

            // Assert
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.InputParameters.StochasticSoilModel);
            Assert.IsNull(calculation.InputParameters.StochasticSoilProfile);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                stochasticSoilModelCollection,
                calculation,
                calculation.InputParameters
            }, affectedObjects);
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
            foreach (string profileName in profileNames)
            {
                model.StochasticSoilProfiles.Add(
                    new StochasticSoilProfile(1.0 / profileNames.Length, SoilProfileType.SoilProfile1D, -1)
                    {
                        SoilProfile = new TestPipingSoilProfile(profileName)
                    });
            }
            return model;
        }

        private StochasticSoilProfile CloneAndSlightlyModify(StochasticSoilProfile profile)
        {
            return new StochasticSoilProfile(profile.Probability, profile.SoilProfileType, profile.SoilProfileId)
            {
                SoilProfile = new PipingSoilProfile(
                    profile.SoilProfile.Name,
                    profile.SoilProfile.Bottom - 0.2,
                    profile.SoilProfile.Layers,
                    profile.SoilProfile.SoilProfileType,
                    profile.SoilProfile.PipingSoilProfileId)
            };
        }
    }
}