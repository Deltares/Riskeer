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
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Primitives.TestUtil;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class PipingStochasticSoilModelUpdateDataStrategyTest
    {
        private const string sourceFilePath = "path";

        [Test]
        public void Constructor_WithoutCalculations_CreatesNewInstance()
        {
            // Call
            TestDelegate test = () => new PipingStochasticSoilModelUpdateDataStrategy(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_WithCalculations_CreatesNewInstance()
        {
            // Call
            var strategy = new PipingStochasticSoilModelUpdateDataStrategy(new PipingFailureMechanism());

            // Assert
            Assert.IsInstanceOf<IStochasticSoilModelUpdateModelStrategy<PipingStochasticSoilModel>>(strategy);
            Assert.IsInstanceOf<UpdateDataStrategyBase<PipingStochasticSoilModel, PipingFailureMechanism>>(strategy);
        }

        [Test]
        public void UpdateModelWithImportedData_ReadStochasticSoilModelsNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new PipingStochasticSoilModelUpdateDataStrategy(new PipingFailureMechanism());

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
            var strategy = new PipingStochasticSoilModelUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(new List<PipingStochasticSoilModel>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateModelWithImportedData_WithCurrentModelsAndImportedMultipleModelsWithSameName_ThrowsUpdateDataException()
        {
            // Setup
            const string nonUniqueName = "non-unique name";

            var targetCollection = new PipingStochasticSoilModelCollection();
            targetCollection.AddRange(new[]
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(nonUniqueName)
            }, sourceFilePath);

            var strategy = new PipingStochasticSoilModelUpdateDataStrategy(new PipingFailureMechanism());
            PipingStochasticSoilModel[] importedStochasticSoilModels =
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(nonUniqueName),
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(nonUniqueName)
            };

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(importedStochasticSoilModels, sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(test);
            const string expectedMessage = "Geïmporteerde data moet unieke elementen bevatten.";
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void UpdateModelWithImportedData_WithoutCurrentModelsAndImportedMultipleModelsWithSameName_ThrowsUpdateDataException()
        {
            // Setup
            const string nonUniqueName = "non-unique name";

            var targetCollection = new PipingStochasticSoilModelCollection();

            var strategy = new PipingStochasticSoilModelUpdateDataStrategy(new PipingFailureMechanism());
            PipingStochasticSoilModel[] importedStochasticSoilModels =
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(nonUniqueName),
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(nonUniqueName)
            };

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(importedStochasticSoilModels, sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(test);
            const string expectedMessage = "Geïmporteerde data moet unieke elementen bevatten.";
            Assert.AreEqual(expectedMessage, exception.Message);

            CollectionAssert.IsEmpty(targetCollection);
        }

        [Test]
        public void UpdateModelWithImportedData_WithoutCurrentModelAndNoImportedModels_NoChangeNoNotification()
        {
            // Setup
            var strategy = new PipingStochasticSoilModelUpdateDataStrategy(new PipingFailureMechanism());
            var targetCollection = new PipingStochasticSoilModelCollection();

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(Enumerable.Empty<PipingStochasticSoilModel>(), "path");

            // Assert
            CollectionAssert.IsEmpty(targetCollection);
            CollectionAssert.IsEmpty(affectedObjects);
        }

        [Test]
        public void UpdateModelWithImportedData_WithCurrentModelAndImportedModelWithOtherName_ModelReplaced()
        {
            // Setup
            PipingStochasticSoilModel existingModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("existing");

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);
            var strategy = new PipingStochasticSoilModelUpdateDataStrategy(pipingFailureMechanism);
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
        public void UpdateModelWithImportedData_WithCurrentModelAndImportedModelWithSameName_ModelUpdated()
        {
            // Setup
            const string modelsName = "same model";
            PipingStochasticSoilModel existingModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(modelsName);

            var failureMechanism = new PipingFailureMechanism();
            PipingStochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            var strategy = new PipingStochasticSoilModelUpdateDataStrategy(failureMechanism);
            PipingStochasticSoilModel readModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(modelsName);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new[]
            {
                readModel
            }, sourceFilePath);

            // Assert
            Assert.AreEqual(1, targetCollection.Count);
            Assert.AreSame(existingModel, targetCollection[0]);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                targetCollection,
                existingModel
            }, affectedObjects);
        }

        [Test]
        public void UpdateModelWithImportedData_UpdateCurrentModelWithImportedModelWithOtherProfiles_ProfilesAdded()
        {
            // Setup
            const string modelsName = "same model";
            PipingStochasticSoilModel existingModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(modelsName);

            var failureMechanism = new PipingFailureMechanism();
            PipingStochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            var strategy = new PipingStochasticSoilModelUpdateDataStrategy(failureMechanism);
            PipingStochasticSoilModel readModel = CreateSimpleModel(modelsName, "new profile A", "new profile B");

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new[]
            {
                readModel
            }, sourceFilePath);

            // Assert
            Assert.AreEqual(1, targetCollection.Count);
            Assert.AreSame(existingModel, targetCollection[0]);
            Assert.AreEqual(2, targetCollection[0].StochasticSoilProfiles.Count());
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                targetCollection,
                existingModel
            }, affectedObjects);
        }

        [Test]
        public void UpdateModelWithImportedData_ProfilesAssignedToCalculationsWithOneImportedModelProfileRemoved_OneProfileRemovedCalculationUpdatedAccordingly()
        {
            // Setup
            const string modelsName = "same model";
            PipingStochasticSoilModel existingModel = CreateSimpleModel(modelsName, "Unaffected Profile", "Removed Profile");

            var failureMechanism = new PipingFailureMechanism();
            PipingStochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            PipingStochasticSoilProfile firstExistingProfile = existingModel.StochasticSoilProfiles.First();
            PipingStochasticSoilModel readModel = CreateSimpleModel(modelsName, firstExistingProfile.SoilProfile.Name);

            var calculationWithNotUpdatedProfile = new PipingCalculationScenario(new GeneralPipingInput());
            calculationWithNotUpdatedProfile.InputParameters.StochasticSoilModel = existingModel;
            calculationWithNotUpdatedProfile.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles.ElementAt(0);
            calculationWithNotUpdatedProfile.Output = new PipingOutput(new PipingOutput.ConstructionProperties());

            var calculationWithDeletedProfile = new PipingCalculationScenario(new GeneralPipingInput());
            calculationWithDeletedProfile.InputParameters.StochasticSoilModel = existingModel;
            calculationWithDeletedProfile.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles.ElementAt(1);
            calculationWithDeletedProfile.Output = new PipingOutput(new PipingOutput.ConstructionProperties());

            failureMechanism.CalculationsGroup.Children.Add(calculationWithDeletedProfile);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithNotUpdatedProfile);

            var strategy = new PipingStochasticSoilModelUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new[]
            {
                readModel
            }, sourceFilePath).ToArray();

            // Assert
            PipingStochasticSoilModel firstSoilModel = targetCollection[0];
            Assert.AreSame(existingModel, firstSoilModel);
            Assert.AreSame(firstExistingProfile, firstSoilModel.StochasticSoilProfiles.ElementAt(0));

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
            const string modelsName = "same model";
            PipingStochasticSoilModel existingModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(modelsName, new[]
            {
                CreateStochasticSoilProfile("Updated Profile"),
                CreateStochasticSoilProfile("Unaffected Profile")
            });

            var failureMechanism = new PipingFailureMechanism();
            PipingStochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            PipingStochasticSoilProfile changedProfile = CloneAndSlightlyModify(existingModel.StochasticSoilProfiles.ElementAt(0));
            PipingStochasticSoilModel readModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(modelsName, new[]
            {
                changedProfile,
                CreateStochasticSoilProfile("Unaffected Profile")
            });

            var calculationWithUpdatedProfile = new PipingCalculationScenario(new GeneralPipingInput());
            calculationWithUpdatedProfile.InputParameters.StochasticSoilModel = existingModel;
            calculationWithUpdatedProfile.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles.ElementAt(0);
            calculationWithUpdatedProfile.Output = new PipingOutput(new PipingOutput.ConstructionProperties());

            var calculationWithNotUpdatedProfile = new PipingCalculationScenario(new GeneralPipingInput());
            calculationWithNotUpdatedProfile.InputParameters.StochasticSoilModel = existingModel;
            calculationWithNotUpdatedProfile.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles.ElementAt(1);
            calculationWithNotUpdatedProfile.Output = new PipingOutput(new PipingOutput.ConstructionProperties());

            failureMechanism.CalculationsGroup.Children.Add(calculationWithNotUpdatedProfile);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithUpdatedProfile);

            var strategy = new PipingStochasticSoilModelUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new[]
            {
                readModel
            }, sourceFilePath).ToArray();

            // Assert
            Assert.AreEqual(1, targetCollection.Count);
            PipingStochasticSoilModel firstSoilModel = targetCollection[0];
            Assert.AreSame(existingModel, firstSoilModel);

            PipingStochasticSoilProfile[] stochasticSoilProfiles = firstSoilModel.StochasticSoilProfiles.ToArray();
            Assert.AreEqual(2, stochasticSoilProfiles.Length);

            PipingStochasticSoilProfile updatedStochasticSoilProfile = existingModel.StochasticSoilProfiles.ElementAt(0);
            Assert.AreSame(updatedStochasticSoilProfile, stochasticSoilProfiles[0]);
            Assert.IsFalse(calculationWithUpdatedProfile.HasOutput);
            Assert.AreSame(updatedStochasticSoilProfile, calculationWithUpdatedProfile.InputParameters.StochasticSoilProfile);

            PipingStochasticSoilProfile unaffectedStochasticSoilProfile = existingModel.StochasticSoilProfiles.ElementAt(1);
            Assert.AreSame(unaffectedStochasticSoilProfile, stochasticSoilProfiles[1]);
            Assert.IsTrue(calculationWithNotUpdatedProfile.HasOutput);
            Assert.AreSame(unaffectedStochasticSoilProfile, calculationWithNotUpdatedProfile.InputParameters.StochasticSoilProfile);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                targetCollection,
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
            PipingStochasticSoilModel existingModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel();
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            calculation.InputParameters.StochasticSoilModel = existingModel;
            calculation.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles.First();
            calculation.Output = new PipingOutput(new PipingOutput.ConstructionProperties());

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            PipingStochasticSoilModelCollection pipingStochasticSoilModelCollection = failureMechanism.StochasticSoilModels;
            pipingStochasticSoilModelCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            var strategy = new PipingStochasticSoilModelUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new List<PipingStochasticSoilModel>(), sourceFilePath).ToArray();

            // Assert
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.InputParameters.StochasticSoilModel);
            Assert.IsNull(calculation.InputParameters.StochasticSoilProfile);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                pipingStochasticSoilModelCollection,
                calculation,
                calculation.InputParameters
            }, affectedObjects);
        }

        [Test]
        public void UpdateModelWithImportedData_ProfilesAssignedToCalculationsOneWithRemovedProfile_OnlyCalculationWithRemovedProfileUpdated()
        {
            // Setup
            const string modelsName = "same model";
            PipingStochasticSoilModel existingModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(modelsName, new[]
            {
                CreateStochasticSoilProfile("Removed Profile"),
                CreateStochasticSoilProfile("Unaffected Profile")
            });

            var failureMechanism = new PipingFailureMechanism();
            PipingStochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);
            PipingStochasticSoilProfile removedProfile = existingModel.StochasticSoilProfiles.ElementAt(0);
            PipingStochasticSoilProfile unaffectedProfile = existingModel.StochasticSoilProfiles.ElementAt(1);

            PipingStochasticSoilModel readModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(modelsName, new[]
            {
                CreateStochasticSoilProfile("Unaffected Profile")
            });

            var calculationWithRemovedProfile = new PipingCalculationScenario(new GeneralPipingInput());
            calculationWithRemovedProfile.InputParameters.StochasticSoilModel = existingModel;
            calculationWithRemovedProfile.InputParameters.StochasticSoilProfile = removedProfile;
            calculationWithRemovedProfile.Output = new PipingOutput(new PipingOutput.ConstructionProperties());

            var calculationWithNotUpdatedProfile = new PipingCalculationScenario(new GeneralPipingInput());
            calculationWithNotUpdatedProfile.InputParameters.StochasticSoilModel = existingModel;
            calculationWithNotUpdatedProfile.InputParameters.StochasticSoilProfile = unaffectedProfile;
            calculationWithNotUpdatedProfile.Output = new PipingOutput(new PipingOutput.ConstructionProperties());

            failureMechanism.CalculationsGroup.Children.Add(calculationWithNotUpdatedProfile);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithRemovedProfile);

            var strategy = new PipingStochasticSoilModelUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new[]
            {
                readModel
            }, sourceFilePath).ToArray();

            // Assert
            PipingStochasticSoilModel firstSoilModel = targetCollection[0];
            Assert.AreSame(existingModel, firstSoilModel);
            Assert.AreEqual(1, firstSoilModel.StochasticSoilProfiles.Count());
            Assert.AreSame(unaffectedProfile, firstSoilModel.StochasticSoilProfiles.First());

            Assert.IsFalse(calculationWithRemovedProfile.HasOutput);
            Assert.IsNull(calculationWithRemovedProfile.InputParameters.StochasticSoilProfile);

            Assert.IsTrue(calculationWithNotUpdatedProfile.HasOutput);
            Assert.AreSame(unaffectedProfile, calculationWithNotUpdatedProfile.InputParameters.StochasticSoilProfile);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                targetCollection,
                firstSoilModel,
                calculationWithRemovedProfile,
                calculationWithRemovedProfile.InputParameters
            }, affectedObjects);
        }

        /// <summary>
        /// Creates a simple model with names for the model and profiles in the model set as specified.
        /// </summary>
        /// <param name="modelName">Name of the created model.</param>
        /// <param name="profileNames">List of names for the profiles to be added to the model.</param>
        /// <returns>A new <see cref="PipingStochasticSoilModel"/>.</returns>
        private static PipingStochasticSoilModel CreateSimpleModel(string modelName, params string[] profileNames)
        {
            var stochasticProfiles = new List<PipingStochasticSoilProfile>();
            foreach (string profileName in profileNames)
            {
                stochasticProfiles.Add(new PipingStochasticSoilProfile(
                                           1.0 / profileNames.Length,
                                           PipingSoilProfileTestFactory.CreatePipingSoilProfile(profileName)));
            }

            return PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(modelName, stochasticProfiles);
        }

        private static PipingStochasticSoilProfile CloneAndSlightlyModify(PipingStochasticSoilProfile profile)
        {
            return new PipingStochasticSoilProfile(profile.Probability,
                                                   new PipingSoilProfile(
                                                       profile.SoilProfile.Name,
                                                       profile.SoilProfile.Bottom - 0.2,
                                                       profile.SoilProfile.Layers,
                                                       profile.SoilProfile.SoilProfileSourceType));
        }

        private static PipingStochasticSoilProfile CreateStochasticSoilProfile(string profileName)
        {
            var random = new Random(21);
            return new PipingStochasticSoilProfile(random.NextDouble(), PipingSoilProfileTestFactory.CreatePipingSoilProfile(profileName));
        }
    }
}