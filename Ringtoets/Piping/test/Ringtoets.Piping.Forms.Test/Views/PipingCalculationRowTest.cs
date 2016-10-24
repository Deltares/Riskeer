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
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.HydraRing.Data;
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
            var calculation = PipingCalculationFactory.CreateCalculationWithValidInput();

            // Call
            var row = new PipingCalculationRow(calculation);

            // Assert
            Assert.AreSame(calculation, row.PipingCalculation);
            Assert.AreEqual(calculation.Name, row.Name);
            Assert.AreEqual(calculation.InputParameters.StochasticSoilModel, row.StochasticSoilModel.WrappedObject);
            Assert.AreEqual(calculation.InputParameters.StochasticSoilProfile, row.StochasticSoilProfile.WrappedObject);
            Assert.AreEqual(calculation.InputParameters.StochasticSoilProfile.Probability.ToString(CultureInfo.CurrentCulture), row.StochasticSoilProfileProbability);
            Assert.AreEqual(calculation.InputParameters.HydraulicBoundaryLocation, row.HydraulicBoundaryLocation.WrappedObject);
            Assert.AreEqual(calculation.InputParameters.DampingFactorExit.Mean, row.DampingFactorExitMean);
            Assert.AreEqual(calculation.InputParameters.PhreaticLevelExit.Mean, row.PhreaticLevelExitMean);
            Assert.AreEqual(calculation.InputParameters.EntryPointL, row.EntryPointL);
            Assert.AreEqual(calculation.InputParameters.ExitPointL, row.ExitPointL);
        }

        [Test]
        public void Name_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = "Test new name";

            var calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            var row = new PipingCalculationRow(calculation);

            int counter = 0;
            using (new Observer(() => counter++)
            {
                Observable = calculation
            })
            {
                // Call
                row.Name = newValue;

                // Assert
                Assert.AreEqual(1, counter);
                Assert.AreEqual(newValue, calculation.Name);
            }
        }

        [Test]
        public void StochasticSoilModel_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = new DataGridViewComboBoxItemWrapper<StochasticSoilModel>(new StochasticSoilModel(0, "test", "test"));

            var calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            var row = new PipingCalculationRow(calculation);

            int counter = 0;
            using (new Observer(() => counter++)
            {
                Observable = calculation.InputParameters
            })
            {
                // Call
                row.StochasticSoilModel = newValue;

                // Assert
                Assert.AreEqual(1, counter);
                Assert.AreEqual(newValue.WrappedObject, calculation.InputParameters.StochasticSoilModel);
            }
        }

        [Test]
        public void StochasticSoilProfile_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = new DataGridViewComboBoxItemWrapper<StochasticSoilProfile>(new StochasticSoilProfile(0, 0, 0));

            var calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            var row = new PipingCalculationRow(calculation);

            int counter = 0;
            using (new Observer(() => counter++)
            {
                Observable = calculation.InputParameters
            })
            {
                // Call
                row.StochasticSoilProfile = newValue;

                // Assert
                Assert.AreEqual(1, counter);
                Assert.AreEqual(newValue.WrappedObject, calculation.InputParameters.StochasticSoilProfile);
            }
        }

        [Test]
        public void HydraulicBoundaryLocation_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = new DataGridViewComboBoxItemWrapper<HydraulicBoundaryLocation>(new HydraulicBoundaryLocation(0, "test", 0.0, 0.0));

            var calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            var row = new PipingCalculationRow(calculation);

            int counter = 0;
            using (new Observer(() => counter++)
            {
                Observable = calculation.InputParameters
            })
            {
                // Call
                row.HydraulicBoundaryLocation = newValue;

                // Assert
                Assert.AreEqual(1, counter);
                Assert.AreEqual(newValue.WrappedObject, calculation.InputParameters.HydraulicBoundaryLocation);
            }
        }

        [Test]
        public void DampingFactorExitMean_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = new Random().Next();

            var calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            var row = new PipingCalculationRow(calculation);

            int counter = 0;
            using (new Observer(() => counter++)
            {
                Observable = calculation.InputParameters
            })
            {
                // Call
                row.DampingFactorExitMean = (RoundedDouble) newValue;

                // Assert
                Assert.AreEqual(1, counter);
                Assert.AreEqual(new RoundedDouble(2, newValue), calculation.InputParameters.DampingFactorExit.Mean);
            }
        }

        [Test]
        public void PhreaticLevelExitMean_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = new Random().Next();

            var calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            var row = new PipingCalculationRow(calculation);

            int counter = 0;
            using (new Observer(() => counter++)
            {
                Observable = calculation.InputParameters
            })
            {
                // Call
                row.PhreaticLevelExitMean = (RoundedDouble) newValue;

                // Assert
                Assert.AreEqual(1, counter);
                Assert.AreEqual(new RoundedDouble(2, newValue), calculation.InputParameters.PhreaticLevelExit.Mean);
            }
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

            var calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
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

            var calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
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

            var calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
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

            var calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
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

            var calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
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

            var calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
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