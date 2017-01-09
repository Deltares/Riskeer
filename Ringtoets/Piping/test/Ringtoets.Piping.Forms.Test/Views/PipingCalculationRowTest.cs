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
            // Call
            TestDelegate test = () => new PipingCalculationRow(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("pipingCalculation", paramName);
        }

        [Test]
        public void Constructor_WithPipingCalculation_PropertiesFromPipingCalculation()
        {
            // Setup
            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();

            // Call
            var row = new PipingCalculationRow(calculation);

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
        }

        [Test]
        public void Constructor_WithPipingCalculationWithInvalidInput_PropertiesFromPipingCalculation()
        {
            // Setup
            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithInvalidInput();

            // Call
            var row = new PipingCalculationRow(calculation);

            // Assert
            Assert.AreSame(calculation, row.PipingCalculation);
            Assert.IsNull(row.StochasticSoilModel.WrappedObject);
            Assert.IsNull(row.StochasticSoilProfile.WrappedObject);
            Assert.AreEqual("0", row.StochasticSoilProfileProbability);
            Assert.IsNull(row.SelectableHydraulicBoundaryLocation.WrappedObject);
        }

        [Test]
        public void Name_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var newValue = "Test new name";

            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            var row = new PipingCalculationRow(calculation);

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
            var newValue = new DataGridViewComboBoxItemWrapper<StochasticSoilModel>(new StochasticSoilModel(0, "test", "test"));

            // Call
            AssertPropertyChangedOutputClearedObserverNotified(
                row => row.StochasticSoilModel = newValue,
                calculation =>
                {
                    // Assert
                    Assert.AreEqual(newValue.WrappedObject, calculation.InputParameters.StochasticSoilModel);
                });
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
                    Assert.AreEqual(oldValue.WrappedObject, calculation.InputParameters.StochasticSoilModel);
                });
        }

        [Test]
        public void StochasticSoilProfile_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = new DataGridViewComboBoxItemWrapper<StochasticSoilProfile>(new StochasticSoilProfile(0, 0, 0));

            // Call
            AssertPropertyChangedOutputClearedObserverNotified(
                row => row.StochasticSoilProfile = newValue,
                calculation =>
                {
                    // Assert
                    Assert.AreEqual(newValue.WrappedObject, calculation.InputParameters.StochasticSoilProfile);
                });
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
                    Assert.AreEqual(oldValue.WrappedObject, calculation.InputParameters.StochasticSoilProfile);
                });
        }

        [Test]
        public void SelectableHydraulicBoundaryLocation_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var testHydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var selectableHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(testHydraulicBoundaryLocation, new Point2D(0, 0));
            var newValue = new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(selectableHydraulicBoundaryLocation);

            // Call
            AssertPropertyChangedOutputClearedObserverNotified(
                row => row.SelectableHydraulicBoundaryLocation = newValue, 
                calculation =>
                {
                    // Assert
                    Assert.AreSame(testHydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);
                });

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
                    Assert.AreEqual(oldValue.WrappedObject.HydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);
                });
        }

        [Test]
        public void DampingFactorExitMean_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = new Random().Next();

            // Call
            AssertPropertyChangedOutputClearedObserverNotified(
                row => row.DampingFactorExitMean = (RoundedDouble) newValue,
                calculation =>
                {
                    // Assert
                    Assert.AreEqual(new RoundedDouble(3, newValue), calculation.InputParameters.DampingFactorExit.Mean);
                });
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
                    Assert.AreEqual(oldValue, calculation.InputParameters.DampingFactorExit.Mean);
                });
        }

        [Test]
        public void PhreaticLevelExitMean_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = new Random().Next();

            // Call
            AssertPropertyChangedOutputClearedObserverNotified(
                row => row.PhreaticLevelExitMean = (RoundedDouble)newValue,
                calculation =>
                {
                    // Assert
                    Assert.AreEqual(new RoundedDouble(3, newValue), calculation.InputParameters.PhreaticLevelExit.Mean);
                });
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
                    Assert.AreEqual(oldValue, calculation.InputParameters.PhreaticLevelExit.Mean);
                });
        }

        [Test]
        public void EntryPointL_OnValidChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            const double newValue = 0.1;

            // Call
            AssertPropertyChangedOutputClearedObserverNotified(
                row => row.EntryPointL = (RoundedDouble)newValue,
                calculation =>
                {
                    // Assert
                    Assert.AreEqual(new RoundedDouble(2, newValue), calculation.InputParameters.EntryPointL);
                });
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
                    Assert.AreEqual(oldValue, calculation.InputParameters.EntryPointL);
                });
        }

        [Test]
        [TestCase(0.2)]
        [TestCase(1.0)]
        public void EntryPointL_EntryPointNotBeforeExitPoint_ThrowsArgumentOutOfRangeExceptionDoesNotNotifyObservers(double newValue)
        {
            AssertPropertyNotChanged(
                row =>
                {
                    // Call
                    TestDelegate call = () => row.EntryPointL = (RoundedDouble)newValue;

                    // Assert
                    var expectedMessage = "Het uittredepunt moet landwaarts van het intredepunt liggen.";
                    TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
                },
                calculation => { });
        }

        [Test]
        public void EntryPointL_NotOnSurfaceLine_ThrowsArgumentOutOfRangeExceptionAndDoesNotNotifyObservers()
        {
            // Setup
            const double newValue = -3.0;

            AssertPropertyNotChanged(
                row =>
                {
                    // Call
                    TestDelegate call = () => row.EntryPointL = (RoundedDouble)newValue;

                    // Assert
                    const string expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0, 1]).";
                    TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
                },
                calculation => { });
        }

        [Test]
        public void ExitPointL_OnValidChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            const double newValue = 0.3;

            // Call
            AssertPropertyChangedOutputClearedObserverNotified(
                row => row.ExitPointL = (RoundedDouble)newValue,
                calculation =>
                {
                    // Assert
                    Assert.AreEqual(new RoundedDouble(2, newValue), calculation.InputParameters.ExitPointL);
                });
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
                    Assert.AreEqual(oldValue, calculation.InputParameters.ExitPointL);
                });
        }

        [Test]
        [TestCase(0.0)]
        [TestCase(-0.2)]
        public void ExitPointL_ExitPointNotBeyondEntryPoint_ThrowsArgumentOutOfRangeExceptionDoesNotNotifyObservers(double newValue)
        {
            AssertPropertyNotChanged(
                row =>
                {
                    // Call
                    TestDelegate call = () => row.ExitPointL = (RoundedDouble)newValue;

                    // Assert
                    var expectedMessage = "Het uittredepunt moet landwaarts van het intredepunt liggen.";
                    TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
                },
                calculation => { });
        }

        [Test]
        public void ExitPointL_NotOnSurfaceLine_ThrowsArgumentOutOfRangeExceptionAndDoesNotNotifyObservers()
        {
            // Setup
            const double newValue = 3.0;

            AssertPropertyNotChanged(
                row =>
                {
                    // Call
                    TestDelegate call = () => row.ExitPointL = (RoundedDouble)newValue;

                    // Assert
                    const string expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0, 1]).";
                    TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
                },
                calculation => { });
        }

        private static void AssertPropertyChangedOutputClearedObserverNotified(
            Action<PipingCalculationRow> setProperty, 
            Action<PipingCalculationScenario> assertions)
        {
            AssertPropertyChangeWithOrWithoutCalculationOutput(setProperty, assertions, true, true);
            AssertPropertyChangeWithOrWithoutCalculationOutput(setProperty, assertions, false, true);
        }

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
            mockRepository.ReplayAll();

            TestPipingOutput assignedOutput = null;

            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            if (hasOutput)
            {
                assignedOutput = new TestPipingOutput();
            }
            calculation.Output = assignedOutput;

            var row = new PipingCalculationRow(calculation);
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
    }
}