using Core.Common.BaseDelftTools;
using Core.Common.BaseDelftTools.Workflow;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.DelftTools.Tests.Shell.Core.WorkFlow
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