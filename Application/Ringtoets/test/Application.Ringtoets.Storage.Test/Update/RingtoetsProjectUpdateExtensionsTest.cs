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

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class RingtoetsProjectUpdateExtensionsTest
    {
        [Test]
        public void Update_WithoutContext_ThrowsArgumentNullException()
        {
            // Setup
            var project = new RingtoetsProject();

            // Call
            TestDelegate test = () => project.Update(new PersistenceRegistry(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_WithoutPersistenceRegistry_ThrowsArgumentNullException()
        {
            // Setup
            var project = new RingtoetsProject();

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    project.Update(null, ringtoetsEntities);
                }
            };

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Update_ContextWithNoProject_ThrowsEntityNotFoundException()
        {
            // Setup
            var project = new RingtoetsProject();

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    project.Update(new PersistenceRegistry(), ringtoetsEntities);
                }
            };

            // Assert
            Assert.Throws<EntityNotFoundException>(test);
        }

        [Test]
        public void Update_ContextWithNoProjectWithId_ThrowsEntityNotFoundException()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var storageId = 1;
            var project = new RingtoetsProject
            {
                StorageId = storageId
            };

            ringtoetsEntities.ProjectEntities.Add(new ProjectEntity
            {
                ProjectEntityId = 2
            });

            // Call
            TestDelegate test = () => project.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            var expectedMessage = String.Format("Het object 'ProjectEntity' met id '{0}' is niet gevonden.", storageId);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithMultipleProjectsWithId_ThrowsEntityNotFoundException()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var storageId = 1;
            var project = new RingtoetsProject
            {
                StorageId = storageId
            };

            ringtoetsEntities.ProjectEntities.Add(new ProjectEntity
            {
                ProjectEntityId = storageId
            });
            ringtoetsEntities.ProjectEntities.Add(new ProjectEntity
            {
                ProjectEntityId = storageId
            });

            // Call
            TestDelegate test = () => project.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            var expectedMessage = String.Format("Het object 'ProjectEntity' met id '{0}' is niet gevonden.", storageId);
            var expectedInnerMessage = "Sequence contains more than one matching element";

            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
            Assert.AreEqual(expectedInnerMessage, exception.InnerException.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithProject_DescriptionUpdated()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var newDescription = "newDescription";
            var project = new RingtoetsProject
            {
                StorageId = 1,
                Description = newDescription
            };

            var entity = new ProjectEntity
            {
                ProjectEntityId = 1,
                Description = string.Empty
            };
            ringtoetsEntities.ProjectEntities.Add(entity);

            // Call
            project.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(newDescription, entity.Description);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ProjectWithNewSection_SectionAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var project = new RingtoetsProject
            {
                StorageId = 1,
                AssessmentSections =
                {
                    new AssessmentSection(AssessmentSectionComposition.Dike)
                }
            };

            var entity = new ProjectEntity
            {
                ProjectEntityId = 1
            };
            ringtoetsEntities.ProjectEntities.Add(entity);

            // Call
            project.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, entity.AssessmentSectionEntities.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ProjectWithExistingSection_NoSectionsAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var section = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                StorageId = 1,
                PipingFailureMechanism =
                {
                    StorageId = 1,
                    CalculationsGroup =
                    {
                        StorageId = 23
                    },
                    PipingProbabilityAssessmentInput =
                    {
                        StorageId = 2
                    }
                },
                GrassCoverErosionInwards =
                {
                    StorageId = 1,
                    CalculationsGroup =
                    {
                        StorageId = 854563
                    },
                    GeneralInput =
                    {
                        StorageId = 3
                    }
                },
                MacrostabilityInwards =
                {
                    StorageId = 1
                },
                HeightStructures =
                {
                    StorageId = 1
                },
                ClosingStructure =
                {
                    StorageId = 1
                },
                StrengthStabilityPointConstruction =
                {
                    StorageId = 1
                },
                StabilityStoneCover =
                {
                    StorageId = 1
                },
                WaveImpactAsphaltCover =
                {
                    StorageId = 1
                },
                GrassCoverErosionOutwards =
                {
                    StorageId = 1
                },
                GrassCoverSlipOffOutwards =
                {
                    StorageId = 1
                },
                PipingStructure =
                {
                    StorageId = 1
                },
                DuneErosion =
                {
                    StorageId = 1
                },
                GrassCoverSlipOffInwards =
                {
                    StorageId = 1
                },
                MacrostabilityOutwards =
                {
                    StorageId = 1
                },
                Microstability =
                {
                    StorageId = 1
                },
                StrengthStabilityLengthwiseConstruction =
                {
                    StorageId = 1
                },
                TechnicalInnovation =
                {
                    StorageId = 1
                },
                WaterPressureAsphaltCover =
                {
                    StorageId = 1
                }
            };
            var project = new RingtoetsProject
            {
                StorageId = 1,
                AssessmentSections =
                {
                    section
                }
            };

            var assessmentSectionEntity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = section.StorageId
            };
            var projectEntity = new ProjectEntity
            {
                ProjectEntityId = project.StorageId,
                AssessmentSectionEntities =
                {
                    assessmentSectionEntity
                }
            };

            ringtoetsEntities.ProjectEntities.Add(projectEntity);
            ringtoetsEntities.AssessmentSectionEntities.Add(assessmentSectionEntity);
            ringtoetsEntities.CalculationGroupEntities.Add(new CalculationGroupEntity
            {
                CalculationGroupEntityId = section.PipingFailureMechanism.CalculationsGroup.StorageId
            });
            ringtoetsEntities.CalculationGroupEntities.Add(new CalculationGroupEntity
            {
                CalculationGroupEntityId = section.GrassCoverErosionInwards.CalculationsGroup.StorageId
            });
            ringtoetsEntities.FailureMechanismEntities.Add(new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1
            });
            ringtoetsEntities.PipingFailureMechanismMetaEntities.Add(new PipingFailureMechanismMetaEntity
            {
                PipingFailureMechanismMetaEntityId = section.PipingFailureMechanism.PipingProbabilityAssessmentInput.StorageId
            });
            ringtoetsEntities.GrassCoverErosionInwardsFailureMechanismMetaEntities.Add(new GrassCoverErosionInwardsFailureMechanismMetaEntity
            {
                GrassCoverErosionInwardsFailureMechanismMetaEntityId = section.GrassCoverErosionInwards.GeneralInput.StorageId
            });

            // Call
            project.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                assessmentSectionEntity
            }, projectEntity.AssessmentSectionEntities);

            mocks.VerifyAll();
        }
    }
}