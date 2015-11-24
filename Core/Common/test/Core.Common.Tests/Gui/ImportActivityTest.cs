using Core.Common.Base;
using Core.Common.Base.Workflow;
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Tests.Gui
{
    [TestFixture]
    public class ImportActivityTest
    {
        private readonly MockRepository mockRepository = new MockRepository();

        [Test]
        public void FileImportActivity()
        {
            var gui = mockRepository.Stub<IGui>();

            var project = new Project("test");

            var importer = mockRepository.Stub<IFileImporter>();
            var fileImportActivity = new FileImportActivity(importer)
            {
                Files = new[]
                {
                    "test1",
                    "test2",
                    "test3"
                }
            };

            Expect.Call(importer.ImportItem("test1")).Repeat.Once().Return(new object());
            Expect.Call(importer.ImportItem("test2")).Repeat.Once().Return(new object());
            Expect.Call(importer.ImportItem("test3")).Repeat.Once().Return(new object());

            mockRepository.ReplayAll();

            var applicationCore = new ApplicationCore
            {
                Project = project
            };

            gui.ApplicationCore = applicationCore;

            // expect some reporting while processing each file
            fileImportActivity.OnImportFinished += (sender, importedObject, theImporter) =>
            {
                project.Items.Add(importedObject);
            };

            fileImportActivity.Initialize();
            fileImportActivity.Initialize();
            fileImportActivity.Execute();

            Assert.AreEqual(3, applicationCore.Project.Items.Count);

            mockRepository.VerifyAll();
        }
    }
}