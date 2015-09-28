using DelftTools.Shell.Core;
using DelftTools.Shell.Core.Workflow;
using NUnit.Framework;
using Rhino.Mocks;

namespace DelftTools.Tests.Shell.Core.WorkFlow
{
    [TestFixture]
    public class FileImportActivityTest
    {
        [Test]
        public void ShouldCancelOfImporterIsResetDuringInitialize()
        {
            var mockRepository = new MockRepository();
            var importerStub = mockRepository.Stub<IFileImporter>();

            mockRepository.ReplayAll();

            importerStub.ShouldCancel = true; // Fake the importer being cancelled

            var fileImportActivity = new FileImportActivity(importerStub);

            fileImportActivity.Initialize();

            Assert.IsFalse(importerStub.ShouldCancel); // ShouldCancel should be reset

            mockRepository.VerifyAll();
        }
    }
}
