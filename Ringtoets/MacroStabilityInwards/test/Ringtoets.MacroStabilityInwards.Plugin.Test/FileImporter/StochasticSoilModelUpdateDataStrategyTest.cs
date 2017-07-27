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
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.IO.Importers;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil;
using Ringtoets.MacroStabilityInwards.Plugin.FileImporter;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Plugin.Test.FileImporter
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
            var strategy = new StochasticSoilModelUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Assert
            Assert.IsInstanceOf<IStochasticSoilModelUpdateModelStrategy>(strategy);
            Assert.IsInstanceOf<UpdateDataStrategyBase<StochasticSoilModel, MacroStabilityInwardsFailureMechanism>>(strategy);
        }

        [Test]
        public void UpdateModelWithImportedData_ReadStochasticSoilModelsNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new StochasticSoilModelUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());

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
            var strategy = new StochasticSoilModelUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(new List<StochasticSoilModel>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateModelWithImportedData_WithCurrentModelsAndImportedMultipleModelsWithSameName_ThrowsUpdateDataException()
        {
            // Setup
            const string nonUniqueName = "non-unique name";

            var targetCollection = new StochasticSoilModelCollection();
            targetCollection.AddRange(new[]
            {
                new TestStochasticSoilModel(nonUniqueName)
            }, sourceFilePath);

            var strategy = new StochasticSoilModelUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());
            var importedStochasticSoilModels = new[]
            {
                new TestStochasticSoilModel(nonUniqueName),
                new TestStochasticSoilModel(nonUniqueName)
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

            var targetCollection = new StochasticSoilModelCollection();

            var strategy = new StochasticSoilModelUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());
            var importedStochasticSoilModels = new[]
            {
                new TestStochasticSoilModel(nonUniqueName),
                new TestStochasticSoilModel(nonUniqueName)
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
            IEnumerable<TestStochasticSoilModel> importedStochasticSoilModels = Enumerable.Empty<TestStochasticSoilModel>();
            var strategy = new StochasticSoilModelUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());
            var targetCollection = new StochasticSoilModelCollection();

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(importedStochasticSoilModels, "path");

            // Assert
            Assert.IsEmpty(targetCollection);
            Assert.IsEmpty(affectedObjects);
        }

        [Test]
        public void UpdateModelWithImportedData_WithCurrentModelAndImportedModelWithOtherName_ModelReplaced()
        {
            // Setup
            var existingModel = new TestStochasticSoilModel("existing");

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);
            var strategy = new StochasticSoilModelUpdateDataStrategy(failureMechanism);
            var readModel = new TestStochasticSoilModel("read");

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
        public void UpdateModelWithImportedData_WithCurrentModelAndImportedModelWithSameName_ModelUpdated()
        {
            // Setup
            const string modelsName = "same model";
            var existingModel = new TestStochasticSoilModel(modelsName);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            StochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            var strategy = new StochasticSoilModelUpdateDataStrategy(failureMechanism);
            var readModel = new TestStochasticSoilModel(modelsName);

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
            var existingModel = new TestStochasticSoilModel(modelsName);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            StochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            var strategy = new StochasticSoilModelUpdateDataStrategy(failureMechanism);
            StochasticSoilModel readModel = CreateSimpleModel(modelsName, "new profile A", "new profile B");

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new[]
            {
                readModel
            }, sourceFilePath);

            // Assert
            Assert.AreEqual(1, targetCollection.Count);
            Assert.AreSame(existingModel, targetCollection[0]);
            Assert.AreEqual(2, targetCollection[0].StochasticSoilProfiles.Count);
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
            var existingModel = new TestStochasticSoilModel(modelsName);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            StochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            StochasticSoilProfile firstExistingProfile = existingModel.StochasticSoilProfiles[0];
            StochasticSoilModel readModel = CreateSimpleModel(modelsName, firstExistingProfile.SoilProfile.Name);

            var calculationWithNotUpdatedProfile = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            calculationWithNotUpdatedProfile.InputParameters.StochasticSoilModel = existingModel;
            calculationWithNotUpdatedProfile.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles[0];
            calculationWithNotUpdatedProfile.Output = new MacroStabilityInwardsOutput();

            var calculationWithDeletedProfile = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            calculationWithDeletedProfile.InputParameters.StochasticSoilModel = existingModel;
            calculationWithDeletedProfile.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles[1];
            calculationWithDeletedProfile.Output = new MacroStabilityInwardsOutput();

            failureMechanism.CalculationsGroup.Children.Add(calculationWithDeletedProfile);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithNotUpdatedProfile);

            var strategy = new StochasticSoilModelUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new[]
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
            const string modelsName = "same model";
            StochasticSoilModel existingModel = CreateStochasticSoilModel(modelsName);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            StochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            StochasticSoilModel readModel = CreateStochasticSoilModel(modelsName);
            StochasticSoilProfile changedProfile = CloneAndSlightlyModify(readModel.StochasticSoilProfiles.ElementAt(0));
            readModel.StochasticSoilProfiles[0] = changedProfile;

            var calculationWithUpdatedProfile = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            calculationWithUpdatedProfile.InputParameters.StochasticSoilModel = existingModel;
            calculationWithUpdatedProfile.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles[0];
            calculationWithUpdatedProfile.Output = new MacroStabilityInwardsOutput();

            var calculationWithNotUpdatedProfile = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            calculationWithNotUpdatedProfile.InputParameters.StochasticSoilModel = existingModel;
            calculationWithNotUpdatedProfile.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles[1];
            calculationWithNotUpdatedProfile.Output = new MacroStabilityInwardsOutput();

            failureMechanism.CalculationsGroup.Children.Add(calculationWithNotUpdatedProfile);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithUpdatedProfile);

            var strategy = new StochasticSoilModelUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new[]
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
            var existingModel = new TestStochasticSoilModel();
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            calculation.InputParameters.StochasticSoilModel = existingModel;
            calculation.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles[0];
            calculation.Output = new MacroStabilityInwardsOutput();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            StochasticSoilModelCollection stochasticSoilModelCollection = failureMechanism.StochasticSoilModels;
            stochasticSoilModelCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            var strategy = new StochasticSoilModelUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new List<StochasticSoilModel>(), sourceFilePath).ToArray();

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

        [Test]
        public void UpdateModelWithImportedData_ProfilesAssignedToCalculationsOneWithRemovedProfile_OnlyCalculationWithRemovedProfileUpdated()
        {
            // Setup
            const string modelsName = "same model";
            var existingModel = new TestStochasticSoilModel(modelsName);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            StochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);
            StochasticSoilProfile removedProfile = existingModel.StochasticSoilProfiles[0];
            StochasticSoilProfile unaffectedProfile = existingModel.StochasticSoilProfiles[1];

            StochasticSoilModel readModel = new TestStochasticSoilModel(modelsName);
            readModel.StochasticSoilProfiles.RemoveAt(0);

            var calculationWithRemovedProfile = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            calculationWithRemovedProfile.InputParameters.StochasticSoilModel = existingModel;
            calculationWithRemovedProfile.InputParameters.StochasticSoilProfile = removedProfile;
            calculationWithRemovedProfile.Output = new MacroStabilityInwardsOutput();

            var calculationWithNotUpdatedProfile = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            calculationWithNotUpdatedProfile.InputParameters.StochasticSoilModel = existingModel;
            calculationWithNotUpdatedProfile.InputParameters.StochasticSoilProfile = unaffectedProfile;
            calculationWithNotUpdatedProfile.Output = new MacroStabilityInwardsOutput();

            failureMechanism.CalculationsGroup.Children.Add(calculationWithNotUpdatedProfile);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithRemovedProfile);

            var strategy = new StochasticSoilModelUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new[]
            {
                readModel
            }, sourceFilePath).ToArray();

            // Assert
            StochasticSoilModel firstSoilModel = targetCollection[0];
            Assert.AreSame(existingModel, firstSoilModel);
            Assert.AreEqual(1, firstSoilModel.StochasticSoilProfiles.Count);
            Assert.AreSame(unaffectedProfile, firstSoilModel.StochasticSoilProfiles[0]);

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

        private static StochasticSoilModel CreateStochasticSoilModel(string modelsName)
        {
            var model = new StochasticSoilModel(modelsName);

            model.StochasticSoilProfiles.AddRange(new[]
            {
                new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 0)
                {
                    SoilProfile = new MacroStabilityInwardsSoilProfile1D(
                        "A",
                        0.0,
                        new[]
                        {
                            new MacroStabilityInwardsSoilLayer1D(0.0)
                        },
                        SoilProfileType.SoilProfile1D,
                        0)
                },
                new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 0)
                {
                    SoilProfile = new MacroStabilityInwardsSoilProfile1D(
                        "B",
                        0.0,
                        new[]
                        {
                            new MacroStabilityInwardsSoilLayer1D(0.0)
                        },
                        SoilProfileType.SoilProfile1D,
                        0)
                }
            });
            return model;
        }

        /// <summary>
        /// Creates a simple model with names for the model and profiles in the model set as specified.
        /// </summary>
        /// <param name="modelName">Name of the created model.</param>
        /// <param name="profileNames">List of names for the profiles to be added to the model.</param>
        /// <returns>A new <see cref="StochasticSoilModel"/>.</returns>
        private static StochasticSoilModel CreateSimpleModel(string modelName, params string[] profileNames)
        {
            var model = new StochasticSoilModel(modelName);
            foreach (string profileName in profileNames)
            {
                model.StochasticSoilProfiles.Add(
                    new StochasticSoilProfile(1.0 / profileNames.Length, SoilProfileType.SoilProfile1D, -1)
                    {
                        SoilProfile = new TestMacroStabilityInwardsSoilProfile1D(profileName)
                    });
            }
            return model;
        }

        private static StochasticSoilProfile CloneAndSlightlyModify(StochasticSoilProfile profile)
        {
            var soilProfile = (MacroStabilityInwardsSoilProfile1D) profile.SoilProfile;
            return new StochasticSoilProfile(profile.Probability, profile.SoilProfileType, profile.SoilProfileId)
            {
                SoilProfile = new MacroStabilityInwardsSoilProfile1D(
                    soilProfile.Name,
                    soilProfile.Bottom - 0.2,
                    soilProfile.Layers,
                    soilProfile.SoilProfileType,
                    soilProfile.MacroStabilityInwardsSoilProfileId)
            };
        }
    }
}