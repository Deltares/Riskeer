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
using Application.Ringtoets.Storage.Update.GrassCoverErosionInwards;

using Core.Common.Base.Geometry;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Test.Update.GrassCoverErosionInwards
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismUpdateExtensionsTest
    {
        [Test]
        public void Update_WithoutContext_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.Update(new PersistenceRegistry(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_WithoutPersistenceRegistry_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

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
        public void Update_ContextWithNoGrassCoverErosionInwardsFailureMechanism_ThrowsEntityNotFoundException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

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
        public void Update_ContextWithNoGrassCoverErosionInwardsFailureMechanismWithId_ThrowsEntityNotFoundException()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var storageId = 1;
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
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
        public void Update_ContextWithGrassCoverErosionInwardsFailureMechanism_PropertiesUpdated()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                StorageId = 1,
                IsRelevant = true,
                GeneralInput =
                {
                    StorageId = 62981,
                    N = 13
                },
                CalculationsGroup =
                {
                    StorageId = 209
                }
            };

            var generalInputEntity = new GrassCoverErosionInwardsFailureMechanismMetaEntity
            {
                GrassCoverErosionInwardsFailureMechanismMetaEntityId = failureMechanism.GeneralInput.StorageId,
                N = 2
            };
            var rootCalculationGroup = new CalculationGroupEntity
            {
                CalculationGroupEntityId = failureMechanism.CalculationsGroup.StorageId
            };
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = failureMechanism.StorageId,
                IsRelevant = Convert.ToByte(false),
                GrassCoverErosionInwardsFailureMechanismMetaEntities =
                {
                    generalInputEntity
                },
                CalculationGroupEntity = rootCalculationGroup
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.CalculationGroupEntities.Add(rootCalculationGroup);
            ringtoetsEntities.GrassCoverErosionInwardsFailureMechanismMetaEntities.Add(generalInputEntity);

            // Call
            failureMechanism.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(Convert.ToByte(true), failureMechanismEntity.IsRelevant);
            Assert.AreEqual(failureMechanism.GeneralInput.N, generalInputEntity.N);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithNewDikeProfiles_DikeProfilesAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                StorageId = 1,
                GeneralInput =
                {
                    StorageId = 2
                },
                DikeProfiles =
                {
                    new DikeProfile(new Point2D(0, 0),
                                    new[]
                                    {
                                        new RoughnessPoint(new Point2D(0, 0), 1),
                                        new RoughnessPoint(new Point2D(1, 1), 1)
                                    },
                                    new Point2D[0], null, new DikeProfile.ConstructionProperties()),
                    new DikeProfile(new Point2D(2, 2),
                                    new[]
                                    {
                                        new RoughnessPoint(new Point2D(3, 3), 1),
                                        new RoughnessPoint(new Point2D(4, 4), 1)
                                    },
                                    new Point2D[0], null, new DikeProfile.ConstructionProperties())
                },
                CalculationsGroup =
                {
                    StorageId = 765
                }
            };

            var rootGroupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = failureMechanism.CalculationsGroup.StorageId
            };
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = failureMechanism.StorageId,
                CalculationGroupEntity = rootGroupEntity
            };
            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.CalculationGroupEntities.Add(rootGroupEntity);
            ringtoetsEntities.GrassCoverErosionInwardsFailureMechanismMetaEntities.Add(new GrassCoverErosionInwardsFailureMechanismMetaEntity
            {
                GrassCoverErosionInwardsFailureMechanismMetaEntityId = failureMechanism.GeneralInput.StorageId
            });

            // Call
            failureMechanism.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(2, failureMechanismEntity.DikeProfileEntities.Count);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithUpdatedDikeProfiles_NoNewDikeProfilesAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                StorageId = 1,
                GeneralInput =
                {
                    StorageId = 2
                },
                DikeProfiles =
                {
                    new DikeProfile(new Point2D(0, 0),
                                    new[]
                                    {
                                        new RoughnessPoint(new Point2D(0, 0), 1),
                                        new RoughnessPoint(new Point2D(1, 1), 1)
                                    },
                                    new Point2D[0], null, new DikeProfile.ConstructionProperties())
                    {
                        StorageId = 3
                    },
                    new DikeProfile(new Point2D(2, 2),
                                    new[]
                                    {
                                        new RoughnessPoint(new Point2D(3, 3), 1),
                                        new RoughnessPoint(new Point2D(4, 4), 1)
                                    },
                                    new Point2D[0], null, new DikeProfile.ConstructionProperties())
                    {
                        StorageId = 4
                    }
                },
                CalculationsGroup =
                {
                    StorageId = 405986
                }
            };

            var rootGroupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = failureMechanism.CalculationsGroup.StorageId
            };
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = failureMechanism.StorageId,
                CalculationGroupEntity = rootGroupEntity
            };
            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.CalculationGroupEntities.Add(rootGroupEntity);
            ringtoetsEntities.GrassCoverErosionInwardsFailureMechanismMetaEntities.Add(new GrassCoverErosionInwardsFailureMechanismMetaEntity
            {
                GrassCoverErosionInwardsFailureMechanismMetaEntityId = failureMechanism.GeneralInput.StorageId
            });
            ringtoetsEntities.DikeProfileEntities.Add(new DikeProfileEntity
            {
                DikeProfileEntityId = failureMechanism.DikeProfiles[0].StorageId,
                Name = "A"
            });
            ringtoetsEntities.DikeProfileEntities.Add(new DikeProfileEntity
            {
                DikeProfileEntityId = failureMechanism.DikeProfiles[1].StorageId,
                Name = "B"
            });

            // Call
            failureMechanism.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(2, ringtoetsEntities.DikeProfileEntities.Count());
            Assert.AreEqual("A", ringtoetsEntities.DikeProfileEntities.ElementAt(0).Name);
            Assert.AreEqual("B", ringtoetsEntities.DikeProfileEntities.ElementAt(1).Name);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithNewFailureMechanismSections_FailureMechanismSectionsAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                StorageId = 1,
                GeneralInput =
                {
                    StorageId = 2
                },
                CalculationsGroup =
                {
                    StorageId = 30495
                }
            };
            failureMechanism.AddSection(new FailureMechanismSection("", new[]
            {
                new Point2D(0, 0)
            }));

            var rootGroupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = failureMechanism.CalculationsGroup.StorageId
            };
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = failureMechanism.StorageId,
                CalculationGroupEntity = rootGroupEntity
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.CalculationGroupEntities.Add(rootGroupEntity);
            ringtoetsEntities.GrassCoverErosionInwardsFailureMechanismMetaEntities.Add(new GrassCoverErosionInwardsFailureMechanismMetaEntity
            {
                GrassCoverErosionInwardsFailureMechanismMetaEntityId = failureMechanism.GeneralInput.StorageId
            });

            // Call
            failureMechanism.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, failureMechanismEntity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(1, failureMechanismEntity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverErosionInwardsSectionResultEntities).Count());

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithUpdatedFailureMechanismSections_NoNewFailureMechanismSectionsAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                StorageId = 1,
                GeneralInput =
                {
                    StorageId = 2
                },
                CalculationsGroup =
                {
                    StorageId = 4968
                }
            };
            var testName = "testName";
            failureMechanism.AddSection(new FailureMechanismSection(testName, new[]
            {
                new Point2D(0, 0)
            })
            {
                StorageId = 1
            });

            var rootGroupEntity = new CalculationGroupEntity
            {
                CalculationGroupEntityId = failureMechanism.CalculationsGroup.StorageId
            };
            var failureMechanismSectionEntity = new FailureMechanismSectionEntity
            {
                FailureMechanismSectionEntityId = 1
            };
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1,
                FailureMechanismSectionEntities =
                {
                    failureMechanismSectionEntity
                },
                CalculationGroupEntity = rootGroupEntity
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.CalculationGroupEntities.Add(rootGroupEntity);
            ringtoetsEntities.GrassCoverErosionInwardsFailureMechanismMetaEntities.Add(new GrassCoverErosionInwardsFailureMechanismMetaEntity
            {
                GrassCoverErosionInwardsFailureMechanismMetaEntityId = failureMechanism.GeneralInput.StorageId
            });
            ringtoetsEntities.FailureMechanismSectionEntities.Add(failureMechanismSectionEntity);

            // Call
            failureMechanism.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, failureMechanismEntity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(1, failureMechanismEntity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverErosionInwardsSectionResultEntities).Count());
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

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                StorageId = 1,
                CalculationsGroup =
                {
                    StorageId = 1
                },
                GeneralInput =
                {
                    StorageId = 2
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
            ringtoetsEntities.GrassCoverErosionInwardsFailureMechanismMetaEntities.Add(new GrassCoverErosionInwardsFailureMechanismMetaEntity
            {
                GrassCoverErosionInwardsFailureMechanismMetaEntityId = failureMechanism.GeneralInput.StorageId,
                FailureMechanismEntityId = failureMechanism.StorageId
            });

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

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                StorageId = 1,
                CalculationsGroup =
                {
                    StorageId = 1
                },
                GeneralInput = 
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
            ringtoetsEntities.GrassCoverErosionInwardsFailureMechanismMetaEntities.Add(new GrassCoverErosionInwardsFailureMechanismMetaEntity
            {
                GrassCoverErosionInwardsFailureMechanismMetaEntityId = failureMechanism.GeneralInput.StorageId,
                FailureMechanismEntityId = failureMechanism.StorageId
            });

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