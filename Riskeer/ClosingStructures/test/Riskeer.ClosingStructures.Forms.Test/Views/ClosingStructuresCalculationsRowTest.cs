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
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Data.TestUtil;
using Riskeer.ClosingStructures.Forms.Views;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;

namespace Riskeer.ClosingStructures.Forms.Test.Views
{
    [TestFixture]
    public class ClosingStructuresCalculationRowTest
    {
        private const int useBreakWaterColumnIndex = 3;
        private const int breakWaterTypeColumnIndex = 4;
        private const int breakWaterHeightColumnIndex = 5;
        private const int useForeshoreColumnIndex = 6;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var calculationScenario = new StructuresCalculationScenario<ClosingStructuresInput>();

            // Call
            var row = new ClosingStructuresCalculationRow(calculationScenario, handler);

            // Asserts
            Assert.IsInstanceOf<CalculationRow<StructuresCalculationScenario<ClosingStructuresInput>>>(row);
            Assert.IsInstanceOf<IHasColumnStateDefinitions>(row);

            Assert.AreSame(calculationScenario, row.Calculation);

            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(4, columnStateDefinitions.Count);

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, useBreakWaterColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, breakWaterTypeColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, breakWaterHeightColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, useForeshoreColumnIndex);
            mocks.VerifyAll();
        }

        [Test]
        public void SelectableHydraulicBoundaryLocation_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newLocation = new TestHydraulicBoundaryLocation();
            var selectableHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(newLocation, new Point2D(0, 0));
            var newValue = new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(selectableHydraulicBoundaryLocation);

            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.SelectableHydraulicBoundaryLocation = newValue, calculation);
        }

        [Test]
        public void ForeshoreProfile_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            ForeshoreProfile newProfile = new TestForeshoreProfile(new Point2D(0.0, 0.0));
            var newValue = new DataGridViewComboBoxItemWrapper<ForeshoreProfile>(newProfile);

            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.ForeshoreProfile = newValue, calculation);
        }

        [Test]
        public void ForeshoreProfile_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            DataGridViewComboBoxItemWrapper<ForeshoreProfile> oldValue = null;

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.ForeshoreProfile;
                    row.ForeshoreProfile = row.ForeshoreProfile;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue.WrappedObject, calculation.InputParameters.ForeshoreProfile);
                });
        }

        [Test]
        public void UseBreakWater_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.UseBreakWater = true, calculation);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void UseBreakWater_ChangeToEqualValue_NoNotificationsOutputNotCleared(bool useBreakWater)
        {
            // Setup
            bool oldValue = useBreakWater;

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
        [TestCase(true, true)]
        [TestCase(false, false)]
        public void UseBreakWaterState_AlwaysOnChange_CorrectColumnStates(bool useBreakWaterState, bool columnIsEnabled)
        {
            // Setup
            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>();

            // Call
            var row = new ClosingStructuresCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new ClosingStructuresInput()))
            {
                UseBreakWater = useBreakWaterState
            };

            // Asserts
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[breakWaterTypeColumnIndex], columnIsEnabled);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[breakWaterHeightColumnIndex], columnIsEnabled);
        }

        [Test]
        [TestCase(BreakWaterType.Wall, BreakWaterType.Dam)]
        [TestCase(BreakWaterType.Caisson, BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Dam, BreakWaterType.Caisson)]
        public void BreakWaterType_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged(BreakWaterType breakWaterType, BreakWaterType newBreakWaterType)
        {
            // Setup
            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    BreakWater =
                    {
                        Type = breakWaterType
                    }
                }
            };

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.BreakWaterType = newBreakWaterType, calculation);
        }

        [Test]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Dam)]
        public void BreakWaterType_ChangeToEqualValue_NoNotificationsOutputNotCleared(BreakWaterType breakWaterType)
        {
            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    breakWaterType = row.BreakWaterType;
                    row.BreakWaterType = row.BreakWaterType;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(breakWaterType);
                    Assert.AreEqual(breakWaterType, calculation.InputParameters.BreakWater.Type);
                });
        }

        [Test]
        public void BreakWaterHeight_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = new RoundedDouble(4, 16);

            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>();

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
            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.UseForeshoreGeometry = true, calculation);
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
                    oldValue = row.UseForeshoreGeometry;
                    row.UseForeshoreGeometry = row.UseForeshoreGeometry;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue, calculation.InputParameters.UseForeshore);
                });
        }

        [Test]
        public void UseForeshoreState_DikeProfileHasForeshoreGeometry_CorrectColumnState()
        {
            // Setup
            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    UseForeshore = true,
                    ForeshoreProfile = new TestForeshoreProfile(new[]
                    {
                        new Point2D(0.0, 0.0)
                    })
                }
            };

            // Call
            var row = new ClosingStructuresCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new ClosingStructuresInput()));

            // Asserts
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[useForeshoreColumnIndex], true);
        }

        [Test]
        public void UseForeshoreState_CalculationWithoutDikeProfile_CorrectColumnState()
        {
            // Setup
            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>();

            // Call
            var row = new ClosingStructuresCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new ClosingStructuresInput()));

            // Asserts
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateIsDisabled(columnStateDefinitions[useForeshoreColumnIndex]);
        }

        [Test]
        public void UseForeshoreState_CalculationWithDikeProfileWithoutForeshoreGeometry_CorrectColumnState()
        {
            // Setup
            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile()
                }
            };

            // Call
            var row = new ClosingStructuresCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new ClosingStructuresInput()));

            // Asserts
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateIsDisabled(columnStateDefinitions[useForeshoreColumnIndex]);
        }

        [Test]
        [TestCase(ClosingStructureInflowModelType.FloodedCulvert, ClosingStructureInflowModelType.LowSill)]
        [TestCase(ClosingStructureInflowModelType.LowSill, ClosingStructureInflowModelType.VerticalWall)]
        [TestCase(ClosingStructureInflowModelType.VerticalWall, ClosingStructureInflowModelType.FloodedCulvert)]
        public void InflowModelType_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged(ClosingStructureInflowModelType inflowModelType, ClosingStructureInflowModelType newInflowModelType)
        {
            // Setup
            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = inflowModelType
                }
            };

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.InflowModelType = newInflowModelType, calculation);
        }

        [Test]
        [TestCase(ClosingStructureInflowModelType.FloodedCulvert)]
        [TestCase(ClosingStructureInflowModelType.LowSill)]
        [TestCase(ClosingStructureInflowModelType.VerticalWall)]
        public void InflowModelType_ChangeToEqualValue_NoNotificationsOutputNotCleared(ClosingStructureInflowModelType inflowModelType)
        {
            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    inflowModelType = row.InflowModelType;
                    row.InflowModelType = row.InflowModelType;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(inflowModelType);
                    Assert.AreEqual(inflowModelType, calculation.InputParameters.InflowModelType);
                });
        }

        [Test]
        public void MeanInsideWaterLevel_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = new RoundedDouble(4, 0.03);

            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.MeanInsideWaterLevel = newValue, calculation);
        }

        [Test]
        public void MeanInsideWaterLevel_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            var oldValue = new RoundedDouble(4, 0.03);

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.MeanInsideWaterLevel;
                    row.MeanInsideWaterLevel = row.MeanInsideWaterLevel;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue, calculation.InputParameters.InsideWaterLevel.Mean);
                });
        }

        [Test]
        public void CriticalOvertoppingDischarge_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = new RoundedDouble(4, 0.03);

            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.CriticalOvertoppingDischarge = newValue, calculation);
        }

        [Test]
        public void CriticalOvertoppingDischarge_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            var oldValue = new RoundedDouble(4, 0.03);

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.CriticalOvertoppingDischarge;
                    row.CriticalOvertoppingDischarge = row.CriticalOvertoppingDischarge;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue, calculation.InputParameters.CriticalOvertoppingDischarge.Mean);
                });
        }

        [Test]
        public void AllowedLevelIncreaseStorage_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = new RoundedDouble(4, 0.03);

            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.AllowedLevelIncreaseStorage = newValue, calculation);
        }

        [Test]
        public void AllowedLevelIncreaseStorage_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            var oldValue = new RoundedDouble(4, 0.03);

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.AllowedLevelIncreaseStorage;
                    row.AllowedLevelIncreaseStorage = row.AllowedLevelIncreaseStorage;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue, calculation.InputParameters.AllowedLevelIncreaseStorage.Mean);
                });
        }

        private static void SetPropertyAndVerifyNotificationsAndOutputForCalculation(
            Action<ClosingStructuresCalculationRow> setProperty,
            StructuresCalculationScenario<ClosingStructuresInput> calculation)
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

            var row = new ClosingStructuresCalculationRow(calculation, handler);

            // Call
            setProperty(row);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }

        /// <summary>
        /// Asserts that the output of a <see cref="StructuresCalculationScenario{ClosingStructuresInput}"/> remains
        /// unaffected (and therefore no change notification occurring) when the input for
        /// that calculation has been changed using an instance of <see cref="ClosingStructuresCalculationRow"/>.
        /// </summary>
        /// <param name="setProperty">The function that changes a property of the <see cref="ClosingStructuresCalculationRow"/>
        /// instance. This function should not throw exceptions.</param>
        /// <param name="assertions">The additional assertions to be performed on the <see cref="StructuresCalculationScenario{ClosingStructuresInput}"/>
        /// whose input has been changed.</param>
        private static void AssertPropertyNotChanged(
            Action<ClosingStructuresCalculationRow> setProperty,
            Action<StructuresCalculationScenario<ClosingStructuresInput>> assertions)
        {
            AssertPropertyChangeWithOrWithoutCalculationOutput(setProperty, assertions, true, false);
            AssertPropertyChangeWithOrWithoutCalculationOutput(setProperty, assertions, false, false);
        }

        private static void AssertPropertyChangeWithOrWithoutCalculationOutput(
            Action<ClosingStructuresCalculationRow> setProperty,
            Action<StructuresCalculationScenario<ClosingStructuresInput>> assertions,
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

            StructuresOutput assignedOutput = null;

            StructuresCalculationScenario<ClosingStructuresInput> calculation = ClosingStructuresCalculationScenarioTestFactory.CreateNotCalculatedClosingStructuresCalculationScenario(new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0)
            }));
            calculation.InputParameters.HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            if (hasOutput)
            {
                assignedOutput = new TestStructuresOutput();
            }

            calculation.Output = assignedOutput;

            var row = new ClosingStructuresCalculationRow(calculation, handler);
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