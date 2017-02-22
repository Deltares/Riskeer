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
using System.Globalization;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.KernelWrapper.TestUtil;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingCalculationRowTest
    {
        [Test]
        public void Constructor_WithoutPipingCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new PipingCalculationRow(null, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("pipingCalculation", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutHandler_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingCalculationRow(new PipingCalculationScenario(new GeneralPipingInput()), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("handler", paramName);
        }

        [Test]
        public void Constructor_WithPipingCalculation_PropertiesFromPipingCalculation()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();
            
            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();

            // Call
            var row = new PipingCalculationRow(calculation, handler);

            // Assert
            Assert.AreSame(calculation, row.PipingCalculation);
            Assert.AreEqual(calculation.Name, row.Name);
            Assert.AreSame(calculation.InputParameters.StochasticSoilModel, row.StochasticSoilModel.WrappedObject);
            Assert.AreSame(calculation.InputParameters.StochasticSoilProfile, row.StochasticSoilProfile.WrappedObject);
            Assert.AreEqual(calculation.InputParameters.StochasticSoilProfile.Probability.ToString(CultureInfo.CurrentCulture), row.StochasticSoilProfileProbability);
            Assert.AreSame(calculation.InputParameters.HydraulicBoundaryLocation, row.SelectableHydraulicBoundaryLocation.WrappedObject.HydraulicBoundaryLocation);
            Assert.AreEqual(calculation.InputParameters.DampingFactorExit.Mean, row.DampingFactorExitMean);
            Assert.AreEqual(calculation.InputParameters.PhreaticLevelExit.Mean, row.PhreaticLevelExitMean);
            Assert.AreEqual(calculation.InputParameters.EntryPointL, row.EntryPointL);
            Assert.AreEqual(calculation.InputParameters.ExitPointL, row.ExitPointL);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithPipingCalculationWithInvalidInput_PropertiesFromPipingCalculation()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithInvalidInput();

            // Call
            var row = new PipingCalculationRow(calculation, handler);

            // Assert
            Assert.AreSame(calculation, row.PipingCalculation);
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

            var newValue = "Test new name";

            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            var row = new PipingCalculationRow(calculation, handler);

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
            var newModel = new StochasticSoilModel(0, "test", "test");
            var newValue = new DataGridViewComboBoxItemWrapper<StochasticSoilModel>(newModel);

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(row => row.StochasticSoilModel = newValue,
                                                                    newModel, calculation);
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

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(row => row.StochasticSoilProfile = newValue,
                                                                    newProfile, calculation);
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

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(row => row.SelectableHydraulicBoundaryLocation = newValue,
                                                                    newLocation, calculation);

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

        [Test]
        public void DampingFactorExitMean_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            RoundedDouble dampingFactorExitMean = (RoundedDouble) 2.3;

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(row => row.DampingFactorExitMean = dampingFactorExitMean,
                                                                    dampingFactorExitMean, calculation);
        }

        [Test]
        public void DampingFactorExitMean_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            var oldValue = RoundedDouble.NaN;

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.DampingFactorExitMean;
                    row.DampingFactorExitMean = row.DampingFactorExitMean;
                },
                calculation =>
                {
                    // Assert
                    Assert.False(double.IsNaN(oldValue));
                    Assert.AreEqual(oldValue, calculation.InputParameters.DampingFactorExit.Mean);
                });
        }

        [Test]
        public void PhreaticLevelExitMean_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            RoundedDouble phreaticLevelExitMean = (RoundedDouble) 5.1;

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(row => row.PhreaticLevelExitMean = phreaticLevelExitMean,
                                                                    phreaticLevelExitMean, calculation);
        }

        [Test]
        public void PhreaticLevelExitMean_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            var oldValue = RoundedDouble.NaN;

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.PhreaticLevelExitMean;
                    row.PhreaticLevelExitMean = row.PhreaticLevelExitMean;
                },
                calculation =>
                {
                    // Assert
                    Assert.False(double.IsNaN(oldValue));
                    Assert.AreEqual(oldValue, calculation.InputParameters.PhreaticLevelExit.Mean);
                });
        }

        [Test]
        public void EntryPointL_OnValidChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            RoundedDouble entryPointL = (RoundedDouble) 0.1;

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(row => row.EntryPointL = entryPointL,
                                                                    entryPointL, calculation);
        }

        [Test]
        public void EntryPointL_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            var oldValue = RoundedDouble.NaN;

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.EntryPointL;
                    row.EntryPointL = row.EntryPointL;
                },
                calculation =>
                {
                    // Assert
                    Assert.False(double.IsNaN(oldValue));
                    Assert.AreEqual(oldValue, calculation.InputParameters.EntryPointL);
                });
        }

        [Test]
        [TestCase(0.2)]
        [TestCase(1.0)]
        public void EntryPointL_EntryPointNotBeforeExitPoint_ThrowsArgumentOutOfRangeExceptionDoesNotNotifyObservers(double newValue)
        {
            // Setup
            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            RoundedDouble entryPointL = (RoundedDouble) newValue;

            // Call & Assert
            const string expectedMessage = "Het uittredepunt moet landwaarts van het intredepunt liggen.";
            SetPropertyToInvalidValueAndVerifyException(row => row.EntryPointL = entryPointL,
                                                        entryPointL, calculation,
                                                        expectedMessage);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void EntryPointL_NotOnSurfaceLine_ThrowsArgumentOutOfRangeExceptionAndDoesNotNotifyObservers()
        {
            // Setup
            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            RoundedDouble entryPointL = (RoundedDouble) (-3.0);

            // Call & Assert
            const string expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0,0, 1,0]).";
            SetPropertyToInvalidValueAndVerifyException(row => row.EntryPointL = entryPointL,
                                                        entryPointL, calculation,
                                                        expectedMessage);
        }

        [Test]
        public void ExitPointL_OnValidChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            RoundedDouble exitPointL = (RoundedDouble) 0.3;

            // Call & Assert
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(row => row.ExitPointL = exitPointL,
                                                                    exitPointL, calculation);
        }

        [Test]
        public void ExitPointL_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            var oldValue = RoundedDouble.NaN;

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.ExitPointL;
                    row.ExitPointL = row.ExitPointL;
                },
                calculation =>
                {
                    // Assert
                    Assert.False(double.IsNaN(oldValue));
                    Assert.AreEqual(oldValue, calculation.InputParameters.ExitPointL);
                });
        }

        [Test]
        [TestCase(0.0)]
        [TestCase(-0.2)]
        public void ExitPointL_ExitPointNotBeyondEntryPoint_ThrowsArgumentOutOfRangeExceptionDoesNotNotifyObservers(double newValue)
        {
            // Setup
            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            RoundedDouble exitPointL = (RoundedDouble) newValue;

            // Call & Assert
            const string expectedMessage = "Het uittredepunt moet landwaarts van het intredepunt liggen.";
            SetPropertyToInvalidValueAndVerifyException(row => row.ExitPointL = exitPointL,
                                                        exitPointL, calculation,
                                                        expectedMessage);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ExitPointL_NotOnSurfaceLine_ThrowsArgumentOutOfRangeExceptionAndDoesNotNotifyObservers()
        {
            // Setup
            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            RoundedDouble exitPointL = (RoundedDouble) 3.0;

            // Call & Assert
            const string expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0,0, 1,0]).";
            SetPropertyToInvalidValueAndVerifyException(row => row.ExitPointL = exitPointL,
                                                        exitPointL, calculation,
                                                        expectedMessage);
        }

        /// <summary>
        /// Asserts that the output of a <see cref="PipingCalculationScenario"/> remains
        /// unaffected (and therefore no change notification occurring) when the input for
        /// that calculation has been changed using an instance of <see cref="PipingCalculationRow"/>.
        /// </summary>
        /// <param name="setProperty">The function that changes a property of the <see cref="PipingCalculationRow"/>
        /// instance. This function should not throw exceptions.</param>
        /// <param name="assertions">The additional assertions to be performed on the <see cref="PipingCalculationScenario"/>
        /// whose input has been changed.</param>
        private static void AssertPropertyNotChanged(
            Action<PipingCalculationRow> setProperty, 
            Action<PipingCalculationScenario> assertions)
        {
            AssertPropertyChangeWithOrWithoutCalculationOutput(setProperty, assertions, true, false);
            AssertPropertyChangeWithOrWithoutCalculationOutput(setProperty, assertions, false, false);
        }

        private static void AssertPropertyChangeWithOrWithoutCalculationOutput(
            Action<PipingCalculationRow> setProperty,
            Action<PipingCalculationScenario> assertions, 
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

            TestPipingOutput assignedOutput = null;

            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            if (hasOutput)
            {
                assignedOutput = new TestPipingOutput();
            }
            calculation.Output = assignedOutput;

            var row = new PipingCalculationRow(calculation, handler);
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

        private static void SetPropertyToInvalidValueAndVerifyException<TPropertyValue>(
           Action<PipingCalculationRow> setProperty,
           TPropertyValue value,
           PipingCalculationScenario calculation,
           string expectedMessage)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            mocks.ReplayAll();

            var handler = new ObservableSetPropertyValueAfterConfirmationParameterTester<TPropertyValue>(
                calculation.InputParameters,
                value,
                new[]
                {
                    observable
                });

            var row = new PipingCalculationRow(calculation, handler);

            // Call
            TestDelegate test = () => setProperty(row);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }

        private static void SetPropertyAndVerifyNotifcationsAndOutputForCalculation<TPropertyValue>(
           Action<PipingCalculationRow> setProperty,
           TPropertyValue value,
           PipingCalculationScenario calculation)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var handler = new ObservableSetPropertyValueAfterConfirmationParameterTester<TPropertyValue>(
                calculation.InputParameters,
                value,
                new[]
                {
                    observable
                });

            var row = new PipingCalculationRow(calculation, handler);

            // Call
            setProperty(row);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }
    }
}