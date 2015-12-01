using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.Base.Service;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Test.Gui
{
    [TestFixture]
    public class ImportActivityTest
    {
        private readonly MockRepository mockRepository = new MockRepository();

        [Test]
        public void FileImportActivity()
        {
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

            Expect.Call(importer.ImportItem("test1")).Repeat.Once().Return(new object()).WhenCalled(o =>
            {
                project.Items.Add(o);
            });
            Expect.Call(importer.ImportItem("test2")).Repeat.Once().Return(new object()).WhenCalled(o =>
            {
                project.Items.Add(o);
            });
            Expect.Call(importer.ImportItem("test3")).Repeat.Once().Return(new object()).WhenCalled(o =>
            {
                project.Items.Add(o);
            });

            mockRepository.ReplayAll();

            fileImportActivity.Run();

            Assert.AreEqual(3, project.Items.Count);

            mockRepository.VerifyAll();
        }
    }
}