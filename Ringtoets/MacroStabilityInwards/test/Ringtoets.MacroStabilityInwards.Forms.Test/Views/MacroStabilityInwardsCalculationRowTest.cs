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
using System.Globalization;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.Views;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationRowTest
    {
        [Test]
        public void Constructor_WithoutCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new MacroStabilityInwardsCalculationRow(null, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("macroStabilityInwardsCalculation", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutHandler_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsCalculationRow(new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput()), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("handler", paramName);
        }

        [Test]
        public void Constructor_WithCalculation_PropertiesFromCalculation()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();

            // Call
            var row = new MacroStabilityInwardsCalculationRow(calculation, handler);

            // Assert
            Assert.AreSame(calculation, row.MacroStabilityInwardsCalculation);
            Assert.AreEqual(calculation.Name, row.Name);
            Assert.AreSame(calculation.InputParameters.StochasticSoilModel, row.StochasticSoilModel.WrappedObject);
            Assert.AreSame(calculation.InputParameters.StochasticSoilProfile, row.StochasticSoilProfile.WrappedObject);
            Assert.AreEqual(calculation.InputParameters.StochasticSoilProfile.Probability.ToString(CultureInfo.CurrentCulture), row.StochasticSoilProfileProbability);
            Assert.AreSame(calculation.InputParameters.HydraulicBoundaryLocation, row.SelectableHydraulicBoundaryLocation.WrappedObject.HydraulicBoundaryLocation);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithCalculationWithInvalidInput_PropertiesFromCalculation()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithInvalidInput();

            // Call
            var row = new MacroStabilityInwardsCalculationRow(calculation, handler);

            // Assert
            Assert.AreSame(calculation, row.MacroStabilityInwardsCalculation);
            Assert.IsNull(row.StochasticSoilModel.WrappedObject);
            Assert.IsNull(row.StochasticSoilProfile.WrappedObject);
            Assert.AreEqual("0", row.StochasticSoilProfileProbability);
            Assert.IsNull(row.SelectableHydraulicBoundaryLocation.WrappedObject);
            mocks.VerifyAll();
        }

        [Test]
        public void Name_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            const string newValue = "Test new name";

            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();
            var row = new MacroStabilityInwardsCalculationRow(calculation, handler);

            calculation.Attach(observer);

            // Call
            row.Name = newValue;

            // Assert
            Assert.AreEqual(newValue, calculation.Name);
            mockRepository.VerifyAll();
        }

        [Test]
        public void StochasticSoilModel_AlwaysOnChange_NotifyObserverCalculationPropertyChangedOutputCleared()
        {
            // Setup
            var newModel = new StochasticSoilModel("test");
            var newValue = new DataGridViewComboBoxItemWrapper<StochasticSoilModel>(newModel);

            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(row => row.StochasticSoilModel = newValue, calculation);
        }

        [Test]
        public void StochasticSoilModel_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            DataGridViewComboBoxItemWrapper<StochasticSoilModel> oldValue = null;

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
            var newProfile = new StochasticSoilProfile(0, 0, 0);
            var newValue = new DataGridViewComboBoxItemWrapper<StochasticSoilProfile>(newProfile);

            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(row => row.StochasticSoilProfile = newValue, calculation);
        }

        [Test]
        public void StochasticSoilProfile_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            DataGridViewComboBoxItemWrapper<StochasticSoilProfile> oldValue = null;

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

            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(row => row.SelectableHydraulicBoundaryLocation = newValue, calculation);
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

            TestMacroStabilityInwardsOutput assignedOutput = null;

            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();
            if (hasOutput)
            {
                assignedOutput = new TestMacroStabilityInwardsOutput();
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

        private static void SetPropertyToInvalidValueAndVerifyException(
            Action<MacroStabilityInwardsCalculationRow> setProperty,
            MacroStabilityInwardsCalculationScenario calculation,
            string expectedMessage)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            mocks.ReplayAll();

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(
                new[]
                {
                    observable
                });

            var row = new MacroStabilityInwardsCalculationRow(calculation, handler);

            // Call
            TestDelegate test = () => setProperty(row);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }

        private static void SetPropertyAndVerifyNotifcationsAndOutputForCalculation(
            Action<MacroStabilityInwardsCalculationRow> setProperty,
            MacroStabilityInwardsCalculationScenario calculation)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester(
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