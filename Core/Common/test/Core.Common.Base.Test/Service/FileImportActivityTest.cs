using System;
using System.Drawing;
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
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: fileImporter")]
        public void Constructor_ImporterEqualsNull_ArgumentExceptionIsThrown()
        {
            // Setup / Call / Assert
            new FileImportActivity(null, new object(), new[]
            {
                ""
            });
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: target")]
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
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: filePaths")]
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
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "filePaths")]
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
        public void Name_FileImportActivityWithFileImporter_NameShouldBeSameAsImporterName()
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
        public void Run_FileImportActivityWithFileImporterForTwoFiles_AllProvidedFilesShouldBeImported()
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
        public void Cancel_FileImportActivityWithFileImporter_CancelsImporter()
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
        public void Run_CancelledFileImportActivityWithFileImporterForTwoFiles_NoImportsShouldBePerformed()
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
        public void Run_CancelledAndRanFileImportActivityWithFileImporterForTwoFiles_AllProvidedFilesShouldBeImported()
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

        [Test]
        public void Run_FileImportActivityWithSimpleFileImporterForOneFile_ProgressTextShouldBeSetAfterImporterProgressChanged()
        {
            // Setup
            var target = new object();
            var fileImporter = new SimpleFileImporter();

            var fileImportActivity = new FileImportActivity(fileImporter, target, new[]
            {
                "file"
            });

            // Call
            fileImportActivity.Run(); // Reuse the activity

            // Assert
            Assert.AreEqual("Stap 1 van 10  |  Step description", fileImportActivity.ProgressText);
        }

        [Test]
        public void Finish_FileImportActivityWithFileImporter_NoLogicPerformed()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();
            var target = new object();

            mocks.ReplayAll();

            var fileImportActivity = new FileImportActivity(fileImporter, target, new[]
            {
                "file1",
                "file2"
            });

            // Call
            fileImportActivity.Finish();

            // Assert
            mocks.VerifyAll();
        }

        private class SimpleFileImporter : IFileImporter
        {
            public string Name
            {
                get
                {
                    return "";
                }
            }

            public string Category
            {
                get
                {
                    return "";
                }
            }

            public Bitmap Image
            {
                get
                {
                    return null;
                }
            }

            public Type SupportedItemType
            {
                get
                {
                    return null;
                }
            }

            public string FileFilter
            {
                get
                {
                    return "";
                }
            }

            public ProgressChangedDelegate ProgressChanged { private get; set; }

            public bool Import(object targetItem, string filePath)
            {
                if (ProgressChanged != null)
                {
                    ProgressChanged("Step description", 1, 10);
                }

                return true;
            }

            public void Cancel()
            {

            }
        }
    }
}
