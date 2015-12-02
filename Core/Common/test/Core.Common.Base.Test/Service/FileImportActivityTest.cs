using Core.Common.Base.IO;
using Core.Common.Base.Service;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Base.Test.Service
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

            var fileImportActivity = new FileImportTestActivity(importerStub, new object(), new string[0]);

            fileImportActivity.PerformInitialize();

            Assert.IsFalse(importerStub.ShouldCancel); // ShouldCancel should be reset

            mockRepository.VerifyAll();
        }

        private class FileImportTestActivity : FileImportActivity
        {
            public FileImportTestActivity(IFileImporter importer, object target, string[] files) : base(importer, target, files) { }

            public void PerformInitialize()
            {
                Initialize();
            }
        }
    }
}