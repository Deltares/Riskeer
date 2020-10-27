﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.Views;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Primitives.TestUtil;

namespace Riskeer.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingCalculationRowTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 0.0),
                new Point3D(1.0, 0.0, 10)
            });

            PipingStochasticSoilModel stochasticSoilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel();
            PipingStochasticSoilProfile stochasticSoilProfile = stochasticSoilModel.StochasticSoilProfiles.First();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var dampingFactorExitMean = (RoundedDouble) 1.0;
            var phreaticLevelExitMean = (RoundedDouble) 2.0;

            var calculation = new TestPipingCalculationScenario
            {
                InputParameters =
                {
                    DampingFactorExit =
                    {
                        Mean = dampingFactorExitMean
                    },
                    PhreaticLevelExit =
                    {
                        Mean = phreaticLevelExitMean
                    },
                    SurfaceLine = surfaceLine,
                    StochasticSoilModel = stochasticSoilModel,
                    StochasticSoilProfile = stochasticSoilProfile,
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            // Call
            var row = new PipingCalculationRow(calculation, handler);

            // Assert
            Assert.IsInstanceOf<CalculationRow<IPipingCalculationScenario<PipingInput>>>(row);

            Assert.AreSame(calculation, row.Calculation);
            Assert.AreEqual(calculation.Name, row.Name);
            Assert.AreSame(stochasticSoilModel, row.StochasticSoilModel.WrappedObject);
            Assert.AreSame(stochasticSoilProfile, row.StochasticSoilProfile.WrappedObject);
            Assert.AreEqual(2, row.StochasticSoilProfileProbability.NumberOfDecimalPlaces);
            Assert.AreEqual(stochasticSoilProfile.Probability * 100, row.StochasticSoilProfileProbability, row.StochasticSoilProfileProbability.GetAccuracy());
            Assert.AreSame(hydraulicBoundaryLocation, row.SelectableHydraulicBoundaryLocation.WrappedObject.HydraulicBoundaryLocation);
            Assert.AreEqual(dampingFactorExitMean, row.DampingFactorExitMean);
            Assert.AreEqual(phreaticLevelExitMean, row.PhreaticLevelExitMean);
            Assert.AreEqual(calculation.InputParameters.EntryPointL, row.EntryPointL);
            Assert.AreEqual(calculation.InputParameters.ExitPointL, row.ExitPointL);
            mocks.VerifyAll();
        }

        [Test]
        public void StochasticSoilModel_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            PipingStochasticSoilModel newModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel();
            var newValue = new DataGridViewComboBoxItemWrapper<PipingStochasticSoilModel>(newModel);

            var calculation = new TestPipingCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.StochasticSoilModel = newValue, calculation);
        }

        [Test]
        public void StochasticSoilModel_ChangeToEqualValue_NoNotificationsAndOutputNotCleared()
        {
            // Setup
            DataGridViewComboBoxItemWrapper<PipingStochasticSoilModel> oldValue = null;

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
            var newProfile = new PipingStochasticSoilProfile(0, PipingSoilProfileTestFactory.CreatePipingSoilProfile());
            var newValue = new DataGridViewComboBoxItemWrapper<PipingStochasticSoilProfile>(newProfile);

            var calculation = new TestPipingCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.StochasticSoilProfile = newValue, calculation);
        }

        [Test]
        public void StochasticSoilProfile_ChangeToEqualValue_NoNotificationsAndOutputNotCleared()
        {
            // Setup
            DataGridViewComboBoxItemWrapper<PipingStochasticSoilProfile> oldValue = null;

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

            var calculation = new TestPipingCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.SelectableHydraulicBoundaryLocation = newValue, calculation);
        }

        [Test]
        public void DampingFactorExitMean_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var calculation = new TestPipingCalculationScenario();
            var dampingFactorExitMean = (RoundedDouble) 2.3;

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.DampingFactorExitMean = dampingFactorExitMean, calculation);
        }

        [Test]
        public void DampingFactorExitMean_ChangeToEqualValue_NoNotificationsAndOutputNotCleared()
        {
            // Setup
            RoundedDouble oldValue = RoundedDouble.NaN;

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
            var calculation = new TestPipingCalculationScenario();
            var phreaticLevelExitMean = (RoundedDouble) 5.1;

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.PhreaticLevelExitMean = phreaticLevelExitMean, calculation);
        }

        [Test]
        public void PhreaticLevelExitMean_ChangeToEqualValue_NoNotificationsAndOutputNotCleared()
        {
            // Setup
            RoundedDouble oldValue = RoundedDouble.NaN;

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
            var calculation = new TestPipingCalculationScenario();
            var entryPointL = (RoundedDouble) 0.1;

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.EntryPointL = entryPointL, calculation);
        }

        [Test]
        public void EntryPointL_ChangeToEqualValue_NoNotificationsAndOutputNotCleared()
        {
            // Setup
            RoundedDouble oldValue = RoundedDouble.NaN;

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
            IPipingCalculationScenario<PipingInput> calculation =
                PipingCalculationScenarioTestFactory.CreateCalculationWithValidInput(new TestHydraulicBoundaryLocation());
            var entryPointL = (RoundedDouble) newValue;

            // Call & Assert
            const string expectedMessage = "Het uittredepunt moet landwaarts van het intredepunt liggen.";
            SetPropertyToInvalidValueAndVerifyException(row => row.EntryPointL = entryPointL, calculation,
                                                        expectedMessage);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void EntryPointL_NotOnSurfaceLine_ThrowsArgumentOutOfRangeExceptionAndDoesNotNotifyObservers()
        {
            // Setup
            IPipingCalculationScenario<PipingInput> calculation =
                PipingCalculationScenarioTestFactory.CreateCalculationWithValidInput(new TestHydraulicBoundaryLocation());
            var entryPointL = (RoundedDouble) (-3.0);

            // Call & Assert
            const string expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0,0, 1,0]).";
            SetPropertyToInvalidValueAndVerifyException(row => row.EntryPointL = entryPointL, calculation,
                                                        expectedMessage);
        }

        [Test]
        public void ExitPointL_OnValidChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            IPipingCalculationScenario<PipingInput> calculation =
                PipingCalculationScenarioTestFactory.CreateCalculationWithValidInput(new TestHydraulicBoundaryLocation());
            var exitPointL = (RoundedDouble) 0.3;

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.ExitPointL = exitPointL, calculation);
        }

        [Test]
        public void ExitPointL_ChangeToEqualValue_NoNotificationsAndOutputNotCleared()
        {
            // Setup
            RoundedDouble oldValue = RoundedDouble.NaN;

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
            IPipingCalculationScenario<PipingInput> calculation =
                PipingCalculationScenarioTestFactory.CreateCalculationWithValidInput(new TestHydraulicBoundaryLocation());
            var exitPointL = (RoundedDouble) newValue;

            // Call & Assert
            const string expectedMessage = "Het uittredepunt moet landwaarts van het intredepunt liggen.";
            SetPropertyToInvalidValueAndVerifyException(row => row.ExitPointL = exitPointL, calculation,
                                                        expectedMessage);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ExitPointL_NotOnSurfaceLine_ThrowsArgumentOutOfRangeExceptionAndDoesNotNotifyObservers()
        {
            // Setup
            IPipingCalculationScenario<PipingInput> calculation =
                PipingCalculationScenarioTestFactory.CreateCalculationWithValidInput(new TestHydraulicBoundaryLocation());
            var exitPointL = (RoundedDouble) 3.0;

            // Call & Assert
            const string expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0,0, 1,0]).";
            SetPropertyToInvalidValueAndVerifyException(row => row.ExitPointL = exitPointL, calculation,
                                                        expectedMessage);
        }

        /// <summary>
        /// Asserts that the output of a <see cref="IPipingCalculationScenario{TPipingInput}"/> remains
        /// unaffected (and therefore no change notification occurring) when the input for
        /// that calculation has been changed using an instance of <see cref="PipingCalculationRow"/>.
        /// </summary>
        /// <param name="setProperty">The function that changes a property of the <see cref="PipingCalculationRow"/>
        /// instance. This function should not throw exceptions.</param>
        /// <param name="assertions">The additional assertions to be performed on the <see cref="IPipingCalculationScenario{TPipingInput}"/>
        /// whose input has been changed.</param>
        private static void AssertPropertyNotChanged(
            Action<PipingCalculationRow> setProperty,
            Action<IPipingCalculationScenario<PipingInput>> assertions)
        {
            AssertPropertyChangeWithOrWithoutCalculationOutput(setProperty, assertions, true, false);
            AssertPropertyChangeWithOrWithoutCalculationOutput(setProperty, assertions, false, false);
        }

        private static void AssertPropertyChangeWithOrWithoutCalculationOutput(
            Action<PipingCalculationRow> setProperty,
            Action<IPipingCalculationScenario<PipingInput>> assertions,
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

            var calculation = new TestPipingCalculationScenario(hasOutput);

            var row = new PipingCalculationRow(calculation, handler);
            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(inputObserver);

            // Call
            setProperty(row);

            // Assert
            assertions(calculation);
            if (expectUpdates)
            {
                Assert.IsFalse(calculation.HasOutput);
            }
            else
            {
                Assert.AreEqual(hasOutput, calculation.HasOutput);
            }

            mockRepository.VerifyAll();
        }

        private static void SetPropertyToInvalidValueAndVerifyException(
            Action<PipingCalculationRow> setProperty,
            IPipingCalculationScenario<PipingInput> calculation,
            string expectedMessage)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            mocks.ReplayAll();

            var handler = new SetPropertyValueAfterConfirmationParameterTester(
                new[]
                {
                    observable
                });

            var row = new PipingCalculationRow(calculation, handler);

            // Call
            void Call() => setProperty(row);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }

        private static void SetPropertyAndVerifyNotificationsAndOutputForCalculation(
            Action<PipingCalculationRow> setProperty,
            IPipingCalculationScenario<PipingInput> calculation)
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

            var row = new PipingCalculationRow(calculation, handler);

            // Call
            setProperty(row);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }
    }
}