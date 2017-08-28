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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil;
using Ringtoets.MacroStabilityInwards.Plugin.FileImporter;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Plugin.Test.FileImporter
{
    [TestFixture]
    public class MacroStabilityInwardsStochasticSoilModelUpdateDataStrategyTest
    {
        private const string sourceFilePath = "path";

        [Test]
        public void Constructor_WithoutCalculations_CreatesNewInstance()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_WithCalculations_CreatesNewInstance()
        {
            // Call
            var strategy = new MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Assert
            Assert.IsInstanceOf<IStochasticSoilModelUpdateModelStrategy<MacroStabilityInwardsStochasticSoilModel>>(strategy);
            Assert.IsInstanceOf<UpdateDataStrategyBase<MacroStabilityInwardsStochasticSoilModel, MacroStabilityInwardsFailureMechanism>>(strategy);
        }

        [Test]
        public void UpdateModelWithImportedData_ReadStochasticSoilModelsNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());

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
            var strategy = new MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateModelWithImportedData(new List<MacroStabilityInwardsStochasticSoilModel>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateModelWithImportedData_WithCurrentModelsAndImportedMultipleModelsWithSameName_ThrowsUpdateDataException()
        {
            // Setup
            const string nonUniqueName = "non-unique name";

            var targetCollection = new MacroStabilityInwardsStochasticSoilModelCollection();
            targetCollection.AddRange(new[]
            {
                new TestStochasticSoilModel(nonUniqueName)
            }, sourceFilePath);

            var strategy = new MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());
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

            var targetCollection = new MacroStabilityInwardsStochasticSoilModelCollection();

            var strategy = new MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());
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
            var strategy = new MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());
            var targetCollection = new MacroStabilityInwardsStochasticSoilModelCollection();

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(importedStochasticSoilModels, "path");

            // Assert
            CollectionAssert.IsEmpty(targetCollection);
            CollectionAssert.IsEmpty(affectedObjects);
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
            var strategy = new MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy(failureMechanism);
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
            MacroStabilityInwardsStochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            var strategy = new MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy(failureMechanism);
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
            MacroStabilityInwardsStochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            var strategy = new MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy(failureMechanism);
            MacroStabilityInwardsStochasticSoilModel readModel = CreateSimpleModel(modelsName, "new profile A", "new profile B");

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
            MacroStabilityInwardsStochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            MacroStabilityInwardsStochasticSoilProfile firstExistingProfile = existingModel.StochasticSoilProfiles[0];
            MacroStabilityInwardsStochasticSoilModel readModel = CreateSimpleModel(modelsName, firstExistingProfile.SoilProfile.Name);

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

            var strategy = new MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new[]
            {
                readModel
            }, sourceFilePath).ToArray();

            // Assert
            MacroStabilityInwardsStochasticSoilModel firstSoilModel = targetCollection[0];
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
        public void UpdateModelWithImportedData_ProfilesAssignedToCalculations1DProfileChanged_OnlyCalculationWithChangedProfileUpdated()
        {
            // Setup
            const string modelsName = "same model";
            MacroStabilityInwardsStochasticSoilModel existingModel = CreateStochasticSoilModel(modelsName);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            MacroStabilityInwardsStochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            MacroStabilityInwardsStochasticSoilModel readModel = CreateStochasticSoilModel(modelsName);
            MacroStabilityInwardsStochasticSoilProfile changedProfile = CloneAndSlightlyModify1DProfile(readModel.StochasticSoilProfiles.ElementAt(0));
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

            var strategy = new MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new[]
            {
                readModel
            }, sourceFilePath).ToArray();

            // Assert
            MacroStabilityInwardsStochasticSoilModel firstSoilModel = targetCollection[0];
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
        public void UpdateModelWithImportedData_ProfilesAssignedToCalculations2DProfileChanged_OnlyCalculationWithChangedProfileUpdated()
        {
            // Setup
            const string modelsName = "same model";
            MacroStabilityInwardsStochasticSoilModel existingModel = CreateStochasticSoilModel(modelsName);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            MacroStabilityInwardsStochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            MacroStabilityInwardsStochasticSoilModel readModel = CreateStochasticSoilModel(modelsName);
            MacroStabilityInwardsStochasticSoilProfile changedProfile = CloneAndSlightlyModify2DProfile(readModel.StochasticSoilProfiles.ElementAt(1));
            readModel.StochasticSoilProfiles[1] = changedProfile;

            var calculationWithNotUpdatedProfile = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            calculationWithNotUpdatedProfile.InputParameters.StochasticSoilModel = existingModel;
            calculationWithNotUpdatedProfile.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles[0];
            calculationWithNotUpdatedProfile.Output = new MacroStabilityInwardsOutput();

            var calculationWithUpdatedProfile = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            calculationWithUpdatedProfile.InputParameters.StochasticSoilModel = existingModel;
            calculationWithUpdatedProfile.InputParameters.StochasticSoilProfile = existingModel.StochasticSoilProfiles[1];
            calculationWithUpdatedProfile.Output = new MacroStabilityInwardsOutput();

            failureMechanism.CalculationsGroup.Children.Add(calculationWithUpdatedProfile);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithNotUpdatedProfile);

            var strategy = new MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new[]
            {
                readModel
            }, sourceFilePath).ToArray();

            // Assert
            MacroStabilityInwardsStochasticSoilModel firstSoilModel = targetCollection[0];
            Assert.AreSame(existingModel, firstSoilModel);
            Assert.AreEqual(2, firstSoilModel.StochasticSoilProfiles.Count);
            Assert.AreSame(existingModel.StochasticSoilProfiles[0], firstSoilModel.StochasticSoilProfiles[0]);
            Assert.AreSame(existingModel.StochasticSoilProfiles[1], firstSoilModel.StochasticSoilProfiles[1]);

            Assert.IsFalse(calculationWithUpdatedProfile.HasOutput);
            Assert.AreSame(existingModel.StochasticSoilProfiles[1], calculationWithUpdatedProfile.InputParameters.StochasticSoilProfile);

            Assert.IsTrue(calculationWithNotUpdatedProfile.HasOutput);
            Assert.AreSame(existingModel.StochasticSoilProfiles[0], calculationWithNotUpdatedProfile.InputParameters.StochasticSoilProfile);

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
            MacroStabilityInwardsStochasticSoilModelCollection stochasticSoilModelCollection = failureMechanism.StochasticSoilModels;
            stochasticSoilModelCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            var strategy = new MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new List<MacroStabilityInwardsStochasticSoilModel>(), sourceFilePath).ToArray();

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
            MacroStabilityInwardsStochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);
            MacroStabilityInwardsStochasticSoilProfile removedProfile = existingModel.StochasticSoilProfiles[0];
            MacroStabilityInwardsStochasticSoilProfile unaffectedProfile = existingModel.StochasticSoilProfiles[1];

            MacroStabilityInwardsStochasticSoilModel readModel = new TestStochasticSoilModel(modelsName);
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

            var strategy = new MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateModelWithImportedData(new[]
            {
                readModel
            }, sourceFilePath).ToArray();

            // Assert
            MacroStabilityInwardsStochasticSoilModel firstSoilModel = targetCollection[0];
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

        private static MacroStabilityInwardsStochasticSoilModel CreateStochasticSoilModel(string modelsName)
        {
            var model = new MacroStabilityInwardsStochasticSoilModel(modelsName);

            model.StochasticSoilProfiles.AddRange(new[]
            {
                new MacroStabilityInwardsStochasticSoilProfile(0.5, new MacroStabilityInwardsSoilProfile1D(
                                                                   "A",
                                                                   0.0,
                                                                   new[]
                                                                   {
                                                                       new MacroStabilityInwardsSoilLayer1D(0.0)
                                                                   },
                                                                   SoilProfileType.SoilProfile1D,
                                                                   0)),
                new MacroStabilityInwardsStochasticSoilProfile(0.5, new MacroStabilityInwardsSoilProfile2D(
                                                                   "B",
                                                                   new[]
                                                                   {
                                                                       new MacroStabilityInwardsSoilLayer2D(new Ring(new[]
                                                                       {
                                                                           new Point2D(3, 2),
                                                                           new Point2D(4, 5)
                                                                       }), Enumerable.Empty<Ring>())
                                                                   },
                                                                   SoilProfileType.SoilProfile2D,
                                                                   0))
            });
            return model;
        }

        /// <summary>
        /// Creates a simple model with names for the model and profiles in the model set as specified.
        /// </summary>
        /// <param name="modelName">Name of the created model.</param>
        /// <param name="profileNames">List of names for the profiles to be added to the model.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsStochasticSoilModel"/>.</returns>
        private static MacroStabilityInwardsStochasticSoilModel CreateSimpleModel(string modelName, params string[] profileNames)
        {
            var model = new MacroStabilityInwardsStochasticSoilModel(modelName);
            foreach (string profileName in profileNames)
            {
                model.StochasticSoilProfiles.Add(
                    new MacroStabilityInwardsStochasticSoilProfile(1.0 / profileNames.Length, new TestMacroStabilityInwardsSoilProfile1D(profileName)));
            }
            return model;
        }

        private static MacroStabilityInwardsStochasticSoilProfile CloneAndSlightlyModify2DProfile(MacroStabilityInwardsStochasticSoilProfile profile)
        {
            var soilProfile = (MacroStabilityInwardsSoilProfile2D) profile.SoilProfile;
            return new MacroStabilityInwardsStochasticSoilProfile(profile.Probability, new MacroStabilityInwardsSoilProfile2D(
                                                                      soilProfile.Name,
                                                                      soilProfile.Layers.Select(s => new MacroStabilityInwardsSoilLayer2D(
                                                                                                    new Ring(s.OuterRing.Points.Select(p => new Point2D(p.Y - 1, p.Y))),
                                                                                                    s.Holes)),
                                                                      soilProfile.SoilProfileType,
                                                                      soilProfile.MacroStabilityInwardsSoilProfileId));
        }

        private static MacroStabilityInwardsStochasticSoilProfile CloneAndSlightlyModify1DProfile(MacroStabilityInwardsStochasticSoilProfile profile)
        {
            var soilProfile = (MacroStabilityInwardsSoilProfile1D) profile.SoilProfile;
            return new MacroStabilityInwardsStochasticSoilProfile(profile.Probability, new MacroStabilityInwardsSoilProfile1D(
                                                                      soilProfile.Name,
                                                                      soilProfile.Bottom - 0.2,
                                                                      soilProfile.Layers,
                                                                      soilProfile.SoilProfileType,
                                                                      soilProfile.MacroStabilityInwardsSoilProfileId));
        }
    }
}