using System;
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
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "fileImporter")]
        public void Constructor_ImporterEqualsNull_ArgumentExceptionIsThrown()
        {
            new FileImportActivity(null, new object(), new[]
            {
                ""
            });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "target")]
        public void Constructor_TargetEqualsNull_ArgumentExceptionIsThrown()
        {
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();

            mocks.ReplayAll();

            new FileImportActivity(fileImporter, null, new[]
            {
                ""
            });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "files")]
        public void Constructor_FilesEqualsNull_ArgumentExceptionIsThrown()
        {
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();

            mocks.ReplayAll();

            new FileImportActivity(fileImporter, new object(), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "files")]
        public void Constructor_FilesEmpty_ArgumentExceptionIsThrown()
        {
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();

            mocks.ReplayAll();

            new FileImportActivity(fileImporter, new object(), new string[0]);
        }
    }
}
