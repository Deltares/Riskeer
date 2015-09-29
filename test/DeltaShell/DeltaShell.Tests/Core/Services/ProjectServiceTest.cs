using System;
using System.IO;
using DelftTools.Shell.Core;
using DelftTools.Shell.Core.Dao;
using DeltaShell.Core.Services;
using NUnit.Framework;
using Rhino.Mocks;
using SharpTestsEx;

namespace DeltaShell.Tests.Core.Services
{
    /// <summary>
    /// Move MRU tests tot separate test case
    /// </summary>
    [TestFixture]
    public class ProjectServiceTest
    {
        private MockRepository mocks;
        private IProjectRepository repository;
        private IProjectRepositoryFactory factory;

        [SetUp]
        public void SetUpMocks()
        {
            mocks = new MockRepository();
            repository = mocks.DynamicMock<IProjectRepository>();
            factory = mocks.Stub<IProjectRepositoryFactory>();
            Expect.Call(factory.CreateNew()).Return(repository).Repeat.Once();
        }

        [Test]
        public void CreateNewProjectShouldCreateATemporaryProject()
        {
            var newProject = new Project();
            repository.Expect(r => r.GetProject()).Return(newProject).Repeat.Any();

            Expect.Call(repository.Path).Repeat.Any().Return("");

            mocks.ReplayAll();

            var projectService = new ProjectService(factory);

            var project = projectService.CreateNewProjectInTemporaryFolder();

            mocks.VerifyAll();

            Assert.IsTrue(project.IsTemporary, "project should be temporary");
        }

        [Test]
        public void CloseProjectClosesRepository()
        {
            var project = new Project("project");

            Expect.Call(repository.Path).Repeat.Any().Return("a_path");
            Expect.Call(repository.IsOpen).Return(true);
            Expect.Call(repository.Close);

            mocks.ReplayAll();

            var projectService = new ProjectService(factory);

            projectService.Close(project);

            mocks.VerifyAll();
        }

        [Test]
        public void SaveProjectAsTemporyProjectShouldMarkProjectAsNotTemporary()
        {
            var project = new Project("project") { IsTemporary = true };

            Expect.Call(repository.Path).Return("a_path").Repeat.Any();
            Expect.Call(delegate { repository.SaveAs(null, Path.GetFullPath("myproject.dsproj")); }).IgnoreArguments().Repeat.Once();

            mocks.ReplayAll();

            var projectService = new ProjectService(factory);

            projectService.SaveProjectAs(project, "myproject.dsproj");

            mocks.VerifyAll();
            Assert.IsFalse(project.IsTemporary);
        }

        [Test]
        public void SaveProjectToSameLocation()
        {

            var project = new Project("project") { IsChanged = true };

            Expect.Call(repository.Path).Repeat.Any().Return(Path.GetFullPath("myproject.dsproj"));
            Expect.Call(delegate { repository.SaveOrUpdate(project); }).Repeat.Once();

            mocks.ReplayAll();

            var projectService = new ProjectService(factory);

            projectService.SaveProjectAs(project, "myproject.dsproj");

            mocks.VerifyAll();
        }

        [Test]
        public void DisposeShouldCallDisposeOfRepository()
        {
            Expect.Call(repository.IsOpen).Return(true);
            Expect.Call(() => repository.Dispose());
            mocks.ReplayAll();

            var projectService = new ProjectService(factory);
            projectService.Dispose();
            mocks.VerifyAll();
        }

        [Test]
        public void FireSavingAndSavedEvents()
        {
            Expect.Call(repository.Path).Return(@"C:\mockpath").Repeat.Any();
            mocks.ReplayAll();

            var savedCallCount = 0;
            var savingCallCount = 0;
            var projectService = new ProjectService(factory);
            projectService.ProjectSaved += delegate { savedCallCount++; };
            projectService.ProjectSaving += delegate { savingCallCount++; };
            var project = new Project();
            projectService.Save(project);

            mocks.VerifyAll();

            savedCallCount.Should().Be.EqualTo(1);
            savingCallCount.Should().Be.EqualTo(1);
        }

        [Test]
        public void FailingSaveShouldFireFailEvent()
        {
            Expect.Call(repository.Path).Return(@"C:\mockpath").Repeat.Any();
            Expect.Call(() => repository.SaveOrUpdate(Arg<Project>.Is.Anything))
                .IgnoreArguments()
                .Throw(new Exception("save failed"))
                .Repeat.Any();
            mocks.ReplayAll();

            var savedCallCount = 0;
            var savingCallCount = 0;
            var saveFailedCallCount = 0;
            var projectService = new ProjectService(factory);
            projectService.ProjectSaved += delegate { savedCallCount++; };
            projectService.ProjectSaving += delegate { savingCallCount++; };
            projectService.ProjectSaveFailed += delegate { saveFailedCallCount++; };
            var project = new Project();
            Assert.Throws<Exception>(() => projectService.Save(project));

            mocks.VerifyAll();

            savingCallCount.Should().Be.EqualTo(1);
            savedCallCount.Should().Be.EqualTo(0);
            saveFailedCallCount.Should().Be.EqualTo(1);
        }
    }
}