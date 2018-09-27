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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;
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
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(nonUniqueName)
            }, sourceFilePath);

            var strategy = new MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());
            MacroStabilityInwardsStochasticSoilModel[] importedStochasticSoilModels =
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(nonUniqueName),
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(nonUniqueName)
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
            MacroStabilityInwardsStochasticSoilModel[] importedStochasticSoilModels =
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(nonUniqueName),
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(nonUniqueName)
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
            IEnumerable<MacroStabilityInwardsStochasticSoilModel> importedStochasticSoilModels = Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>();
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
            MacroStabilityInwardsStochasticSoilModel existingModel = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("existing");

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);
            var strategy = new MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy(failureMechanism);
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
        public void UpdateModelWithImportedData_WithCurrentModelAndImportedModelWithSameName_ModelUpdated()
        {
            // Setup
            const string modelsName = "same model";
            MacroStabilityInwardsStochasticSoilModel existingModel = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(modelsName);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            MacroStabilityInwardsStochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            var strategy = new MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy(failureMechanism);
            MacroStabilityInwardsStochasticSoilModel readModel = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(modelsName);

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
            MacroStabilityInwardsStochasticSoilModel existingModel = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(modelsName);

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
            Assert.AreEqual(existingModel.StochasticSoilProfiles.Count(), targetCollection[0].StochasticSoilProfiles.Count());
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
            MacroStabilityInwardsStochasticSoilModel existingModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(modelsName,
                                                                                                   CreateStochasticSoilProfiles());

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            MacroStabilityInwardsStochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            MacroStabilityInwardsStochasticSoilProfile firstExistingProfile = existingModel.StochasticSoilProfiles.ElementAt(0);
            MacroStabilityInwardsStochasticSoilModel readModel = CreateSimpleModel(modelsName, firstExistingProfile.SoilProfile.Name);

            var calculationWithNotUpdatedProfile = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    StochasticSoilModel = existingModel,
                    StochasticSoilProfile = existingModel.StochasticSoilProfiles.ElementAt(0)
                },
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

            var calculationWithDeletedProfile = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    StochasticSoilModel = existingModel,
                    StochasticSoilProfile = existingModel.StochasticSoilProfiles.ElementAt(1)
                },
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

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
            Assert.AreEqual(1, firstSoilModel.StochasticSoilProfiles.Count());
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
        public void UpdateModelWithImportedData_ProfilesAssignedToCalculations1DProfileChanged_OnlyCalculationWithChangedProfileUpdated()
        {
            // Setup
            const string modelsName = "same model";
            MacroStabilityInwardsStochasticSoilModel existingModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(modelsName, CreateStochasticSoilProfiles());

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            MacroStabilityInwardsStochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            List<MacroStabilityInwardsStochasticSoilProfile> readStochasticSoilProfiles = CreateStochasticSoilProfiles();
            MacroStabilityInwardsStochasticSoilProfile changedProfile = CloneAndSlightlyModify1DProfile(readStochasticSoilProfiles[0]);
            readStochasticSoilProfiles[0] = changedProfile;
            MacroStabilityInwardsStochasticSoilModel readModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(modelsName, readStochasticSoilProfiles);

            var calculationWithUpdatedProfile = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    StochasticSoilModel = existingModel,
                    StochasticSoilProfile = existingModel.StochasticSoilProfiles.ElementAt(0)
                },
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

            var calculationWithNotUpdatedProfile = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    StochasticSoilModel = existingModel,
                    StochasticSoilProfile = existingModel.StochasticSoilProfiles.ElementAt(1)
                },
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

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
            Assert.AreEqual(2, firstSoilModel.StochasticSoilProfiles.Count());

            MacroStabilityInwardsStochasticSoilProfile updatedStochasticSoilProfile = existingModel.StochasticSoilProfiles.ElementAt(0);
            Assert.AreSame(updatedStochasticSoilProfile, firstSoilModel.StochasticSoilProfiles.ElementAt(0));
            Assert.IsFalse(calculationWithUpdatedProfile.HasOutput);
            Assert.AreSame(updatedStochasticSoilProfile, calculationWithUpdatedProfile.InputParameters.StochasticSoilProfile);

            MacroStabilityInwardsStochasticSoilProfile unaffectedStochasticSoilProfile = existingModel.StochasticSoilProfiles.ElementAt(1);
            Assert.AreSame(unaffectedStochasticSoilProfile, firstSoilModel.StochasticSoilProfiles.ElementAt(1));
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
        public void UpdateModelWithImportedData_ProfilesAssignedToCalculations2DProfileChanged_OnlyCalculationWithChangedProfileUpdated()
        {
            // Setup
            const string modelsName = "same model";

            MacroStabilityInwardsStochasticSoilModel existingModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(modelsName, CreateStochasticSoilProfiles());

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            MacroStabilityInwardsStochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);

            List<MacroStabilityInwardsStochasticSoilProfile> readStochasticSoilProfiles = CreateStochasticSoilProfiles();
            MacroStabilityInwardsStochasticSoilProfile changedProfile = CloneAndSlightlyModify2DProfile(readStochasticSoilProfiles[1]);
            readStochasticSoilProfiles[1] = changedProfile;
            MacroStabilityInwardsStochasticSoilModel readModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(modelsName, readStochasticSoilProfiles);

            var calculationWithNotUpdatedProfile = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    StochasticSoilModel = existingModel,
                    StochasticSoilProfile = existingModel.StochasticSoilProfiles.ElementAt(0)
                },
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

            var calculationWithUpdatedProfile = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    StochasticSoilModel = existingModel,
                    StochasticSoilProfile = existingModel.StochasticSoilProfiles.ElementAt(1)
                },
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

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
            Assert.AreEqual(2, firstSoilModel.StochasticSoilProfiles.Count());

            MacroStabilityInwardsStochasticSoilProfile unaffectedStochasticSoilProfile = existingModel.StochasticSoilProfiles.ElementAt(0);
            Assert.AreSame(unaffectedStochasticSoilProfile, firstSoilModel.StochasticSoilProfiles.ElementAt(0));
            Assert.IsTrue(calculationWithNotUpdatedProfile.HasOutput);
            Assert.AreSame(unaffectedStochasticSoilProfile, calculationWithNotUpdatedProfile.InputParameters.StochasticSoilProfile);

            MacroStabilityInwardsStochasticSoilProfile updatedStochasticSoilProfile = existingModel.StochasticSoilProfiles.ElementAt(1);
            Assert.AreSame(updatedStochasticSoilProfile, firstSoilModel.StochasticSoilProfiles.ElementAt(1));
            Assert.IsFalse(calculationWithUpdatedProfile.HasOutput);
            Assert.AreSame(updatedStochasticSoilProfile, calculationWithUpdatedProfile.InputParameters.StochasticSoilProfile);

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
            MacroStabilityInwardsStochasticSoilModel existingModel = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel();
            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    StochasticSoilModel = existingModel,
                    StochasticSoilProfile = existingModel.StochasticSoilProfiles.ElementAt(0)
                },
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

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
            MacroStabilityInwardsStochasticSoilModel existingModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(modelsName, CreateStochasticSoilProfiles());

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            MacroStabilityInwardsStochasticSoilModelCollection targetCollection = failureMechanism.StochasticSoilModels;
            targetCollection.AddRange(new[]
            {
                existingModel
            }, sourceFilePath);
            MacroStabilityInwardsStochasticSoilProfile removedProfile = existingModel.StochasticSoilProfiles.ElementAt(0);
            MacroStabilityInwardsStochasticSoilProfile unaffectedProfile = existingModel.StochasticSoilProfiles.ElementAt(1);

            List<MacroStabilityInwardsStochasticSoilProfile> readStochasticSoilProfiles = CreateStochasticSoilProfiles();
            readStochasticSoilProfiles.RemoveAt(0);
            MacroStabilityInwardsStochasticSoilModel readModel = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(modelsName, readStochasticSoilProfiles);

            var calculationWithRemovedProfile = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    StochasticSoilModel = existingModel,
                    StochasticSoilProfile = removedProfile
                },
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

            var calculationWithNotUpdatedProfile = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    StochasticSoilModel = existingModel,
                    StochasticSoilProfile = unaffectedProfile
                },
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

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
            Assert.AreEqual(1, firstSoilModel.StochasticSoilProfiles.Count());
            Assert.AreSame(unaffectedProfile, firstSoilModel.StochasticSoilProfiles.ElementAt(0));

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
        /// Creates a collection of <see cref="MacroStabilityInwardsStochasticSoilProfile"/>
        /// with all the supported <see cref="IMacroStabilityInwardsSoilProfile{T}"/>.
        /// </summary>
        /// <returns>A collection of <see cref="MacroStabilityInwardsStochasticSoilProfile"/>.</returns>
        private static List<MacroStabilityInwardsStochasticSoilProfile> CreateStochasticSoilProfiles()
        {
            return new List<MacroStabilityInwardsStochasticSoilProfile>
            {
                new MacroStabilityInwardsStochasticSoilProfile(0.5, MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D()),
                new MacroStabilityInwardsStochasticSoilProfile(0.5, MacroStabilityInwardsSoilProfile2DTestFactory.CreateMacroStabilityInwardsSoilProfile2D())
            };
        }

        /// <summary>
        /// Creates a simple model with names for the model and profiles in the model set as specified.
        /// </summary>
        /// <param name="modelName">Name of the created model.</param>
        /// <param name="profileNames">List of names for the profiles to be added to the model.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsStochasticSoilModel"/>.</returns>
        private static MacroStabilityInwardsStochasticSoilModel CreateSimpleModel(string modelName, params string[] profileNames)
        {
            var stochasticSoilProfiles = new List<MacroStabilityInwardsStochasticSoilProfile>();
            foreach (string profileName in profileNames)
            {
                MacroStabilityInwardsSoilProfile1D soilProfile = MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D(profileName);
                stochasticSoilProfiles.Add(new MacroStabilityInwardsStochasticSoilProfile(1.0 / profileNames.Length, soilProfile));
            }

            return MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel(modelName, stochasticSoilProfiles);
        }

        private static MacroStabilityInwardsStochasticSoilProfile CloneAndSlightlyModify2DProfile(MacroStabilityInwardsStochasticSoilProfile profile)
        {
            var soilProfile = (MacroStabilityInwardsSoilProfile2D) profile.SoilProfile;
            return new MacroStabilityInwardsStochasticSoilProfile(profile.Probability, new MacroStabilityInwardsSoilProfile2D(
                                                                      soilProfile.Name,
                                                                      soilProfile.Layers.Select(s => new MacroStabilityInwardsSoilLayer2D(
                                                                                                    new Ring(s.OuterRing.Points.Select(p => new Point2D(p.Y - 1, p.Y))),
                                                                                                    s.Data,
                                                                                                    s.NestedLayers)),
                                                                      soilProfile.PreconsolidationStresses.Select(stress => new MacroStabilityInwardsPreconsolidationStress(new Point2D(stress.Location.X + 1,
                                                                                                                                                                                        stress.Location.Y),
                                                                                                                                                                            stress.Stress))));
        }

        private static MacroStabilityInwardsStochasticSoilProfile CloneAndSlightlyModify1DProfile(MacroStabilityInwardsStochasticSoilProfile profile)
        {
            var soilProfile = (MacroStabilityInwardsSoilProfile1D) profile.SoilProfile;
            return new MacroStabilityInwardsStochasticSoilProfile(profile.Probability, new MacroStabilityInwardsSoilProfile1D(
                                                                      soilProfile.Name,
                                                                      soilProfile.Bottom - 0.2,
                                                                      soilProfile.Layers));
        }
    }
}