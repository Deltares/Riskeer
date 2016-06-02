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

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;

using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

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
            TestDelegate test = () => failureMechanism.Update(new PersistenceRegistry(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_WithoutPersistenceRegistry_ArgumentNullException()
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
            Assert.AreEqual("registry", paramName);
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
                    failureMechanism.Update(new PersistenceRegistry(), ringtoetsEntities);
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
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

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
            TestDelegate test = () => failureMechanism.Update(new PersistenceRegistry(), ringtoetsEntities);

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
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = 1,
                IsRelevant = true,
                CalculationsGroup =
                {
                    StorageId = 4
                }
            };

            var rootCalculationGroup = new CalculationGroupEntity
            {
                CalculationGroupEntityId = failureMechanism.CalculationsGroup.StorageId
            };
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = failureMechanism.StorageId,
                IsRelevant = Convert.ToByte(false),
                CalculationGroupEntity = rootCalculationGroup
            };
            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.CalculationGroupEntities.Add(rootCalculationGroup);

            // Call
            failureMechanism.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(Convert.ToByte(true), failureMechanismEntity.IsRelevant);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithNewStochasticSoilModel_SoilModelsAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = 1,
                IsRelevant = true,
                CalculationsGroup =
                {
                    StorageId = 1
                },
                StochasticSoilModels =
                {
                    new StochasticSoilModel(-1, string.Empty, string.Empty)
                }
            };

            var rootCalculationGroupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = failureMechanism.CalculationsGroup.StorageId
            };
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = failureMechanism.StorageId,
                IsRelevant = Convert.ToByte(false),
                CalculationGroupEntity = rootCalculationGroupEntity
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.CalculationGroupEntities.Add(rootCalculationGroupEntity);

            // Call
            failureMechanism.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, failureMechanismEntity.StochasticSoilModelEntities.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithUpdatedStochasticSoilModel_NoNewSoilModelAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var stochasticSoilModel = new StochasticSoilModel(-1, string.Empty, string.Empty)
            {
                StorageId = 1
            };
            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = 1,
                CalculationsGroup =
                {
                    StorageId = 4
                },
                StochasticSoilModels =
                {
                    stochasticSoilModel
                }
            };

            var stochasticSoilModelEntity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = stochasticSoilModel.StorageId
            };
            var rootCalculationGroupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = failureMechanism.CalculationsGroup.StorageId
            };
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = failureMechanism.StorageId,
                CalculationGroupEntity = rootCalculationGroupEntity,
                StochasticSoilModelEntities =
                {
                    stochasticSoilModelEntity
                }
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.CalculationGroupEntities.Add(rootCalculationGroupEntity);
            ringtoetsEntities.StochasticSoilModelEntities.Add(stochasticSoilModelEntity);

            // Call
            failureMechanism.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, failureMechanismEntity.StochasticSoilModelEntities.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithNewSurfaceLine_SurfaceLineEntitiesUpdated()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = 1,
                IsRelevant = true,
                CalculationsGroup =
                {
                    StorageId = 3
                }
            };
            failureMechanism.SurfaceLines.Add(new RingtoetsPipingSurfaceLine
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(1.1, 2.2)
            });

            var rootCalculationGroupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = failureMechanism.CalculationsGroup.StorageId
            };
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = failureMechanism.StorageId,
                IsRelevant = Convert.ToByte(false),
                CalculationGroupEntity = rootCalculationGroupEntity
            };
            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.CalculationGroupEntities.Add(rootCalculationGroupEntity);

            // Call
            failureMechanism.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, failureMechanismEntity.SurfaceLineEntities.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithUpdatedSurfaceLineEntity_NoNewSurfaceLineEntitiesAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                StorageId = 23,
                ReferenceLineIntersectionWorldPoint = new Point2D(45.67, 34.46)
            };

            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = 1,
                CalculationsGroup =
                {
                    StorageId = 54
                }
            };
            failureMechanism.SurfaceLines.Add(surfaceLine);

            var surfaceLineEntity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = surfaceLine.StorageId
            };
            var rootCalculationGroupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = failureMechanism.CalculationsGroup.StorageId
            };
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = failureMechanism.StorageId,
                CalculationGroupEntity = rootCalculationGroupEntity,
                SurfaceLineEntities = 
                {
                    surfaceLineEntity
                }
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.CalculationGroupEntities.Add(rootCalculationGroupEntity);
            ringtoetsEntities.SurfaceLineEntities.Add(surfaceLineEntity);

            // Call
            failureMechanism.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, failureMechanismEntity.SurfaceLineEntities.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithNewFailureMechanismSections_FailureMechanismSectionsAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = 1,
                CalculationsGroup =
                {
                    StorageId = 1
                }
            };
            failureMechanism.AddSection(new FailureMechanismSection("", new[] { new Point2D(0, 0) }));

            var rootCalculationGroupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = failureMechanism.CalculationsGroup.StorageId
            };
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = failureMechanism.StorageId,
                CalculationGroupEntity = rootCalculationGroupEntity
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.CalculationGroupEntities.Add(rootCalculationGroupEntity);

            // Call
            failureMechanism.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, failureMechanismEntity.FailureMechanismSectionEntities.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithUpdatedFailureMechanismSections_NoNewFailureMechanismSectionsAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = 1,
                CalculationsGroup =
                {
                    StorageId = 97
                }
            };
            var testName = "testName";
            var failureMechanismSection = new FailureMechanismSection(testName, new[] { new Point2D(0, 0) })
            {
                StorageId = 1
            };
            failureMechanism.AddSection(failureMechanismSection);

            var failureMechanismSectionEntity = new FailureMechanismSectionEntity
            {
                FailureMechanismSectionEntityId = failureMechanismSection.StorageId,
            };
            var rootCalculationGroupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = failureMechanism.CalculationsGroup.StorageId
            };
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = failureMechanism.StorageId,
                CalculationGroupEntity = rootCalculationGroupEntity,
                FailureMechanismSectionEntities = 
                {
                    failureMechanismSectionEntity
                }
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.CalculationGroupEntities.Add(rootCalculationGroupEntity);
            ringtoetsEntities.FailureMechanismSectionEntities.Add(failureMechanismSectionEntity);

            // Call
            failureMechanism.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, failureMechanismEntity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(testName, failureMechanismEntity.FailureMechanismSectionEntities.ElementAt(0).Name);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithNewCalculationGroup_CalculationGroupEntityAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = 1,
                CalculationsGroup =
                {
                    StorageId = 1
                }
            };
            var newCalculationGroup = new CalculationGroup
            {
                Name = "new group"
            };
            failureMechanism.CalculationsGroup.Children.Add(newCalculationGroup);

            var rootCalculationGroupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = failureMechanism.CalculationsGroup.StorageId,
                Name = "Berekeningen",
                IsEditable = 0,
                Order = 0
            };
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = failureMechanism.StorageId,
                CalculationGroupEntity = rootCalculationGroupEntity
            };
            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.CalculationGroupEntities.Add(rootCalculationGroupEntity);

            var registry = new PersistenceRegistry();

            // Call
            failureMechanism.Update(registry, ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, rootCalculationGroupEntity.CalculationGroupEntity1.Count);
            CalculationGroupEntity newlyAddedGroupEntity = rootCalculationGroupEntity.CalculationGroupEntity1.First();
            Assert.AreEqual(newCalculationGroup.Name, newlyAddedGroupEntity.Name);
            Assert.AreEqual(1, newlyAddedGroupEntity.IsEditable);
            Assert.AreEqual(0, newlyAddedGroupEntity.Order);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithUnchangedCalculationGroup_NoNewCalculationGroupEntityAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = 1,
                CalculationsGroup =
                {
                    StorageId = 1
                }
            };
            var alreadySavedChildGroup = new CalculationGroup
            {
                Name = "saved child group",
                StorageId = 2
            };
            failureMechanism.CalculationsGroup.Children.Add(alreadySavedChildGroup);

            var childGroupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = alreadySavedChildGroup.StorageId,
                Name = alreadySavedChildGroup.Name,
                IsEditable = 1,
                Order = 0
            };
            var rootCalculationGroupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = failureMechanism.CalculationsGroup.StorageId,
                Name = "Berekeningen",
                IsEditable = 0,
                Order = 0,
                CalculationGroupEntity1 =
                {
                    childGroupEntity
                }
            };
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = failureMechanism.StorageId,
                CalculationGroupEntity = rootCalculationGroupEntity
            };
            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.CalculationGroupEntities.Add(rootCalculationGroupEntity);
            ringtoetsEntities.CalculationGroupEntities.Add(childGroupEntity);

            var registry = new PersistenceRegistry();

            // Call
            failureMechanism.Update(registry, ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, rootCalculationGroupEntity.CalculationGroupEntity1.Count);
            CalculationGroupEntity retainedCalculationGroupEntity = rootCalculationGroupEntity.CalculationGroupEntity1.First();
            Assert.AreEqual(alreadySavedChildGroup.Name, retainedCalculationGroupEntity.Name);
            Assert.AreEqual(1, retainedCalculationGroupEntity.IsEditable);
            Assert.AreEqual(0, retainedCalculationGroupEntity.Order);
            Assert.AreEqual(alreadySavedChildGroup.StorageId, retainedCalculationGroupEntity.CalculationGroupEntityId);
            mocks.VerifyAll();
        }
    }
}