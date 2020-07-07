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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.ChangeHandlers;
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
        private const int breakWaterTypeColumnIndex = 4;
        private const int breakWaterHeightColumnIndex = 5;
        private const int useForeshoreColumnIndex = 6;

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

            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = grassCoverErosionInwardsCalculationRow.ColumnStateDefinitions;
            Assert.AreEqual(3, columnStateDefinitions.Count);

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, breakWaterTypeColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, breakWaterHeightColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, useForeshoreColumnIndex);

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

        [Test]
        public void DikeProfile_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            DikeProfile newProfile = DikeProfileTestFactory.CreateDikeProfile(new Point2D(0.0, 0.0));
            var newValue = new DataGridViewComboBoxItemWrapper<DikeProfile>(newProfile);

            var calculation = new GrassCoverErosionInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.DikeProfile = newValue, calculation);
        }

        [Test]
        public void DikeProfile_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            DataGridViewComboBoxItemWrapper<DikeProfile> oldValue = null;

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.DikeProfile;
                    row.DikeProfile = row.DikeProfile;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue.WrappedObject, calculation.InputParameters.DikeProfile);
                });
        }

        [Test]
        public void UseBreakWater_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            const bool newValue = true;

            var calculation = new GrassCoverErosionInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.UseBreakWater = newValue, calculation);
        }

        [Test]
        public void UseBreakWater_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            var oldValue = true;

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.UseBreakWater;
                    row.UseBreakWater = row.UseBreakWater;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue, calculation.InputParameters.UseBreakWater);
                });
        }

        [Test]
        [TestCase(true, true, false)]
        [TestCase(false, false, true)]
        public void UseBreakWaterState_AlwaysOnChange_CorrectColumnStates(bool useBreakWaterState, bool expectedColumnState, bool isReadOnly)
        {
            // Setup
            var mockRepository = new MockRepository();
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculationScenario();
            // Call
            var row = new GrassCoverErosionInwardsCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new GrassCoverErosionInwardsInput()))
            {
                UseBreakWater = useBreakWaterState
            };

            // Asserts
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(3, columnStateDefinitions.Count);

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[breakWaterTypeColumnIndex], expectedColumnState, isReadOnly);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[breakWaterHeightColumnIndex], expectedColumnState, isReadOnly);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Caisson)]
        public void BreakWaterType_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged(BreakWaterType breakWaterType)
        {
            // Setup
            BreakWaterType newValue = breakWaterType;

            var calculation = new GrassCoverErosionInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.BreakWaterType = newValue, calculation);
        }

        [Test]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Dam)]
        public void BreakWaterType_ChangeToEqualValue_NoNotificationsOutputNotCleared(BreakWaterType breakWaterType)
        {
            // Setup
            BreakWaterType oldValue = breakWaterType;

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.BreakWaterType;
                    row.BreakWaterType = row.BreakWaterType;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue, calculation.InputParameters.BreakWater.Type);
                });
        }

        [Test]
        public void BreakWaterHeight_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = new RoundedDouble(4, 16);

            var calculation = new GrassCoverErosionInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.BreakWaterHeight = newValue, calculation);
        }

        [Test]
        public void BreakWaterHeight_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            var oldValue = new RoundedDouble(4, 16);

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.BreakWaterHeight;
                    row.BreakWaterHeight = row.BreakWaterHeight;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue, calculation.InputParameters.BreakWater.Height);
                });
        }

        [Test]
        public void UseForeShoreGeometry_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            const bool newValue = true;

            var calculation = new GrassCoverErosionInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.UseForeShoreGeometry = newValue, calculation);
        }

        [Test]
        public void UseForeShoreGeometry_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            var oldValue = true;

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.UseForeShoreGeometry;
                    row.UseForeShoreGeometry = row.UseForeShoreGeometry;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue, calculation.InputParameters.UseForeshore);
                });
        }

        [Test]
        public void UseForeshoreState_DikeProfilePresent_CorrectColumnState()
        {
            // Setup
            var mockRepository = new MockRepository();
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculationScenario
            {
                InputParameters =
                {
                    DikeProfile = new DikeProfile(new Point2D(2.0, 0.0),
                                                  new[]
                                                  {
                                                      new RoughnessPoint(new Point2D(2.0, 0.0), 1.1d),
                                                      new RoughnessPoint(new Point2D(2.0, 0.0), 1.1d)
                                                  },
                                                  new[]
                                                  {
                                                      new Point2D(2.0, 0.0),
                                                      new Point2D(2.0, 0.0)
                                                  },
                                                  new BreakWater(BreakWaterType.Dam, 1.0), new DikeProfile.ConstructionProperties
                                                  {
                                                      Id = "testProperty",
                                                      Name = "testName",
                                                      Orientation = 2.0,
                                                      X0 = 1.0
                                                  })
                }
            };

            // Call
            var row = new GrassCoverErosionInwardsCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new GrassCoverErosionInwardsInput()))
            {
                UseForeShoreGeometry = true
            };

            // Asserts
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(3, columnStateDefinitions.Count);

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[useForeshoreColumnIndex], true);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UseForeshoreState_DikeProfileNotPresent_CorrectColumnState()
        {
            // Setup
            var mockRepository = new MockRepository();
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculationScenario();

            // Call
            var row = new GrassCoverErosionInwardsCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new GrassCoverErosionInwardsInput()));

            // Asserts
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(3, columnStateDefinitions.Count);

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateIsDisabled(columnStateDefinitions[useForeshoreColumnIndex]);

            mockRepository.VerifyAll();
        }

        [Test]
        public void DikeHeight_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = new RoundedDouble(2, 2.08);

            var calculation = new GrassCoverErosionInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.DikeHeight = newValue, calculation);
        }

        [Test]
        public void DikeHeight_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            var oldValue = new RoundedDouble(2, 2.08);

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.DikeHeight;
                    row.BreakWaterHeight = row.DikeHeight;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue, calculation.InputParameters.DikeHeight);
                });
        }

        [Test]
        public void MeanCriticalFlowRate_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = new RoundedDouble(4, 0.03);

            var calculation = new GrassCoverErosionInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.MeanCriticalFlowRate = newValue, calculation);
        }

        [Test]
        public void MeanCriticalFlowRate_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            var oldValue = new RoundedDouble(4, 0.03);

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.MeanCriticalFlowRate;
                    row.MeanCriticalFlowRate = row.MeanCriticalFlowRate;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue, calculation.InputParameters.CriticalFlowRate.Mean);
                });
        }

        [Test]
        public void StandardDeviationCriticalFlowRate_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = new RoundedDouble(4, 0.0004);

            var calculation = new GrassCoverErosionInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.StandardDeviationCriticalFlowRate = newValue, calculation);
        }

        [Test]
        public void StandardDeviationCriticalFlowRate_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            var oldValue = new RoundedDouble(4, 0.0004);

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.StandardDeviationCriticalFlowRate;
                    row.StandardDeviationCriticalFlowRate = row.StandardDeviationCriticalFlowRate;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue, calculation.InputParameters.CriticalFlowRate.StandardDeviation);
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