using System.Linq;
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
            // wrap importer by activity
            var gui = mockRepository.Stub<IGui>();
            var app = mockRepository.Stub<IApplication>();
            var project = new Project("test");

            gui.Application = app;
            app.Expect(a => a.Project).Return(project).Repeat.Any();

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

            // expect some reporting while processing each file
            fileImportActivity.OnImportFinished += (sender, importedObject, theImporter) =>
            {
                project.Items.Add(importedObject);
            };

            fileImportActivity.Initialize();
            fileImportActivity.Initialize();
            fileImportActivity.Execute();

            Assert.AreEqual(3, app.Project.Items.Count());
        }
    }
}