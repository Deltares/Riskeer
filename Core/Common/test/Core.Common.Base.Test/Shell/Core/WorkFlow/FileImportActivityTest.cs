using Core.Common.Base.IO;
using Core.Common.Base.Service;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Base.Test.Shell.Core.WorkFlow
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

            var fileImportActivity = new FileImportTestActivity(importerStub);

            fileImportActivity.PerformInitialize();

            Assert.IsFalse(importerStub.ShouldCancel); // ShouldCancel should be reset

            mockRepository.VerifyAll();
        }

        private class FileImportTestActivity : FileImportActivity
        {
            public FileImportTestActivity(IFileImporter importer, object target = null) : base(importer, target) { }

            public void PerformInitialize()
            {
                Initialize();
            }
        }
    }
}