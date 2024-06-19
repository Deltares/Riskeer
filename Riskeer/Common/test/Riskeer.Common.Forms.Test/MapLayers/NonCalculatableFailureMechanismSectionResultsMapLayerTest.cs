// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.Data.TestUtil;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.MapLayers;
using Riskeer.Common.Forms.TestUtil;

namespace Riskeer.Common.Forms.Test.MapLayers
{
    [TestFixture]
    public class NonCalculatableFailureMechanismSectionResultsMapLayerTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new NonCalculatableFailureMechanismSectionResultsMapLayer<TestFailureMechanismSectionResult>(
                null, result => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_PerformAssemblyFuncNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new NonCalculatableFailureMechanismSectionResultsMapLayer<TestFailureMechanismSectionResult>(
                new TestFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("performAssemblyFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, string.Empty);

            FailureMechanismSectionAssemblyResult assemblyResult = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult();

            // Call
            using (var mapLayer = new NonCalculatableFailureMechanismSectionResultsMapLayer<TestFailureMechanismSectionResult>(
                       failureMechanism, result => assemblyResult))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(mapLayer);
                MapDataTestHelper.AssertAssemblyMapData(failureMechanism, assemblyResult, mapLayer.MapData);
            }
        }

        [Test]
        public void GivenMapLayerWithFailureMechanismSectionAssemblyResults_WhenChangingSectionResultsDataAndObserversNotified_ThenMapDataUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(21);
            var failureMechanism = new TestFailureMechanism();
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, string.Empty);

            FailureMechanismSectionAssemblyResult assemblyResult = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult();

            using (var mapLayer = new NonCalculatableFailureMechanismSectionResultsMapLayer<TestFailureMechanismSectionResult>(
                       failureMechanism, result => assemblyResult))
            {
                mapLayer.MapData.Attach(observer);

                // Precondition
                MapDataTestHelper.AssertAssemblyMapData(failureMechanism, assemblyResult, mapLayer.MapData);

                // When
                assemblyResult = new FailureMechanismSectionAssemblyResult(random.NextDouble(),
                                                                           random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());

                TestFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.First();
                sectionResult.NotifyObservers();

                // Then
                MapDataTestHelper.AssertAssemblyMapData(failureMechanism, assemblyResult, mapLayer.MapData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenMapLayerWithFailureMechanismSectionAssemblyResults_WhenChangingFailureMechanismDataAndObserversNotified_ThenMapDataUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, string.Empty);

            FailureMechanismSectionAssemblyResult assemblyResult = FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult();

            using (var mapLayer = new NonCalculatableFailureMechanismSectionResultsMapLayer<TestFailureMechanismSectionResult>(
                       failureMechanism, result => assemblyResult))
            {
                mapLayer.MapData.Attach(observer);

                // Precondition
                MapDataTestHelper.AssertAssemblyMapData(failureMechanism, assemblyResult, mapLayer.MapData);

                // When
                failureMechanism.SetSections(new[]
                {
                    FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1)
                    }),
                    FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                    {
                        new Point2D(1, 1),
                        new Point2D(2, 2)
                    })
                }, string.Empty);
                failureMechanism.NotifyObservers();

                // Then
                MapDataTestHelper.AssertAssemblyMapData(failureMechanism, assemblyResult, mapLayer.MapData);
            }

            mocks.VerifyAll();
        }
    }
}