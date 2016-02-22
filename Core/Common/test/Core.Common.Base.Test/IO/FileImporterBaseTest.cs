﻿using System;
using System.Collections.Generic;
using System.Drawing;

using Core.Common.Base.IO;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Base.Test.IO
{
    [TestFixture]
    public class FileImporterBaseTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup

            // Call
            var simpleImporter = new SimpleFileImporter();

            // Assert
            Assert.IsInstanceOf<IFileImporter>(simpleImporter);
        }

        [Test]
        public void DoPostImportUpdates_TargetIsObservable_NotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observableInstance = mocks.Stub<IObservable>();
            observableInstance.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var simpleImporter = new SimpleFileImporter();

            // Call
            simpleImporter.DoPostImportUpdates(observableInstance);

            // Assert
            mocks.VerifyAll(); // Assert NotifyObservers is called
        }

        [Test]
        public void DoPostImportUpdates_HasAffectedOtherObjects_NotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observableInstance = mocks.Stub<IObservable>();
            observableInstance.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var simpleImporter = new SimpleFileImporter
            {
                GetAffectedNonTargetObservableInstancesOverride = new[]
                {
                    observableInstance
                }
            };

            // Call
            simpleImporter.DoPostImportUpdates(new object());

            // Assert
            mocks.VerifyAll(); // Assert NotifyObservers is called
        }

        [Test]
        public void DoPostImportUpdates_ImportCancelled_NoNotifyObserversCalled()
        {
            // Setup
            var mocks = new MockRepository();
            var observableInstance = mocks.StrictMock<IObservable>();

            var observableTarget = mocks.StrictMock<IObservable>();
            mocks.ReplayAll();

            var simpleImporter = new SimpleFileImporter
            {
                GetAffectedNonTargetObservableInstancesOverride = new[]
                {
                    observableInstance
                }
            };
            simpleImporter.Cancel();

            // Call
            simpleImporter.DoPostImportUpdates(observableTarget);

            // Assert
            mocks.VerifyAll(); // Assert no NotifyObservers were called
        }

        [Test]
        public void NotifyProgress_NoDelegateSet_DoNothing()
        {
            // Setup
            var simpleImporter = new SimpleFileImporter
            {
                ProgressChanged = null
            };

            // Call
            TestDelegate call = () => simpleImporter.TestNotifyProgress("A", 1, 2);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void NotifyProgress_DelegateSet_CallProgressChanged()
        {
            // Setup
            var expectedDescription = "B";
            var expectedStep = 1;
            var expectedNumberOfSteps = 3;

            var simpleImporter = new SimpleFileImporter();
            int progressChangedCallCount = 0;
            simpleImporter.ProgressChanged = (description, step, steps) =>
            {
                Assert.AreEqual(expectedDescription, description);
                Assert.AreEqual(expectedStep, step);
                Assert.AreEqual(expectedNumberOfSteps, steps);
                progressChangedCallCount++;
            };

            // Call
            simpleImporter.TestNotifyProgress(expectedDescription, expectedStep, expectedNumberOfSteps);

            // Assert
            Assert.AreEqual(1, progressChangedCallCount);
        }

        private class SimpleFileImporter : FileImporterBase
        {
            public override string Name
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override string Category
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override Bitmap Image
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override Type SupportedItemType
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override string FileFilter
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override ProgressChangedDelegate ProgressChanged { protected get; set; }
            public IObservable[] GetAffectedNonTargetObservableInstancesOverride { get; set; }

            protected override IEnumerable<IObservable> GetAffectedNonTargetObservableInstances()
            {
                return GetAffectedNonTargetObservableInstancesOverride ?? base.GetAffectedNonTargetObservableInstances();
            }

            public override bool Import(object targetItem, string filePath)
            {
                throw new NotImplementedException();
            }

            public void TestNotifyProgress(string currentStepName, int currentStep, int totalNumberOfSteps)
            {
                NotifyProgress(currentStepName, currentStep, totalNumberOfSteps);
            }
        }
    }
}