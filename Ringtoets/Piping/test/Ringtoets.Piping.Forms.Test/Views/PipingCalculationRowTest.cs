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
using RingtoetsPipingDataResources = Ringtoets.Piping.Data.Properties.Resources;

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
        public void StochasticSoilModel_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var newValue = new DataGridViewComboBoxItemWrapper<StochasticSoilModel>(new StochasticSoilModel(0, "test", "test"));

            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            var row = new PipingCalculationRow(calculation);

            calculation.InputParameters.Attach(observer);

            // Call
            row.StochasticSoilModel = newValue;

            // Assert
            Assert.AreEqual(newValue.WrappedObject, calculation.InputParameters.StochasticSoilModel);
            mockRepository.VerifyAll();
        }

        [Test]
        public void StochasticSoilProfile_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var newValue = new DataGridViewComboBoxItemWrapper<StochasticSoilProfile>(new StochasticSoilProfile(0, 0, 0));

            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            var row = new PipingCalculationRow(calculation);
            calculation.InputParameters.Attach(observer);

            // Call
            row.StochasticSoilProfile = newValue;

            // Assert
            Assert.AreEqual(newValue.WrappedObject, calculation.InputParameters.StochasticSoilProfile);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SelectableHydraulicBoundaryLocation_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var selectableHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(new TestHydraulicBoundaryLocation(), new Point2D(0, 0));
            var newValue = new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(selectableHydraulicBoundaryLocation);

            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            var row = new PipingCalculationRow(calculation);

            calculation.InputParameters.Attach(observer);

            // Call
            row.SelectableHydraulicBoundaryLocation = newValue;

            // Assert
            Assert.AreSame(newValue.WrappedObject, selectableHydraulicBoundaryLocation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void DampingFactorExitMean_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var newValue = new Random().Next();

            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            var row = new PipingCalculationRow(calculation);
            calculation.InputParameters.Attach(observer);

            // Call
            row.DampingFactorExitMean = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(new RoundedDouble(2, newValue), calculation.InputParameters.DampingFactorExit.Mean);
            mockRepository.VerifyAll();
        }

        [Test]
        public void PhreaticLevelExitMean_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var newValue = new Random().Next();

            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            var row = new PipingCalculationRow(calculation);

            calculation.InputParameters.Attach(observer);

            // Call
            row.PhreaticLevelExitMean = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(new RoundedDouble(2, newValue), calculation.InputParameters.PhreaticLevelExit.Mean);
            mockRepository.VerifyAll();
        }

        [Test]
        public void EntryPointL_OnValidChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var newValue = 0.1;

            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            var row = new PipingCalculationRow(calculation);

            calculation.InputParameters.Attach(observer);

            // Call
            row.EntryPointL = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(new RoundedDouble(2, newValue), calculation.InputParameters.EntryPointL);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0.2)]
        [TestCase(1.0)]
        public void EntryPointL_EntryPointNotBeforeExitPoint_ThrowsArgumentOutOfRangeExceptionDoesNotNotifyObservers(double newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            var row = new PipingCalculationRow(calculation);

            calculation.InputParameters.Attach(observer);

            // Call
            TestDelegate call = () => row.EntryPointL = (RoundedDouble) newValue;

            // Assert
            var expectedMessage = RingtoetsPipingDataResources.PipingInput_EntryPointL_greater_or_equal_to_ExitPointL;
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        public void EntryPointL_NotOnSurfaceLine_ThrowsArgumentOutOfRangeExceptionAndDoesNotNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            var row = new PipingCalculationRow(calculation);

            calculation.InputParameters.Attach(observer);

            var newValue = -3.0;

            // Call
            TestDelegate call = () => row.EntryPointL = (RoundedDouble) newValue;

            // Assert
            const string expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0, 1]).";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        public void ExitPointL_OnValidChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var newValue = 0.3;

            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            var row = new PipingCalculationRow(calculation);

            calculation.InputParameters.Attach(observer);

            // Call
            row.ExitPointL = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(new RoundedDouble(2, newValue), calculation.InputParameters.ExitPointL);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0.0)]
        [TestCase(-0.2)]
        public void ExitPointL_ExitPointNotBeyondEntryPoint_ThrowsArgumentOutOfRangeExceptionDoesNotNotifyObservers(double newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            var row = new PipingCalculationRow(calculation);

            calculation.InputParameters.Attach(observer);

            // Call
            TestDelegate call = () => row.ExitPointL = (RoundedDouble) newValue;

            // Assert
            var expectedMessage = RingtoetsPipingDataResources.PipingInput_EntryPointL_greater_or_equal_to_ExitPointL;
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
            mocks.VerifyAll(); // No observer notified
        }

        [Test]
        public void ExitPointL_NotOnSurfaceLine_ThrowsArgumentOutOfRangeExceptionAndDoesNotNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            var row = new PipingCalculationRow(calculation);

            calculation.InputParameters.Attach(observer);

            var newValue = 3.0;

            // Call
            TestDelegate call = () => row.ExitPointL = (RoundedDouble) newValue;

            // Assert
            const string expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0, 1]).";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
            mocks.VerifyAll(); // No observer notified
        }
    }
}