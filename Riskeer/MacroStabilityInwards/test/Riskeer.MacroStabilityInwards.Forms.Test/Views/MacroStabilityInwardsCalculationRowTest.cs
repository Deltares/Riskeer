// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Riskeer.MacroStabilityInwards.Forms.Views;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationRowTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);

            MacroStabilityInwardsStochasticSoilModel stochasticSoilModel = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel();
            MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile = stochasticSoilModel.StochasticSoilProfiles.First();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine,
                    StochasticSoilModel = stochasticSoilModel,
                    StochasticSoilProfile = stochasticSoilProfile,
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            // Call
            var row = new MacroStabilityInwardsCalculationRow(calculation, handler);

            // Assert
            Assert.AreSame(calculation, row.Calculation);
            Assert.AreEqual(calculation.Name, row.Name);
            Assert.AreSame(calculation.InputParameters.StochasticSoilModel, row.StochasticSoilModel.WrappedObject);
            Assert.AreSame(calculation.InputParameters.StochasticSoilProfile, row.StochasticSoilProfile.WrappedObject);
            Assert.AreEqual(2, row.StochasticSoilProfileProbability.NumberOfDecimalPlaces);
            Assert.AreEqual(calculation.InputParameters.StochasticSoilProfile.Probability * 100, row.StochasticSoilProfileProbability, row.StochasticSoilProfileProbability.GetAccuracy());
            Assert.AreSame(hydraulicBoundaryLocation, row.SelectableHydraulicBoundaryLocation.WrappedObject.HydraulicBoundaryLocation);
            mocks.VerifyAll();
        }

        [Test]
        public void StochasticSoilModel_AlwaysOnChange_NotifyObserverCalculationPropertyChangedOutputCleared()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilModel newModel = MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel();
            var newValue = new DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilModel>(newModel);

            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.StochasticSoilModel = newValue, calculation);
        }

        [Test]
        public void StochasticSoilModel_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilModel> oldValue = null;

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.StochasticSoilModel;
                    row.StochasticSoilModel = row.StochasticSoilModel;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue.WrappedObject, calculation.InputParameters.StochasticSoilModel);
                });
        }

        [Test]
        public void StochasticSoilProfile_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            MacroStabilityInwardsSoilProfile1D soilProfile = MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D();
            var newProfile = new MacroStabilityInwardsStochasticSoilProfile(0, soilProfile);
            var newValue = new DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilProfile>(newProfile);

            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.StochasticSoilProfile = newValue, calculation);
        }

        [Test]
        public void StochasticSoilProfile_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            DataGridViewComboBoxItemWrapper<MacroStabilityInwardsStochasticSoilProfile> oldValue = null;

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.StochasticSoilProfile;
                    row.StochasticSoilProfile = row.StochasticSoilProfile;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue.WrappedObject, calculation.InputParameters.StochasticSoilProfile);
                });
        }

        [Test]
        public void SelectableHydraulicBoundaryLocation_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newLocation = new TestHydraulicBoundaryLocation();
            var selectableHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(newLocation, new Point2D(0, 0));
            var newValue = new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(selectableHydraulicBoundaryLocation);

            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.SelectableHydraulicBoundaryLocation = newValue, calculation);
        }

        [Test]
        public void SelectableHydraulicBoundaryLocation_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation> oldValue = null;

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.SelectableHydraulicBoundaryLocation;
                    row.SelectableHydraulicBoundaryLocation = row.SelectableHydraulicBoundaryLocation;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue.WrappedObject.HydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);
                });
        }

        /// <summary>
        /// Asserts that the output of a <see cref="MacroStabilityInwardsCalculationScenario"/> remains
        /// unaffected (and therefore no change notification occurring) when the input for
        /// that calculation has been changed using an instance of <see cref="MacroStabilityInwardsCalculationRow"/>.
        /// </summary>
        /// <param name="setProperty">The function that changes a property of the <see cref="MacroStabilityInwardsCalculationRow"/>
        /// instance. This function should not throw exceptions.</param>
        /// <param name="assertions">The additional assertions to be performed on the <see cref="MacroStabilityInwardsCalculationScenario"/>
        /// whose input has been changed.</param>
        private static void AssertPropertyNotChanged(
            Action<MacroStabilityInwardsCalculationRow> setProperty,
            Action<MacroStabilityInwardsCalculationScenario> assertions)
        {
            AssertPropertyChangeWithOrWithoutCalculationOutput(setProperty, assertions, true, false);
            AssertPropertyChangeWithOrWithoutCalculationOutput(setProperty, assertions, false, false);
        }

        private static void AssertPropertyChangeWithOrWithoutCalculationOutput(
            Action<MacroStabilityInwardsCalculationRow> setProperty,
            Action<MacroStabilityInwardsCalculationScenario> assertions,
            bool hasOutput,
            bool expectUpdates)
        {
            // Setup
            var mockRepository = new MockRepository();
            var inputObserver = mockRepository.StrictMock<IObserver>();
            if (expectUpdates)
            {
                inputObserver.Expect(o => o.UpdateObserver());
            }

            var calculationObserver = mockRepository.StrictMock<IObserver>();
            if (expectUpdates && hasOutput)
            {
                calculationObserver.Expect(o => o.UpdateObserver());
            }

            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            MacroStabilityInwardsOutput assignedOutput = null;

            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            if (hasOutput)
            {
                assignedOutput = MacroStabilityInwardsOutputTestFactory.CreateOutput();
            }

            calculation.Output = assignedOutput;

            var row = new MacroStabilityInwardsCalculationRow(calculation, handler);
            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(inputObserver);

            // Call
            setProperty(row);

            // Assert
            assertions(calculation);
            if (expectUpdates)
            {
                Assert.IsNull(calculation.Output);
            }
            else
            {
                Assert.AreSame(assignedOutput, calculation.Output);
            }

            mockRepository.VerifyAll();
        }

        private static void SetPropertyAndVerifyNotificationsAndOutputForCalculation(
            Action<MacroStabilityInwardsCalculationRow> setProperty,
            MacroStabilityInwardsCalculationScenario calculation)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var handler = new SetPropertyValueAfterConfirmationParameterTester(
                new[]
                {
                    observable
                });

            var row = new MacroStabilityInwardsCalculationRow(calculation, handler);

            // Call
            setProperty(row);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }
    }
}