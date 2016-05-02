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
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class PipingFailureMechanismUpdateExtensionsTest
    {
        [Test]
        public void Update_WithoutContext_ArgumentNullException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.Update(new UpdateConversionCollector(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_WithoutCollector_ArgumentNullException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    failureMechanism.Update(null, ringtoetsEntities);
                }
            };

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Update_ContextWithNoPipingFailureMechanism_EntityNotFoundException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    failureMechanism.Update(new UpdateConversionCollector(), ringtoetsEntities);
                }
            };

            // Assert
            var expectedMessage = String.Format("Het object 'FailureMechanismEntity' met id '{0}' is niet gevonden.", 0);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Update_ContextWithNoPipingFailureMechanismWithId_EntityNotFoundException()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var storageId = 1;
            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = storageId
            };

            ringtoetsEntities.FailureMechanismEntities.Add(new FailureMechanismEntity
            {
                FailureMechanismEntityId = 2
            });

            // Call
            TestDelegate test = () => failureMechanism.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            var expectedMessage = String.Format("Het object 'FailureMechanismEntity' met id '{0}' is niet gevonden.", storageId);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithPipingFailureMechanism_PropertiesUpdated()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = 1,
                IsRelevant = true
            };

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1,
                IsRelevant = Convert.ToByte(false)
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);

            // Call
            failureMechanism.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(Convert.ToByte(true), failureMechanismEntity.IsRelevant);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithNewStochasticSoilModel_SoilModelsAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = 1,
                IsRelevant = true,
                StochasticSoilModels =
                {
                    new StochasticSoilModel(-1, string.Empty, string.Empty)
                }
            };

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1,
                IsRelevant = Convert.ToByte(false)
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);

            // Call
            failureMechanism.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, failureMechanismEntity.StochasticSoilModelEntities.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithUpdatedStochasticSoilModel_NoNewSoilModelAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = 1,
                StochasticSoilModels =
                {
                    new StochasticSoilModel(-1, string.Empty, string.Empty)
                    {
                        StorageId = 1
                    }
                }
            };

            var stochasticSoilModelEntity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = 1
            };
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1,
                StochasticSoilModelEntities =
                {
                    stochasticSoilModelEntity
                }
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.StochasticSoilModelEntities.Add(stochasticSoilModelEntity);

            // Call
            failureMechanism.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, failureMechanismEntity.StochasticSoilModelEntities.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithNewFailureMechanismSections_FailureMechanismSectionsAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = 1
            };
            failureMechanism.AddSection(new FailureMechanismSection("", new[] { new Point2D(0, 0) }));

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1,
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);

            // Call
            failureMechanism.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, failureMechanismEntity.FailureMechanismSectionEntities.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithUpdatedFailureMechanismSections_NoNewFailureMechanismSectionsAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = 1
            };
            var testName = "testName";
            failureMechanism.AddSection(new FailureMechanismSection(testName, new[] { new Point2D(0, 0) })
            {
                StorageId = 1
            });

            var failureMechanismSectionEntity = new FailureMechanismSectionEntity
            {
                FailureMechanismSectionEntityId = 1,
            };
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1,
                FailureMechanismSectionEntities = 
                {
                    failureMechanismSectionEntity
                }
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.FailureMechanismSectionEntities.Add(failureMechanismSectionEntity);

            // Call
            failureMechanism.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, failureMechanismEntity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(testName, failureMechanismEntity.FailureMechanismSectionEntities.ElementAt(0).Name);

            mocks.VerifyAll();
        } 
    }
}