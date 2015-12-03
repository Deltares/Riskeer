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
            // Setup / Call / Assert
            new FileImportActivity(null, new object(), new[]
            {
                ""
            });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "target")]
        public void Constructor_TargetEqualsNull_ArgumentExceptionIsThrown()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();

            mocks.ReplayAll();

            // Call / Assert
            new FileImportActivity(fileImporter, null, new[]
            {
                ""
            });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "files")]
        public void Constructor_FilesEqualsNull_ArgumentExceptionIsThrown()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();

            mocks.ReplayAll();

            // Call / Assert
            new FileImportActivity(fileImporter, new object(), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "files")]
        public void Constructor_FilesEmpty_ArgumentExceptionIsThrown()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();

            mocks.ReplayAll();

            // Call / Assert
            new FileImportActivity(fileImporter, new object(), new string[0]);
        }

        [Test]
        public void Name_FileImportActivity_NameShouldBeSameAsImporterName()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();

            fileImporter.Expect(i => i.Name).Return("Importer name").Repeat.Any();

            mocks.ReplayAll();

            // Call
            var fileImportActivity = new FileImportActivity(fileImporter, new object(), new[]
            {
                ""
            });

            // Assert
            Assert.AreEqual(fileImporter.Name, fileImportActivity.Name);
        }

        [Test]
        public void Run_FileImportActivityForTwoFiles_AllProvidedFilesShouldBeImported()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();
            var target = new object();

            fileImporter.Expect(x => x.ProgressChanged = null).IgnoreArguments().Repeat.Any();
            fileImporter.Expect(i => i.Import(target, "file1")).Return(true).Repeat.Once();
            fileImporter.Expect(i => i.Import(target, "file2")).Return(true).Repeat.Once();

            mocks.ReplayAll();

            var fileImportActivity = new FileImportActivity(fileImporter, target, new[]
            {
                "file1",
                "file2"
            });

            // Call
            fileImportActivity.Run();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Cancel_FileImportActivity_CancelsImporter()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();
            var target = new object();

            fileImporter.Expect(x => x.ProgressChanged = null).IgnoreArguments().Repeat.Any();
            fileImporter.Expect(x => x.Cancel()).IgnoreArguments().Repeat.Any();

            mocks.ReplayAll();

            var fileImportActivity = new FileImportActivity(fileImporter, target, new[]
            {
                "file1",
                "file2"
            });

            // Call
            fileImportActivity.Cancel();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Run_CancelledFileImportActivityForTwoFiles_NoImportsShouldBePerformed()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();
            var target = new object();

            fileImporter.Expect(x => x.ProgressChanged = null).IgnoreArguments().Repeat.Any();
            fileImporter.Expect(x => x.Cancel()).IgnoreArguments().Repeat.Any();

            mocks.ReplayAll();

            var fileImportActivity = new FileImportActivity(fileImporter, target, new[]
            {
                "file1",
                "file2"
            });

            fileImportActivity.Cancel();

            // Call
            fileImportActivity.Run();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Run_CancelledAndRanFileImportActivityForTwoFiles_AllProvidedFilesShouldBeImported()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();
            var target = new object();

            fileImporter.Expect(x => x.ProgressChanged = null).IgnoreArguments().Repeat.Any();
            fileImporter.Expect(i => i.Import(target, "file1")).Return(true).Repeat.Once();
            fileImporter.Expect(i => i.Import(target, "file2")).Return(true).Repeat.Once();
            fileImporter.Expect(x => x.Cancel()).IgnoreArguments().Repeat.Any();

            mocks.ReplayAll();

            var fileImportActivity = new FileImportActivity(fileImporter, target, new[]
            {
                "file1",
                "file2"
            });

            fileImportActivity.Cancel();
            fileImportActivity.Run();

            // Call
            fileImportActivity.Run(); // Reuse the activity

            // Assert
            mocks.VerifyAll();
        }
    }
}
