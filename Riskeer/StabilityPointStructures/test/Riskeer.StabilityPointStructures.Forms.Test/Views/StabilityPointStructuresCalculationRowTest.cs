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
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Data.TestUtil;
using Riskeer.StabilityPointStructures.Forms.Views;

namespace Riskeer.StabilityPointStructures.Forms.Test.Views
{
    [TestFixture]
    public class StabilityPointStructuresCalculationRowTest
    {
        private const int useBreakWaterColumnIndex = 3;
        private const int breakWaterTypeColumnIndex = 4;
        private const int breakWaterHeightColumnIndex = 5;
        private const int useForeshoreColumnIndex = 6;
        private const int constructiveStrengthLinearLoadModelColumnIndex = 8;
        private const int constructiveStrengthQuadraticLoadModelColumnIndex = 9;
        private const int stabilityLinearLoadModelColumnIndex = 10;
        private const int stabilityQuadraticLoadModelColumnIndex = 11;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var calculationScenario = new StructuresCalculationScenario<StabilityPointStructuresInput>();

            // Call
            var row = new StabilityPointStructuresCalculationRow(calculationScenario, handler);

            // Asserts
            Assert.IsInstanceOf<CalculationRow<StructuresCalculationScenario<StabilityPointStructuresInput>>>(row);
            Assert.IsInstanceOf<IHasColumnStateDefinitions>(row);

            Assert.AreSame(calculationScenario, row.Calculation);

            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            Assert.AreEqual(8, columnStateDefinitions.Count);

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, useBreakWaterColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, breakWaterTypeColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, breakWaterHeightColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, useForeshoreColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, constructiveStrengthLinearLoadModelColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, constructiveStrengthQuadraticLoadModelColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, stabilityLinearLoadModelColumnIndex);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateDefinition(columnStateDefinitions, stabilityQuadraticLoadModelColumnIndex);
            mocks.VerifyAll();
        }

        [Test]
        public void SelectableHydraulicBoundaryLocation_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newLocation = new TestHydraulicBoundaryLocation();
            var selectableHydraulicBoundaryLocation = new SelectableHydraulicBoundaryLocation(newLocation, new Point2D(0, 0));
            var newValue = new DataGridViewComboBoxItemWrapper<SelectableHydraulicBoundaryLocation>(selectableHydraulicBoundaryLocation);

            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.SelectableHydraulicBoundaryLocation = newValue, calculation);
        }

        [Test]
        public void ForeshoreProfile_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            ForeshoreProfile newProfile = new TestForeshoreProfile(new Point2D(0.0, 0.0));
            var newValue = new DataGridViewComboBoxItemWrapper<ForeshoreProfile>(newProfile);

            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();

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
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();

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
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();

            // Call
            var row = new StabilityPointStructuresCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new StabilityPointStructuresInput()))
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
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
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

            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();

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
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();

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
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
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
            var row = new StabilityPointStructuresCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new StabilityPointStructuresInput()));
            
            // Asserts
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[useForeshoreColumnIndex], true);
        }

        [Test]
        public void UseForeshoreState_CalculationWithoutDikeProfile_CorrectColumnState()
        {
            // Setup
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();

            // Call
            var row = new StabilityPointStructuresCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new StabilityPointStructuresInput()));

            // Asserts
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateIsDisabled(columnStateDefinitions[useForeshoreColumnIndex]);
        }

        [Test]
        public void UseForeshoreState_CalculationWithDikeProfileWithoutForeshoreGeometry_CorrectColumnState()
        {
            // Setup
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile()
                }
            };

            // Call
            var row = new StabilityPointStructuresCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new StabilityPointStructuresInput()));

            // Asserts
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnStateIsDisabled(columnStateDefinitions[useForeshoreColumnIndex]);
        }

        [Test]
        [TestCase(LoadSchematizationType.Linear, LoadSchematizationType.Quadratic)]
        [TestCase(LoadSchematizationType.Quadratic, LoadSchematizationType.Linear)]
        public void LoadSchematizationType_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged(LoadSchematizationType loadSchematizationType, LoadSchematizationType newLoadSchematizationType)
        {
            // Setup
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    LoadSchematizationType = loadSchematizationType
                }
            };

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.LoadSchematizationType = newLoadSchematizationType, calculation);
        }

        [Test]
        [TestCase(LoadSchematizationType.Linear)]
        [TestCase(LoadSchematizationType.Quadratic)]
        public void LoadSchematizationType_ChangeToEqualValue_NoNotificationsOutputNotCleared(LoadSchematizationType breakWaterType)
        {
            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    breakWaterType = row.LoadSchematizationType;
                    row.LoadSchematizationType = row.LoadSchematizationType;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(breakWaterType);
                    Assert.AreEqual(breakWaterType, calculation.InputParameters.LoadSchematizationType);
                });
        }

        [Test]
        [TestCase(LoadSchematizationType.Linear, true)]
        [TestCase(LoadSchematizationType.Quadratic, false)]
        public void LoadSchematizationState_AlwaysOnChange_CorrectColumnStates(LoadSchematizationType loadSchematizationType, bool columnIsEnabled)
        {
            // Setup
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();

            // Call
            var row = new StabilityPointStructuresCalculationRow(calculation, new ObservablePropertyChangeHandler(calculation, new StabilityPointStructuresInput()))
            {
                LoadSchematizationType = loadSchematizationType
            };

            // Asserts
            IDictionary<int, DataGridViewColumnStateDefinition> columnStateDefinitions = row.ColumnStateDefinitions;

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[constructiveStrengthLinearLoadModelColumnIndex], columnIsEnabled);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[stabilityLinearLoadModelColumnIndex], columnIsEnabled);

            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[constructiveStrengthQuadraticLoadModelColumnIndex], !columnIsEnabled);
            DataGridViewControlColumnStateDefinitionTestHelper.AssertColumnState(columnStateDefinitions[stabilityQuadraticLoadModelColumnIndex], !columnIsEnabled);
        }

        [Test]
        public void ConstructiveStrengthLinearLoadModel_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = new RoundedDouble(4, 0.03);

            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.ConstructiveStrengthLinearLoadModel = newValue, calculation);
        }

        [Test]
        public void ConstructiveStrengthLinearLoadModel_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            var oldValue = new RoundedDouble(4, 0.03);

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.ConstructiveStrengthLinearLoadModel;
                    row.ConstructiveStrengthLinearLoadModel = row.ConstructiveStrengthLinearLoadModel;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue, calculation.InputParameters.ConstructiveStrengthLinearLoadModel.Mean);
                });
        }

        [Test]
        public void ConstructiveStrengthQuadraticLoadModel_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = new RoundedDouble(4, 0.03);

            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.ConstructiveStrengthQuadraticLoadModel = newValue, calculation);
        }

        [Test]
        public void ConstructiveStrengthQuadraticLoadModel_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            var oldValue = new RoundedDouble(4, 0.03);

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.ConstructiveStrengthQuadraticLoadModel;
                    row.ConstructiveStrengthQuadraticLoadModel = row.ConstructiveStrengthQuadraticLoadModel;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue, calculation.InputParameters.ConstructiveStrengthQuadraticLoadModel.Mean);
                });
        }

        [Test]
        public void StabilityLinearLoadModel_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            var oldValue = new RoundedDouble(4, 0.03);

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.StabilityLinearLoadModel;
                    row.StabilityLinearLoadModel = row.StabilityLinearLoadModel;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue, calculation.InputParameters.StabilityLinearLoadModel.Mean);
                });
        }

        [Test]
        public void StabilityQuadraticLoadModel_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = new RoundedDouble(4, 0.03);

            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.StabilityQuadraticLoadModel = newValue, calculation);
        }

        [Test]
        public void StabilityQuadraticLoadModel_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            var oldValue = new RoundedDouble(4, 0.03);

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.StabilityQuadraticLoadModel;
                    row.StabilityQuadraticLoadModel = row.StabilityQuadraticLoadModel;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue, calculation.InputParameters.StabilityQuadraticLoadModel.Mean);
                });
        }

        [Test]
        public void EvaluationLevel_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var newValue = new RoundedDouble(4, 0.03);

            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();

            // Call & Assert
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(row => row.EvaluationLevel = newValue, calculation);
        }

        [Test]
        public void EvaluationLevel_ChangeToEqualValue_NoNotificationsOutputNotCleared()
        {
            // Setup
            var oldValue = new RoundedDouble(4, 0.03);

            // Call
            AssertPropertyNotChanged(
                row =>
                {
                    oldValue = row.EvaluationLevel;
                    row.EvaluationLevel = row.EvaluationLevel;
                },
                calculation =>
                {
                    // Assert
                    Assert.NotNull(oldValue);
                    Assert.AreEqual(oldValue, calculation.InputParameters.EvaluationLevel);
                });
        }

        private static void SetPropertyAndVerifyNotificationsAndOutputForCalculation(
            Action<StabilityPointStructuresCalculationRow> setProperty,
            StructuresCalculationScenario<StabilityPointStructuresInput> calculation)
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

            var row = new StabilityPointStructuresCalculationRow(calculation, handler);

            // Call
            setProperty(row);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }

        /// <summary>
        /// Asserts that the output of a <see cref="StructuresCalculationScenario{StabilityPointStructuresInput}"/> remains
        /// unaffected (and therefore no change notification occurring) when the input for
        /// that calculation has been changed using an instance of <see cref="StabilityPointStructuresCalculationRow"/>.
        /// </summary>
        /// <param name="setProperty">The function that changes a property of the <see cref="StabilityPointStructuresCalculationRow"/>
        /// instance. This function should not throw exceptions.</param>
        /// <param name="assertions">The additional assertions to be performed on the <see cref="StructuresCalculationScenario{StabilityPointStructuresInput}"/>
        /// whose input has been changed.</param>
        private static void AssertPropertyNotChanged(
            Action<StabilityPointStructuresCalculationRow> setProperty,
            Action<StructuresCalculationScenario<StabilityPointStructuresInput>> assertions)
        {
            AssertPropertyChangeWithOrWithoutCalculationOutput(setProperty, assertions, true, false);
            AssertPropertyChangeWithOrWithoutCalculationOutput(setProperty, assertions, false, false);
        }

        private static void AssertPropertyChangeWithOrWithoutCalculationOutput(
            Action<StabilityPointStructuresCalculationRow> setProperty,
            Action<StructuresCalculationScenario<StabilityPointStructuresInput>> assertions,
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

            StructuresCalculationScenario<StabilityPointStructuresInput> calculation = StabilityPointStructuresCalculationScenarioTestFactory.CreateNotCalculatedStabilityPointStructuresCalculationScenario(new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0)
            }));
            calculation.InputParameters.HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            if (hasOutput)
            {
                assignedOutput = new TestStructuresOutput();
            }

            calculation.Output = assignedOutput;

            var row = new StabilityPointStructuresCalculationRow(calculation, handler);
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