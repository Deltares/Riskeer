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
            // Call
            TestDelegate test = () => new FileImportActivity(null, "");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            StringAssert.EndsWith("fileImporter", paramName);
        }

        [Test]
        public void Constructor_NameIsNull_ArgumentExceptionIsThrown()
        {
            // Setup
            var mocks = new MockRepository();
            var importer = mocks.Stub<IFileImporter>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FileImportActivity(importer, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            StringAssert.EndsWith("name", paramName);
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
            var fileImportActivity = new FileImportActivity(fileImporter, name);

            // Assert
            Assert.AreEqual(name, fileImportActivity.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void Run_FileImportActivityWithFileImporter_FileImporterImportCalled()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();
            fileImporter.Stub(x => x.ProgressChanged = null).IgnoreArguments();
            fileImporter.Expect(i => i.Import()).Return(true);
            mocks.ReplayAll();

            var fileImportActivity = new FileImportActivity(fileImporter, "");

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
            fileImporter.Stub(x => x.ProgressChanged = null).IgnoreArguments();
            fileImporter.Expect(x => x.Cancel());
            mocks.ReplayAll();

            var fileImportActivity = new FileImportActivity(fileImporter, "");

            // Call
            fileImportActivity.Cancel();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Run_FileImportActivityWithSimpleFileImporter_ProgressTextShouldBeSetAfterImporterProgressChanged()
        {
            // Setup
            var fileImporter = new SimpleFileImporter<object>(new object());

            var fileImportActivity = new FileImportActivity(fileImporter, "");

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

            var fileImporter = new SimpleFileImporter<ObservableList<object>>(target);
            var fileImportActivity = new FileImportActivity(fileImporter, "");

            // Call
            fileImportActivity.Finish();

            // Assert
            mocks.VerifyAll();
        }

        private class SimpleFileImporter<T> : FileImporterBase<T>
        {
            public SimpleFileImporter(T importTarget) : base("", importTarget) {}

            public override bool Import()
            {
                NotifyProgress("Step description", 1, 10);

                return true;
            }
        }
    }
}