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
        public void Constructor_ImporterEqualsNull_ArgumentExceptionIsThrown()
        {
            // Setup
            TestDelegate test = () => new FileImportActivity(null, new object(), "");

            // Call
            var message = Assert.Throws<ArgumentNullException>(test).Message;

            // Assert
            StringAssert.EndsWith("fileImporter", message);
        }

        [Test]
        public void Constructor_TargetEqualsNull_ArgumentExceptionIsThrown()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();

            mocks.ReplayAll();

            TestDelegate test = () => new FileImportActivity(fileImporter, null, "");

            // Call
            var message = Assert.Throws<ArgumentNullException>(test).Message;

            // Assert
            StringAssert.EndsWith("target", message);
        }

        [Test]
        public void Constructor_FilePathEqualsNull_ArgumentExceptionIsThrown()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();

            mocks.ReplayAll();

            TestDelegate test = () => new FileImportActivity(fileImporter, new object(), null);

            // Call
            var message = Assert.Throws<ArgumentNullException>(test).Message;

            // Assert
            StringAssert.EndsWith("filePath", message);
        }

        [Test]
        public void Name_FileImportActivityWithFileImporter_NameShouldBeSameAsImporterName()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();

            fileImporter.Stub(i => i.Name).Return("Importer name");

            mocks.ReplayAll();

            // Call
            var fileImportActivity = new FileImportActivity(fileImporter, new object(), "");

            // Assert
            Assert.AreEqual(fileImporter.Name, fileImportActivity.Name);
        }

        [Test]
        public void Run_FileImportActivityWithFileImporter_ProvidedFileShouldBeImported()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();
            var target = new object();

            fileImporter.Stub(x => x.ProgressChanged = null).IgnoreArguments();
            fileImporter.Expect(i => i.Import(target, "file")).Return(true);

            mocks.ReplayAll();

            var fileImportActivity = new FileImportActivity(fileImporter, target, "file");

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

            fileImporter.Stub(x => x.ProgressChanged = null).IgnoreArguments();
            fileImporter.Expect(x => x.Cancel());

            mocks.ReplayAll();

            var fileImportActivity = new FileImportActivity(fileImporter, target, "");

            // Call
            fileImportActivity.Cancel();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Run_FileImportActivityWithSimpleFileImporter_ProgressTextShouldBeSetAfterImporterProgressChanged()
        {
            // Setup
            var target = new object();
            var fileImporter = new SimpleFileImporter();

            var fileImportActivity = new FileImportActivity(fileImporter, target, "file");

            // Call
            fileImportActivity.Run(); // Reuse the activity

            // Assert
            Assert.AreEqual("Stap 1 van 10  |  Step description", fileImportActivity.ProgressText);
        }

        [Test]
        public void Finish_FileImportActivityWithFileImporterAndObservableTarget_ObserversOfTargetAreNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();
            var observer = mocks.StrictMock<IObserver>();

            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var target = new ObservableList<object>();
            target.Attach(observer);

            var fileImportActivity = new FileImportActivity(fileImporter, target, "");

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
