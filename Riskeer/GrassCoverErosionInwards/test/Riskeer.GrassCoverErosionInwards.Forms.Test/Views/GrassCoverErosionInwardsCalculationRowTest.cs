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
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Forms.Views;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationRowTest
    {
        [Test]
        public void Constructor_WithoutCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            void Call() => new GrassCoverErosionInwardsCalculationRow(null, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("grassCoverErosionInwardsCalculationScenario", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutHandler_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new GrassCoverErosionInwardsCalculationRow(new GrassCoverErosionInwardsCalculationScenario(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("handler", paramName);
        }

        [Test]
        public void Constructor_WithCalculation_PropertiesCorrectlySet()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();
            var grassCoverErosionInwardsCalculationScenario = new GrassCoverErosionInwardsCalculationScenario();

            // Call
            var grassCoverErosionInwardsCalculationRow = new GrassCoverErosionInwardsCalculationRow(grassCoverErosionInwardsCalculationScenario, handler);

            // Asserts
            Assert.AreSame(grassCoverErosionInwardsCalculationScenario, grassCoverErosionInwardsCalculationRow.GrassCoverErosionInwardsCalculationScenario);

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

            var calculation = new GrassCoverErosionInwardsCalculationScenario();
            var row = new GrassCoverErosionInwardsCalculationRow(calculation, handler);

            calculation.Attach(observer);

            // Call
            row.Name = newValue;

            // Assert
            Assert.AreEqual(newValue, calculation.Name);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SelectableHydraulicBoundaryLocation_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newLocation = new TestHydraulicBoundaryLocation();
            var selectableHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(newLocation, new Point2D(0, 0));
            var newValue = new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(selectableHydraulicBoundaryLocation);

            var calculation = new GrassCoverErosionInwardsCalculationScenario();

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

        private static void SetPropertyAndVerifyNotificationsAndOutputForCalculation(
            Action<GrassCoverErosionInwardsCalculationRow> setProperty,
            GrassCoverErosionInwardsCalculationScenario calculation)
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

            var row = new GrassCoverErosionInwardsCalculationRow(calculation, handler);

            // Call
            setProperty(row);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }

        /// <summary>
        /// Asserts that the output of a <see cref="GrassCoverErosionInwardsCalculationScenario"/> remains
        /// unaffected (and therefore no change notification occurring) when the input for
        /// that calculation has been changed using an instance of <see cref="GrassCoverErosionInwardsCalculationRow"/>.
        /// </summary>
        /// <param name="setProperty">The function that changes a property of the <see cref="GrassCoverErosionInwardsCalculationRow"/>
        /// instance. This function should not throw exceptions.</param>
        /// <param name="assertions">The additional assertions to be performed on the <see cref="GrassCoverErosionInwardsCalculationScenario"/>
        /// whose input has been changed.</param>
        private static void AssertPropertyNotChanged(
            Action<GrassCoverErosionInwardsCalculationRow> setProperty,
            Action<GrassCoverErosionInwardsCalculationScenario> assertions)
        {
            AssertPropertyChangeWithOrWithoutCalculationOutput(setProperty, assertions, true, false);
            AssertPropertyChangeWithOrWithoutCalculationOutput(setProperty, assertions, false, false);
        }

        private static void AssertPropertyChangeWithOrWithoutCalculationOutput(
            Action<GrassCoverErosionInwardsCalculationRow> setProperty,
            Action<GrassCoverErosionInwardsCalculationScenario> assertions,
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

            GrassCoverErosionInwardsOutput assignedOutput = null;

            GrassCoverErosionInwardsCalculationScenario calculation = GrassCoverErosionInwardsCalculationScenarioTestFactory.CreateNotCalculatedGrassCoverErosionInwardsCalculationScenario(new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0)
            }));
            calculation.InputParameters.HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            if (hasOutput)
            {
                assignedOutput = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(0.2), null, null);
            }

            calculation.Output = assignedOutput;

            var row = new GrassCoverErosionInwardsCalculationRow(calculation, handler);
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
        }
    }
}