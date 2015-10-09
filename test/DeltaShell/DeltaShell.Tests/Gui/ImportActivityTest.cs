using System.Linq;
using DelftTools.Shell.Core;
using DelftTools.Shell.Core.Workflow;
using DelftTools.Shell.Gui;
using NUnit.Framework;
using Rhino.Mocks;

namespace DeltaShell.Tests.Gui
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
            fileImportActivity.ImportedItemOwner = app.Project;
            fileImportActivity.OnImportFinished += (sender, importedObject, theImporter) =>
            {
                var projectToAddTo = sender.ImportedItemOwner as Project;

                if (projectToAddTo != null)
                {
                    projectToAddTo.Items.Add(importedObject);
                }
            };
            fileImportActivity.Initialize();
            fileImportActivity.Initialize();
            fileImportActivity.Execute();

            Assert.AreEqual(3, app.Project.Items.Count());
        }
    }
}