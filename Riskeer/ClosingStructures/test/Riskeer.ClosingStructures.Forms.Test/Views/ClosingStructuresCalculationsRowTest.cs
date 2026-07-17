// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
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
using Core.Common.TestUtil;
using NUnit.Framework;
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
using NSubstitute;

namespace Riskeer.ClosingStructures.Forms.Test.Views
{
    [TestFixture]
    public class ClosingStructuresCalculationRowTest
    {
        private const int useBreakWaterColumnIndex = 3;
        private const int breakWaterTypeColumnIndex = 4;
        private const int breakWaterHeightColumnIndex = 5;
        private const int useForeshoreColumnIndex = 6;
        private const int inflowModelTypeColumnIndex = 7;
        private const int meanInsideWaterLevelColumnIndex = 8;
        private const int criticalOvertoppingDischargeColumnIndex = 9;
        private const int allowedLevelIncreaseStorageColumnIndex = 10;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var handler = Substitute.For<IObservablePropertyChangeHandler>();
            var calculationScenario = new StructuresCalculationScenario<ClosingStructuresInput>();

            // Call
            var row = new ClosingStructuresCalculationRow(calculationScenario, handler);

            // Assert
            Assert.IsInstanceOf<CalculationRow<StructuresCalculationScenario<ClosingStructuresInput>>>(row);
            Assert.IsInstanceOf<IHasColumnStateDefinitions>(row);

            Assert.AreSame(calculationScenario, row.Calculation);

            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(8, columnStateDefinitions.Count);

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, useBreakWaterColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, breakWaterTypeColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, breakWaterHeightColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, useForeshoreColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, inflowModelTypeColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, meanInsideWaterLevelColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, criticalOvertoppingDischargeColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, allowedLevelIncreaseStorageColumnIndex);
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
        public void ForeshoreProfile_ChangeToEqualValue_NoNotificationsAndOutputNotCleared()
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
        public void UseBreakWater_ChangeToEqualValue_NoNotificationsAndOutputNotCleared(bool useBreakWater)
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
        public void BreakWaterType_ChangeToEqualValue_NoNotificationsAndOutputNotCleared()
        {
            // Setup
            var oldValue = (BreakWaterType) 0;

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

            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.BreakWaterHeight = newValue, calculation);
        }

        [Test]
        public void BreakWaterHeight_ChangeToEqualValue_NoNotificationsAndOutputNotCleared()
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
        public void UseForeShoreGeometry_ChangeToEqualValue_NoNotificationsAndOutputNotCleared()
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
        public void InflowModelType_InputWithoutStructure_ReturnsNull()
        {
            // Setup
            var handler = Substitute.For<IObservablePropertyChangeHandler>();
            var random = new Random(21);
            var inflowModelType = random.NextEnumValue<ClosingStructureInflowModelType>();
            var row = new ClosingStructuresCalculationRow(new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = inflowModelType
                }
            }, handler);

            // Call & Assert
            Assert.IsNull(row.InflowModelType);
        }

        [Test]
        public void InflowModelType_InputWithStructure_ReturnsExpectedInflowModelType()
        {
            // Setup
            var handler = Substitute.For<IObservablePropertyChangeHandler>();
            var random = new Random(21);
            var inflowModelType = random.NextEnumValue<ClosingStructureInflowModelType>();
            var row = new ClosingStructuresCalculationRow(new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure(),
                    InflowModelType = inflowModelType
                }
            }, handler);

            // Call & Assert
            Assert.AreEqual(inflowModelType, row.InflowModelType);
        }

        [Test]
        [TestCase(ClosingStructureInflowModelType.FloodedCulvert, ClosingStructureInflowModelType.LowSill)]
        [TestCase(ClosingStructureInflowModelType.LowSill, ClosingStructureInflowModelType.VerticalWall)]
        [TestCase(ClosingStructureInflowModelType.VerticalWall, ClosingStructureInflowModelType.FloodedCulvert)]
        public void InflowModelType_AlwaysOnChangeToValue_NotifyObserverAndCalculationPropertyChanged(ClosingStructureInflowModelType inflowModelType, ClosingStructureInflowModelType newInflowModelType)
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
        public void InflowModelType_AlwaysOnChangeToValue_InflowModelTypeChangedFired()
        {
            // Setup
            var inflowModelTypeChangedCounter = 0;
            var random = new Random(645);

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new IObservable[0]);
            var row = new ClosingStructuresCalculationRow(new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = random.NextEnumValue<ClosingStructureInflowModelType>()
                }
            }, handler);

            row.InflowModelTypeChanged += (s, a) => inflowModelTypeChangedCounter++;

            // Call
            row.InflowModelType = random.NextEnumValue<ClosingStructureInflowModelType>();

            // Assert
            Assert.AreEqual(1, inflowModelTypeChangedCounter);
        }

        [Test]
        public void InflowModelType_ChangeToEqualValue_NoNotificationsAndOutputNotCleared()
        {
            // Setup
            var oldValue = (ClosingStructureInflowModelType) 0;

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.InflowModelType.Value;
                    row.InflowModelType = row.InflowModelType;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue, calculation.InputParameters.InflowModelType);
                });
        }

        [Test]
        public void InflowModelType_ChangeToNull_NoNotificationsAndOutputNotCleared()
        {
            // Setup
            var oldValue = (ClosingStructureInflowModelType) 0;

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.InflowModelType.Value;
                    row.InflowModelType = null;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue, calculation.InputParameters.InflowModelType);
                });
        }

        [Test]
        public void InflowModelType_ChangeToEqualValue_InflowModelTypeChangedNotFired()
        {
            // Setup
            var inflowModelTypeChangedCounter = 0;
            var random = new Random(21);

            var handler = Substitute.For<IObservablePropertyChangeHandler>();
            var inflowModelType = random.NextEnumValue<ClosingStructureInflowModelType>();
            var row = new ClosingStructuresCalculationRow(new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = inflowModelType
                }
            }, handler);

            row.InflowModelTypeChanged += (s, a) => inflowModelTypeChangedCounter++;

            // Call
            row.InflowModelType = inflowModelType;

            // Assert
            Assert.AreEqual(0, inflowModelTypeChangedCounter);
        }

        [Test]
        public void InflowModelType_ChangeToNull_InflowModelTypeChangedNotFired()
        {
            // Setup
            var inflowModelTypeChangedCounter = 0;
            var random = new Random(21);

            var handler = Substitute.For<IObservablePropertyChangeHandler>();
            var inflowModelType = random.NextEnumValue<ClosingStructureInflowModelType>();
            var row = new ClosingStructuresCalculationRow(new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = inflowModelType
                }
            }, handler);

            row.InflowModelTypeChanged += (s, a) => inflowModelTypeChangedCounter++;

            // Call
            row.InflowModelType = null;

            // Assert
            Assert.AreEqual(0, inflowModelTypeChangedCounter);
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
        public void MeanInsideWaterLevel_ChangeToEqualValue_NoNotificationsAndOutputNotCleared()
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
        public void CriticalOvertoppingDischarge_ChangeToEqualValue_NoNotificationsAndOutputNotCleared()
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
        public void AllowedLevelIncreaseStorage_ChangeToEqualValue_NoNotificationsAndOutputNotCleared()
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
            var observable = Substitute.For<IObservable>();

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
            observable.Received().NotifyObservers();
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
            var inputObserver = Substitute.For<IObserver>();
            if (expectUpdates) {}

            var calculationObserver = Substitute.For<IObserver>();
            if (expectUpdates && hasOutput) {}

            var handler = Substitute.For<IObservablePropertyChangeHandler>();
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
                inputObserver.Received().UpdateObserver();
                calculationObserver.Received().UpdateObserver();
            }
            else
            {
                Assert.AreSame(assignedOutput, calculation.Output);
                inputObserver.DidNotReceive().UpdateObserver();
                calculationObserver.DidNotReceive().UpdateObserver();
            }
        }

        #region Column states

        [Test]
        public void Constructor_ForeshoreProfileNull_CorrectColumnStates()
        {
            // Setup
            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>();

            // Call
            var row = new ClosingStructuresCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new ClosingStructuresInput()));

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[useBreakWaterColumnIndex], false);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[breakWaterTypeColumnIndex], false);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[breakWaterHeightColumnIndex], false);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[useForeshoreColumnIndex], false);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_ForeshoreProfileWithoutGeometry_CorrectColumnStates(bool useBreakWater)
        {
            // Setup
            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(),
                    UseBreakWater = useBreakWater
                }
            };

            // Call
            var row = new ClosingStructuresCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new ClosingStructuresInput()));

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[useBreakWaterColumnIndex], true);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[breakWaterTypeColumnIndex], useBreakWater);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[breakWaterHeightColumnIndex], useBreakWater);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[useForeshoreColumnIndex], false);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_ForeshoreProfileWithGeometry_CorrectColumnStates(bool useBreakWater)
        {
            // Setup
            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(new[]
                    {
                        new Point2D(0.0, 0.0)
                    }),
                    UseBreakWater = useBreakWater
                }
            };

            // Call
            var row = new ClosingStructuresCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new ClosingStructuresInput()));

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[useBreakWaterColumnIndex], true);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[breakWaterTypeColumnIndex], useBreakWater);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[breakWaterHeightColumnIndex], useBreakWater);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[useForeshoreColumnIndex], true);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void UseBreakWater_AlwaysOnChange_CorrectColumnStates(bool useBreakWater)
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
            var row = new ClosingStructuresCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new ClosingStructuresInput()))
            {
                UseBreakWater = useBreakWater
            };

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[breakWaterTypeColumnIndex], useBreakWater);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[breakWaterHeightColumnIndex], useBreakWater);
        }

        [Test]
        public void ForeshoreProfile_OnChangeToNull_CorrectColumnStates()
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
            var row = new ClosingStructuresCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new ClosingStructuresInput()))
            {
                ForeshoreProfile = new DataGridViewComboBoxItemWrapper<ForeshoreProfile>(null)
            };

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[useBreakWaterColumnIndex], false);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[breakWaterTypeColumnIndex], false);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[breakWaterHeightColumnIndex], false);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[useForeshoreColumnIndex], false);
        }

        [Test]
        public void ForeshoreProfile_OnChangeToProfileWithoutGeometry_CorrectColumnStates()
        {
            // Setup
            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>();

            // Call
            var row = new ClosingStructuresCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new ClosingStructuresInput()))
            {
                ForeshoreProfile = new DataGridViewComboBoxItemWrapper<ForeshoreProfile>(new TestForeshoreProfile())
            };

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[useBreakWaterColumnIndex], true);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[breakWaterTypeColumnIndex], false);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[breakWaterHeightColumnIndex], false);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[useForeshoreColumnIndex], false);
        }

        [Test]
        public void ForeshoreProfile_OnChangeToProfileWithGeometry_CorrectColumnStates()
        {
            // Setup
            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>();

            // Call
            var row = new ClosingStructuresCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new ClosingStructuresInput()))
            {
                ForeshoreProfile = new DataGridViewComboBoxItemWrapper<ForeshoreProfile>(new TestForeshoreProfile(new[]
                {
                    new Point2D(0.0, 0.0)
                }))
            };

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[useBreakWaterColumnIndex], true);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[breakWaterTypeColumnIndex], false);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[breakWaterHeightColumnIndex], false);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[useForeshoreColumnIndex], true);
        }

        [Test]
        public void InflowModelType_InputWithoutStructure_CorrectColumnStates()
        {
            // Setup
            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>();

            // Call
            var row = new ClosingStructuresCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new ClosingStructuresInput()));

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[inflowModelTypeColumnIndex], false);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[meanInsideWaterLevelColumnIndex], false);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[criticalOvertoppingDischargeColumnIndex], false);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[allowedLevelIncreaseStorageColumnIndex], false);
        }

        [Test]
        public void InflowModelType_InputWithStructure_CorrectColumnStates()
        {
            // Setup
            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>();

            calculation.InputParameters.Structure = new TestClosingStructure(ClosingStructureInflowModelType.LowSill);

            // Call
            var row = new ClosingStructuresCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new ClosingStructuresInput()));

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[inflowModelTypeColumnIndex], true);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[meanInsideWaterLevelColumnIndex], true);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[criticalOvertoppingDischargeColumnIndex], true);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[allowedLevelIncreaseStorageColumnIndex], true);
        }

        [Test]
        [TestCase(ClosingStructureInflowModelType.VerticalWall, ClosingStructureInflowModelType.FloodedCulvert, true)]
        [TestCase(ClosingStructureInflowModelType.FloodedCulvert, ClosingStructureInflowModelType.LowSill, true)]
        [TestCase(ClosingStructureInflowModelType.LowSill, ClosingStructureInflowModelType.VerticalWall, false)]
        public void InflowModelType_AlwaysOnChangeToValue_CorrectColumnStates(
            ClosingStructureInflowModelType inflowModelType, ClosingStructureInflowModelType newInflowModelType, bool meanInsideWaterLevelIsEnabled)
        {
            // Setup
            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>();

            calculation.InputParameters.Structure = new TestClosingStructure(inflowModelType);

            // Call
            var row = new ClosingStructuresCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new ClosingStructuresInput()))
            {
                InflowModelType = newInflowModelType
            };

            // Assert
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[inflowModelTypeColumnIndex], true);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[meanInsideWaterLevelColumnIndex], meanInsideWaterLevelIsEnabled);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[criticalOvertoppingDischargeColumnIndex], true);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[allowedLevelIncreaseStorageColumnIndex], true);
        }

        #endregion
    }
}