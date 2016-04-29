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

using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update;
using NUnit.Framework;
using Rhino.Mocks;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class UpdateConversionCollectorTest
    {
        [Test]
        public void Constructor_CreatesNewInstance()
        {
            // Call
            var collector = new UpdateConversionCollector();

            // Assert
            Assert.IsInstanceOf<CreateConversionCollector>(collector);
        }

        [Test]
        public void RemoveUntouched_ProjectEntityInUpdatedList_ProjectEntityNotRemoved()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            var projectEntity = new ProjectEntity();
            ringtoetsEntities.ProjectEntities.Add(projectEntity);

            var collector = new UpdateConversionCollector();
            collector.Update(projectEntity);

            // Call
            collector.RemoveUntouched(ringtoetsEntities);

            // Assert
            Assert.AreEqual(1,  ringtoetsEntities.ProjectEntities.Count());
        }

        [Test]
        public void RemoveUntouched_ProjectEntityNotInUpdatedList_ProjectEntityRemoved()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();
            ringtoetsEntities.ProjectEntities.Add(new ProjectEntity());

            var collector = new UpdateConversionCollector();

            // Call
            collector.RemoveUntouched(ringtoetsEntities);

            // Assert
            Assert.AreEqual(0,  ringtoetsEntities.ProjectEntities.Count());
        }

        [Test]
        public void RemoveUntouched_AssessmentSectionEntityInUpdatedList_AssessmentSectionEntityNotRemoved()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            var assessmentSectionEntity = new AssessmentSectionEntity();
            ringtoetsEntities.AssessmentSectionEntities.Add(assessmentSectionEntity);

            var collector = new UpdateConversionCollector();
            collector.Update(assessmentSectionEntity);

            // Call
            collector.RemoveUntouched(ringtoetsEntities);

            // Assert
            Assert.AreEqual(1,  ringtoetsEntities.AssessmentSectionEntities.Count());
        }

        [Test]
        public void RemoveUntouched_AssessmentSectionEntityNotInUpdatedList_AssessmentSectionEntityRemoved()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();
            ringtoetsEntities.AssessmentSectionEntities.Add(new AssessmentSectionEntity());

            var collector = new UpdateConversionCollector();

            // Call
            collector.RemoveUntouched(ringtoetsEntities);

            // Assert
            Assert.AreEqual(0,  ringtoetsEntities.AssessmentSectionEntities.Count());
        }

        [Test]
        public void RemoveUntouched_FailureMechanismEntityInUpdatedList_FailureMechanismEntityNotRemoved()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            var failureMechanismEntity = new FailureMechanismEntity();
            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new UpdateConversionCollector();
            collector.Update(failureMechanismEntity);

            // Call
            collector.RemoveUntouched(ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, ringtoetsEntities.FailureMechanismEntities.Count());
        }

        [Test]
        public void RemoveUntouched_FailureMechanismEntityNotInUpdatedList_FailureMechanismEntityRemoved()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();
            ringtoetsEntities.FailureMechanismEntities.Add(new FailureMechanismEntity());

            var collector = new UpdateConversionCollector();

            // Call
            collector.RemoveUntouched(ringtoetsEntities);

            // Assert
            Assert.AreEqual(0, ringtoetsEntities.FailureMechanismEntities.Count());
        }

        [Test]
        public void RemoveUntouched_HydraulicLocationEntityInUpdatedList_HydraulicLocationEntityNotRemoved()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            var hydraulicLocationEntity = new HydraulicLocationEntity();
            ringtoetsEntities.HydraulicLocationEntities.Add(hydraulicLocationEntity);

            var collector = new UpdateConversionCollector();
            collector.Update(hydraulicLocationEntity);

            // Call
            collector.RemoveUntouched(ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, ringtoetsEntities.HydraulicLocationEntities.Count());
        }

        [Test]
        public void RemoveUntouched_HydraulicLocationEntityNotInUpdatedList_HydraulicLocationEntityRemoved()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();
            ringtoetsEntities.HydraulicLocationEntities.Add(new HydraulicLocationEntity());

            var collector = new UpdateConversionCollector();

            // Call
            collector.RemoveUntouched(ringtoetsEntities);

            // Assert
            Assert.AreEqual(0, ringtoetsEntities.HydraulicLocationEntities.Count());
        }

        [Test]
        public void RemoveUntouched_StochasticSoilModelEntityInUpdatedList_StochasticSoilModelEntityNotRemoved()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            var stochasticSoilModelEntity = new StochasticSoilModelEntity();
            ringtoetsEntities.StochasticSoilModelEntities.Add(stochasticSoilModelEntity);

            var collector = new UpdateConversionCollector();
            collector.Update(stochasticSoilModelEntity);

            // Call
            collector.RemoveUntouched(ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, ringtoetsEntities.StochasticSoilModelEntities.Count());
        }

        [Test]
        public void RemoveUntouched_StochasticSoilModelEntityNotInUpdatedList_StochasticSoilModelEntityRemoved()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();
            ringtoetsEntities.StochasticSoilModelEntities.Add(new StochasticSoilModelEntity());

            var collector = new UpdateConversionCollector();

            // Call
            collector.RemoveUntouched(ringtoetsEntities);

            // Assert
            Assert.AreEqual(0, ringtoetsEntities.StochasticSoilModelEntities.Count());
        }

        [Test]
        public void RemoveUntouched_StochasticSoilProfileEntityInUpdatedList_StochasticSoilProfileEntityNotRemoved()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            var stochasticSoilProfileEntity = new StochasticSoilProfileEntity();
            ringtoetsEntities.StochasticSoilProfileEntities.Add(stochasticSoilProfileEntity);

            var collector = new UpdateConversionCollector();
            collector.Update(stochasticSoilProfileEntity);

            // Call
            collector.RemoveUntouched(ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, ringtoetsEntities.StochasticSoilProfileEntities.Count());
        }

        [Test]
        public void RemoveUntouched_StochasticSoilProfileEntityNotInUpdatedList_StochasticSoilProfileEntityRemoved()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();
            ringtoetsEntities.StochasticSoilProfileEntities.Add(new StochasticSoilProfileEntity());

            var collector = new UpdateConversionCollector();

            // Call
            collector.RemoveUntouched(ringtoetsEntities);

            // Assert
            Assert.AreEqual(0, ringtoetsEntities.StochasticSoilProfileEntities.Count());
        }

        [Test]
        public void RemoveUntouched_SoilProfileEntityInUpdatedList_SoilProfileEntityNotRemoved()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            var soilProfileEntity = new SoilProfileEntity();
            ringtoetsEntities.SoilProfileEntities.Add(soilProfileEntity);

            var collector = new UpdateConversionCollector();
            collector.Update(soilProfileEntity);

            // Call
            collector.RemoveUntouched(ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, ringtoetsEntities.SoilProfileEntities.Count());
        }

        [Test]
        public void RemoveUntouched_SoilProfileEntityNotInUpdatedList_SoilProfileEntityRemoved()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();
            ringtoetsEntities.SoilProfileEntities.Add(new SoilProfileEntity());

            var collector = new UpdateConversionCollector();

            // Call
            collector.RemoveUntouched(ringtoetsEntities);

            // Assert
            Assert.AreEqual(0, ringtoetsEntities.SoilProfileEntities.Count());
        }

        [Test]
        public void RemoveUntouched_SoilLayerEntityInUpdatedList_SoilLayerEntityNotRemoved()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            var soilLayerEntity = new SoilLayerEntity();
            ringtoetsEntities.SoilLayerEntities.Add(soilLayerEntity);

            var collector = new UpdateConversionCollector();
            collector.Update(soilLayerEntity);

            // Call
            collector.RemoveUntouched(ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, ringtoetsEntities.SoilLayerEntities.Count());
        }

        [Test]
        public void RemoveUntouched_SoilLayerEntityNotInUpdatedList_SoilLayerEntityRemoved()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();
            ringtoetsEntities.SoilLayerEntities.Add(new SoilLayerEntity());

            var collector = new UpdateConversionCollector();

            // Call
            collector.RemoveUntouched(ringtoetsEntities);

            // Assert
            Assert.AreEqual(0, ringtoetsEntities.SoilLayerEntities.Count());
        }
    }
}