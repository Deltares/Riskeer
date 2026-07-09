// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using NSubstitute;
using NUnit.Framework;

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
            Assert.AreEqual("fileImporter", paramName);
        }

        [Test]
        public void Constructor_DescriptionIsNull_ArgumentExceptionIsThrown()
        {
            // Setup
            var importer = Substitute.For<IFileImporter>();

            // Call
            TestDelegate call = () => new FileImportActivity(importer, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("description", paramName);
        }

        [Test]
        public void Description_FileImportActivityWithFileImporter_DescriptionShouldBeSameAsImporterDescription()
        {
            // Setup
            var fileImporter = Substitute.For<IFileImporter>();

            const string description = "Importer description";

            // Call
            var fileImportActivity = new FileImportActivity(fileImporter, description);

            // Assert
            Assert.AreEqual(description, fileImportActivity.Description);
        }

        [Test]
        public void Run_FileImportActivityWithFileImporter_FileImporterImportCalled()
        {
            // Setup
            var fileImporter = Substitute.For<IFileImporter>();
            fileImporter.Import().Returns(true);

            var fileImportActivity = new FileImportActivity(fileImporter, "");

            // Call
            fileImportActivity.Run();

            // Assert
            fileImporter.Received().Import();
        }

        [Test]
        public void Run_FileImportActivityWithFileImporterAndImportFails_ImportActivityStateFailed()
        {
            // Setup
            var fileImporter = Substitute.For<IFileImporter>();
            fileImporter.Import().Returns(false);

            var fileImportActivity = new FileImportActivity(fileImporter, "");

            // Call
            fileImportActivity.Run();

            // Assert
            Assert.AreEqual(ActivityState.Failed, fileImportActivity.State);
            fileImporter.Received().Import();
        }

        [Test]
        public void Cancel_WhenImportingAndUncancelable_ImportActivityStateExecuted()
        {
            // Setup
            var fileImporter = Substitute.For<IFileImporter>();
            fileImporter.Import().Returns(true);

            var fileImportActivity = new FileImportActivity(fileImporter, "");
            fileImportActivity.ProgressChanged += (sender, args) =>
            {
                if (fileImportActivity.State != ActivityState.Canceled)
                {
                    // Call 
                    fileImportActivity.Cancel();
                }
            };

            // Assert
            fileImportActivity.Run();
            Assert.AreEqual(ActivityState.Executed, fileImportActivity.State);
        }

        [Test]
        public void Cancel_WhenImportingAndCancelable_ImportActivityStateCanceled()
        {
            // Setup
            var fileImporter = new SimpleFileImporter<object>(new object());
            var fileImportActivity = new FileImportActivity(fileImporter, "");
            fileImportActivity.ProgressChanged += (sender, args) =>
            {
                if (fileImportActivity.State != ActivityState.Canceled)
                {
                    // Call 
                    fileImportActivity.Cancel();
                }
            };

            // Assert
            fileImportActivity.Run();
            Assert.AreEqual(ActivityState.Canceled, fileImportActivity.State);
        }

        [Test]
        public void Cancel_FileImportActivityWithFileImporter_CancelsImporter()
        {
            // Setup
            var fileImporter = Substitute.For<IFileImporter>();

            var fileImportActivity = new FileImportActivity(fileImporter, "");

            // Call
            fileImportActivity.Cancel();

            // Assert
            fileImporter.Received().Cancel();
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
        [TestCase(ActivityState.Executed)]
        [TestCase(ActivityState.Failed)]
        [TestCase(ActivityState.Canceled)]
        [TestCase(ActivityState.Skipped)]
        public void Finish_FileImportActivityWithFileImporterObservableTargetAndSpecificState_ObserversOfTargetAreNotified(ActivityState state)
        {
            // Setup
            var observer = Substitute.For<IObserver>();

            var target = new ObservableList<object>();
            target.Attach(observer);

            var fileImporter = new SimpleFileImporter<ObservableList<object>>(target);
            var fileImportActivity = new TestFileImportActivity(fileImporter, "", state);

            // Call
            fileImportActivity.Finish();

            // Assert
            observer.Received().UpdateObserver();
        }

        [Test]
        public void Finish_FileImportActivityWithFileImporterObservableTargetAndNoneState_ObserversOfTargetAreNotNotified()
        {
            // Setup
            var observer = Substitute.For<IObserver>();

            var target = new ObservableList<object>();
            target.Attach(observer);

            var fileImporter = new SimpleFileImporter<ObservableList<object>>(target);
            var fileImportActivity = new TestFileImportActivity(fileImporter, "", ActivityState.None);

            // Call
            fileImportActivity.Finish();

            // Assert
            observer.DidNotReceive().UpdateObserver();
        }

        private class SimpleFileImporter<T> : FileImporterBase<T> where T : class
        {
            public SimpleFileImporter(T importTarget) : base("", importTarget) {}

            protected override void LogImportCanceledMessage() {}

            protected override bool OnImport()
            {
                NotifyProgress("Step description", 1, 10);

                return false;
            }
        }

        private class TestFileImportActivity : FileImportActivity
        {
            public TestFileImportActivity(IFileImporter fileImporter, string description, ActivityState state)
                : base(fileImporter, description)
            {
                State = state;
            }
        }
    }
}