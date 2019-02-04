// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Base.Test.IO
{
    [TestFixture]
    public class FileImporterBaseTest
    {
        [Test]
        public void Constructor_FilePathNull_ThrowArgumentNullException()
        {
            // Setup
            var importTarget = new object();

            // Call
            TestDelegate call = () => new SimpleFileImporter<object>(null, importTarget);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("filePath", paramName);
        }

        [Test]
        public void Constructor_ImportTargetNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SimpleFileImporter<object>(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("importTarget", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string filePath = "C://temp";
            var importTarget = new object();

            // Call
            var simpleImporter = new SimpleFileImporter<object>(filePath, importTarget);

            // Assert
            Assert.IsInstanceOf<IFileImporter>(simpleImporter);
            Assert.AreEqual(filePath, simpleImporter.GetFilePath());
        }

        [Test]
        public void DoPostImportUpdates_TargetIsObservable_NotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observableInstance = mocks.Stub<IObservable>();
            observableInstance.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var simpleImporter = new SimpleFileImporter<IObservable>(observableInstance);

            // Call
            simpleImporter.DoPostImport();

            // Assert
            mocks.VerifyAll(); // Assert NotifyObservers is called
        }

        [Test]
        public void DoPostImportUpdates_ImportCanceled_NoNotifyObserversCalled()
        {
            // Setup
            var mocks = new MockRepository();
            var observableTarget = mocks.StrictMock<IObservable>();
            mocks.ReplayAll();

            var simpleImporter = new SimpleFileImporter<IObservable>(observableTarget);
            simpleImporter.Cancel();

            // Call
            simpleImporter.DoPostImport();

            // Assert
            mocks.VerifyAll(); // Assert no NotifyObservers were called
        }

        [Test]
        public void NotifyProgress_NoDelegateSet_DoNothing()
        {
            // Setup
            var importTarget = new object();
            var simpleImporter = new SimpleFileImporter<object>(importTarget);
            simpleImporter.SetProgressChanged(null);

            // Call
            TestDelegate call = () => simpleImporter.TestNotifyProgress("A", 1, 2);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void NotifyProgress_DelegateSet_CallProgressChanged()
        {
            // Setup
            const string expectedDescription = "B";
            const int expectedStep = 1;
            const int expectedNumberOfSteps = 3;

            var importTarget = new object();
            var simpleImporter = new SimpleFileImporter<object>(importTarget);
            var progressChangedCallCount = 0;
            simpleImporter.SetProgressChanged((description, step, steps) =>
            {
                Assert.AreEqual(expectedDescription, description);
                Assert.AreEqual(expectedStep, step);
                Assert.AreEqual(expectedNumberOfSteps, steps);
                progressChangedCallCount++;
            });

            // Call
            simpleImporter.TestNotifyProgress(expectedDescription, expectedStep, expectedNumberOfSteps);

            // Assert
            Assert.AreEqual(1, progressChangedCallCount);
        }

        [Test]
        public void Import_CancelGivesSuccessfulImport_LogsMessage()
        {
            //  Setup
            var importTarget = new object();
            var simpleImporter = new SimpleFileImporter<object>(importTarget)
            {
                ImportSuccessful = true
            };

            simpleImporter.SetProgressChanged((description, step, steps) => simpleImporter.Cancel());

            // Call
            Action call = () => simpleImporter.Import();

            // Assert
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>(
                                                                "Huidige actie was niet meer te annuleren en is daarom voortgezet.",
                                                                LogLevelConstant.Warn));
        }

        [Test]
        public void Import_CancelGivesUnsuccessfulImport_CallsLogImportCanceledMessage()
        {
            //  Setup
            var importTarget = new object();
            var simpleImporter = new SimpleFileImporter<object>(importTarget)
            {
                ImportSuccessful = false
            };

            simpleImporter.SetProgressChanged((description, step, steps) => simpleImporter.Cancel());

            // Call
            simpleImporter.Import();

            // Assert
            Assert.IsTrue(simpleImporter.LogCanceledMessageCalled);
        }

        [Test]
        public void Import_ImportSuccessful_LogsImportSuccessfulMessage()
        {
            // Setup
            var importTarget = new object();
            var simpleImporter = new SimpleFileImporter<object>(importTarget)
            {
                ImportSuccessful = true
            };

            // Call
            Action call = () => simpleImporter.Import();

            // Asset
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>(
                                                                $"Gegevens zijn geïmporteerd vanuit bestand '{string.Empty}'.",
                                                                LogLevelConstant.Info));
        }

        private class SimpleFileImporter<T> : FileImporterBase<T>
        {
            public SimpleFileImporter(T importTarget) : this("", importTarget) {}

            public SimpleFileImporter(string filePath, T importTarget) : base(filePath, importTarget)
            {
                LogCanceledMessageCalled = false;
            }

            public bool ImportSuccessful { private get; set; }

            public bool LogCanceledMessageCalled { get; private set; }

            public void TestNotifyProgress(string currentStepName, int currentStep, int totalNumberOfSteps)
            {
                NotifyProgress(currentStepName, currentStep, totalNumberOfSteps);
            }

            public string GetFilePath()
            {
                return FilePath;
            }

            protected override bool OnImport()
            {
                TestNotifyProgress(string.Empty, 1, 1);
                return ImportSuccessful;
            }

            protected override void LogImportCanceledMessage()
            {
                LogCanceledMessageCalled = true;
            }
        }
    }
}