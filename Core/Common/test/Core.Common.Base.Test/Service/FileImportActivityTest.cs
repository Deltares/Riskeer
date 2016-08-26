// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

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
        public void Constructor_ImporterEqualsNull_ArgumentExceptionIsThrown()
        {
            // Setup
            TestDelegate test = () => new FileImportActivity(null, new object(), "", "");

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

            TestDelegate test = () => new FileImportActivity(fileImporter, null, "", "");

            // Call
            var message = Assert.Throws<ArgumentNullException>(test).Message;

            // Assert
            StringAssert.EndsWith("target", message);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FilePathEqualsNull_ArgumentExceptionIsThrown()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();

            mocks.ReplayAll();

            TestDelegate test = () => new FileImportActivity(fileImporter, new object(), null, "");

            // Call
            var message = Assert.Throws<ArgumentNullException>(test).Message;

            // Assert
            StringAssert.EndsWith("filePath", message);
            mocks.VerifyAll();
        }

        [Test]
        public void Name_FileImportActivityWithFileImporter_NameShouldBeSameAsImporterName()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();
            mocks.ReplayAll();

            const string name = "Importer name";

            // Call
            var fileImportActivity = new FileImportActivity(fileImporter, new object(), "", name);

            // Assert
            Assert.AreEqual(name, fileImportActivity.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void Run_FileImportActivityWithFileImporter_ProvidedFileShouldBeImported()
        {
            // Setup
            var target = new object();

            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();
            fileImporter.Stub(x => x.ProgressChanged = null).IgnoreArguments();
            fileImporter.Expect(i => i.Import("file")).Return(true);
            mocks.ReplayAll();

            var fileImportActivity = new FileImportActivity(fileImporter, target, "file", "");

            // Call
            fileImportActivity.Run();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Cancel_FileImportActivityWithFileImporter_CancelsImporter()
        {
            // Setup
            var target = new object();

            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();
            fileImporter.Stub(x => x.ProgressChanged = null).IgnoreArguments();
            fileImporter.Expect(x => x.Cancel());
            mocks.ReplayAll();

            var fileImportActivity = new FileImportActivity(fileImporter, target, "", "");

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

            var fileImportActivity = new FileImportActivity(fileImporter, target, "file", "");

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
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var target = new ObservableList<object>();
            target.Attach(observer);
            var fileImporter = new SimpleFileImporter();
            var fileImportActivity = new FileImportActivity(fileImporter, target, "", "");

            // Call
            fileImportActivity.Finish();

            // Assert
            mocks.VerifyAll();
        }

        private class SimpleFileImporter : FileImporterBase
        {
            public override ProgressChangedDelegate ProgressChanged { protected get; set; }

            public override bool Import(string filePath)
            {
                NotifyProgress("Step description", 1, 10);

                return true;
            }
        }
    }
}