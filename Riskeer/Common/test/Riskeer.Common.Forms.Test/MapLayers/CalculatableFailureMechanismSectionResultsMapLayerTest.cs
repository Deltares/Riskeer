// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.MapLayers;
using Riskeer.Common.Forms.TestUtil;

namespace Riskeer.Common.Forms.Test.MapLayers
{
    [TestFixture]
    public class CalculatableFailureMechanismSectionResultsMapLayerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new TestCalculatableFailureMechanism();
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, string.Empty);

            var assemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

            // Call
            using (var mapLayer = new CalculatableFailureMechanismSectionResultsMapLayer<TestCalculatableFailureMechanism, TestFailureMechanismSectionResult, TestCalculationInput>(
                       failureMechanism, result => assemblyResult))
            {
                // Assert
                Assert.IsInstanceOf<NonCalculatableFailureMechanismSectionResultsMapLayer<TestFailureMechanismSectionResult>>(mapLayer);
            }
        }

        [Test]
        public void GivenMapLayerWithFailureMechanismSectionAssemblyResults_WhenChangingCalculationGroupDataAndObserversNotified_ThenMapDataUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(21);
            var failureMechanism = new TestCalculatableFailureMechanism();
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, string.Empty);

            var assemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

            using (var mapLayer = new CalculatableFailureMechanismSectionResultsMapLayer<TestCalculatableFailureMechanism, TestFailureMechanismSectionResult, TestCalculationInput>(
                       failureMechanism, result => assemblyResult))
            {
                mapLayer.MapData.Attach(observer);

                // Precondition
                MapDataTestHelper.AssertAssemblyMapData(failureMechanism, assemblyResult, mapLayer.MapData);

                // When
                assemblyResult = new FailureMechanismSectionAssemblyResult(
                    random.NextDouble(), random.NextDouble(), random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

                failureMechanism.CalculationsGroup.Children.Add(new TestCalculation());
                failureMechanism.CalculationsGroup.NotifyObservers();

                // Then
                MapDataTestHelper.AssertAssemblyMapData(failureMechanism, assemblyResult, mapLayer.MapData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenMapLayerWithFailureMechanismSectionAssemblyResults_WhenChangingCalculationScenarioDataAndObserversNotified_ThenMapDataUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(21);
            var calculationScenario = new TestCalculationScenario();
            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                calculationScenario
            });
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, string.Empty);

            var assemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

            using (var mapLayer = new CalculatableFailureMechanismSectionResultsMapLayer<TestCalculatableFailureMechanism, TestFailureMechanismSectionResult, TestCalculationInput>(
                       failureMechanism, result => assemblyResult))
            {
                mapLayer.MapData.Attach(observer);

                // Precondition
                MapDataTestHelper.AssertAssemblyMapData(failureMechanism, assemblyResult, mapLayer.MapData);

                // When
                assemblyResult = new FailureMechanismSectionAssemblyResult(
                    random.NextDouble(), random.NextDouble(), random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

                calculationScenario.IsRelevant = false;
                calculationScenario.NotifyObservers();

                // Then
                MapDataTestHelper.AssertAssemblyMapData(failureMechanism, assemblyResult, mapLayer.MapData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenMapLayerWithFailureMechanismSectionAssemblyResults_WhenChangingRootCalculationScenarioInputDataAndObserversNotified_ThenMapDataUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(21);
            var calculationScenario = new TestCalculationScenario();
            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                calculationScenario
            });
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, string.Empty);

            var assemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

            using (var mapLayer = new CalculatableFailureMechanismSectionResultsMapLayer<TestCalculatableFailureMechanism, TestFailureMechanismSectionResult, TestCalculationInput>(
                       failureMechanism, result => assemblyResult))
            {
                mapLayer.MapData.Attach(observer);

                // Precondition
                MapDataTestHelper.AssertAssemblyMapData(failureMechanism, assemblyResult, mapLayer.MapData);

                // When
                assemblyResult = new FailureMechanismSectionAssemblyResult(
                    random.NextDouble(), random.NextDouble(), random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

                calculationScenario.InputParameters.HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
                calculationScenario.InputParameters.NotifyObservers();

                // Then
                MapDataTestHelper.AssertAssemblyMapData(failureMechanism, assemblyResult, mapLayer.MapData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenMapLayerWithFailureMechanismSectionAssemblyResults_WhenChangingNestedCalculationScenarioInputDataAndObserversNotified_ThenMapDataUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(21);
            var calculationScenario = new TestCalculationScenario();
            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                calculationScenario
            });
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, string.Empty);

            var nestedCalculationScenario = new TestCalculationScenario();
            var nestedCalculationGroup = new CalculationGroup();
            nestedCalculationGroup.Children.Add(nestedCalculationScenario);
            failureMechanism.CalculationsGroup.Children.Add(nestedCalculationGroup);

            var assemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextDouble(), random.NextDouble(), random.NextDouble(),
                random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

            using (var mapLayer = new CalculatableFailureMechanismSectionResultsMapLayer<TestCalculatableFailureMechanism, TestFailureMechanismSectionResult, TestCalculationInput>(
                       failureMechanism, result => assemblyResult))
            {
                mapLayer.MapData.Attach(observer);

                // Precondition
                MapDataTestHelper.AssertAssemblyMapData(failureMechanism, assemblyResult, mapLayer.MapData);

                // When
                assemblyResult = new FailureMechanismSectionAssemblyResult(
                    random.NextDouble(), random.NextDouble(), random.NextDouble(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

                nestedCalculationScenario.InputParameters.HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
                nestedCalculationScenario.InputParameters.NotifyObservers();

                // Then
                MapDataTestHelper.AssertAssemblyMapData(failureMechanism, assemblyResult, mapLayer.MapData);
            }

            mocks.VerifyAll();
        }
    }
}