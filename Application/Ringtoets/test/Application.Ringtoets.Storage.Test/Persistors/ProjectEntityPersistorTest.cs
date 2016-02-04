using System;
using System.Collections.Generic;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Persistors;
using Application.Ringtoets.Storage.Test.DbContext;
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace Application.Ringtoets.Storage.Test.Persistors
{
    [TestFixture]
    public class ProjectEntityPersistorTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullDataSet_ThrowsArgumentNullException()
        {
            // Call
            ProjectEntityPersistor p = new ProjectEntityPersistor(null);
        }

        [Test]
        public void Constructor_EmptyDataset_NewInstance()
        {
            // Setup
            var dbset = DbTestSet.GetDbTestSet(mockRepository, new List<ProjectEntity>());
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            mockRepository.ReplayAll();

            // Call
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Assert
            Assert.IsInstanceOf<ProjectEntityPersistor>(persistor);
        }

        [Test]
        public void GetEntityAsModel_EmptyDataset_DoesNotThrowException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();

            var dbset = DbTestSet.GetDbTestSet(mockRepository, new List<ProjectEntity>());
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            TestDelegate test = () => persistor.GetEntityAsModel();

            // Assert
            Assert.DoesNotThrow(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetEntityAsModel_SingleEntityInDataSet_ProjectEntityFromDataSetAsModel()
        {
            // Setup
            const long storageId = 1234L;
            const string description = "description";
            var dbset = DbTestSet.GetDbTestSet(mockRepository, new List<ProjectEntity>
            {
                new ProjectEntity
                {
                    ProjectEntityId = storageId,
                    Description = description
                }
            });
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            Project model = persistor.GetEntityAsModel();

            // Assert
            Assert.IsInstanceOf<Project>(model);
            Assert.AreEqual(storageId, model.StorageId);
            Assert.AreEqual(description, model.Description);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetEntityAsModel_NoProjectNameSet_ProjectEntityFromDataSetAsModelWithDefaultName()
        {
            // Setup
            const long storageId = 1234L;
            const string description = "description";
            string defaultProjectName = new Project().Name;
            var dbset = DbTestSet.GetDbTestSet(mockRepository, new List<ProjectEntity>
            {
                new ProjectEntity
                {
                    ProjectEntityId = storageId,
                    Description = description
                }
            });
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            Project model = persistor.GetEntityAsModel();

            // Assert
            Assert.IsInstanceOf<Project>(model);
            Assert.AreEqual(storageId, model.StorageId);
            Assert.AreEqual(defaultProjectName, model.Name);
            Assert.AreEqual(description, model.Description);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetEntityAsModel_MultipleEntitiesInDataSet_ThrowsInvalidOperationException()
        {
            // Setup
            var dbset = DbTestSet.GetDbTestSet(mockRepository, new List<ProjectEntity>
            {
                new ProjectEntity
                {
                    ProjectEntityId = 1
                },
                new ProjectEntity
                {
                    ProjectEntityId = 2
                }
            });
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            TestDelegate test = () => persistor.GetEntityAsModel();

            // Assert
            Assert.Throws<InvalidOperationException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void AddEntity_NullData_ThrowsArgumentNullException()
        {
            // Setup
            var dbSetMethodAddWasHit = 0;
            var dbset = DbTestSet.GetDbTestSet(mockRepository, new List<ProjectEntity>());
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            dbset.Stub(m => m.Add(new ProjectEntity())).IgnoreArguments().Return(new ProjectEntity())
                 .WhenCalled(invocation => dbSetMethodAddWasHit++);
            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            TestDelegate test = () => persistor.AddEntity(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual(0, dbSetMethodAddWasHit);

            mockRepository.VerifyAll();
        }

        [Test]
        public void AddEntity_ValidProject_UpdatedDataSet()
        {
            // Setup
            var dbSetMethodAddWasHit = 0;
            const long storageId = 1234L;
            const string description = "description";
            var dbset = DbTestSet.GetDbTestSet(mockRepository, new List<ProjectEntity>());
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset).Repeat.Twice();
            ProjectEntity projectEntity = new ProjectEntity();
            Project project = new Project
            {
                StorageId = storageId,
                Description = description
            };
            dbset.Stub(m => m.Add(projectEntity)).IgnoreArguments().Return(projectEntity)
                 .WhenCalled(invocation => dbSetMethodAddWasHit++);
            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            persistor.AddEntity(project);

            // Assert
            Assert.AreEqual(1, dbSetMethodAddWasHit);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateEntity_NullData_ThrowsArgumentNullException()
        {
            // Setup
            var dbset = DbTestSet.GetDbTestSet(mockRepository, new List<ProjectEntity>());
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            TestDelegate test = () => persistor.UpdateEntity(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateEntity_UnknownProject_ThrowsException()
        {
            // Setup
            Project project = new Project
            {
                StorageId = 1
            };
            var dbset = DbTestSet.GetDbTestSet(mockRepository, new List<ProjectEntity>
            {
                new ProjectEntity
                {
                    ProjectEntityId = 2
                }
            });
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            TestDelegate test = () => persistor.UpdateEntity(project);

            // Assert
            Assert.Throws<EntityNotFoundException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateEntity_ValidProject_ReturnsTheProjectAsProjectEntity()
        {
            // Setup
            const long storageId = 1234L;
            Project project = new Project
            {
                StorageId = storageId
            };
            var dbset = DbTestSet.GetDbTestSet(mockRepository, new List<ProjectEntity>
            {
                new ProjectEntity
                {
                    ProjectEntityId = storageId
                }
            });
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            ProjectEntity entity = persistor.UpdateEntity(project);

            // Assert
            Assert.IsInstanceOf<ProjectEntity>(entity);
            Assert.AreEqual(project.StorageId, entity.ProjectEntityId);

            mockRepository.VerifyAll();
        }
    }
}