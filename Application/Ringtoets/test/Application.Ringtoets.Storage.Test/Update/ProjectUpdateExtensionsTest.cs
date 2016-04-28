using System;
using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class ProjectUpdateExtensionsTest
    {
        [Test]
        public void Update_WithoutContext_ArgumentNullException()
        {
            // Setup
            var project = new Project();

            // Call
            TestDelegate test = () => project.Update(new UpdateConversionCollector(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        } 

        [Test]
        public void Update_WithoutCollector_ArgumentNullException()
        {
            // Setup
            var project = new Project();

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
            Assert.AreEqual("collector", paramName);
        } 

        [Test]
        public void Update_ContextWithNoProject_EntityNotFoundException()
        {
            // Setup
            var project = new Project();

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    project.Update(new UpdateConversionCollector(), ringtoetsEntities);
                }
            };

            // Assert
            Assert.Throws<EntityNotFoundException>(test);
        } 

        [Test]
        public void Update_ContextWithNoProjectWithId_EntityNotFoundException()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var storageId = 1;
            var project = new Project
            {
                StorageId = storageId
            };

            ringtoetsEntities.ProjectEntities.Add(new ProjectEntity
            {
                ProjectEntityId = 2
            });

            // Call
            TestDelegate test = () => project.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            var expectedMessage = String.Format("Het object 'ProjectEntity' met id '{0}' is niet gevonden.", storageId);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithMultipleProjectsWithId_EntityNotFoundException()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var storageId = 1;
            var project = new Project
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
            TestDelegate test = () => project.Update(new UpdateConversionCollector(), ringtoetsEntities);

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
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var newDescription = "newDescription";
            var project = new Project
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
            project.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(newDescription, entity.Description);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ProjectWithNewSection_SectionAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var newDescription = "newDescription";
            var project = new Project
            {
                StorageId = 1,
                Items =
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
            project.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, entity.AssessmentSectionEntities.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ProjectWithExistingSection_NoSectionsAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            var project = new Project
            {
                StorageId = 1,
                Items =
                {
                    new AssessmentSection(AssessmentSectionComposition.Dike)
                    {
                        StorageId = 1
                    }
                }
            };

            var assessmentSectionEntity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = 1
            };
            var projectEntity = new ProjectEntity
            {
                ProjectEntityId = 1,
                AssessmentSectionEntities =
                {
                    assessmentSectionEntity
                }
            };

            ringtoetsEntities.ProjectEntities.Add(projectEntity);
            ringtoetsEntities.AssessmentSectionEntities.Add(assessmentSectionEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(new FailureMechanismEntity());

            // Call
            project.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            CollectionAssert.AreEqual(new [] {assessmentSectionEntity}, projectEntity.AssessmentSectionEntities);

            mocks.VerifyAll();
        } 
    }
}